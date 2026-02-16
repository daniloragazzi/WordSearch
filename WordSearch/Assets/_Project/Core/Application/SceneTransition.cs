using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace RagazziStudios.Core.Application
{
    /// <summary>
    /// Gerencia transições suaves (fade) entre cenas.
    /// Componente anexado ao GameManager (DontDestroyOnLoad).
    /// Cria um overlay Canvas persistente com fade + loading indicator.
    /// </summary>
    public class SceneTransition : MonoBehaviour
    {
        [Header("Configuração")]
        [SerializeField] private float _fadeDuration = 0.3f;

        private CanvasGroup _overlayGroup;
        private bool _isTransitioning;

        // Loading indicator elements
        private GameObject _loadingContainer;
        private Image _spinnerImage;
        private Image _progressBarFill;
        private TMP_Text _loadingText;
        private RectTransform _spinnerRect;

        /// <summary>Se uma transição está ativa. Impede dupla transição.</summary>
        public bool IsTransitioning => _isTransitioning;

        private void Awake()
        {
            CreateOverlay();
        }

        /// <summary>
        /// Cria o overlay Canvas/Image/CanvasGroup para fade + loading UI.
        /// Renderiza sobre tudo (sortingOrder alto).
        /// </summary>
        private void CreateOverlay()
        {
            var overlayGO = new GameObject("TransitionOverlay");
            overlayGO.transform.SetParent(transform, false);

            var canvas = overlayGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;

            var scaler = overlayGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            overlayGO.AddComponent<GraphicRaycaster>();

            _overlayGroup = overlayGO.AddComponent<CanvasGroup>();
            _overlayGroup.alpha = 0f;
            _overlayGroup.blocksRaycasts = false;
            _overlayGroup.interactable = false;

            // Imagem de fundo que cobre toda a tela
            var imgGO = new GameObject("FadeImage");
            imgGO.transform.SetParent(overlayGO.transform, false);

            var img = imgGO.AddComponent<Image>();
            img.color = new Color(0.118f, 0.118f, 0.180f, 1f); // #1E1E2E
            img.raycastTarget = false;

            var rt = imgGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            // --- Loading Container (centered) ---
            CreateLoadingIndicator(overlayGO.transform);
        }

        private void CreateLoadingIndicator(Transform parent)
        {
            _loadingContainer = new GameObject("LoadingContainer");
            _loadingContainer.transform.SetParent(parent, false);

            var containerRect = _loadingContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(300, 160);
            containerRect.anchoredPosition = Vector2.zero;

            // --- Spinner (arc circle) ---
            var spinnerGO = new GameObject("Spinner");
            spinnerGO.transform.SetParent(_loadingContainer.transform, false);

            _spinnerRect = spinnerGO.AddComponent<RectTransform>();
            _spinnerRect.anchorMin = new Vector2(0.5f, 1f);
            _spinnerRect.anchorMax = new Vector2(0.5f, 1f);
            _spinnerRect.sizeDelta = new Vector2(48, 48);
            _spinnerRect.anchoredPosition = new Vector2(0, -30);

            _spinnerImage = spinnerGO.AddComponent<Image>();
            _spinnerImage.raycastTarget = false;

            // Create a procedural arc sprite for the spinner
            _spinnerImage.sprite = CreateSpinnerSprite();
            _spinnerImage.color = new Color(0.29f, 0.56f, 0.89f, 1f); // primary blue

            // --- "Carregando..." text ---
            var textGO = new GameObject("LoadingText");
            textGO.transform.SetParent(_loadingContainer.transform, false);

            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0f, 0.3f);
            textRect.anchorMax = new Vector2(1f, 0.6f);
            textRect.sizeDelta = Vector2.zero;

            _loadingText = textGO.AddComponent<TextMeshProUGUI>();
            _loadingText.text = "Carregando...";
            _loadingText.fontSize = 22;
            _loadingText.alignment = TextAlignmentOptions.Center;
            _loadingText.color = new Color(0.85f, 0.85f, 0.90f, 1f);
            _loadingText.raycastTarget = false;

            // --- Progress bar background ---
            var barBgGO = new GameObject("ProgressBarBg");
            barBgGO.transform.SetParent(_loadingContainer.transform, false);

            var barBgRect = barBgGO.AddComponent<RectTransform>();
            barBgRect.anchorMin = new Vector2(0.1f, 0f);
            barBgRect.anchorMax = new Vector2(0.9f, 0f);
            barBgRect.sizeDelta = new Vector2(0, 8);
            barBgRect.anchoredPosition = new Vector2(0, 20);

            var barBgImg = barBgGO.AddComponent<Image>();
            barBgImg.color = new Color(1f, 1f, 1f, 0.15f);
            barBgImg.raycastTarget = false;

            // --- Progress bar fill ---
            var barFillGO = new GameObject("ProgressBarFill");
            barFillGO.transform.SetParent(barBgGO.transform, false);

            var barFillRect = barFillGO.AddComponent<RectTransform>();
            barFillRect.anchorMin = Vector2.zero;
            barFillRect.anchorMax = new Vector2(0f, 1f); // starts at 0 width
            barFillRect.sizeDelta = Vector2.zero;

            _progressBarFill = barFillGO.AddComponent<Image>();
            _progressBarFill.color = new Color(0.29f, 0.56f, 0.89f, 1f); // primary blue
            _progressBarFill.raycastTarget = false;

            _loadingContainer.SetActive(false);
        }

        /// <summary>
        /// Cria um sprite de arco (3/4 de círculo) para o spinner.
        /// </summary>
        private Sprite CreateSpinnerSprite()
        {
            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color32[size * size];

            float center = size * 0.5f;
            float outerR = size * 0.5f;
            float innerR = size * 0.5f - 6f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x + 0.5f - center;
                    float dy = y + 0.5f - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    // Ring (between inner and outer radius)
                    float outerAlpha = Mathf.Clamp01((outerR - dist) / 1.5f + 0.5f);
                    float innerAlpha = Mathf.Clamp01((dist - innerR) / 1.5f + 0.5f);
                    float ringAlpha = outerAlpha * innerAlpha;

                    // Arc: skip a 90-degree gap (bottom-right quadrant)
                    float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
                    if (angle < 0) angle += 360f;
                    // Gap from 280 to 360 degrees (bottom-right)
                    float gapStart = 280f, gapEnd = 360f;
                    if (angle >= gapStart && angle <= gapEnd)
                    {
                        // Smooth edges at gap boundaries
                        float edgeDist = Mathf.Min(
                            Mathf.Abs(angle - gapStart),
                            Mathf.Abs(angle - gapEnd));
                        ringAlpha *= Mathf.Clamp01(edgeDist / 8f);
                    }

                    pixels[y * size + x] = new Color32(255, 255, 255,
                        (byte)(ringAlpha * 255));
                }
            }

            tex.SetPixels32(pixels);
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;

            return Sprite.Create(tex,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                100f);
        }

        /// <summary>
        /// Faz a transição: fade out → loading → carrega cena → fade in.
        /// </summary>
        /// <param name="sceneName">Nome da cena destino.</param>
        /// <param name="onBeforeLoad">Callback opcional antes de carregar (ex: mudar state).</param>
        public void TransitionTo(string sceneName, Action onBeforeLoad = null)
        {
            if (_isTransitioning) return;
            StartCoroutine(TransitionCoroutine(sceneName, onBeforeLoad));
        }

        private IEnumerator TransitionCoroutine(string sceneName, Action onBeforeLoad)
        {
            _isTransitioning = true;

            // Bloquear input durante transição
            _overlayGroup.blocksRaycasts = true;

            // Fade OUT (tela escurece)
            yield return FadeCoroutine(0f, 1f);

            // Callback pré-carregamento
            onBeforeLoad?.Invoke();

            // Mostrar loading indicator
            _loadingContainer.SetActive(true);
            SetProgressBar(0f);

            // Carregar cena com progresso
            var op = SceneManager.LoadSceneAsync(sceneName);
            if (op != null)
            {
                op.allowSceneActivation = false;

                // Animate loading while scene loads
                while (op.progress < 0.9f)
                {
                    SetProgressBar(op.progress / 0.9f);
                    RotateSpinner();
                    yield return null;
                }

                // Scene is ready, fill bar to 100%
                SetProgressBar(1f);
                RotateSpinner();
                yield return null;

                // Activate scene
                op.allowSceneActivation = true;

                while (!op.isDone)
                {
                    RotateSpinner();
                    yield return null;
                }
            }

            // Esperar 1 frame para a nova cena se estabilizar
            yield return null;

            // Ocultar loading indicator
            _loadingContainer.SetActive(false);

            // Fade IN (tela clareia)
            yield return FadeCoroutine(1f, 0f);

            // Liberar input
            _overlayGroup.blocksRaycasts = false;
            _isTransitioning = false;
        }

        private void SetProgressBar(float progress)
        {
            if (_progressBarFill == null) return;
            var rt = _progressBarFill.rectTransform;
            rt.anchorMax = new Vector2(Mathf.Clamp01(progress), 1f);
        }

        private void RotateSpinner()
        {
            if (_spinnerRect == null) return;
            _spinnerRect.Rotate(0f, 0f, -360f * Time.unscaledDeltaTime * 1.5f);
        }

        private IEnumerator FadeCoroutine(float from, float to)
        {
            float elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / _fadeDuration);
                _overlayGroup.alpha = Mathf.Lerp(from, to, t);
                yield return null;
            }
            _overlayGroup.alpha = to;
        }

        /// <summary>
        /// Faz apenas o fade IN (útil para a primeira cena após Boot).
        /// </summary>
        public void FadeIn()
        {
            StartCoroutine(FadeInCoroutine());
        }

        private IEnumerator FadeInCoroutine()
        {
            _overlayGroup.alpha = 1f;
            _overlayGroup.blocksRaycasts = true;
            yield return FadeCoroutine(1f, 0f);
            _overlayGroup.blocksRaycasts = false;
        }
    }
}
