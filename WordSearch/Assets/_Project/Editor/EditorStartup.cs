using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RagazziStudios.Editor
{
    /// <summary>
    /// Garante que, ao pressionar Play no Editor, o jogo sempre inicie
    /// pela cena Boot (index 0), independente de qual cena está aberta.
    /// Isso é necessário porque o GameManager (DontDestroyOnLoad) é criado
    /// na Boot e precisa carregar dados antes de navegar para MainMenu/Game.
    /// </summary>
    [InitializeOnLoad]
    public static class EditorStartup
    {
        private const string BOOT_SCENE_PATH = "Assets/_Project/Scenes/Boot.unity";

        static EditorStartup()
        {
            SetPlayModeStartScene();
        }

        private static void SetPlayModeStartScene()
        {
            var bootScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BOOT_SCENE_PATH);
            if (bootScene != null)
            {
                EditorSceneManager.playModeStartScene = bootScene;
                // Debug.Log("[EditorStartup] Play mode will always start from Boot.unity");
            }
            else
            {
                Debug.LogWarning(
                    $"[EditorStartup] Boot scene not found at '{BOOT_SCENE_PATH}'. " +
                    "Run 'Build → Ragazzi Studios → 🎬 Create All Scenes' first.");
            }
        }
    }
}
