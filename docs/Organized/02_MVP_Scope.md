# 02 — Escopo do MVP

> **Código:** DEF-002 / DOC-002
> **Status:** ✅ Concluído
> **Data:** 2026-02-15
> **Origem:** [Discussion_02](../Brainstorm/Discussion_02.md)

---

## Visão do MVP

> A menor versão jogável, publicável e monetizável do Caça-Palavras.

O jogador abre o app → escolhe uma categoria → escolhe um nível → encontra palavras no grid arrastando o dedo → completa o nível → avança para o próximo. Pode assistir um ad para ganhar uma dica. A cada 3 níveis vê um interstitial.

---

## Mecânicas do Grid

| Aspecto | Definição |
|---------|-----------|
| Tamanhos | 8x8 (fácil), 10x10 (médio), 12x12 (difícil) |
| Direções | Horizontal ➡️, Vertical ⬇️, Diagonal ↘️ |
| Palavras invertidas | Não |
| Palavras por nível | 5 a 10 (proporcional ao grid) |
| Seleção | Arrastar dedo sobre letras |
| Preenchimento | Letras aleatórias nos espaços vazios |

---

## Progressão

| Aspecto | Definição |
|---------|-----------|
| Estrutura | Categorias → Níveis sequenciais |
| Dificuldade | Cresce com tamanho do grid + nº de palavras |
| Desbloqueio | Linear (nível 1 → 2 → 3...) |
| Níveis por categoria | 10-20 |
| Score / Estrelas | Não no MVP |

---

## Categorias

| # | Categoria | Exemplos |
|---|-----------|----------|
| 1 | Animais | gato, cachorro, elefante, baleia |
| 2 | Alimentos | arroz, banana, queijo, chocolate |
| 3 | Corpo Humano | cabeça, braço, coração, pulmão |
| 4 | Natureza | rio, montanha, floresta, oceano |
| 5 | Profissões | médico, professor, bombeiro, piloto |
| 6 | Países | brasil, japão, frança, canadá |
| 7 | Esportes | futebol, tênis, natação, vôlei |
| 8 | Cores e Formas | vermelho, círculo, triângulo, azul |

---

## Telas

```
[Splash] → [Menu Principal] → [Seleção de Categoria] → [Seleção de Nível] → [Jogo] → [Popup Vitória]
```

| Tela | Conteúdo |
|------|----------|
| Splash | Logo + loading |
| Menu Principal | Botão "Jogar" + Botão "Configurações" |
| Seleção de Categoria | Grid/lista de categorias com indicador de progresso |
| Seleção de Nível | Grid de níveis (bloqueado / desbloqueado / completo) |
| Tela de Jogo | Grid de letras + lista de palavras + botão dica |
| Popup Vitória | Mensagem de parabéns + botão próximo nível |

### Configurações
- Som on/off
- Música on/off
- Idioma (preparado, apenas PT-BR ativo)

---

## Monetização

| Tipo | Implementação | Frequência |
|------|--------------|------------|
| Interstitial | A cada 3 níveis completos | Moderada |
| Rewarded Ad | Botão "Dica" → assistir ad = revelar 1 palavra | Sob demanda do jogador |

| Tipo | Decisão |
|------|---------|
| Banner | ❌ Não (polui UX do grid) |
| IAP | ❌ Não no MVP (v1.1) |

---

## Stack Técnica

| Componente | Tecnologia |
|------------|-----------|
| Engine | Unity (2D) |
| Ads | Google AdMob |
| Analytics | Unity Analytics |
| Storage (progresso) | PlayerPrefs (local) |
| Storage (palavras) | JSON embarcado (Resources/) |
| Geração de níveis | Runtime com seed determinístico |
| Backend | Nenhum |
| Idioma | PT-BR (arquitetura i18n pronta) |

---

## Armazenamento de Dados

### Estrutura de arquivos

```
Resources/
  Data/
    categories.json          → lista de categorias
    words/
      animais.json           → palavras da categoria
      alimentos.json
      corpo_humano.json
      natureza.json
      profissoes.json
      paises.json
      esportes.json
      cores_formas.json
```

### Formato categories.json

```json
{
  "categories": [
    { "id": "animais", "name": { "pt-BR": "Animais" }, "icon": "cat_animais", "levelCount": 15 }
  ]
}
```

### Formato words (por categoria)

```json
{
  "categoryId": "animais",
  "words": ["gato", "cachorro", "elefante", "baleia", "tigre", ...]
}
```

### Geração de níveis

- Níveis **não são pré-montados** — gerados em **runtime**
- Algoritmo sorteia N palavras → posiciona no grid → preenche com letras aleatórias
- Usa **seed determinístico** baseado no número do nível (mesmo nível = mesmo grid sempre)
- JSONs gerados offline por scripts (Python/IA) durante o desenvolvimento

### Preparação multi-idioma

- Campo `name` com chave de idioma: `{ "pt-BR": "Animais", "en-US": "Animals" }`
- Arquivos de palavras por idioma futuramente: `animais_en.json`, `animais_es.json`

---

## Métricas de Sucesso

| Métrica | Target |
|---------|--------|
| Jogador completa 1 categoria | ✅ Sim |
| Sessão média | 3-5 minutos |
| Retenção D1 | > 30% |
| Crash rate | < 1% |
| Ad fill rate | > 80% |

---

## Explicitamente fora do MVP

| Feature | Versão planejada |
|---------|-----------------|
| Palavras invertidas | v1.1 |
| Score / Estrelas | v1.1 |
| Leaderboard | v1.2 |
| Conquistas | v1.1 |
| Multi-idioma jogável | v1.1 |
| IAP | v1.1 |
| Banner ads | Nunca |
| Modo diário | v1.2 |
| Tema escuro/claro | v1.1 |
| Tutorial interativo | v1.1 |
| Compartilhamento social | v1.2 |
| Perfil do jogador | v1.2 |
| Animações elaboradas | v1.1 |
| Efeitos sonoros variados | v1.1 |

---

## Próximos Passos

→ **DEF-003** — Definir engine base / arquitetura
