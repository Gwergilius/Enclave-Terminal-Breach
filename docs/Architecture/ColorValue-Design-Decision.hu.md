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

![Réteg diagram][img-layer]

A diagram forrása: [ColorValue-LayerDiagram.mmd][src-layer]. További formátumok: [PlantUML][src-layer-puml], [DOT][src-layer-gv], [draw.io][src-layer-drawio].

## Tesztelés

A `ColorValue` típus teljes egészében unit tesztelt:

```csharp
[TestFixture]
public class ColorValueTests
{
    [Test]
    public void FromHex_ValidHex_CreatesCorrectColor()
    {
        var color = ColorValue.FromHex("#66ff66");
        Assert.That(color.R, Is.EqualTo(102));
        Assert.That(color.G, Is.EqualTo(255));
        Assert.That(color.B, Is.EqualTo(102));
    }
    
    [Test]
    public void ToCssRgba_ReturnsValidCssString()
    {
        var color = new ColorValue(102, 255, 102, 255);
        Assert.That(color.ToCssRgba(), Is.EqualTo("rgba(102, 255, 102, 1.0)"));
    }
}
```

## Jövőbeli megfontolások

### Opcionális: System.Drawing.Color híd (csak Windows)

Kizárólag Windowson dolgozó fejlesztőknek opcionális konverziókat adhatnánk:

```csharp
public record ColorValue(byte R, byte G, byte B, byte A = 255)
{
    #if NET6_0_OR_GREATER && WINDOWS
    public System.Drawing.Color ToSystemDrawingColor() 
        => System.Drawing.Color.FromArgb(A, R, G, B);
    
    public static ColorValue FromSystemDrawingColor(System.Drawing.Color color)
        => new(color.R, color.G, color.B, color.A);
    #endif
}
```

Ez csak Windowson lenne elérhető, és egyértelműen dokumentáltan platform-specifikus maradna.

### Lehetséges bővítések

1. **HSL/HSV támogatás**
   ```csharp
   public (double H, double S, double L) ToHsl();
   public static ColorValue FromHsl(double h, double s, double l);
   ```

2. **Színmanipuláció**
   ```csharp
   public ColorValue Lighten(double amount);
   public ColorValue Darken(double amount);
   public ColorValue WithAlpha(byte alpha);
   ```

3. **Színinterpoláció**
   ```csharp
   public static ColorValue Lerp(ColorValue a, ColorValue b, double t);
   ```

## Összefoglalás

Az egyedi `ColorValue` record típust választottuk a többplatformos követelményeink legjobb megoldásaként. Bár saját kód karbantartását igényli, a valódi platformfüggetlenség, a rugalmas konverzió és a zero függőség előnyei túlsúlyban vannak a költségekhez képest.

A döntés lehetővé teszi:
- ✅ ViewModelek megosztását MAUI és Blazor között
- ✅ Platformfüggetlen üzleti logika írását
- ✅ Színnel kapcsolatos funkciók egyszerű unit tesztelését
- ✅ Bármely .NET platformra történő telepítést korlátozás nélkül

## Hivatkozások

- [.NET 6 System.Drawing.Common Breaking Change][.NET 6 Breaking Change]
- [Microsoft.Maui.Graphics dokumentáció][Microsoft.Maui.Graphics Documentation]
- [C# Records dokumentáció][C# Records Documentation]

[//]: #References-and-image-links

[.NET 6 Breaking Change]: https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only
[Microsoft.Maui.Graphics Documentation]: https://learn.microsoft.com/en-us/dotnet/maui/user-interface/graphics/
[C# Records Documentation]: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record
[img-layer]: ../Images/ColorValue-LayerDiagram.mmd.svg
[src-layer]: ../Images/ColorValue-LayerDiagram.mmd
[src-layer-puml]: ../Images/ColorValue-LayerDiagram.puml
[src-layer-gv]: ../Images/ColorValue-LayerDiagram.gv
[src-layer-drawio]: ../Images/ColorValue-LayerDiagram.drawio
---

**Dokumentum verzió:** 1.0  
**Utolsó frissítés:** 2026-01-05  
**Szerző:** AI Assistant  
**Státusz:** Jóváhagyva

[English]: ./ColorValue-Design-Decision.md
