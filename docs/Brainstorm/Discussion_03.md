# Discussion 03 — Engine Base / Arquitetura

> **Código:** DEF-003
> **Status:** 🟡 Em discussão
> **Data:** 2026-02-15
> **Objetivo:** Definir a arquitetura de código, patterns e estrutura de pastas do projeto Unity, pensando em reutilização para futuros jogos.

---

## Princípio Guia

> A arquitetura deve separar claramente o **core reutilizável** (engine) do **conteúdo específico** (jogo).
> Trocar o "jogo" deve ser como trocar a roupa, não o esqueleto.

---

## 1. Arquitetura de Camadas

### Proposta: Clean Architecture adaptada para Unity

```
┌─────────────────────────────────────────────┐
│                    UI Layer                  │  ← Telas, HUD, animações
│              (MonoBehaviours)                │
├─────────────────────────────────────────────┤
│              Application Layer              │  ← Game flow, estados, managers
│            (GameManager, etc.)              │
├─────────────────────────────────────────────┤
│               Domain Layer                  │  ← Regras do jogo, lógica pura
│         (Grid, WordPlacer, Solver)          │
├─────────────────────────────────────────────┤
│            Infrastructure Layer             │  ← Ads, Storage, Analytics, i18n
│         (AdMob, PlayerPrefs, etc.)          │
├─────────────────────────────────────────────┤
│               Data Layer                    │  ← JSON, ScriptableObjects
│         (WordDatabase, Categories)          │
└─────────────────────────────────────────────┘
```

| Camada | Responsabilidade | Depende de | Reutilizável? |
|--------|-----------------|------------|---------------|
| **UI** | Apresentação, input do jogador | Application | Parcial (temas mudam) |
| **Application** | Orquestração, game flow, estados | Domain, Infra | Sim |
| **Domain** | Lógica pura do jogo (grid, palavras) | Nada | 100% |
| **Infrastructure** | Serviços externos (ads, storage) | Nada | 100% |
| **Data** | Dados do jogo (palavras, categorias) | Nada | Estrutura sim, conteúdo não |

### Por que esta abordagem?
- **Domain** sem dependência do Unity = testável, portável, reutilizável
- **Infrastructure** isolada = trocar AdMob por outro SDK sem tocar no jogo
- **UI** separada = trocar visual sem tocar na lógica
- Alinha com o PRD (camadas Domain, Application, Infrastructure, UI)

---

## 2. Design Patterns

### Proposta

| Pattern | Onde | Por quê |
|---------|------|---------|
| **Singleton** | GameManager, AudioManager, AdManager | Managers globais, acesso fácil |
| **Observer/Events** | Comunicação entre camadas | Desacoplamento, UI reage a eventos |
| **State Machine** | Game flow (menu → playing → paused → win) | Controle claro de estados |
| **Factory** | Geração de grids, níveis | Criação padronizada, seed-based |
| **Service Locator** | Acesso a Infrastructure services | Desacopla quem usa de quem implementa |
| **ScriptableObjects** | Configurações, temas, settings | Data-driven, editável no Unity Inspector |

### O que NÃO usar no MVP
- **ECS** (Entity Component System) — overkill para 2D casual
- **Dependency Injection framework** (Zenject) — overhead desnecessário
- **Addressables** — complexidade de asset management prematura

---

## 3. Estrutura de Pastas Unity

### Proposta

```
Assets/
├── _Project/                          ← Tudo do projeto (separado dos plugins)
│   ├── Core/                          ← ENGINE REUTILIZÁVEL
│   │   ├── Domain/
│   │   │   ├── Grid/
│   │   │   │   ├── GridGenerator.cs        ← Gera grid com palavras
│   │   │   │   ├── GridData.cs             ← Modelo do grid
│   │   │   │   └── GridValidator.cs        ← Valida posicionamento
│   │   │   ├── Words/
│   │   │   │   ├── WordPlacer.cs           ← Posiciona palavra no grid
│   │   │   │   ├── WordFinder.cs           ← Verifica se seleção é válida
│   │   │   │   └── WordDatabase.cs         ← Acesso ao banco de palavras
│   │   │   └── Level/
│   │   │       ├── LevelGenerator.cs       ← Gera nível com seed
│   │   │       └── LevelData.cs            ← Modelo do nível
│   │   │
│   │   ├── Application/
│   │   │   ├── GameManager.cs              ← Orquestrador principal
│   │   │   ├── LevelManager.cs             ← Controle de progressão
│   │   │   ├── GameState.cs                ← State machine do jogo
│   │   │   └── ScoreManager.cs             ← (futuro) pontuação
│   │   │
│   │   └── Infrastructure/
│   │       ├── Ads/
│   │       │   ├── IAdsService.cs           ← Interface
│   │       │   └── AdMobService.cs          ← Implementação
│   │       ├── Storage/
│   │       │   ├── IStorageService.cs       ← Interface
│   │       │   └── PlayerPrefsStorage.cs    ← Implementação
│   │       ├── Analytics/
│   │       │   ├── IAnalyticsService.cs     ← Interface
│   │       │   └── UnityAnalyticsService.cs ← Implementação
│   │       └── Localization/
│   │           ├── ILocalizationService.cs  ← Interface
│   │           └── JsonLocalizationService.cs ← Implementação
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
│   │   │   │   ├── GridView.cs              ← Renderiza o grid
│   │   │   │   ├── WordListView.cs          ← Lista de palavras
│   │   │   │   ├── LetterCell.cs            ← Célula individual
│   │   │   │   └── SelectionLine.cs         ← Linha de seleção
│   │   │   └── Popups/
│   │   │       ├── WinPopup.cs
│   │   │       ├── PausePopup.cs
│   │   │       └── SettingsPopup.cs
│   │   │
│   │   ├── Config/
│   │   │   ├── GameConfig.asset             ← ScriptableObject com settings
│   │   │   └── ThemeConfig.asset            ← Cores, fontes, visual
│   │   │
│   │   └── GameInstaller.cs                 ← Bootstrap / setup inicial
│   │
│   ├── Resources/
│   │   └── Data/
│   │       ├── categories.json
│   │       └── words/
│   │           ├── animais.json
│   │           ├── alimentos.json
│   │           └── ...
│   │
│   ├── Scenes/
│   │   ├── Boot.unity                       ← Cena de inicialização
│   │   ├── MainMenu.unity                   ← Menu principal
│   │   └── Game.unity                       ← Cena do jogo
│   │
│   ├── Art/
│   │   ├── Sprites/
│   │   ├── Fonts/
│   │   └── UI/
│   │
│   └── Audio/
│       ├── SFX/
│       └── Music/
│
├── Plugins/                           ← SDKs de terceiros
│   ├── AdMob/
│   └── ...
│
└── StreamingAssets/                    ← (se necessário para dados grandes)
```

### Lógica da estrutura

| Pasta | Propósito | Ao criar novo jogo... |
|-------|-----------|----------------------|
| `Core/` | Engine reutilizável | **Copia intacto** |
| `Game/` | Específico do jogo | **Cria novo** |
| `Resources/Data/` | Conteúdo | **Troca dados** |
| `Scenes/` | Cenas Unity | **Adapta** |
| `Art/`, `Audio/` | Assets visuais/sonoros | **Troca** |
| `Plugins/` | SDKs | **Mantém** |

---

## 4. Interfaces e Desacoplamento

### Proposta: Interfaces para todos os serviços de infraestrutura

```
IAdsService          → AdMobService (hoje) → UnityAdsService (futuro)
IStorageService      → PlayerPrefsStorage (hoje) → CloudStorage (futuro)
IAnalyticsService    → UnityAnalyticsService (hoje) → FirebaseAnalytics (futuro)
ILocalizationService → JsonLocalizationService (hoje)
```

### Por que interfaces?
- Trocar implementação sem alterar quem consome
- Facilita testes (mock)
- Permite migrar de SDK sem refactoring do jogo

---

## 5. Game Flow

### Proposta: State Machine simples

```
[Boot] → [MainMenu] → [CategorySelect] → [LevelSelect] → [Playing] → [Win] → [LevelSelect]
                ↑                                              │
                └──────────────────────────────────────────────┘
                                  (voltar ao menu)
```

| Estado | Responsabilidade |
|--------|-----------------|
| Boot | Inicializar serviços, carregar dados |
| MainMenu | Aguardar input do jogador |
| CategorySelect | Mostrar categorias, progresso |
| LevelSelect | Mostrar níveis da categoria |
| Playing | Gameplay ativo (interação com grid) |
| Win | Mostrar popup, ad interstitial (se aplicável), próximo nível |

---

## 6. Comunicação entre Camadas

### Proposta: Event-driven com C# Events

```
Domain (gera evento)  →  Application (processa)  →  UI (reage)

Exemplo:
  WordFound (event)    →  GameManager.OnWordFound()  →  GridView.HighlightWord()
                                                      →  WordListView.MarkFound()
```

| Evento | Quem dispara | Quem escuta |
|--------|-------------|-------------|
| `OnWordFound` | Domain/WordFinder | UI/GridView, UI/WordListView |
| `OnLevelComplete` | Application/LevelManager | UI/WinPopup, Infra/Analytics |
| `OnHintUsed` | Application/GameManager | Infra/AdsService, UI/GridView |
| `OnCategorySelected` | UI/CategoryScreen | Application/LevelManager |

### Por que C# Events e não UnityEvents?
- Mais performático
- Tipagem forte
- Sem dependência do Inspector
- Padrão C# — funciona fora do Unity (Domain layer)

---

## 7. Naming Conventions

### Proposta

| Tipo | Convenção | Exemplo |
|------|-----------|---------|
| Classes | PascalCase | `GridGenerator`, `WordPlacer` |
| Interfaces | I + PascalCase | `IAdsService`, `IStorageService` |
| Métodos | PascalCase | `GenerateGrid()`, `PlaceWord()` |
| Variáveis privadas | _camelCase | `_gridSize`, `_wordList` |
| Variáveis públicas | camelCase | `gridSize` (em MonoBehaviours) |
| Constantes | UPPER_SNAKE | `MAX_GRID_SIZE`, `MIN_WORD_LENGTH` |
| Eventos | On + PascalCase | `OnWordFound`, `OnLevelComplete` |
| Enums | PascalCase | `Direction.Horizontal` |
| Pastas | PascalCase | `Core/`, `Domain/`, `Infrastructure/` |
| Arquivos JSON | snake_case | `corpo_humano.json` |
| Cenas | PascalCase | `MainMenu.unity` |

---

## Resumo da Proposta

| Aspecto | Decisão proposta |
|---------|-----------------|
| Arquitetura | Clean Architecture (5 camadas) |
| Core vs Game | Separação clara em pastas |
| Patterns | Singleton, Observer, State Machine, Factory, Service Locator |
| Infra | Interfaces para todos os serviços |
| Comunicação | C# Events |
| Game Flow | State Machine simples |
| Naming | C# conventions + Unity conventions |
| Domain | Lógica pura, sem dependência Unity |

---

## Perguntas para Decisão

1. Concorda com a separação `Core/` (engine) vs `Game/` (específico)?
2. Arquitetura em 5 camadas está ok ou prefere simplificar?
3. C# Events para comunicação — ok?
4. Naming conventions propostas estão boas?
5. 3 cenas (Boot, MainMenu, Game) ou prefere menos?
6. Algo que gostaria de mudar ou adicionar?

---

## Decisão

> ✅ **Decidido em 2026-02-15**

**Todas as propostas aprovadas sem alterações:**

| Aspecto | Decisão |
|---------|----------|
| Arquitetura | Clean Architecture (5 camadas) |
| Separação | `Core/` (engine) vs `Game/` (específico) |
| Patterns | Singleton, Observer, State Machine, Factory, Service Locator |
| Infra | Interfaces para todos os serviços |
| Comunicação | C# Events |
| Game Flow | State Machine simples |
| Naming | C# + Unity conventions (conforme tabela) |
| Domain | Lógica pura, sem dependência Unity |
| Cenas | 3 cenas (Boot, MainMenu, Game) |
| Pastas | Estrutura completa definida |

---

## Próximos Passos

- [x] Criar documento organizado: `Organized/03_Architecture.md` (DOC-003)
- [x] Atualizar Execution_Tracker
- [ ] Avançar para próximas definições pendentes
