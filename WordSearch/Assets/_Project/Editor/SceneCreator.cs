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
    /// Cria as 3 cenas do jogo (Boot, MainMenu, Game) com hierarquia completa.
    /// Adiciona TODOS os scripts e conecta TODAS as referências automaticamente.
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
            Debug.Log("[SceneCreator] ✅ 3 cenas criadas com scripts conectados!");
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
            gmGO.AddComponent<Core.Application.GameManager>();

            // --- BootLoader ---
            var bootGO = new GameObject("BootLoader");
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
            SetPrivateField(bootLoader, "_loadingCanvasGroup", loadingCG);

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
                "⚙ CONFIGURAÇÕES", new Vector2(0, -100));

            // Version text
            var versionGO = CreateTextElement(menuScreenGO.transform, "VersionText",
                "v0.1.0", 16, TextAlignmentOptions.Bottom, new Vector2(0, -400));

            // MainMenuScreen script
            var mainMenuScript = menuScreenGO.AddComponent<Game.UI.Screens.MainMenuScreen>();
            SetPrivateField(mainMenuScript, "_playButton", playBtnGO.GetComponent<Button>());
            SetPrivateField(mainMenuScript, "_settingsButton", settingsBtnGO.GetComponent<Button>());
            SetPrivateField(mainMenuScript, "_titleText", titleGO.GetComponent<TMP_Text>());
            SetPrivateField(mainMenuScript, "_playButtonText",
                playBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            SetPrivateField(mainMenuScript, "_versionText", versionGO.GetComponent<TMP_Text>());
            SetPrivateField(mainMenuScript, "_popupParent", canvasGO.transform);
            SetPrivateField(mainMenuScript, "_canvasGroup", menuCanvasGroup);

            // ═══ CategorySelect Screen ═══
            var catScreenGO = CreateScreen(canvasGO.transform, "CategorySelectScreen");
            catScreenGO.SetActive(false);

            var catTitleGO = CreateTextElement(catScreenGO.transform, "Title", "CATEGORIAS", 36,
                TextAlignmentOptions.Center, new Vector2(0, 380));
            var catBackBtnGO = CreateButton(catScreenGO.transform, "BackButton",
                "←", new Vector2(-300, 380));
            SetButtonSize(catBackBtnGO, new Vector2(80, 60));

            var catHeaderTMP = catTitleGO.GetComponent<TMP_Text>();

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

            // CategoryButtonItem prefab placeholder
            var catButtonPrefab = CreateCategoryButtonPrefab();
            catButtonPrefab.transform.SetParent(catScreenGO.transform, false);

            var catScript = catScreenGO.AddComponent<Game.UI.Screens.CategorySelectScreen>();
            SetPrivateField(catScript, "_categoryContainer", catGrid.transform);
            SetPrivateField(catScript, "_categoryButtonPrefab", catButtonPrefab);
            SetPrivateField(catScript, "_headerText", catHeaderTMP);
            SetPrivateField(catScript, "_backButton", catBackBtnGO.GetComponent<Button>());

            // ═══ LevelSelect Screen ═══
            var lvlScreenGO = CreateScreen(canvasGO.transform, "LevelSelectScreen");
            lvlScreenGO.SetActive(false);

            var lvlTitleGO = CreateTextElement(lvlScreenGO.transform, "Title",
                "NÍVEIS", 36, TextAlignmentOptions.Center, new Vector2(0, 380));
            var lvlCatNameGO = CreateTextElement(lvlScreenGO.transform, "CategoryName",
                "", 24, TextAlignmentOptions.Center, new Vector2(0, 330));
            var lvlBackBtnGO = CreateButton(lvlScreenGO.transform, "BackButton",
                "←", new Vector2(-300, 380));
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

            // LevelButtonItem prefab placeholder
            var lvlButtonPrefab = CreateLevelButtonPrefab();
            lvlButtonPrefab.transform.SetParent(lvlScreenGO.transform, false);

            var lvlScript = lvlScreenGO.AddComponent<Game.UI.Screens.LevelSelectScreen>();
            SetPrivateField(lvlScript, "_levelContainer", lvlGrid.transform);
            SetPrivateField(lvlScript, "_levelButtonPrefab", lvlButtonPrefab);
            SetPrivateField(lvlScript, "_headerText", lvlTitleGO.GetComponent<TMP_Text>());
            SetPrivateField(lvlScript, "_categoryNameText", lvlCatNameGO.GetComponent<TMP_Text>());
            SetPrivateField(lvlScript, "_backButton", lvlBackBtnGO.GetComponent<Button>());

            // ═══ Settings Popup (hidden) ═══
            var settingsGO = CreateScreen(canvasGO.transform, "SettingsPopup");
            settingsGO.SetActive(false);
            var settingsCG = settingsGO.AddComponent<CanvasGroup>();

            // Popup panel interior
            var settingsPanel = new GameObject("PopupPanel");
            settingsPanel.transform.SetParent(settingsGO.transform, false);
            var settingsPanelImg = settingsPanel.AddComponent<Image>();
            settingsPanelImg.color = new Color(0.2f, 0.22f, 0.35f, 1f);
            var settingsPanelRect = settingsPanel.GetComponent<RectTransform>();
            settingsPanelRect.anchorMin = new Vector2(0.1f, 0.2f);
            settingsPanelRect.anchorMax = new Vector2(0.9f, 0.8f);
            settingsPanelRect.sizeDelta = Vector2.zero;

            var settTitleGO = CreateTextElement(settingsPanel.transform, "Title",
                "CONFIGURAÇÕES", 30, TextAlignmentOptions.Center, new Vector2(0, 200));
            var settSoundLabelGO = CreateTextElement(settingsPanel.transform, "SoundLabel",
                "Som", 22, TextAlignmentOptions.MidlineLeft, new Vector2(-100, 80));
            var settMusicLabelGO = CreateTextElement(settingsPanel.transform, "MusicLabel",
                "Música", 22, TextAlignmentOptions.MidlineLeft, new Vector2(-100, 20));
            var settLangLabelGO = CreateTextElement(settingsPanel.transform, "LanguageLabel",
                "Idioma", 22, TextAlignmentOptions.MidlineLeft, new Vector2(-100, -40));

            // Toggles
            var soundToggleGO = CreateToggle(settingsPanel.transform, "SoundToggle", new Vector2(150, 80));
            var musicToggleGO = CreateToggle(settingsPanel.transform, "MusicToggle", new Vector2(150, 20));

            // Dropdown
            var langDropdownGO = CreateDropdown(settingsPanel.transform, "LanguageDropdown",
                new Vector2(100, -40));

            var settCloseBtnGO = CreateButton(settingsPanel.transform, "CloseButton",
                "✕", new Vector2(280, 200));
            SetButtonSize(settCloseBtnGO, new Vector2(60, 60));

            var settScript = settingsGO.AddComponent<Game.UI.Popups.SettingsPopup>();
            SetPrivateField(settScript, "_titleText", settTitleGO.GetComponent<TMP_Text>());
            SetPrivateField(settScript, "_soundLabel", settSoundLabelGO.GetComponent<TMP_Text>());
            SetPrivateField(settScript, "_musicLabel", settMusicLabelGO.GetComponent<TMP_Text>());
            SetPrivateField(settScript, "_languageLabel", settLangLabelGO.GetComponent<TMP_Text>());
            SetPrivateField(settScript, "_soundToggle", soundToggleGO.GetComponent<Toggle>());
            SetPrivateField(settScript, "_musicToggle", musicToggleGO.GetComponent<Toggle>());
            SetPrivateField(settScript, "_languageDropdown", langDropdownGO.GetComponent<TMP_Dropdown>());
            SetPrivateField(settScript, "_closeButton", settCloseBtnGO.GetComponent<Button>());
            SetPrivateField(settScript, "_canvasGroup", settingsCG);
            SetPrivateField(settScript, "_popupPanel", settingsPanelRect);

            // Wire SettingsPopup reference in MainMenuScreen
            SetPrivateField(mainMenuScript, "_settingsPopupPrefab", settingsGO);

            // ═══ NavigationController ═══
            var navGO = new GameObject("NavigationController");
            var navScript = navGO.AddComponent<Game.UI.Screens.NavigationController>();
            SetPrivateField(navScript, "_mainMenuScreen", menuScreenGO);
            SetPrivateField(navScript, "_categorySelectScreen", catScreenGO);
            SetPrivateField(navScript, "_levelSelectScreen", lvlScreenGO);

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
                "Nível 1", 18, TextAlignmentOptions.Center, new Vector2(0, -30));
            var progressTextGO = CreateTextElement(header.transform, "ProgressText",
                "0/5", 18, TextAlignmentOptions.Right, new Vector2(300, -30));
            var backBtnGO = CreateButton(header.transform, "BackButton",
                "←", new Vector2(-400, 0));
            SetButtonSize(backBtnGO, new Vector2(70, 50));
            var hintBtnGO = CreateButton(header.transform, "HintButton",
                "💡", new Vector2(400, 0));
            SetButtonSize(hintBtnGO, new Vector2(70, 50));
            var pauseBtnGO = CreateButton(header.transform, "PauseButton",
                "⏸", new Vector2(330, 0));
            SetButtonSize(pauseBtnGO, new Vector2(70, 50));

            // ═══ GridView ═══
            var gridViewGO = new GameObject("GridView");
            gridViewGO.transform.SetParent(canvasGO.transform, false);
            var gridViewRect = gridViewGO.AddComponent<RectTransform>();
            gridViewRect.anchorMin = new Vector2(0.02f, 0.25f);
            gridViewRect.anchorMax = new Vector2(0.98f, 0.88f);
            gridViewRect.sizeDelta = Vector2.zero;

            // Grid container with GridLayoutGroup
            var gridContainer = new GameObject("GridContainer");
            gridContainer.transform.SetParent(gridViewGO.transform, false);
            var gridLayout = gridContainer.AddComponent<GridLayoutGroup>();
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
            var gridContainerRect = gridContainer.GetComponent<RectTransform>();
            gridContainerRect.anchorMin = Vector2.zero;
            gridContainerRect.anchorMax = Vector2.one;
            gridContainerRect.sizeDelta = Vector2.zero;

            // LetterCell prefab
            var letterCellPrefab = CreateLetterCellPrefab();
            letterCellPrefab.transform.SetParent(gridViewGO.transform, false);

            // GridView script
            var gridViewScript = gridViewGO.AddComponent<Game.UI.Components.GridView>();
            SetPrivateField(gridViewScript, "_letterCellPrefab", letterCellPrefab);
            SetPrivateField(gridViewScript, "_gridLayout", gridLayout);
            SetPrivateField(gridViewScript, "_gridContainer", gridViewRect);

            // ═══ SelectionLine ═══
            var selectionLineGO = new GameObject("SelectionLine");
            selectionLineGO.transform.SetParent(canvasGO.transform, false);
            var slRect = selectionLineGO.AddComponent<RectTransform>();
            slRect.anchorMin = Vector2.zero;
            slRect.anchorMax = Vector2.one;
            slRect.sizeDelta = Vector2.zero;
            // Transparent image for raycasting
            var slImage = selectionLineGO.AddComponent<Image>();
            slImage.color = new Color(0, 0, 0, 0);
            slImage.raycastTarget = true;

            // Line visual element
            var lineVisualGO = new GameObject("LineVisual");
            lineVisualGO.transform.SetParent(selectionLineGO.transform, false);
            var lineImage = lineVisualGO.AddComponent<Image>();
            lineImage.color = new Color(0.4f, 0.7f, 1f, 0.6f);
            lineImage.raycastTarget = false;
            var lineVisualRect = lineVisualGO.GetComponent<RectTransform>();
            lineVisualRect.sizeDelta = new Vector2(0, 40);
            lineVisualGO.SetActive(false);

            var selLineScript = selectionLineGO.AddComponent<Game.UI.Components.SelectionLine>();
            SetPrivateField(selLineScript, "_gridView", gridViewScript);
            SetPrivateField(selLineScript, "_lineVisual", lineVisualRect);
            SetPrivateField(selLineScript, "_lineImage", lineImage);

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

            // WordListItem prefab
            var wordItemPrefab = CreateWordListItemPrefab();
            wordItemPrefab.transform.SetParent(wordListGO.transform, false);

            var wordListScript = wordListGO.AddComponent<Game.UI.Components.WordListView>();
            SetPrivateField(wordListScript, "_wordContainer", wordGrid.transform);
            SetPrivateField(wordListScript, "_wordItemPrefab", wordItemPrefab);

            // ═══ WinPopup (hidden) ═══
            var winPopupGO = CreateScreen(canvasGO.transform, "WinPopup");
            winPopupGO.SetActive(false);
            var winBg = winPopupGO.GetComponent<Image>();
            if (winBg != null) winBg.color = new Color(0, 0, 0, 0.7f);
            var winCG = winPopupGO.AddComponent<CanvasGroup>();

            // Popup panel
            var winPanel = new GameObject("PopupPanel");
            winPanel.transform.SetParent(winPopupGO.transform, false);
            var winPanelImg = winPanel.AddComponent<Image>();
            winPanelImg.color = new Color(0.2f, 0.22f, 0.35f, 1f);
            var winPanelRect = winPanel.GetComponent<RectTransform>();
            winPanelRect.anchorMin = new Vector2(0.1f, 0.25f);
            winPanelRect.anchorMax = new Vector2(0.9f, 0.75f);
            winPanelRect.sizeDelta = Vector2.zero;

            var winTitleGO = CreateTextElement(winPanel.transform, "Title",
                "🎉 PARABÉNS!", 42, TextAlignmentOptions.Center, new Vector2(0, 150));
            var winMsgGO = CreateTextElement(winPanel.transform, "Message",
                "Nível Completo!", 24, TextAlignmentOptions.Center, new Vector2(0, 80));
            var winStatsGO = CreateTextElement(winPanel.transform, "Stats",
                "5 palavras • 1:23", 22, TextAlignmentOptions.Center, new Vector2(0, 30));
            var winNextBtnGO = CreateButton(winPanel.transform, "NextButton",
                "PRÓXIMO NÍVEL", new Vector2(0, -50));
            var winLvlBtnGO = CreateButton(winPanel.transform, "LevelSelectButton",
                "SELECIONAR NÍVEL", new Vector2(0, -120));

            var winScript = winPopupGO.AddComponent<Game.UI.Popups.WinPopup>();
            SetPrivateField(winScript, "_titleText", winTitleGO.GetComponent<TMP_Text>());
            SetPrivateField(winScript, "_messageText", winMsgGO.GetComponent<TMP_Text>());
            SetPrivateField(winScript, "_statsText", winStatsGO.GetComponent<TMP_Text>());
            SetPrivateField(winScript, "_nextLevelButton", winNextBtnGO.GetComponent<Button>());
            SetPrivateField(winScript, "_nextLevelButtonText",
                winNextBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            SetPrivateField(winScript, "_levelSelectButton", winLvlBtnGO.GetComponent<Button>());
            SetPrivateField(winScript, "_levelSelectButtonText",
                winLvlBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            SetPrivateField(winScript, "_canvasGroup", winCG);
            SetPrivateField(winScript, "_popupPanel", winPanelRect);

            // ═══ GameplayController ═══
            var gpGO = new GameObject("GameplayController");
            var gpScript = gpGO.AddComponent<Game.UI.GameplayController>();
            SetPrivateField(gpScript, "_gridView", gridViewScript);
            SetPrivateField(gpScript, "_selectionLine", selLineScript);
            SetPrivateField(gpScript, "_wordListView", wordListScript);
            SetPrivateField(gpScript, "_categoryText", catTitleGO.GetComponent<TMP_Text>());
            SetPrivateField(gpScript, "_levelText", levelTextGO.GetComponent<TMP_Text>());
            SetPrivateField(gpScript, "_progressText", progressTextGO.GetComponent<TMP_Text>());
            SetPrivateField(gpScript, "_hintButton", hintBtnGO.GetComponent<Button>());
            SetPrivateField(gpScript, "_pauseButton", pauseBtnGO.GetComponent<Button>());
            SetPrivateField(gpScript, "_backButton", backBtnGO.GetComponent<Button>());
            SetPrivateField(gpScript, "_winPopupPrefab", winPopupGO);
            SetPrivateField(gpScript, "_popupParent", canvasGO.transform);

            SaveScene(scene, "Game");
        }

        // ═══════════════════════════════════════════════════
        //  Prefab Factories (runtime scene objects as templates)
        // ═══════════════════════════════════════════════════

        private static GameObject CreateCategoryButtonPrefab()
        {
            var go = new GameObject("CategoryButtonPrefab");
            go.SetActive(false);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.25f, 0.28f, 0.45f, 1f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = image;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 120);

            // Icon placeholder
            var iconGO = new GameObject("Icon");
            iconGO.transform.SetParent(go.transform, false);
            var iconImg = iconGO.AddComponent<Image>();
            iconImg.color = Color.white;
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
            var progressRect = progressGO.GetComponent<RectTransform>();
            progressRect.anchoredPosition = new Vector2(20, -20);
            progressRect.sizeDelta = new Vector2(200, 30);

            return go;
        }

        private static GameObject CreateLevelButtonPrefab()
        {
            var go = new GameObject("LevelButtonPrefab");
            go.SetActive(false);

            var image = go.AddComponent<Image>();
            image.color = Color.white;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = image;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 120);

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);
            var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
            labelTMP.text = "1";
            labelTMP.fontSize = 28;
            labelTMP.alignment = TextAlignmentOptions.Center;
            labelTMP.color = new Color(0.15f, 0.15f, 0.15f);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;

            return go;
        }

        private static GameObject CreateLetterCellPrefab()
        {
            var go = new GameObject("LetterCellPrefab");
            go.SetActive(false);

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

            var cellScript = go.AddComponent<Game.UI.Components.LetterCell>();
            SetPrivateField(cellScript, "_letterText", letterTMP);
            SetPrivateField(cellScript, "_backgroundImage", bgImage);

            return go;
        }

        private static GameObject CreateWordListItemPrefab()
        {
            var go = new GameObject("WordListItemPrefab");
            go.SetActive(false);

            var wordGO = new GameObject("WordText");
            wordGO.transform.SetParent(go.transform, false);
            var wordTMP = wordGO.AddComponent<TextMeshProUGUI>();
            wordTMP.text = "PALAVRA";
            wordTMP.fontSize = 18;
            wordTMP.alignment = TextAlignmentOptions.MidlineLeft;
            wordTMP.color = Color.white;
            var wordRect = wordGO.GetComponent<RectTransform>();
            wordRect.anchorMin = Vector2.zero;
            wordRect.anchorMax = Vector2.one;
            wordRect.sizeDelta = Vector2.zero;

            // Strikethrough line
            var strikeGO = new GameObject("StrikethroughLine");
            strikeGO.transform.SetParent(go.transform, false);
            var strikeImg = strikeGO.AddComponent<Image>();
            strikeImg.color = new Color(0.3f, 0.8f, 0.3f);
            var strikeRect = strikeGO.GetComponent<RectTransform>();
            strikeRect.anchorMin = new Vector2(0, 0.45f);
            strikeRect.anchorMax = new Vector2(1, 0.55f);
            strikeRect.sizeDelta = Vector2.zero;
            strikeGO.SetActive(false);

            var itemScript = go.AddComponent<Game.UI.Components.WordListItem>();
            SetPrivateField(itemScript, "_wordText", wordTMP);
            SetPrivateField(itemScript, "_strikethroughLine", strikeGO);

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

            // Background
            var bgGO = new GameObject("Background");
            bgGO.transform.SetParent(go.transform, false);
            var bgImage = bgGO.AddComponent<Image>();
            bgImage.color = new Color(0.4f, 0.4f, 0.4f);
            var bgRect = bgGO.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            // Checkmark
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

            // Caption label
            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);
            var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
            labelTMP.text = "Português (BR)";
            labelTMP.fontSize = 18;
            labelTMP.alignment = TextAlignmentOptions.Center;
            labelTMP.color = Color.white;
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;

            // Template
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

            // Viewport
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

            // Content
            var contentGO = new GameObject("Content");
            contentGO.transform.SetParent(viewportGO.transform, false);
            var contentRect = contentGO.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = Vector2.one;
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.sizeDelta = new Vector2(0, 40);

            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;

            // Item
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

            // TMP_Dropdown component
            var dropdown = go.AddComponent<TMP_Dropdown>();
            dropdown.targetGraphic = bgImage;
            dropdown.template = templateRect;
            dropdown.captionText = labelTMP;
            dropdown.itemText = itemLabelTMP;

            return go;
        }

        // ═══════════════════════════════════════════════════
        //  Reflection Helper
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Define um campo privado [SerializeField] via SerializedObject.
        /// Funciona como arrastar no Inspector — persiste na cena salva.
        /// </summary>
        private static void SetPrivateField(Object target, string fieldName, Object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop != null)
            {
                prop.objectReferenceValue = value;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            else
            {
                Debug.LogWarning($"[SceneCreator] ⚠️ Field '{fieldName}' not found on {target.GetType().Name}");
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
