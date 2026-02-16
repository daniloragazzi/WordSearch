# 06 — Modelo de Níveis

> **Código:** DEF-008 / DOC-006
> **Status:** ✅ Concluído
> **Data:** 2026-02-15
> **Origem:** [Discussion_05](../Brainstorm/Discussion_05.md)

---

## Estrutura de Conteúdo

| Dado | Valor |
|------|-------|
| Categorias | 8 |
| Níveis por categoria | 15 |
| **Total de níveis MVP** | **120** |
| Palavras por categoria | ~50 mínimo |
| **Total de palavras** | **~400+** |

---

## Progressão de Dificuldade

| Níveis | Grid | Palavras | Dificuldade |
|--------|------|----------|-------------|
| 1-5 | 8x8 | 5-6 | Fácil |
| 6-10 | 10x10 | 6-8 | Médio |
| 11-15 | 12x12 | 8-10 | Difícil |

---

## Algoritmo de Geração

```
GenerateLevel(categoryId, levelNumber):
    1. seed = Hash(categoryId + levelNumber)
    2. random = new SeededRandom(seed)
    3. config = GetDifficultyConfig(levelNumber)
    4. words = LoadWords(categoryId)
    5. selectedWords = random.Pick(words, config.wordCount)
    6. grid = new Grid(config.gridSize)
    7. for each word in selectedWords:
         direction = random.Pick([Horizontal, Vertical, Diagonal])
         position = FindValidPosition(grid, word, direction)
         PlaceWord(grid, word, position, direction)
    8. FillEmptyCells(grid, random)
    9. return Level(grid, selectedWords)
```

### Seed Determinístico

- Mesmo nível = mesmo grid, sempre
- Não precisa salvar o grid gerado
- Debugável e reprodutível
- Justo para todos os jogadores

---

## Regras de Palavras

| Regra | Detalhe |
|-------|---------|
| Tamanho mínimo | 3 letras |
| Tamanho máximo | gridSize - 1 |
| Repetição | Sem repetição dentro do mesmo nível |
| **Grid** | **Sem acentos** (CORACAO) |
| **Lista** | **Com acentos** (CORAÇÃO) |
| Distribuição | Mix de palavras curtas e longas |

---

## Desbloqueio e Progresso

| Aspecto | Regra |
|---------|-------|
| Categorias | Todas desbloqueadas desde o início |
| Níveis | Completar N desbloqueia N+1 |
| Replay | Pode repetir qualquer nível completo |
| Storage | PlayerPrefs: `{categoryId}_level_{n} = completed` |

---

## Categorias MVP

| # | Categoria | ID |
|---|-----------|-----|
| 1 | Animais | `animais` |
| 2 | Alimentos | `alimentos` |
| 3 | Corpo Humano | `corpo_humano` |
| 4 | Natureza | `natureza` |
| 5 | Profissões | `profissoes` |
| 6 | Países | `paises` |
| 7 | Esportes | `esportes` |
| 8 | Cores e Formas | `cores_formas` |
