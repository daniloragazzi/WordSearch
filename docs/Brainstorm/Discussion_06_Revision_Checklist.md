# Discussion 06 — Checklist de Revisão (Projeto + UX/Layout)

> **Códigos:** REV-001..003, ARQ-001..003, UX-006..009, TST-007, DOC-009  
> **Status:** 🟡 Em discussão  
> **Data:** 2026-02-17  
> **Objetivo:** Definir checklist oficial para revisão de decisões de projeto, usabilidade e layout visual.

---

## Princípio

> Revisar primeiro, executar depois.
> A revisão deve gerar decisões rastreáveis com vínculo explícito entre Brainstorm → ActionPlan → Execution_Plan.

---

## 1) Governança da Revisão

| Código | Item | Critério de aceite |
|--------|------|--------------------|
| REV-001 | Consolidar critérios da revisão | Lista final de critérios aprovada e registrada |
| REV-002 | Mapear decisões para manter/ajustar/remover | Cada decisão com justificativa e impacto |
| REV-003 | Criar backlog priorizado P0/P1/P2 | Backlog ordenado por impacto x esforço |

### Checklist
- [ ] Critérios de revisão aprovados
- [ ] Matriz de decisões produzida
- [ ] Priorização consolidada

---

## 2) Projeto e Arquitetura

| Código | Item | Pergunta-chave |
|--------|------|----------------|
| ARQ-001 | Revisar separação Core/Game e state machine | A arquitetura atual ainda sustenta escala de portfólio? |
| ARQ-002 | Revisar serviços mock/real | A troca mock→real está segura para produção? |
| ARQ-003 | Revisar modo desafio no funil MVP | O desafio está isolado sem distorcer métricas do MVP? |

### Checklist
- [ ] Fluxo de estados validado (MainMenu, Category, Level, Challenge, Playing, Win, Pause)
- [ ] Contratos de serviço revisados (Ads, Analytics, Storage, Localization)
- [ ] Regras de desafio e impacto analítico documentados

---

## 3) Usabilidade e Layout Visual

| Código | Item | Pergunta-chave |
|--------|------|----------------|
| UX-006 | Revisão heurística do fluxo completo | O usuário conclui o ciclo principal sem fricção? |
| UX-007 | Auditoria de contraste e tipografia | A leitura está clara em todos os estados e telas? |
| UX-008 | Validação em múltiplas resoluções | O layout mantém hierarquia e toque confortável? |
| UX-009 | Consistência visual (tema vs hardcoded) | O visual depende majoritariamente de tokens de tema? |

### Checklist
- [ ] Menu → Categoria → Nível → Jogo → Vitória validado
- [ ] Contraste e legibilidade aprovados
- [ ] Responsividade validada em resoluções-alvo Android
- [ ] Divergências tema/hardcoded listadas com ação corretiva

---

## 4) Teste e Fechamento

| Código | Item | Critério de aceite |
|--------|------|--------------------|
| TST-007 | Regressão funcional pós-ajustes | Fluxos críticos sem regressão |
| DOC-009 | Relatório consolidado da revisão | Documento final com decisões, backlog e evidências |

### Checklist
- [ ] Regressão executada e registrada
- [ ] Evidências anexadas (prints, logs, commits)
- [ ] Relatório final publicado

---

## Rastreabilidade

| Origem | Destino |
|-------|---------|
| Brainstorm (`Discussion_06`) | `ActionPlan.md` (Fase 4) |
| `ActionPlan.md` (Fase 4) | `Execution_Plan.md` (execução operacional) |
| `Execution_Plan.md` | Evidências objetivas (arquivo/commit/teste/build) |

---

## Próximos Passos

- [ ] Aprovar checklist da revisão
- [ ] Mover ações para acompanhamento em `docs/Organized/Execution_Tracker.md`
- [ ] Iniciar `REV-001`

---

## REV-001 — Critérios Iniciais (v0.1)

> Critérios consolidados para iniciar a revisão estruturada.
> Status: rascunho operacional (base para REV-002).

### Critérios por dimensão

| Dimensão | Critério | Indicador de validação |
|---------|----------|------------------------|
| Arquitetura | Separação `Core/` vs `Game/` preservada | Nenhum acoplamento indevido identificado |
| Arquitetura | Fluxo da state machine coerente com UX | Transições válidas sem estado órfão |
| Serviços | Troca mock/real segura para produção | Checklist de ativação concluído sem regressão |
| Produto | Funil MVP protegido de efeitos colaterais | Métricas principais separadas do modo desafio |
| Usabilidade | Fluxo principal concluído sem fricção alta | Jornada Menu→Vitória sem bloqueios críticos |
| Layout visual | Contraste e legibilidade consistentes | Textos críticos legíveis em telas-alvo |
| Responsividade | Layout estável em múltiplas resoluções | Sem overlap/corte em resoluções definidas |
| Consistência visual | Tema centralizado prevalece | Redução de cores hardcoded fora do tema |

### Saídas esperadas de REV-001

- Matriz de critérios aprovados para execução da revisão.
- Base objetiva para classificação manter/ajustar/remover (REV-002).
- Insumos para priorização P0/P1/P2 (REV-003).
