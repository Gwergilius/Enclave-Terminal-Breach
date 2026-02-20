# Platform-Specific Boot Timing Implementation
**English** | [Magyar]

## Summary

Timing constants have been moved into `IPlatformInfoService`, so each platform has its own timing. This yields platform-dependent boot sequence speed that is lore-accurate (RAVEN is older, slower hardware; ECHELON is modern, faster Pip-Boy).

## Modified Files

### 1. Interface and Service Implementations

#### ✅ Core/Services/IPlatformInfoService.cs
**New timing properties:**
```csharp
TimeSpan LineDelay { get; }          // Normal line load time
TimeSpan SlowDelay { get; }          // Important/dramatic lines
TimeSpan OkStatusDelay { get; }      // Delay before showing "OK"
TimeSpan ProgressUpdate { get; }     // Progress bar refresh rate
TimeSpan ProgressDuration { get; }   // Full progress bar animation
TimeSpan WarningPause { get; }       // Warning screen pause
TimeSpan FinalPause { get; }         // Before main app transition
```

#### ✅ Console/Services/ConsolePlatformInfoService.cs
**RAVEN v0.3.1 - Slower timing (older hardware):**
```csharp
LineDelay:         200ms  // vs. ECHELON 150ms
SlowDelay:         500ms  // vs. ECHELON 400ms
OkStatusDelay:     250ms  // vs. ECHELON 200ms
ProgressUpdate:    0ms    // Not used (no progress bars)
ProgressDuration:  0ms    // Not used (no progress bars)
WarningPause:      1200ms // vs. ECHELON 1000ms
FinalPause:        600ms  // vs. ECHELON 500ms
```

**Lore rationale:** 180 lbs dedicated SIGINT console, not optimised for speed, proof of concept.

#### ✅ Maui/Services/MauiPlatformInfoService.cs
**ECHELON v2.1.7 - Optimised timing (modern Pip-Boy):**
```csharp
LineDelay:         150ms  // Standard
SlowDelay:         400ms  // Standard
OkStatusDelay:     200ms  // Standard
ProgressUpdate:    50ms   // Smooth animation
ProgressDuration:  800ms  // Full animation
WarningPause:      1000ms // Standard
FinalPause:        500ms  // Standard
```

**Lore rationale:** Final pre-war version, fully optimised for speed and efficiency.

#### ✅ Web/Services/BlazorPlatformInfoService.cs
**GHOST v1.2.4 - Mid-range timing (web browser):**
```csharp
LineDelay:         175ms  // Between RAVEN and ECHELON
SlowDelay:         450ms  // Between RAVEN and ECHELON
OkStatusDelay:     225ms  // Between RAVEN and ECHELON
ProgressUpdate:    60ms   // Slightly slower animation
ProgressDuration:  900ms  // Slightly longer animation
WarningPause:      1100ms // Between RAVEN and ECHELON
FinalPause:        550ms  // Between RAVEN and ECHELON
```

**Lore rationale:** Field prototype, web-based platform, possible network latency.

#### ✅ TestHelpers/TestPlatformInfoService.cs
**Test timing - Very fast (unit tests):**
```csharp
LineDelay:         1ms
SlowDelay:         1ms
OkStatusDelay:     1ms
ProgressUpdate:    1ms
ProgressDuration:  10ms
WarningPause:      1ms
FinalPause:        1ms
```

**Rationale:** Tests run quickly without waiting for seconds.

### 2. ViewModel changes

#### ✅ Shared/ViewModels/BootSequence/PhaseBase.cs
**Old constants (static readonly) removed:**
```csharp
// REMOVED:
public static readonly TimeSpan SLOW_DELAY = 400.Millisecs();
public static readonly TimeSpan OK_STATUS_DELAY = 200.Millisecs();
// etc...
```

**New protected properties (from PlatformInfo):**
```csharp
protected TimeSpan LineDelay => PlatformInfo.LineDelay;
protected TimeSpan SlowDelay => PlatformInfo.SlowDelay;
protected TimeSpan OkStatusDelay => PlatformInfo.OkStatusDelay;
protected TimeSpan ProgressUpdate => PlatformInfo.ProgressUpdate;
protected TimeSpan ProgressDuration => PlatformInfo.ProgressDuration;
protected TimeSpan WarningPause => PlatformInfo.WarningPause;
protected TimeSpan FinalPause => PlatformInfo.FinalPause;
```

#### ✅ Shared/ViewModels/BootSequence/SystemInitializationPhase.cs
**Constant references updated:**
- `SLOW_DELAY` → `SlowDelay`
- `LINE_DELAY` → `LineDelay`
- `OK_STATUS_DELAY` → `OkStatusDelay`

#### ✅ Shared/ViewModels/BootSequence/ProjectHeader.cs
**Constant references updated:**
- `FINAL_PAUSE` → `FinalPause`

#### ✅ Shared/ViewModels/BootSequence/ReadyState.cs
**Constant references updated:**
- `FINAL_PAUSE` → `FinalPause`

#### ✅ Shared/ViewModels/BootSequence/AuthorizationWarning.cs
**Constant references updated:**
- `WARNING_PAUSE` → `WarningPause`

#### ✅ Shared/ViewModels/BootSequence/SystemIntegrityCheck.cs
**Constant references updated:**
- `LINE_DELAY` → `LineDelay`
- `PROGRESS_DURATION` → `ProgressDuration`
- `PROGRESS_UPDATE` → `ProgressUpdate`

## Platform Timing Comparison

| Timing Constant   | RAVEN (Console) | GHOST (Web) | ECHELON (MAUI) | Test |
|-------------------|-----------------|-------------|----------------|------|
| **LineDelay**     | 200ms ⏱️         | 175ms       | 150ms ⚡       | 1ms  |
| **SlowDelay**     | 500ms ⏱️         | 450ms       | 400ms ⚡       | 1ms  |
| **OkStatusDelay** | 250ms ⏱️         | 225ms       | 200ms ⚡       | 1ms  |
| **ProgressUpdate**| 0ms (N/A) ❌    | 60ms        | 50ms ⚡        | 1ms  |
| **ProgressDuration**| 0ms (N/A) ❌  | 900ms       | 800ms ⚡       | 10ms |
| **WarningPause**  | 1200ms ⏱️        | 1100ms      | 1000ms ⚡      | 1ms  |
| **FinalPause**    | 600ms ⏱️         | 550ms       | 500ms ⚡       | 1ms  |

**Legend:**
- ⏱️ = Slower (older hardware)
- ⚡ = Faster (modern hardware)
- ❌ = Not used (no progress bars in POC)

## Lore-accurate Boot Sequence Speed

### RAVEN (Console POC) - Slow
**Total boot time:** ~9-10 seconds
- Older, dedicated SIGINT console (180 lbs)
- Not optimised for speed
- Proof of Concept version
- No progress bar animation

### GHOST (Web) - Medium
**Total boot time:** ~7-8 seconds
- Web browser platform
- Field prototype version
- Possible network latency
- Has progress bar animation

### ECHELON (MAUI) - Fast
**Total boot time:** ~6-7 seconds
- Modern Pip-Boy 3000 Mark IV
- Final pre-war version
- Fully optimised
- Smooth progress bar animation

### Test - Very fast
**Total boot time:** ~50-100 milliseconds
- For fast unit test runs
- No visual delay
- Logic testing only

## Benefits

### ✅ Cleaner architecture
- No platform-specific `if` logic in ViewModels
- All platform-specific data in `IPlatformInfoService`
- Single Responsibility Principle respected

### ✅ Lore-accurate differences
- RAVEN slower (older hardware)
- ECHELON faster (modern Pip-Boy)
- GHOST in between (web-based)
- User experience reflects platform characteristics

### ✅ Easier maintenance
- Timing changes in one place (PlatformInfoService)
- No need to change every Phase
- Fast timing (1ms) in tests

### ✅ Scalable
- Adding a new platform is straightforward
- Only a new PlatformInfoService implementation is needed
- Phases automatically use the new timing

## Testing

### Build check:
```bash
cd "C:\Users\GergelyToth2\OneDrive\Source\Gwergilius\Fallout Terminal Hacker.Android"
dotnet build
```

### Console POC run:
```bash
cd "src/Enclave.Echelon/App/Console"
dotnet run
```

**Expected behaviour:**
- Boot sequence slower than ECHELON version
- Noticeable difference in timing
- No progress bar (RAVEN POC limitation)

## Breaking Changes

### ⚠️ PhaseBase changes
**Old API (REMOVED):**
```csharp
public static readonly TimeSpan LINE_DELAY = 150.Millisecs();
```

**New API:**
```csharp
protected TimeSpan LineDelay => PlatformInfo.LineDelay;
```

**Migration:**
- Phase classes: `CONSTANT_NAME` → `PropertyName`
- e.g. `LINE_DELAY` → `LineDelay`
- e.g. `WARNING_PAUSE` → `WarningPause`

### ⚠️ External code referencing constants
Any external code that referenced `PhaseBase.CONSTANT_NAME` static constants will no longer work. In the new design, timing values are instance-specific (from PlatformInfo).

## Next Steps

1. ✅ **Timing constants moved** - DONE
2. ❌ **Implement AddPasswordView** - Next
3. ❌ **Implement HackingView**
4. ❌ **Implement GameOverView**
5. ❌ **Full gameplay testing**

## Summary

The refactor was successful. Timing constants are now platform-specific, giving lore-accurate boot sequence speed. RAVEN (Console POC) is slower, ECHELON (MAUI) is faster, GHOST (Web) is in between, and Test timing is very fast for unit tests.

Each platform gets its timing from its own `IPlatformInfoService` implementation, so there is no need for platform-specific `if` logic in ViewModels. This results in cleaner, more maintainable code.

---

**Date:** 2026-01-10  
**Status:** ✅ Done  
**Build:** ✅ Successful  
**Tests:** Pending (no boot sequence unit tests yet)

[Magyar]: ./BOOT_TIMING_REFACTOR.hu.md