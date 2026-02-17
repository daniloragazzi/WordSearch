using UnityEngine;

namespace RagazziStudios.Game.Config
{
    /// <summary>
    /// ScriptableObject com a paleta de cores e configuração visual do tema.
    /// Centraliza todas as cores usadas na UI para fácil personalização.
    /// Criar via: Assets → Create → Ragazzi → Game Theme
    /// </summary>
    [CreateAssetMenu(fileName = "GameTheme", menuName = "Ragazzi/Game Theme")]
    public class GameTheme : ScriptableObject
    {
        // ═══════════════════════════════════════════════════
        //  IDENTIFICAÇÃO DO TEMA
        // ═══════════════════════════════════════════════════

        [Header("Identificação")]
        [Tooltip("Marcar como true para o tema escuro (usado pelo ThemeManager)")]
        public bool isDark = false;

        // ═══════════════════════════════════════════════════
        //  PALETA PRIMÁRIA
        // ═══════════════════════════════════════════════════

        [Header("Paleta Primária")]
        [Tooltip("Cor principal da marca — azul vibrante")]
        public Color primary = new Color(0.20f, 0.47f, 0.96f);       // #3378F5

        [Tooltip("Variante escura do primário")]
        public Color primaryDark = new Color(0.13f, 0.33f, 0.73f);   // #2254BA

        [Tooltip("Variante clara do primário")]
        public Color primaryLight = new Color(0.55f, 0.73f, 1.00f);  // #8CBAFF

        // ═══════════════════════════════════════════════════
        //  PALETA SECUNDÁRIA
        // ═══════════════════════════════════════════════════

        [Header("Paleta Secundária")]
        [Tooltip("Cor de destaque — laranja quente")]
        public Color accent = new Color(1.00f, 0.60f, 0.20f);        // #FF9933

        [Tooltip("Variante escura do accent")]
        public Color accentDark = new Color(0.85f, 0.45f, 0.10f);    // #D9731A

        [Tooltip("Verde de sucesso/encontrado")]
        public Color success = new Color(0.30f, 0.80f, 0.40f);       // #4DCC66

        [Tooltip("Amarelo de dica/warning")]
        public Color warning = new Color(1.00f, 0.85f, 0.30f);       // #FFD94D

        [Tooltip("Vermelho de erro")]
        public Color error = new Color(0.93f, 0.30f, 0.30f);         // #ED4D4D

        // ═══════════════════════════════════════════════════
        //  FUNDOS
        // ═══════════════════════════════════════════════════

        [Header("Fundos")]
        [Tooltip("Fundo principal da tela")]
        public Color background = new Color(0.96f, 0.97f, 1.00f);    // #F5F8FF

        [Tooltip("Fundo de painéis/cards")]
        public Color surface = new Color(1.00f, 1.00f, 1.00f);       // #FFFFFF

        [Tooltip("Fundo de popup (overlay semi-transparente)")]
        public Color overlay = new Color(0.00f, 0.00f, 0.00f, 0.50f);// #00000080

        [Tooltip("Fundo da grade de letras")]
        public Color gridBackground = new Color(0.92f, 0.94f, 0.98f);// #EBF0FA

        // ═══════════════════════════════════════════════════
        //  TEXTOS
        // ═══════════════════════════════════════════════════

        [Header("Textos")]
        [Tooltip("Texto principal (escuro)")]
        public Color textPrimary = new Color(0.12f, 0.14f, 0.20f);   // #1F2433

        [Tooltip("Texto secundário (cinza)")]
        public Color textSecondary = new Color(0.45f, 0.50f, 0.58f); // #738094

        [Tooltip("Texto sobre fundo colorido (branco)")]
        public Color textOnColor = new Color(1.00f, 1.00f, 1.00f);   // #FFFFFF

        [Tooltip("Texto desabilitado")]
        public Color textDisabled = new Color(0.70f, 0.73f, 0.78f);  // #B3BAC7

        // ═══════════════════════════════════════════════════
        //  GRID / GAMEPLAY
        // ═══════════════════════════════════════════════════

        [Header("Grid — Células")]
        [Tooltip("Célula normal")]
        public Color cellNormal = new Color(0.95f, 0.96f, 0.98f);    // #F2F5FA

        [Tooltip("Célula selecionada (arraste)")]
        public Color cellSelected = new Color(0.40f, 0.65f, 1.00f);  // #66A6FF

        [Tooltip("Célula de palavra encontrada")]
        public Color cellFound = new Color(0.30f, 0.80f, 0.40f);     // #4DCC66

        [Tooltip("Célula de dica")]
        public Color cellHint = new Color(1.00f, 0.85f, 0.30f);      // #FFD94D

        [Tooltip("Letra na célula (estado normal)")]
        public Color cellLetterNormal = new Color(0.15f, 0.17f, 0.22f);// #262C38

        [Tooltip("Letra na célula (estado ativo — selecionado/encontrado)")]
        public Color cellLetterActive = new Color(1.00f, 1.00f, 1.00f);// #FFFFFF

        // ═══════════════════════════════════════════════════
        //  LISTA DE PALAVRAS
        // ═══════════════════════════════════════════════════

        [Header("Lista de Palavras")]
        [Tooltip("Palavra não encontrada")]
        public Color wordNormal = new Color(0.20f, 0.22f, 0.28f);    // #333847

        [Tooltip("Palavra encontrada")]
        public Color wordFound = new Color(0.30f, 0.80f, 0.40f);     // #4DCC66

        // ═══════════════════════════════════════════════════
        //  BOTÕES
        // ═══════════════════════════════════════════════════

        [Header("Botões")]
        [Tooltip("Fundo botão primário")]
        public Color buttonPrimary = new Color(0.20f, 0.47f, 0.96f); // #3378F5

        [Tooltip("Fundo botão primário pressionado")]
        public Color buttonPrimaryPressed = new Color(0.13f, 0.33f, 0.73f);

        [Tooltip("Fundo botão secundário")]
        public Color buttonSecondary = new Color(0.90f, 0.92f, 0.96f);// #E6EBF5

        [Tooltip("Fundo botão desabilitado")]
        public Color buttonDisabled = new Color(0.85f, 0.87f, 0.90f);// #D9DEE6

        // ═══════════════════════════════════════════════════
        //  CATEGORIAS (cores individuais)
        // ═══════════════════════════════════════════════════

        [Header("Categorias — Cores dos Cards")]
        public Color categoryAnimais = new Color(0.40f, 0.75f, 0.45f);   // Verde natural
        public Color categoryAlimentos = new Color(0.95f, 0.55f, 0.25f); // Laranja apetitoso
        public Color categoryEsportes = new Color(0.30f, 0.55f, 0.95f);  // Azul esportivo
        public Color categoryProfissoes = new Color(0.60f, 0.45f, 0.85f);// Roxo profissional
        public Color categoryNatureza = new Color(0.25f, 0.70f, 0.60f);  // Verde-água
        public Color categoryCorpoHumano = new Color(0.90f, 0.45f, 0.50f);// Rosa-vermelho
        public Color categoryPaises = new Color(0.50f, 0.65f, 0.85f);    // Azul mapa
        public Color categoryCoresFormas = new Color(0.85f, 0.65f, 0.35f);// Dourado artístico

        // ═══════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Retorna a cor temática de uma categoria pelo ID.
        /// </summary>
        public Color GetCategoryColor(string categoryId)
        {
            switch (categoryId)
            {
                case "animais": return categoryAnimais;
                case "alimentos": return categoryAlimentos;
                case "esportes": return categoryEsportes;
                case "profissoes": return categoryProfissoes;
                case "natureza": return categoryNatureza;
                case "corpo_humano": return categoryCorpoHumano;
                case "paises": return categoryPaises;
                case "cores_formas": return categoryCoresFormas;
                default: return primary;
            }
        }

        /// <summary>
        /// Retorna a cor de texto adequada para um fundo colorido
        /// (branco para fundos escuros, escuro para fundos claros).
        /// </summary>
        public Color GetContrastText(Color backgroundColor)
        {
            float luminance = 0.299f * backgroundColor.r +
                              0.587f * backgroundColor.g +
                              0.114f * backgroundColor.b;
            return luminance > 0.5f ? textPrimary : textOnColor;
        }
    }
}
