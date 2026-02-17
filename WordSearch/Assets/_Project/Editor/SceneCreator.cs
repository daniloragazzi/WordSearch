using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Game.Config;

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

        // Nunito font assets (loaded once per CreateAllScenes call)
        private static TMP_FontAsset _fontRegular;
        private static TMP_FontAsset _fontSemiBold;
        private static TMP_FontAsset _fontBold;
        private static TMP_FontAsset _fontExtraBold;

        // Sprites (loaded once per CreateAllScenes call)
        private static Sprite _btnPrimary;
        private static Sprite _btnCircle;
        private static Sprite _panelPopup;
        private static Sprite _panelCard;
        private static Sprite _cellBg;

        // Theme colors (loaded once per CreateAllScenes call)
        private static Color _colorBackground;
        private static Color _colorPanel;
        private static Color _colorPrimary;
        private static Color _colorTextPrimary;
        private static Color _colorTextSecondary;
        private static Color _colorTextOnColor;
        private static Color _colorOverlay;
        private static Color _colorSuccess;
        private static Color _colorGridBg;
        private static Color _colorTransparent;
        private static Color _colorSelectionLine;
        private static Color _colorLockedOverlay;

        private static void LoadThemeColors()
        {
            var themeGuids = AssetDatabase.FindAssets("t:GameTheme");
            GameTheme theme = null;

            if (themeGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(themeGuids[0]);
                theme = AssetDatabase.LoadAssetAtPath<GameTheme>(path);
            }

            if (theme != null)
            {
                _colorBackground = theme.background;
                _colorPanel = theme.primaryDark;
                _colorPrimary = theme.primary;
                _colorTextPrimary = theme.textPrimary;
                _colorTextSecondary = theme.textSecondary;
                _colorTextOnColor = theme.textOnColor;
                _colorOverlay = theme.overlay;
                _colorSuccess = theme.success;
                _colorGridBg = theme.gridBackground;
                _colorTransparent = new Color(0f, 0f, 0f, 0.01f);
                _colorSelectionLine = new Color(theme.cellSelected.r, theme.cellSelected.g, theme.cellSelected.b, 0.6f);
                _colorLockedOverlay = new Color(theme.textDisabled.r, theme.textDisabled.g, theme.textDisabled.b, 0.5f);
            }
            else
            {
                _colorBackground = new Color(0.12f, 0.15f, 0.25f);
                _colorPanel = new Color(0.2f, 0.22f, 0.35f, 1f);
                _colorPrimary = new Color(0.29f, 0.56f, 0.89f, 1f);
                _colorTextPrimary = new Color(0.15f, 0.15f, 0.15f);
                _colorTextSecondary = new Color(0.7f, 0.7f, 0.7f);
                _colorTextOnColor = Color.white;
                _colorOverlay = new Color(0, 0, 0, 0.7f);
                _colorSuccess = new Color(0.3f, 0.8f, 0.3f);
                _colorGridBg = new Color(0.95f, 0.95f, 0.95f);
                _colorTransparent = new Color(0f, 0f, 0f, 0.01f);
                _colorSelectionLine = new Color(0.4f, 0.7f, 1f, 0.6f);
                _colorLockedOverlay = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }

        [MenuItem("Build/Ragazzi Studios/🎬 Create All Scenes", priority = 1)]
        public static void CreateAllScenes()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            LoadThemeColors();
            LoadFonts();
            LoadSprites();

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
            cam.backgroundColor = _colorBackground;
            cam.clearFlags = CameraClearFlags.SolidColor;
            camGO.tag = "MainCamera";

            // --- GameManager (DontDestroyOnLoad) ---
            var gmGO = new GameObject("GameManager");
            gmGO.SetActive(false); // Desativar antes de AddComponent para evitar Awake/OnEnable prematuro
            gmGO.AddComponent<Core.Application.GameManager>();
            gmGO.SetActive(true);

            // --- MusicManager (DontDestroyOnLoad) ---
            var musicGO = new GameObject("MusicManager");
            musicGO.SetActive(false);
            var musicSource = musicGO.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = 0.3f;
            var musicManager = musicGO.AddComponent<Core.Application.MusicManager>();
            Wire(musicManager, "_musicSource", musicSource);
            var ambientClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/Music/ambient_loop.wav");
            if (ambientClip != null)
                Wire(musicManager, "_ambientLoop", ambientClip);
            musicGO.SetActive(true);

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
            fadeImage.color = _colorBackground;
            var fadeRect = fadeGO.GetComponent<RectTransform>();
            fadeRect.anchorMin = Vector2.zero;
            fadeRect.anchorMax = Vector2.one;
            fadeRect.sizeDelta = Vector2.zero;

            // --- Wiring: BootLoader ---
            Wire(bootLoader, "_loadingCanvasGroup", loadingCG);
            bootGO.SetActive(true);

            ApplyFontsToScene();
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
            cam.backgroundColor = _colorPanel;
            cam.clearFlags = CameraClearFlags.SolidColor;
            camGO.tag = "MainCamera";

            // --- Canvas ---
            var canvasGO = CreateCanvas("UICanvas");

            // ═══ MainMenu Screen ═══
            var menuScreenGO = CreateScreen(canvasGO.transform, "MainMenuScreen");
            var menuCanvasGroup = menuScreenGO.AddComponent<CanvasGroup>();

            // --- Layout container (centralizado verticalmente, anchor-based) ---
            var menuLayout = new GameObject("MenuLayout");
            menuLayout.transform.SetParent(menuScreenGO.transform, false);
            var menuLayoutRect = menuLayout.AddComponent<RectTransform>();
            menuLayoutRect.anchorMin = new Vector2(0.1f, 0.15f);
            menuLayoutRect.anchorMax = new Vector2(0.9f, 0.85f);
            menuLayoutRect.sizeDelta = Vector2.zero;
            var menuVLayout = menuLayout.AddComponent<VerticalLayoutGroup>();
            menuVLayout.childAlignment = TextAnchor.MiddleCenter;
            menuVLayout.spacing = 20;
            menuVLayout.childControlWidth = true;
            menuVLayout.childControlHeight = false;
            menuVLayout.childForceExpandWidth = true;
            menuVLayout.childForceExpandHeight = false;
            menuVLayout.padding = new RectOffset(40, 40, 0, 0);

            // Title
            var titleGO = new GameObject("Title");
            titleGO.transform.SetParent(menuLayout.transform, false);
            var titleTMP = titleGO.AddComponent<TextMeshProUGUI>();
            titleTMP.text = "CAÇA-PALAVRAS";
            titleTMP.fontSize = 48;
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.color = _colorTextOnColor;
            titleTMP.raycastTarget = false;
            var titleLayoutElem = titleGO.AddComponent<LayoutElement>();
            titleLayoutElem.preferredHeight = 70;

            // Subtitle
            var subtitleGO = new GameObject("Subtitle");
            subtitleGO.transform.SetParent(menuLayout.transform, false);
            var subtitleTMP = subtitleGO.AddComponent<TextMeshProUGUI>();
            subtitleTMP.text = "por Ragazzi Studios";
            subtitleTMP.fontSize = 20;
            subtitleTMP.alignment = TextAlignmentOptions.Center;
            subtitleTMP.color = _colorTextSecondary;
            subtitleTMP.raycastTarget = false;
            var subtitleLayoutElem = subtitleGO.AddComponent<LayoutElement>();
            subtitleLayoutElem.preferredHeight = 40;

            // Spacer
            var spacer1 = new GameObject("Spacer");
            spacer1.transform.SetParent(menuLayout.transform, false);
            spacer1.AddComponent<RectTransform>();
            var spacerLayout1 = spacer1.AddComponent<LayoutElement>();
            spacerLayout1.preferredHeight = 40;

            // Play button
            var playBtnGO = CreateButton(menuLayout.transform, "PlayButton",
                "JOGAR", Vector2.zero);
            SetButtonSize(playBtnGO, new Vector2(0, 70));
            var playLE = playBtnGO.AddComponent<LayoutElement>();
            playLE.preferredHeight = 70;
            playLE.flexibleWidth = 1;

            // Challenge button
            var challengeBtnGO = CreateButton(menuLayout.transform, "ChallengeButton",
                "DESAFIO", Vector2.zero);
            SetButtonSize(challengeBtnGO, new Vector2(0, 70));
            var chalLE = challengeBtnGO.AddComponent<LayoutElement>();
            chalLE.preferredHeight = 70;
            chalLE.flexibleWidth = 1;

            // Settings button
            var settingsBtnGO = CreateButton(menuLayout.transform, "SettingsButton",
                "Configuracoes", Vector2.zero);
            SetButtonSize(settingsBtnGO, new Vector2(0, 70));
            var settLE = settingsBtnGO.AddComponent<LayoutElement>();
            settLE.preferredHeight = 70;
            settLE.flexibleWidth = 1;

            // Version text (fixed at bottom of screen)
            var versionGO = new GameObject("VersionText");
            versionGO.transform.SetParent(menuScreenGO.transform, false);
            var versionTMP = versionGO.AddComponent<TextMeshProUGUI>();
            versionTMP.text = "v0.1.0";
            versionTMP.fontSize = 16;
            versionTMP.alignment = TextAlignmentOptions.Center;
            versionTMP.color = _colorTextSecondary;
            versionTMP.raycastTarget = false;
            var versionRect = versionGO.GetComponent<RectTransform>();
            versionRect.anchorMin = new Vector2(0.2f, 0.02f);
            versionRect.anchorMax = new Vector2(0.8f, 0.07f);
            versionRect.sizeDelta = Vector2.zero;

            // MainMenuScreen script (desativado durante wiring)
            menuScreenGO.SetActive(false);
            var mainMenuScript = menuScreenGO.AddComponent<Game.UI.Screens.MainMenuScreen>();

            Wire(mainMenuScript, "_playButton", playBtnGO.GetComponent<Button>());
            Wire(mainMenuScript, "_challengeButton", challengeBtnGO.GetComponent<Button>());
            Wire(mainMenuScript, "_settingsButton", settingsBtnGO.GetComponent<Button>());
            Wire(mainMenuScript, "_titleText", titleTMP);
            Wire(mainMenuScript, "_playButtonText",
                playBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            Wire(mainMenuScript, "_challengeButtonText",
                challengeBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            Wire(mainMenuScript, "_settingsButtonText",
                settingsBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            Wire(mainMenuScript, "_versionText", versionTMP);
            Wire(mainMenuScript, "_popupParent", canvasGO.transform);
            Wire(mainMenuScript, "_canvasGroup", menuCanvasGroup);

            menuScreenGO.SetActive(true);

            // ═══ CategorySelect Screen ═══
            var catScreenGO = CreateScreen(canvasGO.transform, "CategorySelectScreen");

            // Header bar (top 8%)
            var catHeader = new GameObject("Header");
            catHeader.transform.SetParent(catScreenGO.transform, false);
            var catHeaderRect = catHeader.AddComponent<RectTransform>();
            catHeaderRect.anchorMin = new Vector2(0, 0.92f);
            catHeaderRect.anchorMax = Vector2.one;
            catHeaderRect.sizeDelta = Vector2.zero;

            var catTitleGO = new GameObject("Title");
            catTitleGO.transform.SetParent(catHeader.transform, false);
            var catTitleTMP = catTitleGO.AddComponent<TextMeshProUGUI>();
            catTitleTMP.text = "CATEGORIAS";
            catTitleTMP.fontSize = 32;
            catTitleTMP.alignment = TextAlignmentOptions.Center;
            catTitleTMP.color = _colorTextOnColor;
            catTitleTMP.raycastTarget = false;
            var catTitleRect = catTitleGO.GetComponent<RectTransform>();
            catTitleRect.anchorMin = Vector2.zero;
            catTitleRect.anchorMax = Vector2.one;
            catTitleRect.sizeDelta = Vector2.zero;

            var catBackBtnGO = new GameObject("BackButton");
            catBackBtnGO.transform.SetParent(catHeader.transform, false);
            var catBackImg = catBackBtnGO.AddComponent<Image>();
            catBackImg.color = _colorPrimary;
            ApplySprite(catBackImg, _btnCircle);
            var catBackBtn = catBackBtnGO.AddComponent<Button>();
            catBackBtn.targetGraphic = catBackImg;
            var catBackRect = catBackBtnGO.GetComponent<RectTransform>();
            catBackRect.anchorMin = new Vector2(0, 1);
            catBackRect.anchorMax = new Vector2(0, 1);
            catBackRect.pivot = new Vector2(0, 1);
            catBackRect.anchoredPosition = new Vector2(24, -18);
            catBackRect.sizeDelta = new Vector2(88, 88);
            var catBackLabel = new GameObject("Label");
            catBackLabel.transform.SetParent(catBackBtnGO.transform, false);
            var catBackTMP = catBackLabel.AddComponent<TextMeshProUGUI>();
            catBackTMP.text = "<";
            catBackTMP.fontSize = 28;
            catBackTMP.alignment = TextAlignmentOptions.Center;
            catBackTMP.color = _colorTextOnColor;
            catBackTMP.raycastTarget = false;
            var catBackLabelRect = catBackLabel.GetComponent<RectTransform>();
            catBackLabelRect.anchorMin = Vector2.zero;
            catBackLabelRect.anchorMax = Vector2.one;
            catBackLabelRect.sizeDelta = Vector2.zero;

            // ScrollView for categories (below header)
            var catScrollGO = new GameObject("ScrollView");
            catScrollGO.transform.SetParent(catScreenGO.transform, false);
            var catScrollRect = catScrollGO.AddComponent<RectTransform>();
            catScrollRect.anchorMin = new Vector2(0.03f, 0.02f);
            catScrollRect.anchorMax = new Vector2(0.97f, 0.90f);
            catScrollRect.sizeDelta = Vector2.zero;
            var catScrollView = catScrollGO.AddComponent<ScrollRect>();
            catScrollView.horizontal = false;
            catScrollView.vertical = true;
            var catScrollMask = catScrollGO.AddComponent<UnityEngine.UI.Mask>();
            catScrollMask.showMaskGraphic = false;
            var catScrollImg = catScrollGO.AddComponent<Image>();
            catScrollImg.color = _colorTransparent;

            // Content container inside scroll
            var catGrid = new GameObject("CategoryGrid");
            catGrid.transform.SetParent(catScrollGO.transform, false);
            var catGridLayout = catGrid.AddComponent<GridLayoutGroup>();
            catGridLayout.cellSize = new Vector2(480, 110);
            catGridLayout.spacing = new Vector2(12, 16);
            catGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            catGridLayout.constraintCount = 2;
            catGridLayout.childAlignment = TextAnchor.UpperCenter;
            catGridLayout.padding = new RectOffset(16, 16, 12, 12);
            var catGridRect = catGrid.GetComponent<RectTransform>();
            catGridRect.anchorMin = new Vector2(0, 1);
            catGridRect.anchorMax = new Vector2(1, 1);
            catGridRect.pivot = new Vector2(0.5f, 1);
            catGridRect.sizeDelta = new Vector2(0, 0);
            var catContentSizeFitter = catGrid.AddComponent<ContentSizeFitter>();
            catContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            catScrollView.content = catGridRect;

            // CategoryButtonItem prefab template
            var catButtonPrefab = CreateCategoryButtonPrefab(catScreenGO.transform);

            catScreenGO.SetActive(false);
            var catScript = catScreenGO.AddComponent<Game.UI.Screens.CategorySelectScreen>();
            Wire(catScript, "_categoryContainer", catGrid.transform);
            Wire(catScript, "_categoryButtonPrefab", catButtonPrefab);
            Wire(catScript, "_headerText", catTitleTMP);
            Wire(catScript, "_backButton", catBackBtn);

            // ═══ LevelSelect Screen ═══
            var lvlScreenGO = CreateScreen(canvasGO.transform, "LevelSelectScreen");

            // --- Header area (top 15%) ---
            var lvlHeader = new GameObject("Header");
            lvlHeader.transform.SetParent(lvlScreenGO.transform, false);
            var lvlHeaderRect = lvlHeader.AddComponent<RectTransform>();
            lvlHeaderRect.anchorMin = new Vector2(0f, 0.85f);
            lvlHeaderRect.anchorMax = new Vector2(1f, 1f);
            lvlHeaderRect.offsetMin = Vector2.zero;
            lvlHeaderRect.offsetMax = Vector2.zero;

            // Back button (left side of header)
            var lvlBackBtnGO = new GameObject("BackButton");
            lvlBackBtnGO.transform.SetParent(lvlHeader.transform, false);
            var lvlBackImg = lvlBackBtnGO.AddComponent<Image>();
            lvlBackImg.color = _colorPrimary;
            ApplySprite(lvlBackImg, _btnCircle);
            lvlBackBtnGO.AddComponent<Button>().targetGraphic = lvlBackImg;
            var lvlBackRect = lvlBackBtnGO.GetComponent<RectTransform>();
            lvlBackRect.anchorMin = new Vector2(0f, 1f);
            lvlBackRect.anchorMax = new Vector2(0f, 1f);
            lvlBackRect.pivot = new Vector2(0f, 1f);
            lvlBackRect.anchoredPosition = new Vector2(24f, -24f);
            lvlBackRect.sizeDelta = new Vector2(88f, 88f);
            var lvlBackLabel = new GameObject("Label");
            lvlBackLabel.transform.SetParent(lvlBackBtnGO.transform, false);
            var lvlBackTMP = lvlBackLabel.AddComponent<TextMeshProUGUI>();
            lvlBackTMP.text = "<";
            lvlBackTMP.fontSize = 28;
            lvlBackTMP.alignment = TextAlignmentOptions.Center;
            lvlBackTMP.color = _colorTextOnColor;
            lvlBackTMP.raycastTarget = false;
            var lvlBackLabelRect = lvlBackLabel.GetComponent<RectTransform>();
            lvlBackLabelRect.anchorMin = Vector2.zero;
            lvlBackLabelRect.anchorMax = Vector2.one;
            lvlBackLabelRect.sizeDelta = Vector2.zero;

            // Title "Níveis" (center-top of header)
            var lvlTitleGO = new GameObject("Title");
            lvlTitleGO.transform.SetParent(lvlHeader.transform, false);
            var lvlTitleTMP = lvlTitleGO.AddComponent<TextMeshProUGUI>();
            lvlTitleTMP.text = "Níveis";
            lvlTitleTMP.fontSize = 32;
            lvlTitleTMP.alignment = TextAlignmentOptions.Center;
            lvlTitleTMP.color = _colorTextOnColor;
            lvlTitleTMP.raycastTarget = false;
            var lvlTitleRect = lvlTitleGO.GetComponent<RectTransform>();
            lvlTitleRect.anchorMin = new Vector2(0.15f, 0.5f);
            lvlTitleRect.anchorMax = new Vector2(0.85f, 1f);
            lvlTitleRect.offsetMin = Vector2.zero;
            lvlTitleRect.offsetMax = Vector2.zero;

            // Category name (center-bottom of header)
            var lvlCatNameGO = new GameObject("CategoryName");
            lvlCatNameGO.transform.SetParent(lvlHeader.transform, false);
            var lvlCatNameTMP = lvlCatNameGO.AddComponent<TextMeshProUGUI>();
            lvlCatNameTMP.text = "";
            lvlCatNameTMP.fontSize = 20;
            lvlCatNameTMP.alignment = TextAlignmentOptions.Center;
            lvlCatNameTMP.color = _colorTextSecondary;
            lvlCatNameTMP.raycastTarget = false;
            var lvlCatNameRect = lvlCatNameGO.GetComponent<RectTransform>();
            lvlCatNameRect.anchorMin = new Vector2(0.15f, 0f);
            lvlCatNameRect.anchorMax = new Vector2(0.85f, 0.5f);
            lvlCatNameRect.offsetMin = Vector2.zero;
            lvlCatNameRect.offsetMax = Vector2.zero;

            // --- Level grid (below header) ---
            var lvlGrid = new GameObject("LevelGrid");
            lvlGrid.transform.SetParent(lvlScreenGO.transform, false);
            var lvlGridLayout = lvlGrid.AddComponent<GridLayoutGroup>();
            lvlGridLayout.cellSize = new Vector2(120, 120);
            lvlGridLayout.spacing = new Vector2(15, 15);
            lvlGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            lvlGridLayout.constraintCount = 5;
            lvlGridLayout.childAlignment = TextAnchor.UpperCenter;
            lvlGridLayout.padding = new RectOffset(10, 10, 10, 10);
            var lvlGridRect = lvlGrid.GetComponent<RectTransform>();
            lvlGridRect.anchorMin = new Vector2(0.02f, 0.02f);
            lvlGridRect.anchorMax = new Vector2(0.98f, 0.84f);
            lvlGridRect.offsetMin = Vector2.zero;
            lvlGridRect.offsetMax = Vector2.zero;

            // LevelButtonItem prefab template
            var lvlButtonPrefab = CreateLevelButtonPrefab(lvlScreenGO.transform);

            lvlScreenGO.SetActive(false);
            var lvlScript = lvlScreenGO.AddComponent<Game.UI.Screens.LevelSelectScreen>();
            Wire(lvlScript, "_levelContainer", lvlGrid.transform);
            Wire(lvlScript, "_levelButtonPrefab", lvlButtonPrefab);
            Wire(lvlScript, "_headerText", lvlTitleTMP);
            Wire(lvlScript, "_categoryNameText", lvlCatNameTMP);
            Wire(lvlScript, "_backButton", lvlBackBtnGO.GetComponent<Button>());

            // ═══ Settings Popup (hidden) ═══
            var settingsGO = CreateScreen(canvasGO.transform, "SettingsPopup");
            var settingsCG = settingsGO.AddComponent<CanvasGroup>();

            var settingsPanel = new GameObject("PopupPanel");
            settingsPanel.transform.SetParent(settingsGO.transform, false);
            var settingsPanelImg = settingsPanel.AddComponent<Image>();
            settingsPanelImg.color = _colorPanel;
            ApplySprite(settingsPanelImg, _panelPopup);
            var settingsPanelRect = settingsPanel.GetComponent<RectTransform>();
            settingsPanelRect.anchorMin = new Vector2(0.08f, 0.25f);
            settingsPanelRect.anchorMax = new Vector2(0.92f, 0.75f);
            settingsPanelRect.sizeDelta = Vector2.zero;

            var settTitleGO = CreateTextElement(settingsPanel.transform, "Title",
                "CONFIGURACOES", 30, TextAlignmentOptions.Center, new Vector2(0, 130));
            var settSoundLabelGO = CreateTextElement(settingsPanel.transform, "SoundLabel",
                "Som", 24, TextAlignmentOptions.MidlineLeft, new Vector2(-220, 45));
            var settMusicLabelGO = CreateTextElement(settingsPanel.transform, "MusicLabel",
                "Musica", 24, TextAlignmentOptions.MidlineLeft, new Vector2(-220, -25));
            var settLangLabelGO = CreateTextElement(settingsPanel.transform, "LanguageLabel",
                "Idioma", 24, TextAlignmentOptions.MidlineLeft, new Vector2(-220, -95));

            var settTitleTMP = settTitleGO.GetComponent<TMP_Text>();
            var settSoundTMP = settSoundLabelGO.GetComponent<TMP_Text>();
            var settMusicTMP = settMusicLabelGO.GetComponent<TMP_Text>();
            var settLangTMP = settLangLabelGO.GetComponent<TMP_Text>();
            if (settTitleTMP != null) settTitleTMP.color = _colorTextOnColor;
            if (settSoundTMP != null) settSoundTMP.color = _colorTextOnColor;
            if (settMusicTMP != null) settMusicTMP.color = _colorTextOnColor;
            if (settLangTMP != null) settLangTMP.color = _colorTextOnColor;

            // Anchor-based layout inside settings panel (more stable on tall screens)
            var settTitleRect = settTitleGO.GetComponent<RectTransform>();
            if (settTitleRect != null)
            {
                settTitleRect.anchorMin = new Vector2(0.15f, 0.70f);
                settTitleRect.anchorMax = new Vector2(0.85f, 0.88f);
                settTitleRect.anchoredPosition = Vector2.zero;
                settTitleRect.sizeDelta = Vector2.zero;
            }

            var settSoundRect = settSoundLabelGO.GetComponent<RectTransform>();
            if (settSoundRect != null)
            {
                settSoundRect.anchorMin = new Vector2(0.08f, 0.52f);
                settSoundRect.anchorMax = new Vector2(0.45f, 0.64f);
                settSoundRect.anchoredPosition = Vector2.zero;
                settSoundRect.sizeDelta = Vector2.zero;
            }

            var settMusicRect = settMusicLabelGO.GetComponent<RectTransform>();
            if (settMusicRect != null)
            {
                settMusicRect.anchorMin = new Vector2(0.08f, 0.38f);
                settMusicRect.anchorMax = new Vector2(0.45f, 0.50f);
                settMusicRect.anchoredPosition = Vector2.zero;
                settMusicRect.sizeDelta = Vector2.zero;
            }

            var settLangRect = settLangLabelGO.GetComponent<RectTransform>();
            if (settLangRect != null)
            {
                settLangRect.anchorMin = new Vector2(0.08f, 0.24f);
                settLangRect.anchorMax = new Vector2(0.45f, 0.36f);
                settLangRect.anchoredPosition = Vector2.zero;
                settLangRect.sizeDelta = Vector2.zero;
            }

            var soundToggleGO = CreateToggle(settingsPanel.transform, "SoundToggle", new Vector2(220, 45));
            var musicToggleGO = CreateToggle(settingsPanel.transform, "MusicToggle", new Vector2(220, -25));
            var langDropdownGO = CreateDropdown(settingsPanel.transform, "LanguageDropdown",
                new Vector2(160, -95));
            var settCloseBtnGO = CreateButton(settingsPanel.transform, "CloseButton",
                "X", new Vector2(300, 130));
            SetButtonSize(settCloseBtnGO, new Vector2(72, 72));

            var soundToggleRect = soundToggleGO.GetComponent<RectTransform>();
            if (soundToggleRect != null)
            {
                soundToggleRect.anchorMin = new Vector2(0.68f, 0.52f);
                soundToggleRect.anchorMax = new Vector2(0.86f, 0.64f);
                soundToggleRect.anchoredPosition = Vector2.zero;
                soundToggleRect.sizeDelta = Vector2.zero;
            }

            var musicToggleRect = musicToggleGO.GetComponent<RectTransform>();
            if (musicToggleRect != null)
            {
                musicToggleRect.anchorMin = new Vector2(0.68f, 0.38f);
                musicToggleRect.anchorMax = new Vector2(0.86f, 0.50f);
                musicToggleRect.anchoredPosition = Vector2.zero;
                musicToggleRect.sizeDelta = Vector2.zero;
            }

            var langDropdownRect = langDropdownGO.GetComponent<RectTransform>();
            if (langDropdownRect != null)
            {
                langDropdownRect.anchorMin = new Vector2(0.54f, 0.23f);
                langDropdownRect.anchorMax = new Vector2(0.90f, 0.37f);
                langDropdownRect.anchoredPosition = Vector2.zero;
                langDropdownRect.sizeDelta = Vector2.zero;
            }

            var closeRect = settCloseBtnGO.GetComponent<RectTransform>();
            if (closeRect != null)
            {
                closeRect.anchorMin = new Vector2(0.82f, 0.76f);
                closeRect.anchorMax = new Vector2(0.94f, 0.90f);
                closeRect.anchoredPosition = Vector2.zero;
                closeRect.sizeDelta = Vector2.zero;
            }

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

            // ═══ ChallengeSelect Screen ═══
            var chalScreenGO = CreateScreen(canvasGO.transform, "ChallengeSelectScreen");

            // Header
            var chalHeader = new GameObject("Header");
            chalHeader.transform.SetParent(chalScreenGO.transform, false);
            var chalHeaderRect = chalHeader.AddComponent<RectTransform>();
            chalHeaderRect.anchorMin = new Vector2(0, 0.92f);
            chalHeaderRect.anchorMax = Vector2.one;
            chalHeaderRect.sizeDelta = Vector2.zero;

            var chalTitleGO = new GameObject("Title");
            chalTitleGO.transform.SetParent(chalHeader.transform, false);
            var chalTitleTMP = chalTitleGO.AddComponent<TextMeshProUGUI>();
            chalTitleTMP.text = "DESAFIO";
            chalTitleTMP.fontSize = 32;
            chalTitleTMP.alignment = TextAlignmentOptions.Center;
            chalTitleTMP.color = _colorTextOnColor;
            chalTitleTMP.raycastTarget = false;
            var chalTitleRect = chalTitleGO.GetComponent<RectTransform>();
            chalTitleRect.anchorMin = Vector2.zero;
            chalTitleRect.anchorMax = Vector2.one;
            chalTitleRect.sizeDelta = Vector2.zero;

            var chalBackBtnGO = new GameObject("BackButton");
            chalBackBtnGO.transform.SetParent(chalHeader.transform, false);
            var chalBackImg = chalBackBtnGO.AddComponent<Image>();
            chalBackImg.color = _colorPrimary;
            ApplySprite(chalBackImg, _btnCircle);
            var chalBackBtn = chalBackBtnGO.AddComponent<Button>();
            chalBackBtn.targetGraphic = chalBackImg;
            var chalBackRect = chalBackBtnGO.GetComponent<RectTransform>();
            chalBackRect.anchorMin = new Vector2(0, 1);
            chalBackRect.anchorMax = new Vector2(0, 1);
            chalBackRect.pivot = new Vector2(0, 1);
            chalBackRect.anchoredPosition = new Vector2(24, -18);
            chalBackRect.sizeDelta = new Vector2(88, 88);
            var chalBackLabel = new GameObject("Label");
            chalBackLabel.transform.SetParent(chalBackBtnGO.transform, false);
            var chalBackTMP = chalBackLabel.AddComponent<TextMeshProUGUI>();
            chalBackTMP.text = "<";
            chalBackTMP.fontSize = 28;
            chalBackTMP.alignment = TextAlignmentOptions.Center;
            chalBackTMP.color = _colorTextOnColor;
            chalBackTMP.raycastTarget = false;
            var chalBackLabelRect = chalBackLabel.GetComponent<RectTransform>();
            chalBackLabelRect.anchorMin = Vector2.zero;
            chalBackLabelRect.anchorMax = Vector2.one;
            chalBackLabelRect.sizeDelta = Vector2.zero;

            // Description text
            var chalDescGO = CreateTextElement(chalScreenGO.transform, "Description",
                "Escolha o tamanho do grid\n10 palavras de todas as categorias",
                20, TextAlignmentOptions.Center, new Vector2(0, 250));
            var chalDescTMP = chalDescGO.GetComponent<TMP_Text>();
            if (chalDescTMP != null) chalDescTMP.color = _colorTextOnColor;

            // Challenge buttons (3 sizes: 20 rows, varying cols)
            var chal10GO = CreateButton(chalScreenGO.transform, "Challenge20x10",
                "20 x 10", new Vector2(0, 100));
            SetButtonSize(chal10GO, new Vector2(400, 80));
            var chal10Sub = CreateTextElement(chalScreenGO.transform, "Sub10",
                "Normal", 16, TextAlignmentOptions.Center, new Vector2(0, 50));
            var chal10SubTMP = chal10Sub.GetComponent<TMP_Text>();
            if (chal10SubTMP != null) chal10SubTMP.color = _colorTextOnColor;

            var chal14GO = CreateButton(chalScreenGO.transform, "Challenge20x14",
                "20 x 14", new Vector2(0, -30));
            SetButtonSize(chal14GO, new Vector2(400, 80));
            var chal14Sub = CreateTextElement(chalScreenGO.transform, "Sub14",
                "Dificil", 16, TextAlignmentOptions.Center, new Vector2(0, -80));
            var chal14SubTMP = chal14Sub.GetComponent<TMP_Text>();
            if (chal14SubTMP != null) chal14SubTMP.color = _colorTextOnColor;

            var chal16GO = CreateButton(chalScreenGO.transform, "Challenge20x16",
                "20 x 16", new Vector2(0, -160));
            SetButtonSize(chal16GO, new Vector2(400, 80));
            var chal16Sub = CreateTextElement(chalScreenGO.transform, "Sub16",
                "Extremo", 16, TextAlignmentOptions.Center, new Vector2(0, -210));
            var chal16SubTMP = chal16Sub.GetComponent<TMP_Text>();
            if (chal16SubTMP != null) chal16SubTMP.color = _colorTextOnColor;

            chalScreenGO.SetActive(false);
            var chalScript = chalScreenGO.AddComponent<Game.UI.Screens.ChallengeSelectScreen>();
            Wire(chalScript, "_challenge20x10", chal10GO.GetComponent<Button>());
            Wire(chalScript, "_challenge20x14", chal14GO.GetComponent<Button>());
            Wire(chalScript, "_challenge20x16", chal16GO.GetComponent<Button>());
            Wire(chalScript, "_backButton", chalBackBtn);
            Wire(chalScript, "_titleText", chalTitleTMP);

            // ═══ NavigationController ═══
            var navGO = new GameObject("NavigationController");
            var navScript = navGO.AddComponent<Game.UI.Screens.NavigationController>();
            Wire(navScript, "_mainMenuScreen", menuScreenGO);
            Wire(navScript, "_categorySelectScreen", catScreenGO);
            Wire(navScript, "_levelSelectScreen", lvlScreenGO);
            Wire(navScript, "_challengeSelectScreen", chalScreenGO);

            ApplyFontsToScene();
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
            cam.backgroundColor = _colorBackground;
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
            var timerTextGO = CreateTextElement(header.transform, "TimerText",
                "0:00", 16, TextAlignmentOptions.Left, Vector2.zero);

            var catTitleTMP = catTitleGO.GetComponent<TMP_Text>();
            var levelTextTMP = levelTextGO.GetComponent<TMP_Text>();
            var progressTextTMP = progressTextGO.GetComponent<TMP_Text>();
            var timerTextTMP = timerTextGO.GetComponent<TMP_Text>();
            if (catTitleTMP != null) catTitleTMP.color = _colorTextOnColor;
            if (levelTextTMP != null) levelTextTMP.color = _colorTextOnColor;
            if (progressTextTMP != null) progressTextTMP.color = _colorTextOnColor;
            if (timerTextTMP != null) timerTextTMP.color = _colorTextOnColor;

            // Reposition timer: anchor top-left, right next to back button
            var timerRect = timerTextGO.GetComponent<RectTransform>();
            timerRect.anchorMin = new Vector2(0f, 1f);
            timerRect.anchorMax = new Vector2(0f, 1f);
            timerRect.pivot = new Vector2(0f, 1f);
            timerRect.anchoredPosition = new Vector2(120f, -36f);
            timerRect.sizeDelta = new Vector2(120f, 40f);

            var backBtnGO = CreateButton(header.transform, "BackButton",
                "<", new Vector2(-400, 0));
            SetButtonSize(backBtnGO, new Vector2(88, 88));
            ApplySprite(backBtnGO.GetComponent<Image>(), _btnCircle);
            var backBtnRect = backBtnGO.GetComponent<RectTransform>();
            backBtnRect.anchorMin = new Vector2(0f, 1f);
            backBtnRect.anchorMax = new Vector2(0f, 1f);
            backBtnRect.pivot = new Vector2(0f, 1f);
            backBtnRect.anchoredPosition = new Vector2(24f, -24f);
            var backBtnLabel = backBtnGO.GetComponentInChildren<TextMeshProUGUI>();
            if (backBtnLabel != null) backBtnLabel.fontSize = 28;

            // --- Pause button (top-right, leftmost) ---
            var pauseBtnGO = CreateButton(header.transform, "PauseButton",
                "||", Vector2.zero);
            SetButtonSize(pauseBtnGO, new Vector2(56, 56));
            ApplySprite(pauseBtnGO.GetComponent<Image>(), _btnCircle);
            var pauseBtnRect = pauseBtnGO.GetComponent<RectTransform>();
            pauseBtnRect.anchorMin = new Vector2(1f, 1f);
            pauseBtnRect.anchorMax = new Vector2(1f, 1f);
            pauseBtnRect.pivot = new Vector2(1f, 1f);
            pauseBtnRect.anchoredPosition = new Vector2(-152f, -16f);
            var pauseBtnLabel = pauseBtnGO.GetComponentInChildren<TextMeshProUGUI>();
            if (pauseBtnLabel != null) pauseBtnLabel.fontSize = 24;

            // --- Hint/Dica button (top-right, rightmost) ---
            var hintBtnGO = CreateButton(header.transform, "HintButton",
                "Dica", Vector2.zero);
            SetButtonSize(hintBtnGO, new Vector2(100, 56));
            var hintBtnRect = hintBtnGO.GetComponent<RectTransform>();
            hintBtnRect.anchorMin = new Vector2(1f, 1f);
            hintBtnRect.anchorMax = new Vector2(1f, 1f);
            hintBtnRect.pivot = new Vector2(1f, 1f);
            hintBtnRect.anchoredPosition = new Vector2(-24f, -16f);
            var hintBtnLabel = hintBtnGO.GetComponentInChildren<TextMeshProUGUI>();
            if (hintBtnLabel != null) hintBtnLabel.fontSize = 20;

            // ═══ GridView ═══
            var gridViewGO = new GameObject("GridView");
            gridViewGO.transform.SetParent(canvasGO.transform, false);
            var gridViewRect = gridViewGO.AddComponent<RectTransform>();
            gridViewRect.anchorMin = new Vector2(0.04f, 0.25f);
            gridViewRect.anchorMax = new Vector2(0.96f, 0.88f);
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

            // ═══ SelectionLine (covers only the grid area, not the header/footer) ═══
            var selectionLineGO = new GameObject("SelectionLine");
            selectionLineGO.transform.SetParent(canvasGO.transform, false);
            var slRect = selectionLineGO.AddComponent<RectTransform>();
            slRect.anchorMin = new Vector2(0.04f, 0.25f);
            slRect.anchorMax = new Vector2(0.96f, 0.88f);
            slRect.sizeDelta = Vector2.zero;
            var slImage = selectionLineGO.AddComponent<Image>();
            slImage.color = _colorTransparent;
            slImage.raycastTarget = true;

            var lineVisualGO = new GameObject("LineVisual");
            lineVisualGO.transform.SetParent(selectionLineGO.transform, false);
            var lineImage = lineVisualGO.AddComponent<Image>();
            lineImage.color = _colorSelectionLine;
            lineImage.raycastTarget = false;
            var lineVisualRect = lineVisualGO.GetComponent<RectTransform>();
            lineVisualRect.sizeDelta = new Vector2(0, 28);
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
            if (winBg != null) winBg.color = _colorOverlay;
            var winCG = winPopupGO.AddComponent<CanvasGroup>();

            var winPanel = new GameObject("PopupPanel");
            winPanel.transform.SetParent(winPopupGO.transform, false);
            var winPanelImg = winPanel.AddComponent<Image>();
            winPanelImg.color = _colorPanel;
            ApplySprite(winPanelImg, _panelPopup);
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

            var winTitleTMP = winTitleGO.GetComponent<TMP_Text>();
            var winMsgTMP = winMsgGO.GetComponent<TMP_Text>();
            var winStatsTMP = winStatsGO.GetComponent<TMP_Text>();
            if (winTitleTMP != null) winTitleTMP.color = _colorTextOnColor;
            if (winMsgTMP != null) winMsgTMP.color = _colorTextOnColor;
            if (winStatsTMP != null) winStatsTMP.color = _colorTextOnColor;

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

            // ═══ PausePopup (hidden) ═══
            var pausePopupGO = new GameObject("PausePopup");
            pausePopupGO.transform.SetParent(canvasGO.transform, false);
            var pausePopupRect = pausePopupGO.AddComponent<RectTransform>();
            pausePopupRect.anchorMin = Vector2.zero;
            pausePopupRect.anchorMax = Vector2.one;
            pausePopupRect.sizeDelta = Vector2.zero;
            var pauseBg = pausePopupGO.AddComponent<Image>();
            pauseBg.color = _colorOverlay;
            var pauseCG = pausePopupGO.AddComponent<CanvasGroup>();

            var pausePanel = new GameObject("PopupPanel");
            pausePanel.transform.SetParent(pausePopupGO.transform, false);
            var pausePanelImg = pausePanel.AddComponent<Image>();
            pausePanelImg.color = _colorPanel;
            ApplySprite(pausePanelImg, _panelPopup);
            var pausePanelRect = pausePanel.GetComponent<RectTransform>();
            pausePanelRect.anchorMin = new Vector2(0.1f, 0.3f);
            pausePanelRect.anchorMax = new Vector2(0.9f, 0.7f);
            pausePanelRect.sizeDelta = Vector2.zero;

            var pauseTitleGO = CreateTextElement(pausePanel.transform, "Title",
                "Pausado", 36, TextAlignmentOptions.Center, new Vector2(0, 120));
            var pauseTitleTMP = pauseTitleGO.GetComponent<TMP_Text>();
            if (pauseTitleTMP != null) pauseTitleTMP.color = _colorTextOnColor;
            var pauseContinueBtnGO = CreateButton(pausePanel.transform, "ContinueButton",
                "Continuar", new Vector2(0, 30));
            var pauseRestartBtnGO = CreateButton(pausePanel.transform, "RestartButton",
                "Reiniciar", new Vector2(0, -40));
            var pauseMenuBtnGO = CreateButton(pausePanel.transform, "MenuButton",
                "Menu", new Vector2(0, -110));

            pausePopupGO.SetActive(false);
            var pauseScript = pausePopupGO.AddComponent<Game.UI.Popups.PausePopup>();
            Wire(pauseScript, "_titleText", pauseTitleGO.GetComponent<TMP_Text>());
            Wire(pauseScript, "_continueButton", pauseContinueBtnGO.GetComponent<Button>());
            Wire(pauseScript, "_continueButtonText",
                pauseContinueBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            Wire(pauseScript, "_restartButton", pauseRestartBtnGO.GetComponent<Button>());
            Wire(pauseScript, "_restartButtonText",
                pauseRestartBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            Wire(pauseScript, "_menuButton", pauseMenuBtnGO.GetComponent<Button>());
            Wire(pauseScript, "_menuButtonText",
                pauseMenuBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            Wire(pauseScript, "_canvasGroup", pauseCG);
            Wire(pauseScript, "_popupPanel", pausePanelRect);

            // ═══ TutorialPopup (hidden) ═══
            var tutorialPopupGO = new GameObject("TutorialPopup");
            tutorialPopupGO.transform.SetParent(canvasGO.transform, false);
            var tutorialPopupRect = tutorialPopupGO.AddComponent<RectTransform>();
            tutorialPopupRect.anchorMin = Vector2.zero;
            tutorialPopupRect.anchorMax = Vector2.one;
            tutorialPopupRect.sizeDelta = Vector2.zero;
            var tutorialBg = tutorialPopupGO.AddComponent<Image>();
            tutorialBg.color = _colorOverlay;
            var tutorialCG = tutorialPopupGO.AddComponent<CanvasGroup>();

            var tutorialPanel = new GameObject("PopupPanel");
            tutorialPanel.transform.SetParent(tutorialPopupGO.transform, false);
            var tutorialPanelImg = tutorialPanel.AddComponent<Image>();
            tutorialPanelImg.color = _colorPanel;
            ApplySprite(tutorialPanelImg, _panelPopup);
            var tutorialPanelRect = tutorialPanel.GetComponent<RectTransform>();
            tutorialPanelRect.anchorMin = new Vector2(0.08f, 0.2f);
            tutorialPanelRect.anchorMax = new Vector2(0.92f, 0.8f);
            tutorialPanelRect.sizeDelta = Vector2.zero;

            var tutTitleGO = CreateTextElement(tutorialPanel.transform, "Title",
                "Como Jogar", 34, TextAlignmentOptions.Center, new Vector2(0, 180));
            var tutTitleTMP = tutTitleGO.GetComponent<TMP_Text>();
            if (tutTitleTMP != null) tutTitleTMP.color = _colorTextOnColor;

            var tutStep1GO = CreateTextElement(tutorialPanel.transform, "Step1",
                "1. Encontre as palavras escondidas no grid",
                20, TextAlignmentOptions.Left, new Vector2(0, 100));
            var tutStep1TMP = tutStep1GO.GetComponent<TMP_Text>();
            if (tutStep1TMP != null) tutStep1TMP.color = _colorTextOnColor;

            var tutStep2GO = CreateTextElement(tutorialPanel.transform, "Step2",
                "2. Arraste o dedo sobre as letras para selecionar",
                20, TextAlignmentOptions.Left, new Vector2(0, 40));
            var tutStep2TMP = tutStep2GO.GetComponent<TMP_Text>();
            if (tutStep2TMP != null) tutStep2TMP.color = _colorTextOnColor;

            var tutStep3GO = CreateTextElement(tutorialPanel.transform, "Step3",
                "3. Use o bot\u00e3o Dica se precisar de ajuda",
                20, TextAlignmentOptions.Left, new Vector2(0, -20));
            var tutStep3TMP = tutStep3GO.GetComponent<TMP_Text>();
            if (tutStep3TMP != null) tutStep3TMP.color = _colorTextOnColor;

            var tutDismissBtnGO = CreateButton(tutorialPanel.transform, "DismissButton",
                "Entendi!", new Vector2(0, -140));

            tutorialPopupGO.SetActive(false);
            var tutScript = tutorialPopupGO.AddComponent<Game.UI.Popups.TutorialPopup>();
            Wire(tutScript, "_titleText", tutTitleGO.GetComponent<TMP_Text>());
            Wire(tutScript, "_step1Text", tutStep1GO.GetComponent<TMP_Text>());
            Wire(tutScript, "_step2Text", tutStep2GO.GetComponent<TMP_Text>());
            Wire(tutScript, "_step3Text", tutStep3GO.GetComponent<TMP_Text>());
            Wire(tutScript, "_dismissButton", tutDismissBtnGO.GetComponent<Button>());
            Wire(tutScript, "_dismissButtonText",
                tutDismissBtnGO.transform.Find("Label").GetComponent<TMP_Text>());
            Wire(tutScript, "_canvasGroup", tutorialCG);
            Wire(tutScript, "_popupPanel", tutorialPanelRect);

            // ═══ GameplayController ═══
            var gpGO = new GameObject("GameplayController");
            var gpScript = gpGO.AddComponent<Game.UI.GameplayController>();
            Wire(gpScript, "_gridView", gridViewScript);
            Wire(gpScript, "_selectionLine", selLineScript);
            Wire(gpScript, "_wordListView", wordListScript);
            Wire(gpScript, "_categoryText", catTitleGO.GetComponent<TMP_Text>());
            Wire(gpScript, "_levelText", levelTextGO.GetComponent<TMP_Text>());
            Wire(gpScript, "_progressText", progressTextGO.GetComponent<TMP_Text>());
            Wire(gpScript, "_timerText", timerTextGO.GetComponent<TMP_Text>());
            Wire(gpScript, "_hintButton", hintBtnGO.GetComponent<Button>());
            Wire(gpScript, "_pauseButton", pauseBtnGO.GetComponent<Button>());
            Wire(gpScript, "_backButton", backBtnGO.GetComponent<Button>());
            Wire(gpScript, "_winPopupPrefab", winPopupGO);
            Wire(gpScript, "_pausePopupPrefab", pausePopupGO);
            Wire(gpScript, "_tutorialPopupPrefab", tutorialPopupGO);
            Wire(gpScript, "_popupParent", canvasGO.transform);

            // --- SFX AudioSource + clips ---
            var sfxSource = gpGO.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            Wire(gpScript, "_sfxSource", sfxSource);
            var wordFoundClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/SFX/word_found.wav");
            var allFoundClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/SFX/all_words_found.wav");
            var invalidClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/SFX/invalid_selection.wav");
            var hintClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/SFX/hint_used.wav");
            var clickClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/SFX/button_click.wav");
            if (wordFoundClip != null) Wire(gpScript, "_wordFoundClip", wordFoundClip);
            if (allFoundClip != null) Wire(gpScript, "_allWordsFoundClip", allFoundClip);
            if (invalidClip != null) Wire(gpScript, "_invalidSelectionClip", invalidClip);
            if (hintClip != null) Wire(gpScript, "_hintUsedClip", hintClip);
            if (clickClip != null) Wire(gpScript, "_buttonClickClip", clickClip);

            ApplyFontsToScene();
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
            image.color = _colorPanel;
            ApplySprite(image, _panelCard);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = image;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(460, 100);

            // Icon area (left 20% of the button)
            var iconGO = new GameObject("IconText");
            iconGO.transform.SetParent(go.transform, false);
            var iconTMP = iconGO.AddComponent<TextMeshProUGUI>();
            iconTMP.text = "";
            iconTMP.fontSize = 28;
            iconTMP.alignment = TextAlignmentOptions.Center;
            iconTMP.color = _colorTextPrimary;
            iconTMP.raycastTarget = false;
            iconTMP.textWrappingMode = TextWrappingModes.NoWrap;
            iconTMP.overflowMode = TextOverflowModes.Truncate;
            var iconRect = iconGO.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0);
            iconRect.anchorMax = new Vector2(0.18f, 1);
            iconRect.offsetMin = new Vector2(8, 8);
            iconRect.offsetMax = new Vector2(0, -8);

            // Icon image (sprite, same area as IconText)
            var iconImgGO = new GameObject("IconImage");
            iconImgGO.transform.SetParent(go.transform, false);
            var iconImg = iconImgGO.AddComponent<Image>();
            iconImg.color = _colorTextPrimary;
            iconImg.raycastTarget = false;
            iconImg.preserveAspect = true;
            var iconImgRect = iconImgGO.GetComponent<RectTransform>();
            iconImgRect.anchorMin = new Vector2(0.02f, 0.15f);
            iconImgRect.anchorMax = new Vector2(0.16f, 0.85f);
            iconImgRect.offsetMin = Vector2.zero;
            iconImgRect.offsetMax = Vector2.zero;
            iconImgGO.SetActive(false);

            // Name text (middle-right, top half)
            var nameGO = new GameObject("NameText");
            nameGO.transform.SetParent(go.transform, false);
            var nameTMP = nameGO.AddComponent<TextMeshProUGUI>();
            nameTMP.text = "Categoria";
            nameTMP.fontSize = 20;
            nameTMP.alignment = TextAlignmentOptions.MidlineLeft;
            nameTMP.color = _colorTextPrimary;
            nameTMP.raycastTarget = false;
            nameTMP.textWrappingMode = TextWrappingModes.Normal;
            nameTMP.overflowMode = TextOverflowModes.Ellipsis;
            var nameRect = nameGO.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.20f, 0.35f);
            nameRect.anchorMax = new Vector2(0.98f, 0.95f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;

            // Progress text (middle-right, bottom)
            var progressGO = new GameObject("ProgressText");
            progressGO.transform.SetParent(go.transform, false);
            var progressTMP = progressGO.AddComponent<TextMeshProUGUI>();
            progressTMP.text = "0/15";
            progressTMP.fontSize = 14;
            progressTMP.alignment = TextAlignmentOptions.MidlineLeft;
            progressTMP.color = _colorTextSecondary;
            progressTMP.raycastTarget = false;
            progressTMP.textWrappingMode = TextWrappingModes.NoWrap;
            var progressRect = progressGO.GetComponent<RectTransform>();
            progressRect.anchorMin = new Vector2(0.20f, 0.05f);
            progressRect.anchorMax = new Vector2(0.98f, 0.35f);
            progressRect.offsetMin = Vector2.zero;
            progressRect.offsetMax = Vector2.zero;

            // CategoryButtonItem script
            go.SetActive(false);
            var catItem = go.AddComponent<Game.UI.Screens.CategoryButtonItem>();
            Wire(catItem, "_button", btn);
            Wire(catItem, "_iconText", iconTMP);
            Wire(catItem, "_iconImage", iconImg);
            Wire(catItem, "_nameText", nameTMP);
            Wire(catItem, "_progressText", progressTMP);

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
            image.color = _colorTextPrimary;
            ApplySprite(image, _cellBg);
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
            labelTMP.color = _colorTextOnColor;
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
            completedTMP.color = _colorSuccess;
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
            lockedImg.color = _colorLockedOverlay;
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
            bgImage.color = _colorTextPrimary;
            ApplySprite(bgImage, _cellBg);

            var letterGO = new GameObject("Letter");
            letterGO.transform.SetParent(go.transform, false);
            var letterTMP = letterGO.AddComponent<TextMeshProUGUI>();
            letterTMP.text = "A";
            letterTMP.fontSize = 28;
            letterTMP.alignment = TextAlignmentOptions.Center;
            letterTMP.color = _colorTextOnColor;
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
            wordTMP.color = _colorTextPrimary;
            wordTMP.raycastTarget = false;
            var wordRect = wordGO.GetComponent<RectTransform>();
            wordRect.anchorMin = Vector2.zero;
            wordRect.anchorMax = Vector2.one;
            wordRect.sizeDelta = Vector2.zero;

            var strikeGO = new GameObject("StrikethroughLine");
            strikeGO.transform.SetParent(go.transform, false);
            var strikeImg = strikeGO.AddComponent<Image>();
            strikeImg.color = _colorSuccess;
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
            image.color = _colorBackground;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            return go;
        }

        private static void LoadSprites()
        {
            _btnPrimary = LoadSprite("Assets/_Project/Art/UI/Buttons/btn_primary.png");
            _btnCircle = LoadSprite("Assets/_Project/Art/UI/Buttons/btn_circle.png");
            _panelPopup = LoadSprite("Assets/_Project/Art/UI/Panels/panel_popup.png");
            _panelCard = LoadSprite("Assets/_Project/Art/UI/Panels/panel_card.png");
            _cellBg = LoadSprite("Assets/_Project/Art/UI/Grid/cell_bg.png");

            int loaded = 0;
            if (_btnPrimary != null) loaded++;
            if (_btnCircle != null) loaded++;
            if (_panelPopup != null) loaded++;
            if (_panelCard != null) loaded++;
            if (_cellBg != null) loaded++;
            Debug.Log($"[SceneCreator] Sprites loaded: {loaded}/5 (run Generate Sprites first if 0)");
        }

        private static Sprite LoadSprite(string assetPath)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        private static void ApplySprite(Image image, Sprite sprite)
        {
            if (image == null || sprite == null) return;
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
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
            tmp.color = _colorTextPrimary;
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
            image.color = _colorPrimary;
            ApplySprite(image, _btnPrimary);

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
            tmp.color = _colorTextOnColor;
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
            rect.sizeDelta = new Vector2(84, 40);

            var bgGO = new GameObject("Background");
            bgGO.transform.SetParent(go.transform, false);
            var bgImage = bgGO.AddComponent<Image>();
            bgImage.color = _colorTextSecondary;
            var bgRect = bgGO.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            var checkGO = new GameObject("Checkmark");
            checkGO.transform.SetParent(bgGO.transform, false);
            var checkImage = checkGO.AddComponent<Image>();
            checkImage.color = _colorSuccess;
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
            bgImage.color = _colorPanel;

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);
            var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
            labelTMP.text = "Portugues (BR)";
            labelTMP.fontSize = 18;
            labelTMP.alignment = TextAlignmentOptions.Center;
            labelTMP.color = _colorTextOnColor;
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
            templateImage.color = _colorPanel;
            var scrollRect = templateGO.AddComponent<ScrollRect>();

            var viewportGO = new GameObject("Viewport");
            viewportGO.transform.SetParent(templateGO.transform, false);
            var viewportRect = viewportGO.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;
            var viewportMask = viewportGO.AddComponent<Mask>();
            var viewportImage = viewportGO.AddComponent<Image>();
            viewportImage.color = _colorTextPrimary;
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
            itemBgImage.color = _colorPanel;
            var itemBgRect = itemBgGO.GetComponent<RectTransform>();
            itemBgRect.anchorMin = Vector2.zero;
            itemBgRect.anchorMax = Vector2.one;
            itemBgRect.sizeDelta = Vector2.zero;

            var itemCheckGO = new GameObject("Item Checkmark");
            itemCheckGO.transform.SetParent(itemBgGO.transform, false);
            var itemCheckImage = itemCheckGO.AddComponent<Image>();
            itemCheckImage.color = _colorPrimary;
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
            itemLabelTMP.color = _colorTextOnColor;
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

        // ═══════════════════════════════════════════════════
        //  Font Helpers
        // ═══════════════════════════════════════════════════

        private static void LoadFonts()
        {
            _fontRegular = FontAssetGenerator.LoadFont("Regular");
            _fontSemiBold = FontAssetGenerator.LoadFont("SemiBold");
            _fontBold = FontAssetGenerator.LoadFont("Bold");
            _fontExtraBold = FontAssetGenerator.LoadFont("ExtraBold");

            if (_fontRegular != null)
                Debug.Log("[SceneCreator] ✅ Fontes Nunito carregadas.");
            else
                Debug.LogWarning("[SceneCreator] ⚠️ Fontes Nunito não encontradas. Execute 'Generate Font Assets' primeiro. Usando fonte padrão.");
        }

        /// <summary>
        /// Returns the appropriate Nunito weight based on font size:
        /// - >= 30: ExtraBold (titles, headers)
        /// - >= 24: Bold (grid letters, level numbers, buttons)
        /// - >= 18: SemiBold (subtitles, buttons)
        /// - < 18: Regular (body text, word list, progress)
        /// </summary>
        private static TMP_FontAsset GetFontForSize(float fontSize)
        {
            if (fontSize >= 30) return _fontExtraBold ?? _fontRegular;
            if (fontSize >= 24) return _fontBold ?? _fontRegular;
            if (fontSize >= 18) return _fontSemiBold ?? _fontRegular;
            return _fontRegular;
        }

        /// <summary>
        /// Applies Nunito fonts to all TMP_Text components in the active scene.
        /// Font weight is auto-selected based on fontSize.
        /// </summary>
        private static void ApplyFontsToScene()
        {
            if (_fontRegular == null) return;

            var scene = EditorSceneManager.GetActiveScene();
            int count = 0;
            foreach (var rootGO in scene.GetRootGameObjects())
            {
                // GetComponentsInChildren with includeInactive=true to catch disabled objects
                var texts = rootGO.GetComponentsInChildren<TMP_Text>(true);
                foreach (var tmp in texts)
                {
                    var font = GetFontForSize(tmp.fontSize);
                    if (font != null)
                    {
                        tmp.font = font;
                        count++;
                    }
                }
            }
            Debug.Log($"[SceneCreator] Fonte Nunito aplicada em {count} textos.");
        }
    }
}
