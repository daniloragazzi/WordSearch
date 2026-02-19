# Checklist de Teste no Device — Termo BR

> Executar no dispositivo real (Android) após cada build de desenvolvimento.
> Marcar ✅ quando validado, ❌ para falhas encontradas.

---

## 1. Fluxo de Navegação

| # | Verificação | Status |
|---|-------------|--------|
| 1.1 | Boot scene carrega e transiciona para MainMenu em < 3s | ⬜ |
| 1.2 | Splash screen exibe logo Ragazzi Studios | ⬜ |
| 1.3 | MainMenu: botão "Jogar" navega para ModeSelect | ⬜ |
| 1.4 | MainMenu: botão "Estatísticas" abre popup com dados | ⬜ |
| 1.5 | MainMenu: botão "Configurações" abre popup com toggles/dropdown | ⬜ |
| 1.6 | ModeSelect: 3 botões (Termo/Dueto/Quartixo) navegam para Game | ⬜ |
| 1.7 | Game: botão Voltar retorna ao ModeSelect | ⬜ |

## 2. Gameplay — Modo Termo (1 palavra)

| # | Verificação | Status |
|---|-------------|--------|
| 2.1 | Teclado virtual responde a toques | ⬜ |
| 2.2 | Letras aparecem nas células da linha ativa | ⬜ |
| 2.3 | Backspace remove a última letra | ⬜ |
| 2.4 | Enter submete a tentativa de 5 letras | ⬜ |
| 2.5 | Enter com < 5 letras mostra feedback (shake ou mensagem) | ⬜ |
| 2.6 | Palavra inválida mostra feedback de erro | ⬜ |
| 2.7 | Animação de flip executa corretamente (scale Y) | ⬜ |
| 2.8 | Cores corretas: verde (Correct), amarelo (Present), cinza (Absent) | ⬜ |
| 2.9 | Teclado atualiza cores das teclas após cada tentativa | ⬜ |
| 2.10 | Vitória: popup com palavra + contagem de tentativas | ⬜ |
| 2.11 | Derrota: popup com palavra revelada | ⬜ |
| 2.12 | 6 tentativas máximas no modo Termo | ⬜ |

## 3. Gameplay — Modo Dueto (2 palavras)

| # | Verificação | Status |
|---|-------------|--------|
| 3.1 | 2 boards exibidos lado a lado | ⬜ |
| 3.2 | 7 tentativas máximas | ⬜ |
| 3.3 | Tentativa aplicada a ambos os boards | ⬜ |
| 3.4 | Board resolvido para de receber guess | ⬜ |
| 3.5 | Vitória quando ambos resolvidos | ⬜ |
| 3.6 | Derrota quando esgota tentativas com board pendente | ⬜ |

## 4. Gameplay — Modo Quartixo (4 palavras)

| # | Verificação | Status |
|---|-------------|--------|
| 4.1 | 4 boards exibidos em grid 2×2 | ⬜ |
| 4.2 | 9 tentativas máximas | ⬜ |
| 4.3 | Cells menores (72px) para caber | ⬜ |
| 4.4 | Vitória quando todos 4 resolvidos | ⬜ |
| 4.5 | Derrota parcial (3 de 4 resolvidos) = Lost | ⬜ |

## 5. Áudio

| # | Verificação | Status |
|---|-------------|--------|
| 5.1 | Música ambiente toca no boot e segue entre cenas | ⬜ |
| 5.2 | Toggle "Música" no Settings desliga/liga a música | ⬜ |
| 5.3 | Toggle "Som" no Settings desliga/liga os SFX | ⬜ |
| 5.4 | SFX de flip toca na animação | ⬜ |
| 5.5 | SFX de vitória toca no popup de win | ⬜ |
| 5.6 | Volume adequado (não alto demais) | ⬜ |

## 6. Tema Claro / Escuro

| # | Verificação | Status |
|---|-------------|--------|
| 6.1 | Dropdown "Tema" em Settings: Sistema/Claro/Escuro | ⬜ |
| 6.2 | Tema Claro aplica fundo branco + texto escuro | ⬜ |
| 6.3 | Tema Escuro aplica fundo escuro + texto claro | ⬜ |
| 6.4 | Tema Sistema segue preferência do dispositivo | ⬜ |
| 6.5 | Cores de feedback (verde/amarelo/cinza) visíveis em ambos os temas | ⬜ |
| 6.6 | Troca de tema persiste ao reiniciar app | ⬜ |

## 7. Layout e Responsividade

| # | Verificação | Status |
|---|-------------|--------|
| 7.1 | Layout correto em 16:9 (ex: 1080×1920) | ⬜ |
| 7.2 | Layout correto em 19.5:9 (ex: 1080×2340) | ⬜ |
| 7.3 | Layout correto em tablet 4:3 (ex: 1536×2048) | ⬜ |
| 7.4 | Safe area respeitada (notch/barra de status) | ⬜ |
| 7.5 | Texto legível em todas as resoluções | ⬜ |

## 8. Persistência

| # | Verificação | Status |
|---|-------------|--------|
| 8.1 | Estatísticas persistem após fechar e reabrir o app | ⬜ |
| 8.2 | Configurações de som/música/tema persistem | ⬜ |
| 8.3 | Streak incrementa em vitórias consecutivas | ⬜ |
| 8.4 | Streak reseta em derrota | ⬜ |

## 9. Performance

| # | Verificação | Status |
|---|-------------|--------|
| 9.1 | FPS estável (> 30 fps em device mid-range) | ⬜ |
| 9.2 | Sem spikes perceptíveis durante flip animation | ⬜ |
| 9.3 | Boot time < 5 segundos | ⬜ |
| 9.4 | Sem memory leaks visíveis (jogar 5+ partidas) | ⬜ |

---

## Registro de Teste

| Data | Device | Android | Build | Resultado | Notas |
|------|--------|---------|-------|-----------|-------|
| — | — | — | — | — | — |
