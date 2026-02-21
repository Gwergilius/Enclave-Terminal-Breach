# Blazor PWA UI – Pip-Boy terminál terv

## Összefoglaló
- Egyetlen főképernyő két állapottal: **Input** (szavak hozzáadása/törlése) és **Hacking** (tipp + találat megadás).
- Pip-Boy stílusú monokróm terminál (paletták és font előbeállítások már elérhetők).
- Alsó status bar az utolsó üzenetet mutatja (info/warn/error), plusz toast a jobb alsó sarokban.
- Konfigurációs panel overlay: paletta, font, UI nyelv (EN/HU). Jelszavak/tippek angol szavak maradnak.
- Az Escape az univerzális bezárás/reset: overlayek bezárása, hackingből vissza Input-ba (reset), Input-ban app kilépés.
- Felhasználói beállítások (paletta, font, UI nyelv, toast időzítés) localStorage-ban perzisztálva.

## Elrendezés (drótvázak)

### Input mód
![Input-Mock]
Status bar: [INFO/WARN/ERR] utolsó üzenet (módot mutatja). Toast overlay: jobb alsó (auto-timeout + bezárás).

### Hacking mód
![Hacking-Mock]
Esc: játék állapot reset és vissza Input-ba. Status bar + Toast overlay ugyanaz, mint Input-ban.

### Konfigurációs panel (overlay)
Hotkey: C (ha a parancssor nincs fókuszban). Paletta/font/nyelv választó, toast timeout, Apply, Esc bezárás alkalmazás nélkül.

### Help panel (overlay)
Hotkey: F1. Rövid játékleírás + billentyűk. Esc bezárás.

**Todo:** Kontextusfüggő Help panel.

## Fő komponensek (Web)
- `TerminalShell`, `WordGrid`, `CommandLine`, `MenuPanel`, `BestGuessPanel`, `MatchChips`, `ConfigPanel`, `HelpPanel`, `StatusBar`, `ToastOverlay`: lásd angol [WebUI-PipBoy.md](WebUI-PipBoy.md).

## Állapot és logika
Mód váltás Input ↔ Hacking; szó limit 20; hossz szabályok; WordGrid kattintás; Autocomplete forrás; Chipek; Hotkey gating: angol dokumentum.

## Status/Toast pipeline (MediatR)
`StatusMessageNotification`, handlerek (Core log, Web status bar, Web toast queue): angol dokumentum.

## Nyitott részletek
Hibakód rövidítések (ERR_LIMIT, ERR_LEN, stb.), toast időzítés alapértelmezett 4s, config perzisztencia localStorage, Help tartalom kétnyelvű.

[//]: #References-and-image-links
[Input-Mock]: ../Images/UI-elements/UI-mockup-Input.svg
[Hacking-Mock]: ../Images/UI-elements/UI-mockup-Hacking.svg
