using UnityEditor;
using UnityEngine;
using System.IO;
using RagazziStudios.Game.Config;

namespace RagazziStudios.Editor
{
    /// <summary>
    /// Gera os ScriptableObject assets GameTheme_Light e GameTheme_Dark.
    /// Menu: Build → Ragazzi Studios → 🎨 Generate Theme Assets
    /// </summary>
    public static class ThemePaletteGenerator
    {
        private const string OUTPUT_PATH = "Assets/_Project/Config/Themes";

        [MenuItem("Build/Ragazzi Studios/Generate Theme Assets", priority = 6)]
        public static void GenerateAll()
        {
            if (!Directory.Exists(OUTPUT_PATH))
            {
                Directory.CreateDirectory(OUTPUT_PATH);
                AssetDatabase.Refresh();
            }

            CreateLightTheme();
            CreateDarkTheme();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[ThemePaletteGenerator] ✅ GameTheme_Light e GameTheme_Dark gerados em " + OUTPUT_PATH);
            EditorUtility.DisplayDialog(
                "Theme Assets",
                "GameTheme_Light e GameTheme_Dark gerados com sucesso em:\n" + OUTPUT_PATH,
                "OK");
        }

        // ─────────────────────────────────────────────────────────────
        //  TEMA CLARO (valores originais do projeto)
        // ─────────────────────────────────────────────────────────────
        private static void CreateLightTheme()
        {
            const string assetPath = OUTPUT_PATH + "/GameTheme_Light.asset";
            var existing = AssetDatabase.LoadAssetAtPath<GameTheme>(assetPath);
            if (existing != null)
            {
                Debug.Log("[ThemePaletteGenerator] GameTheme_Light já existe — ignorando.");
                return;
            }

            var t = ScriptableObject.CreateInstance<GameTheme>();
            t.isDark = false;

            // Paleta primária
            t.primary       = new Color(0.20f, 0.47f, 0.96f);   // #3378F5
            t.primaryDark   = new Color(0.13f, 0.33f, 0.73f);   // #2254BA
            t.primaryLight  = new Color(0.55f, 0.73f, 1.00f);   // #8CBAFF

            // Paleta secundária
            t.accent        = new Color(1.00f, 0.60f, 0.20f);   // #FF9933
            t.accentDark    = new Color(0.85f, 0.45f, 0.10f);   // #D9731A
            t.success       = new Color(0.30f, 0.80f, 0.40f);   // #4DCC66
            t.warning       = new Color(1.00f, 0.85f, 0.30f);   // #FFD94D
            t.error         = new Color(0.93f, 0.30f, 0.30f);   // #ED4D4D

            // Fundos
            t.background    = new Color(0.96f, 0.97f, 1.00f);   // #F5F8FF
            t.surface       = new Color(1.00f, 1.00f, 1.00f);   // #FFFFFF
            t.overlay       = new Color(0.00f, 0.00f, 0.00f, 0.50f);
            t.gridBackground= new Color(0.92f, 0.94f, 0.98f);   // #EBF0FA

            // Textos
            t.textPrimary   = new Color(0.12f, 0.14f, 0.20f);   // #1F2433
            t.textSecondary = new Color(0.45f, 0.50f, 0.58f);   // #738094
            t.textOnColor   = new Color(1.00f, 1.00f, 1.00f);   // #FFFFFF
            t.textDisabled  = new Color(0.70f, 0.73f, 0.78f);   // #B3BAC7

            // Grid — Células
            t.cellNormal        = new Color(0.95f, 0.96f, 0.98f);   // #F2F5FA
            t.cellSelected      = new Color(0.40f, 0.65f, 1.00f);   // #66A6FF
            t.cellFound         = new Color(0.30f, 0.80f, 0.40f);   // #4DCC66
            t.cellHint          = new Color(1.00f, 0.85f, 0.30f);   // #FFD94D
            t.cellLetterNormal  = new Color(0.15f, 0.17f, 0.22f);   // #262C38
            t.cellLetterActive  = new Color(1.00f, 1.00f, 1.00f);   // #FFFFFF

            // Lista de palavras
            t.wordNormal    = new Color(0.20f, 0.22f, 0.28f);   // #333847
            t.wordFound     = new Color(0.30f, 0.80f, 0.40f);   // #4DCC66

            // Botões
            t.buttonPrimary         = new Color(0.20f, 0.47f, 0.96f);  // #3378F5
            t.buttonPrimaryPressed  = new Color(0.13f, 0.33f, 0.73f);  // #2254BA
            t.buttonSecondary       = new Color(0.90f, 0.92f, 0.96f);  // #E6EBF5
            t.buttonDisabled        = new Color(0.85f, 0.87f, 0.90f);  // #D9DEE6

            // Categorias
            t.categoryAnimais       = new Color(0.40f, 0.75f, 0.45f);
            t.categoryAlimentos     = new Color(0.95f, 0.55f, 0.25f);
            t.categoryEsportes      = new Color(0.30f, 0.55f, 0.95f);
            t.categoryProfissoes    = new Color(0.60f, 0.45f, 0.85f);
            t.categoryNatureza      = new Color(0.25f, 0.70f, 0.60f);
            t.categoryCorpoHumano   = new Color(0.90f, 0.45f, 0.50f);
            t.categoryPaises        = new Color(0.50f, 0.65f, 0.85f);
            t.categoryCoresFormas   = new Color(0.85f, 0.65f, 0.35f);

            AssetDatabase.CreateAsset(t, assetPath);
            Debug.Log("[ThemePaletteGenerator] GameTheme_Light criado.");
        }

        // ─────────────────────────────────────────────────────────────
        //  TEMA ESCURO
        // ─────────────────────────────────────────────────────────────
        private static void CreateDarkTheme()
        {
            const string assetPath = OUTPUT_PATH + "/GameTheme_Dark.asset";
            var existing = AssetDatabase.LoadAssetAtPath<GameTheme>(assetPath);
            if (existing != null)
            {
                Debug.Log("[ThemePaletteGenerator] GameTheme_Dark já existe — ignorando.");
                return;
            }

            var t = ScriptableObject.CreateInstance<GameTheme>();
            t.isDark = true;

            // Paleta primária — mesma identidade de marca
            t.primary       = new Color(0.20f, 0.47f, 0.96f);   // #3378F5
            t.primaryDark   = new Color(0.13f, 0.33f, 0.73f);   // #2254BA
            t.primaryLight  = new Color(0.55f, 0.73f, 1.00f);   // #8CBAFF

            // Paleta secundária — cores de estado se mantêm vibrantes no escuro
            t.accent        = new Color(1.00f, 0.60f, 0.20f);   // #FF9933
            t.accentDark    = new Color(0.85f, 0.45f, 0.10f);   // #D9731A
            t.success       = new Color(0.30f, 0.80f, 0.40f);   // #4DCC66
            t.warning       = new Color(1.00f, 0.85f, 0.30f);   // #FFD94D
            t.error         = new Color(0.93f, 0.30f, 0.30f);   // #ED4D4D

            // Fundos — deep navy escuro
            t.background    = new Color(0.06f, 0.07f, 0.09f);   // #0F1117
            t.surface       = new Color(0.11f, 0.12f, 0.18f);   // #1C1F2E
            t.overlay       = new Color(0.00f, 0.00f, 0.00f, 0.75f); // overlay mais denso
            t.gridBackground= new Color(0.08f, 0.09f, 0.13f);   // #141722

            // Textos — claro sobre escuro
            t.textPrimary   = new Color(0.91f, 0.93f, 0.96f);   // #E8ECF5
            t.textSecondary = new Color(0.48f, 0.53f, 0.60f);   // #7A879A
            t.textOnColor   = new Color(1.00f, 1.00f, 1.00f);   // #FFFFFF
            t.textDisabled  = new Color(0.24f, 0.27f, 0.33f);   // #3D4455

            // Grid — células escuras com letras claras
            t.cellNormal        = new Color(0.12f, 0.13f, 0.20f);   // #1E2133
            t.cellSelected      = new Color(0.40f, 0.65f, 1.00f);   // #66A6FF (mantém)
            t.cellFound         = new Color(0.30f, 0.80f, 0.40f);   // #4DCC66 (mantém)
            t.cellHint          = new Color(1.00f, 0.85f, 0.30f);   // #FFD94D (mantém)
            t.cellLetterNormal  = new Color(0.78f, 0.82f, 0.88f);   // #C7D0E0
            t.cellLetterActive  = new Color(1.00f, 1.00f, 1.00f);   // #FFFFFF

            // Lista de palavras — texto claro
            t.wordNormal    = new Color(0.75f, 0.78f, 0.85f);   // #BFC7D9
            t.wordFound     = new Color(0.30f, 0.80f, 0.40f);   // #4DCC66

            // Botões
            t.buttonPrimary         = new Color(0.20f, 0.47f, 0.96f);  // #3378F5
            t.buttonPrimaryPressed  = new Color(0.13f, 0.33f, 0.73f);  // #2254BA
            t.buttonSecondary       = new Color(0.15f, 0.16f, 0.21f);  // #262836
            t.buttonDisabled        = new Color(0.11f, 0.12f, 0.17f);  // #1C1F2B

            // Categorias — cores vibrantes mantidas (bom contraste sobre fundos escuros)
            t.categoryAnimais       = new Color(0.40f, 0.75f, 0.45f);
            t.categoryAlimentos     = new Color(0.95f, 0.55f, 0.25f);
            t.categoryEsportes      = new Color(0.30f, 0.55f, 0.95f);
            t.categoryProfissoes    = new Color(0.60f, 0.45f, 0.85f);
            t.categoryNatureza      = new Color(0.25f, 0.70f, 0.60f);
            t.categoryCorpoHumano   = new Color(0.90f, 0.45f, 0.50f);
            t.categoryPaises        = new Color(0.50f, 0.65f, 0.85f);
            t.categoryCoresFormas   = new Color(0.85f, 0.65f, 0.35f);

            AssetDatabase.CreateAsset(t, assetPath);
            Debug.Log("[ThemePaletteGenerator] GameTheme_Dark criado.");
        }
    }
}
