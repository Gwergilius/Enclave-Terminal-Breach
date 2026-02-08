# Future Architecture: Password Registry and Caching

**English** | [Magyar]

This document describes a potential future architecture improvement for the Fallout Terminal Hacker application, focusing on memory efficiency and performance optimization through the Flyweight pattern and caching.

## Table of Contents

- [Current Architecture](#current-architecture)
- [Proposed Architecture](#proposed-architecture)
- [Component Details](#component-details)
  - [Password Class (Modified)](#password-class-modified)
  - [PasswordRegistry (New)](#passwordregistry-new)
  - [GameSession (Modified)](#gamesession-modified)
- [Architecture Diagram](#architecture-diagram)
- [Benefits](#benefits)
- [Performance Analysis](#performance-analysis)
- [Migration Path](#migration-path)
- [When to Implement](#when-to-implement)

---

## Current Architecture

In the current implementation:

```
GameSession 1                    GameSession 2
┌─────────────────────┐         ┌─────────────────────┐
│ Password("TERMS")   │         │ Password("TERMS")   │  ← Duplicate objects!
│ Password("TEXAS")   │         │ Password("TEXAS")   │
│ Password("TIRES")   │         │ Password("TANKS")   │
│ (IsEliminated=true) │         │ (IsEliminated=false)│
└─────────────────────┘         └─────────────────────┘
```

**Issues:**
- Each `GameSession` creates its own `Password` objects
- `IsEliminated` is stored on the `Password` object (session-specific state on shared concept)
- No caching of `GetMatchCount` results between sessions
- With a 100k word dictionary and multiple sessions, memory usage grows linearly

---

## Proposed Architecture

Apply the **Flyweight Pattern**: Store `Password` objects in a global registry and reference them from `GameSession`.

```
                    PasswordRegistry (Singleton)
                    ┌─────────────────────────────────┐
                    │ "TERMS" → Password("TERMS") ◄───┼───┐
                    │ "TEXAS" → Password("TEXAS") ◄───┼───┼───┐
                    │ "TIRES" → Password("TIRES") ◄───┼───┼───┼───┐
                    │ "TANKS" → Password("TANKS") ◄───┼───┼───┼───┼───┐
                    └─────────────────────────────────┘   │   │   │   │
                                                          │   │   │   │
        ┌─────────────────────────────────────────────────┘   │   │   │
        │                   ┌─────────────────────────────────┘   │   │
        │                   │                   ┌─────────────────┘   │
        ▼                   ▼                   ▼                     │
GameSession 1                                                         │
┌─────────────────────────────────────┐                               │
│ _passwords: [ref, ref, ref]         │ ← References, not copies      │
│ _eliminated: HashSet<Password>      │                               │
│   { Password("TERMS") }             │ ← Session-specific state      │
└─────────────────────────────────────┘                               │
                                                                      │
GameSession 2                                                         │
┌─────────────────────────────────────┐                               │
│ _passwords: [ref, ref, ref, ref]    │◄──────────────────────────────┘
│ _eliminated: HashSet<Password>      │
│   { }  (empty)                      │    Same Password object,
└─────────────────────────────────────┘    different elimination state!
```

---

## Component Details

### Password Class (Modified)

```csharp
public class Password
{
    private static readonly PasswordValidator _validator = new();
    
    // Match count cache - survives across GameSessions
    private readonly Dictionary<Password, int> _matchCountCache = new();

    /// <summary>
    /// Gets the word value of the password.
    /// </summary>
    public string Word { get; }

    // ❌ REMOVED: IsEliminated property (moved to GameSession)

    /// <summary>
    /// Initializes a new instance of the <see cref="Password"/> class.
    /// Internal constructor - use PasswordRegistry.GetOrCreate() instead.
    /// </summary>
    internal Password(string word)
    {
        _validator.ValidateAndThrowArgumentException(word, nameof(word));
        Word = word.ToUpperInvariant();
    }

    /// <summary>
    /// Calculates the number of matching characters with caching.
    /// Results are cached bidirectionally for O(1) subsequent lookups.
    /// </summary>
    public int GetMatchCount(Password other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        // Check cache first
        if (_matchCountCache.TryGetValue(other, out var cached))
        {
            return cached;
        }

        // Calculate match count
        var minLength = Math.Min(Word.Length, other.Word.Length);
        var matchCount = 0;

        for (var i = 0; i < minLength; i++)
        {
            if (Word[i] == other.Word[i])
            {
                matchCount++;
            }
        }

        // Cache bidirectionally (GetMatchCount is symmetric)
        _matchCountCache[other] = matchCount;
        other._matchCountCache[this] = matchCount;

        return matchCount;
    }

    // Subtraction operator uses cached GetMatchCount
    public static int operator -(Password? left, Password? right)
    {
        var leftLength = left?.Word.Length ?? 0;
        var rightLength = right?.Word.Length ?? 0;

        if (left is null || right is null)
        {
            return Math.Max(leftLength, rightLength);
        }

        var maxLength = Math.Max(leftLength, rightLength);
        return maxLength - left.GetMatchCount(right);
    }

    public override string ToString() => Word;
    
    public override bool Equals(object? obj) => 
        obj is Password other && Word.Equals(other.Word, StringComparison.OrdinalIgnoreCase);
    
    public override int GetHashCode() => 
        Word.GetHashCode(StringComparison.OrdinalIgnoreCase);
}
```

### PasswordRegistry (New)

```csharp
/// <summary>
/// Global registry for Password objects implementing the Flyweight pattern.
/// Ensures only one Password instance exists per unique word.
/// </summary>
public class PasswordRegistry
{
    private readonly Dictionary<string, Password> _passwords = 
        new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lock = new();

    /// <summary>
    /// Gets or creates a Password instance for the given word.
    /// Thread-safe for concurrent access.
    /// </summary>
    /// <param name="word">The word to get or create a Password for.</param>
    /// <returns>The Password instance for the word.</returns>
    public Password GetOrCreate(string word)
    {
        var key = word.ToUpperInvariant();

        lock (_lock)
        {
            if (!_passwords.TryGetValue(key, out var password))
            {
                password = new Password(word);
                _passwords[key] = password;
            }

            return password;
        }
    }

    /// <summary>
    /// Checks if a word exists in the registry.
    /// </summary>
    public bool Contains(string word) => 
        _passwords.ContainsKey(word.ToUpperInvariant());

    /// <summary>
    /// Gets the total number of registered passwords.
    /// </summary>
    public int Count => _passwords.Count;

    /// <summary>
    /// Preloads passwords from a word list file.
    /// </summary>
    /// <param name="words">The words to preload.</param>
    public void Preload(IEnumerable<string> words)
    {
        foreach (var word in words)
        {
            GetOrCreate(word);
        }
    }
}
```

### GameSession (Modified)

```csharp
/// <summary>
/// Represents a terminal hacking game session.
/// Elimination state is stored separately from Password objects.
/// </summary>
public class GameSession
{
    private static readonly GameSessionValidator _validator = new();
    private readonly List<Password> _passwords;
    private readonly HashSet<Password> _eliminated = new();

    /// <summary>
    /// Gets all passwords in this game session.
    /// </summary>
    public IReadOnlyList<Password> Passwords => _passwords.AsReadOnly();

    /// <summary>
    /// Gets all passwords that have not been eliminated.
    /// </summary>
    public IEnumerable<Password> RemainingPasswords => 
        _passwords.Where(p => !_eliminated.Contains(p));

    /// <summary>
    /// Gets the count of remaining (non-eliminated) passwords.
    /// </summary>
    public int RemainingCount => _passwords.Count - _eliminated.Count;

    /// <summary>
    /// Gets the word length for this session.
    /// </summary>
    public int WordLength { get; }

    /// <summary>
    /// Initializes a new instance using the PasswordRegistry.
    /// </summary>
    /// <param name="words">The list of potential password words.</param>
    /// <param name="registry">The password registry to use.</param>
    public GameSession(IEnumerable<string> words, PasswordRegistry registry)
    {
        _validator.ValidateAndThrowArgumentException(words, nameof(words));

        if (registry is null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        var wordList = words.ToList();
        WordLength = wordList[0].Length;
        _passwords = wordList.Select(w => registry.GetOrCreate(w)).ToList();
    }

    /// <summary>
    /// Checks if a password has been eliminated in this session.
    /// </summary>
    public bool IsEliminated(Password password) => _eliminated.Contains(password);

    /// <summary>
    /// Eliminates passwords that don't match the expected match count.
    /// </summary>
    public void EliminateByMatchCount(Password guessedPassword, int matchCount)
    {
        if (guessedPassword is null)
        {
            throw new ArgumentNullException(nameof(guessedPassword));
        }

        if (matchCount < 0 || matchCount > WordLength)
        {
            throw new ArgumentOutOfRangeException(nameof(matchCount),
                $"Match count must be between 0 and {WordLength}.");
        }

        // Eliminate passwords that don't produce the expected match count
        foreach (var password in RemainingPasswords
            .Where(p => p.GetMatchCount(guessedPassword) != matchCount))
        {
            _eliminated.Add(password);
        }

        // The guessed word was wrong
        _eliminated.Add(guessedPassword);
    }

    /// <summary>
    /// Resets the game session, clearing all eliminations.
    /// </summary>
    public void Reset() => _eliminated.Clear();
}
```

---

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                              Application                                │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌──────────────────┐     ┌──────────────────┐     ┌──────────────────┐ │
│  │  UI / ViewModel  │────▶│  PasswordSolver  │────▶│   GameSession    │ │
│  └──────────────────┘     └──────────────────┘     └────────┬─────────┘ │
│                                                             │           │
│                                                             │ references│
│                                                             ▼           │
│                           ┌──────────────────────────────────────────┐  │
│                           │           PasswordRegistry               │  │
│                           │  ┌────────────────────────────────────┐  │  │
│                           │  │     Dictionary<string, Password>   │  │  │
│                           │  │                                    │  │  │
│                           │  │  Password ──┬── _matchCountCache   │  │  │
│                           │  │             │   Dictionary<P, int> │  │  │
│                           │  │             │                      │  │  │
│                           │  │  Password ──┴── _matchCountCache   │  │  │
│                           │  │                                    │  │  │
│                           │  └────────────────────────────────────┘  │  │
│                           └──────────────────────────────────────────┘  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## Benefits

| Aspect | Current | Proposed |
|--------|---------|----------|
| **Memory per word** | N × (Password object) per session | 1 × (Password object) total |
| **GetMatchCount (first call)** | O(word_length) | O(word_length) |
| **GetMatchCount (repeated)** | O(word_length) | **O(1)** cache hit |
| **New GameSession creation** | O(n) Password allocations | O(n) dictionary lookups |
| **IsEliminated storage** | On Password (leaks across sessions) | On GameSession (isolated) |
| **Word list with 100k words** | 100k × sessions objects | 100k objects total |

---

## Performance Analysis

### Match Count Cache Efficiency

For a typical game with 12 passwords:
- Total possible comparisons: 12 × 11 / 2 = 66 unique pairs
- With cache: Each pair computed once, then O(1)
- Across multiple games with same words: **100% cache hit rate**

### Memory Savings

Assuming:
- 100,000 words in dictionary
- 10 game sessions
- Password object size: ~100 bytes

| Architecture | Memory Usage |
|--------------|--------------|
| Current | 100,000 × 10 × 100 bytes = **100 MB** |
| Proposed | 100,000 × 100 bytes = **10 MB** |

---

## Migration Path

### Phase 1: Add PasswordRegistry (Non-breaking)
1. Create `PasswordRegistry` class
2. Add constructor overload to `GameSession` that accepts registry
3. Keep existing constructor for backwards compatibility

### Phase 2: Move IsEliminated (Breaking change)
1. Remove `IsEliminated` from `Password`
2. Add `_eliminated` HashSet to `GameSession`
3. Add `IsEliminated(Password)` method to `GameSession`
4. Update all tests

### Phase 3: Add Caching
1. Add `_matchCountCache` to `Password`
2. Update `GetMatchCount` to use cache
3. Benchmark performance improvement

### Phase 4: Cleanup
1. Remove old `GameSession` constructor
2. Make `Password` constructor internal
3. Update DI registration

---

## When to Implement

**Implement this architecture when:**

- [ ] Word list exceeds 10,000 words
- [ ] Multiple game sessions are created frequently
- [ ] Performance profiling shows `GetMatchCount` as bottleneck
- [ ] Memory usage becomes a concern on mobile devices

**Current status:** Not needed yet. The application works with 10-16 passwords per game, and performance is not a concern.

---

## Related Patterns

- **Flyweight Pattern**: Share Password objects across sessions
- **Object Pool**: PasswordRegistry acts as a pool
- **Cache-Aside**: GetMatchCount caches results on first access
- **Identity Map**: One Password instance per unique word

[Magyar]: ./FutureArchitecture.hu.md

