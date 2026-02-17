"""Build desafio.json with minimum word counts per normalized length."""
import json, unicodedata
from pathlib import Path

def normalize(w):
    nfkd = unicodedata.normalize("NFD", w)
    without = "".join(c for c in nfkd if unicodedata.category(c) != "Mn" and c not in (" ", "-", "'"))
    return without.upper()

# Keep all existing words
existing = [
    "casa","porta","janela","telhado","escada","jardim","garagem","varanda","cozinha","quarto",
    "sala","banheiro","sofá","mesa","cadeira","cama","espelho","tapete","cortina","lustre",
    "chave","relógio","telefone","celular","computador","teclado","monitor","mouse","impressora","câmera",
    "livro","caderno","lápis","caneta","borracha","régua","tesoura","cola","pincel","tinta",
    "carro","ônibus","trem","avião","navio","bicicleta","moto","caminhão","helicóptero","foguete",
    "escola","faculdade","hospital","farmácia","mercado","padaria","banco","cinema","teatro","museu",
    "música","dança","pintura","escultura","poesia","romance","comédia","drama","ficção","mistério",
    "amor","paz","alegria","coragem","verdade","justiça","liberdade","respeito","bondade","honra",
    "tempo","memória","sonho","destino","futuro","passado","momento","instante","eternidade","saudade",
    "viagem","aventura","caminho","trilha","mapa","bússola","mochila","barraca","fogueira","lanterna",
    "martelo","enxada","parafuso","prego","serra","furadeira","alicate","serrote","prumo","nível",
    "camisa","calça","sapato","chapéu","luva","cachecol","vestido","saia","gravata","cinto",
]

# New words organized by normalized length
new_3 = [
    "asa","céu","chá","dia","dor","elo","fio","gás","gel","lei",
    "lar","mal","mar","mel","mês","nau","noz","oco","pio","pó",
    "réu","rés","rio","rol","rua","rés","tom","vão","véu","vez",
    "voz","dom","cem","fim","gol","giz","jus","lar","ler","luz",
]

new_4 = [
    "arma","asno","bola","bote","café","dado","dose","faca","fada","fila",
    "goma","hino","jato","jogo","lata","lima","lodo","mala","mina","nota",
    "ouro","pena","poço","ramo","sede","silo","taco","trio","tubo","urna",
    "vaga","vela","vila","zona","arco","beco","cabo","dedo","fogo","gelo",
]

new_5 = [
    "abalo","aceno","acervo","adega","ajuda","algoz","amigo","andar","apoio","apito",
    "atlas","aviso","baixo","balão","barco","bicho","bloco","brasa","brejo","brisa",
    "burro","cabra","campo","capim","casco","cesta","cofre","colar","copos","corda",
    "couro","crise","crivo","dardo","debate","decor","degrau","dieta","disco","dobra",
    "duelo","dupla","email","envio","ervas","exame","falha","farol","fibra","fluxo",
    "forca","fosso","frase","freio","fruto","ganho","garfo","garra","globo","grade",
    "grife","grupo","igual","inato","janta","lacre","laudo","lenço","lista","malha",
    "manga","moeda","molde","nervo","ninho","óbice","pacto","palco","pasta","pedra",
    "pente","peças","pilha","placa","polia","posse","praga","prazo","preço","prima",
    "prova","pulso","quilo","radar","raiva","rasto","risco","ritmo","rolar","salão",
    "saque","senso","sinal","sopro","surdo","surto","susto","tigre","toldo","toque",
    "tosse","traço","traje","treco","tribo","troca","turma","único","usina","valsa",
]

new_6 = [
    "ablução","abismo","açúcar","adubo","alarme","aliado","âncora",
    "anzol","arquivo","ataque","atoleiro","balcão",
]

new_8 = [
    "abandono","abertura","acabamento","acampamento","acessório","armazém",
    "bandeira","barreira","bastidor","cabeceira","canteiro","carimbo",
    "castanha","catedral","clareira","clausura","cobertor","concreto",
    "corredor","cozinheiro","cruzeiro","curvatura",
]

new_9 = [
    "abordagem","aeroporto","almofadas","amplitude","aquarismo",
    "artefatos","assinalar","barbearia","cafeteira","canalizar",
    "carrossel","cativeiro","concretos","congelar","construir",
    "cruzeiros","culinaria","depositar","descansar","diferencas",
    "eletrodos","empacotam","encontrar","equipagem","estampar",
    "fabricado","fermentar","formatura","frigobar","guardados",
    "horizonte","iluminado","imobiliario","indicador","joalheiro",
    "kilometro","labirinto","luminaria","mergulhar","nordestino",
]

new_10 = [
    # These all normalize to exactly 10 characters
    "abandonado","acabamento","barracudas","cavaleiros","consolidar",
    "engrenagem","admiradora","adulterado","alfinetes","alternador",
    "analisando","aplaudindo","aproveitam","argumentar","assobiando",
    "bastonadas","bloqueando","bombardear","brincadeira","calculando",
    "camaronesa","cambalhotas","canalizador","caprichoso","carregando",
    "castiçal","catapultas","chamuscado","chaparralho","cintilando",
    "completado","congelando","conhecedor","cozinhando","decolagens",
    "deformados","degradados","desafiador","designando","desligando",
    "despedidas","duplicados",
]

# Combine all
all_words = list(existing)
for batch in [new_3, new_4, new_5, new_6, new_8, new_9, new_10]:
    all_words.extend(batch)

# Deduplicate by normalized form
seen = set()
unique = []
for w in all_words:
    n = normalize(w)
    if n not in seen and len(n) >= 3:
        seen.add(n)
        unique.append(w)

# Count by length
by_len = {}
for w in unique:
    n = len(normalize(w))
    by_len.setdefault(n, []).append(w)

print(f"Total unique: {len(unique)}")
targets = {3:30, 4:30, 5:100, 6:30, 7:30, 8:30, 9:30, 10:30}
for k in sorted(by_len.keys()):
    t = targets.get(k, 0)
    gap = max(0, t - len(by_len[k]))
    mark = f" *** NEED {gap} MORE ***" if gap > 0 else " OK"
    print(f"  {k} chars: {len(by_len[k])}{mark}")
    if gap > 0 or k in (10, 11):
        for w in by_len[k]:
            print(f"    {w} -> {normalize(w)} ({len(normalize(w))})")

# Write
out = {"categoryId": "desafio", "words": unique}
p = Path(r"c:\repos\unity\WordGames\WordSearch\Assets\_Project\Resources\Data\words\desafio.json")
with open(p, "w", encoding="utf-8") as f:
    json.dump(out, f, ensure_ascii=False, indent=4)
print(f"\nWrote {len(unique)} words to desafio.json")
