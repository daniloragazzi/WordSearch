using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace RagazziStudios.Editor
{
    /// <summary>
    /// Script de build para gerar APK/AAB Android.
    /// Acessível via menu Unity: Build → Ragazzi Studios.
    /// 
    /// Uso:
    ///   Menu: Build → Build APK (Development)
    ///   Menu: Build → Build APK (Release)
    ///   Menu: Build → Build AAB (Play Store)
    ///   CLI:  Unity -batchmode -executeMethod RagazziStudios.Editor.BuildScript.BuildAPK
    /// </summary>
    public static class BuildScript
    {
        private const string APP_NAME = "CacaPalavras";
        private const string BUILD_FOLDER = "Builds/Android";

        // ═══════════════════════════════════════════════════
        //  Menu Items
        // ═══════════════════════════════════════════════════

        [MenuItem("Build/Ragazzi Studios/🔧 Configure Scenes", priority = 0)]
        public static void ConfigureScenes()
        {
            var scenes = new[]
            {
                "Assets/_Project/Scenes/Boot.unity",
                "Assets/_Project/Scenes/MainMenu.unity",
                "Assets/_Project/Scenes/Game.unity"
            };

            var editorScenes = scenes.Select(s => new EditorBuildSettingsScene(s, true)).ToArray();
            EditorBuildSettings.scenes = editorScenes;

            Debug.Log($"[BuildScript] ✅ {scenes.Length} cenas configuradas no Build Settings:");
            foreach (var scene in scenes)
                Debug.Log($"  → {scene}");
        }

        [MenuItem("Build/Ragazzi Studios/📱 Build APK (Development)", priority = 10)]
        public static void BuildAPK_Dev()
        {
            BuildAPK(isDevelopment: true);
        }

        [MenuItem("Build/Ragazzi Studios/📱 Build APK (Release)", priority = 11)]
        public static void BuildAPK_Release()
        {
            BuildAPK(isDevelopment: false);
        }

        [MenuItem("Build/Ragazzi Studios/📦 Build AAB (Play Store)", priority = 20)]
        public static void BuildAAB()
        {
            ConfigureScenes();

            string path = GetBuildPath($"{APP_NAME}.aab");

            EditorUserBuildSettings.buildAppBundle = true;

            var options = new BuildPlayerOptions
            {
                scenes = GetScenePaths(),
                locationPathName = path,
                target = BuildTarget.Android,
                options = BuildOptions.CompressWithLz4HC
            };

            Build(options, "AAB");
        }

        [MenuItem("Build/Ragazzi Studios/📋 Verify Build Settings", priority = 30)]
        public static void VerifySettings()
        {
            Debug.Log("═══════════════════════════════════════════════");
            Debug.Log("  BUILD SETTINGS VERIFICATION");
            Debug.Log("═══════════════════════════════════════════════");

            // Company & Product
            Debug.Log($"Company: {PlayerSettings.companyName}");
            Debug.Log($"Product: {PlayerSettings.productName}");
            Debug.Log($"Package: {PlayerSettings.applicationIdentifier}");
            Debug.Log($"Version: {PlayerSettings.bundleVersion}");
            Debug.Log($"Bundle Code: {PlayerSettings.Android.bundleVersionCode}");

            // Android Settings
            Debug.Log($"Min SDK: {PlayerSettings.Android.minSdkVersion}");
            Debug.Log($"Target SDK: {PlayerSettings.Android.targetSdkVersion}");
            Debug.Log($"Target Arch: {PlayerSettings.Android.targetArchitectures}");
            Debug.Log($"Scripting Backend: {PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android)}");

            // Scenes
            var scenes = EditorBuildSettings.scenes;
            Debug.Log($"Scenes ({scenes.Length}):");
            for (int i = 0; i < scenes.Length; i++)
            {
                string status = scenes[i].enabled ? "✅" : "❌";
                string exists = File.Exists(scenes[i].path) ? "" : " ⚠️ NOT FOUND";
                Debug.Log($"  [{i}] {status} {scenes[i].path}{exists}");
            }

            // Keystore
            bool hasKeystore = !string.IsNullOrEmpty(PlayerSettings.Android.keystoreName);
            Debug.Log($"Keystore: {(hasKeystore ? PlayerSettings.Android.keystoreName : "⚠️ Not configured (using debug keystore)")}");

            Debug.Log("═══════════════════════════════════════════════");
        }

        // ═══════════════════════════════════════════════════
        //  Core Build Methods
        // ═══════════════════════════════════════════════════

        public static void BuildAPK(bool isDevelopment = true)
        {
            ConfigureScenes();

            string suffix = isDevelopment ? "_dev" : "";
            string path = GetBuildPath($"{APP_NAME}{suffix}.apk");

            EditorUserBuildSettings.buildAppBundle = false;

            var buildOptions = BuildOptions.CompressWithLz4HC;
            if (isDevelopment)
            {
                buildOptions |= BuildOptions.Development;
                buildOptions |= BuildOptions.AllowDebugging;
            }

            var options = new BuildPlayerOptions
            {
                scenes = GetScenePaths(),
                locationPathName = path,
                target = BuildTarget.Android,
                options = buildOptions
            };

            Build(options, isDevelopment ? "APK (Development)" : "APK (Release)");
        }

        private static void Build(BuildPlayerOptions options, string buildType)
        {
            Debug.Log($"[BuildScript] 🚀 Starting {buildType} build...");
            Debug.Log($"[BuildScript] Output: {options.locationPathName}");

            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    long sizeBytes = GetFileSize(options.locationPathName);
                    string sizeMB = (sizeBytes / (1024f * 1024f)).ToString("F1");
                    Debug.Log($"[BuildScript] ✅ {buildType} build succeeded!");
                    Debug.Log($"[BuildScript] 📁 Size: {sizeMB} MB");
                    Debug.Log($"[BuildScript] ⏱️ Time: {summary.totalTime}");
                    Debug.Log($"[BuildScript] 📍 Path: {Path.GetFullPath(options.locationPathName)}");
                    break;

                case BuildResult.Failed:
                    Debug.LogError($"[BuildScript] ❌ {buildType} build FAILED!");
                    Debug.LogError($"[BuildScript] Errors: {summary.totalErrors}");
                    break;

                case BuildResult.Cancelled:
                    Debug.LogWarning($"[BuildScript] ⏸️ {buildType} build cancelled.");
                    break;
            }
        }

        // ═══════════════════════════════════════════════════
        //  Helpers
        // ═══════════════════════════════════════════════════

        private static string[] GetScenePaths()
        {
            return EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();
        }

        private static string GetBuildPath(string filename)
        {
            string dir = Path.Combine(Application.dataPath, "..", BUILD_FOLDER);
            Directory.CreateDirectory(dir);
            return Path.Combine(BUILD_FOLDER, filename);
        }

        private static long GetFileSize(string path)
        {
            try
            {
                return new FileInfo(path).Length;
            }
            catch
            {
                return 0;
            }
        }
    }
}
