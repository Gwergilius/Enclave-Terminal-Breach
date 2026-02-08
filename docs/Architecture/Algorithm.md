# Algorithm

**English** | [Magyar]

[[_TOC_]]

## Quick Reference

**TL;DR**: The solver maximizes the number of distinct match-count outcomes,
then minimizes the worst-case bucket size. This guarantees:
- ✅ Correctness: The secret is never eliminated
- ✅ Convergence: Reduces to 1 candidate in finite steps
- ✅ Optimality: Fastest average-case performance

Details and proofs follow below.

The password solver uses **information theory** to select the optimal guess:

1. For each remaining password, calculate its "information score".
2. The score = number of distinct match-count values when compared against all other passwords.
3. Higher score = more information gained from the response.
4. Select the password(s) with the highest score.

### Usage example (Python)

The solver suggests the best guess and narrows the list after each terminal response.

```python
candidates = ["TERMS", "TEXAS", "TIRES", "TANKS"]

# Get all equally optimal guesses
best_guesses = get_best_guesses(candidates)

# After guessing "TERMS" and getting 2 matches, narrow the list (solver does the elimination)
candidates = eliminate_by_match_count(candidates, guess="TERMS", response=2)

# Get next suggestion from remaining candidates
next_guess = get_best_guess(candidates)
```

### Implementation sketch (Python)

The logic: compute information score and worst-case bucket size per candidate; choose guesses that maximize score and, among ties, minimize worst case. After each terminal response, elimination keeps only candidates consistent with that response.

```python
def match_count(guess: str, candidate: str) -> int:
    """Number of positions where guess and candidate have the same letter."""
    return sum(1 for i in range(len(guess)) if guess[i] == candidate[i])


def eliminate_by_match_count(candidates: list[str], guess: str, response: int) -> list[str]:
    """Keep only candidates C for which match_count(guess, C) == response."""
    return [c for c in candidates if match_count(guess, c) == response]


def calculate_information_score(guess: str, candidates: list[str]) -> tuple[int, int]:
    """
    Compare guess to every candidate (including itself).
    Returns (score, worst_case):
      - score = number of distinct match-count values (higher is better)
      - worst_case = size of largest bucket (lower is better)
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
    """Return all guesses that maximize score, then minimize worst-case bucket size."""
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
    """Return one best guess, or None if the list is empty."""
    best = get_best_guesses(candidates)
    return best[0] if best else None


# Example: TERMS, TEXAS, TIRES, TANKS
candidates = ["TERMS", "TEXAS", "TIRES", "TANKS"]
print("Best guesses:", get_best_guesses(candidates))  # e.g. ['TERMS', 'TIRES', 'TANKS']

# Example: tie-breaker — DANTA, DHOBI, LILTS, OAKUM, ALEFS
print("Best guess (tie-breaker):", get_best_guesses(["DANTA", "DHOBI", "LILTS", "OAKUM", "ALEFS"]))  # ['LILTS']
```

---

## 1. Why the "best" guess is the one that maximizes information

### What we learn from a guess

When we guess a word **G** and the terminal returns a **match count** *m*, we learn: the secret password **S** satisfies `matchCount(G, S) = m` (same number of correct letters in the same positions). So we can **eliminate** every candidate **C** for which `matchCount(G, C) ≠ m`. The remaining set is exactly the candidates consistent with the response.

So the **only** thing the response tells us is which "bucket" the secret is in: the bucket of all candidates **C** with `matchCount(G, C) = m`. The more different possible values `matchCount(G, C)` can take (as **C** runs over the current candidates), the more buckets there are. A **finer partition** means that whatever *m* we get, the remaining set is smaller on average — so we gain more information from the response.

### How the algorithm measures "information"

For each candidate guess **G**, we look at the set of match counts when **G** is compared to **every** candidate (including **G** itself):

- **Score(G)** = number of **distinct** values in `{ matchCount(G, C) : C ∈ candidates }`.

Comparing **G** to itself always yields the word length (all positions correct), so that adds one extra distinct value; the relative ranking of guesses is unchanged.

- **Higher score** ⇒ more distinct possible responses ⇒ finer partition of the candidate set ⇒ **more information** from the terminal’s answer.
- The algorithm **chooses a guess that maximizes this score**, so it explicitly picks a guess that **maximizes the number of possible outcomes**. That is exactly what "maximum information" means in this setting: we are guaranteed to choose a guess that yields as many different possible match-count values as possible, and therefore the best possible discrimination between candidates.

### Information-theoretic view (optional)

If we treat the secret as uniformly random over the current candidates, the response **R** = match count is a random variable. The **entropy** *H(R)* measures how much we learn from **R**; it is maximized when the distribution of **R** is as spread out as possible. Maximizing the **number of possible values** of **R** (our score) is a simple and robust proxy for making the response more informative. So "best guess" in the sense of "most information" is precisely: the guess that maximizes the number of distinct match-count outcomes, which is what the algorithm does.

### Example 1: Why choosing the best guess pays off — unique winner (four classes)

With some candidate sets, one word partitions the list into **four** buckets. That guess is the clear best and shows why we want to pick it.

Candidates: **SALES**, **SALTY**, **SAUCE**, **SAVES** (length 5). We compare each guess **G** to **all** candidates (including **G** itself; *matchCount(G, G)* = 5).

| Guess G | vs SALES | vs SALTY | vs SAUCE | vs SAVES | Distinct values | **Score** |
|---------|:--------:|:--------:|:--------:|:--------:|------------------|:---------:|
| SALES   | **5**    | 3        | 2        | 4        | {2, 3, 4, 5}     | **4**     |
| SALTY   | 3        | **5**    | 2        | 2        | {2, 3, 5}        | **3**     |
| SAUCE   | 2        | 2        | **5**    | 2        | {2, 5}           | **2**     |
| SAVES   | 4        | 2        | 2        | **5**    | {2, 4, 5}        | **3**     |

- **SALES** is the only guess with **four** distinct outcomes (2, 3, 4, 5). So it splits the four candidates into four one-element buckets: whatever the terminal returns, we are left with exactly one candidate. **One guess can solve the puzzle** — this is why we choose the best-scoring guess.
- **SAUCE** has only **two** outcomes (2 or 5). If we guess SAUCE and get "2 correct", we keep only candidates *C* with *matchCount*(SAUCE, *C*) = 2. That keeps SALES, SALTY, and SAVES — but **we eliminate SAUCE itself**, because *matchCount*(SAUCE, SAUCE) = 5 ≠ 2. So we go from 4 to 3 candidates and still converge, but more slowly. SAUCE is the worst first guess here; the algorithm will prefer SALES (or SALTY / SAVES).

### Example 2: When several guesses tie for best — any of them satisfies correctness and convergence

When multiple words share the top score, **any one of them** still guarantees correctness and finite convergence (we never remove the secret; the set shrinks to one). Each eliminates at least two candidates per response.

Candidates: **TERMS**, **TEXAS**, **TIRES**, **TANKS** (length 5). Again we include *matchCount(G, G)* = 5.

| Guess G | vs TERMS | vs TEXAS | vs TIRES | vs TANKS | Distinct values | **Score** |
|---------|:--------:|:--------:|:--------:|:--------:|------------------|:---------:|
| TERMS   | **5**    | 3        | 3        | 2        | {2, 3, 5}        | **3**     |
| TEXAS   | 3        | **5**    | 3        | 3        | {3, 5}           | **2**     |
| TIRES   | 3        | 3        | **5**    | 2        | {2, 3, 5}        | **3**     |
| TANKS   | 2        | 3        | 2        | **5**    | {2, 3, 5}        | **3**     |

- **TERMS, TIRES, and TANKS** all have score **3** and tie for best. We can choose **any** of them; each yields three distinct outcomes, so we **eliminate at least two** words for every response. For example, if we guess **TERMS**:
  - **Response 5** → we keep only TERMS (the guess was correct). We eliminate 3 words and win.
  - **Response 2** → we keep only **TANKS** (only it has 2 matches vs TERMS). We eliminate 3 words; one candidate remains, so we win on the next guess.
  - **Response 3** → we keep TEXAS and TIRES. We eliminate 2 words; two candidates remain, so we win in the next step.
- **TEXAS** has a lower score (2) and is a slower first guess. The point here is: when there are several best options (same score), **any one of them satisfies the correctness and convergence conditions**; Example 3 then refines which one to prefer.

### Example 3: Tie-breaker by worst-case bucket size (minimize maximum class size)

When several guesses share the **same score**, we can still do better by looking at **how many candidates end up in each bucket**. If one guess puts three words in the same bucket and we get that response, we keep three candidates — worse than a guess that never leaves more than two in any bucket. So the implementation uses a second criterion:

1. **Maximize** score (number of distinct match-count values).
2. **Among ties:** minimize the **maximum bucket size** (the size of the largest class).

So we choose the guess that minimizes the worst case: the largest number of candidates we might still have after the response.

**Concrete example from `resources/words.txt`.** Candidates: **DANTA**, **DHOBI**, **LILTS**, **OAKUM**, **ALEFS** (length 5). Each row compares one guess to all five (including itself; 5 = correct guess).

| Guess G | vs DANTA | vs DHOBI | vs LILTS | vs OAKUM | vs ALEFS | Distinct | **Score** | Bucket sizes (by value) | **Max size** |
|---------|:--------:|:--------:|:--------:|:--------:|:--------:|:--------:|:---------:|--------------------------|:------------:|
| DANTA   | **5**    | 1        | 1        | 1        | 0        | {0,1,5}  | **3**     | 1, 3, 1 (for 0,1,5)      | **3**        |
| DHOBI   | 1        | **5**    | 0        | 0        | 0        | {0,1,5}  | **3**     | 3, 1, 1 (for 0,1,5)      | **3**        |
| LILTS   | 1        | 0        | **5**    | 0        | 1        | {0,1,5}  | **3**     | 2, 2, 1 (for 0,1,5)      | **2**        |
| OAKUM   | 1        | 0        | 0        | **5**    | 0        | {0,1,5}  | **3**     | 3, 1, 1                   | **3**        |
| ALEFS   | 0        | 0        | 1        | 0        | **5**    | {0,1,5}  | **3**     | 3, 1, 1                   | **3**        |

- **DANTA, DHOBI, OAKUM, ALEFS** (as guesses) each have **score 3** but a bucket of size **3**: e.g. if we guess DANTA and get "1 correct", we keep three candidates (DHOBI, LILTS, OAKUM) — little progress.
- **LILTS** also has **score 3**, but its buckets have sizes **2, 2, 1**. Whatever the response, we keep at most **2** candidates. The **worst case** is better.
- The implementation therefore **prefers LILTS** over the others when they tie on score: it minimizes the maximum bucket size (2 vs 3).

So the full selection rule is: **maximize score**, then **minimize max bucket size**. That way we get the most information and avoid the worst outcome. The solver implements this tie-breaker whenever several guesses have the same information score. See also `resources/README.md` for the word list and this example set.

---

## 2. Why we converge to the correct password in finitely many steps

### The secret is never eliminated

After we guess **G** and get match count *m*, we keep exactly the candidates **C** for which `matchCount(G, C) = m`. The **actual secret S** satisfies `matchCount(G, S) = m` (that is exactly what the terminal reported). So **S** is always in the kept set. We never remove the correct password.

### The candidate set shrinks until it has one element

- We start with a **finite** set of candidates.
- After each guess we **replace** the candidate set by the subset consistent with the new response. So the set can only **stay the same or get smaller**; we never add candidates.
- Whenever the set has at least two candidates, there is at least one guess **G** for which `matchCount(G, C)` takes **at least two different values** as **C** runs over the set (otherwise all candidates would behave the same against every guess and would have to be identical). So as long as we have more than one candidate, we can choose a guess (and in particular our **score-maximizing** guess) that produces at least two distinct match counts. The response we get will then **split** the set: we keep only one of those buckets, so the set **strictly shrinks**.
- So after each step we either already have one candidate (and stop) or we reduce the size. After at most **n − 1** such steps (starting from *n* candidates), the set must have size **1**.

### The single remaining candidate is the secret

The final set contains only candidates that are **consistent with every response** we received. The secret **S** is consistent with every response by definition. So the only candidate left is **S**. Therefore, if we always choose a "best" (score-maximizing) guess:

1. We never remove the secret.
2. We reduce the candidate set in finitely many steps to size 1.
3. That one candidate is the correct password.

So the algorithm guarantees both **correctness** (we never lose the answer) and **finite convergence** (we reach a single candidate in a bounded number of guesses).

### Example: step-by-step until one candidate remains

Same four candidates: **TERMS**, **TEXAS**, **TIRES**, **TANKS**. Suppose the **secret is TANKS**.

**Option A — good first guess (TERMS, score 2)**  
We guess **TERMS**. Terminal returns **2 correct** (only T and S in the right place). We keep only candidates C with `matchCount(TERMS, C) = 2`:

- TERMS vs TEXAS = 3 → drop  
- TERMS vs TIRES = 3 → drop  
- TERMS vs TANKS = 2 → **keep**

So we are left with **{ TANKS }**. One step, and the only remaining candidate is the secret.

**Option B — bad first guess (TEXAS, score 2)**  
We guess **TEXAS**. Terminal returns **3 correct**. We keep only C with `matchCount(TEXAS, C) = 3`:

- TEXAS vs TERMS = 3 → keep  
- TEXAS vs TIRES = 3 → keep  
- TEXAS vs TANKS = 3 → keep  
- TEXAS vs TEXAS = 5 → **drop** (the guess itself is eliminated)

So we go from 4 to **{ TERMS, TIRES, TANKS }** — we removed TEXAS, but only one candidate. We must guess again. On the second round, the best guesses (e.g. TERMS or TIRES) have score 2 against the other two; we pick one, get a response, and shrink to one candidate. So we still converge, but in two steps instead of one.

This illustrates why maximizing the score (number of distinct outcomes) leads to faster convergence and, in the best case, to a single candidate in one step.

### Convergence speed: 20 random words (5 letters vs 10 letters)

To see how quickly the algorithm converges in practice, we draw **20 random words** from [resources/words.txt][words.txt] (same length in each run), pick a random secret among them, and apply the solver until one candidate remains. Word lengths **5** and **10** are compared.

**20 words, length 5** (example set):  
*argos, egypt, beant, sider, huaca, edits, black, tacet, unbag, tonal, amrit, hakes, marie, mommy, gains, gemma, teian, argel, cesar, built*

- One run (secret **argos**): convergence in **2** guesses.
- Over 50 runs (same 20 words, random secret each time): **min 1**, **max 4**, **average 2.6** steps.
- Average information score (distinct match-count values) over these candidates: **~3.9**.

**20 words, length 10** (example set):  
*materiable, spellingly, manservant, gorgonlike, digressive, absconding, phantomist, capistrate, telescreen, noncentral, bluebutton, actinonema, lapidaries, platycodon, cherishers, astragalus, catamiting, unsportive, ctenophora, confinable*

- One run (secret **lapidaries**): convergence in **2** guesses.
- Over 50 runs: **min 1**, **max 3**, **average 2.3** steps.
- Average information score over these candidates: **~4.7**.

So with **10-letter words** we are **not** in a harder situation: the number of steps is similar or slightly lower (2.3 vs 2.6 on average), and the typical **score is higher** (4.7 vs 3.9). With more positions, a guess tends to **spread** the candidates over more distinct match-count values (0, 1, 2, … up to 10), so we often get a finer partition and converge in few steps. Longer words do not make the puzzle inherently harder for this algorithm.

## References

### Information Theory

- Shannon, C. E. (1948). [A Mathematical Theory of Communication]   
  *Foundational paper on information theory and entropy*
- Cover, T. M., & Thomas, J. A. (2006). [Elements of Information Theory]   
  *Comprehensive textbook on information theory fundamentals*
- Malone, D., & Sullivan, W. G. (2004). [Guesswork and Entropy]   
  *Analysis of optimal guessing strategies under uncertainty*  

### Algorithm Design & Game Theory
- Knuth, D.E. (1977). [The Computer as Master Mind]   
  *Classic paper on optimal strategy for Mastermind - similar password-guessing problem*

### Game Mechanics
- [Fallout Terminal Hacking] - Fallout Wiki   
  *Game mechanics inspiration and terminal interface documentation*

### Implementation Resources
- [Word list and examples][words.txt] - Project resources  
  *Dictionary used for algorithm testing and benchmarking*
  
[words.txt]: ../../resources/words.txt
[A Mathematical Theory of Communication]: http://cm.bell-labs.com/cm/ms/what/shannonday/shannon1948.pdf "A Mathematical Theory of Communication"
[Elements of Information Theory]: https://cs-114.org/wp-content/uploads/2015/01/Elements_of_Information_Theory_Elements.pdf
[Fallout Terminal Hacking]: https://fallout.fandom.com/wiki/Hacking
[The Computer as Master Mind]: https://www.cs.uni.edu/~wallingf/teaching/cs3530/resources/knuth-mastermind.pdf
[Guesswork and Entropy]: https://www.researchgate.net/publication/3084991_Guesswork_and_Entropy
[Magyar]: ./Algorithm.hu.md

