#!/usr/bin/env python3
"""
Validador de integridade dos JSONs de palavras do Caça-Palavras.
Verifica: formato, duplicatas, tamanho mínimo, consistência com categories.json.

Uso:
    python validate_words.py
"""

import json
import os
import sys
import unicodedata
from pathlib import Path


# Caminhos relativos ao repositório
REPO_ROOT = Path(__file__).resolve().parent.parent.parent
DATA_DIR = REPO_ROOT / "WordSearch" / "Assets" / "_Project" / "Resources" / "Data"
CATEGORIES_FILE = DATA_DIR / "categories.json"
WORDS_DIR = DATA_DIR / "words"

MIN_WORD_LENGTH = 3
MIN_WORDS_PER_CATEGORY = 50
MAX_WORD_LENGTH = 11  # gridSize - 1 (12x12)


def normalize(word: str) -> str:
    """Remove acentos e converte para maiúsculas (mesmo algoritmo do C#)."""
    nfkd = unicodedata.normalize("NFD", word)
    without_accents = "".join(
        c for c in nfkd
        if unicodedata.category(c) != "Mn" and c not in (" ", "-", "'")
    )
    return unicodedata.normalize("NFC", without_accents).upper()


def load_json(filepath: Path) -> dict:
    """Carrega um arquivo JSON."""
    with open(filepath, "r", encoding="utf-8") as f:
        return json.load(f)


def validate_categories(categories_data: dict) -> list[str]:
    """Valida o arquivo categories.json."""
    errors = []

    if "categories" not in categories_data:
        errors.append("categories.json: campo 'categories' ausente")
        return errors

    cats = categories_data["categories"]
    if not isinstance(cats, list):
        errors.append("categories.json: 'categories' deve ser uma lista")
        return errors

    ids_seen = set()
    for i, cat in enumerate(cats):
        prefix = f"categories.json[{i}]"

        for field in ("id", "name", "icon"):
            if field not in cat:
                errors.append(f"{prefix}: campo '{field}' ausente")

        cat_id = cat.get("id", "")
        if cat_id in ids_seen:
            errors.append(f"{prefix}: id '{cat_id}' duplicado")
        ids_seen.add(cat_id)

    return errors


def validate_words_file(filepath: Path, expected_category_id: str) -> list[str]:
    """Valida um arquivo de palavras."""
    errors = []
    prefix = filepath.name

    try:
        data = load_json(filepath)
    except json.JSONDecodeError as e:
        errors.append(f"{prefix}: JSON inválido — {e}")
        return errors

    # Campo categoryId
    if "categoryId" not in data:
        errors.append(f"{prefix}: campo 'categoryId' ausente")
    elif data["categoryId"] != expected_category_id:
        errors.append(
            f"{prefix}: categoryId '{data['categoryId']}' != esperado '{expected_category_id}'"
        )

    # Campo words
    if "words" not in data:
        errors.append(f"{prefix}: campo 'words' ausente")
        return errors

    words = data["words"]
    if not isinstance(words, list):
        errors.append(f"{prefix}: 'words' deve ser uma lista")
        return errors

    # Contagem mínima
    if len(words) < MIN_WORDS_PER_CATEGORY:
        errors.append(
            f"{prefix}: apenas {len(words)} palavras (mínimo: {MIN_WORDS_PER_CATEGORY})"
        )

    # Validar cada palavra
    normalized_seen = set()
    for i, word in enumerate(words):
        if not isinstance(word, str):
            errors.append(f"{prefix}[{i}]: não é string — {type(word)}")
            continue

        norm = normalize(word)

        # Tamanho mínimo
        if len(norm) < MIN_WORD_LENGTH:
            errors.append(
                f"{prefix}[{i}]: '{word}' → '{norm}' tem {len(norm)} letras (mín: {MIN_WORD_LENGTH})"
            )

        # Tamanho máximo
        if len(norm) > MAX_WORD_LENGTH:
            errors.append(
                f"{prefix}[{i}]: '{word}' → '{norm}' tem {len(norm)} letras (máx: {MAX_WORD_LENGTH})"
            )

        # Duplicata (normalizada)
        if norm in normalized_seen:
            errors.append(f"{prefix}[{i}]: '{word}' → '{norm}' duplicada")
        normalized_seen.add(norm)

        # Caracteres válidos (apenas letras após normalização)
        if not norm.isalpha():
            errors.append(
                f"{prefix}[{i}]: '{word}' → '{norm}' contém caracteres inválidos"
            )

    return errors


def validate_cross_categories(words_dir: Path) -> list[str]:
    """Verifica duplicatas entre categorias."""
    errors = []
    all_words = {}  # normalized -> (category, original)

    for filepath in sorted(words_dir.glob("*.json")):
        try:
            data = load_json(filepath)
        except json.JSONDecodeError:
            continue

        cat_id = data.get("categoryId", filepath.stem)
        for word in data.get("words", []):
            norm = normalize(word)
            if norm in all_words:
                other_cat, other_word = all_words[norm]
                if other_cat != cat_id:
                    errors.append(
                        f"CROSS-DUP: '{word}' ({cat_id}) = '{other_word}' ({other_cat}) → '{norm}'"
                    )
            else:
                all_words[norm] = (cat_id, word)

    return errors


def main():
    print("=" * 60)
    print("🔍 Validador de Dados — Caça-Palavras")
    print("=" * 60)
    print()

    all_errors = []
    all_warnings = []

    # 1. Verificar existência dos arquivos
    if not CATEGORIES_FILE.exists():
        all_errors.append(f"FATAL: {CATEGORIES_FILE} não encontrado")
        print_results(all_errors, all_warnings)
        return 1

    # 2. Validar categories.json
    print("📋 Validando categories.json...")
    categories_data = load_json(CATEGORIES_FILE)
    errors = validate_categories(categories_data)
    all_errors.extend(errors)

    category_ids = [c["id"] for c in categories_data.get("categories", [])]
    print(f"   {len(category_ids)} categorias encontradas: {', '.join(category_ids)}")

    # 3. Validar cada arquivo de palavras
    total_words = 0
    for cat_id in category_ids:
        filepath = WORDS_DIR / f"{cat_id}.json"
        print(f"\n📄 Validando {cat_id}.json...")

        if not filepath.exists():
            all_errors.append(f"{cat_id}.json: arquivo não encontrado em {WORDS_DIR}")
            continue

        errors = validate_words_file(filepath, cat_id)
        all_errors.extend(errors)

        data = load_json(filepath)
        word_count = len(data.get("words", []))
        total_words += word_count
        print(f"   {word_count} palavras")

    # 4. Verificar duplicatas entre categorias
    print(f"\n🔀 Verificando duplicatas entre categorias...")
    cross_errors = validate_cross_categories(WORDS_DIR)
    # Cross-dups são warnings, não errors (podem ser intencionais)
    all_warnings.extend(cross_errors)

    # 5. Verificar arquivos órfãos (sem categoria correspondente)
    for filepath in sorted(WORDS_DIR.glob("*.json")):
        if filepath.stem not in category_ids:
            all_warnings.append(f"ORPHAN: {filepath.name} sem categoria em categories.json")

    # Resultado
    print()
    print_results(all_errors, all_warnings, total_words, len(category_ids))

    return 1 if all_errors else 0


def print_results(errors, warnings, total_words=0, total_cats=0):
    print("=" * 60)
    if errors:
        print(f"❌ {len(errors)} ERRO(S) encontrado(s):")
        for e in errors:
            print(f"   ❌ {e}")
    else:
        print("✅ Nenhum erro encontrado!")

    if warnings:
        print(f"\n⚠️  {len(warnings)} aviso(s):")
        for w in warnings:
            print(f"   ⚠️  {w}")

    if total_words > 0:
        print(f"\n📊 Resumo: {total_cats} categorias, {total_words} palavras totais")

    print("=" * 60)


if __name__ == "__main__":
    sys.exit(main())
