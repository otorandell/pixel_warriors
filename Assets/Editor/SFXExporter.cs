using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PixelWarriors
{
    public static class SFXExporter
    {
        [MenuItem("Tools/Export All SFX to WAV")]
        private static void ExportAllSFX()
        {
            string outputDir = Path.Combine(Application.dataPath, "..", "ExportedSFX");
            Directory.CreateDirectory(outputDir);

            SFXLibrary.Initialize();

            int exported = 0;
            foreach (SFXType type in Enum.GetValues(typeof(SFXType)))
            {
                AudioClip clip = SFXLibrary.Get(type);
                if (clip == null)
                {
                    Debug.LogWarning($"[SFXExporter] No clip for {type}, skipping.");
                    continue;
                }

                string path = Path.Combine(outputDir, $"{type}.wav");
                WriteWav(path, clip);
                exported++;
            }

            Debug.Log($"[SFXExporter] Exported {exported} SFX files to: {outputDir}");
            EditorUtility.RevealInFinder(outputDir);
        }

        private static void WriteWav(string path, AudioClip clip)
        {
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            using BinaryWriter writer = new BinaryWriter(File.Create(path));

            int sampleCount = samples.Length;
            int sampleRate = clip.frequency;
            short channels = (short)clip.channels;
            short bitsPerSample = 16;
            int byteRate = sampleRate * channels * bitsPerSample / 8;
            short blockAlign = (short)(channels * bitsPerSample / 8);
            int dataSize = sampleCount * blockAlign;

            // RIFF header
            writer.Write(new char[] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + dataSize);
            writer.Write(new char[] { 'W', 'A', 'V', 'E' });

            // fmt chunk
            writer.Write(new char[] { 'f', 'm', 't', ' ' });
            writer.Write(16);                // chunk size
            writer.Write((short)1);          // PCM format
            writer.Write(channels);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write(blockAlign);
            writer.Write(bitsPerSample);

            // data chunk
            writer.Write(new char[] { 'd', 'a', 't', 'a' });
            writer.Write(dataSize);

            for (int i = 0; i < sampleCount; i++)
            {
                float clamped = Mathf.Clamp(samples[i], -1f, 1f);
                short pcm = (short)(clamped * short.MaxValue);
                writer.Write(pcm);
            }
        }
    }
}
