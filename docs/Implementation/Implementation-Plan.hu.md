# Blazor PWA UI implementációs lépések (Pip-Boy terminál)

[English] | **Magyar**

## Hatókör

Egyképernyős Pip-Boy stílusú UI Input és Hacking módokkal. Config/Help overlayek, hotkey kezelés, status/toast pipeline, lokalizáció (EN/HU), localStorage beállítások.

## Lépések

1) **Shared szerződések**  
   StatusMessageNotification (message, type Info/Warning/Error, timestamp, code). FluentResults/validáció rövid kódokkal (ERR_LIMIT/ERR_LEN/ERR_DUP/ERR_DICT/ERR_TIMEOUT). MediatR notification handlerek (Core: log, Web: status/toast).

2) **Web szolgáltatások/állapot**  
   IStatusBarState + impl (utolsó üzenet + mód, Notify). IToastService (queue, max 1 látható, timeout config). IConfigService (palette, font, nyelv, toast timeout localStorage-ból). IHotkeyService (globális hotkey, gating ha command line fókuszban).

3) **UI váz**  
   TerminalShell: header (mód, [C] Config, [H] Help, Esc), content (Input/Hacking), status bar, toast overlay. WordGrid, CommandLine, MenuPanel, BestGuessPanel, MatchChips, ConfigPanel, HelpPanel, StatusBar, ToastOverlay.

4) **Input mód**  
   Max 20 szó; hossz/szótár/duplikátum szabályok; hibák status+toast. CommandLine csak Add/Remove állapotban; WordGrid kattintás Remove-ban kitölti. Hotkey: 1 Add, 2 Remove, 3 Start, Esc Exit; letiltva ha cmdline fókuszban.

5) **Hacking mód**  
   Start: cmdline reset, legjobb tipp előtöltés. Tipp után match chipek (0..len); hotkey 0..len. Esc: játék állapot reset Input-ra (győzelem/vereség/feladás egységes).

6) **Lokalizáció (EN/HU)**  
   UI string erőforrások; jelszó/tipp szavak angolul maradnak. Config panel nyelv váltó; Apply menti localStorage-ba és újrarajzol.

7) **Config/Help overlayek**  
   Config: palette, font, nyelv, toast timeout; Apply vagy Esc bezárás mentés nélkül. Help: rövid szabályok + billentyűk; Esc bezárás.

8) **Perzisztencia és bootstrap**  
   Induláskor config localStorage-ból; palette/font/nyelv/toast timeout; alapértelmezések fallback. Változásnál mentés és UI értesítés.

9) **UX finomítás**  
   Cursor villogás/invert, konzisztens paletta, status szín típus szerint. Toast sorrendben (max 1 látható), auto-timeout konfigurálható.

10) **Tesztelés**  
    Unit tesztek state/szolgáltatásokra; integráció/E2E ReqNRoll; hotkey gating és notification flow lefedettség.

[English]: ./Implementation-Plan.md
