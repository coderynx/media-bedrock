import numpy as np
import soundfile as sf
from scipy.signal import butter, lfilter, resample_poly
from typing import Optional, Dict, Union, List
from abc import ABC, abstractmethod


def _butter_filter(
        data: np.ndarray,
        cutoff: Union[float, List[float]],
        fs: int,
        btype: str = 'low',
        order: int = 4
) -> np.ndarray:
    """
    Apply a Butterworth filter to the input signal.
    """
    nyq = 0.5 * fs
    if isinstance(cutoff, (list, tuple)):
        normal_cutoff = [c / nyq for c in cutoff]
    else:
        normal_cutoff = cutoff / nyq

    b, a = butter(order, normal_cutoff, btype=btype)
    return lfilter(b, a, data)


def _apply_delay(
        signal: np.ndarray,
        fs: int,
        delay_sec: float
) -> np.ndarray:
    """
    Delay the signal by a specified number of seconds (padding with zeros).
    """
    delay_samples = int(delay_sec * fs)
    if delay_samples <= 0 or delay_samples >= len(signal):
        return signal
    return np.concatenate((np.zeros(delay_samples), signal[:-delay_samples]))


def _envelope_follower(
        signal: np.ndarray,
        fs: int,
        tau: float = 0.05
) -> np.ndarray:
    """
    Simple exponential envelope follower.
    """
    alpha = np.exp(-1.0 / (fs * tau))
    env = np.zeros(len(signal))
    env[0] = abs(signal[0])
    for i in range(1, len(signal)):
        env[i] = alpha * env[i - 1] + (1 - alpha) * abs(signal[i])
    return env


def _write_wav_int16(
        path: str,
        data: np.ndarray,
        fs: int
) -> None:
    """
    Normalize and write audio data as 16-bit PCM WAV.
    """
    max_abs = np.max(np.abs(data))
    if max_abs > 1e-9:
        data = data / max_abs

    int_data = (data * 32767).astype(np.int16)
    sf.write(path, int_data, fs, format='WAV', subtype='PCM_16')


class PipelineStep(ABC):
    @abstractmethod
    def process(self, context: Dict) -> Dict:
        """Process the context and return updated context."""
        pass


class ResampleStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        audio, fs = context['audio'], context['fs']
        target_sr = int(context['params']['target_sr'])
        if fs != target_sr:
            audio = resample_poly(audio, target_sr, fs, axis=0)
            context['audio'] = audio
            context['fs'] = target_sr
        return context


class DecomposeStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        audio = context['audio']
        L = audio[:, 0]
        R = audio[:, 1]
        context['L'] = L
        context['R'] = R
        context['M'] = 0.5 * (L + R)
        context['S'] = 0.5 * (L - R)
        return context


class ParametricEQStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        fs = context['fs']
        eq_bands = context['params'].get('eq_bands', [])
        M = context['M']
        for band in eq_bands:
            f0 = float(band.get('freq', 1000))
            gain_db = float(band.get('gain_db', 0))
            Q = float(band.get('Q', 1))
            A = 10 ** (gain_db / 40)
            w0 = 2 * np.pi * f0 / fs
            alpha = np.sin(w0) / (2 * Q)

            b0 = 1 + alpha * A
            b1 = -2 * np.cos(w0)
            b2 = 1 - alpha * A
            a0 = 1 + alpha / A
            a1 = -2 * np.cos(w0)
            a2 = 1 - alpha / A

            b = np.array([b0, b1, b2]) / a0
            a = np.array([1, a1 / a0, a2 / a0])
            M = lfilter(b, a, M)

        context['M'] = M
        return context


class BandCompressionStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        comp_params = context['params'].get('comp_params', {})
        for band_name in ['low', 'mid', 'high']:
            cfg = comp_params.get(band_name)
            if not cfg:
                continue
            x = context[band_name]
            threshold = 10 ** (cfg['threshold_db'] / 20)
            mag = np.abs(x)
            over = mag > threshold
            ratio = cfg['ratio']
            mag[over] = threshold + (mag[over] - threshold) / ratio
            context[band_name] = np.sign(x) * mag
        return context


class BandSplitStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        fs = context['fs']
        p = context['params']
        M = context['M']

        context['low'] = _butter_filter(M, p['cutoff_low'], fs, 'low')
        context['mid'] = _butter_filter(
            M, [p['cutoff_low'], p['cutoff_high']], fs, 'bandpass'
        )
        context['high'] = _butter_filter(M, p['cutoff_high'], fs, 'high')
        return context


class EnvelopeStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        fs = context['fs']
        p = context['params']

        env_mid = _envelope_follower(context['mid'], fs, p['tau_mid'])
        env_high = _envelope_follower(context['high'], fs, p['tau_high'])

        context['env_mid'] = env_mid / (np.max(env_mid) + 1e-9)
        context['env_high'] = env_high / (np.max(env_high) + 1e-9)
        return context


class DynamicGainStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        em = context['env_mid']
        eh = context['env_high']

        context['dyn_ctr'] = em
        context['dyn_surr'] = 1 - em
        context['dyn_rear'] = 1 - em
        context['dyn_hght'] = eh
        return context


class ChannelGenStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        p = context['params']
        dc = context['dyn_ctr']
        ds = context['dyn_surr']
        dr = context['dyn_rear']
        dh = context['dyn_hght']

        mid = context['mid']
        high = context['high']
        S = context['S']
        low = context['low']
        fs = context['fs']

        ch: Dict[str, np.ndarray] = {}
        # Center and Front
        ch['FC'] = mid * dc
        ch['FL'] = (mid * (1 - dc)) + S * (1 - dc) + p['shelf_gain'] * high
        ch['FR'] = (mid * (1 - dc)) - S * (1 - dc) + p['shelf_gain'] * high

        # Low-frequency reroute to LFE (true reroute)
        spill = float(p.get('spill_low_to_lfe', 1.0)) * low
        remain = low - spill
        ch['LFE'] = spill
        ch['FL'] += 0.5 * remain
        ch['FR'] += 0.5 * remain

        # Surround and Rear
        ch['SL'] = ds * _apply_delay(mid + 0.4 * S, fs, p['delay_surround'])
        ch['SR'] = ds * _apply_delay(mid - 0.4 * S, fs, p['delay_surround'])
        ch['BL'] = dr * _apply_delay(S, fs, p['delay_rear'])
        ch['BR'] = dr * _apply_delay(-S, fs, p['delay_rear'])

        # Heights
        ch['FHL'] = dh * _apply_delay(high + 0.2 * S, fs, p['delay_height'])
        ch['FHR'] = dh * _apply_delay(high - 0.2 * S, fs, p['delay_height'])
        ch['RHL'] = dh * _apply_delay(S, fs, p['delay_height'] * 1.1)
        ch['RHR'] = dh * _apply_delay(-S, fs, p['delay_height'] * 1.1)

        context['channels'] = ch
        return context


class SpilloverStep(PipelineStep):
    """
    True rerouting spillover: moves energy from one ring to the next.
    """

    def process(self, context: Dict) -> Dict:
        p = context['params']
        ch = context['channels']

        # Center -> Front
        cf = float(p.get('spill_center_to_front', 0.0))
        center = ch['FC']
        spill_cf = cf * center
        ch['FC'] -= spill_cf
        ch['FL'] += spill_cf
        ch['FR'] += spill_cf

        # Front -> Side
        fs_param = float(p.get('spill_front_to_side', 0.0))
        avg_front = 0.5 * (ch['FL'] + ch['FR'])
        spill_fs = fs_param * avg_front
        ch['FL'] -= spill_fs
        ch['FR'] -= spill_fs
        ch['SL'] += spill_fs
        ch['SR'] += spill_fs

        # Side -> Rear
        ss = float(p.get('spill_side_to_surround', 0.0))
        avg_side = 0.5 * (ch['SL'] + ch['SR'])
        spill_ss = ss * avg_side
        ch['SL'] -= spill_ss
        ch['SR'] -= spill_ss
        ch['BL'] += spill_ss
        ch['BR'] += spill_ss

        # Bed -> Height
        bh = float(p.get('spill_base_to_height', 0.0))
        bed_keys = ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR', 'BL', 'BR']
        bed_avg = sum(ch[k] for k in bed_keys) / len(bed_keys)
        spill_bh = bh * bed_avg
        for k in bed_keys:
            ch[k] -= spill_bh / len(bed_keys)
        for k in ['FHL', 'FHR', 'RHL', 'RHR']:
            ch[k] += spill_bh

        context['channels'] = ch
        return context


class DistanceAttenuationStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        dist_map = context['params'].get('distance_map', {})
        exponent = float(context['params'].get('distance_exponent', 1.0))
        ch = context['channels']
        for name, sig in ch.items():
            d = float(dist_map.get(name, 1.0))
            ch[name] = sig / (d ** exponent + 1e-9)
        context['channels'] = ch
        return context


class LoudnessNormalizationStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        out = context['out']
        target = float(context['params'].get('target_loudness_lufs', -23.0))
        power = np.mean(out ** 2)
        loudness = 10 * np.log10(power + 1e-12)
        gain_db = target - loudness
        factor = 10 ** (gain_db / 20)
        context['out'] = out * factor
        print(f"Loudness: {loudness:.2f} dBFS -> {target} dBFS (gain {gain_db:.2f} dB)")
        return context


class StackStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        layout = context['params']['layout']
        layouts = {
            '5.1': ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR'],
            '7.1': ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR', 'BL', 'BR'],
            '5.1.2': ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR', 'FHL', 'FHR'],
            '5.1.4': ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR', 'FHL', 'FHR', 'RHL', 'RHR'],
            '7.1.2': ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR', 'BL', 'BR', 'FHL', 'FHR'],
            '7.1.4': ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR', 'BL', 'BR', 'FHL', 'FHR', 'RHL', 'RHR'],
        }
        order = layouts.get(layout, [])
        ch = context['channels']
        context['out'] = np.stack([ch[k] for k in order], axis=1)
        return context


class WriteStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        out = context['out']
        fs = context['fs']
        path = context['output_path']
        bd = context['params']['bit_depth']
        if bd == 'PCM_16':
            _write_wav_int16(path, out, fs)
        else:
            sf.write(path, out, fs, format='WAV', subtype='PCM_24')
        print(f"Wrote {context['params']['layout']} upmix to {path} @ {fs}Hz ({bd})")
        return context


class UpmixPipeline:
    def __init__(self, steps: List[PipelineStep]):
        self.steps = steps

    def run(self, context: Dict) -> Dict:
        for step in self.steps:
            context = step.process(context)
        return context


def upmix_stereo_to_multichannel(
        input_path: str,
        output_path: str,
        parameters: Optional[Dict] = None
) -> None:
    p: Dict[str, Union[int, float, str]] = {
        'layout': '7.1.4',
        'target_sr': 48000,
        'bit_depth': 'PCM_16',
        'cutoff_low': 100,
        'cutoff_high': 5000,
        'delay_surround': 0.01,
        'delay_rear': 0.02,
        'delay_height': 0.015,
        'tau_mid': 0.2,
        'tau_high': 0.2,
        'eq_bands': [],
        'comp_params': {},
        'distance_map': {},
        'distance_exponent': 1.0,
        'spill_low_to_lfe': 1.0,
        'spill_center_to_front': 0.0,
        'spill_front_to_side': 0.5,
        'spill_side_to_surround': 0.3,
        'spill_base_to_height': 0.4,
        'target_loudness_lufs': -18.0
    }

    if parameters:
        p.update(parameters)

    audio, fs = sf.read(input_path)
    if audio.ndim != 2 or audio.shape[1] != 2:
        raise ValueError("Input must be stereo (2 channels)")

    context = {
        'audio': audio,
        'fs': fs,
        'params': p,
        'output_path': output_path
    }

    pipeline_steps: List[PipelineStep] = [
        ResampleStep(),
        DecomposeStep(),
        ParametricEQStep(),
        BandSplitStep(),
        BandCompressionStep(),
        EnvelopeStep(),
        DynamicGainStep(),
        ChannelGenStep(),
        SpilloverStep(),
        DistanceAttenuationStep(),
        StackStep(),
        LoudnessNormalizationStep(),
        WriteStep()
    ]

    pipeline = UpmixPipeline(pipeline_steps)
    pipeline.run(context)


if __name__ == '__main__':
    import argparse

    parser = argparse.ArgumentParser(
        description='Stereo to multichannel upmixer'
    )
    parser.add_argument('input', help='Path to stereo input WAV')
    parser.add_argument('output', help='Path for multichannel output WAV')
    parser.add_argument(
        '--param', action='append', nargs=2,
        metavar=('KEY', 'VALUE'), help='Override default parameter'
    )
    args = parser.parse_args()

    params: Dict[str, Union[int, float, str]] = {}
    if args.param:
        for key, val in args.param:
            try:
                params[key] = float(val)
            except ValueError:
                params[key] = val

    upmix_stereo_to_multichannel(
        args.input,
        args.output,
        parameters=params
    )

# Deps: numpy, scipy, soundfile
