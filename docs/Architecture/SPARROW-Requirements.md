# SPARROW Requirements

**English** | [Magyar]

## Purpose

SPARROW is the **simplest UI** to run the Core module's [Password Solver][Algorithm] implementations. It serves as a **proof of concept** to demonstrate that the algorithm works and is usable in a minimal, scriptable environment.

## Technical Constraints

- **Console application**: Black-and-white (no ANSI colour or screen positioning).
- **I/O**: Sequential input and output only, via **stdin** and **stdout**.
- **No** cursor positioning or character-level colour changes are supported.

## Version and Identity

- **Product name**: SPARROW (from the application's `Product` attribute).
- **Version**: Taken from the assembly version (e.g. `1.1.0`).

## Language

**All application output** (prompts, messages, labels) **must be in English.** Language selection or localization is out of scope for SPARROW and may be considered in a later release.

## Configuration

SPARROW supports configuration through multiple sources with the following priority (highest to lowest):

1. **Command-line arguments** (override all other sources)
2. **appsettings.json** or **appsettings.yaml** (application defaults)
3. **Built-in defaults** (fallback values)

### Command-Line Arguments

```bash
sparrow [options]

Options:
  -i, --intelligence <level>    Solver intelligence level (default: 1)
                                  0 = Random (HOUSE gambit)
                                  1 = Smart (Best-bucket)
                                  2 = Genius (Tie-breaker)
                                Aliases: house, bucket, tie
                                
  -w, --words <file>            Word list file path (optional)
                                Load candidates from file instead of manual input
                                
  -h, --help                    Show help message and exit
  -v, --version                 Show version information and exit

Examples:
  sparrow                       # Use default settings (intelligence: 1)
  sparrow -i 0                  # Use random solver (HOUSE gambit)
  sparrow -i house              # Same as above (alias)
  sparrow -i 2                  # Use optimized solver (Tie-breaker)
  sparrow -w words.txt          # Load candidates from file
  sparrow -i 2 -w words.txt     # Combine options
```

**Note:** When running with `dotnet run`, the `dotnet` CLI consumes `--help`, `-v`, etc. before the app sees them. Pass application options **after** `--` so they are forwarded to SPARROW: e.g. `dotnet run --project Enclave.Sparrow.csproj -- --help` or `dotnet run -- --help` from the project directory.

**Intelligence Level Aliases:**

| Numeric | Algorithm Name | User-Friendly | Description |
|---------|---------------|---------------|-------------|
| `0` | `house` | `dumb`, `random`, `baseline` | HOUSE gambit - random selection |
| `1` | `bucket` | `smart`, `tactical` | Best-bucket - information theory |
| `2` | `tie` | `genius`, `optimal` | Tie-breaker - optimized strategy |

### Configuration File (appsettings.json)

```json
{
  "Sparrow": {
    "Intelligence": 1,
    "WordListPath": null,
    "Startup": {
      "ShowBanner": true,
      "ShowLoadTime": true
    }
  }
}
```

### Configuration File (appsettings.yaml)

```yaml
Sparrow:
  Intelligence: 1              # 0 (HOUSE), 1 (Best-bucket), 2 (Tie-breaker)
  WordListPath: null           # Optional: path to word list file
  Startup:
    ShowBanner: true
    ShowLoadTime: true
```

**Configuration Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Intelligence` | int/string | `1` | Solver intelligence level (0-2 or alias) |
| `WordListPath` | string | `null` | Optional word list file path |
| `Startup.ShowBanner` | bool | `true` | Display startup banner |
| `Startup.ShowLoadTime` | bool | `true` | Display load time in banner |

**Notes:**
- Command-line arguments **always override** configuration file settings
- Intelligence level accepts both numeric values (0-2) and string aliases ("house", "bucket", "tie")
- Invalid values fall back to default (1 = Best-bucket)

---

## Application Flow

### 1. Startup badge

On launch, the application prints a short banner and load time:

```
SPARROW 1.1.0
Loading system profiles...537 ms
```

The version number is the application's Assembly Version; the product name is the application's ProductName attribute. The "Loading system profiles" line reports the time taken to load (e.g. in milliseconds).

### 2. Data input mode (candidate collection)

The application prompts for password candidates and accepts input until the user signals completion. Every input prompt ends with a question mark or colon.

- **Initial prompt**: `Enter password candidates:` — then wait for input.
- **Subsequent prompts**: `Enter more password candidates (empty line to finish):` — repeated after each non-empty line. The user may keep entering lines until an **empty line** is submitted.

**Input rules:**

- Each line may contain one or more words, separated by one or more spaces.
- **Word length**: The length of the first word ever accepted defines the required length. Any later word with a different length is **rejected**: an error message is printed, and input continues (the invalid word is not added).
- **Removal**: A word prefixed with `-` (e.g. `-TERMS`) removes that word from the current candidate list. The comparison is **case-insensitive** (as is all input and candidate matching).
- **Duplicates**: If a word already present in the list is entered again, it is **ignored** and a warning message is printed.

**After each successful input line:**

- Print the number of candidates currently in the list.
- Print the list of candidates in **multi-column layout**, with the number of columns derived from the word length. Words are ordered **alphabetically** (horizontal order across columns).

### 3. Hacking mode (solving loop)

After data input is finished (empty line), the application switches to hacking mode and uses the Core [solver][Algorithm]:

1. **Best guess**: Call the solver (e.g. `IPasswordSolver.GetBestGuess` or `GetBestGuesses`) with the current candidate list. Present the suggested guess to the user: e.g. `Suggested guess: \`xxxx\`.` **All input prompts** (candidates, match count, etc.) **end with `?` or `:`**.
2. **Terminal response**: Prompt for the match count reported by the Fallout terminal, e.g. `Match count? _`, and read the user's input (integer).
3. **Not won** (match count &lt; word length):
   - Call `NarrowCandidates(candidates, guess, matchCount)` to reduce the list.
   - Print the number of remaining candidates and the list (same multi-column, alphabetical format as in input mode).
   - Repeat from step 1 with the narrowed list.
4. **Won** (match count equals word length): Print a congratulations message and **exit**.
5. **Exit**: The user may terminate the application at any time (e.g. Ctrl+C).

## Core Integration

- The application depends on [Enclave.Echelon.Core][Core] and uses [IPasswordSolver][Algorithm]:
  - `GetBestGuess(candidates)` or `GetBestGuesses(candidates)` to obtain the recommended guess.
  - `NarrowCandidates(candidates, guess, matchCount)` to filter candidates after each terminal response.
- Candidate and guess types align with Core's `Password` (or equivalent) model.

## Example: full game session

The following example shows a complete session with **16 five-letter candidates**. The secret password (unknown to the user; they get match counts from the Fallout terminal) is **TIRES**. The solver suggests guesses; the user enters the match count for each guess until the terminal is cracked.

**Candidates (entered as 1 word, then 7 words, then 8 words, then empty line):**  
TERMS | TEXAS TIRES TANKS SALES SALTY SAUCE SAVES | DANTA DHOBI LILTS OAKUM ALEFS BLOCK BRAVE CHAIR.

**Secret:** TIRES → so the match counts the user enters are: for guess TERMS → 3; for guess TIRES → 5 (win).

```
SPARROW 1.1.0
Loading system profiles...537 ms

Enter password candidates:
TERMS

1 candidate(s):
TERMS

Enter more password candidates (empty line to finish):
TEXAS TIRES TANKS SALES SALTY SAUCE SAVES

8 candidate(s):
SALES  SALTY  SAUCE  SAVES
TANKS  TEXAS  TERMS  TIRES

Enter more password candidates (empty line to finish):
DANTA DHOBI LILTS OAKUM ALEFS BLOCK BRAVE CHAIR

16 candidate(s):
ALEFS  BLOCK  BRAVE  CHAIR
DANTA  DHOBI  LILTS  OAKUM
SALES  SALTY  SAUCE  SAVES
TANKS  TEXAS  TERMS  TIRES

Enter more password candidates (empty line to finish):

Suggested guess: `TERMS`
Match count? 3

1 candidate(s):
TIRES

Suggested guess: `TIRES`
Match count? 5

Correct. Terminal cracked.
```

*(After the congratulations message, the application exits.)*

## Summary

| Aspect | Requirement |
|--------|-------------|
| Role | Simplest UI; POC for Core PasswordSolver |
| I/O | Stdin/stdout only; no colours or cursor positioning |
| Identity | Product name SPARROW; version from Assembly Version |
| Language | All UI output in English; no language selection in this release |
| Configuration | Command-line args (primary), appsettings.json/yaml (fallback), built-in defaults |
| Intelligence Levels | 0 (HOUSE gambit), 1 (Best-bucket - default), 2 (Tie-breaker) |
| Input mode | Prompts for candidates; space-separated words; empty line to finish; length/duplicate/removal rules |
| Output | Candidate count + multi-column alphabetical list after each input |
| Hacking mode | GetBestGuess -> prompt match count -> NarrowCandidates -> repeat or congratulate and exit |
| Exit | Congratulations on win; Ctrl+C (or equivalent) anytime |

[Algorithm]: ./Algorithm.md
[Core]: ../../README.md
[Magyar]: ./SPARROW-Requirements.hu.md
