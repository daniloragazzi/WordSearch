# 03 — Arquitetura

> **Código:** DEF-003 / DOC-003 (inclui DEF-004)
> **Status:** ✅ Concluído
> **Data:** 2026-02-15
> **Origem:** [Discussion_03](../Brainstorm/Discussion_03.md)

---

## Visão Geral

Clean Architecture adaptada para Unity, com separação clara entre **engine reutilizável** (`Core/`) e **jogo específico** (`Game/`).

---

## Camadas

```
┌─────────────────────────────────────────────┐
│                    UI Layer                  │  ← Telas, HUD, animações
├─────────────────────────────────────────────┤
│              Application Layer              │  ← Game flow, estados, managers
├─────────────────────────────────────────────┤
│               Domain Layer                  │  ← Regras do jogo, lógica pura
├─────────────────────────────────────────────┤
│            Infrastructure Layer             │  ← Ads, Storage, Analytics, i18n
├─────────────────────────────────────────────┤
│               Data Layer                    │  ← JSON, ScriptableObjects
└─────────────────────────────────────────────┘
```

| Camada | Responsabilidade | Depende de | Reutilizável? |
|--------|-----------------|------------|---------------|
| **UI** | Apresentação, input do jogador | Application | Parcial |
| **Application** | Orquestração, game flow, estados | Domain, Infra | Sim |
| **Domain** | Lógica pura do jogo (grid, palavras) | Nada | 100% |
| **Infrastructure** | Serviços externos (ads, storage) | Nada | 100% |
| **Data** | Dados do jogo (palavras, categorias) | Nada | Estrutura sim |

---

## Design Patterns

| Pattern | Onde | Por quê |
|---------|------|---------|
| Singleton | GameManager, AudioManager, AdManager | Managers globais |
| Observer/Events | Comunicação entre camadas | Desacoplamento |
| State Machine | Game flow | Controle de estados |
| Factory | Geração de grids, níveis | Criação seed-based |
| Service Locator | Infrastructure services | Desacoplamento |
| ScriptableObjects | Configs, temas, settings | Data-driven |

---

## Interfaces de Infraestrutura

```
IAdsService          → AdMobService
IStorageService      → PlayerPrefsStorage
IAnalyticsService    → UnityAnalyticsService
ILocalizationService → JsonLocalizationService
```

---

## Comunicação entre Camadas

**C# Events** (não UnityEvents)

| Evento | Quem dispara | Quem escuta |
|--------|-------------|-------------|
| `OnWordFound` | Domain/WordFinder | UI/GridView, UI/WordListView |
| `OnLevelComplete` | Application/LevelManager | UI/WinPopup, Infra/Analytics |
| `OnHintUsed` | Application/GameManager | Infra/AdsService, UI/GridView |
| `OnCategorySelected` | UI/CategoryScreen | Application/LevelManager |

---

## Game Flow (State Machine)

```
[Boot] → [MainMenu] → [CategorySelect] → [LevelSelect] → [Playing] → [Win] → [LevelSelect]
```

---

## Cenas

| Cena | Propósito |
|------|-----------|
| Boot.unity | Inicialização de serviços, carga de dados |
| MainMenu.unity | Menu principal |
| Game.unity | Gameplay |

---

## Estrutura de Pastas

```
Assets/
├── _Project/
│   ├── Core/                          ← ENGINE REUTILIZÁVEL
│   │   ├── Domain/
│   │   │   ├── Grid/
│   │   │   │   ├── GridGenerator.cs
│   │   │   │   ├── GridData.cs
│   │   │   │   └── GridValidator.cs
│   │   │   ├── Words/
│   │   │   │   ├── WordPlacer.cs
│   │   │   │   ├── WordFinder.cs
│   │   │   │   └── WordDatabase.cs
│   │   │   └── Level/
│   │   │       ├── LevelGenerator.cs
│   │   │       └── LevelData.cs
│   │   ├── Application/
│   │   │   ├── GameManager.cs
│   │   │   ├── LevelManager.cs
│   │   │   └── GameState.cs
│   │   └── Infrastructure/
│   │       ├── Ads/
│   │       │   ├── IAdsService.cs
│   │       │   └── AdMobService.cs
│   │       ├── Storage/
│   │       │   ├── IStorageService.cs
│   │       │   └── PlayerPrefsStorage.cs
│   │       ├── Analytics/
│   │       │   ├── IAnalyticsService.cs
│   │       │   └── UnityAnalyticsService.cs
│   │       └── Localization/
│   │           ├── ILocalizationService.cs
│   │           └── JsonLocalizationService.cs
│   │
│   ├── Game/                          ← ESPECÍFICO DESTE JOGO
│   │   ├── UI/
│   │   │   ├── Screens/
│   │   │   │   ├── SplashScreen.cs
│   │   │   │   ├── MainMenuScreen.cs
│   │   │   │   ├── CategorySelectScreen.cs
│   │   │   │   ├── LevelSelectScreen.cs
│   │   │   │   └── GameScreen.cs
│   │   │   ├── Components/
│   │   │   │   ├── GridView.cs
│   │   │   │   ├── WordListView.cs
│   │   │   │   ├── LetterCell.cs
│   │   │   │   └── SelectionLine.cs
│   │   │   └── Popups/
│   │   │       ├── WinPopup.cs
│   │   │       ├── PausePopup.cs
│   │   │       └── SettingsPopup.cs
│   │   ├── Config/
│   │   │   ├── GameConfig.asset
│   │   │   └── ThemeConfig.asset
│   │   └── GameInstaller.cs
│   │
│   ├── Resources/Data/
│   │   ├── categories.json
│   │   └── words/
│   │       ├── animais.json
│   │       └── ...
│   │
│   ├── Scenes/
│   │   ├── Boot.unity
│   │   ├── MainMenu.unity
│   │   └── Game.unity
│   │
│   ├── Art/ (Sprites/, Fonts/, UI/)
│   └── Audio/ (SFX/, Music/)
│
├── Plugins/ (AdMob, etc.)
└── StreamingAssets/
```

### Regra de reutilização

| Pasta | Ao criar novo jogo... |
|-------|-----------------------|
| `Core/` | **Copia intacto** |
| `Game/` | **Cria novo** |
| `Resources/Data/` | **Troca dados** |
| `Scenes/` | **Adapta** |
| `Art/`, `Audio/` | **Troca** |
| `Plugins/` | **Mantém** |

---

## Naming Conventions

| Tipo | Convenção | Exemplo |
|------|-----------|---------|
| Classes | PascalCase | `GridGenerator` |
| Interfaces | I + PascalCase | `IAdsService` |
| Métodos | PascalCase | `GenerateGrid()` |
| Variáveis privadas | _camelCase | `_gridSize` |
| Variáveis públicas | camelCase | `gridSize` |
| Constantes | UPPER_SNAKE | `MAX_GRID_SIZE` |
| Eventos | On + PascalCase | `OnWordFound` |
| Enums | PascalCase | `Direction.Horizontal` |
| Pastas | PascalCase | `Domain/` |
| Arquivos JSON | snake_case | `corpo_humano.json` |
| Cenas | PascalCase | `MainMenu.unity` |

---

## Modelo de Portfólio

> **1 jogo = 1 app independente na Play Store**

- Cada jogo tem seu próprio package name (`com.ragazzistudios.*`)
- Deploy, instalação e página na Play Store independentes
- `Core/` é compartilhado (copiado ou via Git submodule)
- `Game/`, `Resources/`, `Art/`, `Audio/` são específicos de cada jogo
- Cross-promotion entre apps (tráfego gratuito)

```
Play Store
  ├── Caça-Palavras        → com.ragazzistudios.wordsearch
  ├── Termo (futuro)       → com.ragazzistudios.termo
  ├── Quiz Bíblico (futuro) → com.ragazzistudios.quizbible
  └── ...
```

---

## Próximos Passos

→ **DEF-005** — Definir pipeline técnico
