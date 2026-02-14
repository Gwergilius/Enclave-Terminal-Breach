# Password solver strategy comparison

**Summary of performance measurements for three guess-selection strategies under identical test conditions (same seeds, same word list, 18 candidates, 20 runs per difficulty).**

## Solver variants

| Variant | Description |
|--------|--------------|
| **Random** | Picks a guess uniformly at random from the candidates (no score). |
| **BestScoreOnly** | Picks a candidate with the best information score (distinct match-count values). On tie, picks randomly among the best. Matches the Excel prototype behaviour. |
| **TieBreaker** | Best score; on tie, picks the one with the **smallest worst-case bucket size** (current production implementation). |

Success is defined as **explicitly guessing the secret** (terminal response = word length) within **at most 4 steps**. Same game setup per run: 18 words of the same length (Fallout difficulty bands), one random secret in the “random secret” scenario; adversarial scenario: no fixed secret, the system always returns the response that leaves the most candidates in play.

---

## (a) Random secret

**Seed 17.** For each difficulty, 20 runs; same 20 games for all three solvers.

**Success rate (find secret in ≤4 steps):**

| Difficulty   | TieBreaker | BestScoreOnly | Random |
|-------------|------------|---------------|--------|
| Very Easy   | 90% (18/20) | 85% (17/20)   | 70% (14/20) |
| Easy        | 90% (18/20) | 85% (17/20)   | 80% (16/20) |
| Average     | 95% (19/20) | 90% (18/20)   | 90% (18/20) |
| Hard        | 100% (20/20) | 100% (20/20) | 100% (20/20) |
| Very Hard   | 100% (20/20) | 100% (20/20) | 100% (20/20) |

**Observation:** TieBreaker is best or tied on every difficulty. BestScoreOnly is close; Random is clearly worse on short words (Very Easy, Easy) and catches up on longer words where many guesses are informative.

---

## (b) Adversarial

**Seed 31.** The “system” always chooses the response that keeps the most candidates. Same 20 games per difficulty for all solvers.

**Success rate (find secret in ≤4 steps):**

| Difficulty   | TieBreaker | BestScoreOnly | Random |
|-------------|------------|---------------|--------|
| Very Easy   | 65% (13/20) | 55% (11/20)   | **15%** (3/20) |
| Easy        | 100% (20/20) | 85% (17/20)   | 50% (10/20) |
| Average     | 100% (20/20) | 100% (20/20) | 85% (17/20) |
| Hard        | 100% (20/20) | 100% (20/20) | 95% (19/20) |
| Very Hard   | 100% (20/20) | 100% (20/20) | 100% (20/20) |

**Steps when unsuccessful (need more than 4 steps):**

| Difficulty   | TieBreaker (failed runs) | BestScoreOnly (failed runs) | Random (failed runs) |
|-------------|---------------------------|-----------------------------|------------------------|
| Very Easy   | min 5, max 6, avg 5.1     | min 5, max 8, avg 5.8       | min 5, max 7, avg 5.4 |
| Easy        | —                         | min 5, max 5, avg 5.0       | min 5, max 5, avg 5.0 |
| Average     | —                         | —                           | min 5, max 5, avg 5.0 |
| Hard        | —                         | —                           | min 5, max 5, avg 5.0 |

**Observation:** Under an adversarial response, TieBreaker is best on every difficulty and the gap is large on short words (Very Easy: 65% vs 55% vs 15%). Random is very weak when the adversary can always keep the largest bucket (Very Easy 15%). BestScoreOnly sits between the two; the worst-case tie-breaker clearly helps under adversarial play.

---

## Conclusion

- **Random secret:** TieBreaker ≥ BestScoreOnly > Random; the difference is largest for short words (4–8 letters).
- **Adversarial:** TieBreaker > BestScoreOnly > Random; the tie-breaker (minimise worst-case bucket size) gives a clear advantage when the response is chosen to maximise remaining candidates.
- The **TieBreaker** strategy (current implementation) is the best of the three in both scenarios and justifies the extra logic over the Excel-style BestScoreOnly strategy.

Test implementation: `PasswordSolverAlgorithmPerformanceTests.SolverComparison_*` (same seeds: 17 for random secret, 31 for adversarial). Solver types: `TieBreakerPasswordSolver`, `BestBucketPasswordSolver`, `HouseGambitPasswordSolver` in `Enclave.Echelon.Core.Services` (Lore: ECHELON, RAVEN, SPARROW HOUSE gambit).
