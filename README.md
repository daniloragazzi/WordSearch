# WordGames — Ragazzi Studios

> Ecossistema de jogos casuais Android (word games, puzzles, quizzes)

## Sobre

Projeto de estúdio indie digital focado em jogos casuais para Android, com engine base reutilizável para produção em série.

## Primeiro Jogo: Caça-Palavras

- **Package:** `com.ragazzistudios.wordsearch`
- **Engine:** Unity 6.3 LTS (6000.3.8f1)
- **Plataforma:** Android
- **Idioma:** Português (BR) — arquitetura multi-idioma

## Estrutura

```
WordGames/
├── docs/                  ← Documentação do projeto
│   ├── Brainstorm/        ← Discussões e brainstorm
│   └── Organized/         ← Documentos formais
├── scripts/               ← Scripts de automação (Python, shell)
│   ├── build/
│   ├── data/
│   └── utils/
└── WordSearch/            ← Projeto Unity
    └── Assets/
        ├── _Project/
        │   ├── Core/      ← Engine reutilizável
        │   ├── Game/      ← Específico deste jogo
        │   ├── Resources/ ← Dados (JSONs)
        │   ├── Scenes/    ← Cenas Unity
        │   ├── Art/       ← Assets visuais
        │   └── Audio/     ← Assets de áudio
        └── Plugins/       ← SDKs de terceiros
```

## Stack

- Unity 6.3 LTS
- VS Code
- Git + GitHub
- Python (scripts de dados)
- Google AdMob
- Unity Analytics

## Documentação

Consulte `docs/Organized/` para documentação completa do projeto.

## Licença

Proprietary — Ragazzi Studios © 2026
