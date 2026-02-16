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
| BLD-001 | Primeiro build Android (APK) | ⬜ | TST-004 | Configurações Android |
| TST-005 | Teste no device real | ⬜ | BLD-001 | Gameplay, ads, performance |
| BLD-002 | Criar conta Google Play Developer | ⬜ | — | Ragazzi Studios, taxa $25 |
| BLD-003 | Preparar assets Play Store (screenshots, descrição) | ⬜ | TST-005 | Listing da loja |
| BLD-004 | Build AAB (Android App Bundle) | ⬜ | TST-005 | Formato exigido pela Play Store |
| BLD-005 | Publicar na Play Store | ⬜ | BLD-003, BLD-004 | Closed testing → Production |

---

## Resumo de Progresso

| Etapa | Total | ⬜ | 🔵 | ✅ | % |
|-------|-------|-----|-----|-----|---|
| 2.1 Setup | 4 | 0 | 0 | 4 | 100% |
| 2.2 Domain | 6 | 0 | 0 | 6 | 100% |
| 2.3 Infrastructure | 4 | 0 | 0 | 4 | 100% |
| 2.4 Application | 3 | 0 | 0 | 3 | 100% |
| 2.5 Dados | 5 | 0 | 0 | 5 | 100% |
| 2.6 UI/Cenas | 11 | 0 | 0 | 11 | 100% |
| 2.7 Design | 5 | 0 | 0 | 5 | 100% |
| 2.8 Testes/Integração | 6 | 0 | 0 | 6 | 100% |
| 2.9 Build/Publicação | 5 | 5 | 0 | 0 | 0% |
| **TOTAL** | **49** | **5** | **0** | **44** | **90%** |

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
