# Discussion 05 — Naming, Modelo de Níveis e Analytics

> **Códigos:** DEF-006, DEF-008, DEF-010
> **Status:** 🟡 Em discussão
> **Data:** 2026-02-15
> **Objetivo:** Fechar as 3 últimas definições da Fase 1 em uma única discussão.

---

## PARTE 1 — DEF-006: Naming / Branding

### Pesquisa de Existência

| Pesquisa | Resultado |
|----------|----------|
| Play Store "Ragazzi Studios" | ❌ Nenhum desenvolvedor encontrado com esse nome |
| Play Store "Ragazzi" | Apenas "Ragazzi: Delivery" (app de comida, Netclues Inc.) — sem relação |
| Google "Ragazzi Studios" game | ❌ Sem resultados relevantes |

> **Conclusão:** Nome "Ragazzi Studios" aparenta estar **disponível** no contexto de games/Play Store.
> ⚠️ **Recomendação:** Validar disponibilidade do domínio e conta de desenvolvedor Google Play antes do lançamento. Para o MVP, podemos avançar.

### Proposta de Naming

| Item | Proposta | Justificativa |
|------|----------|---------------|
| **Nome do estúdio** | Ragazzi Studios | Conforme definido pelo owner |
| **Package name (Android)** | `com.ragazzistudios.*` | Domínio reverso, padrão Android |
| **Package do primeiro jogo** | `com.ragazzistudios.wordsearch` | Claro e direto |
| **Nome do app (Play Store)** | "Caça-Palavras" (provisório) | Simples, pesquisável, SEO-friendly em PT-BR |
| **Nome interno do projeto** | WordSearch | Usado no código e pastas |
| **Company Name (Unity)** | RagazziStudios | Sem espaço, PascalCase |
| **Conta Google Play** | Ragazzi Studios | Display name na loja |

### Nome do App — Opções

Para o nome na Play Store, precisa ser atrativo e pesquisável:

| Opção | Nome | Prós | Contras |
|-------|------|------|---------|
| A | Caça-Palavras | Direto, SEO forte | Genérico |
| B | Caça-Palavras: Desafio | Um pouco mais atrativo | Mais longo |
| C | Palavra Escondida | Diferente do padrão | Menos pesquisável |
| D | Caça-Palavras — Ragazzi | Vincula ao estúdio | Longo demais |

**Recomendação:** Opção **A** — "Caça-Palavras" como nome principal. Simples, direto, máximo SEO. O nome do estúdio aparece separado como developer name na Play Store.

### Branding Mínimo (MVP)

| Item | MVP | Detalhe |
|------|-----|---------|
| Logo do estúdio | Placeholder simples | Texto estilizado "Ragazzi Studios" |
| Ícone do app | Necessário | Grid de letras estilizado (pode ser simples) |
| Paleta de cores | Definir 3-4 cores | Tons quentes e amigáveis |
| Fonte | 1 fonte principal | Legível, casual, gratuita (Google Fonts) |

> Design elaborado fica para depois. MVP = funcional e limpo.

---

## PARTE 2 — DEF-008: Modelo de Níveis

### Como os níveis funcionam

```
Categoria (ex: Animais)
  └── Nível 1  → seed(1)  → grid 8x8,   5 palavras
  └── Nível 2  → seed(2)  → grid 8x8,   5 palavras
  └── ...
  └── Nível 5  → seed(5)  → grid 8x8,   6 palavras
  └── Nível 6  → seed(6)  → grid 10x10, 6 palavras
  └── ...
  └── Nível 11 → seed(11) → grid 10x10, 8 palavras
  └── Nível 12 → seed(12) → grid 12x12, 8 palavras
  └── ...
  └── Nível 15 → seed(15) → grid 12x12, 10 palavras
```

### Regras de Progressão de Dificuldade

| Níveis | Grid | Palavras | Dificuldade |
|--------|------|----------|-------------|
| 1-5 | 8x8 | 5-6 | Fácil |
| 6-10 | 10x10 | 6-8 | Médio |
| 11-15 | 12x12 | 8-10 | Difícil |

### Algoritmo de Geração (conceito)

```
function GenerateLevel(categoryId, levelNumber):
    1. seed = Hash(categoryId + levelNumber)
    2. random = new SeededRandom(seed)
    3. config = GetDifficultyConfig(levelNumber)  // grid size, word count
    4. words = LoadWords(categoryId)
    5. selectedWords = random.Pick(words, config.wordCount)
    6. grid = new Grid(config.gridSize)
    7. for each word in selectedWords:
         direction = random.Pick([Horizontal, Vertical, Diagonal])
         position = FindValidPosition(grid, word, direction)
         PlaceWord(grid, word, position, direction)
    8. FillEmptyCells(grid, random)  // letras aleatórias
    9. return Level(grid, selectedWords)
```

### Seed Determinístico — Por quê?

| Benefício | Explicação |
|-----------|-----------|
| **Reprodutível** | Mesmo nível = mesmo grid, sempre |
| **Sem storage** | Não precisa salvar o grid gerado |
| **Debugável** | Fácil testar nível específico |
| **Justo** | Todos os jogadores veem o mesmo nível |

### Seleção de Palavras

| Regra | Detalhe |
|-------|---------|
| Tamanho mínimo | 3 letras |
| Tamanho máximo | Limitado pelo grid (gridSize - 1) |
| Sem repetição | Dentro do mesmo nível |
| Sem acentos no grid | Grid usa letras sem acento |
| Lista mostra com acento | "CORAÇÃO" na lista, "CORACAO" no grid |
| Distribuição de tamanho | Mix de palavras curtas e longas |

### Desbloqueio e Progresso

| Aspecto | Regra |
|---------|-------|
| Desbloqueio | Completar nível N desbloqueia N+1 |
| Categorias | Todas desbloqueadas desde o início |
| Replay | Pode repetir qualquer nível completo |
| Progresso salvo | PlayerPrefs: `{categoryId}_level_{n} = completed` |

### Total de conteúdo MVP

| Dado | Valor |
|------|-------|
| Categorias | 8 |
| Níveis por categoria | 15 |
| **Total de níveis** | **120** |
| Palavras necessárias por categoria | ~50 mínimo (para variedade) |
| **Total de palavras necessárias** | **~400+** |

---

## PARTE 3 — DEF-010: Sistema de Analytics

### Proposta: Unity Analytics (gratuito)

Mínimo de eventos para validar o MVP:

### Eventos a Rastrear

| Evento | Quando | Dados | Por quê |
|--------|--------|-------|---------|
| `game_start` | App aberto | — | MAU, DAU |
| `level_start` | Início de nível | category, level, difficulty | Engajamento |
| `level_complete` | Nível concluído | category, level, time_seconds, hints_used | Retenção, dificuldade |
| `level_quit` | Saiu sem completar | category, level, time_seconds, words_found | Abandono, frustração |
| `hint_used` | Usou dica (rewarded ad) | category, level | Monetização |
| `ad_shown` | Ad exibido | ad_type (interstitial/rewarded) | Revenue tracking |
| `ad_clicked` | Ad clicado | ad_type | CTR |
| `category_selected` | Escolheu categoria | category | Preferência |
| `session_end` | Fechou app | session_duration | Sessão |

### O que NÃO rastrear no MVP
- Dados pessoais
- Localização
- Funnel de compra (sem IAP)
- A/B testing (prematuro)
- Heatmaps de toque (complexo)

### Dashboard mínimo (Unity Analytics)

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

## Resumo das 3 Definições

| DEF | Decisão proposta |
|-----|-----------------|
| DEF-006 | Estúdio: Ragazzi Studios / Package: com.ragazzistudios.wordsearch / App: "Caça-Palavras" |
| DEF-008 | 15 níveis/categoria, 3 faixas de dificuldade, seed determinístico, 120 níveis total |
| DEF-010 | Unity Analytics, 9 eventos essenciais, foco em retenção e monetização |

---

## Perguntas para Decisão

### DEF-006
1. "Caça-Palavras" como nome do app — ok?
2. `com.ragazzistudios.wordsearch` como package — ok?
3. Branding mínimo (placeholder) no MVP — ok?

### DEF-008
1. 15 níveis por categoria (120 total) — ok?
2. Progressão 8x8 → 10x10 → 12x12 — ok?
3. Todas as categorias desbloqueadas desde o início — ok?
4. Grid sem acentos, lista com acentos — ok?

### DEF-010
1. Unity Analytics — ok?
2. Os 9 eventos listados cobrem o necessário?
3. Algo a mais para rastrear?

---

## Decisão

> ✅ **Decidido em 2026-02-15**

### DEF-006 — Naming / Branding

| Item | Decisão |
|------|---------|
| Estúdio | Ragazzi Studios |
| Package base | com.ragazzistudios.* |
| Package primeiro jogo | com.ragazzistudios.wordsearch |
| App Name (Play Store) | Caça-Palavras |
| Company Name (Unity) | RagazziStudios |
| Nome interno | WordSearch |
| Branding MVP | Placeholder mínimo |

### DEF-008 — Modelo de Níveis

| Item | Decisão |
|------|---------|
| Níveis por categoria | 15 |
| Total de níveis | 120 (8 categorias × 15) |
| Progressão | 8x8 (fácil) → 10x10 (médio) → 12x12 (difícil) |
| Geração | Runtime com seed determinístico |
| Categorias | Todas desbloqueadas desde o início |
| Acentos | Grid sem acento, lista com acento |

### DEF-010 — Analytics

| Item | Decisão |
|------|---------|
| Ferramenta | Unity Analytics |
| Eventos | 9 eventos essenciais |
| Foco | Retenção e monetização |

### Decisão Adicional — Modelo de Portfólio

| Item | Decisão |
|------|---------|
| Modelo | **1 jogo = 1 app independente** na Play Store |
| Deploy | Cada jogo tem seu próprio build e deploy |
| Instalação | Cada jogo é instalado separadamente pelo usuário |
| Cross-promotion | Apps promovem uns aos outros (tráfego gratuito) |
| Engine | Core/ compartilhado, Game/ específico por jogo |

---

## Próximos Passos

- [x] Criar documentos organizados: `05_Naming.md`, `06_Level_Model.md`, `07_Analytics.md`
- [x] Registrar decisão de apps independentes na arquitetura
- [x] Atualizar Execution_Tracker
- [x] **Fase 1 — CONCLUÍDA** 🎉
