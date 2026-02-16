using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RagazziStudios.Core.Application
{
    /// <summary>
    /// Gerencia transições suaves (fade) entre cenas.
    /// Componente anexado ao GameManager (DontDestroyOnLoad).
    /// Cria um overlay Canvas persistente com uma Image preta.
    /// </summary>
    public class SceneTransition : MonoBehaviour
    {
        [Header("Configuração")]
        [SerializeField] private float _fadeDuration = 0.3f;

        private CanvasGroup _overlayGroup;
        private bool _isTransitioning;

        /// <summary>Se uma transição está ativa. Impede dupla transição.</summary>
        public bool IsTransitioning => _isTransitioning;

        private void Awake()
        {
            CreateOverlay();
        }

        /// <summary>
        /// Cria o overlay Canvas/Image/CanvasGroup para fade.
        /// Renderiza sobre tudo (sortingOrder alto).
        /// </summary>
        private void CreateOverlay()
        {
            var overlayGO = new GameObject("TransitionOverlay");
            overlayGO.transform.SetParent(transform, false);

            var canvas = overlayGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;

            overlayGO.AddComponent<CanvasScaler>();

            var raycaster = overlayGO.AddComponent<GraphicRaycaster>();

            _overlayGroup = overlayGO.AddComponent<CanvasGroup>();
            _overlayGroup.alpha = 0f;
            _overlayGroup.blocksRaycasts = false;
            _overlayGroup.interactable = false;

            // Imagem preta que cobre toda a tela
            var imgGO = new GameObject("FadeImage");
            imgGO.transform.SetParent(overlayGO.transform, false);

            var img = imgGO.AddComponent<Image>();
            img.color = new Color(0.118f, 0.118f, 0.180f, 1f); // #1E1E2E (mesmo tom do bg)
            img.raycastTarget = false;

            var rt = imgGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
        }

        /// <summary>
        /// Faz a transição: fade out → carrega cena → fade in.
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

            // Carregar cena
            var op = SceneManager.LoadSceneAsync(sceneName);
            while (op != null && !op.isDone)
                yield return null;

            // Esperar 1 frame para a nova cena se estabilizar
            yield return null;

            // Fade IN (tela clareia)
            yield return FadeCoroutine(1f, 0f);

            // Liberar input
            _overlayGroup.blocksRaycasts = false;
            _isTransitioning = false;
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
