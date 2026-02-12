# Platform szolgáltatások – tervezési összefoglaló

**[English]** | Magyar

A tervezett platform szolgáltatási réteg és célarchitektúra az `Enclave Terminal Breach` projekthez (tervezési fázis).

## Célarchitektúra – platform implementációk

### 1. Platform-specifikus szolgáltatás implementációk

Egy interfész a Core-ban: **`IPlatformInfoService`** – platform leírása, ProjectCodename, Version, PlatformName, Description, időzítések, boot szövegek. Console, MAUI, Blazor saját implementációt regisztrál a DI-ban.

**RAVEN v0.3.1** (Console): `App/Console/Services/ConsolePlatformInfoService.cs` – SIGINT Console (POC), 47 perc feltörés, 34% siker.  
**GHOST v1.2.4** (Blazor): `App/Web/Services/BlazorPlatformInfoService.cs` – Web Browser (SIGNET), 8 perc, 81%.  
**ECHELON v2.1.7** (MAUI): `App/Maui/Services/MauiPlatformInfoService.cs` – Pip-Boy 3000 Mark IV, 2–4 perc, 94%.

### 2. SIGNET hálózat dokumentáció

Lásd [Project-History][Project-History] – SIGNET szekció. Összefoglalva: Signal Intelligence Network, száloptika (Raven Rock, Poseidon, ENCLAVE, NEST), 256 bites titkosítás, levegőrésznyi elválasztás, GHOST v1.2.4 SIGNET standard, kettős bevezetés (SIGNET + Pip-Boy), 2287-ben még üzemel.

### 3. Architektúra összefoglaló

Platform folyamat:  
![Platform folyamat][Platform Flow]

Cél platform folyamat (implementálandó).

### 4. Lore idővonal – verziókonzisztencia

```
2076 ápr.–aug.:   RAVEN v0.3.1 (Console POC)
                        ↓
2076 szept.:      GHOST v1.0.0 (Pip-Boy + SIGNET web)
                        ↓
2076 november:    GHOST v1.2.4 (SIGNET standard)
                        ↓
2077 febr.–okt.:  ECHELON v2.0.0 → v2.1.7 (Pip-Boy terep)
                        ↓
2077. okt. 23.:   [A NAGY HÁBORÚ]
                        ↓
2287:             ECHELON v2.1.7, GHOST v1.2.4 továbbra is standard
```

### 5. Implementációs lépések – platform integráció

MAUI/Blazor: `AddSingleton<IPlatformInfoService, ...>`, `AddTransient<IGameSession>(sp => new GameSession(sp.GetRequiredService<IPlatformInfoService>()))`.

## Tervezési döntések

**Miért SIGNET:** narratíva, webböngésző magyarázat, GHOST vs ECHELON indok, kettős bevezetés.  
**Miért külön verziók:** RAVEN POC, GHOST képzés/létesítmény, ECHELON terep.  
**Miért GameSession wrapper:** nincs extra ViewModel függés, tiszta architektúra, tesztelhetőség.

## Lore célok (implementáció során ellenőrizendő)

- ☐ Verziószámok egyeznek a [Project-History][Project-History]-val  
- ☐ Platform nevek lore-konzisztensek  
- ☐ SIGNET magyarázat a webes bevezetésre  
- ☐ Idővonal konzisztens (RAVEN → GHOST → ECHELON)  
- ☐ Poszt-háborús folytonosság (2287)  
- ☐ Kettős bevezetés taktikai értelemben következetes  

[//]: #References-and-image-links
[English]: ./PlatformServicesSummary.md
[Project-History]: ../Lore/Project-History.md
[Platform Flow]: ../Images/PlatformServicesSummary-PlatformFlow.drawio.svg
