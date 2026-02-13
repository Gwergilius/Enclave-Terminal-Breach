# Enclave.Echelon.Core

Core business logic for Enclave Terminal Breach: password solver algorithm, domain models, and shared validation. Used by terminal/UIs and tests.

## Contents

| Folder / area | Description |
|---------------|-------------|
| **Models/** | Domain types: `Password` (word + match-count cache), `ScoreInfo` (information score and worst-case bucket for a guess). |
| **Services/** | `IPasswordSolver` and implementations: `PasswordSolver` (information score + tie-breaker), `BestScoreOnlySolver`, `RandomGuessSolver`. |
| **Validators/** | FluentValidation validators (e.g. `PasswordValidator` for word format). |
| **Extensions/** | Shared extensions (e.g. validation helpers). |
| **Resources/** | Embedded word list (`words.txt`, 4–15 letters) for candidate generation and tests. |

## Password solver

The solver picks the next guess to maximise information (see [docs/Architecture/Algorithm.md](../../docs/Architecture/Algorithm.md)):

- **Information score** – Number of distinct match-count values when comparing a candidate to all others (higher = better).
- **Tie-breaker** – Among best-scoring candidates, choose the one with the **smallest worst-case bucket size** (minimax).

Main API:

- `GetBestGuess(candidates)` – One recommended guess, or `null` if none.
- `GetBestGuesses(candidates)` – All guesses with the best score (and best worst-case when using `PasswordSolver`).
- `CalculateInformationScore(password, candidates)` – Score and worst-case for a given guess.
- `NarrowCandidates(candidates, guess, matchCount)` – Restrict candidates to those consistent with the terminal response.

Implementations:

- **PasswordSolver** – Production: best score, then tie-break by worst-case bucket.
- **BestScoreOnlySolver** – Best score only; ties broken randomly (Excel-prototype style).
- **RandomGuessSolver** – Random choice among candidates (for comparison tests).

## Dependencies

- **Enclave.Common** – Resource loading (e.g. word list), FluentResults.
- **FluentValidation** – Validation for `Password` and related types.

## Tests

- **Enclave.Echelon.Core.Tests** – Unit tests (`PasswordSolverTests`, `PasswordValidatorTests`, etc.) and performance/convergence tests (`PasswordSolverAlgorithmPerformanceTests`) for random secret and adversarial scenarios.

## See also

- [Algorithm](../../docs/Architecture/Algorithm.md) – Solver design and examples
- [SolverComparison](../../docs/Architecture/SolverComparison.md) – Strategy comparison (TieBreaker vs BestScoreOnly vs Random)
- [Source root README](../README.md) – folder structure and solution
