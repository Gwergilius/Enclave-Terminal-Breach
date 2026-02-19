# Platform-Specific Boot Timing Implementation

[English] | **Magyar**

## Összefoglaló

Az időzítési konstansok sikeresen átkerültek az `IPlatformInfoService`-be, így minden platform a saját timing-jaival rendelkezik. Ez platform-függő boot sequence sebességet eredményez, ami lore-accurate (a RAVEN régebbi, lassabb hardver, az ECHELON modern, gyors Pip-Boy).

## Módosított fájlok

### 1. Interface és Service Implementációk

#### ✅ Core/Services/IPlatformInfoService.cs
**Új timing property-k:**
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
**RAVEN v0.3.1 - Lassabb timing (régebbi hardware):**
```csharp
LineDelay:         200ms  // vs. ECHELON 150ms
SlowDelay:         500ms  // vs. ECHELON 400ms
OkStatusDelay:     250ms  // vs. ECHELON 200ms
ProgressUpdate:    0ms    // Not used (no progress bars)
ProgressDuration:  0ms    // Not used (no progress bars)
WarningPause:      1200ms // vs. ECHELON 1000ms
FinalPause:        600ms  // vs. ECHELON 500ms
```

**Lore indoklás:** 180 lbs dedikált SIGINT konzol, nem optimalizált sebességre, proof of concept.

#### ✅ Maui/Services/MauiPlatformInfoService.cs
**ECHELON v2.1.7 - Optimalizált timing (modern Pip-Boy):**
```csharp
LineDelay:         150ms  // Standard
SlowDelay:         400ms  // Standard
OkStatusDelay:     200ms  // Standard
ProgressUpdate:    50ms   // Smooth animation
ProgressDuration:  800ms  // Full animation
WarningPause:      1000ms // Standard
FinalPause:        500ms  // Standard
```

**Lore indoklás:** Final pre-war version, teljes mértékben optimalizálva sebességre és hatékonyságra.

#### ✅ Web/Services/BlazorPlatformInfoService.cs
**GHOST v1.2.4 - Köztes timing (web browser):**
```csharp
LineDelay:         175ms  // Between RAVEN and ECHELON
SlowDelay:         450ms  // Between RAVEN and ECHELON
OkStatusDelay:     225ms  // Between RAVEN and ECHELON
ProgressUpdate:    60ms   // Slightly slower animation
ProgressDuration:  900ms  // Slightly longer animation
WarningPause:      1100ms // Between RAVEN and ECHELON
FinalPause:        550ms  // Between RAVEN and ECHELON
```

**Lore indoklás:** Field prototype, web-based platform, esetleges network latency.

#### ✅ TestHelpers/TestPlatformInfoService.cs
**Test timing - Nagyon gyors (unit tesztek):**
```csharp
LineDelay:         1ms
SlowDelay:         1ms
OkStatusDelay:     1ms
ProgressUpdate:    1ms
ProgressDuration:  10ms
WarningPause:      1ms
FinalPause:        1ms
```

**Indoklás:** Tesztek gyorsan fussanak, ne várjunk másodperceket.

### 2. ViewModel módosítások

#### ✅ Shared/ViewModels/BootSequence/PhaseBase.cs
**Régi konstansok (static readonly) eltávolítva:**
```csharp
// REMOVED:
public static readonly TimeSpan SLOW_DELAY = 400.Millisecs();
public static readonly TimeSpan OK_STATUS_DELAY = 200.Millisecs();
// etc...
```

**Új protected property-k (PlatformInfo-ból):**
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
**Konstans hivatkozások frissítve:**
- `SLOW_DELAY` → `SlowDelay`
- `LINE_DELAY` → `LineDelay`
- `OK_STATUS_DELAY` → `OkStatusDelay`

#### ✅ Shared/ViewModels/BootSequence/ProjectHeader.cs
**Konstans hivatkozások frissítve:**
- `FINAL_PAUSE` → `FinalPause`

#### ✅ Shared/ViewModels/BootSequence/ReadyState.cs
**Konstans hivatkozások frissítve:**
- `FINAL_PAUSE` → `FinalPause`

#### ✅ Shared/ViewModels/BootSequence/AuthorizationWarning.cs
**Konstans hivatkozások frissítve:**
- `WARNING_PAUSE` → `WarningPause`

#### ✅ Shared/ViewModels/BootSequence/SystemIntegrityCheck.cs
**Konstans hivatkozások frissítve:**
- `LINE_DELAY` → `LineDelay`
- `PROGRESS_DURATION` → `ProgressDuration`
- `PROGRESS_UPDATE` → `ProgressUpdate`

## Platform Timing Összehasonlítás

| Timing Constant   | RAVEN (Console) | GHOST (Web) | ECHELON (MAUI) | Test |
|-------------------|-----------------|-------------|----------------|------|
| **LineDelay**     | 200ms ⏱️         | 175ms       | 150ms ⚡       | 1ms  |
| **SlowDelay**     | 500ms ⏱️         | 450ms       | 400ms ⚡       | 1ms  |
| **OkStatusDelay** | 250ms ⏱️         | 225ms       | 200ms ⚡       | 1ms  |
| **ProgressUpdate**| 0ms (N/A) ❌    | 60ms        | 50ms ⚡        | 1ms  |
| **ProgressDuration**| 0ms (N/A) ❌  | 900ms       | 800ms ⚡       | 10ms |
| **WarningPause**  | 1200ms ⏱️        | 1100ms      | 1000ms ⚡      | 1ms  |
| **FinalPause**    | 600ms ⏱️         | 550ms       | 500ms ⚡       | 1ms  |

**Jelmagyarázat:**
- ⏱️ = Lassabb (régebbi hardware)
- ⚡ = Gyorsabb (modern hardware)
- ❌ = Nem használt (no progress bars in POC)

## Lore-accurate Boot Sequence sebesség

### RAVEN (Console POC) - Lassú
**Teljes boot idő:** ~9-10 másodperc
- Régebbi, dedikált SIGINT konzol (180 lbs)
- Nem optimalizált sebességre
- Proof of Concept verzió
- Nincs progress bar animáció

### GHOST (Web) - Közepes
**Teljes boot idő:** ~7-8 másodperc
- Web browser platform
- Field prototype verzió
- Lehetséges network latency
- Van progress bar animáció

### ECHELON (MAUI) - Gyors
**Teljes boot idő:** ~6-7 másodperc
- Modern Pip-Boy 3000 Mark IV
- Final pre-war version
- Teljes optimalizálás
- Smooth progress bar animáció

### Test - Nagyon gyors
**Teljes boot idő:** ~50-100 milliszekundum
- Unit test gyors futásához
- Nincs vizuális késleltetés
- Csak a logika tesztelése

## Előnyök

### ✅ Tisztább architektúra
- Nincs platform-specifikus `if` logika a ViewModelekben
- Minden platform-specifikus adat az `IPlatformInfoService`-ben
- Single Responsibility Principle betartása

### ✅ Lore-accurate különbségek
- RAVEN lassabb (régebbi hardware)
- ECHELON gyorsabb (modern Pip-Boy)
- GHOST közepes (web-based)
- Felhasználói élmény tükrözi a platform jellemzőit

### ✅ Könnyebb karbantartás
- Timing változtatások egy helyen (PlatformInfoService)
- Nem kell minden Phase-t módosítani
- Tesztekben gyors timing (1ms)

### ✅ Skálázható
- Új platform hozzáadása egyszerű
- Csak új PlatformInfoService implementáció kell
- Phase-ek automatikusan használják az új timing-okat

## Tesztelés

### Build ellenőrzés:
```bash
cd "C:\Users\GergelyToth2\OneDrive\Source\Gwergilius\Fallout Terminal Hacker.Android"
dotnet build
```

### Console POC futtatás:
```bash
cd "src/Enclave.Echelon/App/Console"
dotnet run
```

**Elvárt viselkedés:**
- Boot sequence lassabb, mint az ECHELON verzió
- Jól érzékelhető különbség a timing-okban
- Nincs progress bar (RAVEN POC limitáció)

## Breaking Changes

### ⚠️ PhaseBase változások
**Régi API (REMOVED):**
```csharp
public static readonly TimeSpan LINE_DELAY = 150.Millisecs();
```

**Új API:**
```csharp
protected TimeSpan LineDelay => PlatformInfo.LineDelay;
```

**Migráció:**
- Phase osztályok: `CONSTANT_NAME` → `PropertyName`
- Pl.: `LINE_DELAY` → `LineDelay`
- Pl.: `WARNING_PAUSE` → `WarningPause`

### ⚠️ Külső kód, ami hivatkozott a konstansokra
Ha volt bármilyen külső kód, ami a `PhaseBase.CONSTANT_NAME` static konstansokra hivatkozott, az már nem fog működni. Az új megoldásban a timing-ok példány-specifikusak (PlatformInfo-ból jönnek).

## Következő lépések

1. ✅ **Timing konstansok áthelyezve** - KÉSZ
2. ❌ **AddPasswordView implementálása** - Következő
3. ❌ **HackingView implementálása**
4. ❌ **GameOverView implementálása**
5. ❌ **Teljes játékmenet tesztelés**

## Összefoglalás

A refaktorálás sikeres volt! A timing konstansok most platform-specifikusak, ami lore-accurate boot sequence sebességet eredményez. A RAVEN (Console POC) lassabb, az ECHELON (MAUI) gyorsabb, a GHOST (Web) közepes, a Test timing pedig nagyon gyors a unit tesztek számára.

Minden platform a saját `IPlatformInfoService` implementációjából kapja a timing értékeket, így nincs szükség platform-specifikus `if` logikára a ViewModelekben. Ez tisztább, karbantarthatóbb kódot eredményez.

---

**Dátum:** 2026-01-10  
**Státusz:** ✅ Kész  
**Build:** ✅ Sikeres  
**Tesztek:** Függőben (egyelőre nincsenek boot sequence unit tesztek)

[English]: ./BOOT_TIMING_REFACTOR.md