# Discussion 08 — Termo-Like: Escopo Detalhado do MVP

> **Status:** ✅ Concluído
> **Data:** 2026-02-17
> **Objetivo:** Definir escopo completo do MVP — telas, fluxo, modos de jogo, banco de palavras e monetização.

---

## Resumo das Decisões (Discussion_07)

| Item | Decisão |
|------|---------|
| Modo | Ilimitado |
| Palavra | 5 letras, sem acento |
| Modos | 1 palavra (6 tent.), 2 palavras (7 tent.), 4 palavras (9 tent.) |
| Diferencial | Duordle / Quordle integrados |
| Visual | Derivado do Caça-Palavras (`GameTheme` unificado) |

---

## Telas do MVP

### Fluxo principal

```
Boot → MainMenu → ModeSelect → Gameplay → WinPopup / LosePopup
                     ↑                         ↓
                  (voltar)               (jogar novamente)
```

### Descrição de cada tela

| Tela | Descrição |
|------|-----------|
| **Boot** | Carrega dados (banco de palavras), inicializa serviços, vai para MainMenu |
| **MainMenu** | Logo do app, botão Jogar, botão Configurações |
| **ModeSelect** | Escolha do modo: 1 palavra / 2 palavras / 4 palavras |
| **Gameplay** | Grade de tentativas + teclado virtual |
| **WinPopup** | Parabéns, número de tentativas, botão Jogar Novamente |
| **LosePopup** | Palavra revelada, botão Jogar Novamente |
| **SettingsPopup** | Som, Música, Tema (claro/escuro) |

---

## Layout do Gameplay por Modo

### Modo 1 palavra (clássico)
- 1 grade central: 6 linhas × 5 colunas
- Teclado embaixo (3 linhas: QWERTYUIOP / ASDFGHJKL / ZXCVBNM)
- Header: modo atual + botão configurações

### Modo 2 palavras (Duordle)
- 2 grades lado a lado (ou empilhadas em portrait): 7 linhas × 5 colunas cada
- Mesmo teclado compartilhado (cores refletem pior caso entre as 2 palavras)
- Palavra resolvida fica "travada" com borda de vitória; tentativas continuam para a outra

### Modo 4 palavras (Quordle)
- 4 grades em 2×2: 9 linhas × 5 colunas cada
- Mesmo teclado compartilhado
- Cada grade travada individualmente ao ser resolvida

---

## Mecânica de Feedback (todos os modos)

| Estado da letra | Cor | Token no GameTheme |
|-----------------|-----|--------------------|
| Correta (posição certa) | 🟩 Verde | `letterCorrect` |
| Presente (posição errada) | 🟨 Amarelo | `letterPresent` |
| Ausente | ⬛ Cinza escuro | `letterAbsent` |
| Não tentada | ⬜ Cinza claro | `letterUnused` |

- Teclado reflete o **melhor estado** já visto para cada letra.
- Em modos multi-palavra, o teclado reflete o **pior estado entre as grades** (mais conservador).

---

## Teclado Virtual

Layout PT-BR sem acentos:

```
Q  W  E  R  T  Y  U  I  O  P
 A  S  D  F  G  H  J  K  L
   Z  X  C  V  B  N  M  ⌫
              ENTER
```

- Tecla `⌫` apaga última letra
- Tecla `ENTER` submete tentativa (só ativa com 5 letras)
- Cada tecla muda de cor conforme feedback acumulado

---

## Validação de Entrada

- Palavra deve ter exatamente **5 letras**
- Palavra deve existir no **dicionário de palavras válidas** (lista de aceite)
- Entrada inválida: shake na linha atual + mensagem breve ("Palavra não encontrada")
- Sem acento — normalização aplicada no banco e na entrada do usuário

---

## Banco de Palavras

Duas listas distintas:

| Lista | Finalidade | Volume mínimo |
|-------|------------|---------------|
| **Palavras-alvo** | Sorteadas como resposta | 1.000+ palavras comuns de 5 letras |
| **Palavras válidas** | Aceitas como tentativa (inclui as alvo) | 3.000+ palavras de 5 letras |

- Todas sem acento, uppercase internamente, lowercase exibido
- Geradas via script Python (mesmo padrão do Caça-Palavras)
- Armazenadas em JSON: `Resources/Data/words_5.json` e `Resources/Data/valid_5.json`
- Sorteio da palavra-alvo: seed baseada em índice de partida (determinístico, sem repetição imediata)

---

## Progressão e Estado

| Dado | Armazenamento | Chave |
|------|---------------|-------|
| Partidas jogadas | PlayerPrefs | `TERMO_GAMES_PLAYED` |
| Partidas vencidas | PlayerPrefs | `TERMO_GAMES_WON` |
| Sequência atual (streak) | PlayerPrefs | `TERMO_STREAK` |
| Maior sequência | PlayerPrefs | `TERMO_MAX_STREAK` |
| Distribuição de tentativas (1–6) | PlayerPrefs (JSON) | `TERMO_GUESS_DIST` |
| Índice atual da palavra-alvo | PlayerPrefs | `TERMO_WORD_INDEX` |

- Estatísticas exibidas no WinPopup / LosePopup

---

## Monetização

| Mecanismo | Trigger | Observações |
|-----------|---------|-------------|
| **Interstitial** | Ao fechar WinPopup / LosePopup | Frequência: 1 a cada 3 partidas |
| **Rewarded** | Botão "Dica" durante gameplay | Revela 1 letra correta na posição; 1 uso por partida |
| **IAP (futuro)** | Remover ads | Fora do MVP |

---

## Tela de Estatísticas (incluída no MVP)

Popup acessível no header com:
- Total de partidas / % vitórias
- Sequência atual / maior sequência
- Distribuição de tentativas (barras horizontais)

Armazenamento local via PlayerPrefs — sem necessidade de backend.

---

## Decisões

> ✅ **Decidido em 2026-02-17**

| # | Questão | Decisão | Notas |
|---|---------|---------|-------|
| 1 | **Estatísticas** | Incluir no MVP (local) | PlayerPrefs — sem backend; popup acessível no header |
| 2 | **LosePopup** | Revelar palavra imediatamente | Sem confirmação prévia — feedback direto |
| 3 | **Animação de revelação** | Flip carta por carta | Estilo Wordle original — uma célula por vez, da esquerda para a direita |
| 4 | **Layout do teclado** | QWERTY | Familiar, padrão universal |

---

## Próximos Passos

- [x] Responder perguntas acima
- [ ] Discussion_09: Arquitetura técnica (estrutura do projeto, reaproveitamento do Core)
- [ ] Criar ActionPlan do segundo app com tarefas detalhadas
