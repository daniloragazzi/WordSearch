"""
build_termo.py
--------------
Gera os dois bancos de palavras do Termo BR usando múltiplas fontes:

  ① ICF  (corpus de frequência — ~420 k linhas, "palavra,frequência")
  ② conjugações  (todas as formas conjugadas de verbos PT-BR, 1 por linha)
  ③ verbos       (infinitivos, 1 por linha)
  ④ pt_BR.dic    (dicionário Hunspell do LibreOffice — fallback)
  ⑤ municipios-br, paises, estados-br  (nomes próprios, aceitos como válidos)

Saída:
  words_5.json   ← palavras-alvo (conhecidas, boa frequência / qualidade curada)
  valid_5.json   ← dicionário completo (tudo aceito como palpite)

Critérios:
  • words_5 (alvos): palavras bem conhecidas — ICF freq ≤ TARGET_FREQ_MAX,
    ou presentes no Hunspell, excluindo romanos e nomes-só-próprios.
  • valid_5 (palpites): todas as fontes + plurais gerados + conjugações.

Uso:
  python scripts/data/build_termo.py
"""

import json
import re
import unicodedata
import argparse
from pathlib import Path


# ---------------------------------------------------------------------------
# Config
# ---------------------------------------------------------------------------

REPO_ROOT = Path(__file__).resolve().parents[2]
DATA_DIR = REPO_ROOT / "Data"

OUTPUT_DIR = (
    REPO_ROOT / "TermoBR" / "Termo" / "Assets" / "_Project" / "Resources" / "Data"
)
WORDS_TARGET_FILE = OUTPUT_DIR / "words_5.json"
WORDS_VALID_FILE  = OUTPUT_DIR / "valid_5.json"

# Source files
ICF_FILE        = DATA_DIR / "icf"
CONJ_FILE       = DATA_DIR / "conjugações"
VERBOS_FILE     = DATA_DIR / "verbos"
DIC_FILE        = DATA_DIR / "pt_BR.dic"
MUNICIPIOS_FILE = DATA_DIR / "municipios-br"
PAISES_FILE     = DATA_DIR / "paises"
ESTADOS_FILE    = DATA_DIR / "estados-br"

# Frequency threshold: lower value = more common.
# Words with freq <= this are considered "well-known" for targets.
TARGET_FREQ_MAX = 18.0

# Encodings to try for Hunspell .dic
ENCODINGS = ["utf-8", "iso-8859-1", "latin-1"]

# Only uppercase A-Z after normalization
VALID_CHARS = set("ABCDEFGHIJKLMNOPQRSTUVWXYZ")

# Pattern: only Portuguese letters (with accents)
LETTERS_ONLY = re.compile(r"^[a-záàâãéêíóôõúüçA-ZÁÀÂÃÉÊÍÓÔÕÚÜÇ]+$")


# ---------------------------------------------------------------------------
# Roman numeral filter
# ---------------------------------------------------------------------------

ROMAN_NUMERALS: set[str] = set()


def _build_roman_set():
    vals = [
        (1000, "M"), (900, "CM"), (500, "D"), (400, "CD"),
        (100, "C"), (90, "XC"), (50, "L"), (40, "XL"),
        (10, "X"), (9, "IX"), (5, "V"), (4, "IV"), (1, "I"),
    ]
    for num in range(1, 4000):
        roman, n = "", num
        for v, s in vals:
            while n >= v:
                roman += s
                n -= v
        if len(roman) <= 5:
            ROMAN_NUMERALS.add(roman)


_build_roman_set()


def is_roman(norm: str) -> bool:
    return norm in ROMAN_NUMERALS


# ---------------------------------------------------------------------------
# Normalization
# ---------------------------------------------------------------------------

def normalize(word: str) -> str:
    nfd = unicodedata.normalize("NFD", word)
    return "".join(c for c in nfd if unicodedata.category(c) != "Mn").upper()


def strip_flags(entry: str) -> str:
    return entry.split("/")[0].strip()


def is_pure(raw: str) -> bool:
    return bool(LETTERS_ONLY.match(raw))


def is_valid_5(n: str) -> bool:
    return len(n) == 5 and all(c in VALID_CHARS for c in n) and not is_roman(n)


def is_valid_n(n: str, length: int) -> bool:
    return len(n) == length and all(c in VALID_CHARS for c in n)


# ---------------------------------------------------------------------------
# Plural expansion (4-letter base → 5-letter plural)
# ---------------------------------------------------------------------------

def expand_plurals(base: str) -> list[str]:
    """Portuguese plural rules for a 4-letter normalized word → 5-letter forms."""
    if len(base) != 4:
        return []
    cands: list[str] = []
    if base.endswith(("AL", "EL", "OL", "UL")):
        cands.append(base[:-1] + "IS")       # oral→orais
    elif base.endswith("IL"):
        cands.append(base[:-2] + "IS")        # funil→funis (but 4→4)
        cands.append(base[:-2] + "EIS")       # fácil→fáceis
    elif base.endswith(("EM", "AM", "OM", "UM", "IM")):
        cands.append(base[:-1] + "NS")        # item→itens
    elif base.endswith("AO"):
        cands.append(base[:-2] + "OES")       # leão→leões
        cands.append(base[:-2] + "AES")       # cão→cães
        cands.append(base + "S")              # mão→mãos (MAOS)
    elif base.endswith(("R", "Z", "S")):
        cands.append(base + "ES")             # mar→mares (5 letters)
    else:
        cands.append(base + "S")              # gato→gatos
    return [c for c in cands if len(c) == 5 and all(x in VALID_CHARS for x in c)]


# ---------------------------------------------------------------------------
# Source readers
# ---------------------------------------------------------------------------

def read_icf(path: Path) -> dict[str, float]:
    """Returns {normalized_word: frequency}. Lower freq = more common."""
    result: dict[str, float] = {}
    if not path.exists():
        return result
    with path.open(encoding="utf-8") as f:
        for line in f:
            parts = line.strip().split(",")
            if len(parts) < 2:
                continue
            raw = parts[0]
            if not is_pure(raw):
                continue
            n = normalize(raw)
            if all(c in VALID_CHARS for c in n):
                freq = float(parts[1])
                if n not in result or freq < result[n]:
                    result[n] = freq
    return result


def read_wordlist(path: Path) -> set[str]:
    """Reads a file with one word per line, returns set of ALL normalized words."""
    words: set[str] = set()
    if not path.exists():
        return words
    text = None
    for enc in ENCODINGS:
        try:
            text = path.read_text(encoding=enc, errors="strict")
            break
        except (UnicodeDecodeError, ValueError):
            continue
    if text is None:
        print(f"  ⚠ Não foi possível ler {path}")
        return words

    for line in text.splitlines():
        raw = strip_flags(line).strip()
        if not raw or raw[0].isdigit():
            continue
        if not is_pure(raw):
            continue
        n = normalize(raw)
        if all(c in VALID_CHARS for c in n):
            words.add(n)
    return words


def read_names(path: Path) -> set[str]:
    """Reads names — only single-word entries without spaces/hyphens."""
    names: set[str] = set()
    if not path.exists():
        return names
    with path.open(encoding="utf-8") as f:
        for line in f:
            raw = line.strip()
            if not raw or " " in raw or "'" in raw or "-" in raw:
                continue
            if not is_pure(raw):
                continue
            n = normalize(raw)
            if all(c in VALID_CHARS for c in n):
                names.add(n)
    return names


# ---------------------------------------------------------------------------
# Main build
# ---------------------------------------------------------------------------

def build_sets() -> tuple[list[str], list[str]]:
    # ── 1. Read all sources ──────────────────────────────────────────

    print("  [1/6] Lendo ICF (corpus de frequência)...")
    icf = read_icf(ICF_FILE)
    icf_5 = {w for w in icf if is_valid_5(w)}
    icf_4 = {w for w in icf if is_valid_n(w, 4)}
    print(f"         ICF total: {len(icf):,}  |  5-letras: {len(icf_5):,}  |  4-letras: {len(icf_4):,}")

    print("  [2/6] Lendo conjugações...")
    conj_all = read_wordlist(CONJ_FILE)
    conj_5 = {w for w in conj_all if is_valid_5(w)}
    print(f"         Conjugações total: {len(conj_all):,}  |  5-letras: {len(conj_5):,}")

    print("  [3/6] Lendo verbos (infinitivos)...")
    verbos_all = read_wordlist(VERBOS_FILE)
    verbos_5 = {w for w in verbos_all if is_valid_5(w)}
    print(f"         Verbos total: {len(verbos_all):,}  |  5-letras: {len(verbos_5):,}")

    print("  [4/6] Lendo pt_BR.dic (Hunspell)...")
    dic_all = read_wordlist(DIC_FILE)
    dic_5 = {w for w in dic_all if is_valid_5(w)}
    dic_4 = {w for w in dic_all if is_valid_n(w, 4)}
    print(f"         Hunspell total: {len(dic_all):,}  |  5-letras: {len(dic_5):,}  |  4-letras: {len(dic_4):,}")

    print("  [5/6] Lendo nomes (municípios, países, estados)...")
    names = set()
    for p in [MUNICIPIOS_FILE, PAISES_FILE, ESTADOS_FILE]:
        n = read_names(p)
        names |= n
    names_5 = {w for w in names if is_valid_5(w)}
    print(f"         Nomes 5-letras: {len(names_5):,}")

    # ── 2. Generate plurals from all 4-letter bases ──────────────────

    print("  [6/6] Gerando plurais...")
    all_bases_4 = icf_4 | dic_4
    # Also extract implicit 4-letter bases from longer words
    for w in icf:
        if len(w) >= 5 and len(w) <= 8 and w[3] in "AEIOU":
            all_bases_4.add(w[:4])
    for w in dic_all:
        if len(w) >= 5 and len(w) <= 8 and w[3] in "AEIOU":
            all_bases_4.add(w[:4])

    plurals: set[str] = set()
    for base in all_bases_4:
        for p in expand_plurals(base):
            if not is_roman(p):
                plurals.add(p)
    print(f"         Bases de 4 letras: {len(all_bases_4):,}  |  Plurais gerados: {len(plurals):,}")

    # ── 3. Build valid_5 (all accepted guesses) ─────────────────────

    valid_set = icf_5 | conj_5 | dic_5 | names_5 | plurals | verbos_5
    valid_set = {w for w in valid_set if not is_roman(w)}

    # ── 4. Build words_5 (target words — curated quality) ───────────

    target_set: set[str] = set()

    # 4a. ICF words with good frequency (well-known)
    icf_targets = {w for w in icf_5 if icf[w] <= TARGET_FREQ_MAX}
    target_set |= icf_targets

    # 4b. Hunspell 5-letter words (already curated by dictionary maintainers)
    target_set |= dic_5

    # 4c. 5-letter infinitives from verbos list
    target_set |= verbos_5

    # 4d. Plurals: confirmed by ICF directly, OR whose singular base is in ICF
    icf_confirmed_plurals = plurals & set(icf.keys())
    # Also: if the 4-letter base is in ICF with good freq, accept the plural as target
    base_confirmed_plurals: set[str] = set()
    for base in all_bases_4:
        if base in icf and icf[base] <= TARGET_FREQ_MAX:
            for p in expand_plurals(base):
                if not is_roman(p):
                    base_confirmed_plurals.add(p)
    target_set |= icf_confirmed_plurals | base_confirmed_plurals

    # 4e. Conjugations that are confirmed by ICF with good frequency
    icf_confirmed_conj = {w for w in conj_5 if w in icf and icf[w] <= TARGET_FREQ_MAX}
    target_set |= icf_confirmed_conj

    # 4f. Exclude names that are ONLY proper nouns (not common words)
    proper_only = names_5 - icf_5 - dic_5
    target_set -= proper_only

    # 4e. Final roman filter
    target_set = {w for w in target_set if not is_roman(w)}

    # 4f. All targets must be valid
    valid_set |= target_set

    # ── 5. Stats ─────────────────────────────────────────────────────

    only_icf  = icf_5 - dic_5 - conj_5 - plurals
    only_dic  = dic_5 - icf_5 - conj_5 - plurals
    only_conj = conj_5 - icf_5 - dic_5 - plurals
    only_plur = plurals - icf_5 - dic_5 - conj_5

    print()
    print(f"  ── Resumo ──────────────────────────────────")
    print(f"  ICF 5-letras              : {len(icf_5):>7,}")
    print(f"  Conjugações 5-letras      : {len(conj_5):>7,}")
    print(f"  Hunspell 5-letras         : {len(dic_5):>7,}")
    print(f"  Verbos 5-letras           : {len(verbos_5):>7,}")
    print(f"  Nomes 5-letras            : {len(names_5):>7,}")
    print(f"  Plurais gerados           : {len(plurals):>7,}")
    print(f"  ─────────────────────────────────────────────")
    print(f"  Exclusivos ICF            : {len(only_icf):>7,}")
    print(f"  Exclusivos Hunspell       : {len(only_dic):>7,}")
    print(f"  Exclusivos Conjugações    : {len(only_conj):>7,}")
    print(f"  Exclusivos Plurais        : {len(only_plur):>7,}")
    print(f"  ─────────────────────────────────────────────")
    print(f"  Total valid_5             : {len(valid_set):>7,}")
    print(f"  Total words_5 (alvos)     : {len(target_set):>7,}")

    return sorted(target_set), sorted(valid_set)


def write_json(path: Path, words: list[str]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(
        json.dumps({"words": words}, ensure_ascii=False, indent=2),
        encoding="utf-8",
    )


# ---------------------------------------------------------------------------
# Verification
# ---------------------------------------------------------------------------

def verify(targets: list[str], valid: list[str]):
    target_set = set(targets)
    valid_set = set(valid)

    print()
    print("  ── Verificação de palavras válidas ─────────")
    must_valid = {
        "MUITO": "ICF top", "SOBRE": "ICF top", "MUNDO": "ICF top",
        "TEMPO": "ICF top", "FORMA": "ICF top", "PARTE": "ICF top",
        "GATOS": "plural de GATO", "CASAS": "plural de CASA",
        "RAIOS": "plural de RAIO", "JOGOS": "plural de JOGO",
        "LUTEM": "conjugação", "LUTAM": "conjugação",
        "LUTEI": "conjugação", "LUTOU": "conjugação",
        "COMEM": "conjugação", "ABRIR": "verbo",
        "ANDAR": "verbo", "FAZER": "verbo",
        "BAHIA": "estado", "CHILE": "país",
        "CHINA": "país", "EGITO": "país",
        "MESMO": "ICF", "AINDA": "ICF",
    }
    ok, fail = 0, 0
    for w, desc in must_valid.items():
        if w in valid_set:
            ok += 1
        else:
            fail += 1
            print(f"    ✗ {w} ({desc}) AUSENTE no valid_5!")
    print(f"    {ok}/{ok+fail} palavras de verificação presentes no valid_5")

    print()
    print("  ── Verificação de alvos ────────────────────")
    must_target = ["MUITO", "SOBRE", "MUNDO", "TEMPO", "FORMA", "PARTE", "MESMO", "FAZER", "ABRIR"]
    ok, fail = 0, 0
    for w in must_target:
        if w in target_set:
            ok += 1
        else:
            fail += 1
            print(f"    ✗ {w} AUSENTE no words_5!")
    print(f"    {ok}/{ok+fail} palavras de verificação presentes no words_5")

    print()
    print("  ── Verificação de romanos excluídos ────────")
    romans = ["XVIII", "XLVII", "MCMXL", "DCCLI", "LXIII"]
    for r in romans:
        if len(r) == 5:
            status = "✓ excluído" if r not in valid_set else "✗ PRESENTE (BUG!)"
            print(f"    {r}: {status}")

    print()
    print("  ── Verificação de plurais ──────────────────")
    must_plurals = {
        "GATOS": "GATO+S", "CASAS": "CASA+S", "LIVROS": "6 letras (não gera)",
        "PAPEIS": "PAPEL→PAPEIS", "RAIOS": "RAIO+S", "JOGOS": "JOGO+S",
        "LEOES": "LEAO→OES", "MARES": "MAR+ES", "LUZES": "LUZ+ES",
        "ATUNS": "ATUM→NS",
    }
    for w, desc in must_plurals.items():
        if len(w) != 5:
            continue
        status = "✓" if w in valid_set else "✗"
        print(f"    {w} ({desc}): {status}")


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main() -> None:
    parser = argparse.ArgumentParser(description="Gera bancos de palavras para o Termo BR.")
    args = parser.parse_args()

    print(f"\n{'='*60}")
    print(f"  build_termo.py — Banco de palavras Termo BR (v2)")
    print(f"{'='*60}")
    print(f"  Fontes   : ICF, conjugações, verbos, pt_BR.dic")
    print(f"             municípios, países, estados")
    print(f"  Saída    : {OUTPUT_DIR}")
    print()

    targets, valid = build_sets()
    print()

    print("→ Gravando words_5.json (palavras-alvo)...")
    write_json(WORDS_TARGET_FILE, targets)
    print(f"  {WORDS_TARGET_FILE}")

    print("→ Gravando valid_5.json (dicionário completo)...")
    write_json(WORDS_VALID_FILE, valid)
    print(f"  {WORDS_VALID_FILE}")

    verify(targets, valid)

    print()
    print(f"  Amostra words_5 : {targets[:10]}")
    print(f"  Amostra valid_5 : {[w for w in valid if w not in set(targets)][:10]}")
    print()
    print("✓ Concluído.")
    print()


if __name__ == "__main__":
    main()
