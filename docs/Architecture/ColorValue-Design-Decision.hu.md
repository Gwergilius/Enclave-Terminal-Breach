# ColorValue tervezési döntés

**[English]** | Magyar

## Áttekintés

Egyedi `ColorValue` record a `System.Drawing.Color` vagy `Microsoft.Maui.Graphics.Color` helyett: platformfüggetlen színreprezentáció Shared ViewModelekben, MAUI-ban és Blazorban.

## Megfontolt lehetőségek

**1. System.Drawing.Color:** Ismert .NET típus, de **.NET 6+ csak Windows** – elutasítva.  
**2. Microsoft.Maui.Graphics.Color:** MAUI-ban jó, de **Shared nem hivatkozhat MAUI-ra**, **Blazor nem használhatja** – elutasítva.  
**3. Egyedi ColorValue record (választott):** Zero függőség, valóban többplatformos, record (értékegyenlőség, immutabilitás), FromHex/ToCssRgba/ToCssRgb; MAUI konverter és Blazor CSS string. ✅ **Választva.**

## API

`new ColorValue(R, G, B, A)`, `ColorValue.FromHex("#66ff66")`, `ToHex()`, `ToCssRgba()`, `ToCssRgb()`, ColorValue.Transparent/White/Black. MAUI: ColorValueToColorConverter. Blazor: `ToCssRgb()` a style-ban.

## Réteg diagram

Prezentációs réteg (MAUI, Blazor) → konverterek → **ColorValue** (platformfüggetlen) ← TerminalButtonViewModel (Shared). Előnyök: valóban többplatformos ViewModelek, rugalmas megjelenítés, konzisztens viselkedés, egy forrás.

## Tesztelés és jövő

ColorValue teljes unit teszt alatt. Opcionális: Windows-only System.Drawing híd; lehetséges bővítések: HSL, Lighten/Darken, Lerp.

[English]: ./ColorValue-Design-Decision.md
