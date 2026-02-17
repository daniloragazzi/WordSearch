# Discussion 09 — Termo-Like: Arquitetura Técnica

> **Status:** 🟡 Em discussão
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

### ✅ Confirmadas

| # | Questão | Decisão |
|---|---------|---------|
| 3 | **Package name Android** | `com.ragazzistudios.termo` ✅ |
| 4 | **Nome do app** | **Termo BR** ✅ |

---

### 1. Estrutura de projeto — pasta `Termo/` no mesmo repo?

| Opção | Descrição | Prós | Contras |
|-------|-----------|------|---------|
| **A — Mesmo repo** (recomendado) | Pasta `Termo/` ao lado de `WordSearch/` | Histórico unificado; docs e scripts Python compartilhados; 1 `git push` atualiza tudo; fácil de comparar código entre apps | Repo cresce com o tempo; builds precisam de cuidado para não misturar assets |
| **B — Repo separado** | `github.com/daniloragazzi/Termo` novo | Isolamento total; CI/CD independente por app | Sincronização manual de correções no Core; 2 repositórios para gerenciar; contexto fragmentado |

**Impacto prático agora:** baixo — a diferença real aparece quando houver 3+ apps ou equipe maior. Para 1 desenvolvedor com 2 apps, o mesmo repo é claramente mais simples.

---

### 2. Core compartilhado — copiar agora vs package local imediato?

| Opção | Descrição | Prós | Contras |
|-------|-----------|------|---------|
| **A — Copiar agora** (pragmático) | `Core/` do WordSearch copiado para `Termo/`; cada projeto tem sua cópia | Começa rápido; sem overhead de configuração de package; cada app evolui independente | Correções no Core precisam ser aplicadas em 2 lugares; divergência cresce com o tempo |
| **B — Package local** (estruturado) | `Core/` extraído para pasta `Packages/RagazziCore/` compartilhada entre os 2 projetos Unity usando `"file:../../Packages/RagazziCore"` no manifest | Correção em 1 lugar reflete nos 2 apps; base técnica certa para escalar | Configuração inicial mais trabalhosa; Unity Package local tem limitações de hot-reload; requer disciplina de versionamento |

**Impacto prático:** a diferença real aparece na 2ª ou 3ª correção de bug no Core. Com 2 apps, copiar é aceitável se houver disciplina. Com 3+ apps, o package se paga rápido.

**Meio-termo possível:** copiar agora + marcar como débito técnico explícito para extrair o package quando o 3º app for planejado.

---

### 5. SceneCreator — novo independente vs derivado do Caça-Palavras?

| Opção | Descrição | Prós | Contras |
|-------|-----------|------|---------|
| **A — Novo independente** | `SceneCreator.cs` escrito do zero para o Termo | Código limpo sem herança de lógica do Caça-Palavras; mais fácil de ler e manter isoladamente | Perde helpers já testados (`CreateButton`, `CreateTextElement`, `AddThemeBinding`, etc.) |
| **B — Derivado** (copiar + adaptar) | Copiar `SceneCreator.cs` do WordSearch, remover lógica de grid/palavras, manter helpers de UI | Aproveita ~40% do código já testado (helpers de UI, ThemeBinding, fontes, sprites); começa mais rápido | Arquivo começa maior; risco de arrastar lógica desnecessária se a limpeza for incompleta |

**Helpers reutilizáveis do WordSearch** (valem a pena preservar):
- `CreateButton()`, `CreateTextElement()`, `CreateScreen()`
- `AddThemeBinding()`, `AddCameraThemeBinding()`
- `ApplyFontsToScene()`, `ApplySprite()`
- `Wire()` (reflexão para serializar campos)

**Lógica a descartar:**
- `CreateGridView()`, `CreateLetterCell()`, `CreateWordList()`, `CreateLevelButtonPrefab()`
- Todo o setup de `LevelSelectScreen`, `CategorySelectScreen`, `ChallengeSelectScreen`

---

## Próximos Passos

- [ ] Tomar decisões 1, 2 e 5
- [ ] Criar projeto Unity `Termo/` com estrutura de pastas
- [ ] Copiar/estruturar `Core/` conforme decisão 2
- [ ] Criar ActionPlan detalhado (equivalente ao do App 1)
