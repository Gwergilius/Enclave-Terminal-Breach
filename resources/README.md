# Resources

This folder holds shared resources for the **Enclave Terminal Breach** project.

## `words.txt`

- **Purpose:** Word list for the Terminal Hacking assistant (password candidate pool and solver logic).
- **Format:** One word per line, UTF-8. Words are used in **lowercase**; the application can normalize case as needed (e.g. for display, match them to the in-game terminal).
- **Content:** Large dictionary; word lengths vary. For the Fallout-style minigame, the solver typically works with words of a **fixed length** per session (e.g. 4–5 for very easy, up to 13–15 for very hard). Filter by length when loading candidates for a given puzzle.
- **Usage:** Load words of the required length for the current terminal difficulty; the password solver uses this set to compute information scores and tie-breakers (see [Algorithm](../docs/Architecture/Algorithm.md)).

### Example sets from this list

Documentation and tests use words that appear in `words.txt`, for example:

- **Examples 1 & 2** in [Algorithm.md](../docs/Architecture/Algorithm.md): `SALES`, `SALTY`, `SAUCE`, `SAVES` (unique best guess); `TERMS`, `TEXAS`, `TIRES`, `TANKS` (tied best guesses).
- **Example 3 (tie-breaker)** in [Algorithm.md](../docs/Architecture/Algorithm.md): The set **DANTA**, **DHOBI**, **LILTS**, **OAKUM**, **ALEFS** illustrates the “minimize maximum bucket size” rule: all five have score 3 as a guess, but **LILTS** has maximum bucket size 2 while the others have 3; the implementation prefers **LILTS**.

All of the above words are present in `words.txt` (case-insensitive match after normalization).
