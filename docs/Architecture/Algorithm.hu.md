# Algoritmus

**[English]** | Magyar

## Gyors referencia

**TL;DR:** A solver maximalizálja a különböző egyezési-szám kimenetelek számát, majd minimalizálja a legrosszabb esetű bucket méretét. Ez garantálja:
- ✅ Korrektséget: a titkos jelszó soha nem esik ki
- ✅ Konvergenciát: véges lépésben 1 jelölthez redukálódik
- ✅ Optimalitást: leggyorsabb átlagos teljesítmény

Részletek és indoklás alább.

A jelszó solver **információelméletet** használ az optimális tipp kiválasztásához:

1. Minden megmaradt jelszóhoz számítsd az „információs score”-t.
2. A score = a különböző egyezési-szám értékek száma, ha az összes többi jelszóval összehasonlítjuk.
3. Magasabb score = több információ a válaszból.
4. Válaszd a legmagasabb scoreú jelszó(ka)t.

### Használati példa (Python)

A solver a legjobb tippet javasolja és minden terminál válasz után leszűkíti a listát.

```python
candidates = ["TERMS", "TEXAS", "TIRES", "TANKS"]

best_guesses = get_best_guesses(candidates)

candidates = eliminate_by_match_count(candidates, guess="TERMS", response=2)

next_guess = get_best_guess(candidates)
```

### Implementációs váz (Python)

Logika: információ score és legrosszabb bucket méret jelöltenként; tippek, amelyek maximalizálják a score-t és holtversenyben minimalizálják a legrosszabb esetet. Minden válasz után az elimináció csak a válasszal konzisztens jelölteket tartja.

```python
def match_count(guess: str, candidate: str) -> int:
    """Pozíciók száma, ahol a guess és a candidate betűje megegyezik."""
    return sum(1 for i in range(len(guess)) if guess[i] == candidate[i])


def eliminate_by_match_count(candidates: list[str], guess: str, response: int) -> list[str]:
    """Csak azokat a C jelölteket tartsuk, amelyekre match_count(guess, C) == response."""
    return [c for c in candidates if match_count(guess, c) == response]


def calculate_information_score(guess: str, candidates: list[str]) -> tuple[int, int]:
    """
    Összehasonlítjuk a guess-t minden jelölttel (önmagával is).
    Visszaadja (score, worst_case):
      - score = különböző egyezési-szám értékek száma (magasabb jobb)
      - worst_case = legnagyobb bucket mérete (alacsonyabb jobb)
    """
    length = len(guess)
    buckets = [0] * (length + 1)
    for c in candidates:
        m = match_count(guess, c)
        buckets[m] += 1
    score = sum(1 for b in buckets if b > 0)
    worst_case = max(buckets) if buckets else 0
    return score, worst_case


def get_best_guesses(candidates: list[str]) -> list[str]:
    """Visszaadja az összes tippet, amely maximalizálja a score-t, majd minimalizálja a legrosszabb bucket méretet."""
    if not candidates:
        return []
    if len(candidates) == 1:
        return candidates.copy()

    scored = [
        (word, *calculate_information_score(word, candidates))
        for word in candidates
    ]
    scored.sort(key=lambda x: (-x[1], x[2]))
    best_score, best_worst = scored[0][1], scored[0][2]
    return [w for w, s, wc in scored if s == best_score and wc == best_worst]


def get_best_guess(candidates: list[str]) -> str | None:
    """Egy legjobb tipp, vagy None üres lista esetén."""
    best = get_best_guesses(candidates)
    return best[0] if best else None


# Példa: TERMS, TEXAS, TIRES, TANKS
candidates = ["TERMS", "TEXAS", "TIRES", "TANKS"]
print("Best guesses:", get_best_guesses(candidates))  # pl. ['TERMS', 'TIRES', 'TANKS']

# Példa: tie-breaker — DANTA, DHOBI, LILTS, OAKUM, ALEFS
print("Best guess (tie-breaker):", get_best_guesses(["DANTA", "DHOBI", "LILTS", "OAKUM", "ALEFS"]))  # ['LILTS']
```

---

## 1. Miért a „legjobb” tipp az, ami maximalizálja az információt

### Mit tanulunk egy tippből

Ha **G**-t tippeljük és a terminál **m** egyezési számot ad vissza, azt tudjuk: a titkos **S** teljesíti `matchCount(G, S) = m`. Minden **C** jelöltet **kiesztünk**, ahol `matchCount(G, C) ≠ m`. A megmaradt halmaz pontosan a válasszal konzisztens jelöltek.

A válasz csak annyit mond: melyik „bucket”-ben van a titkos (azok a **C**-k, ahol `matchCount(G, C) = m`). Minél több különböző értéket vehet fel `matchCount(G, C)`, annál több a bucket. Finomabb partíció → átlagosan kisebb megmaradt halmaz → több információ.

### Hogyan méri az algoritmus az „információt”

Minden **G** tippre: **Score(G)** = a `{ matchCount(G, C) : C ∈ candidates }` **különböző** értékeinek száma (G önmagával is; *matchCount(G, G)* = szó hossza). Magasabb score ⇒ finomabb partíció ⇒ több információ. Az algoritmus a **score-t maximalizáló** tippet választja.

### Példa 1: Egyértelmű nyertes (négy osztály)

Jelöltek: **SALES**, **SALTY**, **SAUCE**, **SAVES** (hossz 5). Minden **G**-t mindennel összehasonlítunk (G önmagával is: 5).

| Guess G | vs SALES | vs SALTY | vs SAUCE | vs SAVES | Distinct values | **Score** |
|---------|:--------:|:--------:|:--------:|:--------:|------------------|:---------:|
| SALES   | **5**    | 3        | 2        | 4        | {2, 3, 4, 5}     | **4**     |
| SALTY   | 3        | **5**    | 2        | 2        | {2, 3, 5}        | **3**     |
| SAUCE   | 2        | 2        | **5**    | 2        | {2, 5}           | **2**     |
| SAVES   | 4        | 2        | 2        | **5**    | {2, 4, 5}        | **3**     |

**SALES** az egyetlen négy különböző kimenettel (2, 3, 4, 5) → egy tipp megoldhatja. **SAUCE** csak két kimenet (2 vagy 5) → lassabb konvergencia; az algoritmus SALES-t (vagy SALTY/SAVES) preferálja.

### Példa 2: Több legjobb holtversenyben – bármelyik korrekt és konvergál

Jelöltek: **TERMS**, **TEXAS**, **TIRES**, **TANKS**. TERMS, TIRES, TANKS score **3**; TEXAS **2**. Bármelyik legjobb (score 3) garantálja a konvergenciát; minden válasznál legalább két szó kiesik.

### Példa 3: Tie-breaker a legrosszabb bucket méret alapján

Ugyanazon score mellett: minimalizáljuk a **legnagyobb bucket méretét**. Példa: **DANTA**, **DHOBI**, **LILTS**, **OAKUM**, **ALEFS**. LILTS bucket méretei 2, 2, 1 (max 2); a többié 3. Az implementáció **LILTS**-t preferálja. Teljes szabály: **maximalizáljuk a score-t**, majd **minimalizáljuk a max bucket méretet**. Lásd `resources/README.md` a szólistához és ehhez a példahalmazhoz.

---

## 2. Miért konvergálunk véges lépésben a helyes jelszóhoz

### A titkos soha nem esik ki

A válasz szerint `matchCount(G, S) = m`; **S** mindig a megmaradt halmazban van.

### A jelölthalmaz 1 elemig zsugorodik

Véges kezdőhalmaz; minden tipp után csak a válasszal konzisztens részhalmaz marad (soha nem nő). Ha legalább két jelölt van, van olyan **G**, amire `matchCount(G, C)` legalább két értéket vesz fel → a válasz szétválasztja a halmazt, tehát szigorúan zsugorodik. Legfeljebb **n − 1** lépésben 1 elem marad.

### Az egyetlen megmaradt jelölt a titkos

A végső halmaz csak a minden válasszal konzisztens jelölteket tartalmazza; **S** az egyetlen ilyen. Tehát: 1) soha nem vesszük ki a titkot, 2) véges lépésben 1 jelölt marad, 3) az a helyes jelszó. Az algoritmus garantálja a **korrektséget** és a **véges konvergenciát**.

### Konvergencia sebesség: 20 véletlen szó (5 vs 10 betű)

[resources/words.txt][words.txt]-ből 20 véletlen szó (azonos hossz), véletlen titkos, solver addig, amíg 1 jelölt marad.

**20 szó, 5 betű:** min 1, max 4, átlag **2,6** lépés; átlagos score ~3,9.  
**20 szó, 10 betű:** min 1, max 3, átlag **2,3** lépés; átlagos score ~4,7.  

A 10 betűs szavak **nem** nehezebbek: hasonló vagy kevesebb lépés, magasabb score (több pozíció → finomabb partíció).

## Hivatkozások

### Információelmélet
- Shannon (1948). [A Mathematical Theory of Communication]
- Cover & Thomas (2006). [Elements of Information Theory]
- Malone & Sullivan (2004). [Guesswork and Entropy]

### Algoritmus és játékelmélet
- Knuth (1977). [The Computer as Master Mind] – Mastermind optimális stratégia

### Játék mechanika
- [Fallout Terminal Hacking] – Fallout Wiki

### Implementáció
- [Szólista és példák][words.txt] – Projekt erőforrások

[English]: ./Algorithm.md
[words.txt]: ../../resources/words.txt
[A Mathematical Theory of Communication]: http://cm.bell-labs.com/cm/ms/what/shannonday/shannon1948.pdf
[Elements of Information Theory]: https://cs-114.org/wp-content/uploads/2015/01/Elements_of_Information_Theory_Elements.pdf
[Fallout Terminal Hacking]: https://fallout.fandom.com/wiki/Hacking
[The Computer as Master Mind]: https://www.cs.uni.edu/~wallingf/teaching/cs3530/resources/knuth-mastermind.pdf
[Guesswork and Entropy]: https://www.researchgate.net/publication/3084991_Guesswork_and_Entropy
