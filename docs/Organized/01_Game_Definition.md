# 01 — Definição do Jogo

> **Código:** DEF-001 / DOC-001
> **Status:** ✅ Concluído
> **Data:** 2026-02-15
> **Origem:** [Discussion_01](../Brainstorm/Discussion_01.md)

---

## Jogo Escolhido

**Caça-Palavras (Word Search)**

---

## Definições Fundamentais

| Aspecto | Definição |
|---------|-----------|
| Tipo | Caça-Palavras (Word Search) |
| Plataforma | Android |
| Engine | Unity |
| Tema | Genérico (máxima abrangência) |
| Idioma MVP | Português (BR) |
| Multi-idioma | Não no MVP, mas arquitetura preparada |
| Escopo | MVP mínimo |

---

## Conceito do Jogo

O jogador recebe um **grid de letras** e uma **lista de palavras** para encontrar. As palavras podem estar posicionadas em diferentes direções dentro do grid. O jogador seleciona as letras arrastando o dedo para marcar as palavras encontradas.

---

## Características Essenciais (MVP)

- Grid de letras com tamanho variável
- Lista de palavras a encontrar
- Palavras em múltiplas direções (horizontal, vertical, diagonal)
- Feedback visual ao encontrar palavra
- Progressão por níveis
- Categorias temáticas genéricas

---

## Decisões de Arquitetura

### Internacionalização (i18n)
- MVP apenas em PT-BR
- Toda string de UI externalizada desde o início
- Banco de palavras com estrutura preparada para múltiplos idiomas
- Sistema de localização na arquitetura base

### Tema Genérico
- Categorias amplas (animais, comida, profissões, países, cores, etc.)
- Banco de palavras o mais abrangente possível
- Estrutura que permita adicionar temas específicos futuramente

### Reaproveitamento
- Engine de grid reutilizável
- Sistema de banco de palavras modular
- Sistema de temas desacoplado
- Pipeline de geração de níveis automática

---

## Alinhamento com PRD

| Princípio PRD | Como se aplica |
|---------------|----------------|
| Engine base reutilizável | Grid system, word database, tema system |
| Data-driven | Níveis gerados a partir de banco de dados de palavras |
| Escala por repetição | Mesmo core, diferentes temas e dados |
| Produção em série | Template replicável para variantes temáticas |
| Automação | Geração automática de níveis |

---

## Próximos Passos

→ **DEF-002** — Definir escopo detalhado do MVP
