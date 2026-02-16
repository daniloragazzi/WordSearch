# Discussion 04 — Pipeline Técnico

> **Código:** DEF-005
> **Status:** 🟡 Em discussão
> **Data:** 2026-02-15
> **Objetivo:** Definir o pipeline completo de desenvolvimento, build e deploy.

---

## Contexto

O PRD define como princípios: CLI-first, VS Code como IDE, IA integrada, automação, escala por repetição. Precisamos traduzir isso em um pipeline concreto.

---

## 1. Ambiente de Desenvolvimento

### Proposta

| Ferramenta | Uso | Justificativa |
|------------|-----|---------------|
| **Unity 6.3 LTS** | Engine de jogo | LTS = estabilidade, versão 6000.3.8f1 |
| **VS Code** | IDE principal | Conforme PRD, extensões C# excelentes |
| **Git** | Versionamento | Padrão |
| **GitHub** | Repositório remoto | CI/CD gratuito, Actions |
| **Python** | Scripts auxiliares (geração de dados, automação) | Versatilidade, IA |
| **GitHub Copilot / IA** | Copiloto de dev | Conforme PRD |

### Extensões VS Code recomendadas
- C# (Microsoft)
- Unity (Microsoft)
- GitLens
- TODO Highlight
- Markdown Preview

---

## 2. Git Strategy

### Proposta: Git Flow simplificado

```
main          ← produção (versões publicadas)
  └── develop ← desenvolvimento ativo
       ├── feature/grid-system
       ├── feature/word-database
       └── feature/ad-integration
```

| Branch | Propósito |
|--------|-----------|
| `main` | Versão em produção na Play Store |
| `develop` | Branch de integração |
| `feature/*` | Features individuais |
| `hotfix/*` | Correções urgentes em produção |
| `release/*` | Preparação para release |

### Convenção de commits

```
feat: adiciona sistema de grid
fix: corrige validação de palavra diagonal
docs: atualiza escopo do MVP
refactor: extrai WordPlacer do GridGenerator
chore: configura AdMob SDK
data: adiciona palavras categoria esportes
```

| Prefixo | Uso |
|---------|-----|
| `feat` | Nova feature |
| `fix` | Correção de bug |
| `docs` | Documentação |
| `refactor` | Refatoração sem mudar comportamento |
| `chore` | Tarefas de manutenção |
| `data` | Alterações em dados/conteúdo |
| `style` | Formatação, sem mudança de código |
| `test` | Testes |

---

## 3. Build Pipeline

### Proposta: Automação progressiva

#### Fase 1 — MVP (manual + scripts)
```
[Dev local] → [Build Unity CLI] → [APK] → [Upload manual Play Store]
```

| Etapa | Como |
|-------|------|
| Desenvolver | VS Code + Unity |
| Build | Unity CLI (batch mode) via script local |
| Testar | APK no device / emulador |
| Deploy | Upload manual no Google Play Console |

#### Fase 2 — Pós-MVP (automatizado)
```
[Push GitHub] → [GitHub Actions] → [Build Unity] → [APK/AAB] → [Deploy Play Store]
```

### Por que manual no MVP?
- GitHub Actions + Unity requer licença Unity Plus/Pro para CI
- Alternativas gratuitas (GameCI) têm limitações
- Não vale o investimento de tempo na fase MVP
- Scripts locais já automatizam o suficiente

### Script de build proposto (local)

```
scripts/
  build_android.sh    ← Build APK/AAB via Unity CLI
  generate_words.py   ← Gerar JSONs de palavras
  validate_data.py    ← Validar JSONs antes do build
```

---

## 4. Pipeline de Dados (Palavras)

### Proposta: Geração offline via Python

```
[Curadoria/IA] → [Python script] → [JSON validado] → [Resources/Data/]
```

| Etapa | Ferramenta | Output |
|-------|-----------|--------|
| Geração de palavras | Python + IA (ChatGPT/Copilot) | Lista bruta |
| Curadoria | Python script (filtros) | Lista limpa |
| Formatação | Python script | JSON formatado |
| Validação | Python script | JSON validado |
| Integração | Copiar para Resources/Data/ | Pronto para build |

### Regras de validação
- Sem duplicatas
- Sem palavras menores que 3 letras
- Sem caracteres especiais (apenas letras + acentos)
- Normalização de acentos para o grid (grid sem acento, lista com acento)
- Mínimo de palavras por categoria (ex: 50)

---

## 5. Versionamento de App

### Proposta

```
v[major].[minor].[patch]

Exemplos:
  v0.1.0  → Primeiro build jogável (alpha)
  v0.2.0  → Alpha com ads
  v1.0.0  → MVP publicado na Play Store
  v1.1.0  → Primeiro update (features cortadas do MVP)
```

| Campo | Quando incrementa |
|-------|-------------------|
| major | Mudança grande, breaking |
| minor | Nova feature |
| patch | Bug fix |

### Version codes (Android)
- `versionCode` incrementa a cada build: 1, 2, 3, 4...
- `versionName` segue semver: "1.0.0", "1.1.0"

---

## 6. Testes

### Proposta: Pragmática para MVP

| Tipo | Cobertura MVP | Ferramenta |
|------|--------------|-----------|
| **Unit tests** | Domain layer (Grid, Words) | Unity Test Runner (NUnit) |
| **Manual testing** | UI, fluxo completo | APK no device real |
| **Data validation** | JSONs de palavras | Python script |
| **Play testing** | Gameplay, UX | Testers manuais |

### O que NÃO testar no MVP
- UI automatizado (muito frágil, alto custo)
- Performance profiling (prematuro)
- Integration tests (complexidade desnecessária)

### Foco de testes unitários
- `GridGenerator` — gera grid válido?
- `WordPlacer` — posiciona palavra corretamente?
- `WordFinder` — detecta seleção válida?
- `LevelGenerator` — seed produz resultado determinístico?

---

## 7. Estrutura de Scripts/Ferramentas

### Proposta

```
WordGames/
├── Assets/                    ← Projeto Unity
├── docs/                      ← Documentação (já existe)
├── scripts/
│   ├── build/
│   │   └── build_android.sh
│   ├── data/
│   │   ├── generate_words.py
│   │   ├── validate_data.py
│   │   └── format_json.py
│   └── utils/
│       └── setup_project.sh
├── .gitignore
├── .gitattributes
├── README.md
└── CHANGELOG.md
```

---

## Resumo

| Aspecto | Decisão proposta |
|---------|-----------------|
| Unity | 6.3 LTS (6000.3.8f1) |
| IDE | VS Code |
| Git | Git Flow simplificado |
| Commits | Conventional Commits |
| Build MVP | Local via Unity CLI + script |
| Build futuro | GitHub Actions (pós-MVP) |
| Dados | Python scripts offline → JSON |
| Versionamento | SemVer |
| Testes | Unit tests na Domain layer + manual |

---

## Perguntas para Decisão

1. Unity 2022 LTS ou prefere versão mais recente?
2. GitHub como repositório remoto — ok?
3. Git Flow simplificado — ok?
4. Build manual no MVP e automação depois — ok?
5. Python para pipeline de dados — ok?
6. Testes unitários apenas na Domain layer no MVP — ok?
7. Algo a ajustar?

---

## Decisão

> ✅ **Decidido em 2026-02-15**

| Aspecto | Decisão |
|---------|----------|
| Unity | **6.3 LTS (6000.3.8f1)** via Unity Hub |
| IDE | VS Code |
| Git | Git Flow simplificado |
| Commits | Conventional Commits |
| Repositório | GitHub |
| Build MVP | Local via Unity CLI + script |
| Build automatizado | Adiado — será implementado quando houver vários jogos no projeto |
| Dados | Python scripts offline → JSON |
| Versionamento | SemVer |
| Testes | Unit tests na Domain layer + manual |

---

## Próximos Passos

- [x] Criar documento organizado: `Organized/04_Pipeline.md` (DOC-004)
- [x] Atualizar Execution_Tracker
- [ ] Avançar para próximas definições pendentes
