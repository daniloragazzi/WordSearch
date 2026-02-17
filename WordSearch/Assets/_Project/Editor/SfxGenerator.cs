using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace RagazziStudios.Editor
{
    /// <summary>
    /// Gera clips de áudio SFX procedurais para o projeto.
    /// Menu: Build > Ragazzi Studios > Generate SFX
    /// </summary>
    public static class SfxGenerator
    {
        private const string OUTPUT_PATH = "Assets/_Project/Audio/SFX";
        private const int SAMPLE_RATE = 44100;

        [MenuItem("Build/Ragazzi Studios/Generate SFX")]
        public static void GenerateAll()
        {
            EnsureDirectory(OUTPUT_PATH);

            GenerateWordFound();
            GenerateAllWordsFound();
            GenerateButtonClick();
            GenerateHintUsed();
            GenerateInvalidSelection();

            AssetDatabase.Refresh();
            Debug.Log($"[SfxGenerator] 5 SFX clips generated in {OUTPUT_PATH}");
        }

        // ─── word_found: ascending happy ding ───
        private static void GenerateWordFound()
        {
            float duration = 0.35f;
            int samples = (int)(SAMPLE_RATE * duration);
            var data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SAMPLE_RATE;
                float env = Mathf.Exp(-t * 8f); // decay envelope

                // Two harmonics: fundamental + octave
                float freq = 880f + t * 400f; // ascending from A5
                float sample = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.6f;
                sample += Mathf.Sin(2f * Mathf.PI * freq * 2f * t) * 0.25f;

                data[i] = sample * env * 0.7f;
            }

            SaveWav(Path.Combine(OUTPUT_PATH, "word_found.wav"), data);
        }

        // ─── all_words_found: celebratory multi-tone fanfare ───
        private static void GenerateAllWordsFound()
        {
            float duration = 0.9f;
            int samples = (int)(SAMPLE_RATE * duration);
            var data = new float[samples];

            // Three ascending notes: C5, E5, G5 (major chord arpeggio)
            float[] freqs = { 523.25f, 659.25f, 783.99f };
            float noteLen = duration / freqs.Length;

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SAMPLE_RATE;
                int noteIndex = Mathf.Min((int)(t / noteLen), freqs.Length - 1);
                float noteTime = t - noteIndex * noteLen;
                float env = Mathf.Exp(-noteTime * 4f);

                float freq = freqs[noteIndex];
                float sample = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.5f;
                sample += Mathf.Sin(2f * Mathf.PI * freq * 1.5f * t) * 0.2f; // fifth harmonic
                sample += Mathf.Sin(2f * Mathf.PI * freq * 2f * t) * 0.15f; // octave

                data[i] = sample * env * 0.7f;
            }

            SaveWav(Path.Combine(OUTPUT_PATH, "all_words_found.wav"), data);
        }

        // ─── button_click: short soft click ───
        private static void GenerateButtonClick()
        {
            float duration = 0.08f;
            int samples = (int)(SAMPLE_RATE * duration);
            var data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SAMPLE_RATE;
                float env = Mathf.Exp(-t * 60f); // very fast decay

                float sample = Mathf.Sin(2f * Mathf.PI * 1200f * t) * 0.4f;
                sample += Mathf.Sin(2f * Mathf.PI * 600f * t) * 0.3f;

                data[i] = sample * env * 0.5f;
            }

            SaveWav(Path.Combine(OUTPUT_PATH, "button_click.wav"), data);
        }

        // ─── hint_used: gentle notification chime ───
        private static void GenerateHintUsed()
        {
            float duration = 0.4f;
            int samples = (int)(SAMPLE_RATE * duration);
            var data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SAMPLE_RATE;
                float env = Mathf.Exp(-t * 6f);

                // Soft descending tone
                float freq = 1100f - t * 300f;
                float sample = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.5f;
                sample += Mathf.Sin(2f * Mathf.PI * freq * 0.5f * t) * 0.2f;

                data[i] = sample * env * 0.5f;
            }

            SaveWav(Path.Combine(OUTPUT_PATH, "hint_used.wav"), data);
        }

        // ─── invalid_selection: short error buzz ───
        private static void GenerateInvalidSelection()
        {
            float duration = 0.2f;
            int samples = (int)(SAMPLE_RATE * duration);
            var data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SAMPLE_RATE;
                float env = Mathf.Exp(-t * 12f);

                // Dissonant low buzz
                float sample = Mathf.Sin(2f * Mathf.PI * 180f * t) * 0.4f;
                sample += Mathf.Sin(2f * Mathf.PI * 220f * t) * 0.3f;
                sample += Mathf.Sin(2f * Mathf.PI * 260f * t) * 0.2f; // minor second creates tension

                data[i] = sample * env * 0.6f;
            }

            SaveWav(Path.Combine(OUTPUT_PATH, "invalid_selection.wav"), data);
        }

        // ─── WAV File Writer ───

        private static void SaveWav(string path, float[] data)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            using (var writer = new BinaryWriter(stream))
            {
                int channels = 1;
                int bitsPerSample = 16;
                int byteRate = SAMPLE_RATE * channels * bitsPerSample / 8;
                int blockAlign = channels * bitsPerSample / 8;
                int dataSize = data.Length * blockAlign;

                // RIFF header
                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + dataSize);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

                // fmt chunk
                writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                writer.Write(16); // chunk size
                writer.Write((short)1); // PCM
                writer.Write((short)channels);
                writer.Write(SAMPLE_RATE);
                writer.Write(byteRate);
                writer.Write((short)blockAlign);
                writer.Write((short)bitsPerSample);

                // data chunk
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                writer.Write(dataSize);

                for (int i = 0; i < data.Length; i++)
                {
                    float clamped = Mathf.Clamp(data[i], -1f, 1f);
                    short pcm = (short)(clamped * 32767f);
                    writer.Write(pcm);
                }
            }
        }

        private static void EnsureDirectory(string path)
        {
            string fullPath = Path.Combine(Application.dataPath, "..", path);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
        }
    }
}
