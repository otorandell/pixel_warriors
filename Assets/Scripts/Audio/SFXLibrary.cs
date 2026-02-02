using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public static class SFXLibrary
    {
        private static Dictionary<SFXType, AudioClip> _clips;
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;
            _clips = new Dictionary<SFXType, AudioClip>();

            // UI
            _clips[SFXType.UIClick] = GenerateSquareTone("UIClick", 800f, 0.05f);
            _clips[SFXType.UIConfirm] = GenerateTwoTone("UIConfirm", 600f, 900f, 0.12f, WaveType.Square);
            _clips[SFXType.UICancel] = GenerateTwoTone("UICancel", 500f, 300f, 0.12f, WaveType.Square);
            _clips[SFXType.UIPopup] = GenerateSweep("UIPopup", 400f, 800f, 0.08f, WaveType.Sine);
            _clips[SFXType.TabClick] = GenerateSquareTone("TabClick", 1000f, 0.03f);

            // Combat
            _clips[SFXType.PhysicalHit] = GenerateImpact("PhysicalHit", 120f, 0.10f);
            _clips[SFXType.MagicalHit] = GenerateMagicHit("MagicalHit", 1200f, 400f, 0.15f);
            _clips[SFXType.CriticalHit] = GenerateImpact("CriticalHit", 200f, 0.18f);
            _clips[SFXType.Miss] = GenerateSquareTone("Miss", 1200f, 0.08f);
            _clips[SFXType.Dodge] = GenerateSweep("Dodge", 600f, 1200f, 0.10f, WaveType.Sine);
            _clips[SFXType.Heal] = GenerateArpeggio("Heal", new[] { 523f, 659f, 784f }, 0.065f, WaveType.Sine);
            _clips[SFXType.Defeated] = GenerateDefeatSound("Defeated", 400f, 80f, 0.30f);

            // Battle Flow
            _clips[SFXType.BattleStart] = GenerateArpeggio("BattleStart", new[] { 262f, 330f, 392f, 523f }, 0.10f, WaveType.Square);
            _clips[SFXType.Victory] = GenerateArpeggio("Victory", new[] { 262f, 330f, 392f, 523f, 659f, 784f }, 0.10f, WaveType.Sine);
            _clips[SFXType.BattleDefeat] = GenerateArpeggio("BattleDefeat", new[] { 262f, 233f, 196f, 165f }, 0.125f, WaveType.Square);
            _clips[SFXType.TurnNotify] = GenerateSineTone("TurnNotify", 660f, 0.06f);

            _initialized = true;
        }

        public static AudioClip Get(SFXType type)
        {
            if (_clips != null && _clips.TryGetValue(type, out AudioClip clip))
                return clip;
            return null;
        }

        // --- Waveform Primitives ---

        private enum WaveType { Square, Sine }

        private static float SampleWave(WaveType type, float phase)
        {
            return type switch
            {
                WaveType.Square => phase % 1f < 0.5f ? 1f : -1f,
                WaveType.Sine => Mathf.Sin(phase * 2f * Mathf.PI),
                _ => 0f
            };
        }

        private static float WhiteNoise()
        {
            return Random.Range(-1f, 1f);
        }

        private static float Envelope(float t, float duration, float attackTime = 0.005f)
        {
            float decayStart = duration * 0.3f;
            if (t < attackTime) return t / attackTime;
            if (t > decayStart) return Mathf.Max(0f, 1f - (t - decayStart) / (duration - decayStart));
            return 1f;
        }

        private static AudioClip CreateClip(string name, float[] samples)
        {
            AudioClip clip = AudioClip.Create(name, samples.Length, 1, AudioConfig.SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        // --- Generators ---

        private static AudioClip GenerateSquareTone(string name, float freq, float duration)
        {
            int sampleCount = (int)(duration * AudioConfig.SampleRate);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)AudioConfig.SampleRate;
                float phase = t * freq;
                float env = Envelope(t, duration);
                samples[i] = SampleWave(WaveType.Square, phase) * env * 0.4f;
            }

            return CreateClip(name, samples);
        }

        private static AudioClip GenerateSineTone(string name, float freq, float duration)
        {
            int sampleCount = (int)(duration * AudioConfig.SampleRate);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)AudioConfig.SampleRate;
                float phase = t * freq;
                float env = Envelope(t, duration);
                samples[i] = SampleWave(WaveType.Sine, phase) * env * 0.5f;
            }

            return CreateClip(name, samples);
        }

        private static AudioClip GenerateTwoTone(string name, float freq1, float freq2, float duration, WaveType waveType)
        {
            int sampleCount = (int)(duration * AudioConfig.SampleRate);
            float[] samples = new float[sampleCount];
            float halfDuration = duration * 0.5f;

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)AudioConfig.SampleRate;
                float freq = t < halfDuration ? freq1 : freq2;
                float phase = t * freq;
                float env = Envelope(t, duration);
                samples[i] = SampleWave(waveType, phase) * env * 0.4f;
            }

            return CreateClip(name, samples);
        }

        private static AudioClip GenerateSweep(string name, float freqStart, float freqEnd, float duration, WaveType waveType)
        {
            int sampleCount = (int)(duration * AudioConfig.SampleRate);
            float[] samples = new float[sampleCount];
            float phase = 0f;

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)AudioConfig.SampleRate;
                float progress = t / duration;
                float freq = Mathf.Lerp(freqStart, freqEnd, progress);
                phase += freq / AudioConfig.SampleRate;
                float env = Envelope(t, duration);
                samples[i] = SampleWave(waveType, phase) * env * 0.4f;
            }

            return CreateClip(name, samples);
        }

        private static AudioClip GenerateArpeggio(string name, float[] frequencies, float noteLength, WaveType waveType)
        {
            float totalDuration = frequencies.Length * noteLength;
            int sampleCount = (int)(totalDuration * AudioConfig.SampleRate);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)AudioConfig.SampleRate;
                int noteIndex = Mathf.Min((int)(t / noteLength), frequencies.Length - 1);
                float noteT = t - noteIndex * noteLength;
                float phase = noteT * frequencies[noteIndex];

                // Per-note envelope
                float noteEnv = Envelope(noteT, noteLength, 0.002f);
                // Global fade-in on first note, fade-out on last note
                float globalEnv = 1f;
                if (t < 0.01f) globalEnv = t / 0.01f;
                if (t > totalDuration - 0.03f) globalEnv = (totalDuration - t) / 0.03f;

                samples[i] = SampleWave(waveType, phase) * noteEnv * Mathf.Max(0f, globalEnv) * 0.35f;
            }

            return CreateClip(name, samples);
        }

        private static AudioClip GenerateImpact(string name, float lowFreq, float duration)
        {
            int sampleCount = (int)(duration * AudioConfig.SampleRate);
            float[] samples = new float[sampleCount];
            float noiseDuration = duration * 0.4f;

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)AudioConfig.SampleRate;
                float env = Envelope(t, duration, 0.001f);

                // Low square wave thud
                float phase = t * lowFreq;
                float square = SampleWave(WaveType.Square, phase) * 0.3f;

                // Noise burst (decays faster)
                float noise = 0f;
                if (t < noiseDuration)
                {
                    float noiseEnv = 1f - (t / noiseDuration);
                    noise = WhiteNoise() * noiseEnv * 0.5f;
                }

                samples[i] = (square + noise) * env;
            }

            return CreateClip(name, samples);
        }

        private static AudioClip GenerateMagicHit(string name, float freqStart, float freqEnd, float duration)
        {
            int sampleCount = (int)(duration * AudioConfig.SampleRate);
            float[] samples = new float[sampleCount];
            float phase = 0f;
            float noiseTail = duration * 0.3f;

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)AudioConfig.SampleRate;
                float progress = t / duration;
                float freq = Mathf.Lerp(freqStart, freqEnd, progress);
                phase += freq / AudioConfig.SampleRate;
                float env = Envelope(t, duration, 0.002f);

                // Descending sine sweep
                float sweep = SampleWave(WaveType.Sine, phase) * 0.4f;

                // Noise tail at the end
                float noise = 0f;
                float tailStart = duration - noiseTail;
                if (t > tailStart)
                {
                    float tailProgress = (t - tailStart) / noiseTail;
                    float tailEnv = Mathf.Sin(tailProgress * Mathf.PI);
                    noise = WhiteNoise() * tailEnv * 0.15f;
                }

                samples[i] = (sweep + noise) * env;
            }

            return CreateClip(name, samples);
        }

        private static AudioClip GenerateDefeatSound(string name, float freqStart, float freqEnd, float duration)
        {
            int sampleCount = (int)(duration * AudioConfig.SampleRate);
            float[] samples = new float[sampleCount];
            float phase = 0f;

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)AudioConfig.SampleRate;
                float progress = t / duration;
                float freq = Mathf.Lerp(freqStart, freqEnd, progress);
                phase += freq / AudioConfig.SampleRate;

                // Long fade-out envelope
                float env = 1f - progress;
                env *= env; // Quadratic decay

                // Descending square + fading noise
                float square = SampleWave(WaveType.Square, phase) * 0.3f;
                float noise = WhiteNoise() * (1f - progress) * 0.2f;

                samples[i] = (square + noise) * env;
            }

            return CreateClip(name, samples);
        }
    }
}
