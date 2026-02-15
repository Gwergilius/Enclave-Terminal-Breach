# Future Architecture: Password Registry and Caching

**English** | [Magyar]

This document describes a potential future architecture improvement for the Fallout Terminal Hacker application, focusing on memory efficiency and performance optimization through the Flyweight pattern and caching.

## Table of Contents

- [Current Architecture][anchor-current]
- [Proposed Architecture][anchor-proposed]
- [Component Details][anchor-details]
  - [Password Class (Modified)][anchor-password]
  - [PasswordRegistry (New)][anchor-registry]
  - [GameSession (Modified)][anchor-gamesession]
- [Architecture Diagram][anchor-arch-diagram]
- [Benefits][anchor-benefits]
- [Performance Analysis][anchor-perf]
- [Migration Path][anchor-migration]
- [When to Implement][anchor-when]
- [Future UI/UX Architecture][anchor-uiux]
  - [Information Architecture][anchor-info-arch]
  - [Additional Feature Concepts][anchor-features]
  - [Phased Implementation Roadmap][anchor-roadmap]

---

## Current Architecture

In the current implementation:

![Current Architecture][img-current]

Diagram source: [FutureArchitecture-CurrentArchitecture.drawio][src-current]. Other formats: [PlantUML][src-current-puml], [Mermaid][src-current-mmd], [DOT][src-current-gv].

**Issues:**
- Each `GameSession` creates its own `Password` objects
- `IsEliminated` is stored on the `Password` object (session-specific state on shared concept)
- No caching of `GetMatchCount` results between sessions
- With a 100k word dictionary and multiple sessions, memory usage grows linearly

---

## Proposed Architecture

Apply the **Flyweight Pattern**: Store `Password` objects in a global registry and reference them from `GameSession`.

![Proposed Architecture][img-proposed]

Diagram source: [FutureArchitecture-ProposedArchitecture.drawio][src-proposed]. Other formats: [PlantUML][src-proposed-puml], [Mermaid][src-proposed-mmd], [DOT][src-proposed-gv].

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

    // âŒ REMOVED: IsEliminated property (moved to GameSession)

    /// <summary>
    /// Initializes a new instance of the <see cref="Password"/> class.
    /// Internal constructor - use PasswordRegistry.GetOrCreate() instead.
    /// </summary>
    internal Password(string word)
    {
        _validator.ValidateAndThrowArgumentException(word);
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
        _validator.ValidateAndThrowArgumentException(words);

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

![Architecture Diagram][img-arch]

Diagram source: [FutureArchitecture.drawio][src-arch].

---

## Benefits

| Aspect | Current | Proposed |
|--------|---------|----------|
| **Memory per word** | N Ã— (Password object) per session | 1 Ã— (Password object) total |
| **GetMatchCount (first call)** | O(word_length) | O(word_length) |
| **GetMatchCount (repeated)** | O(word_length) | **O(1)** cache hit |
| **New GameSession creation** | O(n) Password allocations | O(n) dictionary lookups |
| **IsEliminated storage** | On Password (leaks across sessions) | On GameSession (isolated) |
| **Word list with 100k words** | 100k Ã— sessions objects | 100k objects total |

---

## Performance Analysis

### Match Count Cache Efficiency

Match counts are cached bidirectionally per pair: the **first** call for a pair (in either order) costs **O(word length)** and stores the result in both passwordsâ€™ caches; every **subsequent** call for that pair is **O(1)** from cache.

For a typical game with 12 passwords:
- Total possible comparisons: 12 Ã— 11 / 2 = 66 unique pairs
- With cache: Each pair computed once, then O(1) for all later lookups
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
- **[Flyweight Pattern]**: Share Password objects across sessions 
- **[Object Pool]**: PasswordRegistry acts as a pool 
- **[Cache-Aside]**: GetMatchCount caches results on first access 
- **[Identity Map]**: One Password instance per unique word 

---

## Future UI/UX Architecture

This section outlines potential future enhancements for the user interface, documentation, and community features.

### Information Architecture

**Goal:** Provide multiple access points to lore, documentation, and help content to serve different user needs and contexts.

#### A) Dedicated Website (Marketing + Lore Hub)

**Purpose:** Public-facing portal for discovery, documentation, and community building.

**Advantages:**
- **SEO & Discovery:** Users can find the project through search engines
- **Portfolio Showcase:** Demonstrates full-stack development capabilities
- **Deep Dive Content:** Long-form technical articles and detailed lore
- **Community Building:** Comment sections, Discord/Reddit integration
- **Shareable:** Individual pages can be linked and shared

**Proposed Structure:**

```
enclave-echelon.com (or fallout-terminal-hacker.com)
│
├── Home
│   ├── Hero Section: "ECHELON Terminal Breach System"
│   ├── Quick Pitch: What is this application?
│   ├── Screenshots & Demo Video
│   └── Download Links (Google Play, Web App, GitHub)
│
├── Lore (Enclave Eyes Only)
│   ├── Project History (complete narrative)
│   ├── PHOSPHOR Technical Overview
│   ├── Character Profiles
│   │   ├── Dr. Elizabeth Krane
│   │   ├── Dr. Marcus Aldridge
│   │   └── Col. Augustus Autumn Sr.
│   ├── Interactive Timeline
│   │   └── Visual timeline from 2076 to 2287
│   └── Classified Documents
│       ├── Boot Sequence Documentation
│       ├── Component Architecture
│       └── Version History
│
├── Documentation
│   ├── Getting Started Guide
│   ├── How to Use ECHELON
│   ├── Algorithm Explained (simplified Algorithm.md)
│   ├── Fallout Terminal Hacking Guide
│   ├── Solver Strategy Guide
│   └── FAQ
│
├── Development Blog
│   ├── "Why I Built This"
│   ├── "PHOSPHOR Architecture Deep-Dive"
│   ├── "Syncing Fiction with Code Evolution"
│   ├── "From SPARROW to ECHELON: A Development Journey"
│   └── Release Notes & Changelogs
│
└── Download / Links
    ├── Google Play Store
    ├── Web Application
    ├── GitHub Repository (if open source)
    └── Community Links (Discord, Reddit)
```

**Technical Stack Suggestions:**
- **Static Site Generator:** Hugo, Astro, or Jekyll
- **Styling:** Custom Pip-Boy/Terminal aesthetic with green phosphor theme
- **Hosting:** GitHub Pages, Netlify, or Vercel (free tier)
- **Interactive Elements:** JavaScript for timeline, document viewer

#### B) In-App "Civilopedia" (Integrated Help System)

**Purpose:** Immersive, offline-accessible documentation within the application itself.

**Advantages:**
- **Immersion:** Maintains Pip-Boy aesthetic without breaking the experience
- **Offline Access:** All content available without internet connection
- **Context-Aware:** Tooltips and inline help linked to current screen
- **Progressive Disclosure:** Show only relevant information at the right time
- **No External Dependencies:** Self-contained experience

**Proposed Structure (Pip-Boy Terminal Aesthetic):**

```
╔══════════════════════════════════════════╗
║ ECHELON DATABASE - OMEGA-7 ACCESS        ║
║ [CLASSIFIED - AUTHORIZED PERSONNEL ONLY] ║
╚══════════════════════════════════════════╝

┌─ SYSTEMS ────────────────────────────────┐
│ > Project History                        │
│ > PHOSPHOR Terminal Emulator             │
│ > Component Architecture                 │
│   ├─ SIGINT Console                      │
│   ├─ NEXUS Connection                    │
│   └─ BLACKBIRD Stealth                   │
│ > Version History Timeline               │
│ > Boot Sequence Documentation            │
└──────────────────────────────────────────┘

┌─ OPERATIONS ─────────────────────────────┐
│ > Quick Start Guide                      │
│ > Terminal Hacking Tutorial              │
│ > Solver Strategy Breakdown              │
│   ├─ HOUSE Gambit (Random)               │
│   ├─ Best-Bucket Strategy                │
│   └─ Tie-Breaker Optimization            │
│ > Troubleshooting & FAQ                  │
│ > Keyboard Shortcuts Reference           │
└──────────────────────────────────────────┘

┌─ PERSONNEL ──────────────────────────────┐
│ > Dr. Elizabeth Krane (1952-2078)        │
│ > Dr. Marcus Aldridge (2055-?)           │
│ > Col. Augustus Autumn Sr.               │
│ > Robert House (Adversary Profile)       │
│ > Agent "Gwergilius" (1961-?)            │
│   └─ [CLEARANCE INSUFFICIENT]            │
└──────────────────────────────────────────┘

┌─ INTELLIGENCE ───────────────────────────┐
│ > RobCo UOS Vulnerabilities              │
│ > Fallout Terminal Mechanics             │
│ > NX-Series Terminal Specifications      │
│ > Brotherhood Countermeasures            │
│ > Institute Terminal Analysis            │
└──────────────────────────────────────────┘

┌─ SETTINGS ───────────────────────────────┐
│ > Color Palette Selection                │
│   ├─ Green Phosphor (Classic)            │
│   ├─ Amber Phosphor                      │
│   ├─ White Phosphor                      │
│   └─ Blue Phosphor                       │
│ > Audio Settings                         │
│ > Boot Sequence (Enable/Disable)         │
│ > Advanced Options                       │
└──────────────────────────────────────────┘
```

**Implementation Notes:**
- Markdown files embedded as assembly resources
- Custom Markdown renderer with Pip-Boy styling
- Searchable database with keyword indexing
- Bookmark/favorite document feature
- "Recently Viewed" history

**Example Code Structure:**

```csharp
public class LoreDatabase
{
    private readonly Dictionary<string, LoreDocument> _documents;
    
    public LoreDocument GetDocument(string documentId)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Echelon.Lore.{documentId}.md";
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        
        return new LoreDocument
        {
            Id = documentId,
            Content = reader.ReadToEnd(),
            Theme = ThemeManager.Current
        };
    }
}

// Usage in View
<LoreViewer DocumentId="project-history" 
            Theme="GreenPhosphor" 
            EnableSearch="true" />
```

#### C) Hybrid Approach (Recommended)

**Strategy:** Implement both systems with complementary roles.

**In-App (MAUI/Blazor):**
- Quick reference guides and tutorials
- Essential lore summaries (1-2 page versions)
- Contextual tooltips and inline help
- Offline-first design
- Immersive Pip-Boy terminal aesthetic

**Website:**
- Complete lore archive with all documents
- Long-form technical articles
- Development blog and devlogs
- Community resources and links
- Marketing and app promotion

**Content Sync Strategy:**
- Maintain single source of truth (Markdown files in repository)
- Website displays full documents
- App embeds curated/abbreviated versions
- Automated build process ensures consistency

### Additional Feature Concepts

#### 1. Interactive Timeline Visualization

**Concept:** Visual representation of ECHELON development history from 2076 to 2287.

```
Timeline Features:
├─ Interactive Nodes (clickable events)
├─ Project Phases
│  ├─ SPARROW (March-April 2076)
│  ├─ RAVEN (April-August 2076)
│  ├─ GHOST (September 2076-January 2077)
│  └─ ECHELON (February-October 2077)
├─ Key Personnel Annotations
│  ├─ Dr. Krane (active 2076-2078)
│  └─ Dr. Aldridge (active 2078-?)
├─ Historical Events
│  ├─ Great War (October 23, 2077)
│  ├─ Charleston Tragedy (Winter 2077-2078)
│  └─ Project IRONCLAD (2085)
└─ Version Milestones
   ├─ PHOSPHOR 1.0, 2.0, 3.0
   └─ Component Integrations
```

**Implementation:** D3.js, Timeline.js, or custom SVG visualization

#### 2. Classified Document Aesthetic

**Concept:** Present lore documents as authentic-looking classified files.

```
Visual Elements:
├─ Document Headers
│  ├─ Classification Level (TOP SECRET - OMEGA-7)
│  ├─ Document Control Numbers (ECHELON-HIST-001)
│  └─ Distribution Restrictions
├─ Redacted Sections ([REDACTED] blocks)
├─ Official Stamps & Signatures
│  ├─ "AUTHORIZED BY COL. AUTUMN"
│  ├─ Date stamps
│  └─ Security markings
├─ Aging Effects
│  ├─ Subtle paper texture
│  ├─ Coffee stains (optional)
│  └─ Worn edges
└─ Typewriter Font (Courier or similar)
```

**CSS Example:**

```css
.classified-document {
    font-family: 'Courier New', monospace;
    background: #f4f1e8;
    border: 2px solid #333;
    box-shadow: 0 0 10px rgba(0,0,0,0.3);
    padding: 2em;
}

.classification-header {
    border-top: 3px solid #c00;
    border-bottom: 3px solid #c00;
    text-align: center;
    font-weight: bold;
    padding: 0.5em;
    background: #fff;
}

.redacted {
    background: #000;
    color: #000;
    user-select: none;
}
```

#### 3. Easter Eggs & Hidden Content

**Concept:** Reward curious users with hidden lore and developer commentary.

**Ideas:**
- **Konami Code:** Unlock developer commentary mode
- **NEST MAINFRAME ACCESS:** Secret command in help system reveals hidden documents
- **"Looking Glass Mode":** Simulated SIGNET browser interface
- **Terminal Commands:** Type special commands in app (e.g., `/history`, `/personnel`)
- **Version Number Secrets:** Clicking version number 7 times reveals changelog

**Example: Developer Profile Easter Egg**

When user types `/gwergilius` or `/developer` in the help terminal:
```
╔══════════════════════════════════════════╗
║ ECHELON DEVELOPMENT TEAM                 ║
╚══════════════════════════════════════════╝

Historical Team (2076-2078):
- Dr. Elizabeth Krane (Project Lead)
- Dr. Marcus Aldridge (Deputy Director)
- [23 additional personnel - see ECHELON-HIST-001]

Modern Implementation (2025-2026):
The ECHELON system you are using was reconstructed 
from recovered pre-war documentation by:

GWERGILIUS (Gergely Tóth)
Software Developer & Historical Reconstruction Specialist
Born: 1961 (Pre-War era, somehow still operational)
Specialization: Cross-platform .NET, Fallout lore integration

> "I found Dr. Krane's notes in a Vault-Tec storage 
> facility. Turns out PHOSPHOR wasn't just fiction - 
> it was good software architecture. So I built it."

RECRUITMENT NOTICE:
The Enclave seeks individuals skilled in:
- Cross-platform development (.NET, MAUI, Blazor)
- Terminal emulation and UI/UX design
- Fallout universe documentation
- Hungarian-English translation

Contact: recruitment@enclave.gov
(Signal currently unavailable due to nuclear war)

Press [ESC] to return to main menu
```

#### 4. Exportable Field Manual

**Concept:** Allow users to download comprehensive documentation as a single document.

**Formats:**
- **PDF:** Professional "ECHELON Operator's Manual"
- **ePub/Kindle:** E-reader friendly version
- **Print-Optimized:** Clean formatting for physical printing

**Contents:**
- Complete Project History
- Component documentation
- Algorithm explanations
- Terminal hacking guide
- Troubleshooting procedures

**Header Example:**

```
╔═════════════════════════════════════════╗
║ ECHELON OPERATOR'S MANUAL               ║
║ Version 2.1.7 - Field Operations Guide  ║
║                                         ║
║ CLASSIFICATION: TOP SECRET - OMEGA-7    ║
║ AUTHORIZED PERSONNEL ONLY               ║
╚═════════════════════════════════════════╝

This document contains sensitive information 
regarding ECHELON Terminal Breach operations...
```

#### 5. Audio Logs (Future Enhancement)

**Concept:** Immersive audio content in Fallout holotape style.

**Content Ideas:**
- "Dr. Krane's Research Log, Entry 47..."
- "NEST Command Briefing: ECHELON Deployment"
- "Field Report: Charleston Tragedy Investigation"

**Implementation:**
- Text-to-speech generation (Azure Cognitive Services, ElevenLabs)
- Voice acting (community contribution or professional)
- Tape recorder UI aesthetic
- Audio waveform visualization

### Phased Implementation Roadmap

#### Phase 1: MVP (SPARROW/RAVEN era)
**Focus:** Core functionality only

- [ ] Basic in-app help (How to Use)
- [ ] README on GitHub with project overview
- [ ] Simple documentation in repository

**Timeline:** Current development phase

#### Phase 2: Polish (GHOST era)
**Focus:** Enhanced user experience

- [ ] In-app Civilopedia with full lore database
- [ ] Embedded Markdown viewer with Pip-Boy styling
- [ ] Basic web landing page (download + overview)
- [ ] Color palette settings integration

**Timeline:** Beta release preparation

#### Phase 3: Community (ECHELON era)
**Focus:** Public presence and engagement

- [ ] Full website with lore hub
- [ ] Development blog
- [ ] Interactive timeline visualization
- [ ] Exportable field manual (PDF)
- [ ] Community links (Discord/Reddit)

**Timeline:** Public launch and promotion

#### Phase 4: Advanced Features (Post-Launch)
**Focus:** Premium experience

- [ ] Audio logs
- [ ] Easter eggs and hidden content
- [ ] Multilingual support (Hungarian, English, etc.)
- [ ] User-contributed content system
- [ ] Achievement/badge system

**Timeline:** Long-term enhancement

### Success Metrics

**Website Analytics:**
- Page views and unique visitors
- Time on site (especially lore pages)
- Download conversion rate
- Bounce rate from landing page

**In-App Engagement:**
- Help system usage frequency
- Most-viewed lore documents
- Search query analysis
- Color palette preferences

**Community Growth:**
- GitHub stars/forks
- Discord/Reddit membership
- User-generated content submissions
- Social media mentions

### Technical Considerations

**Accessibility:**
- WCAG 2.1 AA compliance
- Screen reader compatibility
- Keyboard navigation support
- High contrast mode option

**Performance:**
- Lazy loading for large documents
- Image optimization
- Embedded font subset loading
- Service worker for offline access (web)

**Maintenance:**
- Single source of truth for content (Git repository)
- Automated deployment pipeline
- Version control for documentation
- Regular content audits

---

## When to Implement UI/UX Enhancements

**In-App Help (Phase 1-2):**
- [ ] Multiple screens/features need explanation
- [ ] User testing reveals confusion points
- [ ] Lore content reaches critical mass

**Website (Phase 2-3):**
- [ ] App reaches beta stability
- [ ] Core features are complete
- [ ] Ready for public awareness campaign

**Advanced Features (Phase 4):**
- [ ] Established user base exists
- [ ] Community actively engages with content
- [ ] Resources available for polish work

**Current Recommendation:** Focus on core application functionality first. Document ideas in FutureArchitecture.md for implementation when appropriate.

[Magyar]: ./FutureArchitecture.hu.md
[anchor-current]: #current-architecture
[anchor-proposed]: #proposed-architecture
[anchor-details]: #component-details
[anchor-password]: #password-class-modified
[anchor-registry]: #passwordregistry-new
[anchor-gamesession]: #gamesession-modified
[anchor-arch-diagram]: #architecture-diagram
[anchor-benefits]: #benefits
[anchor-perf]: #performance-analysis
[anchor-migration]: #migration-path
[anchor-when]: #when-to-implement
[anchor-uiux]: #future-uiux-architecture
[anchor-info-arch]: #information-architecture
[anchor-features]: #additional-feature-concepts
[anchor-roadmap]: #phased-implementation-roadmap
[img-current]: ../Images/FutureArchitecture-CurrentArchitecture.drawio.svg
[src-current]: ../Images/FutureArchitecture-CurrentArchitecture.drawio
[src-current-puml]: ../Images/FutureArchitecture-CurrentArchitecture.puml
[src-current-mmd]: ../Images/FutureArchitecture-CurrentArchitecture.mmd
[src-current-gv]: ../Images/FutureArchitecture-CurrentArchitecture.gv
[img-proposed]: ../Images/FutureArchitecture-ProposedArchitecture.drawio.svg
[src-proposed]: ../Images/FutureArchitecture-ProposedArchitecture.drawio
[src-proposed-puml]: ../Images/FutureArchitecture-ProposedArchitecture.puml
[src-proposed-mmd]: ../Images/FutureArchitecture-ProposedArchitecture.mmd
[src-proposed-gv]: ../Images/FutureArchitecture-ProposedArchitecture.gv
[img-arch]: ../Images/FutureArchitecture-Application.drawio.svg
[src-arch]: ../Images/FutureArchitecture.drawio
[Flyweight Pattern]: https://refactoring.guru/design-patterns/flyweight
[Object Pool]: https://medium.com/@ahsan.majeed086/object-pool-pattern-464f4dcc1c75
[Cache-Aside]: https://learn.microsoft.com/en-us/azure/architecture/patterns/cache-aside
[Identity Map]: https://martinfowler.com/eaaCatalog/identityMap.html