# Design Specs — Caça-Palavras MVP

> Guia completo de design visual. Referência para criação de todos os assets.
> Paleta de cores implementada em `GameTheme.cs` (ScriptableObject).

---

## DSN-001 — Paleta de Cores

### Filosofia
Cores **limpas, amigáveis e acessíveis**. Tom predominante azul com acentos quentes (laranja).
Inspiração: apps educacionais/casual — Duolingo, Wordscapes, CodyCross.

### Paleta Principal

| Uso | Nome | Hex | RGB |
|-----|------|-----|-----|
| Primário | `primary` | `#3378F5` | 51, 120, 245 |
| Primário escuro | `primaryDark` | `#2254BA` | 34, 84, 186 |
| Primário claro | `primaryLight` | `#8CBAFF` | 140, 186, 255 |
| Accent | `accent` | `#FF9933` | 255, 153, 51 |
| Sucesso | `success` | `#4DCC66` | 77, 204, 102 |
| Warning | `warning` | `#FFD94D` | 255, 217, 77 |
| Erro | `error` | `#ED4D4D` | 237, 77, 77 |

### Fundos

| Uso | Hex |
|-----|-----|
| Tela principal | `#F5F8FF` |
| Cards/painéis | `#FFFFFF` |
| Overlay popup | `#00000080` |
| Grade de letras | `#EBF0FA` |

### Categorias

| Categoria | Hex | Personalidade |
|-----------|-----|---------------|
| Animais | `#66BF73` | Verde natural |
| Alimentos | `#F28C40` | Laranja apetitoso |
| Esportes | `#4D8CF2` | Azul esportivo |
| Profissões | `#9973D9` | Roxo profissional |
| Natureza | `#40B399` | Verde-água |
| Corpo Humano | `#E6737F` | Rosa-vermelho |
| Países | `#80A6D9` | Azul mapa |
| Cores e Formas | `#D9A659` | Dourado artístico |

---

## DSN-002 — Ícone do App

### Conceito
Grid estilizado 3×3 com letras formando "ABC" ou "CP" (Caça-Palavras).
Fundo gradiente `primary` → `primaryDark`. Letras brancas arredondadas.

### Especificações

| Propriedade | Valor |
|-------------|-------|
| Tamanho base | 512×512 px |
| Formato | PNG 32-bit (transparente) |
| Adaptive icon (Android) | Foreground: letras + grid |
| | Background: gradiente azul |
| Safe zone | 66% central (339×339 px) |
| Border radius | Automático pelo OS |

### Layout do Ícone

```
┌─────────────────┐
│  ┌───┬───┬───┐  │
│  │ C │ A │ Ç │  │  ← Letras brancas, bold
│  ├───┼───┼───┤  │
│  │ A │ ★ │ P │  │  ← ★ = lupa ou destaque
│  ├───┼───┼───┤  │
│  │   │ L │   │  │
│  └───┴───┴───┘  │
│   Fundo #3378F5 │  ← Gradiente azul
└─────────────────┘
```

### Arquivos Necessários

| Arquivo | Tamanho | Uso |
|---------|---------|-----|
| `icon_192.png` | 192×192 | Android mdpi |
| `icon_512.png` | 512×512 | Play Store |
| `icon_foreground.png` | 432×432 | Adaptive icon |
| `icon_background.png` | 432×432 | Adaptive icon background |

> **Local:** `Assets/_Project/Art/Icons/`

---

## DSN-003 — Splash Screen

### Conceito
Logo "Ragazzi Studios" centralizado sobre fundo branco.
Simples, profissional, carrega rápido.

### Especificações

| Propriedade | Valor |
|-------------|-------|
| Resolução | 1080×1920 px (portrait) |
| Fundo | `#FFFFFF` (branco) |
| Logo | Texto "RAGAZZI" + ícone estilizado |
| Fonte logo | Nunito ExtraBold |
| Cor texto | `#3378F5` (primary) |
| Subtexto | "STUDIOS" em `#738094` (textSecondary) |
| Duração | 0.5s (configurável no BootLoader) |

### Layout

```
┌──────────────────┐
│                  │
│                  │
│                  │
│     🎮           │  ← Ícone pequeno ou lupa
│   RAGAZZI        │  ← Nunito ExtraBold, #3378F5
│   STUDIOS        │  ← Nunito Regular, #738094, menor
│                  │
│                  │
│                  │
└──────────────────┘
```

### Arquivos Necessários

| Arquivo | Tamanho | Formato |
|---------|---------|---------|
| `splash_logo.png` | 512×256 | PNG com alpha |
| `splash_background.png` | 1080×1920 | PNG opaco |

> **Local:** `Assets/_Project/Art/Splash/`
> **Unity:** Player Settings → Splash Image → Static Splash Image

---

## DSN-004 — Fonte Tipográfica

### Fonte Selecionada: **Nunito**

| Propriedade | Valor |
|-------------|-------|
| Nome | [Nunito](https://fonts.google.com/specimen/Nunito) |
| Designer | Vernon Adams |
| Licença | SIL Open Font License (✅ uso comercial) |
| Tipo | Sans-serif, arredondada |
| Motivo | Legível em telas pequenas, casual, amigável |

### Pesos Necessários

| Peso | Uso |
|------|-----|
| Regular (400) | Texto corpo, lista de palavras |
| SemiBold (600) | Botões, subtítulos |
| Bold (700) | Letras do grid, números de nível |
| ExtraBold (800) | Títulos, cabeçalhos |

### Setup no Unity (TextMeshPro)

1. Baixar `.ttf` de https://fonts.google.com/specimen/Nunito
2. Importar em `Assets/_Project/Art/Fonts/`
3. Criar SDF Font Assets via **Window → TextMeshPro → Font Asset Creator**:

| Campo | Valor |
|-------|-------|
| Source Font | Nunito-Regular.ttf (e cada peso) |
| Sampling Point Size | 64 |
| Padding | 5 |
| Packing Method | Optimum |
| Atlas Resolution | 512×512 (Regular/SemiBold) |
| | 256×256 (Bold/ExtraBold — menos caracteres) |
| Character Set | Extended ASCII |
| Render Mode | SDFAA |

4. Salvar em `Assets/_Project/Art/Fonts/SDF/`
5. Referenciar nos componentes TMP_Text

### Caracteres Especiais (PT-BR)

Garantir que o atlas inclua:
`À Á Â Ã Ç É Ê Í Ó Ô Õ Ú Ü à á â ã ç é ê í ó ô õ ú ü`

### Tamanhos Recomendados

| Elemento | Tamanho TMP | Peso |
|----------|-------------|------|
| Título tela | 48 | ExtraBold |
| Subtítulo | 32 | Bold |
| Botão | 28 | SemiBold |
| Letra do grid | 24-36 (adaptativo) | Bold |
| Palavra na lista | 22 | Regular |
| Progresso/stats | 18 | Regular |
| Versão/footer | 14 | Regular |

---

## DSN-005 — Sprites UI

### Lista de Sprites Necessários

#### Botões

| Sprite | Tamanho | Tipo | Descrição |
|--------|---------|------|-----------|
| `btn_primary.png` | 256×80 | 9-slice | Botão primário, cantos arredondados 16px |
| `btn_secondary.png` | 256×80 | 9-slice | Botão secundário, borda 2px |
| `btn_circle.png` | 80×80 | Normal | Botão circular (settings, hint, back) |
| `btn_disabled.png` | 256×80 | 9-slice | Botão desabilitado |

#### Painéis

| Sprite | Tamanho | Tipo | Descrição |
|--------|---------|------|-----------|
| `panel_card.png` | 256×256 | 9-slice | Card de categoria, cantos 12px, sombra sutil |
| `panel_popup.png` | 400×300 | 9-slice | Popup (win, settings), cantos 16px |
| `panel_header.png` | 512×80 | 9-slice | Barra de cabeçalho |

#### Grid

| Sprite | Tamanho | Tipo | Descrição |
|--------|---------|------|-----------|
| `cell_normal.png` | 64×64 | Normal | Célula padrão, cantos 8px |
| `cell_selected.png` | 64×64 | Normal | Célula selecionada |
| `cell_found.png` | 64×64 | Normal | Célula de palavra encontrada |
| `cell_hint.png` | 64×64 | Normal | Célula de dica |
| `selection_line.png` | 64×16 | Sliced H | Linha de seleção (stretchable) |

#### Ícones

| Sprite | Tamanho | Tipo | Descrição |
|--------|---------|------|-----------|
| `icon_back.png` | 48×48 | Normal | Seta voltar ← |
| `icon_settings.png` | 48×48 | Normal | Engrenagem ⚙ |
| `icon_hint.png` | 48×48 | Normal | Lâmpada 💡 |
| `icon_pause.png` | 48×48 | Normal | Pause ⏸ |
| `icon_sound_on.png` | 48×48 | Normal | Som ligado 🔊 |
| `icon_sound_off.png` | 48×48 | Normal | Som desligado 🔇 |
| `icon_music_on.png` | 48×48 | Normal | Música ligada 🎵 |
| `icon_music_off.png` | 48×48 | Normal | Música desligada |
| `icon_lock.png` | 48×48 | Normal | Cadeado nível bloqueado 🔒 |
| `icon_check.png` | 48×48 | Normal | Check nível completo ✓ |
| `icon_star.png` | 48×48 | Normal | Estrela (decorativo) ⭐ |

#### Decorativos

| Sprite | Tamanho | Tipo | Descrição |
|--------|---------|------|-----------|
| `bg_gradient.png` | 64×256 | Tiled V | Gradiente vertical sutil para fundo |
| `divider.png` | 256×2 | Sliced H | Linha divisória |
| `progress_bar_bg.png` | 200×16 | 9-slice | Fundo barra de progresso |
| `progress_bar_fill.png` | 200×16 | 9-slice | Preenchimento barra de progresso |

### Configuração 9-Slice (Unity)

Para sprites 9-slice, configurar no Sprite Editor:
- **Border:** 16px em cada lado (para cantos de 16px)
- **Sprite Mode:** Single
- **Mesh Type:** Tight
- **Pixels Per Unit:** 100

### Organização de Pastas

```
Assets/_Project/Art/
├── Fonts/
│   ├── Nunito-Regular.ttf
│   ├── Nunito-SemiBold.ttf
│   ├── Nunito-Bold.ttf
│   ├── Nunito-ExtraBold.ttf
│   └── SDF/
│       ├── Nunito-Regular SDF.asset
│       ├── Nunito-SemiBold SDF.asset
│       ├── Nunito-Bold SDF.asset
│       └── Nunito-ExtraBold SDF.asset
├── Icons/
│   ├── icon_192.png
│   ├── icon_512.png
│   ├── icon_foreground.png
│   └── icon_background.png
├── Splash/
│   ├── splash_logo.png
│   └── splash_background.png
└── UI/
    ├── Buttons/
    │   ├── btn_primary.png
    │   ├── btn_secondary.png
    │   ├── btn_circle.png
    │   └── btn_disabled.png
    ├── Panels/
    │   ├── panel_card.png
    │   ├── panel_popup.png
    │   └── panel_header.png
    ├── Grid/
    │   ├── cell_normal.png
    │   ├── cell_selected.png
    │   ├── cell_found.png
    │   ├── cell_hint.png
    │   └── selection_line.png
    ├── Icons/
    │   ├── icon_back.png
    │   ├── icon_settings.png
    │   ├── icon_hint.png
    │   ├── icon_pause.png
    │   ├── icon_sound_on.png
    │   ├── icon_sound_off.png
    │   ├── icon_music_on.png
    │   ├── icon_music_off.png
    │   ├── icon_lock.png
    │   ├── icon_check.png
    │   └── icon_star.png
    └── Misc/
        ├── bg_gradient.png
        ├── divider.png
        ├── progress_bar_bg.png
        └── progress_bar_fill.png
```

---

## Resumo de Produção

| Prioridade | Item | Qtd | Pode usar placeholder? |
|------------|------|-----|------------------------|
| 🔴 Alta | Células do grid | 4 | ✅ Cor sólida com cantos |
| 🔴 Alta | Botões (9-slice) | 4 | ✅ Retângulo arredondado |
| 🟡 Média | Ícones de ação | 11 | ✅ Usar Unicode/emoji |
| 🟡 Média | Painéis (9-slice) | 3 | ✅ Retângulo branco com sombra |
| 🟢 Baixa | Ícone do app | 4 | ✅ Usar ícone Unity default |
| 🟢 Baixa | Splash screen | 2 | ✅ Texto simples no Boot |
| 🟢 Baixa | Decorativos | 4 | ✅ Cores flat |

> **Para MVP:** Todos os sprites podem ser placeholders gerados proceduralmente.
> A paleta de cores (GameTheme) já provê o visual mínimo necessário.
