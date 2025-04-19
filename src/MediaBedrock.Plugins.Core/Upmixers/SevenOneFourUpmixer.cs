using NAudio.Dsp;
using NAudio.Wave;

namespace MediaBedrock.Plugins.Core.Upmixers;

public sealed class SevenOneFourUpmixer
{
    public void Upmix(string inputFilePath, string outputFilePath)
    {
        const uint outputChannels = 12;

        using var reader = new AudioFileReader(inputFilePath);

        var sampleRate = reader.WaveFormat.SampleRate;
        var inputChannels = reader.WaveFormat.Channels;
        var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, (int)outputChannels);

        var buffer = new float[sampleRate * inputChannels];
        var outBuffer = new float[buffer.Length * outputChannels];

        const uint lfeCutOffFrequency = 120;
        const float lfeQFactor = 0.707f;

        var lfeFilter = BiQuadFilter.LowPassFilter(sampleRate, lfeCutOffFrequency, lfeQFactor);
        var highPassFilterLeft = BiQuadFilter.HighPassFilter(sampleRate, lfeCutOffFrequency, lfeQFactor);
        var highPassFilterRight = BiQuadFilter.HighPassFilter(sampleRate, lfeCutOffFrequency, lfeQFactor);

        const float heightShelfGain = -0.50f;
        const float heightShelfCutOffFrequency = 18600.0f;
        const float heightShelfSlope = 1.0f;

        var heightShelfFilter = BiQuadFilter.HighShelf(
            sampleRate,
            heightShelfCutOffFrequency,
            heightShelfSlope,
            heightShelfGain);

        var inverseHeightShelfFilter = BiQuadFilter.LowShelf(
            sampleRate,
            heightShelfCutOffFrequency,
            heightShelfSlope,
            -heightShelfGain);

        const float rearShelfGain = -0.50f;
        const float rearShelfCutOffFrequency = 18600.0f;
        const float rearShelfSlope = 1.0f;

        var surroundShelfFilter = BiQuadFilter.HighShelf(
            sampleRate,
            rearShelfCutOffFrequency,
            rearShelfSlope,
            rearShelfGain);

        const float surroundShelfGain = -0.50f;
        const float surroundShelfCutOffFrequency = 18600.0f;
        const float surroundShelfSlope = 1.0f;

        var rearSurroundShelfFilter = BiQuadFilter.HighShelf(
            sampleRate,
            surroundShelfCutOffFrequency,
            surroundShelfSlope,
            surroundShelfGain);

        // Weights to decompose the stereo signal.
        const float midWeight = 0.2f;
        const float frontWeight = 0.4f;
        const float rearWeight = 0.3f;

        // Gains for each channel.
        const float frontGain = 1.0f;
        const float heightFrontGain = 0.5f;
        const float centerGain = 1.0f;
        const float lfeGain = 1.0f;
        const float rearGain = 1.5f;
        const float heightRearGain = 1.5f;
        const float surroundGain = 1.5f;

        using var writer = new WaveFileWriter(outputFilePath, waveFormat);

        int samplesRead;
        while ((samplesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            for (var i = 0; i < samplesRead / reader.WaveFormat.Channels; i++)
            {
                var left = buffer[i * 2];
                var right = buffer[i * 2 + 1];

                var highPassedLeft = highPassFilterLeft.Transform(left);
                var highPassedRight = highPassFilterRight.Transform(right);

                // Mid-Side Processing
                var mid = highPassedLeft * midWeight + highPassedRight * midWeight;
                var side = (highPassedLeft - highPassedRight) / 2;

                // Side Processing
                var front = side * frontWeight;
                var sideSurround = side * rearWeight;
                var rearSurround = side - (front + sideSurround);

                // Front Left (L)
                var frontLeft = inverseHeightShelfFilter.Transform(front) * frontGain;
                outBuffer[GetChannelIndex(i, SevenOneFourChannel.FrontLeft)] = frontLeft;

                // Front Right (R)
                var frontRight = inverseHeightShelfFilter.Transform(-front) * frontGain;
                outBuffer[GetChannelIndex(i, SevenOneFourChannel.FrontRight)] = frontRight;

                // Center (C)
                var center = mid * centerGain;
                outBuffer[GetChannelIndex(i, SevenOneFourChannel.Center)] = center;

                // Low-Frequency Effects (LFE)
                var lowFrequencyEffects = (lfeFilter.Transform(left) + lfeFilter.Transform(right)) / 2 * lfeGain;
                outBuffer[GetChannelIndex(i, SevenOneFourChannel.LowFrequencyEffects)] = lowFrequencyEffects;

                // Left Side (LS)
                var sideLeft = surroundShelfFilter.Transform(sideSurround) * rearGain;
                outBuffer[GetChannelIndex(i, SevenOneFourChannel.RearLeft)] = sideLeft;

                // Right Side (RS)
                var sideRight = surroundShelfFilter.Transform(-sideSurround) * rearGain;
                outBuffer[GetChannelIndex(i, SevenOneFourChannel.RearRight)] = sideRight;

                // Left Surround (LSS)
                var leftSurround = rearSurroundShelfFilter.Transform(rearSurround) * surroundGain;
                outBuffer[GetChannelIndex(i, SevenOneFourChannel.RearSurroundLeft)] = leftSurround;

                // Right Surround (RSS)
                var rightSurround = rearSurroundShelfFilter.Transform(-rearSurround) * surroundGain;
                outBuffer[GetChannelIndex(i, SevenOneFourChannel.RearSurroundRight)] = rightSurround;

                // Height Front Left (HFL)
                var frontHeightLeft = heightShelfFilter.Transform(front) * heightFrontGain;
                outBuffer[GetChannelIndex(i, SevenOneFourChannel.FrontHeightLeft)] = frontHeightLeft;

                // Height Front Right (HFR)
                var frontRightHeight = heightShelfFilter.Transform(-front) * heightFrontGain;
                outBuffer[GetChannelIndex(i, SevenOneFourChannel.FrontHeightRight)] = frontRightHeight;

                // Height Rear Left (HRL)
                var heightRearLeft = heightShelfFilter.Transform(rearSurround) * heightRearGain;
                outBuffer[GetChannelIndex(i, SevenOneFourChannel.HeightRearLeft)] = heightRearLeft;

                // Height Rear Right (HRR)
                var heightRearRight = heightShelfFilter.Transform(-rearSurround) * heightRearGain;
                outBuffer[GetChannelIndex(i, SevenOneFourChannel.HeightRearRight)] = heightRearRight;
            }

            var writeCount = (int)(samplesRead / reader.WaveFormat.Channels * outputChannels);
            writer.WriteSamples(outBuffer, 0, writeCount);
        }
    }

    private static int GetChannelIndex(int index, SevenOneFourChannel channel)
    {
        const int channelCount = 12;
        return index * channelCount + (int)channel;
    }
}