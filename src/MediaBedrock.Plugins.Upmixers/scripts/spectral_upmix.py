import numpy as np
import soundfile as sf
from scipy.signal import butter, lfilter, resample_poly
from typing import Optional, Dict, Union, List
from abc import ABC, abstractmethod
import librosa
from sklearn.decomposition import NMF


def _butter_filter(data: np.ndarray, cutoff: Union[float, List[float]], fs: int,
                   btype: str = 'low', order: int = 4) -> np.ndarray:
    nyq = 0.5 * fs
    if isinstance(cutoff, (list, tuple)):
        normal_cutoff = [c / nyq for c in cutoff]
    else:
        normal_cutoff = cutoff / nyq
    b, a = butter(order, normal_cutoff, btype=btype)
    return lfilter(b, a, data)


def _apply_delay(signal: np.ndarray, fs: int, delay_sec: float) -> np.ndarray:
    delay_samples = int(delay_sec * fs)
    if delay_samples <= 0 or delay_samples >= len(signal):
        return signal
    return np.concatenate((np.zeros(delay_samples), signal[:-delay_samples]))


def _envelope_follower(signal: np.ndarray, fs: int, tau: float = 0.05) -> np.ndarray:
    alpha = np.exp(-1.0 / (fs * tau))
    env = np.zeros(len(signal))
    env[0] = abs(signal[0])
    for i in range(1, len(signal)):
        env[i] = alpha * env[i - 1] + (1 - alpha) * abs(signal[i])
    return env


def _write_wav_int16(path: str, data: np.ndarray, fs: int) -> None:
    max_abs = np.max(np.abs(data))
    if max_abs > 1e-9:
        data = data / max_abs
    int_data = (data * 32767).astype(np.int16)
    sf.write(path, int_data, fs, format='WAV', subtype='PCM_16')


class PipelineStep(ABC):
    @abstractmethod
    def process(self, context: Dict) -> Dict:
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
        context['L'], context['R'] = L, R
        context['M'] = 0.5 * (L + R)
        context['S'] = 0.5 * (L - R)
        return context


class SpectralSeparationStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        audio = context['audio'].T
        fs = context['fs']
        mono = np.mean(audio, axis=0)
        S = librosa.stft(mono)
        H, P = librosa.decompose.hpss(S)
        percussive = librosa.istft(P)
        mag = np.abs(H)
        n_components = int(context['params'].get('nmf_components', 2))
        model = NMF(n_components=n_components, init='random', random_state=0)
        W = model.fit_transform(mag)
        Hn = model.components_
        stems = {'percussive': percussive}
        phase = np.angle(H)
        for i in range(n_components):
            comp_mag = np.outer(W[:, i], Hn[i, :])
            comp_spec = comp_mag * np.exp(1j * phase)
            stems[f'harmonic_{i}'] = librosa.istft(comp_spec)
        context['objects'] = stems
        return context


class ObjectPlacementStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        stems = context.get('objects', {})
        chan = context['channels']
        trajectories = context['params'].get('object_trajectories', {})
        for name, sig in stems.items():
            traj = trajectories.get(name, {})
            for ch_name, gain_env in traj.items():
                # Support constant gains, list envelopes, or linear specs
                if isinstance(gain_env, (int, float)):
                    chan[ch_name] = chan.get(ch_name, 0) + sig * float(gain_env)
                    continue
                if isinstance(gain_env, dict) and 'start' in gain_env and 'end' in gain_env:
                    # linear ramp from start to end
                    start = float(gain_env['start'])
                    end = float(gain_env['end'])
                    env = np.linspace(start, end, sig.shape[0])
                else:
                    # assume list of raw values
                    env = np.array(gain_env)
                    if env.ndim == 0:
                        env = np.full(sig.shape[0], float(env))
                    elif env.shape[0] != sig.shape[0]:
                        env = np.interp(
                            np.arange(sig.shape[0]),
                            np.linspace(0, sig.shape[0] - 1, len(env)),
                            env
                        )
                chan[ch_name] = chan.get(ch_name, 0) + sig * env
        context['channels'] = chan
        return context


class ParametricEQStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        fs = context['fs']
        eq_bands = context['params'].get('eq_bands', [])
        M = context['M']
        for band in eq_bands:
            f0 = float(band['freq']);
            gain_db = float(band['gain_db']);
            Q = float(band['Q'])
            A = 10 ** (gain_db / 40)
            w0 = 2 * np.pi * f0 / fs
            alpha = np.sin(w0) / (2 * Q)
            b0 = 1 + alpha * A;
            b1 = -2 * np.cos(w0);
            b2 = 1 - alpha * A
            a0 = 1 + alpha / A;
            a1 = -2 * np.cos(w0);
            a2 = 1 - alpha / A
            b = np.array([b0, b1, b2]) / a0
            a = np.array([1, a1 / a0, a2 / a0])
            M = lfilter(b, a, M)
        context['M'] = M
        return context


class BandCompressionStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        comp_params = context['params'].get('comp_params', {})
        for band in ['low', 'mid', 'high']:
            cfg = comp_params.get(band)
            if not cfg:
                continue
            x = context[band]
            thresh = 10 ** (cfg['threshold_db'] / 20)
            mag = np.abs(x)
            over = mag > thresh
            mag[over] = thresh + (mag[over] - thresh) / cfg['ratio']
            context[band] = np.sign(x) * mag
        return context


class BandSplitStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        fs = context['fs'];
        p = context['params'];
        M = context['M']
        context['low'] = _butter_filter(M, p['cutoff_low'], fs, 'low')
        context['mid'] = _butter_filter(M, [p['cutoff_low'], p['cutoff_high']], fs, 'bandpass')
        context['high'] = _butter_filter(M, p['cutoff_high'], fs, 'high')
        return context


class EnvelopeStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        fs = context['fs'];
        p = context['params']
        env_mid = _envelope_follower(context['mid'], fs, p['tau_mid'])
        env_high = _envelope_follower(context['high'], fs, p['tau_high'])
        context['env_mid'] = env_mid / (np.max(env_mid) + 1e-9)
        context['env_high'] = env_high / (np.max(env_high) + 1e-9)
        return context


class DynamicGainStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        em = context['env_mid'];
        eh = context['env_high']
        context['dyn_ctr'] = em
        context['dyn_surr'] = 1 - em
        context['dyn_rear'] = 1 - em
        context['dyn_hght'] = eh
        return context


class PsychoMaskingStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        p = context['params'];
        thr = 10 ** (p['mask_threshold_db'] / 20)
        em = context['env_mid']
        mask = np.where(em > thr, thr / (em + 1e-9), 1.0)
        context['dyn_surr'] *= mask
        context['dyn_rear'] *= mask
        context['dyn_hght'] *= mask
        return context


class ChannelGenStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        p = context['params'];
        dc = context['dyn_ctr']
        ds = context['dyn_surr'];
        dr = context['dyn_rear'];
        dh = context['dyn_hght']
        mid = context['mid'];
        high = context['high'];
        S = context['S'];
        low = context['low']
        fs = context['fs'];
        ch: Dict[str, np.ndarray] = {}
        ch['FC'] = mid * dc
        ch['FL'] = (mid * (1 - dc)) + S * (1 - dc) + p['shelf_gain'] * high
        ch['FR'] = (mid * (1 - dc)) - S * (1 - dc) + p['shelf_gain'] * high
        spill = float(p['spill_low_to_lfe']) * low
        remain = low - spill
        ch['LFE'] = spill
        ch['FL'] += 0.5 * remain;
        ch['FR'] += 0.5 * remain
        ch['SL'] = ds * _apply_delay(mid + 0.4 * S, fs, p['delay_surround'])
        ch['SR'] = ds * _apply_delay(mid - 0.4 * S, fs, p['delay_surround'])
        ch['BL'] = dr * _apply_delay(S, fs, p['delay_rear'])
        ch['BR'] = dr * _apply_delay(-S, fs, p['delay_rear'])
        ch['FHL'] = dh * _apply_delay(high + 0.2 * S, fs, p['delay_height'])
        ch['FHR'] = dh * _apply_delay(high - 0.2 * S, fs, p['delay_height'])
        ch['RHL'] = dh * _apply_delay(S, fs, p['delay_height'] * 1.1)
        ch['RHR'] = dh * _apply_delay(-S, fs, p['delay_height'] * 1.1)
        context['channels'] = ch
        return context


class SpilloverStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        p = context['params'];
        ch = context['channels']
        cf = p['spill_center_to_front'];
        center = ch['FC'];
        spill_cf = cf * center
        ch['FC'] -= spill_cf;
        ch['FL'] += spill_cf;
        ch['FR'] += spill_cf
        fs_p = p['spill_front_to_side'];
        avg_f = 0.5 * (ch['FL'] + ch['FR']);
        spill_fs = fs_p * avg_f
        ch['FL'] -= spill_fs;
        ch['FR'] -= spill_fs;
        ch['SL'] += spill_fs;
        ch['SR'] += spill_fs
        ss = p['spill_side_to_surround'];
        avg_s = 0.5 * (ch['SL'] + ch['SR']);
        spill_ss = ss * avg_s
        ch['SL'] -= spill_ss;
        ch['SR'] -= spill_ss;
        ch['BL'] += spill_ss;
        ch['BR'] += spill_ss
        bh = p['spill_base_to_height'];
        bed_keys = ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR', 'BL', 'BR']
        bed_avg = sum(ch[k] for k in bed_keys) / len(bed_keys);
        spill_bh = bh * bed_avg
        for k in bed_keys: ch[k] -= spill_bh / len(bed_keys)
        for k in ['FHL', 'FHR', 'RHL', 'RHR']: ch[k] += spill_bh
        context['channels'] = ch;
        return context


class DistanceAttenuationStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        dm = context['params']['distance_map'];
        exp = context['params']['distance_exponent'];
        ch = context['channels']
        for n, sig in ch.items(): ch[n] = sig / (float(dm.get(n, 1.0)) ** exp + 1e-9)
        context['channels'] = ch;
        return context


class LoudnessNormalizationStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        out = context['out'];
        tgt = context['params']['target_loudness_lufs']
        loud = 10 * np.log10(np.mean(out ** 2) + 1e-12);
        factor = 10 ** ((tgt - loud) / 20)
        context['out'] = out * factor
        return context


class StackStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        layout = context['params']['layout']
        maps = {
            '5.1': ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR'],
            '7.1': ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR', 'BL', 'BR'],
            '5.1.2': ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR', 'FHL', 'FHR'],
            '5.1.4': ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR', 'FHL', 'FHR', 'RHL', 'RHR'],
            '7.1.2': ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR', 'BL', 'BR', 'FHL', 'FHR'],
            '7.1.4': ['FL', 'FR', 'FC', 'LFE', 'SL', 'SR', 'BL', 'BR', 'FHL', 'FHR', 'RHL', 'RHR'],
        }
        order = maps[layout];
        ch = context['channels'];
        context['out'] = np.stack([ch[k] for k in order], axis=1)
        return context


class WriteStep(PipelineStep):
    def process(self, context: Dict) -> Dict:
        out, fs, path = context['out'], context['fs'], context['output_path']
        bd = context['params']['bit_depth']
        if bd == 'PCM_16':
            _write_wav_int16(path, out, fs)
        else:
            sf.write(path, out, fs, format='WAV', subtype='PCM_24')
        print(f"Wrote {context['params']['layout']} upmix to {path} @ {fs}Hz ({bd})")
        return context


class UpmixPipeline:
    def __init__(self, steps: List[PipelineStep]): self.steps = steps

    def run(self, context: Dict) -> Dict:
        for s in self.steps: context = s.process(context)
        return context


def upmix(
        input_path: str,
        output_path: str,
        parameters: Optional[Dict] = None
) -> None:
    p = {
        'nmf_components': 10,
        'object_trajectories': {
            # Percussive: ping-pong L ↔ R on side surrounds
            'percussive': {
                'SL': {'start': 0.2, 'end': 0.8},
                'SR': {'start': 0.8, 'end': 0.2},
            },
            # Harmonic_0: tight center
            'harmonic_0': {'FC': 1.0},
            # Harmonic_1: front L→R sweep
            'harmonic_1': {
                'FL': {'start': 0.8, 'end': 0.2},
                'FR': {'start': 0.2, 'end': 0.8},
            },
            # Harmonic_2: front heights fade-in
            'harmonic_2': {
                'FHL': {'start': 0.0, 'end': 0.6},
                'FHR': {'start': 0.0, 'end': 0.6},
            },
            # Harmonic_3: side surrounds static
            'harmonic_3': {'SL': 0.7, 'SR': 0.7},
            # Harmonic_4: rear surrounds static
            'harmonic_4': {'BL': 0.7, 'BR': 0.7},
            # Harmonic_5: rear heights fade
            'harmonic_5': {
                'RHL': {'start': 0.0, 'end': 0.5},
                'RHR': {'start': 0.0, 'end': 0.5},
            },
            # Harmonic_6–9: spread evenly around overhead
            'harmonic_6': {'FHL': 0.5, 'FHR': 0.0, 'RHL': 0.0, 'RHR': 0.5},
            'harmonic_7': {'FHL': 0.0, 'FHR': 0.5, 'RHL': 0.5, 'RHR': 0.0},
            'harmonic_8': {'FHL': 0.3, 'FHR': 0.3, 'RHL': 0.3, 'RHR': 0.3},
            'harmonic_9': {'FHL': 0.6, 'FHR': 0.6, 'RHL': 0.2, 'RHR': 0.2},
        },
        'eq_bands': [
            {'freq': 200.0, 'gain_db': -3.0, 'Q': 0.7},
            {'freq': 1000.0, 'gain_db': 1.0, 'Q': 1.0},
            {'freq': 6000.0, 'gain_db': 2.0, 'Q': 1.2},
        ],
        'cutoff_low': 100.0,
        'cutoff_high': 10000.0,
        'shelf_gain': 0.3,
        'comp_params': {},
        'spill_center_to_front': 0.15,
        'spill_front_to_side': 0.75,
        'spill_side_to_surround': 0.55,
        'spill_base_to_height': 0.50,
        'spill_low_to_lfe': 0.5,
        'delay_surround': 0.020,
        'delay_rear': 0.030,
        'delay_height': 0.025,
        'tau_mid': 0.02,
        'tau_high': 0.1,
        'mask_threshold_db': -20.0,
        'distance_map': {},
        'distance_exponent': 0,
        'target_loudness_lufs': -18.0,
        'layout': '7.1.4',
        'target_sr': 48000,
        'bit_depth': 'PCM_24',
    }

    if parameters: p.update(parameters)
    audio, fs = sf.read(input_path);
    if audio.ndim != 2 or audio.shape[1] != 2: raise ValueError("Input must be stereo")
    context = {'audio': audio, 'fs': fs, 'params': p, 'output_path': output_path, 'channels': {}}
    steps = [
        ResampleStep(), DecomposeStep(), SpectralSeparationStep(), ObjectPlacementStep(),
        ParametricEQStep(), BandSplitStep(), BandCompressionStep(), EnvelopeStep(),
        DynamicGainStep(), PsychoMaskingStep(), ChannelGenStep(), SpilloverStep(),
        DistanceAttenuationStep(), StackStep(), LoudnessNormalizationStep(), WriteStep()
    ]
    UpmixPipeline(steps).run(context)


if __name__ == '__main__':
    import argparse

    parser = argparse.ArgumentParser(description='Stereo to multichannel upmixer')
    parser.add_argument('input')
    parser.add_argument('output')
    parser.add_argument('--param', action='append', nargs=2, metavar=('KEY', 'VALUE'))
    args = parser.parse_args()
    params = {}
    if args.param:
        for k, v in args.param:
            try:
                params[k] = float(v)
            except:
                params[k] = v
    upmix(args.input, args.output, parameters=params)

# Deps: numpy, scipy, soundfile, librosa, scikit-learn
