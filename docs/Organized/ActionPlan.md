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
| DEV-007 | Implementar IStorageService + PlayerPrefsStorage | ⬜ | CFG-001 | Salvar/carregar progresso |
| DEV-008 | Implementar ILocalizationService + JsonLocalization | ⬜ | CFG-001 | Strings de UI externalizadas |
| DEV-009 | Implementar IAdsService + AdMobService (placeholder) | ⬜ | CFG-001 | Interface + mock para dev |
| DEV-010 | Implementar IAnalyticsService + UnityAnalytics | ⬜ | CFG-001 | Interface + implementação básica |

### 2.4 — Application Layer (Core)

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DEV-011 | Implementar GameState (state machine) | ⬜ | DEV-005 | Boot, Menu, Playing, Win |
| DEV-012 | Implementar GameManager (orquestrador) | ⬜ | DEV-011 | Singleton, coordena tudo |
| DEV-013 | Implementar LevelManager (progressão) | ⬜ | DEV-005, DEV-007 | Desbloqueio, save/load |

### 2.5 — Dados e Conteúdo

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DAT-001 | Criar script Python para gerar palavras | ⬜ | — | Geração + curadoria com IA |
| DAT-002 | Gerar banco de palavras (8 categorias, ~50+/cat) | ⬜ | DAT-001 | Validação: sem duplicatas, min 3 letras |
| DAT-003 | Criar categories.json | ⬜ | DAT-002 | Estrutura i18n pronta |
| DAT-004 | Criar JSONs de palavras por categoria | ⬜ | DAT-002 | 8 arquivos JSON |
| DAT-005 | Criar script Python de validação | ⬜ | DAT-004 | Validar integridade dos JSONs |

### 2.6 — UI e Cenas

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DEV-014 | Criar cena Boot.unity | ⬜ | DEV-012 | Inicialização, loading |
| DEV-015 | Criar cena MainMenu.unity + MainMenuScreen | ⬜ | DEV-014 | Botão Jogar, Configurações |
| DEV-016 | Implementar CategorySelectScreen | ⬜ | DEV-013, DAT-003 | Grid de categorias + progresso |
| DEV-017 | Implementar LevelSelectScreen | ⬜ | DEV-013 | Grid de níveis, bloqueado/desbloqueado |
| DEV-018 | Criar cena Game.unity | ⬜ | DEV-012 | Cena do gameplay |
| DEV-019 | Implementar GridView (renderizar grid) | ⬜ | DEV-003, DEV-018 | Grid visual, letras |
| DEV-020 | Implementar LetterCell (célula individual) | ⬜ | DEV-019 | Visual da letra, estados |
| DEV-021 | Implementar SelectionLine (arrastar dedo) | ⬜ | DEV-019 | Input touch/drag |
| DEV-022 | Implementar WordListView (lista de palavras) | ⬜ | DEV-018 | Palavras a encontrar, riscado |
| DEV-023 | Implementar WinPopup | ⬜ | DEV-012 | Parabéns + próximo nível |
| DEV-024 | Implementar SettingsPopup | ⬜ | DEV-008 | Som, música, idioma |

### 2.7 — Design e Assets

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| DSN-001 | Definir paleta de cores | ⬜ | — | 3-4 cores, tons quentes/amigáveis |
| DSN-002 | Criar ícone do app | ⬜ | DSN-001 | Grid de letras estilizado |
| DSN-003 | Criar splash screen | ⬜ | DSN-001 | Logo Ragazzi Studios |
| DSN-004 | Selecionar fonte (Google Fonts) | ⬜ | — | Legível, casual, gratuita |
| DSN-005 | Criar sprites UI (botões, painéis, ícones) | ⬜ | DSN-001 | Mínimo necessário |

### 2.8 — Integração e Testes

| Código | Ação | Status | Dependência | Notas |
|--------|------|--------|-------------|-------|
| TST-001 | Testes unitários Domain/Grid | ⬜ | DEV-003 | NUnit, Unity Test Runner |
| TST-002 | Testes unitários Domain/Words | ⬜ | DEV-004 | Validação de seleção |
| TST-003 | Testes unitários Domain/Level | ⬜ | DEV-005 | Seed determinístico |
| CFG-005 | Integrar Google AdMob SDK | ⬜ | DEV-009 | SDK real, test ads |
| CFG-006 | Integrar Unity Analytics | ⬜ | DEV-010 | Eventos configurados |
| TST-004 | Teste integrado completo | ⬜ | Todos DEV | Fluxo completo no editor |

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
| 2.3 Infrastructure | 4 | 4 | 0 | 0 | 0% |
| 2.4 Application | 3 | 3 | 0 | 0 | 0% |
| 2.5 Dados | 5 | 5 | 0 | 0 | 0% |
| 2.6 UI/Cenas | 11 | 11 | 0 | 0 | 0% |
| 2.7 Design | 5 | 5 | 0 | 0 | 0% |
| 2.8 Testes/Integração | 6 | 6 | 0 | 0 | 0% |
| 2.9 Build/Publicação | 5 | 5 | 0 | 0 | 0% |
| **TOTAL** | **49** | **39** | **0** | **10** | **20%** |

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
