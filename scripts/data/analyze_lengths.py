import json, unicodedata, sys
from pathlib import Path

def normalize(w):
    nfkd = unicodedata.normalize("NFD", w)
    without = "".join(c for c in nfkd if unicodedata.category(c) != "Mn" and c not in (" ", "-", "'"))
    return without.upper()

p = Path(r"c:\repos\unity\WordGames\WordSearch\Assets\_Project\Resources\Data\words\desafio.json")
with open(p, "r", encoding="utf-8-sig") as f:
    data = json.load(f)

words = data["words"]
by_len = {}
for w in words:
    n = len(normalize(w))
    by_len.setdefault(n, []).append(w)

print(f"Total: {len(words)} palavras")
for k in sorted(by_len.keys()):
    print(f"  {k} chars: {len(by_len[k])} palavras")
