# Action Plan — WordGames Studio

> Plano de ações detalhado para execução do projeto.
> Atualizado a cada mudança de status.

---

## Legenda

| Emoji | Status |
|-------|--------|
| ⬜ | Não iniciado |
| 🔵 | Em andamento |
| ✅ | Concluído |
| 🔴 | Bloqueado |
| ⏸️ | Pausado |

---

## Convenção de Rastreabilidade

- Todas as ações seguem código `XXX-000` (prefixo de 3 letras + 3 dígitos).
- `ActionPlan` concentra o plano de ações (escopo e dependências).
- `Execution_Tracker` concentra o acompanhamento de execução (status, datas e histórico).
- Referências cruzadas devem sempre apontar para:
  - documento de origem em `docs/Brainstorm/` quando existir,
  - ação correspondente no próprio `ActionPlan`.

---

## Fase 2 — Desenvolvimento do MVP (Caça-Palavras)

### 2.1 — Setup e Configuração

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| CFG-001 | Criar projeto Unity e estrutura de pastas | ✅ | — | Unity 6.3 LTS, template 2D, pastas conforme 03_Architecture |
| CFG-002 | Configurar Git (.gitignore, .gitattributes) | ✅ | CFG-001 | git init, commit inicial, branches main+develop |
| CFG-003 | Criar repositório GitHub | ✅ | CFG-002 | github.com/daniloragazzi/WordSearch |
| CFG-004 | Configurar VS Code para Unity | ✅ | CFG-001 | .vscode/, .editorconfig, extensões C#/Unity |

### 2.2 — Domain Layer (Core)

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DEV-001 | Implementar GridData (modelo do grid) | ✅ | CFG-001 | CellData, Direction, GridData, WordPlacement |
| DEV-002 | Implementar WordPlacer (posicionar palavras) | ✅ | DEV-001 | Horizontal, vertical, diagonal, validação colisão |
| DEV-003 | Implementar GridGenerator (gerar grid completo) | ✅ | DEV-002 | Seed determinístico, preenchimento |
| DEV-004 | Implementar WordFinder (validar seleção) | ✅ | DEV-001 | Seleção bidirecional, eventos, dica |
| DEV-005 | Implementar LevelGenerator (gerar nível) | ✅ | DEV-003 | DifficultyConfig, seed hash, seleção palavras |
| DEV-006 | Implementar WordDatabase (carregar palavras) | ✅ | DEV-001 | TextNormalizer, WordModels, WordCategory |

### 2.3 — Infrastructure Layer (Core)

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DEV-007 | Implementar IStorageService + PlayerPrefsStorage | ✅ | CFG-001 | + StorageKeys helper |
| DEV-008 | Implementar ILocalizationService + JsonLocalization | ✅ | CFG-001 | + modelos JSON de localização |
| DEV-009 | Implementar IAdsService + MockAdsService | ✅ | CFG-001 | Mock para dev, AdMob em CFG-005 |
| DEV-010 | Implementar IAnalyticsService + MockAnalytics | ✅ | CFG-001 | Mock para dev, Unity Analytics em CFG-006 |

### 2.4 — Application Layer (Core)

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DEV-011 | Implementar GameState (state machine) | ✅ | DEV-005 | GameStateMachine + transições validadas |
| DEV-012 | Implementar GameManager (orquestrador) | ✅ | DEV-011 | Singleton, serviços, ads, fluxo |
| DEV-013 | Implementar LevelManager (progressão) | ✅ | DEV-005, DEV-007 | Save/load, desbloqueio, geração níveis |

### 2.5 — Dados e Conteúdo

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DAT-001 | Criar script Python para gerar palavras | ✅ | — | Geração curada diretamente |
| DAT-002 | Gerar banco de palavras (8 categorias, ~50+/cat) | ✅ | DAT-001 | 440 palavras, 55/categoria, 0 erros |
| DAT-003 | Criar categories.json | ✅ | DAT-002 | 8 categorias com id, name, icon |
| DAT-004 | Criar JSONs de palavras por categoria | ✅ | DAT-002 | 8 arquivos JSON validados |
| DAT-005 | Criar script Python de validação | ✅ | DAT-004 | validate_words.py: formato, dups, tamanho |

### 2.6 — UI e Cenas

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DEV-014 | Criar cena Boot.unity | ✅ | DEV-012 | BootLoader.cs: carrega dados, transiciona para MainMenu |
| DEV-015 | Criar cena MainMenu.unity + MainMenuScreen | ✅ | DEV-014 | MainMenuScreen.cs + NavigationController.cs |
| DEV-016 | Implementar CategorySelectScreen | ✅ | DEV-013, DAT-003 | CategorySelectScreen.cs + CategoryButtonItem.cs |
| DEV-017 | Implementar LevelSelectScreen | ✅ | DEV-013 | LevelSelectScreen.cs + LevelButtonItem.cs |
| DEV-018 | Criar cena Game.unity | ✅ | DEV-012 | GameplayController.cs |
| DEV-019 | Implementar GridView (renderizar grid) | ✅ | DEV-003, DEV-018 | GridView.cs |
| DEV-020 | Implementar LetterCell (célula individual) | ✅ | DEV-019 | LetterCell.cs com 4 estados visuais |
| DEV-021 | Implementar SelectionLine (arrastar dedo) | ✅ | DEV-019 | SelectionLine.cs, seleção linear touch/drag |
| DEV-022 | Implementar WordListView (lista de palavras) | ✅ | DEV-018 | WordListView.cs + WordListItem.cs, strikethrough |
| DEV-023 | Implementar WinPopup | ✅ | DEV-012 | WinPopup.cs com animação e stats |
| DEV-024 | Implementar SettingsPopup | ✅ | DEV-008 | SettingsPopup.cs, som/música/idioma |

### 2.7 — Design e Assets

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DSN-001 | Definir paleta de cores | ✅ | — | GameTheme.cs ScriptableObject, 30+ cores |
| DSN-002 | Criar ícone do app | ✅ | DSN-001 | Spec em 08_Design_Specs.md, adaptive icon |
| DSN-003 | Criar splash screen | ✅ | DSN-001 | Spec Ragazzi Studios, Nunito ExtraBold |
| DSN-004 | Selecionar fonte (Google Fonts) | ✅ | — | Nunito (SIL OFL), 4 pesos, guia TMP |
| DSN-005 | Criar sprites UI (botões, painéis, ícones) | ✅ | DSN-001 | 30+ specs + PlaceholderSprites.cs procedural |

### 2.8 — Integração e Testes

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| TST-001 | Testes unitários Domain/Grid | ✅ | DEV-003 | GridTests.cs: 30+ test cases (CellData, GridData, WordPlacer, GridGenerator, WordPlacement, WordFinder) |
| TST-002 | Testes unitários Domain/Words | ✅ | DEV-004 | WordsTests.cs: TextNormalizer + WordDatabase |
| TST-003 | Testes unitários Domain/Level | ✅ | DEV-005 | LevelTests.cs: DifficultyConfig + LevelGenerator + LevelData |
| CFG-005 | Integrar Google AdMob SDK | ✅ | DEV-009 | AdMobService.cs (stub), Test IDs, doc 09_SDK_Integration_Guide.md |
| CFG-006 | Integrar Unity Analytics | ✅ | DEV-010 | UnityAnalyticsService.cs (stub), 9 eventos, doc integrado |
| TST-004 | Teste integrado completo | ✅ | Todos DEV | IntegrationTestRunner.cs: 5 testes e2e (120 níveis, determinismo, WordFinder) |

### 2.9 — Build e Publicação

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| BLD-001 | Primeiro build Android (APK) | ✅ | TST-004 | BuildScript.cs, SceneCreator.cs, PlayerSettings Android, doc 10_Build_Test_Guide.md |
| TST-005 | Teste no device real | ✅ | BLD-001 | Checklists 6.1, 6.2, 6.3 OK. Bugs corrigidos (orientation, layout, botões) |
| BLD-002 | Criar conta Google Play Developer | ⏸️ | — | Conta criada, pendente validação — pausado para melhorias |
| BLD-003 | Preparar assets Play Store (screenshots, descrição) | ⏸️ | TST-005 | Pausado — retomar após Fase 3 |
| BLD-004 | Build AAB (Android App Bundle) | ⏸️ | TST-005 | Pausado — retomar após Fase 3 |
| BLD-005 | Publicar na Play Store | ⏸️ | BLD-003, BLD-004 | Pausado — retomar após Fase 3 |

---

## Fase 3 — Melhorias e Polimento

> Foco em qualidade visual, áudio, UX e funcionalidades faltantes antes da publicação.

### 3.1 — Áudio (SFX e Música)

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| AUD-001 | Criar/obter SFX (word found, all found, tap, hint, error) | ✅ | — | `SfxGenerator.cs` gera 5 WAVs procedurais via menu Unity |
| AUD-002 | Criar/obter música de fundo (loop) | ✅ | — | `MusicGenerator.cs` gera loop ambient 32s procedural |
| AUD-003 | Integrar SFX no gameplay | ✅ | AUD-001 | AudioSource + 5 clips wired no SceneCreator; PlaySfx em word/invalid/hint/pause/back |
| AUD-004 | Implementar MusicManager (play/pause/volume) | ✅ | AUD-002 | `MusicManager.cs` singleton DontDestroyOnLoad, criado na Boot scene |
| AUD-005 | Corrigir toggle Som/Música no Settings | ✅ | AUD-003, AUD-004 | Som controla SFX via flag; Música controla MusicManager.SetEnabled |

### 3.2 — Fonte e Tipografia

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| FNT-001 | Importar fonte Nunito (TTF, 4 pesos) | ✅ | — | 4 TTFs baixados do Google Fonts (Regular, SemiBold, Bold, ExtraBold) |
| FNT-002 | Gerar TMP SDF Font Assets | ✅ | FNT-001 | FontAssetGenerator.cs — menu 'Generate Font Assets' no Unity |
| FNT-003 | Aplicar Nunito em todos os textos (SceneCreator) | ✅ | FNT-002 | ApplyFontsToScene() auto-aplica peso por fontSize |

### 3.3 — UI Visual e Sprites

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DSN-006 | Criar sprites de botões (9-slice) | ✅ | — | SpriteGenerator.cs: btn_primary (256x96), btn_secondary, btn_circle (96x96) |
| DSN-007 | Criar sprites de painéis/cards | ✅ | — | panel_popup (256x256), panel_card (192x192), panel_overlay, cell_bg (96x96) |
| DSN-008 | Criar ícones de categoria (8 ícones) | ✅ | — | 8 ícones procedurais 128x128 (pata, maçã, coração, folha, estrela, globo, bola, quadrados) |
| DSN-009 | Criar ícone do app (adaptive icon) | ✅ | — | 512x512 gradiente radial azul + grid 3x3 + linha de seleção diagonal |
| DSN-010 | Criar splash screen art | ✅ | — | 512x512 com "R" estilizado Ragazzi Studios |
| DSN-011 | Aplicar cores por categoria | ✅ | DSN-008 | CategoryButtonItem.cs com 8 cores únicas + ApplyCategoryColor/Icon |

### 3.4 — Animações e Feedback Visual

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| ANI-001 | Animação de transição entre telas (fade/slide) | ✅ | — | SceneTransition.cs com overlay fade 0.3s + LoadSceneAsync |
| ANI-002 | Animação de célula selecionada (pulse/scale) | ✅ | — | Pop 1→1.15→1 em 120ms ao SetState(Selected) |
| ANI-003 | Animação de palavra encontrada (flash + cor) | ✅ | — | Pulso cascata nas células (scale 1→1.25→1, 40ms delay) |
| ANI-004 | Celebração de vitória (partículas/confete) | ✅ | — | ConfettiEffect.cs — 60 UI Images coloridas caindo com sway/rotação |
| ANI-005 | Feedback de seleção inválida (shake) | ✅ | — | Grid shake (sin 50Hz) + flash vermelho nas células |

### 3.5 — Gameplay e UX

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| UX-001 | Implementar Pause Popup | ✅ | — | PausePopup.cs com Time.timeScale=0, animações unscaled |
| UX-002 | Mostrar timer durante gameplay | ✅ | — | Timer M:SS no header, atualiza via Update() |
| UX-003 | Melhorar visual da SelectionLine (endpoints arredondados) | ✅ | — | Sprite pílula procedural 9-slice + linhas coloridas persistentes por palavra |
| UX-004 | Tutorial de primeiro uso | ✅ | — | TutorialPopup.cs com 3 passos, flag `TUTORIAL_COMPLETED` no storage |
| UX-005 | Tela de loading entre cenas | ✅ | ANI-001 | Spinner rotativo + progress bar azul durante LoadSceneAsync |

### 3.6 — Teste e Build Final

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| TST-006 | Teste completo no device (pós-melhorias) | ✅ | 3.1–3.5 | Todas melhorias visuais/UX validadas no device |
| BLD-006 | Novo build APK com melhorias | ✅ | TST-006 | APK gerado e testado com sucesso |

### 3.7 — Funcionalidades Extras

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| FT-001 | Modo Desafio — tela de seleção + grids grandes | ✅ | — | ChallengeSelectScreen, 3 tamanhos (20x10, 20x14, 20x16), 10 palavras mistas de todas as categorias |
| FT-002 | Responsividade — MainMenu anchor-based layout + margens grid | ✅ | — | VerticalLayoutGroup no menu, grid margins 4%, category grid padding ajustado |

---

## Fase 4 — Revisão Estruturada (Projeto + UX/Layout)

> Checklist definido em brainstorm para manter trilha de decisão antes da execução.
> Referência: `docs/Brainstorm/Discussion_06_Revision_Checklist.md`

### 4.1 — Governança da Revisão

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| REV-001 | Consolidar critérios da revisão (arquitetura, produto, UX, visual) | ✅ | — | Baseado em `Discussion_06_Revision_Checklist.md` |
| REV-002 | Mapear decisões atuais para manter/ajustar/remover | ✅ | REV-001 | Saída: matriz de decisão com justificativa |
| REV-003 | Criar backlog priorizado da revisão (P0/P1/P2) | ✅ | REV-002 | Backlog consolidado e ativo como referência de execução |

### 4.2 — Projeto e Arquitetura

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| ARQ-001 | Revisar decisões de arquitetura Core/Game e state machine | ✅ | REV-001 | Aderência validada v0.1; ajustes pontuais registrados no tracker |
| ARQ-002 | Revisar estratégia de serviços mock/real (Ads/Analytics/Storage) | 🔴 | REV-001 | Bloqueado por dependências externas de produção (SDK/IDs/consentimento) |
| ARQ-003 | Revisar política de extensão do modo desafio no fluxo principal | ✅ | ARQ-001 | Política definida v1: desafio segmentado por modo e KPI separado do funil MVP |

### 4.3 — Usabilidade e Layout Visual

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| UX-006 | Executar revisão heurística do fluxo completo (Menu → Vitória) | ✅ | REV-001 | Fricções mapeadas e classificadas; onboarding delegado a UX-004 |
| UX-007 | Auditar contraste, tipografia e legibilidade por tela | ✅ | UX-006 | Contraste e legibilidade ajustados/validados na rodada atual |
| UX-008 | Validar responsividade em múltiplas resoluções Android | ✅ | UX-006 | Validação iterativa em device real; P0 eliminados; ajustes de header/nav/settings aplicados |
| UX-009 | Revisar consistência visual (tema vs cores hardcoded) | ✅ | UX-007 | Migração e validação visual consolidadas (gates fechados no tracker) |

### 4.4 — Validação e Fechamento

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| TST-007 | Rodar regressão funcional após ajustes da revisão | ✅ | ARQ-003, UX-009 | Regressão funcional consolidada e encerrada no tracker |
| DOC-009 | Publicar relatório consolidado da revisão | ✅ | REV-003, TST-007 | Relatório final consolidado em `11_Review_Report.md` |

---

## Fase 5 — Melhorias Pós-MVP (enquanto aguarda validação Google Play)

> Três frentes de melhoria aproveitando o tempo de espera da validação da conta de desenvolvedor.
> Todas podem ser desenvolvidas em paralelo e são independentes entre si.

### 5.1 — Ícone do App (baseado em screenshot de jogo real)

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| ICO-001 | Gerar ícone do app com visual de grid de jogo real | ✅ | — | `AppIconGenerator.cs`: 1024×1024 com grid 8×8, letras bitmap 5×7, found cells (FERRUGEM+CUBO) em verde, fundo escuro |
| ICO-002 | Gerar variações adaptive icon (foreground + background) | ✅ | ICO-001 | Foreground 432×432 (safe zone 66.7%) + Background 432×432 (sólido escuro) |
| ICO-003 | Configurar ícone no ProjectSettings (todas resoluções Android) | ✅ | ICO-002 | `AssignToPlayerSettings()`: Adaptive (fg+bg), Legacy e Round configurados via API |
| ICO-004 | Gerar ícone Play Store (512×512 Feature Graphic) | ✅ | ICO-001 | `app_icon.png` 512×512 combinado (grid+fundo) para Play Store |

### 5.2 — Expansão do Banco de Palavras + Base Desafio

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DAT-006 | Expandir banco de palavras por categoria (meta: 100+ por cat) | ✅ | — | 8 categorias expandidas: 105–111 palavras cada (total 873 vs 440 anteriores) |
| DAT-007 | Criar banco de palavras genérico para Desafio (`desafio.json`) | ✅ | — | 120 palavras genéricas (objetos, móveis, ferramentas, roupas, sentimentos, viagem) |
| DAT-008 | Atualizar `LevelManager` para carregar `desafio.json` no modo Desafio | ✅ | DAT-007 | BootLoader carrega desafio.json; LevelManager prioriza pool desafio + categorias |
| DAT-009 | Atualizar `validate_words.py` para incluir `desafio.json` | ✅ | DAT-007 | Validação explícita desafio, MIN_WORDS 80, MAX_WORD_LENGTH 19, UTF-8 BOM fix |
| DAT-010 | Validar banco expandido completo (sem erros) | ✅ | DAT-006, DAT-009 | 0 erros, 12 avisos cross-dup intencionais, 993 palavras total |

### 5.3 — Tema Claro / Escuro (com detecção do sistema)

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| THM-001 | Criar paleta de cores para tema escuro (`GameTheme` dark) | ✅ | — | `ThemePaletteGenerator.cs`: gera `GameTheme_Light` e `GameTheme_Dark` em `Config/Themes/`; campo `isDark` adicionado ao `GameTheme` |
| THM-002 | Implementar `ThemeManager` (singleton, detecção sistema, persistência) | ✅ | THM-001 | `ThemeManager.cs` em `Core/Application/`: singleton DontDestroyOnLoad, detecta Android dark mode via JNI, persiste via `StorageKeys.THEME_MODE`, evento `OnThemeChanged`, criado na Boot scene pelo SceneCreator |
| THM-003 | Adicionar seletor de tema no `SettingsPopup` (Sistema / Claro / Escuro) | ✅ | THM-002 | Dropdown com 3 opções wired em `SettingsPopup`; SceneCreator cria label + dropdown na linha 4 do painel |
| THM-004 | Refatorar `SceneCreator` para gerar cenas sem cores hardcoded no tema | ✅ | THM-001 | `LoadThemeColors()` carrega especificamente `GameTheme_Light`; `ThemeColorBinding` adicionado a painel e labels do SettingsPopup; `SetPanelAnchors` helper adicionado |
| THM-005 | Implementar troca de tema em runtime (atualizar todos os componentes) | ✅ | THM-002, THM-004 | `ThemeColorBinding.cs` em `Core/Application/`: assina `ThemeManager.OnThemeChanged`, aplica token de cor a `Image`/`TMP_Text`/`Graphic`; `ThemeColorRole` enum com 18 tokens |
| THM-006 | Validar tema escuro no device real (contraste, legibilidade, responsividade) | ✅ | THM-005 | Teste completo em device: todas as telas, popups, grid, word list, settings |

### 5.4 — Validação e Build Fase 5

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| TST-008 | Teste completo no device (pós-melhorias Fase 5) | ✅ | 5.1–5.3 | Validar ícone, palavras expandidas, tema escuro/claro, transições |
| BLD-007 | Novo build APK com melhorias Fase 5 | ✅ | TST-008 | APK dev gerado e validado no device; build release (AAB) pendente junto com BLD-004 |

---

## App 2 — Termo BR

> Segundo app do estúdio. Repo separado: `github.com/daniloragazzi/TermoBR`.
> Core compartilhado via package local `RagazziCore` (`com.ragazzistudios.core`).
> Referências: Discussion_07..09 em `docs/Brainstorm/`.

### T1.0 — RagazziCore Package (pré-requisito de todos os apps)

> **Executar antes de qualquer trabalho no TermoBR.**
> O package também será backportado para o WordSearch — validando que o App 1 continua funcionando antes de avançar.

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| PKG-000 | Criar repositório GitHub `RagazziCore` | ✅ | — | `github.com/daniloragazzi/RagazziCore`, repo público, branches main+develop |
| PKG-001 | Criar estrutura do package com `package.json` | ✅ | PKG-000 | `"name": "com.ragazzistudios.core"`, Runtime/, Editor/, README |
| PKG-002 | Portar `Core/Infrastructure` para o package | ✅ | PKG-001 | StorageService, LocalizationService, AdsService, AnalyticsService, ServiceLocator |
| PKG-003 | Portar `Core/Application` para o package | ✅ | PKG-001 | ThemeManager, MusicManager, GameStateMachine base, StorageKeys |
| PKG-004 | Atualizar `GameTheme` com tokens de feedback do Termo | ✅ | PKG-001 | Adicionar `letterCorrect`, `letterPresent`, `letterAbsent`, `letterUnused`, `letterOnColor` |
| PKG-005 | Portar `ThemePaletteGenerator` para o package Editor | ✅ | PKG-004 | Gerar `GameTheme_Light` e `GameTheme_Dark` com novos tokens |
| PKG-006 | Referenciar package no WordSearch via URL git e remover Core local | ✅ | PKG-001..005 | `"com.ragazzistudios.core": "https://github.com/daniloragazzi/RagazziCore.git"` no manifest.json do WordSearch; remover `Assets/_Project/Core/` |
| PKG-007 | Validar que o WordSearch compila e funciona com o package | ✅ | PKG-006 | Regenerar cenas, build APK dev, teste no device — sem regressão |

### T1.1 — Setup e Configuração (TermoBR)

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| CFG-T01 | Criar repositório GitHub `TermoBR` | ⬜ | PKG-007 | `github.com/daniloragazzi/TermoBR`, branches main+develop, .gitignore Unity |
| CFG-T02 | Criar projeto Unity `Termo/` no novo repo | ⬜ | CFG-T01 | Unity 6.3 LTS, template 2D, package name `com.ragazzistudios.termo` |
| CFG-T03 | Configurar Git (.gitignore, .gitattributes, README) | ⬜ | CFG-T01 | Mesmo padrão do WordSearch |
| CFG-T04 | Configurar VS Code para o novo projeto | ⬜ | CFG-T02 | .vscode/, .editorconfig |
| CFG-T05 | Criar estrutura de pastas `Assets/_Project/` | ⬜ | CFG-T02 | Core/, Game/, Editor/, Art/, Resources/ |
| CFG-T06 | Configurar Android build settings | ⬜ | CFG-T02 | Package name, ícone placeholder, keystore dev |

### T1.2 — Integração RagazziCore no TermoBR

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| PKG-T01 | Referenciar RagazziCore no manifest do TermoBR | ⬜ | CFG-T02, PKG-007 | `"com.ragazzistudios.core": "https://github.com/daniloragazzi/RagazziCore.git"` no manifest.json |

### T1.3 — Domain Layer (Termo)

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DEV-T01 | Implementar `LetterState` (enum) | ⬜ | PKG-006 | Correct / Present / Absent / Unused |
| DEV-T02 | Implementar `TermoGuess` | ⬜ | DEV-T01 | 5 letras + array de `LetterState` |
| DEV-T03 | Implementar `TermoBoard` (domain) | ⬜ | DEV-T02 | Lista de `TermoGuess`, palavra-alvo, estado (Playing/Won/Lost) |
| DEV-T04 | Implementar `TermoValidator` | ⬜ | DEV-T03 | Valida tentativa contra palavra-alvo; retorna array de `LetterState` |
| DEV-T05 | Implementar `TermoGame` | ⬜ | DEV-T03 | Estado completo: 1, 2 ou 4 boards; modo enum; contagem de tentativas restantes |

### T1.4 — Application Layer (Termo)

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DEV-T06 | Implementar `WordBankService` | ⬜ | PKG-006 | Carrega `words_5.json` (alvos) e `valid_5.json` (dicionário); normaliza uppercase sem acento |
| DEV-T07 | Implementar `StatsManager` | ⬜ | PKG-003 | Lê/grava partidas jogadas, vitórias, streak, distribuição de tentativas via StorageService |
| DEV-T08 | Implementar `TermoGameManager` | ⬜ | DEV-T05, DEV-T06, DEV-T07 | Singleton; orquestra partida ativa, modo (1/2/4), histórico, sorteio de palavra |
| DEV-T09 | Adaptar `GameStateMachine` para estados do Termo | ⬜ | PKG-003 | Estados: Boot, MainMenu, ModeSelect, Playing, Win, Lose |

### T1.5 — Dados e Conteúdo

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DAT-T01 | Criar script Python `build_termo.py` | ⬜ | — | Filtra + cura palavras de 5 letras; normaliza sem acento; gera 2 JSONs |
| DAT-T02 | Gerar `words_5.json` (palavras-alvo) | ⬜ | DAT-T01 | Meta: 1.000+ palavras comuns de 5 letras |
| DAT-T03 | Gerar `valid_5.json` (dicionário completo) | ⬜ | DAT-T01 | Meta: 3.000+ palavras aceitas como tentativa |
| DAT-T04 | Criar script de validação `validate_termo.py` | ⬜ | DAT-T02, DAT-T03 | Sem duplicatas, comprimento exato 5, charset `[A-Z]`, cobertura mínima |
| DAT-T05 | Validar banco completo (sem erros) | ⬜ | DAT-T04 | 0 erros de formato; palavras-alvo são subconjunto das válidas |

### T1.6 — UI e Cenas

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DEV-T10 | Criar `SceneCreator.cs` (novo, independente) | ⬜ | PKG-006 | Editor tool; gera Boot, MainMenu, Game; sem herança do WordSearch |
| DEV-T11 | Criar cena Boot.unity | ⬜ | DEV-T10 | Carrega banco de palavras, inicializa serviços, vai para MainMenu |
| DEV-T12 | Criar cena MainMenu.unity + `MainMenuScreen` | ⬜ | DEV-T11 | Logo, botão Jogar, botão Configurações |
| DEV-T13 | Implementar `ModeSelectScreen` | ⬜ | DEV-T12 | Seleção de modo: 1 / 2 / 4 palavras com descrição de tentativas |
| DEV-T14 | Criar cena Game.unity | ⬜ | DEV-T09 | Cena principal de gameplay |
| DEV-T15 | Implementar `TermoCell` (UI) | ⬜ | DEV-T14 | Célula com letra, estado visual e animação flip (150ms cada metade, 100ms delay entre células) |
| DEV-T16 | Implementar `TermoBoardView` (UI) | ⬜ | DEV-T15 | Grade N linhas × 5 colunas; suporta 6, 7 ou 9 tentativas conforme modo |
| DEV-T17 | Implementar `TermoKeyboard` (UI) | ⬜ | DEV-T14 | Layout QWERTY PT-BR; teclas Enter e ⌫; cor reflete melhor estado acumulado por letra |
| DEV-T18 | Implementar `GameplayController` | ⬜ | DEV-T08, DEV-T16, DEV-T17 | Coordena boards + teclado + submissão de tentativa + feedback |
| DEV-T19 | Implementar `WinPopup` | ⬜ | DEV-T18 | Número de tentativas, streak, botão Jogar Novamente |
| DEV-T20 | Implementar `LosePopup` | ⬜ | DEV-T18 | Revela palavra-alvo imediatamente, botão Jogar Novamente |
| DEV-T21 | Implementar `StatsPopup` | ⬜ | DEV-T07 | Total de partidas, % vitórias, streak atual/máximo, distribuição de tentativas (barras) |
| DEV-T22 | Implementar `SettingsPopup` | ⬜ | PKG-003 | Som, Música, Tema — reaproveitado via RagazziCore |

### T1.7 — Design e Assets

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DSN-T01 | Definir paleta do Termo BR (derivada do WordSearch + tokens de feedback) | ⬜ | PKG-004, PKG-005 | Verde/amarelo/cinza para feedback; paleta base Ragazzi Studios |
| DSN-T02 | Gerar sprites para células, teclado e painéis | ⬜ | DSN-T01 | `SpriteGenerator.cs` novo ou portado; células quadradas, teclas arredondadas |
| DSN-T03 | Criar ícone do app Termo BR (512×512 + adaptive) | ⬜ | DSN-T01 | `AppIconGenerator.cs` novo; visual com grade 5×5 e letras coloridas (verde/amarelo) |
| DSN-T04 | Criar splash screen Termo BR | ⬜ | DSN-T01 | Identidade Ragazzi Studios com nome "Termo BR" |

### T1.8 — Áudio

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| AUD-T01 | Criar/adaptar SFX (flip, acerto, erro, vitória, derrota) | ⬜ | — | `SfxGenerator.cs` novo ou portado; 5 clips WAV procedurais |
| AUD-T02 | Integrar MusicManager do RagazziCore | ⬜ | PKG-003, DEV-T11 | Loop ambient criado na Boot scene via SceneCreator |

### T1.9 — Testes e Build

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| TST-T01 | Testes unitários Domain (TermoValidator, TermoBoard, TermoGame) | ⬜ | DEV-T05 | Casos: palavra correta, presentes, ausentes, duplicatas, vitória, derrota |
| TST-T02 | Testes unitários Application (WordBankService, StatsManager) | ⬜ | DEV-T07 | Carregamento de JSON, normalização, incremento de stats |
| TST-T03 | Teste integrado completo (Boot → ModeSelect → Gameplay → Win/Lose) | ⬜ | T1.3–T1.6 | Fluxo ponta a ponta nos 3 modos (1/2/4 palavras) |
| TST-T04 | Teste no device real | ⬜ | TST-T03 | Checklists: layout, teclado, animações, áudio, tema claro/escuro |
| BLD-T01 | Build APK dev Termo BR | ⬜ | TST-T04 | APK de desenvolvimento para validação |
| BLD-T02 | Preparar assets Play Store (screenshots, descrição, ícone) | ⬜ | BLD-T01 | Screenshots dos 3 modos; descrição PT-BR |
| BLD-T03 | Build AAB (release) + publicar na Play Store | ⬜ | BLD-T02, CFG-T06 | Dependente de conta Google Play ativa |

---

| Etapa | Total | ⬜ | ⏸️ | 🔵 | 🔴 | ✅ | % |
|-------|-------|-----|-----|-----|-----|-----|---|
| 2.1 Setup | 4 | 0 | 0 | 0 | 0 | 4 | 100% |
| 2.2 Domain | 6 | 0 | 0 | 0 | 0 | 6 | 100% |
| 2.3 Infrastructure | 4 | 0 | 0 | 0 | 0 | 4 | 100% |
| 2.4 Application | 3 | 0 | 0 | 0 | 0 | 3 | 100% |
| 2.5 Dados | 5 | 0 | 0 | 0 | 0 | 5 | 100% |
| 2.6 UI/Cenas | 11 | 0 | 0 | 0 | 0 | 11 | 100% |
| 2.7 Design | 5 | 0 | 0 | 0 | 0 | 5 | 100% |
| 2.8 Testes/Integração | 6 | 0 | 0 | 0 | 0 | 6 | 100% |
| 2.9 Build/Publicação | 6 | 0 | 4 | 0 | 0 | 2 | 33% |
| 3.1 Áudio | 5 | 0 | 0 | 0 | 0 | 5 | 100% |
| 3.2 Fonte | 3 | 0 | 0 | 0 | 0 | 3 | 100% |
| 3.3 UI/Sprites | 6 | 0 | 0 | 0 | 0 | 6 | 100% |
| 3.4 Animações | 5 | 0 | 0 | 0 | 0 | 5 | 100% |
| 3.5 Gameplay/UX | 5 | 0 | 0 | 0 | 0 | 5 | 100% |
| 3.6 Teste Final | 2 | 0 | 0 | 0 | 0 | 2 | 100% |
| 3.7 Extras | 2 | 0 | 0 | 0 | 0 | 2 | 100% |
| 4.1 Governança | 3 | 0 | 0 | 0 | 0 | 3 | 100% |
| 4.2 Arquitetura | 3 | 0 | 0 | 0 | 1 | 2 | 67% |
| 4.3 UX/Layout | 4 | 0 | 0 | 0 | 0 | 4 | 100% |
| 4.4 Validação | 2 | 0 | 0 | 0 | 0 | 2 | 100% |
| 5.1 Ícone | 4 | 0 | 0 | 0 | 0 | 4 | 100% |
| 5.2 Palavras | 5 | 0 | 0 | 0 | 0 | 5 | 100% |
| 5.3 Tema | 6 | 0 | 0 | 0 | 0 | 6 | 100% |
| 5.4 Validação F5 | 2 | 0 | 0 | 0 | 0 | 2 | 100% |
| **TOTAL App 1** | **105** | **0** | **4** | **0** | **1** | **100** | **95%** |

---

## Resumo de Progresso — App 2 (Termo BR)

| Etapa | Total | ⬜ | ⏸️ | 🔵 | 🔴 | ✅ | % |
|-------|-------|-----|-----|-----|-----|-----|---|
| T1.0 RagazziCore Package | 8 | 8 | 0 | 0 | 0 | 0 | 0% |
| T1.1 Setup | 6 | 6 | 0 | 0 | 0 | 0 | 0% |
| T1.2 Integrar Core no TermoBR | 1 | 1 | 0 | 0 | 0 | 0 | 0% |
| T1.3 Domain | 5 | 5 | 0 | 0 | 0 | 0 | 0% |
| T1.4 Application | 4 | 4 | 0 | 0 | 0 | 0 | 0% |
| T1.5 Dados | 5 | 5 | 0 | 0 | 0 | 0 | 0% |
| T1.6 UI/Cenas | 13 | 13 | 0 | 0 | 0 | 0 | 0% |
| T1.7 Design | 4 | 4 | 0 | 0 | 0 | 0 | 0% |
| T1.8 Áudio | 2 | 2 | 0 | 0 | 0 | 0 | 0% |
| T1.9 Testes e Build | 7 | 7 | 0 | 0 | 0 | 0 | 0% |
| **TOTAL App 2** | **48** | **48** | **0** | **0** | **0** | **0** | **0%** |

---

## Ordem de Execução Recomendada

```
App 1 — Caça-Palavras
  Fases 2–5 (concluídas)
  → Aguardando validação Google Play:
    ├── BLD-002..005 (Play Store) — desbloqueiam quando conta for aprovada
    └── ARQ-002 (Ads/Analytics reais) — depende de IDs de produção externos

App 2 — Termo BR (paralelo ao App 1)
  T1.0 RagazziCore Package (repo próprio) → PKG-007 Validar WordSearch ✓
    → T1.1 Setup TermoBR → T1.2 Integrar Core
      → T1.3 Domain + T1.4 Application + T1.5 Dados (paralelos)
        → T1.6 UI/Cenas → T1.7 Design + T1.8 Áudio (paralelos)
          → T1.9 Testes e Build
```

> App 1 e App 2 são desenvolvidos em paralelo e independentes entre si.
> O `RagazziCore` package (T1.2) é pré-requisito crítico de toda a cadeia do App 2.
