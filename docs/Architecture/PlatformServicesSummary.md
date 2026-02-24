# Platform Services Design Summary

**English** | [Magyar]

This document describes the planned platform services layer and target architecture for the `Enclave Terminal Breach` project (design phase, no implementation yet).

## Target architecture – platform implementations

### 1. Platform-Specific Service Implementations

The platform layer is defined by a single interface in Core: **`IPlatformInfoService`**. This interface describes the current platform to the rest of the application and defines platform-dependent constants used by boot sequence, ViewModels, and UI—such as `ProjectCodename`, `Version`, `PlatformName`, `Description`, `screen size`, `timing values` (line delay, progress duration, etc.), and `boot texts` (system modules, applications). Each app (Console, MAUI, Blazor) will provide its own implementation and register it in DI; other services depend only on `IPlatformInfoService`, keeping behaviour platform-agnostic. The `Target File:` references below are the planned locations for these per-platform implementations.

The snippets below describe the intended contract/implementation, to be built.

#### Target: Console POC - RAVEN v2.3.1
**Target File:** `App/Console/Services/ConsolePlatformInfoService.cs`

```csharp
ProjectCodename: "RAVEN"
Version: "v2.3.1"
PlatformName: "SIGINT Console (POC)"
Description: "Proof of Concept - First successful UOS breach implementation"
```

**Lore Context:** April-August 2076, dedicated SIGINT console (180 lbs, not portable), 47-minute breach time, 34% success rate.

#### Target: Blazor Web - GHOST v3.2.4
**Target File:** `App/Web/Services/BlazorPlatformInfoService.cs`

```csharp
ProjectCodename: "GHOST"
Version: "v3.2.4"
PlatformName: "Web Browser (SIGNET Access)"
Description: "Field Prototype - Ghost Revised (Neural pattern recognition)"
```

**Lore Context:** September 2076-January 2077, deployed on SIGNET network, 81% success rate, 8-minute breach time, web-based training and facility operations.

#### Target: MAUI - ECHELON v4.1.7
**Target File:** `App/Maui/Services/MauiPlatformInfoService.cs`

```csharp
ProjectCodename: "ECHELON"
Version: "v4.1.7"
PlatformName: "Pip-Boy 3000 Mark IV"
Description: "Operational Deployment - Final pre-war version (October 2077)"
```

**Lore Context:** October 2077 final pre-war version, 247 units deployed to OMEGA clearance operatives, 94% success rate, 2-4 minute breach time, undetectable stealth mode.

### 2. SIGNET Network Documentation

#### See [Project-History.md] – SIGNET section

**Summary of content in that document:** "SIGNET - Signal Intelligence Network"

**Key Content:**
- **Infrastructure:** Fiber-optic network connecting Raven Rock, Poseidon Oil Rig, Control Station ENCLAVE, NEST
- **Security:** 256-bit quantum-resistant encryption, multi-factor biometric authentication, air-gapped from public networks
- **ECHELON Deployment:** GHOST v1.0.0 first web deployment, v1.2.4 became standard SIGNET build
- **Deployment Strategy:** Dual deployment (SIGNET for facility ops, Pip-Boy for field ops)
- **Post-War Status:** Still operational in surviving Enclave facilities (2287)

**Updated GHOST Version Descriptions:**
- v3.0.0: Added "Dual Deployment" note (Pip-Boy + SIGNET)
- v3.2.4: Added "SIGNET Standard" designation, platform clarification
- New quote from Dr. Krane about SIGNET deployment assessment

### 3. Architecture Summary

Platform Flow:   
![Platform Flow][Platform Flow]

Target platform flow (to be implemented).

### 4. Lore timeline – version consistency

```
April-August 2076:  RAVEN v2.3.1 (Console POC)
                    ↓
Sept 2076:         GHOST v3.0.0 (Pip-Boy + SIGNET web)
                    ↓
November 2076:     GHOST v3.2.4 "Ghost Revised" (SIGNET standard)
                    ↓
Feb-October 2077:  ECHELON v4.0.0 → v4.1.7 (Pip-Boy field deployment)
                    ↓
October 23, 2077:  [THE GREAT WAR]
                    ↓
2287:              ECHELON v4.1.7 still standard
                   GHOST v3.2.4 still on SIGNET
```

### 5. Implementation steps – platform integration

When implementing each platform, register the platform service and GameSession as below.

#### MAUI
Add to `MauiProgram.cs`:
```csharp
services.AddSingleton<IPlatformInfoService, MauiPlatformInfoService>();
services.AddTransient<IGameSession>(sp => new GameSession(
    sp.GetRequiredService<IPlatformInfoService>()));
```

#### Blazor
Add to `Startup.cs` or `Program.cs`:
```csharp
services.AddSingleton<IPlatformInfoService, BlazorPlatformInfoService>();
services.AddTransient<IGameSession>(sp => new GameSession(
    sp.GetRequiredService<IPlatformInfoService>()));
```

## Key Design Decisions

### Why SIGNET?
1. **Signal Intelligence Network** fits Enclave SIGINT Division narrative
2. Explains web browser access (internal network, not internet)
3. Provides lore reason for GHOST vs ECHELON split
4. Creates dual-deployment strategy (facility + field)

### Why Different Versions?
1. **Console (RAVEN)**: POC, shows evolution from prototype
2. **Blazor (GHOST)**: Training/facility version, web-based, lower performance
3. **MAUI (ECHELON)**: Final field version, highest performance, portable

### Why GameSession Wrapper?
1. **No new ViewModel dependencies**: ViewModels don't need IPlatformInfoService
2. **Clean architecture**: Platform info flows through existing GameSession
3. **Single responsibility**: GameSession owns all game-related context
4. **Testability**: Easy to mock with TestPlatformInfoService

## Lore goals (validate during implementation)

- ☐ All version numbers match [Project-History.md]  
- ☐ Platform names are lore-accurate  
- ☐ SIGNET provides canonical explanation for web deployment  
- ☐ Timeline is consistent (RAVEN → GHOST → ECHELON)  
- ☐ Post-war continuity explained (2287 usage)  
- ☐ Dual-deployment strategy makes tactical sense  


[//]: #Refereces-and-image-links
[Project-History.md]: ../Lore/Project-History.md

---

**Date:** 2026-02-20  
**Status:** Planning / Design  
**Ready for:** Next: Implement Core services and first platform (Console POC)

[Magyar]: ./PlatformServicesSummary.hu.md
[Platform Flow]: ../Images/PlatformServicesSummary-PlatformFlow.drawio.svg