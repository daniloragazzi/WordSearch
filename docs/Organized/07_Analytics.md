# 07 — Sistema de Analytics

> **Código:** DEF-010 / DOC-007
> **Status:** ✅ Concluído
> **Data:** 2026-02-15
> **Origem:** [Discussion_05](../Brainstorm/Discussion_05.md)

---

## Ferramenta

**Unity Analytics** (gratuito, integrado)

---

## Eventos

| Evento | Quando | Dados | Objetivo |
|--------|--------|-------|----------|
| `game_start` | App aberto | — | MAU, DAU |
| `level_start` | Início de nível | category, level, difficulty | Engajamento |
| `level_complete` | Nível concluído | category, level, time_seconds, hints_used | Retenção, dificuldade |
| `level_quit` | Saiu sem completar | category, level, time_seconds, words_found | Abandono |
| `hint_used` | Usou dica (rewarded ad) | category, level | Monetização |
| `ad_shown` | Ad exibido | ad_type | Revenue |
| `ad_clicked` | Ad clicado | ad_type | CTR |
| `category_selected` | Escolheu categoria | category | Preferência |
| `session_end` | Fechou app | session_duration | Sessão |

---

## Métricas Derivadas

| Métrica | Fórmula |
|---------|---------|
| DAU / MAU | Contagem de `game_start` únicos |
| Retenção D1/D7 | % de retorno |
| Sessão média | Média de `session_duration` |
| Nível mais jogado | Contagem de `level_start` |
| Nível mais abandonado | Contagem de `level_quit` |
| Taxa de dica | `hint_used` / `level_start` |
| Taxa de ad | `ad_shown` / sessão |

---

## Fora do MVP

- Dados pessoais
- Localização
- A/B testing
- Heatmaps
- Funnel de compra (sem IAP)
