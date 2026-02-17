# 09 — Guia de Integração SDK

> **Projeto:** Caça-Palavras (Word Search)  
> **Desenvolvedor:** Ragazzi Studios  
> **Última atualização:** Stage 2.8

---

## 1. Google AdMob (CFG-005)

### 1.1 Pré-requisitos
- Conta Google AdMob: https://admob.google.com
- App registrado no AdMob Console
- Google Mobile Ads Unity Plugin v9.x+

### 1.2 Instalação do SDK

```
1. Baixar: https://github.com/googleads/googleads-mobile-unity/releases
2. Unity → Assets → Import Package → Custom Package
3. Ou via UPM (git URL): https://github.com/googleads/googleads-mobile-unity.git
```

### 1.3 Configuração

```
1. Unity → Assets → Google Mobile Ads → Settings
2. Preencher:
   - Android App ID: ca-app-pub-XXXXXXXXXXXXXXXX~YYYYYYYYYY
   - Delay app measurement: ✅ ON (GDPR/LGPD compliance)
```

### 1.4 Ad Unit IDs

| Tipo | Teste (Android) | Produção |
|------|-----------------|----------|
| Interstitial | `ca-app-pub-3940256099942544/1033173712` | Criar no AdMob Console |
| Rewarded | `ca-app-pub-3940256099942544/5224354917` | Criar no AdMob Console |

> ⚠️ **IMPORTANTE:** Os IDs de teste são do Google e funcionam sem conta. Substituir por IDs reais apenas na build de produção.

### 1.5 Ativação no Projeto

1. Abrir `GameManager.cs` no Inspector
2. Desmarcar `_useMockServices`
3. O `AdMobService.cs` será registrado em vez do `MockAdsService`
4. Descomentar o código SDK real dentro de `AdMobService.cs`

### 1.6 Fluxo de Ads no Jogo

```
Interstitial:
  → A cada 3 níveis completados (GameManager.TryShowInterstitial)
  → Exibe entre tela de vitória e próximo nível

Rewarded:
  → Quando jogador toca "Dica" (GameManager.RequestHint)
  → Recompensa: revela primeira letra de uma palavra não encontrada
```

### 1.7 Checklist de Produção
- [ ] Criar App no AdMob Console
- [ ] Criar Ad Units (Interstitial + Rewarded)
- [ ] Substituir Test IDs em `AdMobService.cs`
- [ ] Substituir Android App ID nas Settings
- [ ] Descomentar código SDK em `AdMobService.cs`
- [ ] Testar em device real (test ads não funcionam em emulador)
- [ ] Configurar mediation (opcional)
- [ ] Revisar políticas do AdMob (conteúdo, COPPA, etc.)

---

## 2. Unity Analytics (CFG-006)

### 2.1 Pré-requisitos
- Unity Gaming Services habilitado no projeto
- Conta Unity com projeto vinculado

### 2.2 Instalação

```
1. Unity → Edit → Project Settings → Services
2. Vincular projeto ao Unity Dashboard (ou criar novo)
3. Window → Package Manager → Unity Registry:
   - Instalar "Analytics" (com.unity.services.analytics)
   - Instalar "Core" (com.unity.services.core) — dependência
```

### 2.3 Configuração no Dashboard

```
1. https://dashboard.unity.com → Selecionar projeto
2. Analytics → Settings → Enable Analytics
3. Configurar:
   - Data Collection: Standard Events + Custom Events
   - Data Retention: 90 days (padrão)
   - GDPR/LGPD: Configurar região e consent flow
```

### 2.4 Eventos Customizados (DOC-007)

| Evento | Parâmetros | Quando |
|--------|-----------|--------|
| `game_start` | — | App aberto |
| `level_start` | category, level, difficulty | Início de nível |
| `level_complete` | category, level, time_seconds, hints_used | Nível concluído |
| `level_quit` | category, level, time_seconds, words_found | Saiu do nível |
| `hint_used` | category, level | Usou dica |
| `ad_shown` | ad_type | Ad exibido |
| `ad_clicked` | ad_type | Ad clicado |
| `category_selected` | category | Escolheu categoria |
| `session_end` | duration_seconds | Sessão encerrada |

### 2.5 Consent Flow (LGPD/GDPR)

```csharp
// Deve ser implementado ANTES de Initialize()
// Mostrar popup de consentimento na primeira execução
// Salvar preferência em PlayerPrefs

if (userConsentGiven)
{
    AnalyticsService.Instance.StartDataCollection();
}
else
{
    AnalyticsService.Instance.StopDataCollection();
}
```

### 2.6 Ativação no Projeto

1. Abrir `GameManager.cs` no Inspector
2. Desmarcar `_useMockServices`
3. `UnityAnalyticsService.cs` será registrado
4. Descomentar código SDK real dentro de `UnityAnalyticsService.cs`

### 2.7 Checklist de Produção
- [ ] Habilitar Analytics no Unity Dashboard
- [ ] Instalar packages (analytics + core)
- [ ] Vincular projeto
- [ ] Implementar consent flow (LGPD obrigatório no Brasil)
- [ ] Descomentar código em `UnityAnalyticsService.cs`
- [ ] Registrar custom events no Dashboard (validação de schema)
- [ ] Testar em device real
- [ ] Verificar dados chegando no Dashboard
- [ ] Criar funnels e dashboards customizados

---

## 3. Arquitetura de Integração

```
┌──────────────────────────────────────────────┐
│                  GameManager                  │
│                                              │
│  _useMockServices = true?                    │
│     ├─ true  → MockAdsService                │
│     │         → MockAnalyticsService          │
│     └─ false → AdMobService                  │
│                → UnityAnalyticsService         │
│                                              │
│  ServiceLocator.Register<IAdsService>(impl)  │
│  ServiceLocator.Register<IAnalyticsService>()│
└──────────────────────────────────────────────┘
```

### Vantagens da Arquitetura
- **Zero mudanças no código de jogo** ao trocar implementação
- **Toggle simples** no Inspector para mock vs real
- **Testável** — Mock services logam no Console
- **Extensível** — Pode adicionar Firebase, Adjust, etc. criando novas implementações

---

## 4. Prioridade de Integração

| Prioridade | SDK | Quando |
|-----------|-----|--------|
| 🔴 Alta | AdMob | Antes do soft launch (receita) |
| 🟡 Média | Unity Analytics | Antes do soft launch (dados) |
| 🟢 Baixa | Consent Flow | Junto com Analytics (obrigatório) |
| ⚪ Futura | Firebase Crashlytics | Pós-launch (estabilidade) |
| ⚪ Futura | Remote Config | Pós-launch (A/B testing) |
