using UnityEngine;
using UnityEditor;
using System.IO;

namespace RagazziStudios.Editor
{
    /// <summary>
    /// Gera um loop de música ambiente procedural para o projeto.
    /// Menu: Build > Ragazzi Studios > Generate Music
    /// </summary>
    public static class MusicGenerator
    {
        private const string OUTPUT_PATH = "Assets/_Project/Audio/Music";
        private const int SAMPLE_RATE = 44100;

        [MenuItem("Build/Ragazzi Studios/Generate Music")]
        public static void GenerateAll()
        {
            EnsureDirectory(OUTPUT_PATH);
            GenerateAmbientLoop();
            AssetDatabase.Refresh();
            Debug.Log($"[MusicGenerator] Ambient loop generated in {OUTPUT_PATH}");
        }

        /// <summary>
        /// Gera um loop ambient relaxante de ~30 segundos.
        /// Usa acordes de pad com filtro suave e progressão I-vi-IV-V em Dó maior.
        /// </summary>
        private static void GenerateAmbientLoop()
        {
            float duration = 32f; // 32 seconds for clean loop (8 bars at ~60bpm)
            int totalSamples = (int)(SAMPLE_RATE * duration);
            var data = new float[totalSamples];

            // Chord progression: C, Am, F, G (each 4 seconds = 8 bars total at 60bpm)
            // Repeated twice for 32 seconds
            float[][] chords = new float[][]
            {
                new float[] { 261.63f, 329.63f, 392.00f },  // C major (C4, E4, G4)
                new float[] { 220.00f, 261.63f, 329.63f },  // A minor (A3, C4, E4)
                new float[] { 174.61f, 220.00f, 261.63f },  // F major (F3, A3, C4)
                new float[] { 196.00f, 246.94f, 293.66f },  // G major (G3, B3, D4)
            };

            float chordDuration = 4f; // 4 seconds per chord
            float masterVolume = 0.18f; // Soft background level

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / SAMPLE_RATE;
                float globalT = t % (chordDuration * chords.Length);
                int chordIndex = (int)(globalT / chordDuration) % chords.Length;
                float chordT = globalT - chordIndex * chordDuration;

                float[] chord = chords[chordIndex];

                // Smooth crossfade envelope within each chord
                float env = 1f;
                if (chordT < 0.3f) env = chordT / 0.3f; // fade in
                if (chordT > chordDuration - 0.3f) env = (chordDuration - chordT) / 0.3f; // fade out

                float sample = 0f;

                // Pad: sum of sine waves for each note in the chord
                for (int n = 0; n < chord.Length; n++)
                {
                    float freq = chord[n];

                    // Fundamental + soft harmonics for warmth
                    sample += Mathf.Sin(2f * Mathf.PI * freq * t) * 0.35f;
                    sample += Mathf.Sin(2f * Mathf.PI * freq * 2f * t) * 0.12f; // octave
                    sample += Mathf.Sin(2f * Mathf.PI * freq * 3f * t) * 0.05f; // 12th

                    // Slight detuned double for width
                    sample += Mathf.Sin(2f * Mathf.PI * (freq * 1.003f) * t) * 0.15f;
                }

                // Subtle low-frequency oscillation (tremolo)
                float lfo = 1f + Mathf.Sin(2f * Mathf.PI * 0.25f * t) * 0.08f;

                data[i] = sample * env * lfo * masterVolume;
            }

            // Apply global fade-in/out for seamless loop feeling
            int fadeSamples = SAMPLE_RATE; // 1 second fade
            for (int i = 0; i < fadeSamples && i < totalSamples; i++)
            {
                float fade = (float)i / fadeSamples;
                data[i] *= fade;
                data[totalSamples - 1 - i] *= fade;
            }

            SaveWav(Path.Combine(OUTPUT_PATH, "ambient_loop.wav"), data);
        }

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

                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + dataSize);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

                writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)channels);
                writer.Write(SAMPLE_RATE);
                writer.Write(byteRate);
                writer.Write((short)blockAlign);
                writer.Write((short)bitsPerSample);

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
