using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Infrastructure;
using RagazziStudios.Core.Infrastructure.Storage;
using RagazziStudios.Core.Infrastructure.Localization;

namespace RagazziStudios.Game.UI.Popups
{
    /// <summary>
    /// Popup de tutorial exibido na primeira partida do jogador.
    /// Mostra instruções básicas de como jogar e marca a flag
    /// TUTORIAL_COMPLETED no storage ao ser dispensado.
    /// </summary>
    public class TutorialPopup : MonoBehaviour
    {
        [Header("Textos")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _step1Text;
        [SerializeField] private TMP_Text _step2Text;
        [SerializeField] private TMP_Text _step3Text;

        [Header("Botão")]
        [SerializeField] private Button _dismissButton;
        [SerializeField] private TMP_Text _dismissButtonText;

        [Header("Animação")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _popupPanel;
        [SerializeField] private float _animationDuration = 0.3f;

        /// <summary>Callback quando o tutorial é dispensado.</summary>
        public event System.Action OnDismissed;

        private void Start()
        {
            UpdateLocalization();

            if (_dismissButton != null)
                _dismissButton.onClick.AddListener(HandleDismiss);

            PlayEnterAnimation();
        }

        private void UpdateLocalization()
        {
            if (ServiceLocator.TryGet<ILocalizationService>(out var loc))
            {
                if (_titleText != null)
                    _titleText.text = loc.Get("tutorial_title");
                if (_step1Text != null)
                    _step1Text.text = loc.Get("tutorial_step1");
                if (_step2Text != null)
                    _step2Text.text = loc.Get("tutorial_step2");
                if (_step3Text != null)
                    _step3Text.text = loc.Get("tutorial_step3");
                if (_dismissButtonText != null)
                    _dismissButtonText.text = loc.Get("tutorial_dismiss");
            }
            else
            {
                // Fallback PT-BR
                if (_titleText != null)
                    _titleText.text = "Como Jogar";
                if (_step1Text != null)
                    _step1Text.text = "1. Encontre as palavras escondidas no grid";
                if (_step2Text != null)
                    _step2Text.text = "2. Arraste o dedo sobre as letras para selecionar";
                if (_step3Text != null)
                    _step3Text.text = "3. Use o botão Dica se precisar de ajuda";
                if (_dismissButtonText != null)
                    _dismissButtonText.text = "Entendi!";
            }
        }

        private void PlayEnterAnimation()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                StartCoroutine(AnimateIn());
            }

            if (_popupPanel != null)
                _popupPanel.localScale = Vector3.one * 0.8f;
        }

        private System.Collections.IEnumerator AnimateIn()
        {
            float elapsed = 0f;
            Vector3 startScale = Vector3.one * 0.8f;
            Vector3 endScale = Vector3.one;

            while (elapsed < _animationDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / _animationDuration);

                if (_canvasGroup != null)
                    _canvasGroup.alpha = t;
                if (_popupPanel != null)
                    _popupPanel.localScale = Vector3.Lerp(startScale, endScale, t);

                yield return null;
            }

            if (_canvasGroup != null) _canvasGroup.alpha = 1f;
            if (_popupPanel != null) _popupPanel.localScale = Vector3.one;
        }

        private System.Collections.IEnumerator AnimateOutAndDestroy(System.Action callback)
        {
            float elapsed = 0f;

            while (elapsed < _animationDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = 1f - Mathf.SmoothStep(0f, 1f, elapsed / _animationDuration);

                if (_canvasGroup != null)
                    _canvasGroup.alpha = t;
                if (_popupPanel != null)
                    _popupPanel.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, t);

                yield return null;
            }

            callback?.Invoke();
            Destroy(gameObject);
        }

        private void HandleDismiss()
        {
            // Marcar tutorial como visto no storage
            if (ServiceLocator.TryGet<IStorageService>(out var storage))
            {
                storage.SetBool(StorageKeys.TUTORIAL_COMPLETED, true);
                storage.Save();
            }

            StartCoroutine(AnimateOutAndDestroy(() =>
            {
                OnDismissed?.Invoke();
            }));
        }

        private void OnDestroy()
        {
            if (_dismissButton != null)
                _dismissButton.onClick.RemoveListener(HandleDismiss);
        }
    }
}
