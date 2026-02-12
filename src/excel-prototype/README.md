# Excel Prototype

**English** | [Magyar]

The project's first prototype: a password elimination solver implemented in Excel (macro-enabled workbook). It demonstrates the core algorithm before any code-based phase (SPARROW, RAVEN, etc.).

## Algorithm

The logic is described in the [Algorithm] document. In short: for each remaining password candidate, the solver computes an information score (number of distinct match-count outcomes when comparing the guess to all candidates) and chooses a guess that maximizes this score.

This Excel prototype **does not** implement the tie-breaker refinement (minimizing worst-case bucket size when multiple guesses have the same information score). Even so, it is very usable for understanding the flow and for manual play-throughs of the terminal hacking minigame.

## Contents

- **Prototype.xlsm** – Macro-enabled workbook. Open in Excel, enable macros, and follow the sheet instructions to run the solver.

## References

- [Algorithm] – Information-theoretic solver description and implementation notes

[//]: #References
[Algorithm]: ../../docs/Architecture/Algorithm.md
[Magyar]: ./README.hu.md
