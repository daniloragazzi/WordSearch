# Discussion 01 — Definir o Primeiro Tipo de Jogo

> **Status:** 🟡 Em discussão
> **Data:** 2026-02-15
> **Objetivo:** Escolher qual será o primeiro jogo a ser desenvolvido como MVP do estúdio.

---

## Contexto

O PRD lista os seguintes tipos de jogos possíveis:

- Caça-palavras
- Termo-like (Wordle clone)
- Quiz temático
- Puzzle lógico
- Jogos educativos
- Jogos infantis
- Jogos para público sênior
- Jogos offline
- Jogos temáticos (bíblico, concursos, técnico, idiomas)

Precisamos escolher **um** para ser o primeiro MVP, considerando:

---

## Critérios de Decisão

| Critério | Peso | Descrição |
|----------|------|-----------|
| Simplicidade técnica | Alto | Menor complexidade para o primeiro jogo |
| Reaproveitamento futuro | Alto | Componentes reutilizáveis para outros jogos |
| Apelo de mercado | Médio | Popularidade e demanda na Play Store |
| Potencial de monetização | Médio | Facilidade de inserir ads e IAP |
| Riqueza de conteúdo | Médio | Facilidade de gerar dados/níveis em escala |
| Diferenciação | Baixo | Espaço para inovar (menos relevante no MVP) |

---

## Análise dos Candidatos Principais

### 1. 🔤 Caça-Palavras (Word Search)
| Critério | Nota |
|----------|------|
| Simplicidade técnica | ⭐⭐⭐⭐⭐ |
| Reaproveitamento | ⭐⭐⭐⭐ |
| Apelo de mercado | ⭐⭐⭐⭐⭐ |
| Monetização | ⭐⭐⭐⭐ |
| Riqueza de conteúdo | ⭐⭐⭐⭐⭐ |

**Prós:** Grid simples, lógica direta, geração automática de níveis fácil, enorme apelo casual, funciona offline, público amplo (crianças a idosos), temas infinitos.

**Contras:** Mercado saturado, diferenciação difícil.

---

### 2. 🟩 Termo-like (Wordle Clone)
| Critério | Nota |
|----------|------|
| Simplicidade técnica | ⭐⭐⭐⭐ |
| Reaproveitamento | ⭐⭐⭐ |
| Apelo de mercado | ⭐⭐⭐⭐ |
| Monetização | ⭐⭐⭐ |
| Riqueza de conteúdo | ⭐⭐⭐ |

**Prós:** Mecânica viral comprovada, sessões curtas, boa retenção diária.

**Contras:** Mecânica mais restrita (1 palavra/dia), menor geração de conteúdo, menos temas possíveis.

---

### 3. ❓ Quiz Temático
| Critério | Nota |
|----------|------|
| Simplicidade técnica | ⭐⭐⭐⭐ |
| Reaproveitamento | ⭐⭐⭐⭐⭐ |
| Apelo de mercado | ⭐⭐⭐⭐ |
| Monetização | ⭐⭐⭐⭐ |
| Riqueza de conteúdo | ⭐⭐⭐⭐⭐ |

**Prós:** Engine super reutilizável, temas infinitos, conteúdo gerado por IA facilmente.

**Contras:** Não é exatamente um "word game", pode dispersar o foco inicial.

---

## Recomendação

**🔤 Caça-Palavras** parece a escolha mais forte para o MVP porque:

1. **Técnicamente simples** — grid 2D, lógica de busca de palavras
2. **Altamente escalável** — geração automática de níveis com banco de palavras
3. **Público massivo** — um dos gêneros mais populares na Play Store
4. **Componentes reutilizáveis** — grid system, word database, tema system servem para outros jogos
5. **Temas infinitos** — permite criar variantes temáticas facilmente (bíblico, idiomas, etc.)
6. **Funciona offline** — ótimo para mercado brasileiro
7. **Ideal para aprender** — complexidade técnica gerenciável para primeiro projeto Unity

---

## Perguntas para Decidir

1. Concorda com Caça-Palavras como primeiro jogo?
2. Tem preferência por algum tema inicial? (genérico, bíblico, educativo?)
3. Quer incluir alguma mecânica diferencial no MVP ou manter mínimo?
4. Idioma inicial: apenas português ou multilíngue desde o início?

---

## Decisão

> ✅ **Decidido em 2026-02-15**

| Item | Decisão |
|------|---------|
| **Primeiro jogo** | Caça-Palavras (Word Search) |
| **Tema inicial** | Genérico — o mais abrangente possível para maximizar reaproveitamento da base |
| **Escopo MVP** | Mínimo viável — funcionalidades essenciais apenas |
| **Idioma inicial** | Português (BR) |
| **Multi-idioma** | Não no MVP, mas arquitetura preparada para suportar desde o início |

### Justificativa
- Maior simplicidade técnica para primeiro projeto
- Alta escalabilidade de conteúdo (geração automática de níveis)
- Público massivo e amplo
- Componentes altamente reutilizáveis para jogos futuros
- Tema genérico permite a maior base de palavras possível
- Preparar para multi-idioma desde a arquitetura evita refactoring futuro

---

## Próximos Passos

- [x] Criar documento organizado: `Organized/01_Game_Definition.md` (DOC-001)
- [ ] Avançar para Discussion_02: Escopo do MVP (DEF-002)
- [x] Atualizar Execution_Tracker
