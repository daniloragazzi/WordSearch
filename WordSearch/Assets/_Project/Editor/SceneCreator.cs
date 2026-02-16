using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace RagazziStudios.Editor
{
    /// <summary>
    /// Cria as 3 cenas do jogo (Boot, MainMenu, Game) com hierarquia básica.
    /// Menu: Build → Ragazzi Studios → Create Scenes
    /// 
    /// Após executar, as cenas estarão em Assets/_Project/Scenes/ e já
    /// configuradas no Build Settings.
    /// </summary>
    public static class SceneCreator
    {
        private const string SCENES_PATH = "Assets/_Project/Scenes";

        [MenuItem("Build/Ragazzi Studios/🎬 Create All Scenes", priority = 1)]
        public static void CreateAllScenes()
        {
            // Salvar cena atual
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            CreateBootScene();
            CreateMainMenuScene();
            CreateGameScene();

            // Configurar no Build Settings
            BuildScript.ConfigureScenes();

            Debug.Log("[SceneCreator] ✅ All 3 scenes created and configured!");
            Debug.Log("[SceneCreator] 📋 Next: Open Boot scene and attach scripts to GameObjects.");
        }

        // ═══════════════════════════════════════════════════
        //  Boot Scene
        // ═══════════════════════════════════════════════════

        private static void CreateBootScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            var camGO = new GameObject("Main Camera");
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5;
            cam.backgroundColor = new Color(0.12f, 0.12f, 0.18f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            camGO.tag = "MainCamera";

            // GameManager (DontDestroyOnLoad)
            var gmGO = new GameObject("GameManager");
            // Componente será adicionado manualmente pois depende de assembly

            // BootLoader
            var bootGO = new GameObject("BootLoader");

            // Canvas para loading UI
            var canvasGO = CreateCanvas("BootCanvas");

            // Loading text
            var loadingGO = CreateTextElement(canvasGO.transform, "LoadingText",
                "Carregando...", 36, TextAlignmentOptions.Center);
            var loadingRect = loadingGO.GetComponent<RectTransform>();
            loadingRect.anchoredPosition = Vector2.zero;

            // Fade panel (fullscreen overlay)
            var fadeGO = new GameObject("FadePanel");
            fadeGO.transform.SetParent(canvasGO.transform, false);
            var fadeImage = fadeGO.AddComponent<Image>();
            fadeImage.color = new Color(0.12f, 0.12f, 0.18f, 1f);
            var fadeRect = fadeGO.GetComponent<RectTransform>();
            fadeRect.anchorMin = Vector2.zero;
            fadeRect.anchorMax = Vector2.one;
            fadeRect.sizeDelta = Vector2.zero;

            SaveScene(scene, "Boot");
        }

        // ═══════════════════════════════════════════════════
        //  MainMenu Scene
        // ═══════════════════════════════════════════════════

        private static void CreateMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            var camGO = new GameObject("Main Camera");
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5;
            cam.backgroundColor = new Color(0.16f, 0.18f, 0.32f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            camGO.tag = "MainCamera";

            // NavigationController
            var navGO = new GameObject("NavigationController");

            // Canvas
            var canvasGO = CreateCanvas("UICanvas");

            // === MainMenu Screen ===
            var menuScreen = CreateScreen(canvasGO.transform, "MainMenuScreen");
            CreateTextElement(menuScreen.transform, "Title", "CAÇA-PALAVRAS", 48,
                TextAlignmentOptions.Center, new Vector2(0, 200));
            CreateTextElement(menuScreen.transform, "Subtitle", "por Ragazzi Studios", 20,
                TextAlignmentOptions.Center, new Vector2(0, 140));
            CreateButton(menuScreen.transform, "PlayButton", "JOGAR", new Vector2(0, -20));
            CreateButton(menuScreen.transform, "SettingsButton", "⚙", new Vector2(0, -100));

            // === CategorySelect Screen ===
            var catScreen = CreateScreen(canvasGO.transform, "CategorySelectScreen");
            catScreen.SetActive(false);
            CreateTextElement(catScreen.transform, "Title", "CATEGORIAS", 36,
                TextAlignmentOptions.Center, new Vector2(0, 380));
            CreateButton(catScreen.transform, "BackButton", "←", new Vector2(-300, 380));

            // Grid container for categories
            var catGrid = new GameObject("CategoryGrid");
            catGrid.transform.SetParent(catScreen.transform, false);
            var catGridLayout = catGrid.AddComponent<GridLayoutGroup>();
            catGridLayout.cellSize = new Vector2(300, 120);
            catGridLayout.spacing = new Vector2(20, 20);
            catGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            catGridLayout.constraintCount = 2;
            catGridLayout.childAlignment = TextAnchor.UpperCenter;
            var catGridRect = catGrid.GetComponent<RectTransform>();
            catGridRect.anchorMin = new Vector2(0.05f, 0.05f);
            catGridRect.anchorMax = new Vector2(0.95f, 0.85f);
            catGridRect.sizeDelta = Vector2.zero;

            // === LevelSelect Screen ===
            var lvlScreen = CreateScreen(canvasGO.transform, "LevelSelectScreen");
            lvlScreen.SetActive(false);
            CreateTextElement(lvlScreen.transform, "Title", "NÍVEIS", 36,
                TextAlignmentOptions.Center, new Vector2(0, 380));
            CreateButton(lvlScreen.transform, "BackButton", "←", new Vector2(-300, 380));

            // Grid container for levels
            var lvlGrid = new GameObject("LevelGrid");
            lvlGrid.transform.SetParent(lvlScreen.transform, false);
            var lvlGridLayout = lvlGrid.AddComponent<GridLayoutGroup>();
            lvlGridLayout.cellSize = new Vector2(120, 120);
            lvlGridLayout.spacing = new Vector2(15, 15);
            lvlGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            lvlGridLayout.constraintCount = 5;
            lvlGridLayout.childAlignment = TextAnchor.UpperCenter;
            var lvlGridRect = lvlGrid.GetComponent<RectTransform>();
            lvlGridRect.anchorMin = new Vector2(0.1f, 0.2f);
            lvlGridRect.anchorMax = new Vector2(0.9f, 0.85f);
            lvlGridRect.sizeDelta = Vector2.zero;

            // === Settings Popup ===
            var settingsPopup = CreateScreen(canvasGO.transform, "SettingsPopup");
            settingsPopup.SetActive(false);
            CreateTextElement(settingsPopup.transform, "Title", "CONFIGURAÇÕES", 30,
                TextAlignmentOptions.Center, new Vector2(0, 200));
            CreateButton(settingsPopup.transform, "CloseButton", "✕", new Vector2(280, 200));

            SaveScene(scene, "MainMenu");
        }

        // ═══════════════════════════════════════════════════
        //  Game Scene
        // ═══════════════════════════════════════════════════

        private static void CreateGameScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            var camGO = new GameObject("Main Camera");
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5;
            cam.backgroundColor = new Color(0.12f, 0.15f, 0.25f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            camGO.tag = "MainCamera";

            // GameplayController
            var gpGO = new GameObject("GameplayController");

            // Canvas
            var canvasGO = CreateCanvas("GameCanvas");

            // === Header ===
            var header = new GameObject("Header");
            header.transform.SetParent(canvasGO.transform, false);
            var headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 0.9f);
            headerRect.anchorMax = Vector2.one;
            headerRect.sizeDelta = Vector2.zero;

            CreateTextElement(header.transform, "CategoryTitle", "ANIMAIS",
                24, TextAlignmentOptions.Center, Vector2.zero);
            CreateButton(header.transform, "BackButton", "←", new Vector2(-300, 0));
            CreateButton(header.transform, "HintButton", "💡", new Vector2(300, 0));

            // === GridView ===
            var gridContainer = new GameObject("GridView");
            gridContainer.transform.SetParent(canvasGO.transform, false);
            var gridRect = gridContainer.AddComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.02f, 0.25f);
            gridRect.anchorMax = new Vector2(0.98f, 0.88f);
            gridRect.sizeDelta = Vector2.zero;
            var gridLayout = gridContainer.AddComponent<GridLayoutGroup>();
            gridLayout.childAlignment = TextAnchor.MiddleCenter;

            // SelectionLine overlay
            var selectionLine = new GameObject("SelectionLine");
            selectionLine.transform.SetParent(canvasGO.transform, false);
            var slRect = selectionLine.AddComponent<RectTransform>();
            slRect.anchorMin = Vector2.zero;
            slRect.anchorMax = Vector2.one;
            slRect.sizeDelta = Vector2.zero;

            // === WordList ===
            var wordListContainer = new GameObject("WordListView");
            wordListContainer.transform.SetParent(canvasGO.transform, false);
            var wlRect = wordListContainer.AddComponent<RectTransform>();
            wlRect.anchorMin = new Vector2(0.05f, 0.02f);
            wlRect.anchorMax = new Vector2(0.95f, 0.23f);
            wlRect.sizeDelta = Vector2.zero;

            // WordList grid
            var wordGrid = new GameObject("WordGrid");
            wordGrid.transform.SetParent(wordListContainer.transform, false);
            var wgLayout = wordGrid.AddComponent<GridLayoutGroup>();
            wgLayout.cellSize = new Vector2(200, 30);
            wgLayout.spacing = new Vector2(10, 5);
            wgLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            wgLayout.constraintCount = 3;
            var wgRect = wordGrid.GetComponent<RectTransform>();
            wgRect.anchorMin = Vector2.zero;
            wgRect.anchorMax = Vector2.one;
            wgRect.sizeDelta = Vector2.zero;

            // === WinPopup (hidden) ===
            var winPopup = CreateScreen(canvasGO.transform, "WinPopup");
            winPopup.SetActive(false);
            var winBg = winPopup.GetComponent<Image>();
            if (winBg != null) winBg.color = new Color(0, 0, 0, 0.7f);
            CreateTextElement(winPopup.transform, "Title", "🎉 PARABÉNS!", 42,
                TextAlignmentOptions.Center, new Vector2(0, 100));
            CreateTextElement(winPopup.transform, "Stats", "5 palavras • 1:23", 24,
                TextAlignmentOptions.Center, new Vector2(0, 30));
            CreateButton(winPopup.transform, "NextButton", "PRÓXIMO NÍVEL", new Vector2(0, -60));
            CreateButton(winPopup.transform, "LevelSelectButton", "SELECIONAR NÍVEL", new Vector2(0, -130));

            SaveScene(scene, "Game");
        }

        // ═══════════════════════════════════════════════════
        //  UI Factory Helpers
        // ═══════════════════════════════════════════════════

        private static GameObject CreateCanvas(string name)
        {
            var canvasGO = new GameObject(name);
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            // EventSystem
            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                var esGO = new GameObject("EventSystem");
                esGO.AddComponent<EventSystem>();
                esGO.AddComponent<StandaloneInputModule>();
            }

            return canvasGO;
        }

        private static GameObject CreateScreen(Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.15f, 0.17f, 0.28f, 1f);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            return go;
        }

        private static GameObject CreateTextElement(Transform parent, string name,
            string text, int fontSize, TextAlignmentOptions alignment,
            Vector2 position = default)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = alignment;
            tmp.color = Color.white;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(600, 60);
            rect.anchoredPosition = position;

            return go;
        }

        private static GameObject CreateButton(Transform parent, string name,
            string label, Vector2 position)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.29f, 0.56f, 0.89f, 1f); // Primary blue

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 60);
            rect.anchoredPosition = position;

            // Label
            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);

            var tmp = labelGO.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;

            return go;
        }

        private static void SaveScene(Scene scene, string name)
        {
            string dir = SCENES_PATH;
            if (!AssetDatabase.IsValidFolder(dir))
            {
                // Criar subpastas se necessário
                string[] parts = dir.Split('/');
                string current = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    string next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                    {
                        AssetDatabase.CreateFolder(current, parts[i]);
                    }
                    current = next;
                }
            }

            string path = $"{dir}/{name}.unity";
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log($"[SceneCreator] ✅ Scene saved: {path}");
        }
    }
}
