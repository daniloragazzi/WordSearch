using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using System.IO;

namespace RagazziStudios.Editor
{
    /// <summary>
    /// Generates app icons with a real game grid visual (8×8 word search).
    /// Produces master, Play Store, adaptive foreground/background, and legacy/round variants.
    /// Menu: Build > Ragazzi Studios > Generate App Icon (Game Grid)
    /// </summary>
    public static class AppIconGenerator
    {
        private const string ICONS_PATH = "Assets/_Project/Art/Icons";

        // ═══════════════════════════════════════
        //  GRID CONTENT (from game screenshot)
        // ═══════════════════════════════════════

        private static readonly string[] GridRows =
        {
            "IFDLQRYZ",
            "REICEAZX",
            "KRARPIDF",
            "NRGUOOEC",
            "MUOZNOJR",
            "FGNBTBQW",
            "GEAGOPEM",
            "DMLCUBOG"
        };

        private static bool IsFoundCell(int row, int col)
        {
            // FERRUGEM vertical: col 1, all 8 rows
            if (col == 1) return true;
            // CUBO horizontal: row 7, cols 3–6
            if (row == 7 && col >= 3 && col <= 6) return true;
            return false;
        }

        // ═══════════════════════════════════════
        //  5×7 BITMAP FONT (A–Z)
        // ═══════════════════════════════════════

        // Each letter: 7 rows, each row = 5 bits (bit4=leftmost ... bit0=rightmost)
        private static readonly byte[][] Font =
        {
            new byte[] {0x0E,0x11,0x11,0x1F,0x11,0x11,0x11}, // A
            new byte[] {0x1E,0x11,0x11,0x1E,0x11,0x11,0x1E}, // B
            new byte[] {0x0E,0x11,0x10,0x10,0x10,0x11,0x0E}, // C
            new byte[] {0x1C,0x12,0x11,0x11,0x11,0x12,0x1C}, // D
            new byte[] {0x1F,0x10,0x10,0x1C,0x10,0x10,0x1F}, // E
            new byte[] {0x1F,0x10,0x10,0x1C,0x10,0x10,0x10}, // F
            new byte[] {0x0E,0x11,0x10,0x17,0x11,0x11,0x0E}, // G
            new byte[] {0x11,0x11,0x11,0x1F,0x11,0x11,0x11}, // H
            new byte[] {0x0E,0x04,0x04,0x04,0x04,0x04,0x0E}, // I
            new byte[] {0x07,0x02,0x02,0x02,0x02,0x12,0x0C}, // J
            new byte[] {0x11,0x12,0x14,0x18,0x14,0x12,0x11}, // K
            new byte[] {0x10,0x10,0x10,0x10,0x10,0x10,0x1F}, // L
            new byte[] {0x11,0x1B,0x15,0x11,0x11,0x11,0x11}, // M
            new byte[] {0x11,0x19,0x15,0x13,0x11,0x11,0x11}, // N
            new byte[] {0x0E,0x11,0x11,0x11,0x11,0x11,0x0E}, // O
            new byte[] {0x1E,0x11,0x11,0x1E,0x10,0x10,0x10}, // P
            new byte[] {0x0E,0x11,0x11,0x11,0x15,0x12,0x0D}, // Q
            new byte[] {0x1E,0x11,0x11,0x1E,0x14,0x12,0x11}, // R
            new byte[] {0x0E,0x11,0x10,0x0E,0x01,0x11,0x0E}, // S
            new byte[] {0x1F,0x04,0x04,0x04,0x04,0x04,0x04}, // T
            new byte[] {0x11,0x11,0x11,0x11,0x11,0x11,0x0E}, // U
            new byte[] {0x11,0x11,0x11,0x11,0x0A,0x0A,0x04}, // V
            new byte[] {0x11,0x11,0x11,0x15,0x15,0x1B,0x11}, // W
            new byte[] {0x11,0x11,0x0A,0x04,0x0A,0x11,0x11}, // X
            new byte[] {0x11,0x11,0x0A,0x04,0x04,0x04,0x04}, // Y
            new byte[] {0x1F,0x01,0x02,0x04,0x08,0x10,0x1F}, // Z
        };

        // ═══════════════════════════════════════
        //  COLORS
        // ═══════════════════════════════════════

        private static readonly Color32 DarkBg = new Color32(28, 35, 51, 255);
        private static readonly Color32 CellWhite = new Color32(244, 247, 252, 255);
        private static readonly Color32 CellGreen = new Color32(56, 212, 97, 255);
        private static readonly Color32 LetterDark = new Color32(46, 51, 66, 255);
        private static readonly Color32 LetterLight = new Color32(255, 255, 255, 255);

        // ═══════════════════════════════════════
        //  ENTRY POINT
        // ═══════════════════════════════════════

        [MenuItem("Build/Ragazzi Studios/Generate App Icon (Game Grid)", priority = 6)]
        public static void Generate()
        {
            string fullDir = Path.Combine(
                Application.dataPath.Replace("/Assets", ""), ICONS_PATH);
            if (!Directory.Exists(fullDir)) Directory.CreateDirectory(fullDir);

            // 1. Master 1024×1024 (Play Store / high-res)
            var master = DrawIcon(1024);
            SavePNG(master, $"{ICONS_PATH}/app_icon_1024.png");

            // 2. Combined 512×512 (legacy/round base — overwrites old app_icon)
            var combined = DrawIcon(512);
            SavePNG(combined, $"{ICONS_PATH}/app_icon.png");

            // 3. Adaptive foreground (grid on transparent bg, safe-zone aware)
            var fg = DrawAdaptiveForeground(432);
            SavePNG(fg, $"{ICONS_PATH}/app_icon_foreground.png");

            // 4. Adaptive background (solid dark)
            var bg = DrawAdaptiveBackground(432);
            SavePNG(bg, $"{ICONS_PATH}/app_icon_background.png");

            Object.DestroyImmediate(master);
            Object.DestroyImmediate(combined);
            Object.DestroyImmediate(fg);
            Object.DestroyImmediate(bg);

            AssetDatabase.Refresh();
            ConfigureImports();
            AssignToPlayerSettings();

            Debug.Log("[AppIconGenerator] All app icons generated and configured!");
        }

        // ═══════════════════════════════════════
        //  ICON VARIANTS
        // ═══════════════════════════════════════

        /// <summary>Combined icon: dark bg + 8×8 grid with letters. For legacy/round.</summary>
        private static Texture2D DrawIcon(int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var px = new Color32[size * size];

            for (int i = 0; i < px.Length; i++) px[i] = DarkBg;

            float margin = size * 0.05f;
            float area = size - margin * 2;
            DrawGrid(px, size, size, margin, margin, area, area, size >= 72);

            tex.SetPixels32(px);
            tex.Apply();
            return tex;
        }

        /// <summary>Foreground layer: grid on transparent bg, centered in safe zone.</summary>
        private static Texture2D DrawAdaptiveForeground(int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var px = new Color32[size * size];

            for (int i = 0; i < px.Length; i++) px[i] = new Color32(0, 0, 0, 0);

            // Adaptive safe zone: inner 66.7% (72dp of 108dp canvas)
            float padding = size * 0.167f;
            float area = size - padding * 2;
            DrawGrid(px, size, size, padding, padding, area, area, true);

            tex.SetPixels32(px);
            tex.Apply();
            return tex;
        }

        /// <summary>Background layer: solid dark color.</summary>
        private static Texture2D DrawAdaptiveBackground(int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var px = new Color32[size * size];

            for (int i = 0; i < px.Length; i++) px[i] = DarkBg;

            tex.SetPixels32(px);
            tex.Apply();
            return tex;
        }

        // ═══════════════════════════════════════
        //  GRID DRAWING
        // ═══════════════════════════════════════

        private static void DrawGrid(Color32[] pixels, int texW, int texH,
            float originX, float originY, float areaW, float areaH, bool drawLetters)
        {
            const int cols = 8, rows = 8;
            float gap = Mathf.Max(1f, areaW * 0.012f);
            float cellW = (areaW - gap * (cols - 1)) / cols;
            float cellH = (areaH - gap * (rows - 1)) / rows;
            float cell = Mathf.Min(cellW, cellH);
            float corner = Mathf.Max(1f, cell * 0.10f);

            // Center the grid within the area
            float totalW = cell * cols + gap * (cols - 1);
            float totalH = cell * rows + gap * (rows - 1);
            float startX = originX + (areaW - totalW) / 2f;
            float startY = originY + (areaH - totalH) / 2f;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    bool found = IsFoundCell(r, c);
                    Color32 cc = found ? CellGreen : CellWhite;

                    // Texture coords: Y increases upward, row 0 = top of grid
                    float cx = startX + c * (cell + gap);
                    float cy = startY + (rows - 1 - r) * (cell + gap);

                    FillRoundedRect(pixels, texW, texH,
                        cx, cy, cell, cell, corner, cc);

                    if (drawLetters && cell >= 10f)
                    {
                        char letter = GridRows[r][c];
                        Color32 lc = found ? LetterLight : LetterDark;
                        DrawLetter(pixels, texW, texH, letter, cx, cy, cell, lc);
                    }
                }
            }
        }

        // ═══════════════════════════════════════
        //  LETTER RENDERING (5×7 bitmap font)
        // ═══════════════════════════════════════

        private static void DrawLetter(Color32[] pixels, int texW, int texH,
            char ch, float cellX, float cellY, float cellSize, Color32 color)
        {
            int idx = ch - 'A';
            if (idx < 0 || idx >= Font.Length) return;

            byte[] glyph = Font[idx];

            // Letter area: ~50% width × 60% height, centered in cell
            float letterW = cellSize * 0.50f;
            float letterH = cellSize * 0.60f;
            float pixW = letterW / 5f;
            float pixH = letterH / 7f;
            float offsetX = cellX + (cellSize - letterW) / 2f;
            float offsetY = cellY + (cellSize - letterH) / 2f;

            for (int row = 0; row < 7; row++)
            {
                byte bits = glyph[row];
                for (int col = 0; col < 5; col++)
                {
                    if ((bits & (1 << (4 - col))) == 0) continue;

                    // Font row 0 = top of letter → high Y in texture coords
                    float px = offsetX + col * pixW;
                    float py = offsetY + (6 - row) * pixH;

                    FillRect(pixels, texW, texH,
                        (int)px, (int)py,
                        Mathf.Max(1, (int)(pixW + 0.5f)),
                        Mathf.Max(1, (int)(pixH + 0.5f)),
                        color);
                }
            }
        }

        // ═══════════════════════════════════════
        //  DRAWING PRIMITIVES
        // ═══════════════════════════════════════

        private static void FillRect(Color32[] pixels, int w, int h,
            int x, int y, int rw, int rh, Color32 color)
        {
            int x0 = Mathf.Max(0, x);
            int y0 = Mathf.Max(0, y);
            int x1 = Mathf.Min(w, x + rw);
            int y1 = Mathf.Min(h, y + rh);

            for (int py = y0; py < y1; py++)
                for (int px = x0; px < x1; px++)
                    pixels[py * w + px] = BlendColor32(pixels[py * w + px], color);
        }

        private static void FillRoundedRect(Color32[] pixels, int w, int h,
            float rx, float ry, float rw, float rh, float radius, Color32 color)
        {
            int x0 = Mathf.Max(0, (int)rx);
            int y0 = Mathf.Max(0, (int)ry);
            int x1 = Mathf.Min(w - 1, (int)(rx + rw));
            int y1 = Mathf.Min(h - 1, (int)(ry + rh));

            float left = rx, bottom = ry, right = rx + rw, top = ry + rh;
            float cLeft = left + radius, cRight = right - radius;
            float cBottom = bottom + radius, cTop = top - radius;

            for (int py = y0; py <= y1; py++)
            {
                for (int px = x0; px <= x1; px++)
                {
                    float fx = px + 0.5f, fy = py + 0.5f;
                    float dist;

                    if (fx < cLeft && fy < cBottom)
                    {
                        float dx = fx - cLeft, dy = fy - cBottom;
                        dist = radius - Mathf.Sqrt(dx * dx + dy * dy);
                    }
                    else if (fx > cRight && fy < cBottom)
                    {
                        float dx = fx - cRight, dy = fy - cBottom;
                        dist = radius - Mathf.Sqrt(dx * dx + dy * dy);
                    }
                    else if (fx < cLeft && fy > cTop)
                    {
                        float dx = fx - cLeft, dy = fy - cTop;
                        dist = radius - Mathf.Sqrt(dx * dx + dy * dy);
                    }
                    else if (fx > cRight && fy > cTop)
                    {
                        float dx = fx - cRight, dy = fy - cTop;
                        dist = radius - Mathf.Sqrt(dx * dx + dy * dy);
                    }
                    else
                    {
                        dist = Mathf.Min(
                            Mathf.Min(fx - left, right - fx),
                            Mathf.Min(fy - bottom, top - fy));
                    }

                    float alpha = Mathf.Clamp01(dist / 1.5f + 0.5f);
                    if (alpha > 0f)
                    {
                        Color32 c = color;
                        c.a = (byte)(c.a * alpha);
                        pixels[py * w + px] = BlendColor32(pixels[py * w + px], c);
                    }
                }
            }
        }

        private static Color32 BlendColor32(Color32 bg, Color32 fg)
        {
            if (fg.a == 0) return bg;
            if (fg.a == 255) return fg;

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

        private static void SavePNG(Texture2D tex, string assetPath)
        {
            string fullPath = Path.Combine(
                Application.dataPath.Replace("/Assets", ""), assetPath);
            string dir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            File.WriteAllBytes(fullPath, tex.EncodeToPNG());
            Debug.Log($"[AppIconGenerator] Saved: {assetPath}");
        }

        // ═══════════════════════════════════════
        //  IMPORT & PLAYER SETTINGS
        // ═══════════════════════════════════════

        private static void ConfigureImports()
        {
            string[] iconFiles =
            {
                $"{ICONS_PATH}/app_icon.png",
                $"{ICONS_PATH}/app_icon_1024.png",
                $"{ICONS_PATH}/app_icon_foreground.png",
                $"{ICONS_PATH}/app_icon_background.png",
            };

            foreach (var path in iconFiles)
            {
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                importer.textureType = TextureImporterType.Default;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = 2048;
                importer.isReadable = false;
                importer.SaveAndReimport();
            }
        }

        private static void AssignToPlayerSettings()
        {
            try
            {
                var fgTex = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    $"{ICONS_PATH}/app_icon_foreground.png");
                var bgTex = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    $"{ICONS_PATH}/app_icon_background.png");
                var mainTex = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    $"{ICONS_PATH}/app_icon.png");

                if (mainTex == null)
                {
                    Debug.LogWarning(
                        "[AppIconGenerator] Could not load icon textures for assignment.");
                    return;
                }

#pragma warning disable CS0618 // BuildTargetGroup obsolete in Unity 6
                var target = BuildTargetGroup.Android;
                var kinds = PlayerSettings.GetSupportedIconKindsForPlatform(target);

                foreach (var kind in kinds)
                {
                    var icons = PlayerSettings.GetPlatformIcons(target, kind);
                    foreach (var icon in icons)
                    {
                        if (icon.maxLayerCount > 1)
                        {
                            // Adaptive icon: layer 0 = background, layer 1 = foreground
                            if (bgTex != null) icon.SetTexture(bgTex, 0);
                            if (fgTex != null) icon.SetTexture(fgTex, 1);
                        }
                        else
                        {
                            // Legacy / Round: single combined texture
                            icon.SetTexture(mainTex, 0);
                        }
                    }
                    PlayerSettings.SetPlatformIcons(target, kind, icons);
                }
#pragma warning restore CS0618

                Debug.Log("[AppIconGenerator] Icons assigned to Player Settings (Android)!");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(
                    $"[AppIconGenerator] Auto-assign failed: {e.Message}\n" +
                    "Assign manually: Project Settings > Player > Android > Icon");
            }
        }
    }
}
