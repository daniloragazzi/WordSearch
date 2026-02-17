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
        [SerializeField] private GameObject _tutorialPopupPrefab;
        [SerializeField] private Transform _popupParent;

        [Header("Feedback")]
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioClip _wordFoundClip;
        [SerializeField] private AudioClip _allWordsFoundClip;
        [SerializeField] private AudioClip _invalidSelectionClip;
        [SerializeField] private AudioClip _hintUsedClip;
        [SerializeField] private AudioClip _buttonClickClip;

        private WordFinder _wordFinder;
        private LevelManager _levelManager;
        private bool _sfxEnabled = true;

        private void Start()
        {
            // Se GameManager não existe, a cena Boot nunca rodou. Redirecionar.
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("[GameplayController] GameManager not found. Redirecting to Boot scene...");
                SceneManager.LoadScene("Boot");
                return;
            }

            // Ler preferência de SFX
            if (ServiceLocator.TryGet<Core.Infrastructure.Storage.IStorageService>(out var sfxStorage))
                _sfxEnabled = sfxStorage.GetBool(Core.Infrastructure.Storage.StorageKeys.SOUND_ENABLED, true);

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

            // Tutorial de primeiro uso
            ShowTutorialIfFirstTime();
        }

        private void ShowTutorialIfFirstTime()
        {
            if (_tutorialPopupPrefab == null || _popupParent == null) return;

            if (ServiceLocator.TryGet<Core.Infrastructure.Storage.IStorageService>(out var storage))
            {
                if (storage.GetBool(Core.Infrastructure.Storage.StorageKeys.TUTORIAL_COMPLETED, false))
                    return; // já viu o tutorial
            }

            var popup = Instantiate(_tutorialPopupPrefab, _popupParent);
            popup.SetActive(true);
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
            if (_levelManager.IsChallengeMode)
            {
                // Modo desafio: mostrar tamanho do grid
                if (_categoryText != null)
                    _categoryText.text = "Desafio";
                if (_levelText != null)
                {
                    var grid = _levelManager.CurrentLevel?.Difficulty;
                    _levelText.text = grid != null
                        ? $"{grid.GridRows}x{grid.GridCols}"
                        : "Desafio";
                }
            }
            else if (ServiceLocator.TryGet<ILocalizationService>(out var loc))
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

            var result = _wordFinder.CheckSelection(positions);

            // Seleção inválida com mais de 1 célula? Feedback visual
            if (result == null && positions.Count > 1)
            {
                var posCopy = new List<(int row, int col)>(positions);
                StartCoroutine(InvalidSelectionFeedback(posCopy));

                // SFX de erro
                PlaySfx(_invalidSelectionClip);
            }
        }

        /// <summary>
        /// Pisca as células em vermelho e treme o grid levemente.
        /// </summary>
        private IEnumerator InvalidSelectionFeedback(List<(int row, int col)> positions)
        {
            // Esperar 1 frame — ClearVisualSelection já rodou
            yield return null;

            var cells = new List<Components.LetterCell>();
            foreach (var pos in positions)
            {
                var cell = _gridView.GetCell(pos.row, pos.col);
                if (cell != null && !cell.IsFound)
                    cells.Add(cell);
            }

            // Flash vermelho nas células
            foreach (var cell in cells)
                cell.FlashInvalid(0.35f);

            // Shake leve no grid
            var gridRT = _gridView.GetComponent<RectTransform>();
            Vector3 origPos = gridRT.localPosition;
            float duration = 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float offset = Mathf.Sin(elapsed * 50f) * 5f * (1f - elapsed / duration);
                gridRT.localPosition = origPos + new Vector3(offset, 0, 0);
                elapsed += Time.deltaTime;
                yield return null;
            }

            gridRT.localPosition = origPos;
        }

        private void OnWordFound(WordPlacement placement)
        {
            // Marcar no grid visual
            _gridView.MarkWordFound(placement);

            // Linha colorida persistente sobre a palavra
            _selectionLine.ShowFoundLine(placement);

            // Animação de pulso sequencial nas células
            var cellPositions = placement.GetCellPositions();
            for (int i = 0; i < cellPositions.Length; i++)
            {
                var cell = _gridView.GetCell(cellPositions[i].row, cellPositions[i].col);
                cell?.PulseFound(i * 0.04f); // cascata com 40ms de atraso
            }

            // Marcar na lista de palavras
            _wordListView.MarkWordFound(placement);

            // Atualizar progresso
            UpdateProgress();

            // SFX
            PlaySfx(_wordFoundClip);

            Debug.Log($"[Gameplay] Word found: {placement.DisplayWord}");
        }

        private void OnAllWordsFound()
        {
            Debug.Log("[Gameplay] All words found! Level complete!");

            // Celebração: confete!
            if (_popupParent != null)
                Components.ConfettiEffect.Create(_popupParent);

            // SFX de vitória
            PlaySfx(_allWordsFoundClip);

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
            _selectionLine.ShowFoundLine(hint);
            _wordListView.MarkWordFound(hint);
            _levelManager.RegisterHint();

            UpdateProgress();
            PlaySfx(_hintUsedClip);
        }

        private void OnPauseClicked()
        {
            PlaySfx(_buttonClickClip);
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
            PlaySfx(_buttonClickClip);

            // Registrar que saiu sem completar
            _levelManager.QuitLevel(_wordFinder.FoundCount);

            var targetState = _levelManager.IsChallengeMode
                ? GameStateType.ChallengeSelect
                : GameStateType.LevelSelect;

            GameManager.Instance?.StateMachine.TransitionTo(targetState);
            GameManager.Instance?.LoadScene("MainMenu");
        }

        /// <summary>
        /// Toca um clip de SFX respeitando a preferência de som do jogador.
        /// </summary>
        private void PlaySfx(AudioClip clip)
        {
            if (!_sfxEnabled || _sfxSource == null || clip == null) return;
            _sfxSource.PlayOneShot(clip);
        }
    }
}
