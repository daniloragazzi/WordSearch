using UnityEngine;
using UnityEngine.UI;

namespace RagazziStudios.Game.Config
{
    /// <summary>
    /// Gera sprites placeholder proceduralmente em runtime.
    /// Permite rodar o MVP sem assets gráficos prontos.
    /// Usar apenas durante desenvolvimento — substituir por sprites reais para produção.
    /// </summary>
    public static class PlaceholderSprites
    {
        private static Sprite _roundedRect;
        private static Sprite _circle;
        private static Sprite _cell;

        /// <summary>
        /// Retângulo arredondado 9-slice (botões, painéis).
        /// </summary>
        public static Sprite RoundedRect
        {
            get
            {
                if (_roundedRect == null)
                    _roundedRect = CreateRoundedRect(128, 128, 16);
                return _roundedRect;
            }
        }

        /// <summary>
        /// Círculo (botões circulares).
        /// </summary>
        public static Sprite Circle
        {
            get
            {
                if (_circle == null)
                    _circle = CreateCircle(64);
                return _circle;
            }
        }

        /// <summary>
        /// Célula do grid (quadrado levemente arredondado).
        /// </summary>
        public static Sprite Cell
        {
            get
            {
                if (_cell == null)
                    _cell = CreateRoundedRect(64, 64, 8);
                return _cell;
            }
        }

        /// <summary>
        /// Cria uma textura retangular com cantos arredondados.
        /// </summary>
        public static Sprite CreateRoundedRect(int width, int height, int radius)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var pixels = new Color32[width * height];

            Color32 white = new Color32(255, 255, 255, 255);
            Color32 clear = new Color32(0, 0, 0, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool inside = IsInsideRoundedRect(x, y, width, height, radius);
                    pixels[y * width + x] = inside ? white : clear;
                }
            }

            tex.SetPixels32(pixels);
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;

            // 9-slice borders
            Vector4 border = new Vector4(radius + 1, radius + 1, radius + 1, radius + 1);
            return Sprite.Create(tex,
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0.5f),
                100f, 0, SpriteMeshType.FullRect, border);
        }

        /// <summary>
        /// Cria uma textura circular.
        /// </summary>
        public static Sprite CreateCircle(int diameter)
        {
            var tex = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false);
            var pixels = new Color32[diameter * diameter];

            float center = diameter / 2f;
            float radiusSq = center * center;

            Color32 white = new Color32(255, 255, 255, 255);
            Color32 clear = new Color32(0, 0, 0, 0);

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    float dx = x - center + 0.5f;
                    float dy = y - center + 0.5f;
                    float distSq = dx * dx + dy * dy;

                    // Anti-aliasing simples
                    if (distSq < radiusSq - center)
                        pixels[y * diameter + x] = white;
                    else if (distSq < radiusSq + center)
                    {
                        float alpha = 1f - (distSq - radiusSq + center) / (2f * center);
                        pixels[y * diameter + x] = new Color32(255, 255, 255,
                            (byte)(Mathf.Clamp01(alpha) * 255));
                    }
                    else
                        pixels[y * diameter + x] = clear;
                }
            }

            tex.SetPixels32(pixels);
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;

            return Sprite.Create(tex,
                new Rect(0, 0, diameter, diameter),
                new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Cria uma linha horizontal (para selection line).
        /// </summary>
        public static Sprite CreateLine(int width, int height)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var pixels = new Color32[width * height];

            Color32 white = new Color32(255, 255, 255, 255);
            int radius = height / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Cantos arredondados nas extremidades
                    bool inside;
                    if (x < radius)
                    {
                        float dx = x - radius + 0.5f;
                        float dy = y - radius + 0.5f;
                        inside = dx * dx + dy * dy <= radius * radius;
                    }
                    else if (x >= width - radius)
                    {
                        float dx = x - (width - radius) + 0.5f;
                        float dy = y - radius + 0.5f;
                        inside = dx * dx + dy * dy <= radius * radius;
                    }
                    else
                    {
                        inside = true;
                    }

                    pixels[y * width + x] = inside ? white : new Color32(0, 0, 0, 0);
                }
            }

            tex.SetPixels32(pixels);
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;

            // Slice nas extremidades
            Vector4 border = new Vector4(radius, 0, radius, 0);
            return Sprite.Create(tex,
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0.5f),
                100f, 0, SpriteMeshType.FullRect, border);
        }

        /// <summary>
        /// Cria uma barra de progresso (fundo ou fill).
        /// </summary>
        public static Sprite CreateProgressBar(int width, int height)
        {
            return CreateRoundedRect(width, height, height / 2);
        }

        private static bool IsInsideRoundedRect(int x, int y,
            int width, int height, int radius)
        {
            // Verificar cantos
            if (x < radius && y < radius)
                return IsInsideCircle(x, y, radius, radius);
            if (x >= width - radius && y < radius)
                return IsInsideCircle(x, y, width - radius - 1, radius);
            if (x < radius && y >= height - radius)
                return IsInsideCircle(x, y, radius, height - radius - 1);
            if (x >= width - radius && y >= height - radius)
                return IsInsideCircle(x, y, width - radius - 1, height - radius - 1);

            return true;
        }

        private static bool IsInsideCircle(int x, int y, int cx, int cy)
        {
            // Raio é implícito pelo offset passado
            // Usar distância Manhattan aproximada para performance
            float dx = x - cx;
            float dy = y - cy;
            // Como o radius é passado como cx ou cy, pegamos o menor
            int r = Mathf.Min(cx, cy);
            return dx * dx + dy * dy <= r * r;
        }

        /// <summary>
        /// Aplica sprites placeholder em uma Image se ela não tiver sprite.
        /// Útil para setup rápido de prefabs.
        /// </summary>
        public static void ApplyIfMissing(Image image, Sprite placeholder)
        {
            if (image != null && image.sprite == null)
                image.sprite = placeholder;
        }
    }
}
