# Forráskód

**[English]** | Magyar

Az Enclave Terminal Breach többplatformos implementációi.

## Szerkezet (terv)

**1. fázis: SPARROW** – DOS 3.11 proof of concept, stdin/stdout.  
**2. fázis: RAVEN** – Konzol alkalmazás, képernyőpozicionálás, PHOSPHOR absztrakció.  
**3. fázis: GHOST** – Blazor PWA, SIGNET bevezetés.  
**4. fázis: ECHELON** – MAUI többplatformos mobil, Pip-Boy integráció.

## Megosztott komponensek

**Core** – Üzleti logika, algoritmusok, domain modellek. **Shared** – ViewModelek, állapotkezelés, megosztott UI logika. **TestHelpers** – Közös teszt segédletek és mockok. **Tests** – Unit, integrációs, UI tesztek.

## Technológiai stack

.NET 10.0, C# 12.0, MAUI, Blazor, xUnit + ReqNRoll + Playwright.

## Dokumentáció

Fejlesztési irányelvek: [kódolási szabályok][coding standards].

[English]: ./README.md
[coding standards]: ../.cursor/rules/README.hu.md
