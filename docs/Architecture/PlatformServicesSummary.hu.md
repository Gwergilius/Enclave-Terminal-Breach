# Platform szolgáltatások – tervezési összefoglaló

**[English]** | Magyar

A tervezett platform szolgáltatási réteg és célarchitektúra az `Enclave Terminal Breach` projekthez (tervezési fázis).

## Célarchitektúra

Egy interfész a Core-ban: **`IPlatformInfoService`** – platform leírása, ProjectCodename, Version, PlatformName, Description, időzítések, boot szövegek. Console, MAUI, Blazor saját implementációt regisztrál DI-ban.

**RAVEN v0.3.1** (Console): `App/Console/Services/ConsolePlatformInfoService.cs` – SIGINT Console (POC), 47 perc feltörés, 34% siker.  
**GHOST v1.2.4** (Blazor): `App/Web/Services/BlazorPlatformInfoService.cs` – Web Browser (SIGNET), 8 perc, 81%.  
**ECHELON v2.1.7** (MAUI): `App/Maui/Services/MauiPlatformInfoService.cs` – Pip-Boy 3000 Mark IV, 2–4 perc, 94%.

## SIGNET

Lásd [Project-History][Project-History] – SIGNET szekció. Összefoglalva: Signal Intelligence Network, száloptika (Raven Rock, Poseidon, ENCLAVE, NEST), 256 bites titkosítás, levegőrésznyi elválasztás, GHOST v1.2.4 SIGNET standard, kettős bevezetés (SIGNET + Pip-Boy), 2287-ben még üzemel.

## Architektúra diagram

IPlatformInfoService → Console/Blazor/MAUI (RAVEN/GHOST/ECHELON) → GameSession → ViewModels. (ASCII diagram megegyezik az angol verzióval.)

## Lore idővonal

2076 ápr.–aug. RAVEN v0.3.1 → 2076 szept. GHOST v1.0.0 → 2076 nov. GHOST v1.2.4 → 2077 febr.–okt. ECHELON v2.0–v2.1.7 → 2077. okt. 23. [A NAGY HÁBORÚ] → 2287 ECHELON/GHOST továbbra is standard.

## Implementációs lépések

MAUI/Blazor: AddSingleton IPlatformInfoService, AddTransient IGameSession (GameSession(IPlatformInfoService)).

## Tervezési döntések

Miért SIGNET: narratíva, webböngésző magyarázat, GHOST vs ECHELON indok, kettős bevezetés. Miért külön verziók: RAVEN POC, GHOST képzés/létesítmény, ECHELON terep. Miért GameSession wrapper: nincs extra ViewModel függés, tiszta architektúra, tesztelhetőség.

[English]: ./PlatformServicesSummary.md
[Project-History]: ../Lore/Project-History.hu.md
