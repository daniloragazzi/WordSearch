# 04 — Pipeline Técnico

> **Código:** DEF-005 / DOC-004
> **Status:** ✅ Concluído
> **Data:** 2026-02-15
> **Origem:** [Discussion_04](../Brainstorm/Discussion_04.md)

---

## Ambiente de Desenvolvimento

| Ferramenta | Versão / Detalhe |
|------------|-----------------|
| Unity | **6.3 LTS (6000.3.8f1)** — via Unity Hub |
| IDE | VS Code |
| Versionamento | Git |
| Repositório | GitHub |
| Scripts auxiliares | Python |
| IA | GitHub Copilot integrado |

### Extensões VS Code
- C# (Microsoft)
- Unity (Microsoft)
- GitLens
- TODO Highlight
- Markdown Preview

---

## Git Strategy

### Git Flow simplificado

```
main          ← produção (versões publicadas)
  └── develop ← desenvolvimento ativo
       ├── feature/*
       ├── hotfix/*
       └── release/*
```

### Conventional Commits

| Prefixo | Uso |
|---------|-----|
| `feat` | Nova feature |
| `fix` | Correção de bug |
| `docs` | Documentação |
| `refactor` | Refatoração |
| `chore` | Manutenção |
| `data` | Dados/conteúdo |
| `style` | Formatação |
| `test` | Testes |

---

## Build Pipeline

### MVP — Manual + scripts locais

```
[Dev local] → [Build Unity CLI] → [APK] → [Upload manual Play Store]
```

### Futuro — Automatizado (quando houver vários jogos)

```
[Push GitHub] → [GitHub Actions] → [Build Unity] → [AAB] → [Deploy Play Store]
```

> Build automatizado será implementado quando o portfólio justificar o investimento.

### Scripts locais

```
scripts/
├── build/
│   └── build_android.sh
├── data/
│   ├── generate_words.py
│   ├── validate_data.py
│   └── format_json.py
└── utils/
    └── setup_project.sh
```

---

## Pipeline de Dados

```
[Curadoria/IA] → [Python script] → [JSON validado] → [Resources/Data/]
```

### Regras de validação
- Sem duplicatas
- Sem palavras < 3 letras
- Sem caracteres especiais (apenas letras + acentos)
- Normalização de acentos para o grid
- Mínimo 50 palavras por categoria

---

## Versionamento do App

```
v[major].[minor].[patch]

v0.1.0  → Primeiro build jogável
v0.2.0  → Alpha com ads
v1.0.0  → MVP na Play Store
v1.1.0  → Primeiro update
```

---

## Testes

| Tipo | Cobertura MVP | Ferramenta |
|------|--------------|-----------|
| Unit tests | Domain layer | Unity Test Runner (NUnit) |
| Manual testing | UI, fluxo | APK no device real |
| Data validation | JSONs | Python script |

### Foco dos testes unitários
- `GridGenerator` — gera grid válido
- `WordPlacer` — posiciona palavra corretamente
- `WordFinder` — detecta seleção válida
- `LevelGenerator` — seed determinístico

---

## Estrutura do Repositório

```
WordGames/
├── Assets/              ← Projeto Unity
├── docs/                ← Documentação
├── scripts/             ← Scripts de automação
├── .gitignore
├── .gitattributes
├── README.md
└── CHANGELOG.md
```

---

## Próximos Passos

→ **DEF-006** — Naming / branding
→ **DEF-008** — Modelo de níveis
→ **DEF-010** — Sistema de analytics
