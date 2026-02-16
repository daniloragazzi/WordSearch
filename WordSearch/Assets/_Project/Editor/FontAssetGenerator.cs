using UnityEditor;
using UnityEngine;
using TMPro;
using TMPro.EditorUtilities;
using System.IO;

namespace RagazziStudios.Editor
{
    /// <summary>
    /// Gera TMP SDF Font Assets a partir dos TTFs da pasta Art/Fonts/.
    /// Menu: Build → Ragazzi Studios → 🔤 Generate Font Assets
    /// </summary>
    public static class FontAssetGenerator
    {
        private const string FONTS_PATH = "Assets/_Project/Art/Fonts";
        private const string SDF_PATH = "Assets/_Project/Art/Fonts/SDF";

        private static readonly string[] FontFiles = new[]
        {
            "Nunito-Regular",
            "Nunito-SemiBold",
            "Nunito-Bold",
            "Nunito-ExtraBold"
        };

        // Extended ASCII + PT-BR special chars
        private const string CUSTOM_CHARS =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" +
            "0123456789" +
            " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~" +
            "ÀÁÂÃÇÉÊÍÓÔÕÚÜàáâãçéêíóôõúü" +
            "¡¢£¤¥¦§¨©ª«¬®¯°±²³´µ¶·¸¹º»¼½¾¿×÷" +
            "…–—''""•€™";

        [MenuItem("Build/Ragazzi Studios/🔤 Generate Font Assets", priority = 2)]
        public static void GenerateAll()
        {
            if (!Directory.Exists(SDF_PATH))
            {
                Directory.CreateDirectory(SDF_PATH);
                AssetDatabase.Refresh();
            }

            int generated = 0;
            foreach (var fontName in FontFiles)
            {
                var ttfPath = $"{FONTS_PATH}/{fontName}.ttf";
                var sdfPath = $"{SDF_PATH}/{fontName} SDF.asset";

                // Skip if SDF already exists
                if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(sdfPath) != null)
                {
                    Debug.Log($"[FontGenerator] {fontName} SDF already exists, skipping.");
                    continue;
                }

                var sourceFont = AssetDatabase.LoadAssetAtPath<Font>(ttfPath);
                if (sourceFont == null)
                {
                    Debug.LogError($"[FontGenerator] TTF not found: {ttfPath}");
                    continue;
                }

                // Create SDF font asset
                var fontAsset = TMP_FontAsset.CreateFontAsset(
                    sourceFont,
                    64,     // Sampling Point Size
                    5,      // Padding
                    UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA,
                    512,    // Atlas Width
                    512     // Atlas Height
                );

                if (fontAsset == null)
                {
                    Debug.LogError($"[FontGenerator] Failed to create SDF for {fontName}");
                    continue;
                }

                // Try to add custom characters
                fontAsset.TryAddCharacters(CUSTOM_CHARS);

                AssetDatabase.CreateAsset(fontAsset, sdfPath);

                // Save atlas texture
                if (fontAsset.atlasTexture != null)
                {
                    fontAsset.atlasTexture.name = $"{fontName} Atlas";
                    AssetDatabase.AddObjectToAsset(fontAsset.atlasTexture, fontAsset);
                }

                // Save material
                if (fontAsset.material != null)
                {
                    fontAsset.material.name = $"{fontName} Material";
                    AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
                }

                AssetDatabase.SaveAssets();
                generated++;
                Debug.Log($"[FontGenerator] ✅ Created: {sdfPath}");
            }

            AssetDatabase.Refresh();
            Debug.Log($"[FontGenerator] Done! {generated} font assets generated.");
        }

        /// <summary>
        /// Loads a Nunito SDF font asset by weight name.
        /// </summary>
        public static TMP_FontAsset LoadFont(string weight = "Regular")
        {
            var path = $"{SDF_PATH}/Nunito-{weight} SDF.asset";
            var asset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (asset == null)
                Debug.LogWarning($"[FontGenerator] Font not found: {path}. Run 'Generate Font Assets' first.");
            return asset;
        }
    }
}
