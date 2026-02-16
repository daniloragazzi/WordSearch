# 10 — Guia de Build e Teste Local

> **Projeto:** Caça-Palavras (Word Search)  
> **Desenvolvedor:** Ragazzi Studios  
> **Última atualização:** Stage 2.9

---

## 1. Pré-requisitos

### 1.1 Unity Modules Necessários
```
Unity Hub → Installs → Unity 6.3 LTS → Add Modules:
  ✅ Android Build Support
  ✅ Android SDK & NDK Tools
  ✅ OpenJDK
```

> O Unity instala o Android SDK/NDK/JDK automaticamente. Não precisa instalar separadamente.

### 1.2 Verificar Instalação
```
Unity → Edit → Preferences → External Tools:
  ✅ Android SDK → deve mostrar caminho (ex: C:/Program Files/Unity/Hub/Editor/6000.3.8f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK)
  ✅ Android NDK → automático
  ✅ JDK → automático
```

---

## 2. Setup Inicial no Unity (Primeira Vez)

### 2.1 Criar Cenas
```
Menu: Build → Ragazzi Studios → 🎬 Create All Scenes
```
Isso cria 3 cenas com hierarquia de GameObjects prontos:
- `Boot.unity` — Camera, GameManager, BootLoader, Canvas com loading
- `MainMenu.unity` — Camera, NavigationController, Canvas com 3 screens
- `Game.unity` — Camera, GameplayController, Canvas com grid/wordlist/popups

### 2.2 Configurar Build Settings
```
Menu: Build → Ragazzi Studios → 🔧 Configure Scenes
```
Ou manualmente:
```
File → Build Settings:
  Platform: Android (Switch Platform se necessário)
  Scenes In Build:
    [0] Assets/_Project/Scenes/Boot.unity
    [1] Assets/_Project/Scenes/MainMenu.unity
    [2] Assets/_Project/Scenes/Game.unity
```

### 2.3 Verificar Tudo
```
Menu: Build → Ragazzi Studios → 📋 Verify Build Settings
```

### 2.4 Scripts e Referências (Automático ✅)
O `SceneCreator` já faz tudo automaticamente:
- Adiciona **todos os scripts** (MonoBehaviours) aos GameObjects corretos
- Conecta **todas as referências** (SerializeField) via `SerializedObject`
- Cria **prefabs template** (LetterCell, CategoryButton, LevelButton, WordListItem)

> ✅ **Nenhuma configuração manual necessária!** As cenas já estão 100% funcionais.

---

## 3. Gerar APK

### 3.1 Build Development (Recomendado para testes)
```
Menu: Build → Ragazzi Studios → 📱 Build APK (Development)
```
- Inclui símbolos de debug
- Permite profiling
- Mais rápido para compilar
- APK maior (~10-15% maior)

### 3.2 Build Release
```
Menu: Build → Ragazzi Studios → 📱 Build APK (Release)
```
- Otimizado
- Sem debug
- Tamanho final

### 3.3 Output
```
WordSearch/Builds/Android/CacaPalavras_dev.apk  (development)
WordSearch/Builds/Android/CacaPalavras.apk      (release)
```

---

## 4. Teste no Device Real (USB)

### 4.1 Preparar o Dispositivo
```
1. Configurações → Sobre o telefone
2. Tocar "Número da versão" 7 vezes → Ativar Opções de desenvolvedor
3. Configurações → Opções de desenvolvedor:
   ✅ Depuração USB (USB Debugging)
   ✅ Instalar via USB
4. Conectar cabo USB ao PC
5. Aceitar popup "Permitir depuração USB"
```

### 4.2 Instalar APK
```powershell
# Via ADB (se instalado)
adb install -r "Builds/Android/CacaPalavras_dev.apk"

# Ou via Unity
# File → Build Settings → Build And Run
# (instala e executa automaticamente no device conectado)
```

### 4.3 Alternativa: Copiar APK manualmente
```
1. Copiar APK para o celular (WhatsApp, Google Drive, cabo USB)
2. No celular: Abrir APK → Instalar
   (Pode precisar permitir "Instalar de fontes desconhecidas")
```

---

## 5. Teste no Emulador (PC)

### 5.1 Opção A: Android Emulator (recomendado)
```
1. Instalar Android Studio (apenas o emulador):
   https://developer.android.com/studio
2. Tools → AVD Manager → Create Virtual Device
3. Escolher: Pixel 6 (ou similar)
4. System Image: API 34 (Android 14)
5. Start emulator
6. No Unity: Build And Run (detecta o emulador como device)
```

### 5.2 Opção B: Teste direto no Unity Editor
```
1. Abrir Boot.unity
2. Play (▶️)
3. Game view: resolução "1080x1920 Portrait"
```
> ⚠️ No Editor, touch é simulado com mouse. Funcional mas não 100% fiel.

### 5.3 Opção C: Unity Remote (celular como display)
```
1. Instalar "Unity Remote 5" da Play Store no celular
2. Unity → Edit → Project Settings → Editor → Unity Remote → Device: Any Android Device
3. Conectar USB, Play no Editor
4. O jogo renderiza no celular em tempo real
```

---

## 6. Checklist de Teste

### 6.1 Funcional
- [ ] Boot: carregamento sem erros, transição para MainMenu
- [ ] MainMenu: título, botões Play e Settings visíveis
- [ ] Categorias: 8 categorias com nomes e ícones corretos
- [ ] Níveis: 15 níveis visíveis, nível 1 desbloqueado
- [ ] Grid: letras renderizadas, tamanho correto (8x8, 10x10, 12x12)
- [ ] Seleção: arrastar dedo/mouse seleciona letras em linha
- [ ] Palavra encontrada: strikethrough na lista, células mudam cor
- [ ] Todas palavras: popup de vitória aparece
- [ ] Vitória: estatísticas corretas (tempo, palavras)
- [ ] Próximo nível: desbloqueia e abre corretamente
- [ ] Voltar: navegação entre telas funciona
- [ ] Settings: toggles de som/música funcionam

### 6.2 Visual
- [ ] Orientação: apenas portrait, sem rotação
- [ ] Cores: tema aplicado consistentemente
- [ ] Textos: legíveis, sem overflow
- [ ] Grid: centralizado, sem cortes
- [ ] Botões: clicáveis, feedback visual
- [ ] Acentos: CORAÇÃO, LEÃO exibidos corretamente

### 6.3 Performance
- [ ] FPS: >= 30fps estável
- [ ] Loading: < 3 segundos
- [ ] Memória: sem crashes
- [ ] Tamanho APK: < 50MB

---

## 7. Configurações Android do Projeto

| Setting | Valor | Motivo |
|---------|-------|--------|
| Package Name | `com.ragazzistudios.wordsearch` | ID único na Play Store |
| Version | `0.1.0` | MVP |
| Bundle Code | `1` | Incrementar a cada build na Play Store |
| Min SDK | API 24 (Android 7.0) | Cobre 99%+ dos devices |
| Target SDK | API 34 (Android 14) | Requisito Play Store 2025 |
| Scripting Backend | IL2CPP | Performance + requisito Play Store |
| Architecture | ARM64 | Padrão atual, requisito Play Store |
| Orientation | Portrait | Design do jogo |
| Internet | Não obrigatório | Ads precisam, mas jogo funciona offline |

---

## 8. Troubleshooting

### "Android SDK not found"
```
Unity → Edit → Preferences → External Tools
→ Desmarcar "Android SDK Tools Installed with Unity"
→ Remarcar "Android SDK Tools Installed with Unity"
→ Reiniciar Unity
```

### Build falha com IL2CPP
```
Primeiro build pode ser lento (5-10 min).
Se falhar, testar com Mono primeiro:
  Edit → Project Settings → Player → Other Settings
  → Scripting Backend: Mono
  (Depois voltar para IL2CPP)
```

### APK não instala no device
```
1. Verificar se USB Debugging está ativado
2. Desinstalar versão anterior do app
3. Configurações → Segurança → Permitir fontes desconhecidas
```

### Tela preta no device
```
1. Verificar se as cenas estão no Build Settings
2. Menu: Build → Ragazzi Studios → 🔧 Configure Scenes
3. Verificar se Boot é a cena [0]
```
