# Discussion 09 — Termo-Like: Arquitetura Técnica

> **Status:** ✅ Concluído
> **Data:** 2026-02-17
> **Objetivo:** Definir estrutura do projeto Unity, estratégia de reaproveitamento do Core do Caça-Palavras e decisões técnicas específicas do Termo-Like.

---

## Estratégia de Projeto

Dois caminhos possíveis para o segundo app:

| Opção | Descrição | Prós | Contras |
|-------|-----------|------|---------|
| **A — Repositório separado** | Novo projeto Unity do zero, reaproveitando código via copy/paste ou package | Isolamento total, deploys independentes | Duplicação de código, sincronização manual de correções |
| **B — Mesmo repositório (monorepo)** | Novo projeto Unity dentro do mesmo repo, Core como pacote local compartilhado | Código Core compartilhado nativamente, 1 repositório | Builds separadas exigem cuidado com ProjectSettings por app |
| **C — Mesmo repositório, pasta separada** | Segunda pasta `Termo/` no mesmo repo ao lado de `WordSearch/`, Core copiado/linkado | Organização simples, histórico unificado | Core ainda duplicado até extração em package local |

---

## Decisão de Estrutura de Projeto

> A decidir nesta discussão.

### Recomendação: Opção C (mesmo repo, pasta `Termo/`)

Razões:
- Mantém histórico e contexto juntos enquanto o estúdio é pequeno
- Permite evoluir para package local do Core no futuro sem mudar de estratégia
- Builds Android são independentes por projeto Unity (package names diferentes)
- Alinha com a política de identidade visual unificada (GameTheme compartilhado)

Estrutura proposta do repositório após adição do Termo:

```
WordGames/
├── docs/
│   ├── Brainstorm/
│   └── Organized/
│       ├── WordSearch/     ← docs do app 1
│       └── Termo/          ← docs do app 2 (novo)
├── scripts/
│   ├── data/               ← scripts Python (compartilhados)
│   └── build/
├── WordSearch/             ← projeto Unity app 1 (existente)
└── Termo/                  ← projeto Unity app 2 (novo)
```

---

## Reaproveitamento do Core

### Estratégia: Copiar e desacoplar

O `Core/` do Caça-Palavras será **copiado** para o projeto Termo e evoluirá independentemente por ora. Quando houver 3+ apps, extrai-se como Unity Package local.

| Camada | Ação | Arquivos |
|--------|------|----------|
| `Core/Domain` | Copiar — remover `GridData`, `WordPlacer`, `LevelGenerator` | `GameTheme`, `Words/` (manter base) |
| `Core/Infrastructure` | Copiar integral | `StorageService`, `LocalizationService`, `AdsService`, `AnalyticsService` |
| `Core/Application` | Copiar — adaptar `GameManager`, `GameStateMachine` | `ThemeManager`, `MusicManager` (íntegros) |
| `Game/` | Novo — específico do Termo | Todas as telas, componentes e lógica do jogo |
| `Editor/` | Copiar padrão + novo `SceneCreator` | `ThemePaletteGenerator`, `BuildScript`, novo `SceneCreator` |

---

## Novas Camadas de Domínio (Termo)

### Core/Domain — novos modelos

```
Core/Domain/
├── Termo/
│   ├── TermoWord.cs          — palavra-alvo + palavras válidas
│   ├── TermoGuess.cs         — uma tentativa (5 letras + estados)
│   ├── LetterState.cs        — enum: Correct / Present / Absent / Unused
│   ├── TermoBoard.cs         — estado de 1 grade (N tentativas × 5 letras)
│   ├── TermoGame.cs          — estado completo da partida (1/2/4 boards)
│   └── TermoValidator.cs     — valida tentativa contra a palavra-alvo
```

### Core/Application — novos managers

```
Core/Application/
├── TermoGameManager.cs       — orquestra partida, modo, histórico
├── WordBankService.cs        — carrega words_5.json e valid_5.json
└── StatsManager.cs           — lê/grava estatísticas no StorageService
```

---

## Estados do Jogo (State Machine)

```
Boot → MainMenu → ModeSelect → Playing → Win
                                       → Lose
                     ↑___________________|
                          (jogar novamente → ModeSelect ou Playing direto)
```

| Estado | Tela ativa |
|--------|------------|
| `Boot` | Tela de loading |
| `MainMenu` | Menu principal |
| `ModeSelect` | Seleção de modo (1/2/4 palavras) |
| `Playing` | Gameplay |
| `Win` | WinPopup |
| `Lose` | LosePopup |

---

## Componentes de UI (Game/)

```
Game/UI/
├── Screens/
│   ├── MainMenuScreen.cs
│   ├── ModeSelectScreen.cs
│   └── GameplayScreen.cs
├── Components/
│   ├── TermoBoard.cs          — 1 grade (N linhas × 5 células)
│   ├── TermoCell.cs           — 1 célula com letra + estado + animação flip
│   ├── TermoKeyboard.cs       — teclado QWERTY + estado por tecla
│   └── TermoKey.cs            — 1 tecla do teclado
├── Popups/
│   ├── WinPopup.cs
│   ├── LosePopup.cs
│   ├── StatsPopup.cs
│   └── SettingsPopup.cs       — reaproveitado do Caça-Palavras
└── GameplayController.cs      — coordena boards + teclado + estado
```

---

## Animação de Flip

Cada `TermoCell` executa uma rotação em Y ao revelar o feedback:

1. Rotação 0° → 90° (frente some) — duração: 150ms
2. Trocar cor de fundo para o estado final
3. Rotação 90° → 0° (verso aparece) — duração: 150ms
4. Delay entre células: 100ms (da esquerda para direita)

Total por linha: ~950ms (5 células × (300ms flip + 100ms delay) - último delay)

---

## Banco de Palavras — Pipeline Python

Novo script `scripts/data/build_termo.py`:
- Filtra palavras de 5 letras do banco existente (Caça-Palavras)
- Adiciona lista curada de palavras comuns de 5 letras
- Normaliza (sem acento, uppercase)
- Gera `words_5.json` (alvos) e `valid_5.json` (dicionário completo)
- Valida: sem duplicatas, comprimento exato 5, charset `[A-Z]`

---

## GameTheme — Novos Tokens

Adicionar ao `GameTheme.cs` (compartilhado entre apps):

```csharp
[Header("Termo — Feedback")]
public Color letterCorrect  = new Color(0.38f, 0.65f, 0.38f);  // verde
public Color letterPresent  = new Color(0.79f, 0.64f, 0.18f);  // amarelo
public Color letterAbsent   = new Color(0.31f, 0.33f, 0.38f);  // cinza escuro
public Color letterUnused   = new Color(0.82f, 0.84f, 0.87f);  // cinza claro
public Color letterOnColor  = new Color(1f, 1f, 1f);           // texto sobre célula colorida
```

---

## Perguntas para Decidir

## Decisões

> ✅ **Decidido em 2026-02-17**

| # | Questão | Decisão | Justificativa |
|---|---------|---------|---------------|
| 1 | **Estrutura de projeto** | Repositório separado (`github.com/daniloragazzi/TermoBR`) | Isolamento total entre apps; deploys e históricos independentes; facilita eventual transferência ou co-autoria |
| 2 | **Core compartilhado** | Package local desde o início (`RagazziCore`) | Investimento único de setup; correções refletem em todos os apps; base certa para escalar o estúdio |
| 3 | **Package name Android** | `com.ragazzistudios.termo` ✅ | — |
| 4 | **Nome do app** | **Termo BR** ✅ | — |
| 5 | **SceneCreator** | Novo do zero | Código limpo sem herança; Termo tem layout radicalmente diferente do Caça-Palavras (teclado + grades vs grid de letras) |

---

### Impacto das decisões na estrutura de repositórios

```
github.com/daniloragazzi/
├── WordSearch/          ← App 1 (existente)
│   └── WordSearch/      ← projeto Unity
└── TermoBR/             ← App 2 (novo — repo separado)
    ├── Termo/           ← projeto Unity
    ├── Packages/
    │   └── RagazziCore/ ← package local (Core extraído do WordSearch)
    ├── scripts/
    └── docs/
```

### Estrutura do package `RagazziCore`

```
Packages/RagazziCore/
├── package.json                          ← "name": "com.ragazzistudios.core"
├── Runtime/
│   ├── Infrastructure/                   ← StorageService, LocalizationService, AdsService, AnalyticsService
│   ├── Application/                      ← ThemeManager, MusicManager, GameStateMachine, ServiceLocator
│   └── Domain/                           ← GameTheme (com novos tokens letterCorrect/Present/Absent)
├── Editor/
│   └── ThemePaletteGenerator.cs          ← portado do WordSearch
└── README.md
```

Referência no `Termo/Packages/manifest.json`:
```json
"com.ragazzistudios.core": "file:../../Packages/RagazziCore"
```

### Tarefas de setup decorrentes

1. Criar repo `TermoBR` no GitHub
2. Extrair `Core/` do WordSearch para `Packages/RagazziCore/` com `package.json`
3. Criar projeto Unity `Termo/` com estrutura de pastas
4. Referenciar package no manifest do Termo
5. *(Futuro)* Backport do package para o WordSearch também — unificando o Core nos 2 apps

---

## Próximos Passos

- [x] Tomar decisões 1, 2, 3, 4 e 5
- [ ] Criar repo `TermoBR` no GitHub
- [ ] Criar package `RagazziCore` a partir do Core do WordSearch
- [ ] Criar projeto Unity `Termo/` com estrutura de pastas
- [ ] Criar ActionPlan detalhado (equivalente ao do App 1)
