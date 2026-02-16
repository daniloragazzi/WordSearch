using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Application;
using RagazziStudios.Core.Domain.Grid;
using RagazziStudios.Core.Infrastructure;
using RagazziStudios.Core.Infrastructure.Localization;
using RagazziStudios.Game.UI.Components;

namespace RagazziStudios.Game.UI
{
    /// <summary>
    /// Controller principal da cena de gameplay.
    /// Conecta GridView, SelectionLine, WordFinder, WordListView e popups.
    /// </summary>
    public class GameplayController : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private GridView _gridView;
        [SerializeField] private SelectionLine _selectionLine;

        [Header("UI")]
        [SerializeField] private WordListView _wordListView;
        [SerializeField] private TMP_Text _categoryText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private TMP_Text _timerText;

        [Header("Botões")]
        [SerializeField] private Button _hintButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _backButton;

        [Header("Popups")]
        [SerializeField] private GameObject _winPopupPrefab;
        [SerializeField] private GameObject _pausePopupPrefab;
        [SerializeField] private Transform _popupParent;

        [Header("Feedback")]
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioClip _wordFoundClip;
        [SerializeField] private AudioClip _allWordsFoundClip;

        private WordFinder _wordFinder;
        private LevelManager _levelManager;

        private void Start()
        {
            // Se GameManager não existe, a cena Boot nunca rodou. Redirecionar.
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("[GameplayController] GameManager not found. Redirecting to Boot scene...");
                SceneManager.LoadScene("Boot");
                return;
            }

            _levelManager = GameManager.Instance.LevelManager;
            if (_levelManager == null)
            {
                Debug.LogError("[GameplayController] LevelManager not found!");
                return;
            }

            var levelData = _levelManager.CurrentLevel;
            if (levelData == null)
            {
                Debug.LogError("[GameplayController] No current level data!");
                return;
            }

            // Criar WordFinder
            _wordFinder = new WordFinder(levelData.Placements);
            _wordFinder.OnWordFound += OnWordFound;
            _wordFinder.OnAllWordsFound += OnAllWordsFound;

            // Construir grid visual
            _gridView.BuildGrid(levelData.Grid);

            // Preencher lista de palavras
            _wordListView.SetWords(levelData.Placements);

            // Configurar seleção
            _selectionLine.OnSelectionComplete += OnSelectionComplete;

            // Configurar botões
            if (_hintButton != null) _hintButton.onClick.AddListener(OnHintClicked);
            if (_pauseButton != null) _pauseButton.onClick.AddListener(OnPauseClicked);
            if (_backButton != null) _backButton.onClick.AddListener(OnBackClicked);

            // Escutar evento de dica usada do GameManager
            if (GameManager.Instance != null)
                GameManager.Instance.OnHintUsed += OnHintGranted;

            // Atualizar header
            UpdateHeader();
        }

        private void Update()
        {
            UpdateTimer();
        }

        private void OnDestroy()
        {
            if (_wordFinder != null)
            {
                _wordFinder.OnWordFound -= OnWordFound;
                _wordFinder.OnAllWordsFound -= OnAllWordsFound;
            }

            if (_selectionLine != null)
                _selectionLine.OnSelectionComplete -= OnSelectionComplete;

            if (_hintButton != null) _hintButton.onClick.RemoveListener(OnHintClicked);
            if (_pauseButton != null) _pauseButton.onClick.RemoveListener(OnPauseClicked);
            if (_backButton != null) _backButton.onClick.RemoveListener(OnBackClicked);

            if (GameManager.Instance != null)
                GameManager.Instance.OnHintUsed -= OnHintGranted;
        }

        private void UpdateHeader()
        {
            if (ServiceLocator.TryGet<ILocalizationService>(out var loc))
            {
                // Carregar nome da categoria
                var categoriesAsset = Resources.Load<TextAsset>("Data/categories");
                if (categoriesAsset != null && _categoryText != null)
                {
                    var file = JsonUtility.FromJson<Core.Domain.Words.CategoriesFile>(
                        categoriesAsset.text);
                    var cat = file?.categories?.Find(
                        c => c.id == _levelManager.CurrentCategoryId);
                    if (cat != null)
                        _categoryText.text = cat.name;
                }

                if (_levelText != null)
                    _levelText.text = loc.Get("label_level",
                        _levelManager.CurrentLevelNumber);
            }

            UpdateProgress();
        }

        private void UpdateProgress()
        {
            if (_progressText == null || _wordFinder == null) return;

            _progressText.text = $"{_wordFinder.FoundCount}/{_wordFinder.TotalWords}";
        }

        private void UpdateTimer()
        {
            if (_timerText == null || _levelManager == null) return;

            float elapsed = Time.realtimeSinceStartup - _levelManager.LevelStartTime;
            int minutes = Mathf.FloorToInt(elapsed / 60f);
            int seconds = Mathf.FloorToInt(elapsed % 60f);
            _timerText.text = $"{minutes}:{seconds:D2}";
        }

        // --- Callbacks ---

        private void OnSelectionComplete(IReadOnlyList<(int row, int col)> positions)
        {
            if (_wordFinder == null) return;

            // Verificar se a seleção é uma palavra válida
            _wordFinder.CheckSelection(positions);
        }

        private void OnWordFound(WordPlacement placement)
        {
            // Marcar no grid visual
            _gridView.MarkWordFound(placement);

            // Marcar na lista de palavras
            _wordListView.MarkWordFound(placement);

            // Atualizar progresso
            UpdateProgress();

            // SFX
            if (_sfxSource != null && _wordFoundClip != null)
                _sfxSource.PlayOneShot(_wordFoundClip);

            Debug.Log($"[Gameplay] Word found: {placement.DisplayWord}");
        }

        private void OnAllWordsFound()
        {
            Debug.Log("[Gameplay] All words found! Level complete!");

            // SFX de vitória
            if (_sfxSource != null && _allWordsFoundClip != null)
                _sfxSource.PlayOneShot(_allWordsFoundClip);

            // Completar nível via LevelManager
            _levelManager.CompleteLevel();

            // Mostrar interstitial
            GameManager.Instance?.TryShowInterstitial();

            // Mostrar popup de vitória (com delay para feedback)
            StartCoroutine(ShowWinPopupDelayed(1.0f));
        }

        private IEnumerator ShowWinPopupDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_winPopupPrefab != null && _popupParent != null)
            {
                var popup = Instantiate(_winPopupPrefab, _popupParent);
                popup.SetActive(true);
                var winPopup = popup.GetComponent<Popups.WinPopup>();

                if (winPopup != null)
                {
                    float timeSeconds = Time.realtimeSinceStartup - _levelManager.LevelStartTime;
                    winPopup.Setup(
                        _levelManager.CurrentCategoryId,
                        _levelManager.CurrentLevelNumber,
                        _wordFinder.TotalWords,
                        timeSeconds,
                        _levelManager.HasNextLevel);
                }
            }

            // Transicionar state machine
            GameManager.Instance?.StateMachine.TransitionTo(GameStateType.Win);
        }

        private void OnHintClicked()
        {
            GameManager.Instance?.RequestHint();
        }

        private void OnHintGranted()
        {
            // Recompensa recebida — revelar uma palavra
            var hint = _wordFinder.GetHint();
            if (hint == null) return;

            _wordFinder.RevealWord(hint);
            _gridView.HighlightHint(hint);
            _wordListView.MarkWordFound(hint);
            _levelManager.RegisterHint();

            UpdateProgress();
        }

        private void OnPauseClicked()
        {
            GameManager.Instance?.StateMachine.TransitionTo(GameStateType.Pause);
            Time.timeScale = 0f;

            if (_pausePopupPrefab != null && _popupParent != null)
            {
                var popup = Instantiate(_pausePopupPrefab, _popupParent);
                popup.SetActive(true);
            }
        }

        private void OnBackClicked()
        {
            // Registrar que saiu sem completar
            _levelManager.QuitLevel(_wordFinder.FoundCount);

            GameManager.Instance?.StateMachine.TransitionTo(GameStateType.LevelSelect);
            GameManager.Instance?.LoadScene("MainMenu");
        }
    }
}
