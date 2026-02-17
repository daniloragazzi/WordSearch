# Discussion 07 — Segundo App: Termo-Like (Wordle Clone)

> **Status:** 🟡 Em discussão
> **Data:** 2026-02-17
> **Objetivo:** Definir escopo, mecânica, diferenciação e arquitetura do segundo app do estúdio — um jogo estilo Wordle/Termo em português.

---

## Contexto

O primeiro app (Caça-Palavras) está com desenvolvimento concluído e aguardando validação da conta Google Play. Aproveitamos o tempo para iniciar o planejamento do segundo jogo.

**Termo** é o Wordle brasileiro criado por Fernando Serboncini, um dos jogos de palavras mais jogados no Brasil. A mecânica é simples, viral e com excelente retenção diária — características ideais para o segundo MVP do estúdio.

### Referências de mercado
- **Wordle** (NYT) — original em inglês, 1 palavra/dia
- **Termo** (term.ooo) — versão PT-BR, 1 palavra/dia, gratuita na web
- **Letreco** — variante PT-BR com acentuação
- **Quordle / Octordle** — múltiplas palavras simultâneas
- **Wordle Ilimitado** — modo sem limite diário

---

## Mecânica Base

O jogador tem **6 tentativas** para adivinhar uma palavra de **5 letras**. A cada tentativa, as letras recebem feedback visual:

| Cor | Significado |
|-----|-------------|
| 🟩 Verde | Letra correta na posição correta |
| 🟨 Amarelo | Letra existe na palavra, mas em outra posição |
| ⬛ Cinza | Letra não está na palavra |

O teclado na tela também reflete o feedback acumulado das tentativas anteriores.

---

## Principais Decisões a Tomar

### 1. Modo de jogo — Diário vs Ilimitado vs Ambos?

| Modo | Prós | Contras |
|------|------|---------|
| **Diário (1 palavra/dia)** | Alta retenção, viral ("compartilhe seu resultado"), semântica clara | Sessão muito curta, menos monetizável por sessão |
| **Ilimitado** | Mais sessões/dia, melhor para ads, sem frustração de esperar | Perde a âncora social do "jogo do dia" |
| **Ambos** | Melhor dos dois mundos | Mais complexo de implementar e comunicar |

### 2. Tamanho da palavra — 5 letras fixo ou variável?

| Opção | Prós | Contras |
|-------|------|---------|
| **5 letras (padrão Termo)** | Familiar, banco de palavras abundante | Menos variedade |
| **4 e 6 letras opcionais** | Mais modos, mais rejogabilidade | Banco de palavras menor, mais difícil de balancear |
| **Configurável pelo jogador** | Máxima flexibilidade | UI mais complexa |

### 3. Acentuação — com ou sem acento?

| Opção | Prós | Contras |
|-------|------|---------|
| **Com acento** (ex: ÁRVORE) | Mais fiel ao PT-BR real | Teclado mais complexo, banco menor |
| **Sem acento** (normalizado) | Banco maior, teclado simples | Menos natural em português |
| **Misto** (conta como igual) | Inclusivo | Lógica de validação mais complexa |

### 4. Categorias temáticas?

Diferencial possível: além do modo clássico, palavras filtradas por tema (Animais, Esportes, Alimentos, etc.) — reutilizando o banco do Caça-Palavras.

### 5. Modelo de monetização

| Modelo | Viabilidade |
|--------|-------------|
| Ads entre partidas (interstitial) | ✅ Alto — padrão do mercado |
| Rewarded para dicas (revelar letra) | ✅ Alto — mecânica de hint muito natural aqui |
| Modo sem ads (IAP) | 🔵 Médio — premium simples |
| Assinatura de conteúdo temático | ⬜ Baixo para MVP |

---

## Reaproveitamento do App 1 (Caça-Palavras)

| Componente | Reaproveitável? | Notas |
|-----------|-----------------|-------|
| Clean Architecture (Core/Game) | ✅ Total | Mesma estrutura de camadas |
| ThemeManager (claro/escuro) | ✅ Total | Já pronto e testado |
| LocalizationService | ✅ Total | JSON de strings por idioma |
| StorageService (PlayerPrefs) | ✅ Total | Mesma interface |
| MusicManager | ✅ Total | DontDestroyOnLoad já funciona |
| SceneCreator (geração de cenas) | 🔵 Parcial | Padrão reutilizável, conteúdo novo |
| Banco de palavras PT-BR | 🔵 Parcial | Palavras de 5 letras filtradas das categorias existentes |
| SFX procedurais | ✅ Total | Mesmos sons podem ser reaproveitados |
| Sprites e fontes (Nunito) | ✅ Total | Mesma identidade visual base |
| AdMob / Analytics services | ✅ Total | Mesmas interfaces e stubs |
| GameTheme / paletas | 🔵 Parcial | Pode usar base, cores de feedback são novas (verde/amarelo/cinza) |

---

## Diferenciação Potencial

Ideias para se destacar no mercado:

1. **Modo Desafio por Categoria** — palavra do dia de uma categoria específica (ex: "Animais de segunda-feira")
2. **Modo Duplo / Quadruplo** — 2 ou 4 palavras simultâneas (Duordle/Quordle estilo)
3. **Modo Contra o Relógio** — resolver em menos de X segundos
4. **Histórico de partidas** — estatísticas de sequência, distribuição de tentativas
5. **Compartilhamento de resultado** — grade emoji copiável (como Wordle original)
6. **Ranking semanal** — posição entre jogadores (requer backend — P2)

---

## Riscos e Considerações

| Risco | Severidade | Mitigação |
|-------|------------|-----------|
| Banco de palavras de 5 letras insuficiente | Alta | Curar lista dedicada de 2.000+ palavras comuns |
| Monetização fraca (sessões curtas no modo diário) | Média | Priorizar modo ilimitado para ads |
| Comparação desfavorável com Termo (gratuito na web) | Média | Experiência nativa + off-line + categorias como diferencial |
| Acentuação complexa no teclado virtual | Média | Definir política clara desde o início (com ou sem) |

---

## Perguntas para Decidir

1. **Modo principal:** Diário, Ilimitado ou ambos no MVP?
2. **Tamanho da palavra:** Fixo em 5 letras ou queremos variação?
3. **Acentuação:** Com acento, sem acento ou normalizado?
4. **Categorias:** incluir modo temático no MVP ou só clássico?
5. **Diferencial prioritário:** compartilhamento de resultado, modos extras, categorias?
6. **Identidade visual:** mesma paleta base do Caça-Palavras com cores de feedback adaptadas, ou identidade própria?

---

## Próximos Passos

- [ ] Responder às perguntas acima para fechar escopo do MVP
- [ ] Discussion_08: Escopo detalhado + banco de palavras
- [ ] Discussion_09: Arquitetura técnica (reaproveitamento vs novo)
- [ ] Criar `Organized/` equivalente ao processo do App 1
