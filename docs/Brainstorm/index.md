# Brainstorm — Índice de Discussões

> Registro cronológico de todas as sessões de brainstorm e decisões do projeto WordGames.
> Cada discussão originou um ou mais documentos organizados em [`docs/Organized/`](../Organized/).

---

## Visão Geral

| ID | Arquivo | Data | App | Tema | Status |
|----|---------|------|-----|------|--------|
| — | [Wordgames.prd](Wordgames.prd) | 2026-02-15 | Todos | PRD — Visão geral do estúdio e ecossistema de jogos | ✅ Referência |
| DIS-001 | [Discussion_01.md](Discussion_01.md) | 2026-02-15 | Caça-Palavras | Definição do primeiro jogo | ✅ Concluído |
| DIS-002 | [Discussion_02.md](Discussion_02.md) | 2026-02-15 | Caça-Palavras | Escopo do MVP | ✅ Concluído |
| DIS-003 | [Discussion_03.md](Discussion_03.md) | 2026-02-15 | Caça-Palavras | Arquitetura técnica | ✅ Concluído |
| DIS-004 | [Discussion_04.md](Discussion_04.md) | 2026-02-15 | Caça-Palavras | Pipeline técnico | ✅ Concluído |
| DIS-005 | [Discussion_05.md](Discussion_05.md) | 2026-02-15 | Caça-Palavras | Naming, modelo de níveis e analytics | ✅ Concluído |
| DIS-006 | [Discussion_06_Revision_Checklist.md](Discussion_06_Revision_Checklist.md) | 2026-02-17 | Caça-Palavras | Checklist de revisão (Fase 4) | ✅ Concluído |
| DIS-007 | [Discussion_07_TermoLike.md](Discussion_07_TermoLike.md) | 2026-02-17 | Termo BR | Definição do segundo app (Wordle-like) | ✅ Concluído |
| DIS-008 | [Discussion_08_TermoLike_Escopo.md](Discussion_08_TermoLike_Escopo.md) | 2026-02-17 | Termo BR | Escopo detalhado do MVP | ✅ Concluído |
| DIS-009 | [Discussion_09_TermoLike_Arquitetura.md](Discussion_09_TermoLike_Arquitetura.md) | 2026-02-17 | Termo BR | Arquitetura técnica | ✅ Concluído |

---

## Detalhes por Discussão

### PRD — Wordgames.prd
- **Objetivo:** Documento fundacional do estúdio. Define a visão de longo prazo: ecossistema de jogos casuais Android com produção em série, monetização via anúncios/IAP e renda recorrente.
- **Conteúdo:** Visão geral, objetivos do projeto, princípios de design, estratégia de produção e roadmap de apps.
- **Papel:** Referência permanente — não é substituído por documentos organizados, serve como norte estratégico para todas as decisões.

---

### DIS-001 — Definição do Primeiro Jogo
- **Código:** DEF-001
- **Objetivo:** Escolher o primeiro jogo a ser desenvolvido como MVP do estúdio.
- **Decisão:** Caça-Palavras (Word Search) para Android, PT-BR, tema genérico, escopo mínimo.
- **Artefato gerado:** [`01_Game_Definition.md`](../Organized/01_Game_Definition.md)

### DIS-002 — Escopo do MVP
- **Código:** DEF-002
- **Objetivo:** Definir exatamente o que entra e o que fica fora do MVP do Caça-Palavras.
- **Decisões-chave:** Grids 8×8/10×10/12×12, 8 categorias, 5–10 palavras por nível, geração determinística por seed, AdMob (intersticial + rewarded), sem backend.
- **Artefato gerado:** [`02_MVP_Scope.md`](../Organized/02_MVP_Scope.md)

### DIS-003 — Arquitetura Técnica
- **Código:** DEF-003
- **Objetivo:** Definir arquitetura de código, patterns e estrutura de pastas Unity, com foco em reuso entre jogos.
- **Decisões-chave:** Clean Architecture adaptada para Unity, separação `Core/` (reutilizável) vs `Game/` (específico), interfaces de infraestrutura (`IAdsService`, `IStorageService`, `IAnalyticsService`, `ILocalizationService`), state machine de fluxo, 3 cenas.
- **Artefato gerado:** [`03_Architecture.md`](../Organized/03_Architecture.md)

### DIS-004 — Pipeline Técnico
- **Código:** DEF-005
- **Objetivo:** Definir pipeline de desenvolvimento, build e deploy.
- **Decisões-chave:** Unity 6.3 LTS, VS Code, Git Flow simplificado, Conventional Commits, build manual via Unity CLI, pipeline de dados via Python, versionamento `v0.x → v1.0`.
- **Artefato gerado:** [`04_Pipeline.md`](../Organized/04_Pipeline.md)

### DIS-005 — Naming, Modelo de Níveis e Analytics
- **Códigos:** DEF-006, DEF-008, DEF-010
- **Objetivo:** Fechar as 3 últimas definições da Fase 1 em uma única sessão.
- **Decisões-chave:** Nome "Caça-Palavras" (genérico/SEO), níveis lineares por categoria sem estrelas, eventos analytics com `category_id`, `level_number`, `game_mode`.
- **Artefatos gerados:** [`05_Naming.md`](../Organized/05_Naming.md), [`06_Level_Model.md`](../Organized/06_Level_Model.md), [`07_Analytics.md`](../Organized/07_Analytics.md)

### DIS-006 — Checklist de Revisão (Fase 4)
- **Códigos:** REV-001..003, ARQ-001..003, UX-006..009, TST-007, DOC-009
- **Objetivo:** Definir e executar checklist oficial de revisão de decisões de projeto, usabilidade e layout visual.
- **Decisões-chave:** Conformidade arquitetural validada (ARQ-001 ✅, ARQ-003 ✅, ARQ-002 🔴 bloqueado por SDK externos), responsividade validada em dispositivo real, migração de cores para tokens `GameTheme`.
- **Artefato gerado:** [`11_Review_Report.md`](../Organized/11_Review_Report.md)

### DIS-007 — Segundo App: Termo-Like
- **Objetivo:** Definir escopo, mecânica e diferenciação do segundo app — Wordle/Termo em português.
- **Decisões-chave:** Nome "Termo BR", 3 modos (1/2/4 palavras), QWERTY sem acentos, ilimitado (sem modo diário no MVP), compartilhamento de resultado em grade emoji, projeto Unity separado, Core compartilhado via `RagazziCore` package.
- **Artefato gerado:** seção App 2 em [`ActionPlan.md`](../Organized/ActionPlan.md)

### DIS-008 — Termo-Like: Escopo Detalhado do MVP
- **Objetivo:** Definir telas, fluxo, modos de jogo, banco de palavras e monetização do Termo BR.
- **Decisões-chave:** Splash → Menu → ModeSelect → Game, banco de palavras com apenas palavras de 5 letras sem acento, sem IAP no MVP, interstitial entre partidas.
- **Artefato gerado:** seção T1.3/T1.4/T1.5 em [`ActionPlan.md`](../Organized/ActionPlan.md)

### DIS-009 — Termo-Like: Arquitetura Técnica
- **Objetivo:** Definir estrutura do projeto Unity, estratégia de reaproveitamento do Core e decisões técnicas específicas do Termo-Like.
- **Decisões-chave:** `RagazziCore` extraído como UPM package local (`file:../../../RagazziCore`), novo `SceneCreator` específico, `BuildScript` dedicado, `DevConfig` para flags de desenvolvimento.
- **Artefatos gerados:** [`RagazziCore/`](../../RagazziCore/) (package), seção T1.9 em [`ActionPlan.md`](../Organized/ActionPlan.md)

---

## Rastreabilidade: Discussão → Documento Organizado

| Discussão | Documento(s) Organizado(s) |
|-----------|---------------------------|
| PRD | [Wordgames.prd](Wordgames.prd) — documento fundacional, não substituído |
| DIS-001 | [01_Game_Definition.md](../Organized/01_Game_Definition.md) |
| DIS-002 | [02_MVP_Scope.md](../Organized/02_MVP_Scope.md) |
| DIS-003 | [03_Architecture.md](../Organized/03_Architecture.md) |
| DIS-004 | [04_Pipeline.md](../Organized/04_Pipeline.md) |
| DIS-005 | [05_Naming.md](../Organized/05_Naming.md) · [06_Level_Model.md](../Organized/06_Level_Model.md) · [07_Analytics.md](../Organized/07_Analytics.md) |
| DIS-006 | [11_Review_Report.md](../Organized/11_Review_Report.md) |
| DIS-007, DIS-008, DIS-009 | [ActionPlan.md](../Organized/ActionPlan.md) (seção App 2) · [Execution_Tracker.md](../Organized/Execution_Tracker.md) (seção T1.x) |
