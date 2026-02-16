# Execution Tracker — WordGames Studio

> Documento de acompanhamento de todas as ações e decisões do projeto.

---

## Codificação de Ações

| Código | Tipo | Descrição |
|--------|------|-----------|
| **DEF** | Definição | Decisões conceituais e estratégicas |
| **DOC** | Documentação | Criação de documentos formais organizados |
| **DEV** | Desenvolvimento | Implementação de código/features |
| **CFG** | Configuração | Setup de ambiente, ferramentas, pipelines |
| **TST** | Teste | Testes e validações |
| **DSN** | Design | UI/UX, assets visuais, temas |
| **DAT** | Dados | Banco de palavras, conteúdo, níveis |
| **BLD** | Build | Build, deploy, publicação |

> Novos tipos serão adicionados conforme necessário.

---

## Legenda de Status

| Emoji | Status |
|-------|--------|
| ⬜ | Não iniciado |
| 🟡 | Em discussão (Brainstorm) |
| 🔵 | Em andamento |
| ✅ | Concluído |
| 🔴 | Bloqueado |

---

## Fase 1 — Planejamento e Definições

| Código | Ação | Status | Doc Brainstorm | Doc Organized | Data Início | Data Fim |
|--------|------|--------|----------------|---------------|-------------|----------|
| DEF-001 | Definir primeiro tipo de jogo | ✅ | [Discussion_01](../Brainstorm/Discussion_01.md) | [01_Game_Definition](01_Game_Definition.md) | 2026-02-15 | 2026-02-15 |
| DOC-001 | Documentar definição do jogo | ✅ | — | [01_Game_Definition](01_Game_Definition.md) | 2026-02-15 | 2026-02-15 |
| DEF-002 | Definir escopo do MVP | ✅ | [Discussion_02](../Brainstorm/Discussion_02.md) | [02_MVP_Scope](02_MVP_Scope.md) | 2026-02-15 | 2026-02-15 |
| DOC-002 | Documentar escopo do MVP | ✅ | — | [02_MVP_Scope](02_MVP_Scope.md) | 2026-02-15 | 2026-02-15 |
| DEF-003 | Definir engine base / arquitetura | ✅ | [Discussion_03](../Brainstorm/Discussion_03.md) | [03_Architecture](03_Architecture.md) | 2026-02-15 | 2026-02-15 |
| DOC-003 | Documentar arquitetura | ✅ | — | [03_Architecture](03_Architecture.md) | 2026-02-15 | 2026-02-15 |
| DEF-004 | Definir arquitetura de pastas Unity | ✅ | Coberto em [Discussion_03](../Brainstorm/Discussion_03.md) | [03_Architecture](03_Architecture.md) | 2026-02-15 | 2026-02-15 |
| DEF-005 | Definir pipeline técnico | ✅ | [Discussion_04](../Brainstorm/Discussion_04.md) | [04_Pipeline](04_Pipeline.md) | 2026-02-15 | 2026-02-15 |
| DOC-004 | Documentar pipeline técnico | ✅ | — | [04_Pipeline](04_Pipeline.md) | 2026-02-15 | 2026-02-15 |
| DEF-006 | Definir naming / branding | ✅ | [Discussion_05](../Brainstorm/Discussion_05.md) | [05_Naming](05_Naming.md) | 2026-02-15 | 2026-02-15 |
| DOC-005 | Documentar naming / branding | ✅ | — | [05_Naming](05_Naming.md) | 2026-02-15 | 2026-02-15 |
| DEF-007 | Definir estrutura de dados | ✅ | Coberto em [Discussion_02](../Brainstorm/Discussion_02.md) | [02_MVP_Scope](02_MVP_Scope.md) | 2026-02-15 | 2026-02-15 |
| DEF-008 | Definir modelo de níveis | ✅ | [Discussion_05](../Brainstorm/Discussion_05.md) | [06_Level_Model](06_Level_Model.md) | 2026-02-15 | 2026-02-15 |
| DOC-006 | Documentar modelo de níveis | ✅ | — | [06_Level_Model](06_Level_Model.md) | 2026-02-15 | 2026-02-15 |
| DEF-009 | Definir sistema de monetização | ✅ | Coberto em [Discussion_02](../Brainstorm/Discussion_02.md) | [02_MVP_Scope](02_MVP_Scope.md) | 2026-02-15 | 2026-02-15 |
| DEF-010 | Definir sistema de analytics | ✅ | [Discussion_05](../Brainstorm/Discussion_05.md) | [07_Analytics](07_Analytics.md) | 2026-02-15 | 2026-02-15 |
| DOC-007 | Documentar sistema de analytics | ✅ | — | [07_Analytics](07_Analytics.md) | 2026-02-15 | 2026-02-15 |

---

## Fase 2 — Desenvolvimento

> 🟡 Próxima fase. A ser detalhada.

| Código | Ação | Status | Doc Brainstorm | Doc Organized | Data Início | Data Fim |
|--------|------|--------|----------------|---------------|-------------|----------|
| CFG-001 | Setup projeto Unity | ✅ | — | — | 2026-02-15 | 2026-02-15 |
| CFG-002 | Configurar Git (init, .gitignore, .gitattributes) | ✅ | — | — | 2026-02-15 | 2026-02-15 |
| CFG-003 | Criar repositório GitHub | ✅ | — | — | 2026-02-15 | 2026-02-15 |
| CFG-004 | Configurar VS Code para Unity | ✅ | — | — | 2026-02-15 | 2026-02-15 |
| DEV-001 | Implementar Domain/Grid | ✅ | — | — | 2026-02-15 | 2026-02-15 |
| DEV-002 | Implementar Domain/Words | ✅ | — | — | 2026-02-15 | 2026-02-15 |
| DEV-003 | Implementar Domain/Level | ✅ | — | — | 2026-02-15 | 2026-02-15 |
| DEV-004 | Implementar Application layer | ✅ | — | — | 2026-02-15 | 2026-02-15 |
| DEV-005 | Implementar Infrastructure layer | ✅ | — | — | 2026-02-15 | 2026-02-15 |
| DEV-006 | Implementar UI/Screens (Boot, Menu, Category, Level) | ✅ | — | — | 2026-02-15 | 2026-02-15 |
| DEV-007 | Implementar UI/Components (Grid, LetterCell, Selection, WordList, Popups) | ✅ | — | — | 2026-02-15 | 2026-02-15 |
| DAT-001 | Gerar banco de palavras | ✅ | — | — | 2026-02-15 | 2026-02-15 |
| DSN-001 | Criar assets visuais MVP | ✅ | — | [08_Design_Specs](08_Design_Specs.md) | 2026-02-15 | 2026-02-15 |
| TST-001 | Testes unitários Domain | ⬜ | — | — | — | — |
| CFG-002 | Integrar AdMob | ⬜ | — | — | — | — |
| CFG-003 | Integrar Unity Analytics | ⬜ | — | — | — | — |
| BLD-001 | Primeiro build Android (APK) | ⬜ | — | — | — | — |
| TST-002 | Teste no device real | ⬜ | — | — | — | — |
| BLD-002 | Publicar na Play Store | ⬜ | — | — | — | — |

---

## Histórico de Decisões

| Data | Código | Decisão | Contexto |
|------|--------|---------|----------|
| 2026-02-15 | — | Início do planejamento formal | PRD revisado, metodologia Brainstorm → Organized definida |
| 2026-02-15 | — | Sistema de codificação de ações definido | Prefixos de 3 letras (DEF, DOC, DEV, CFG, TST, DSN, DAT, BLD) |
| 2026-02-15 | DEF-001 | Primeiro jogo: Caça-Palavras | Tema genérico, MVP mínimo, PT-BR, arquitetura multi-idioma |
| 2026-02-15 | DOC-001 | Documento 01_Game_Definition criado | Formalização da DEF-001 |
| 2026-02-15 | DEF-002 | Escopo do MVP definido | Grid 3 tamanhos, 8 categorias, ads no MVP, sem IAP |
| 2026-02-15 | DOC-002 | Documento 02_MVP_Scope criado | Formalização da DEF-002 |
| 2026-02-15 | DEF-002 | Dados: JSON embarcado + geração runtime | Atualização da DEF-002 com decisão de armazenamento |
| 2026-02-15 | DEF-003 | Arquitetura: Clean Architecture 5 camadas | Core/ vs Game/, C# Events, Interfaces, State Machine |
| 2026-02-15 | DOC-003 | Documento 03_Architecture criado | Formalização da DEF-003 |
| 2026-02-15 | DEF-004 | Pastas Unity definidas | Coberto integralmente na DEF-003 |
| 2026-02-15 | DEF-007 | Estrutura de dados definida | Coberto na DEF-002 (JSON embarcado) |
| 2026-02-15 | DEF-009 | Monetização definida | Coberto na DEF-002 (interstitial + rewarded) |
| 2026-02-15 | DEF-005 | Pipeline técnico definido | Unity 6.3 LTS, Git Flow, build manual, Python para dados |
| 2026-02-15 | DOC-004 | Documento 04_Pipeline criado | Formalização da DEF-005 |
| 2026-02-15 | DEF-006 | Naming definido | Ragazzi Studios, com.ragazzistudios.wordsearch, "Caça-Palavras" |
| 2026-02-15 | DEF-008 | Modelo de níveis definido | 15 níveis/cat, 120 total, seed determinístico, 3 faixas dificuldade |
| 2026-02-15 | DEF-010 | Analytics definido | Unity Analytics, 9 eventos essenciais |
| 2026-02-15 | — | Modelo de portfólio: apps independentes | 1 jogo = 1 app, deploy separado, cross-promotion |
| 2026-02-15 | — | **FASE 1 CONCLUÍDA** | Todas as definições aprovadas, documentos criados |
| 2026-02-15 | DOC-008 | ActionPlan.md criado | Plano de ações detalhado para Fase 2 (49 ações) |
| 2026-02-15 | CFG-001 | Setup Unity concluído | Projeto criado, pastas, ProjectSettings configurado |
| 2026-02-15 | CFG-002 | Git configurado | git init, .gitignore, .gitattributes, README, CHANGELOG, commit inicial |
| 2026-02-15 | CFG-003 | Repositório GitHub criado | github.com/daniloragazzi/WordSearch, push main+develop |
| 2026-02-15 | CFG-004 | VS Code configurado | .vscode/settings.json, extensions.json, .editorconfig, extensões instaladas |

---

## Notas

- Cada item passa por: **Brainstorm** (discussão) → **Organized** (documentação formal)
- Documentos de brainstorm ficam em `docs/Brainstorm/`
- Documentos organizados ficam em `docs/Organized/`
- Este tracker é atualizado a cada mudança de status
