using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RagazziStudios.Game.UI.Components
{
    /// <summary>
    /// Efeito de confete/celebração usando UI Images.
    /// Cria e anima pedaços coloridos caindo pela tela.
    /// Usa CanvasGroup para fade e se auto-destrói ao finalizar.
    /// </summary>
    public class ConfettiEffect : MonoBehaviour
    {
        [Header("Configuração")]
        [SerializeField] private int _particleCount = 60;
        [SerializeField] private float _duration = 2.5f;
        [SerializeField] private float _spawnDuration = 0.6f;

        /// <summary>Cores do confete.</summary>
        private static readonly Color[] Colors = new Color[]
        {
            new Color(1.0f, 0.35f, 0.35f), // vermelho
            new Color(0.35f, 0.78f, 0.35f), // verde
            new Color(0.40f, 0.60f, 1.00f), // azul
            new Color(1.00f, 0.85f, 0.20f), // amarelo
            new Color(0.90f, 0.55f, 0.20f), // laranja
            new Color(0.70f, 0.35f, 0.75f), // roxo
            new Color(1.00f, 0.50f, 0.70f), // rosa
            new Color(0.35f, 0.85f, 0.85f), // ciano
        };

        /// <summary>
        /// Cria e inicia o efeito de confete. Retorna o GO (auto-destrói).
        /// </summary>
        public static ConfettiEffect Create(Transform parent)
        {
            var go = new GameObject("ConfettiEffect");
            go.transform.SetParent(parent, false);

            // Canvas overlay interno
            var canvas = go.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 100;

            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();

            // Cobrir toda a tela (não bloquear raycast)
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            var effect = go.AddComponent<ConfettiEffect>();
            return effect;
        }

        private void Start()
        {
            StartCoroutine(RunEffect());
        }

        private IEnumerator RunEffect()
        {
            var rt = GetComponent<RectTransform>();
            float screenW = Screen.width;
            float screenH = Screen.height;

            // Spawnar partículas ao longo do spawnDuration
            float interval = _spawnDuration / _particleCount;

            for (int i = 0; i < _particleCount; i++)
            {
                SpawnParticle(rt, screenW, screenH);

                if (interval > 0.001f)
                    yield return new WaitForSeconds(interval);
            }

            // Esperar o restante da duração
            yield return new WaitForSeconds(_duration - _spawnDuration);

            // Auto-destruir
            Destroy(gameObject);
        }

        private void SpawnParticle(RectTransform parentRT, float screenW, float screenH)
        {
            var pGO = new GameObject("Confetti");
            pGO.transform.SetParent(parentRT, false);

            var img = pGO.AddComponent<Image>();
            img.color = Colors[Random.Range(0, Colors.Length)];
            img.raycastTarget = false;

            var pRT = pGO.GetComponent<RectTransform>();

            // Tamanho variado (retangulares — tipo papel picado)
            float w = Random.Range(8f, 18f);
            float h = Random.Range(4f, 12f);
            pRT.sizeDelta = new Vector2(w, h);

            // Posição inicial: topo da tela, X aleatório
            float startX = Random.Range(-screenW * 0.5f, screenW * 0.5f);
            float startY = screenH * 0.5f + Random.Range(20f, 80f);
            pRT.anchoredPosition = new Vector2(startX, startY);

            // Rotação inicial aleatória
            pRT.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

            // Animar queda
            StartCoroutine(AnimateParticle(pRT, img));
        }

        private IEnumerator AnimateParticle(RectTransform pRT, Image img)
        {
            float fallSpeed = Random.Range(400f, 800f);
            float swayFreq = Random.Range(2f, 5f);
            float swayAmp = Random.Range(30f, 80f);
            float rotSpeed = Random.Range(-360f, 360f);
            float lifetime = Random.Range(1.5f, _duration);
            float fadeStart = lifetime * 0.7f;

            float startX = pRT.anchoredPosition.x;
            float elapsed = 0f;
            Color baseColor = img.color;

            while (elapsed < lifetime && pRT != null)
            {
                elapsed += Time.deltaTime;

                // Posição: queda + sway lateral
                float y = pRT.anchoredPosition.y - fallSpeed * Time.deltaTime;
                float x = startX + Mathf.Sin(elapsed * swayFreq) * swayAmp;
                pRT.anchoredPosition = new Vector2(x, y);

                // Rotação contínua
                pRT.Rotate(0, 0, rotSpeed * Time.deltaTime);

                // Fade out no final
                if (elapsed > fadeStart)
                {
                    float alpha = 1f - (elapsed - fadeStart) / (lifetime - fadeStart);
                    img.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
                }

                yield return null;
            }

            if (pRT != null)
                Destroy(pRT.gameObject);
        }
    }
}
