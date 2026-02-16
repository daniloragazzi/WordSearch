using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using TMPro;

namespace RagazziStudios.Editor
{
    /// <summary>
    /// Cria as 3 cenas do jogo (Boot, MainMenu, Game) com hierarquia completa.
    /// Adiciona TODOS os scripts e conecta TODAS as referências automaticamente
    /// usando C# Reflection para garantir que os valores são persistidos na cena.
    /// Menu: Build → Ragazzi Studios → 🎬 Create All Scenes
    /// </summary>
    public static class SceneCreator
    {
        private const string SCENES_PATH = "Assets/_Project/Scenes";

        [MenuItem("Build/Ragazzi Studios/🎬 Create All Scenes", priority = 1)]
        public static void CreateAllScenes()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            CreateBootScene();
            CreateMainMenuScene();
            CreateGameScene();

            BuildScript.ConfigureScenes();

            Debug.Log("══════════════════════════════════════════════════════");
            Debug.Log("[SceneCreator] ✅ 3 cenas criadas com todos os scripts conectados!");
            Debug.Log("[SceneCreator] 📋 Abra Boot.unity e aperte Play (▶️)");
            Debug.Log("══════════════════════════════════════════════════════");
        }

        // ═══════════════════════════════════════════════════
        //  Boot Scene
        // ═══════════════════════════════════════════════════

        private static void CreateBootScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // --- Camera ---
            var camGO = new GameObject("Main Camera");
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5;
            cam.backgroundColor = new Color(0.12f, 0.12f, 0.18f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            camGO.tag = "MainCamera";

            // --- GameManager (DontDestroyOnLoad) ---
            var gmGO = new GameObject("GameManager");
            gmGO.SetActive(false); // Desativar antes de AddComponent para evitar Awake/OnEnable prematuro
            gmGO.AddComponent<Core.Application.GameManager>();
            gmGO.SetActive(true);

            // --- BootLoader ---
            var bootGO = new GameObject("BootLoader");
            bootGO.SetActive(false);
            var bootLoader = bootGO.AddComponent<Game.Boot.BootLoader>();

            // --- Canvas ---
            var canvasGO = CreateCanvas("BootCanvas");

            // Loading text
            var loadingGO = CreateTextElement(canvasGO.transform, "LoadingText",
                "Carregando...", 36, TextAlignmentOptions.Center);
            var loadingRect = loadingGO.GetComponent<RectTransform>();
            loadingRect.anchoredPosition = Vector2.zero;

            // CanvasGroup para fade do loading
            var loadingCG = canvasGO.AddComponent<CanvasGroup>();

            // Fade panel
            var fadeGO = new GameObject("FadePanel");
            fadeGO.transform.SetParent(canvasGO.transform, false);
            var fadeImage = fadeGO.AddComponent<Image>();
            fadeImage.color = new Color(0.12f, 0.12f, 0.18f, 1f);
            var fadeRect = fadeGO.GetComponent<RectTransform>();
            fadeRect.anchorMin = Vector2.zero;
            fadeRect.anchorMax = Vector2.one;
            fadeRect.sizeDelta = Vector2.zero;

            // --- Wiring: BootLoader ---
            Wire(bootLoader, "_loadingCanvasGroup", loadingCG);
            bootGO.SetActive(true);

            SaveScene(scene, "Boot");
        }

        // ═══════════════════════════════════════════════════
        //  MainMenu Scene
        // ═══════════════════════════════════════════════════

        private static void CreateMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // --- Camera ---
            var camGO = new GameObject("Main Camera");
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5;
            cam.backgroundColor = new Color(0.16f, 0.18f, 0.32f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            camGO.tag = "MainCamera";

            // --- Canvas ---
            var canvasGO = CreateCanvas("UICanvas");

            // ═══ MainMenu Screen ═══
            var menuScreenGO = CreateScreen(canvasGO.transform, "MainMenuScreen");
            var menuCanvasGroup = menuScreenGO.AddComponent<CanvasGroup>();

            var titleGO = CreateTextElement(menuScreenGO.transform, "Title",
                "CAÇA-PALAVRAS", 48, TextAlignmentOptions.Center, new Vector2(0, 200));
            var subtitleGO = CreateTextElement(menuScreenGO.transform, "Subtitle",
                "por Ragazzi Studios", 20, TextAlignmentOptions.Center, new Vector2(0, 140));
            var playBtnGO = CreateButton(menuScreenGO.transform, "PlayButton",
                "JOGAR", new Vector2(0, -20));
            var settingsBtnGO = CreateButton(menuScreenGO.transform, "SettingsButton",
                "Configuracoes", new Vector2(0, -100));
            var versionGO = CreateTextElement(menuScreenGO.transform, "VersionText",
                "v0.1.0", 16, TextAlignmentOptions.Bottom, new Vector2(0, -400));

            // MainMenuScreen script (desativado durante wiring)
            menuScreenGO.SetActive(false);
            var mainMenuScript = menuScreenGO.AddComponent<Game.UI.Screens.MainMenuScreen>();

            Wire(mainMenuScript, "_playButton", playBtnGO.GetComponent<Button>());
            Wire(mainMenuScript, "_settingsButton", settingsBtnGO.GetComponent<Button>());
            Wire(mainMenuScript, "_titleText", titleGO.GetComponent<TMP_Text>());
            Wire(mainMenuScript, "_playButtonText",
                playBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            Wire(mainMenuScript, "_settingsButtonText",
                settingsBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            Wire(mainMenuScript, "_versionText", versionGO.GetComponent<TMP_Text>());
            Wire(mainMenuScript, "_popupParent", canvasGO.transform);
            Wire(mainMenuScript, "_canvasGroup", menuCanvasGroup);

            menuScreenGO.SetActive(true);

            // ═══ CategorySelect Screen ═══
            var catScreenGO = CreateScreen(canvasGO.transform, "CategorySelectScreen");

            var catTitleGO = CreateTextElement(catScreenGO.transform, "Title", "CATEGORIAS", 36,
                TextAlignmentOptions.Center, new Vector2(0, 380));
            var catBackBtnGO = CreateButton(catScreenGO.transform, "BackButton",
                "<", new Vector2(-300, 380));
            SetButtonSize(catBackBtnGO, new Vector2(80, 60));

            var catGrid = new GameObject("CategoryGrid");
            catGrid.transform.SetParent(catScreenGO.transform, false);
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

            // CategoryButtonItem prefab template
            var catButtonPrefab = CreateCategoryButtonPrefab(catScreenGO.transform);

            catScreenGO.SetActive(false);
            var catScript = catScreenGO.AddComponent<Game.UI.Screens.CategorySelectScreen>();
            Wire(catScript, "_categoryContainer", catGrid.transform);
            Wire(catScript, "_categoryButtonPrefab", catButtonPrefab);
            Wire(catScript, "_headerText", catTitleGO.GetComponent<TMP_Text>());
            Wire(catScript, "_backButton", catBackBtnGO.GetComponent<Button>());

            // ═══ LevelSelect Screen ═══
            var lvlScreenGO = CreateScreen(canvasGO.transform, "LevelSelectScreen");

            var lvlTitleGO = CreateTextElement(lvlScreenGO.transform, "Title",
                "NIVEIS", 36, TextAlignmentOptions.Center, new Vector2(0, 380));
            var lvlCatNameGO = CreateTextElement(lvlScreenGO.transform, "CategoryName",
                "", 24, TextAlignmentOptions.Center, new Vector2(0, 330));
            var lvlBackBtnGO = CreateButton(lvlScreenGO.transform, "BackButton",
                "<", new Vector2(-300, 380));
            SetButtonSize(lvlBackBtnGO, new Vector2(80, 60));

            var lvlGrid = new GameObject("LevelGrid");
            lvlGrid.transform.SetParent(lvlScreenGO.transform, false);
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

            // LevelButtonItem prefab template
            var lvlButtonPrefab = CreateLevelButtonPrefab(lvlScreenGO.transform);

            lvlScreenGO.SetActive(false);
            var lvlScript = lvlScreenGO.AddComponent<Game.UI.Screens.LevelSelectScreen>();
            Wire(lvlScript, "_levelContainer", lvlGrid.transform);
            Wire(lvlScript, "_levelButtonPrefab", lvlButtonPrefab);
            Wire(lvlScript, "_headerText", lvlTitleGO.GetComponent<TMP_Text>());
            Wire(lvlScript, "_categoryNameText", lvlCatNameGO.GetComponent<TMP_Text>());
            Wire(lvlScript, "_backButton", lvlBackBtnGO.GetComponent<Button>());

            // ═══ Settings Popup (hidden) ═══
            var settingsGO = CreateScreen(canvasGO.transform, "SettingsPopup");
            var settingsCG = settingsGO.AddComponent<CanvasGroup>();

            var settingsPanel = new GameObject("PopupPanel");
            settingsPanel.transform.SetParent(settingsGO.transform, false);
            var settingsPanelImg = settingsPanel.AddComponent<Image>();
            settingsPanelImg.color = new Color(0.2f, 0.22f, 0.35f, 1f);
            var settingsPanelRect = settingsPanel.GetComponent<RectTransform>();
            settingsPanelRect.anchorMin = new Vector2(0.1f, 0.2f);
            settingsPanelRect.anchorMax = new Vector2(0.9f, 0.8f);
            settingsPanelRect.sizeDelta = Vector2.zero;

            var settTitleGO = CreateTextElement(settingsPanel.transform, "Title",
                "CONFIGURACOES", 30, TextAlignmentOptions.Center, new Vector2(0, 200));
            var settSoundLabelGO = CreateTextElement(settingsPanel.transform, "SoundLabel",
                "Som", 22, TextAlignmentOptions.MidlineLeft, new Vector2(-100, 80));
            var settMusicLabelGO = CreateTextElement(settingsPanel.transform, "MusicLabel",
                "Musica", 22, TextAlignmentOptions.MidlineLeft, new Vector2(-100, 20));
            var settLangLabelGO = CreateTextElement(settingsPanel.transform, "LanguageLabel",
                "Idioma", 22, TextAlignmentOptions.MidlineLeft, new Vector2(-100, -40));

            var soundToggleGO = CreateToggle(settingsPanel.transform, "SoundToggle", new Vector2(150, 80));
            var musicToggleGO = CreateToggle(settingsPanel.transform, "MusicToggle", new Vector2(150, 20));
            var langDropdownGO = CreateDropdown(settingsPanel.transform, "LanguageDropdown",
                new Vector2(100, -40));
            var settCloseBtnGO = CreateButton(settingsPanel.transform, "CloseButton",
                "X", new Vector2(280, 200));
            SetButtonSize(settCloseBtnGO, new Vector2(60, 60));

            settingsGO.SetActive(false);
            var settScript = settingsGO.AddComponent<Game.UI.Popups.SettingsPopup>();
            Wire(settScript, "_titleText", settTitleGO.GetComponent<TMP_Text>());
            Wire(settScript, "_soundLabel", settSoundLabelGO.GetComponent<TMP_Text>());
            Wire(settScript, "_musicLabel", settMusicLabelGO.GetComponent<TMP_Text>());
            Wire(settScript, "_languageLabel", settLangLabelGO.GetComponent<TMP_Text>());
            Wire(settScript, "_soundToggle", soundToggleGO.GetComponent<Toggle>());
            Wire(settScript, "_musicToggle", musicToggleGO.GetComponent<Toggle>());
            Wire(settScript, "_languageDropdown", langDropdownGO.GetComponent<TMP_Dropdown>());
            Wire(settScript, "_closeButton", settCloseBtnGO.GetComponent<Button>());
            Wire(settScript, "_canvasGroup", settingsCG);
            Wire(settScript, "_popupPanel", settingsPanelRect);

            // Wire SettingsPopup reference in MainMenuScreen
            Wire(mainMenuScript, "_settingsPopupPrefab", settingsGO);

            // ═══ NavigationController ═══
            var navGO = new GameObject("NavigationController");
            var navScript = navGO.AddComponent<Game.UI.Screens.NavigationController>();
            Wire(navScript, "_mainMenuScreen", menuScreenGO);
            Wire(navScript, "_categorySelectScreen", catScreenGO);
            Wire(navScript, "_levelSelectScreen", lvlScreenGO);

            SaveScene(scene, "MainMenu");
        }

        // ═══════════════════════════════════════════════════
        //  Game Scene
        // ═══════════════════════════════════════════════════

        private static void CreateGameScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // --- Camera ---
            var camGO = new GameObject("Main Camera");
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5;
            cam.backgroundColor = new Color(0.12f, 0.15f, 0.25f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            camGO.tag = "MainCamera";

            // --- Canvas ---
            var canvasGO = CreateCanvas("GameCanvas");

            // ═══ Header ═══
            var header = new GameObject("Header");
            header.transform.SetParent(canvasGO.transform, false);
            var headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 0.9f);
            headerRect.anchorMax = Vector2.one;
            headerRect.sizeDelta = Vector2.zero;

            var catTitleGO = CreateTextElement(header.transform, "CategoryTitle",
                "ANIMAIS", 24, TextAlignmentOptions.Center, Vector2.zero);
            var levelTextGO = CreateTextElement(header.transform, "LevelText",
                "Nivel 1", 18, TextAlignmentOptions.Center, new Vector2(0, -30));
            var progressTextGO = CreateTextElement(header.transform, "ProgressText",
                "0/5", 18, TextAlignmentOptions.Right, new Vector2(300, -30));
            var backBtnGO = CreateButton(header.transform, "BackButton",
                "<", new Vector2(-400, 0));
            SetButtonSize(backBtnGO, new Vector2(70, 50));
            var hintBtnGO = CreateButton(header.transform, "HintButton",
                "Dica", new Vector2(400, 0));
            SetButtonSize(hintBtnGO, new Vector2(100, 50));
            var pauseBtnGO = CreateButton(header.transform, "PauseButton",
                "||", new Vector2(330, 0));
            SetButtonSize(pauseBtnGO, new Vector2(70, 50));

            // ═══ GridView ═══
            var gridViewGO = new GameObject("GridView");
            gridViewGO.transform.SetParent(canvasGO.transform, false);
            var gridViewRect = gridViewGO.AddComponent<RectTransform>();
            gridViewRect.anchorMin = new Vector2(0.02f, 0.25f);
            gridViewRect.anchorMax = new Vector2(0.98f, 0.88f);
            gridViewRect.sizeDelta = Vector2.zero;

            var gridContainer = new GameObject("GridContainer");
            gridContainer.transform.SetParent(gridViewGO.transform, false);
            var gridLayout = gridContainer.AddComponent<GridLayoutGroup>();
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
            var gridContainerRect = gridContainer.GetComponent<RectTransform>();
            gridContainerRect.anchorMin = Vector2.zero;
            gridContainerRect.anchorMax = Vector2.one;
            gridContainerRect.sizeDelta = Vector2.zero;

            // LetterCell prefab
            var letterCellPrefab = CreateLetterCellPrefab(gridViewGO.transform);

            // GridView script
            gridViewGO.SetActive(false);
            var gridViewScript = gridViewGO.AddComponent<Game.UI.Components.GridView>();
            Wire(gridViewScript, "_letterCellPrefab", letterCellPrefab);
            Wire(gridViewScript, "_gridLayout", gridLayout);
            Wire(gridViewScript, "_gridContainer", gridViewRect);
            gridViewGO.SetActive(true);

            // ═══ SelectionLine ═══
            var selectionLineGO = new GameObject("SelectionLine");
            selectionLineGO.transform.SetParent(canvasGO.transform, false);
            var slRect = selectionLineGO.AddComponent<RectTransform>();
            slRect.anchorMin = Vector2.zero;
            slRect.anchorMax = Vector2.one;
            slRect.sizeDelta = Vector2.zero;
            var slImage = selectionLineGO.AddComponent<Image>();
            slImage.color = new Color(0, 0, 0, 0);
            slImage.raycastTarget = true;

            var lineVisualGO = new GameObject("LineVisual");
            lineVisualGO.transform.SetParent(selectionLineGO.transform, false);
            var lineImage = lineVisualGO.AddComponent<Image>();
            lineImage.color = new Color(0.4f, 0.7f, 1f, 0.6f);
            lineImage.raycastTarget = false;
            var lineVisualRect = lineVisualGO.GetComponent<RectTransform>();
            lineVisualRect.sizeDelta = new Vector2(0, 40);
            lineVisualGO.SetActive(false);

            selectionLineGO.SetActive(false);
            var selLineScript = selectionLineGO.AddComponent<Game.UI.Components.SelectionLine>();
            Wire(selLineScript, "_gridView", gridViewScript);
            Wire(selLineScript, "_lineVisual", lineVisualRect);
            Wire(selLineScript, "_lineImage", lineImage);
            selectionLineGO.SetActive(true);

            // ═══ WordListView ═══
            var wordListGO = new GameObject("WordListView");
            wordListGO.transform.SetParent(canvasGO.transform, false);
            var wlRect = wordListGO.AddComponent<RectTransform>();
            wlRect.anchorMin = new Vector2(0.05f, 0.02f);
            wlRect.anchorMax = new Vector2(0.95f, 0.23f);
            wlRect.sizeDelta = Vector2.zero;

            var wordGrid = new GameObject("WordGrid");
            wordGrid.transform.SetParent(wordListGO.transform, false);
            var wgLayout = wordGrid.AddComponent<GridLayoutGroup>();
            wgLayout.cellSize = new Vector2(200, 30);
            wgLayout.spacing = new Vector2(10, 5);
            wgLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            wgLayout.constraintCount = 3;
            var wgRect = wordGrid.GetComponent<RectTransform>();
            wgRect.anchorMin = Vector2.zero;
            wgRect.anchorMax = Vector2.one;
            wgRect.sizeDelta = Vector2.zero;

            var wordItemPrefab = CreateWordListItemPrefab(wordListGO.transform);

            wordListGO.SetActive(false);
            var wordListScript = wordListGO.AddComponent<Game.UI.Components.WordListView>();
            Wire(wordListScript, "_wordContainer", wordGrid.transform);
            Wire(wordListScript, "_wordItemPrefab", wordItemPrefab);
            wordListGO.SetActive(true);

            // ═══ WinPopup (hidden) ═══
            var winPopupGO = CreateScreen(canvasGO.transform, "WinPopup");
            var winBg = winPopupGO.GetComponent<Image>();
            if (winBg != null) winBg.color = new Color(0, 0, 0, 0.7f);
            var winCG = winPopupGO.AddComponent<CanvasGroup>();

            var winPanel = new GameObject("PopupPanel");
            winPanel.transform.SetParent(winPopupGO.transform, false);
            var winPanelImg = winPanel.AddComponent<Image>();
            winPanelImg.color = new Color(0.2f, 0.22f, 0.35f, 1f);
            var winPanelRect = winPanel.GetComponent<RectTransform>();
            winPanelRect.anchorMin = new Vector2(0.1f, 0.25f);
            winPanelRect.anchorMax = new Vector2(0.9f, 0.75f);
            winPanelRect.sizeDelta = Vector2.zero;

            var winTitleGO = CreateTextElement(winPanel.transform, "Title",
                "PARABENS!", 42, TextAlignmentOptions.Center, new Vector2(0, 150));
            var winMsgGO = CreateTextElement(winPanel.transform, "Message",
                "Nivel Completo!", 24, TextAlignmentOptions.Center, new Vector2(0, 80));
            var winStatsGO = CreateTextElement(winPanel.transform, "Stats",
                "5 palavras - 1:23", 22, TextAlignmentOptions.Center, new Vector2(0, 30));
            var winNextBtnGO = CreateButton(winPanel.transform, "NextButton",
                "Proximo Nivel", new Vector2(0, -50));
            var winLvlBtnGO = CreateButton(winPanel.transform, "LevelSelectButton",
                "Selecionar Nivel", new Vector2(0, -120));

            winPopupGO.SetActive(false);
            var winScript = winPopupGO.AddComponent<Game.UI.Popups.WinPopup>();
            Wire(winScript, "_titleText", winTitleGO.GetComponent<TMP_Text>());
            Wire(winScript, "_messageText", winMsgGO.GetComponent<TMP_Text>());
            Wire(winScript, "_statsText", winStatsGO.GetComponent<TMP_Text>());
            Wire(winScript, "_nextLevelButton", winNextBtnGO.GetComponent<Button>());
            Wire(winScript, "_nextLevelButtonText",
                winNextBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            Wire(winScript, "_levelSelectButton", winLvlBtnGO.GetComponent<Button>());
            Wire(winScript, "_levelSelectButtonText",
                winLvlBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            Wire(winScript, "_canvasGroup", winCG);
            Wire(winScript, "_popupPanel", winPanelRect);

            // ═══ GameplayController ═══
            var gpGO = new GameObject("GameplayController");
            var gpScript = gpGO.AddComponent<Game.UI.GameplayController>();
            Wire(gpScript, "_gridView", gridViewScript);
            Wire(gpScript, "_selectionLine", selLineScript);
            Wire(gpScript, "_wordListView", wordListScript);
            Wire(gpScript, "_categoryText", catTitleGO.GetComponent<TMP_Text>());
            Wire(gpScript, "_levelText", levelTextGO.GetComponent<TMP_Text>());
            Wire(gpScript, "_progressText", progressTextGO.GetComponent<TMP_Text>());
            Wire(gpScript, "_hintButton", hintBtnGO.GetComponent<Button>());
            Wire(gpScript, "_pauseButton", pauseBtnGO.GetComponent<Button>());
            Wire(gpScript, "_backButton", backBtnGO.GetComponent<Button>());
            Wire(gpScript, "_winPopupPrefab", winPopupGO);
            Wire(gpScript, "_popupParent", canvasGO.transform);

            SaveScene(scene, "Game");
        }

        // ═══════════════════════════════════════════════════
        //  Prefab Factories (with scripts and wired fields)
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Cria um CategoryButtonItem como template na cena.
        /// Inclui o script CategoryButtonItem com campos conectados.
        /// </summary>
        private static GameObject CreateCategoryButtonPrefab(Transform parent)
        {
            var go = new GameObject("CategoryButtonPrefab");
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.25f, 0.28f, 0.45f, 1f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = image;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 120);

            // Icon (emoji text)
            var iconGO = new GameObject("IconText");
            iconGO.transform.SetParent(go.transform, false);
            var iconTMP = iconGO.AddComponent<TextMeshProUGUI>();
            iconTMP.text = "";
            iconTMP.fontSize = 32;
            iconTMP.alignment = TextAlignmentOptions.Center;
            iconTMP.color = Color.white;
            iconTMP.raycastTarget = false;
            var iconRect = iconGO.GetComponent<RectTransform>();
            iconRect.anchoredPosition = new Vector2(-100, 0);
            iconRect.sizeDelta = new Vector2(50, 50);

            // Name text
            var nameGO = new GameObject("NameText");
            nameGO.transform.SetParent(go.transform, false);
            var nameTMP = nameGO.AddComponent<TextMeshProUGUI>();
            nameTMP.text = "Categoria";
            nameTMP.fontSize = 22;
            nameTMP.alignment = TextAlignmentOptions.MidlineLeft;
            nameTMP.color = Color.white;
            nameTMP.raycastTarget = false;
            var nameRect = nameGO.GetComponent<RectTransform>();
            nameRect.anchoredPosition = new Vector2(20, 10);
            nameRect.sizeDelta = new Vector2(200, 40);

            // Progress text
            var progressGO = new GameObject("ProgressText");
            progressGO.transform.SetParent(go.transform, false);
            var progressTMP = progressGO.AddComponent<TextMeshProUGUI>();
            progressTMP.text = "0/15";
            progressTMP.fontSize = 16;
            progressTMP.alignment = TextAlignmentOptions.MidlineLeft;
            progressTMP.color = new Color(0.7f, 0.7f, 0.7f);
            progressTMP.raycastTarget = false;
            var progressRect = progressGO.GetComponent<RectTransform>();
            progressRect.anchoredPosition = new Vector2(20, -20);
            progressRect.sizeDelta = new Vector2(200, 30);

            // CategoryButtonItem script
            go.SetActive(false);
            var catItem = go.AddComponent<Game.UI.Screens.CategoryButtonItem>();
            Wire(catItem, "_button", btn);
            Wire(catItem, "_iconText", iconTMP);
            Wire(catItem, "_nameText", nameTMP);
            Wire(catItem, "_progressText", progressTMP);
            // _progressFill is optional (not essential for MVP)

            return go;
        }

        /// <summary>
        /// Cria um LevelButtonItem como template na cena.
        /// Inclui o script LevelButtonItem com campos conectados.
        /// </summary>
        private static GameObject CreateLevelButtonPrefab(Transform parent)
        {
            var go = new GameObject("LevelButtonPrefab");
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = Color.white;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = image;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 120);

            // Number label
            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);
            var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
            labelTMP.text = "1";
            labelTMP.fontSize = 28;
            labelTMP.alignment = TextAlignmentOptions.Center;
            labelTMP.color = new Color(0.15f, 0.15f, 0.15f);
            labelTMP.raycastTarget = false;
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;

            // Completed icon (checkmark)
            var completedGO = new GameObject("CompletedIcon");
            completedGO.transform.SetParent(go.transform, false);
            var completedTMP = completedGO.AddComponent<TextMeshProUGUI>();
            completedTMP.text = "OK";
            completedTMP.fontSize = 14;
            completedTMP.alignment = TextAlignmentOptions.Center;
            completedTMP.color = new Color(0.3f, 0.8f, 0.3f);
            completedTMP.raycastTarget = false;
            var completedRect = completedGO.GetComponent<RectTransform>();
            completedRect.anchorMin = new Vector2(0.5f, 0);
            completedRect.anchorMax = new Vector2(1, 0.3f);
            completedRect.sizeDelta = Vector2.zero;
            completedGO.SetActive(false);

            // Locked icon
            var lockedGO = new GameObject("LockedIcon");
            lockedGO.transform.SetParent(go.transform, false);
            var lockedImg = lockedGO.AddComponent<Image>();
            lockedImg.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            var lockedRect = lockedGO.GetComponent<RectTransform>();
            lockedRect.anchorMin = Vector2.zero;
            lockedRect.anchorMax = Vector2.one;
            lockedRect.sizeDelta = Vector2.zero;
            lockedGO.SetActive(false);

            // LevelButtonItem script
            go.SetActive(false);
            var lvlItem = go.AddComponent<Game.UI.Screens.LevelButtonItem>();
            Wire(lvlItem, "_button", btn);
            Wire(lvlItem, "_numberText", labelTMP);
            Wire(lvlItem, "_backgroundImage", image);
            Wire(lvlItem, "_completedIcon", completedGO);
            Wire(lvlItem, "_lockedIcon", lockedGO);

            return go;
        }

        /// <summary>
        /// Cria um LetterCell como template na cena.
        /// </summary>
        private static GameObject CreateLetterCellPrefab(Transform parent)
        {
            var go = new GameObject("LetterCellPrefab");
            go.transform.SetParent(parent, false);

            var bgImage = go.AddComponent<Image>();
            bgImage.color = new Color(0.95f, 0.95f, 0.95f);

            var letterGO = new GameObject("Letter");
            letterGO.transform.SetParent(go.transform, false);
            var letterTMP = letterGO.AddComponent<TextMeshProUGUI>();
            letterTMP.text = "A";
            letterTMP.fontSize = 28;
            letterTMP.alignment = TextAlignmentOptions.Center;
            letterTMP.color = new Color(0.15f, 0.15f, 0.15f);
            letterTMP.raycastTarget = false;
            var letterRect = letterGO.GetComponent<RectTransform>();
            letterRect.anchorMin = Vector2.zero;
            letterRect.anchorMax = Vector2.one;
            letterRect.sizeDelta = Vector2.zero;

            go.SetActive(false);
            var cellScript = go.AddComponent<Game.UI.Components.LetterCell>();
            Wire(cellScript, "_letterText", letterTMP);
            Wire(cellScript, "_backgroundImage", bgImage);

            return go;
        }

        /// <summary>
        /// Cria um WordListItem como template na cena.
        /// </summary>
        private static GameObject CreateWordListItemPrefab(Transform parent)
        {
            var go = new GameObject("WordListItemPrefab");
            go.transform.SetParent(parent, false);

            go.AddComponent<RectTransform>();

            var wordGO = new GameObject("WordText");
            wordGO.transform.SetParent(go.transform, false);
            var wordTMP = wordGO.AddComponent<TextMeshProUGUI>();
            wordTMP.text = "PALAVRA";
            wordTMP.fontSize = 18;
            wordTMP.alignment = TextAlignmentOptions.MidlineLeft;
            wordTMP.color = Color.white;
            wordTMP.raycastTarget = false;
            var wordRect = wordGO.GetComponent<RectTransform>();
            wordRect.anchorMin = Vector2.zero;
            wordRect.anchorMax = Vector2.one;
            wordRect.sizeDelta = Vector2.zero;

            var strikeGO = new GameObject("StrikethroughLine");
            strikeGO.transform.SetParent(go.transform, false);
            var strikeImg = strikeGO.AddComponent<Image>();
            strikeImg.color = new Color(0.3f, 0.8f, 0.3f);
            var strikeRect = strikeGO.GetComponent<RectTransform>();
            strikeRect.anchorMin = new Vector2(0, 0.45f);
            strikeRect.anchorMax = new Vector2(1, 0.55f);
            strikeRect.sizeDelta = Vector2.zero;
            strikeGO.SetActive(false);

            go.SetActive(false);
            var itemScript = go.AddComponent<Game.UI.Components.WordListItem>();
            Wire(itemScript, "_wordText", wordTMP);
            Wire(itemScript, "_strikethroughLine", strikeGO);

            return go;
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

            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                var esGO = new GameObject("EventSystem");
                esGO.AddComponent<EventSystem>();
                esGO.AddComponent<InputSystemUIInputModule>();
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
            tmp.raycastTarget = false;

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
            image.color = new Color(0.29f, 0.56f, 0.89f, 1f);

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 60);
            rect.anchoredPosition = position;

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);

            var tmp = labelGO.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.raycastTarget = false;

            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;

            return go;
        }

        private static void SetButtonSize(GameObject buttonGO, Vector2 size)
        {
            var rect = buttonGO.GetComponent<RectTransform>();
            if (rect != null) rect.sizeDelta = size;
        }

        private static GameObject CreateToggle(Transform parent, string name, Vector2 position)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(60, 30);

            var bgGO = new GameObject("Background");
            bgGO.transform.SetParent(go.transform, false);
            var bgImage = bgGO.AddComponent<Image>();
            bgImage.color = new Color(0.4f, 0.4f, 0.4f);
            var bgRect = bgGO.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            var checkGO = new GameObject("Checkmark");
            checkGO.transform.SetParent(bgGO.transform, false);
            var checkImage = checkGO.AddComponent<Image>();
            checkImage.color = new Color(0.3f, 0.8f, 0.3f);
            var checkRect = checkGO.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0.1f, 0.1f);
            checkRect.anchorMax = new Vector2(0.9f, 0.9f);
            checkRect.sizeDelta = Vector2.zero;

            var toggle = go.AddComponent<Toggle>();
            toggle.isOn = true;
            toggle.targetGraphic = bgImage;
            toggle.graphic = checkImage;

            return go;
        }

        private static GameObject CreateDropdown(Transform parent, string name, Vector2 position)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(200, 40);

            var bgImage = go.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.45f);

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);
            var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
            labelTMP.text = "Portugues (BR)";
            labelTMP.fontSize = 18;
            labelTMP.alignment = TextAlignmentOptions.Center;
            labelTMP.color = Color.white;
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;

            var templateGO = new GameObject("Template");
            templateGO.transform.SetParent(go.transform, false);
            var templateRect = templateGO.AddComponent<RectTransform>();
            templateRect.anchorMin = new Vector2(0, 0);
            templateRect.anchorMax = new Vector2(1, 0);
            templateRect.pivot = new Vector2(0.5f, 1f);
            templateRect.sizeDelta = new Vector2(0, 150);
            var templateImage = templateGO.AddComponent<Image>();
            templateImage.color = new Color(0.25f, 0.25f, 0.4f);
            var scrollRect = templateGO.AddComponent<ScrollRect>();

            var viewportGO = new GameObject("Viewport");
            viewportGO.transform.SetParent(templateGO.transform, false);
            var viewportRect = viewportGO.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;
            var viewportMask = viewportGO.AddComponent<Mask>();
            var viewportImage = viewportGO.AddComponent<Image>();
            viewportImage.color = Color.white;
            viewportMask.showMaskGraphic = false;

            var contentGO = new GameObject("Content");
            contentGO.transform.SetParent(viewportGO.transform, false);
            var contentRect = contentGO.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = Vector2.one;
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.sizeDelta = new Vector2(0, 40);

            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;

            var itemGO = new GameObject("Item");
            itemGO.transform.SetParent(contentGO.transform, false);
            var itemRect = itemGO.AddComponent<RectTransform>();
            itemRect.anchorMin = new Vector2(0, 0.5f);
            itemRect.anchorMax = new Vector2(1, 0.5f);
            itemRect.sizeDelta = new Vector2(0, 40);
            var itemToggle = itemGO.AddComponent<Toggle>();

            var itemBgGO = new GameObject("Item Background");
            itemBgGO.transform.SetParent(itemGO.transform, false);
            var itemBgImage = itemBgGO.AddComponent<Image>();
            itemBgImage.color = new Color(0.3f, 0.3f, 0.45f);
            var itemBgRect = itemBgGO.GetComponent<RectTransform>();
            itemBgRect.anchorMin = Vector2.zero;
            itemBgRect.anchorMax = Vector2.one;
            itemBgRect.sizeDelta = Vector2.zero;

            var itemCheckGO = new GameObject("Item Checkmark");
            itemCheckGO.transform.SetParent(itemBgGO.transform, false);
            var itemCheckImage = itemCheckGO.AddComponent<Image>();
            itemCheckImage.color = new Color(0.4f, 0.7f, 1f);
            var itemCheckRect = itemCheckGO.GetComponent<RectTransform>();
            itemCheckRect.anchorMin = new Vector2(0, 0);
            itemCheckRect.anchorMax = new Vector2(0.1f, 1);
            itemCheckRect.sizeDelta = Vector2.zero;

            var itemLabelGO = new GameObject("Item Label");
            itemLabelGO.transform.SetParent(itemGO.transform, false);
            var itemLabelTMP = itemLabelGO.AddComponent<TextMeshProUGUI>();
            itemLabelTMP.text = "";
            itemLabelTMP.fontSize = 18;
            itemLabelTMP.alignment = TextAlignmentOptions.Center;
            itemLabelTMP.color = Color.white;
            var itemLabelRect = itemLabelGO.GetComponent<RectTransform>();
            itemLabelRect.anchorMin = Vector2.zero;
            itemLabelRect.anchorMax = Vector2.one;
            itemLabelRect.sizeDelta = Vector2.zero;

            itemToggle.targetGraphic = itemBgImage;
            itemToggle.graphic = itemCheckImage;

            templateGO.SetActive(false);

            var dropdown = go.AddComponent<TMP_Dropdown>();
            dropdown.targetGraphic = bgImage;
            dropdown.template = templateRect;
            dropdown.captionText = labelTMP;
            dropdown.itemText = itemLabelTMP;

            return go;
        }

        // ═══════════════════════════════════════════════════
        //  Reflection Wiring (MUST use this instead of SerializedObject)
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Define um campo privado [SerializeField] via C# Reflection.
        /// Isso funciona de forma confiável durante criação de cenas,
        /// diferente de SerializedObject que não persiste neste cenário.
        /// </summary>
        private static void Wire(Component target, string fieldName, object value)
        {
            var type = target.GetType();
            var field = type.GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(target, value);
                EditorUtility.SetDirty(target);
            }
            else
            {
                Debug.LogWarning(
                    $"[SceneCreator] ⚠️ Field '{fieldName}' not found on {type.Name}");
            }
        }

        private static void SaveScene(Scene scene, string name)
        {
            string dir = SCENES_PATH;
            if (!AssetDatabase.IsValidFolder(dir))
            {
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
            Debug.Log($"[SceneCreator] ✅ Cena salva: {path}");
        }
    }
}
