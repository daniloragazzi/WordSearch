using UnityEngine;
using UnityEditor;
using System.IO;

namespace RagazziStudios.Editor
{
    /// <summary>
    /// Gera sprites procedurais para o jogo (botões, painéis, ícones).
    /// Menu: Build > Ragazzi Studios > Generate Sprites
    /// </summary>
    public static class SpriteGenerator
    {
        private const string BUTTONS_PATH = "Assets/_Project/Art/UI/Buttons";
        private const string PANELS_PATH = "Assets/_Project/Art/UI/Panels";
        private const string ICONS_PATH = "Assets/_Project/Art/UI/Icons";
        private const string GRID_PATH = "Assets/_Project/Art/UI/Grid";
        private const string APP_ICON_PATH = "Assets/_Project/Art/Icons";
        private const string SPLASH_PATH = "Assets/_Project/Art/Splash";

        [MenuItem("Build/Ragazzi Studios/Generate Sprites", priority = 3)]
        public static void GenerateAll()
        {
            GenerateButtonSprites();
            GeneratePanelSprites();
            GenerateGridSprites();
            GenerateCategoryIcons();
            GenerateAppIcon();
            GenerateSplashScreen();

            AssetDatabase.Refresh();
            Debug.Log("[SpriteGenerator] All sprites generated!");
        }

        // ═══════════════════════════════════════
        //  BOTÕES (9-slice rounded rect)
        // ═══════════════════════════════════════

        private static void GenerateButtonSprites()
        {
            // Botão primário (arredondado, branco — tintado pela cor no runtime)
            SaveRoundedRect($"{BUTTONS_PATH}/btn_primary.png", 256, 96, 32,
                Color.white, true);

            // Botão secundário (outline)
            SaveRoundedRectOutline($"{BUTTONS_PATH}/btn_secondary.png", 256, 96, 32, 4,
                Color.white);

            // Botão circular pequeno (back, close, pause)
            SaveCircle($"{BUTTONS_PATH}/btn_circle.png", 96, Color.white);

            ConfigureSpriteImport($"{BUTTONS_PATH}/btn_primary.png", 32);
            ConfigureSpriteImport($"{BUTTONS_PATH}/btn_secondary.png", 32);
            ConfigureSpriteImport($"{BUTTONS_PATH}/btn_circle.png", 0);
        }

        // ═══════════════════════════════════════
        //  PAINÉIS (9-slice rounded rect)
        // ═══════════════════════════════════════

        private static void GeneratePanelSprites()
        {
            // Painel de popup (bordas arredondadas)
            SaveRoundedRect($"{PANELS_PATH}/panel_popup.png", 256, 256, 24,
                Color.white, true);

            // Painel card (para categorias, itens)
            SaveRoundedRect($"{PANELS_PATH}/panel_card.png", 192, 192, 16,
                Color.white, true);

            // Overlay escuro semi-transparente
            SaveSolid($"{PANELS_PATH}/panel_overlay.png", 8, 8,
                new Color(0, 0, 0, 0.7f));

            ConfigureSpriteImport($"{PANELS_PATH}/panel_popup.png", 24);
            ConfigureSpriteImport($"{PANELS_PATH}/panel_card.png", 16);
            ConfigureSpriteImport($"{PANELS_PATH}/panel_overlay.png", 0);
        }

        // ═══════════════════════════════════════
        //  GRID
        // ═══════════════════════════════════════

        private static void GenerateGridSprites()
        {
            // Célula do grid (quadrado arredondado)
            SaveRoundedRect($"{GRID_PATH}/cell_bg.png", 96, 96, 12,
                Color.white, true);

            ConfigureSpriteImport($"{GRID_PATH}/cell_bg.png", 12);
        }

        // ═══════════════════════════════════════
        //  ÍCONES DE CATEGORIA
        // ═══════════════════════════════════════

        private static void GenerateCategoryIcons()
        {
            // Cada ícone é 128x128, branco sobre transparente
            // Formas geométricas simples representando cada categoria

            GenerateAnimalIcon();     // animais — pata
            GenerateFoodIcon();       // alimentos — maçã
            GenerateBodyIcon();       // corpo_humano — coração
            GenerateNatureIcon();     // natureza — folha
            GenerateJobsIcon();       // profissoes — estrela
            GenerateCountriesIcon();  // paises — globo
            GenerateSportsIcon();     // esportes — bola
            GenerateColorsIcon();     // cores_formas — paleta/quadrados

            // Configurar imports dos ícones
            string[] catIds = { "animais", "alimentos", "corpo_humano", "natureza",
                                "profissoes", "paises", "esportes", "cores_formas" };
            foreach (var id in catIds)
            {
                ConfigureSpriteImport($"{ICONS_PATH}/cat_{id}.png", 0);
            }

            // Copiar ícones para Resources (para carregar em runtime)
            CopyCategoryIconsToResources(catIds);
        }

        private static void CopyCategoryIconsToResources(string[] catIds)
        {
            string resourcesDir = "Assets/_Project/Resources/CategoryIcons";
            string fullDir = Path.Combine(
                Application.dataPath.Replace("/Assets", ""), resourcesDir);
            if (!Directory.Exists(fullDir))
                Directory.CreateDirectory(fullDir);

            foreach (var id in catIds)
            {
                string src = Path.Combine(
                    Application.dataPath.Replace("/Assets", ""),
                    $"{ICONS_PATH}/cat_{id}.png");
                string dst = Path.Combine(fullDir, $"cat_{id}.png");

                if (File.Exists(src))
                    File.Copy(src, dst, true);
            }

            AssetDatabase.Refresh();

            // Configurar imports na pasta Resources também
            foreach (var id in catIds)
            {
                ConfigureSpriteImport($"{resourcesDir}/cat_{id}.png", 0);
            }
        }

        // ═══════════════════════════════════════
        //  APP ICON
        // ═══════════════════════════════════════

        private static void GenerateAppIcon()
        {
            const int size = 512;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color32[size * size];

            // Background: gradiente radial azul
            Color bgCenter = new Color(0.29f, 0.56f, 0.89f); // #4A8FE3
            Color bgEdge = new Color(0.15f, 0.25f, 0.50f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - size / 2f) / (size / 2f);
                    float dy = (y - size / 2f) / (size / 2f);
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float t = Mathf.Clamp01(dist);
                    Color c = Color.Lerp(bgCenter, bgEdge, t * t);

                    // Rounded square mask
                    float margin = 40f;
                    float radius = 80f;
                    float alpha = RoundedRectAlpha(x, y, margin, margin,
                        size - margin, size - margin, radius);

                    c.a = alpha;
                    pixels[y * size + x] = c;
                }
            }

            // Desenhar grade 3x3 de pontos brancos (representação de word search)
            float dotR = 18f;
            float gridStart = 140f;
            float gridSpacing = 115f;

            for (int gy = 0; gy < 3; gy++)
            {
                for (int gx = 0; gx < 3; gx++)
                {
                    float cx = gridStart + gx * gridSpacing;
                    float cy = gridStart + gy * gridSpacing;

                    DrawFilledCircle(pixels, size, size, cx, cy, dotR,
                        new Color(1, 1, 1, 0.9f));
                }
            }

            // Linha diagonal sobre os pontos (representando seleção)
            DrawThickLine(pixels, size, size,
                gridStart, gridStart,
                gridStart + 2 * gridSpacing, gridStart + 2 * gridSpacing,
                8f, new Color(1f, 0.85f, 0.3f, 0.95f));

            tex.SetPixels32(pixels);
            tex.Apply();

            SaveTexture(tex, $"{APP_ICON_PATH}/app_icon.png");
            Object.DestroyImmediate(tex);

            ConfigureSpriteImport($"{APP_ICON_PATH}/app_icon.png", 0);
        }

        // ═══════════════════════════════════════
        //  SPLASH SCREEN
        // ═══════════════════════════════════════

        private static void GenerateSplashScreen()
        {
            const int w = 512, h = 512;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var pixels = new Color32[w * h];

            // BG transparente (a splash usa o backgroundColor do Player Settings)
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = new Color32(0, 0, 0, 0);

            // "R" estilizado no centro (Ragazzi)
            // Desenhar um "R" com linhas grossas
            Color letterColor = new Color(0.29f, 0.56f, 0.89f, 1f);

            // Haste vertical do R
            DrawThickLine(pixels, w, h, 180, 150, 180, 362, 16f, letterColor);
            // Arco superior (simplificado como linhas)
            DrawThickLine(pixels, w, h, 180, 362, 280, 362, 12f, letterColor);
            DrawThickLine(pixels, w, h, 280, 362, 310, 330, 12f, letterColor);
            DrawThickLine(pixels, w, h, 310, 330, 310, 290, 12f, letterColor);
            DrawThickLine(pixels, w, h, 310, 290, 280, 260, 12f, letterColor);
            DrawThickLine(pixels, w, h, 280, 260, 180, 260, 12f, letterColor);
            // Perna do R
            DrawThickLine(pixels, w, h, 240, 260, 320, 150, 12f, letterColor);

            tex.SetPixels32(pixels);
            tex.Apply();

            SaveTexture(tex, $"{SPLASH_PATH}/splash_logo.png");
            Object.DestroyImmediate(tex);
        }

        // ═══════════════════════════════════════
        //  CATEGORY ICON GENERATORS
        // ═══════════════════════════════════════

        private static void GenerateAnimalIcon()
        {
            // Pata de animal
            const int s = 128;
            var tex = CreateTransparentTexture(s, s);
            var px = tex.GetPixels32();

            // Almofada central (elipse)
            DrawFilledEllipse(px, s, s, 64, 42, 24, 18, Color.white);

            // 4 dedos
            DrawFilledCircle(px, s, s, 40, 78, 12, Color.white);
            DrawFilledCircle(px, s, s, 60, 88, 12, Color.white);
            DrawFilledCircle(px, s, s, 80, 88, 12, Color.white);
            DrawFilledCircle(px, s, s, 96, 78, 12, Color.white);

            tex.SetPixels32(px);
            tex.Apply();
            SaveTexture(tex, $"{ICONS_PATH}/cat_animais.png");
            Object.DestroyImmediate(tex);
        }

        private static void GenerateFoodIcon()
        {
            // Maçã simplificada
            const int s = 128;
            var tex = CreateTransparentTexture(s, s);
            var px = tex.GetPixels32();

            // Corpo da maçã (dois círculos sobrepostos)
            DrawFilledCircle(px, s, s, 52, 48, 28, Color.white);
            DrawFilledCircle(px, s, s, 76, 48, 28, Color.white);

            // Talo
            DrawThickLine(px, s, s, 64, 76, 68, 96, 4f, Color.white);

            // Folha
            DrawFilledEllipse(px, s, s, 80, 92, 12, 6, Color.white);

            tex.SetPixels32(px);
            tex.Apply();
            SaveTexture(tex, $"{ICONS_PATH}/cat_alimentos.png");
            Object.DestroyImmediate(tex);
        }

        private static void GenerateBodyIcon()
        {
            // Coração
            const int s = 128;
            var tex = CreateTransparentTexture(s, s);
            var px = tex.GetPixels32();

            // Dois círculos no topo
            DrawFilledCircle(px, s, s, 46, 72, 22, Color.white);
            DrawFilledCircle(px, s, s, 82, 72, 22, Color.white);

            // Triângulo inferior (preencher com linhas)
            for (int y = 20; y < 72; y++)
            {
                float progress = (72f - y) / 52f; // 0 no topo, 1 na ponta
                float halfWidth = 44f * (1f - progress);
                int left = (int)(64 - halfWidth);
                int right = (int)(64 + halfWidth);
                for (int x = left; x <= right && x < s; x++)
                {
                    if (x >= 0)
                        px[y * s + x] = Color.white;
                }
            }

            tex.SetPixels32(px);
            tex.Apply();
            SaveTexture(tex, $"{ICONS_PATH}/cat_corpo_humano.png");
            Object.DestroyImmediate(tex);
        }

        private static void GenerateNatureIcon()
        {
            // Folha
            const int s = 128;
            var tex = CreateTransparentTexture(s, s);
            var px = tex.GetPixels32();

            // Forma de folha (elipse inclinada + nervura central)
            for (int y = 0; y < s; y++)
            {
                for (int x = 0; x < s; x++)
                {
                    // Rotacionar 30 graus
                    float cx = x - 64f, cy = y - 64f;
                    float angle = 30f * Mathf.Deg2Rad;
                    float rx = cx * Mathf.Cos(angle) - cy * Mathf.Sin(angle);
                    float ry = cx * Mathf.Sin(angle) + cy * Mathf.Cos(angle);

                    // Elipse
                    float ex = rx / 22f;
                    float ey = ry / 40f;
                    if (ex * ex + ey * ey <= 1f)
                        px[y * s + x] = Color.white;
                }
            }

            // Nervura central
            DrawThickLine(px, s, s, 42, 30, 86, 98, 2f, new Color(0, 0, 0, 0));

            tex.SetPixels32(px);
            tex.Apply();
            SaveTexture(tex, $"{ICONS_PATH}/cat_natureza.png");
            Object.DestroyImmediate(tex);
        }

        private static void GenerateJobsIcon()
        {
            // Estrela 5 pontas
            const int s = 128;
            var tex = CreateTransparentTexture(s, s);
            var px = tex.GetPixels32();

            float cx = 64, cy = 64;
            float outerR = 38, innerR = 16;
            int points = 5;

            var verts = new Vector2[points * 2];
            for (int i = 0; i < points * 2; i++)
            {
                float angle = (90f + i * 36f) * Mathf.Deg2Rad;
                float r = (i % 2 == 0) ? outerR : innerR;
                verts[i] = new Vector2(cx + Mathf.Cos(angle) * r,
                                       cy + Mathf.Sin(angle) * r);
            }

            // Preencher polígono estrela
            FillPolygon(px, s, s, verts, Color.white);

            tex.SetPixels32(px);
            tex.Apply();
            SaveTexture(tex, $"{ICONS_PATH}/cat_profissoes.png");
            Object.DestroyImmediate(tex);
        }

        private static void GenerateCountriesIcon()
        {
            // Globo (círculo com linhas)
            const int s = 128;
            var tex = CreateTransparentTexture(s, s);
            var px = tex.GetPixels32();

            // Círculo exterior
            DrawCircleOutline(px, s, s, 64, 64, 36, 3f, Color.white);

            // Meridiano vertical (elipse)
            DrawEllipseOutline(px, s, s, 64, 64, 18, 36, 2f, Color.white);

            // Linhas horizontais (latitudes)
            DrawThickLine(px, s, s, 28, 50, 100, 50, 2f, Color.white);
            DrawThickLine(px, s, s, 28, 64, 100, 64, 2f, Color.white);
            DrawThickLine(px, s, s, 28, 78, 100, 78, 2f, Color.white);

            // Mascarar o que está fora do círculo
            MaskCircle(px, s, s, 64, 64, 37);

            tex.SetPixels32(px);
            tex.Apply();
            SaveTexture(tex, $"{ICONS_PATH}/cat_paises.png");
            Object.DestroyImmediate(tex);
        }

        private static void GenerateSportsIcon()
        {
            // Bola (círculo com padrão)
            const int s = 128;
            var tex = CreateTransparentTexture(s, s);
            var px = tex.GetPixels32();

            // Círculo cheio
            DrawFilledCircle(px, s, s, 64, 64, 36, Color.white);

            // Linhas cruzadas (padrão de bola)
            DrawThickLine(px, s, s, 64, 28, 64, 100, 2.5f,
                new Color(1, 1, 1, 0)); // transparente (corte)
            DrawThickLine(px, s, s, 28, 64, 100, 64, 2.5f,
                new Color(1, 1, 1, 0));

            tex.SetPixels32(px);
            tex.Apply();
            SaveTexture(tex, $"{ICONS_PATH}/cat_esportes.png");
            Object.DestroyImmediate(tex);
        }

        private static void GenerateColorsIcon()
        {
            // Quadrados coloridos 2x2 (representando cores e formas)
            const int s = 128;
            var tex = CreateTransparentTexture(s, s);
            var px = tex.GetPixels32();

            int gap = 6, boxSize = 44;
            int startX = 64 - boxSize - gap / 2;
            int startY = 64 - boxSize - gap / 2;

            // 4 quadrados arredondados
            FillRoundedRectRegion(px, s, s, startX, startY + boxSize + gap,
                boxSize, boxSize, 8, Color.white);
            FillRoundedRectRegion(px, s, s, startX + boxSize + gap, startY + boxSize + gap,
                boxSize, boxSize, 8, new Color(1, 1, 1, 0.85f));
            FillRoundedRectRegion(px, s, s, startX, startY,
                boxSize, boxSize, 8, new Color(1, 1, 1, 0.7f));

            // Círculo no canto inferior direito
            DrawFilledCircle(px, s, s, startX + boxSize + gap + boxSize / 2,
                startY + boxSize / 2, boxSize / 2 - 2, new Color(1, 1, 1, 0.55f));

            tex.SetPixels32(px);
            tex.Apply();
            SaveTexture(tex, $"{ICONS_PATH}/cat_cores_formas.png");
            Object.DestroyImmediate(tex);
        }

        // ═══════════════════════════════════════
        //  DRAWING PRIMITIVES
        // ═══════════════════════════════════════

        private static Texture2D CreateTransparentTexture(int w, int h)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var clear = new Color32[w * h];
            tex.SetPixels32(clear);
            return tex;
        }

        private static void SaveRoundedRect(string path, int w, int h, int radius,
            Color color, bool fill)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var pixels = new Color32[w * h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float alpha = RoundedRectAlpha(x, y, 0, 0, w, h, radius);
                    pixels[y * w + x] = new Color(color.r, color.g, color.b,
                        color.a * alpha);
                }
            }

            tex.SetPixels32(pixels);
            tex.Apply();
            SaveTexture(tex, path);
            Object.DestroyImmediate(tex);
        }

        private static void SaveRoundedRectOutline(string path, int w, int h,
            int radius, int thickness, Color color)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var pixels = new Color32[w * h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float outer = RoundedRectAlpha(x, y, 0, 0, w, h, radius);
                    float inner = RoundedRectAlpha(x, y, thickness, thickness,
                        w - thickness, h - thickness,
                        Mathf.Max(0, radius - thickness));
                    float alpha = outer - inner;
                    pixels[y * w + x] = new Color(color.r, color.g, color.b,
                        color.a * Mathf.Clamp01(alpha));
                }
            }

            tex.SetPixels32(pixels);
            tex.Apply();
            SaveTexture(tex, path);
            Object.DestroyImmediate(tex);
        }

        private static void SaveCircle(string path, int size, Color color)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color32[size * size];
            float r = size * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x + 0.5f - r;
                    float dy = y + 0.5f - r;
                    float dist = r - Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01(dist / 1.5f + 0.5f);
                    pixels[y * size + x] = new Color(color.r, color.g, color.b,
                        color.a * alpha);
                }
            }

            tex.SetPixels32(pixels);
            tex.Apply();
            SaveTexture(tex, path);
            Object.DestroyImmediate(tex);
        }

        private static void SaveSolid(string path, int w, int h, Color color)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var pixels = new Color32[w * h];
            Color32 c32 = color;
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = c32;

            tex.SetPixels32(pixels);
            tex.Apply();
            SaveTexture(tex, path);
            Object.DestroyImmediate(tex);
        }

        private static float RoundedRectAlpha(float px, float py,
            float left, float bottom, float right, float top, float radius)
        {
            float x = px + 0.5f;
            float y = py + 0.5f;

            // Clamp a posição para dentro do retângulo com cantos arredondados
            float cLeft = left + radius;
            float cRight = right - radius;
            float cBottom = bottom + radius;
            float cTop = top - radius;

            float dist;

            if (x < cLeft && y < cBottom)
            {
                // Canto inferior esquerdo
                float dx = x - cLeft;
                float dy = y - cBottom;
                dist = radius - Mathf.Sqrt(dx * dx + dy * dy);
            }
            else if (x > cRight && y < cBottom)
            {
                float dx = x - cRight;
                float dy = y - cBottom;
                dist = radius - Mathf.Sqrt(dx * dx + dy * dy);
            }
            else if (x < cLeft && y > cTop)
            {
                float dx = x - cLeft;
                float dy = y - cTop;
                dist = radius - Mathf.Sqrt(dx * dx + dy * dy);
            }
            else if (x > cRight && y > cTop)
            {
                float dx = x - cRight;
                float dy = y - cTop;
                dist = radius - Mathf.Sqrt(dx * dx + dy * dy);
            }
            else
            {
                // Dentro do retângulo (sem cantos)
                dist = Mathf.Min(
                    x - left, right - x,
                    y - bottom, top - y);
            }

            return Mathf.Clamp01(dist / 1.5f + 0.5f);
        }

        private static void DrawFilledCircle(Color32[] pixels, int w, int h,
            float cx, float cy, float radius, Color color)
        {
            int minX = Mathf.Max(0, (int)(cx - radius - 2));
            int maxX = Mathf.Min(w - 1, (int)(cx + radius + 2));
            int minY = Mathf.Max(0, (int)(cy - radius - 2));
            int maxY = Mathf.Min(h - 1, (int)(cy + radius + 2));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    float dist = radius - Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01(dist / 1.5f + 0.5f);

                    if (alpha > 0)
                        pixels[y * w + x] = BlendColor(pixels[y * w + x],
                            new Color(color.r, color.g, color.b, color.a * alpha));
                }
            }
        }

        private static void DrawFilledEllipse(Color32[] pixels, int w, int h,
            float cx, float cy, float rx, float ry, Color color)
        {
            int minX = Mathf.Max(0, (int)(cx - rx - 2));
            int maxX = Mathf.Min(w - 1, (int)(cx + rx + 2));
            int minY = Mathf.Max(0, (int)(cy - ry - 2));
            int maxY = Mathf.Min(h - 1, (int)(cy + ry + 2));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dx = (x + 0.5f - cx) / rx;
                    float dy = (y + 0.5f - cy) / ry;
                    float d = 1f - Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01(d * rx * 0.5f + 0.5f);

                    if (alpha > 0)
                        pixels[y * w + x] = BlendColor(pixels[y * w + x],
                            new Color(color.r, color.g, color.b, color.a * alpha));
                }
            }
        }

        private static void DrawThickLine(Color32[] pixels, int w, int h,
            float x0, float y0, float x1, float y1, float thickness, Color color)
        {
            float dx = x1 - x0, dy = y1 - y0;
            float len = Mathf.Sqrt(dx * dx + dy * dy);
            if (len < 0.01f) return;

            float nx = -dy / len, ny = dx / len;
            float halfT = thickness * 0.5f;

            int minX = Mathf.Max(0, (int)(Mathf.Min(x0, x1) - halfT - 2));
            int maxX = Mathf.Min(w - 1, (int)(Mathf.Max(x0, x1) + halfT + 2));
            int minY = Mathf.Max(0, (int)(Mathf.Min(y0, y1) - halfT - 2));
            int maxY = Mathf.Min(h - 1, (int)(Mathf.Max(y0, y1) + halfT + 2));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float px = x + 0.5f - x0;
                    float py = y + 0.5f - y0;

                    // Projeção no segmento
                    float t = (px * dx + py * dy) / (len * len);
                    t = Mathf.Clamp01(t);

                    float closestX = x0 + t * dx;
                    float closestY = y0 + t * dy;

                    float ddx = x + 0.5f - closestX;
                    float ddy = y + 0.5f - closestY;
                    float dist = halfT - Mathf.Sqrt(ddx * ddx + ddy * ddy);
                    float alpha = Mathf.Clamp01(dist / 1.5f + 0.5f);

                    if (alpha > 0)
                    {
                        if (color.a < 0.01f)
                        {
                            // "Apagar" — tornar transparente
                            Color32 existing = pixels[y * w + x];
                            existing.a = (byte)(existing.a * (1f - alpha));
                            pixels[y * w + x] = existing;
                        }
                        else
                        {
                            pixels[y * w + x] = BlendColor(pixels[y * w + x],
                                new Color(color.r, color.g, color.b, color.a * alpha));
                        }
                    }
                }
            }
        }

        private static void DrawCircleOutline(Color32[] pixels, int w, int h,
            float cx, float cy, float radius, float thickness, Color color)
        {
            float halfT = thickness * 0.5f;
            int minX = Mathf.Max(0, (int)(cx - radius - halfT - 2));
            int maxX = Mathf.Min(w - 1, (int)(cx + radius + halfT + 2));
            int minY = Mathf.Max(0, (int)(cy - radius - halfT - 2));
            int maxY = Mathf.Min(h - 1, (int)(cy + radius + halfT + 2));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    float d = Mathf.Abs(Mathf.Sqrt(dx * dx + dy * dy) - radius);
                    float alpha = Mathf.Clamp01((halfT - d) / 1.5f + 0.5f);

                    if (alpha > 0)
                        pixels[y * w + x] = BlendColor(pixels[y * w + x],
                            new Color(color.r, color.g, color.b, color.a * alpha));
                }
            }
        }

        private static void DrawEllipseOutline(Color32[] pixels, int w, int h,
            float cx, float cy, float rx, float ry, float thickness, Color color)
        {
            float halfT = thickness * 0.5f;
            int minX = Mathf.Max(0, (int)(cx - rx - halfT - 2));
            int maxX = Mathf.Min(w - 1, (int)(cx + rx + halfT + 2));
            int minY = Mathf.Max(0, (int)(cy - ry - halfT - 2));
            int maxY = Mathf.Min(h - 1, (int)(cy + ry + halfT + 2));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dx = (x + 0.5f - cx) / rx;
                    float dy = (y + 0.5f - cy) / ry;
                    float elDist = Mathf.Sqrt(dx * dx + dy * dy);
                    float d = Mathf.Abs(elDist - 1f) * Mathf.Min(rx, ry);
                    float alpha = Mathf.Clamp01((halfT - d) / 1.5f + 0.5f);

                    if (alpha > 0)
                        pixels[y * w + x] = BlendColor(pixels[y * w + x],
                            new Color(color.r, color.g, color.b, color.a * alpha));
                }
            }
        }

        private static void MaskCircle(Color32[] pixels, int w, int h,
            float cx, float cy, float radius)
        {
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    float dist = radius - Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01(dist / 1.5f + 0.5f);

                    if (alpha < 1f)
                    {
                        Color32 c = pixels[y * w + x];
                        c.a = (byte)(c.a * alpha);
                        pixels[y * w + x] = c;
                    }
                }
            }
        }

        private static void FillPolygon(Color32[] pixels, int w, int h,
            Vector2[] verts, Color color)
        {
            // Simple scanline fill
            float minY = float.MaxValue, maxY = float.MinValue;
            foreach (var v in verts)
            {
                if (v.y < minY) minY = v.y;
                if (v.y > maxY) maxY = v.y;
            }

            for (int y = Mathf.Max(0, (int)minY); y <= Mathf.Min(h - 1, (int)maxY); y++)
            {
                var intersections = new System.Collections.Generic.List<float>();
                float scanY = y + 0.5f;

                for (int i = 0; i < verts.Length; i++)
                {
                    int j = (i + 1) % verts.Length;
                    float y1 = verts[i].y, y2 = verts[j].y;
                    if ((y1 <= scanY && y2 > scanY) || (y2 <= scanY && y1 > scanY))
                    {
                        float t = (scanY - y1) / (y2 - y1);
                        intersections.Add(verts[i].x + t * (verts[j].x - verts[i].x));
                    }
                }

                intersections.Sort();

                for (int i = 0; i + 1 < intersections.Count; i += 2)
                {
                    int xStart = Mathf.Max(0, (int)intersections[i]);
                    int xEnd = Mathf.Min(w - 1, (int)intersections[i + 1]);
                    for (int x = xStart; x <= xEnd; x++)
                    {
                        pixels[y * w + x] = BlendColor(pixels[y * w + x],
                            (Color32)color);
                    }
                }
            }
        }

        private static void FillRoundedRectRegion(Color32[] pixels, int w, int h,
            int rx, int ry, int rw, int rh, int radius, Color color)
        {
            for (int y = ry; y < ry + rh && y < h; y++)
            {
                for (int x = rx; x < rx + rw && x < w; x++)
                {
                    if (x < 0 || y < 0) continue;
                    float alpha = RoundedRectAlpha(x - rx, y - ry,
                        0, 0, rw, rh, radius);
                    if (alpha > 0)
                        pixels[y * w + x] = BlendColor(pixels[y * w + x],
                            new Color(color.r, color.g, color.b, color.a * alpha));
                }
            }
        }

        private static Color32 BlendColor(Color32 bg, Color fg)
        {
            float a = fg.a / 255f;
            float invA = 1f - a;
            return new Color32(
                (byte)(fg.r * a + bg.r * invA),
                (byte)(fg.g * a + bg.g * invA),
                (byte)(fg.b * a + bg.b * invA),
                (byte)Mathf.Min(255, bg.a + fg.a));
        }

        // ═══════════════════════════════════════
        //  FILE I/O
        // ═══════════════════════════════════════

        private static void SaveTexture(Texture2D tex, string assetPath)
        {
            string fullPath = Path.Combine(
                Application.dataPath.Replace("/Assets", ""), assetPath);
            string dir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllBytes(fullPath, tex.EncodeToPNG());
            Debug.Log($"[SpriteGenerator] Saved: {assetPath}");
        }

        private static void ConfigureSpriteImport(string assetPath, int border)
        {
            AssetDatabase.Refresh();

            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;

            if (border > 0)
            {
                importer.spriteBorder = new Vector4(border, border, border, border);
            }

            importer.SaveAndReimport();
        }
    }
}
