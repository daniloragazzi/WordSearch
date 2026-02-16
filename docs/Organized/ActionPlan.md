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
| AUD-001 | Criar/obter SFX (word found, all found, tap, hint, error) | ⬜ | — | Assets royalty-free ou gerados (sfxr/jsfxr) |
| AUD-002 | Criar/obter música de fundo (loop) | ⬜ | — | 1-2 tracks ambient/lo-fi, royalty-free |
| AUD-003 | Integrar SFX no gameplay | ⬜ | AUD-001 | Conectar AudioClips aos eventos existentes |
| AUD-004 | Implementar MusicManager (play/pause/volume) | ⬜ | AUD-002 | Singleton, crossfade, respeitar toggle Settings |
| AUD-005 | Corrigir toggle Som/Música no Settings | ⬜ | AUD-003, AUD-004 | Separar volume SFX vs Music (não usar AudioListener global) |

### 3.2 — Fonte e Tipografia

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| FNT-001 | Importar fonte Nunito (TTF, 4 pesos) | ✅ | — | 4 TTFs baixados do Google Fonts (Regular, SemiBold, Bold, ExtraBold) |
| FNT-002 | Gerar TMP SDF Font Assets | ✅ | FNT-001 | FontAssetGenerator.cs — menu 'Generate Font Assets' no Unity |
| FNT-003 | Aplicar Nunito em todos os textos (SceneCreator) | ✅ | FNT-002 | ApplyFontsToScene() auto-aplica peso por fontSize |

### 3.3 — UI Visual e Sprites

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DSN-006 | Criar sprites de botões (9-slice) | ⬜ | — | Rounded rect, hover/pressed states |
| DSN-007 | Criar sprites de painéis/cards | ⬜ | — | Backgrounds com bordas arredondadas |
| DSN-008 | Criar ícones de categoria (8 ícones) | ⬜ | — | Ícones simples para cada categoria |
| DSN-009 | Criar ícone do app (adaptive icon) | ⬜ | — | Foreground + background layers, 512x512 |
| DSN-010 | Criar splash screen art | ⬜ | — | Logo Ragazzi Studios |
| DSN-011 | Aplicar cores por categoria | ⬜ | DSN-008 | Cada categoria com cor/gradiente próprio |

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
| UX-004 | Tutorial de primeiro uso | ⬜ | — | Overlay simples mostrando como jogar |
| UX-005 | Tela de loading entre cenas | ⬜ | ANI-001 | Progress bar ou spinner durante carregamento |

### 3.6 — Teste e Build Final

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| TST-006 | Teste completo no device (pós-melhorias) | ⬜ | 3.1–3.5 | Validar todas as melhorias no dispositivo |
| BLD-006 | Novo build APK com melhorias | ⬜ | TST-006 | Build para teste final antes de publicar |

---

## Resumo de Progresso

| Etapa | Total | ⬜ | ⏸️ | 🔵 | ✅ | % |
|-------|-------|-----|-----|-----|-----|---|
| 2.1 Setup | 4 | 0 | 0 | 0 | 4 | 100% |
| 2.2 Domain | 6 | 0 | 0 | 0 | 6 | 100% |
| 2.3 Infrastructure | 4 | 0 | 0 | 0 | 4 | 100% |
| 2.4 Application | 3 | 0 | 0 | 0 | 3 | 100% |
| 2.5 Dados | 5 | 0 | 0 | 0 | 5 | 100% |
| 2.6 UI/Cenas | 11 | 0 | 0 | 0 | 11 | 100% |
| 2.7 Design | 5 | 0 | 0 | 0 | 5 | 100% |
| 2.8 Testes/Integração | 6 | 0 | 0 | 0 | 6 | 100% |
| 2.9 Build/Publicação | 6 | 0 | 4 | 0 | 2 | 33% |
| 3.1 Áudio | 5 | 5 | 0 | 0 | 0 | 0% |
| 3.2 Fonte | 3 | 0 | 0 | 0 | 3 | 100% |
| 3.3 UI/Sprites | 6 | 6 | 0 | 0 | 0 | 0% |
| 3.4 Animações | 5 | 0 | 0 | 0 | 5 | 100% |
| 3.5 Gameplay/UX | 5 | 2 | 0 | 0 | 3 | 60% |
| 3.6 Teste Final | 2 | 2 | 0 | 0 | 0 | 0% |
| **TOTAL** | **76** | **15** | **4** | **0** | **57** | **75%** |

---

## Ordem de Execução Recomendada

```
CFG-001..004 (Setup)
  → DEV-001..006 (Domain) + TST-001..003 (Testes Domain)
    → DEV-007..010 (Infrastructure)
      → DEV-011..013 (Application)
        → DAT-001..005 (Dados) — pode ser paralelo
        → DSN-001..005 (Design) — pode ser paralelo
          → DEV-014..024 (UI/Cenas)
            → CFG-005..006 (Integração SDK)
              → TST-004 (Teste integrado)
                → BLD-001..005 (Build/Publicação)
```

> **DAT** e **DSN** podem ser feitos em paralelo com **DEV** da Domain/Infrastructure.
