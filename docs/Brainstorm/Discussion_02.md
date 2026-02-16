# Discussion 02 — Definir Escopo do MVP

> **Código:** DEF-002
> **Status:** 🟡 Em discussão
> **Data:** 2026-02-15
> **Objetivo:** Definir exatamente o que entra e o que fica de fora do MVP do Caça-Palavras.

---

## Princípio Guia

> MVP = a menor versão jogável, publicável e monetizável.
> Tudo que não for essencial para o jogador completar uma partida e voltar amanhã, fica de fora.

---

## 1. Mecânicas do Grid

### Proposta

| Aspecto | MVP | Justificativa |
|---------|-----|---------------|
| Tamanhos de grid | 8x8, 10x10, 12x12 | 3 tamanhos cobre fácil/médio/difícil sem complexidade |
| Direções | Horizontal ➡️, Vertical ⬇️, Diagonal ↘️ | Diagonal é esperada pelo jogador, tirá-la empobrece demais |
| Palavras invertidas | Não | Adiciona complexidade sem valor percebido no MVP |
| Palavras por nível | 5 a 10 (proporcional ao grid) | Suficiente para sessão curta (2-5 min) |
| Seleção | Arrastar dedo sobre letras | Padrão do gênero, intuitivo |
| Letras aleatórias | Preenchimento automático dos espaços vazios | Essencial para o jogo funcionar |

### Por que não incluir palavras invertidas?
- Aumenta complexidade do algoritmo de geração
- Pode frustrar jogadores casuais
- Pode ser adicionado como "modo difícil" em versão futura

---

## 2. Progressão

### Proposta

| Aspecto | MVP | Justificativa |
|---------|-----|---------------|
| Estrutura | Categorias → Níveis sequenciais | Simples e familiar |
| Dificuldade | Cresce com tamanho do grid + nº de palavras | Natural e sem lógica extra |
| Desbloqueio | Linear (nível 1 → 2 → 3...) | Mínimo de lógica de progressão |
| Níveis por categoria | 10-20 níveis | Suficiente para validar retenção |
| Categorias MVP | 6-8 categorias genéricas | Conteúdo suficiente sem excesso |
| Estrelas/score | Não | Adiciona UI e lógica desnecessários no MVP |

### Categorias propostas (genéricas)

| # | Categoria | Exemplo de palavras |
|---|-----------|-------------------|
| 1 | Animais | gato, cachorro, elefante, baleia |
| 2 | Alimentos | arroz, banana, queijo, chocolate |
| 3 | Corpo Humano | cabeça, braço, coração, pulmão |
| 4 | Natureza | rio, montanha, floresta, oceano |
| 5 | Profissões | médico, professor, bombeiro, piloto |
| 6 | Países | brasil, japão, frança, canadá |
| 7 | Esportes | futebol, tênis, natação, vôlei |
| 8 | Cores e Formas | vermelho, círculo, triângulo, azul |

> Tema genérico + categorias amplas = maior banco de palavras possível e máximo reaproveitamento.

---

## 3. Telas do MVP

### Proposta — 5 telas mínimas

```
[Splash] → [Menu Principal] → [Seleção de Categoria] → [Seleção de Nível] → [Jogo] → [Vitória]
```

| Tela | Conteúdo | Complexidade |
|------|----------|-------------|
| **Splash** | Logo + loading | Mínima |
| **Menu Principal** | Botão "Jogar", Botão "Configurações" | Mínima |
| **Seleção de Categoria** | Lista/grid de categorias com progresso | Baixa |
| **Seleção de Nível** | Grid de níveis (bloqueado/desbloqueado/completo) | Baixa |
| **Tela de Jogo** | Grid + lista de palavras + botão dica + timer opcional | Média |
| **Popup Vitória** | "Parabéns" + botão próximo nível | Mínima |

### Tela de Configurações (dentro do menu)
- Som on/off
- Música on/off
- Idioma (preparado mas só PT-BR ativo)

### O que NÃO terá de telas no MVP
- Tela de perfil/avatar
- Leaderboard/ranking
- Loja de itens
- Tela de conquistas
- Tutorial interativo (apenas tooltip simples na primeira partida)

---

## 4. Monetização no MVP

### Proposta: Sim, já com ads desde o MVP

| Tipo de Ad | Quando | Justificativa |
|------------|--------|---------------|
| **Interstitial** | A cada 3 níveis completos | Não intrusivo, receita base |
| **Rewarded Ad** | Botão "Dica" (assistir ad = revelar 1 palavra) | Valor percebido pelo jogador, alta taxa de opt-in |
| **Banner** | Não | Polui a UI do grid, prejudica experiência |

### Por que incluir ads no MVP?
- Validar monetização cedo
- SDK de ads (AdMob) precisa ser integrado de qualquer forma
- Rewarded ads agregam valor ao jogo (dicas grátis)
- Dados reais de CPM desde o início

### IAP no MVP?
**Não.** Motivos:
- Adiciona complexidade de lógica de compra, restore, validação
- Requer configuração na Play Store mais elaborada
- Ads já validam monetização suficientemente
- IAP entra na v1.1

---

## 5. Features cortadas (explicitamente fora do MVP)

### ❌ NÃO entra no MVP

| Feature | Motivo do corte | Versão planejada |
|---------|----------------|-----------------|
| Palavras invertidas | Complexidade desnecessária | v1.1 |
| Sistema de estrelas/score | UI e lógica extra | v1.1 |
| Leaderboard | Requer backend | v1.2 |
| Conquistas/achievements | Lógica extra, UI extra | v1.1 |
| Múltiplos idiomas (jogável) | Conteúdo + teste | v1.1 |
| IAP (compras) | Complexidade de loja | v1.1 |
| Banner ads | Polui UX | Nunca (decisão de design) |
| Modo diário (Wordle-style) | Feature secundária | v1.2 |
| Tema escuro/claro | UI extra | v1.1 |
| Tutorial interativo | Complexidade | v1.1 |
| Compartilhamento social | SDK extra | v1.2 |
| Perfil do jogador | Backend | v1.2 |
| Animações elaboradas | Tempo de produção | v1.1 |
| Efeitos sonoros variados | Produção de assets | v1.1 |

---

## 6. Resumo do MVP

### ✅ O que o MVP faz

> O jogador abre o app → escolhe uma categoria → escolhe um nível → encontra palavras no grid arrastando o dedo → completa o nível → avança para o próximo. Pode assistir um ad para ganhar uma dica. A cada 3 níveis vê um interstitial.

### Stack mínima

| Componente | Tecnologia |
|------------|-----------|
| Engine | Unity (2D) |
| Ads | Google AdMob |
| Analytics | Unity Analytics (gratuito) |
| Storage (progresso) | PlayerPrefs (local) |
| Storage (palavras) | JSON embarcado (Resources/) |
| Geração de níveis | Runtime com seed determinístico |
| Backend | Nenhum |
| Idioma | PT-BR |

### Métricas de sucesso do MVP

| Métrica | Target |
|---------|--------|
| Jogador completa 1 categoria inteira | Sim |
| Sessão média | 3-5 minutos |
| Retenção D1 | > 30% |
| Crash rate | < 1% |
| Ad fill rate | > 80% |

---

## Perguntas para Decisão

1. Concorda com os 3 tamanhos de grid (8x8, 10x10, 12x12)?
2. Diagonal SIM mas invertidas NÃO — ok?
3. As 8 categorias propostas estão boas? Quer trocar alguma?
4. Concorda com ads já no MVP (interstitial + rewarded)?
5. Concorda com a lista de cortes?
6. Algo faltou que deveria estar no MVP?

---

## Decisão

> ✅ **Decidido em 2026-02-15**

**Todas as propostas aprovadas sem alterações:**

| Item | Decisão |
|------|---------|
| Grid | 8x8, 10x10, 12x12 |
| Direções | Horizontal, Vertical, Diagonal (sem inversão) |
| Palavras/nível | 5-10 |
| Categorias | 8 genéricas (Animais, Alimentos, Corpo Humano, Natureza, Profissões, Países, Esportes, Cores e Formas) |
| Níveis/categoria | 10-20 |
| Progressão | Linear |
| Telas | Splash, Menu, Categorias, Níveis, Jogo, Popup Vitória |
| Monetização | Interstitial (a cada 3 níveis) + Rewarded (dica) |
| IAP | Não no MVP |
| Banner | Não |
| Score/estrelas | Não no MVP |

---

## Próximos Passos

- [x] Criar documento organizado: `Organized/02_MVP_Scope.md` (DOC-002)
- [ ] Avançar para Discussion_03: Engine base / Arquitetura (DEF-003)
- [x] Atualizar Execution_Tracker
