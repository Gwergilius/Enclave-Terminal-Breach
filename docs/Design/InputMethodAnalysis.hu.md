# Beviteli módszer elemzés – Fallout Terminal Hacker

Ez a dokumentum a lehetséges jelszavak megadásának különböző módszereit elemzi a Fallout Terminal Hacker mobil alkalmazásban.

## Tartalomjegyzék

- [Kontextus](#kontextus)
- [Beviteli módszerek elemzése](#beviteli-módszerek-elemzése)
- [Összehasonlítás](#összehasonlítás-összefoglaló)
- [Javasolt megoldás](#javasolt-megoldás)
- [Implementációs terv](#implementation-plan)
- [Technikai megfontolások](#technical-considerations)

## Kontextus

A Fallout Terminal Hacker minijáték 10–16, egyforma hosszú (4–15 karakter) angol szót mutat retro zöld foszfor terminál kinézettel, véletlenszerű ASCII karakterekkel keverve. Az alkalmazásnak hatékonyan kell rögzítenie ezeket a szavakat a jelszó-javaslatokhoz. A beviteli módszer jelentősen befolyásolja a felhasználói élményt.

## Beviteli módszerek elemzése

### 1. szóközzel elválasztott szövegbevitel
Leírás, előnyök, hátrányok, UX értékelés: lásd angol [InputMethodAnalysis.md](InputMethodAnalysis.md). **Ajánlott platformok:** Konzol (RAVEN), Web (GHOST).

### 2. Combo box szólistával
Egyenkénti megadás autocomplete-tel, szólistából szűrve. **Ajánlott platformok:** Web (GHOST), MAUI (ECHELON).

### 3. Kamera OCR-rel
Képernyő fotó, OCR szövegkinyerés. **Ajánlott platformok:** MAUI (ECHELON).

### Alternatív módszerek
4. Hibrid OCR kézi javítással, 5. Speech-to-Text, 6. Vágólap alapú bevitel – összehasonlítás: angol dokumentum táblázata.

## Összehasonlítás összefoglaló

| Módszer        | Sebesség | Megbízhatóság | Komplexitás | Offline | Értékelés |
|----------------|----------|---------------|-------------|---------|-----------|
| Szövegbevitel | ⭐⭐      | ⭐⭐           | ⭐          | ✅      | 2/5       |
| Combo Box     | ⭐⭐⭐     | ⭐⭐⭐⭐⭐        | ⭐⭐⭐        | ✅      | 3.5/5     |
| Kamera OCR    | ⭐⭐⭐⭐⭐   | ⭐⭐⭐          | ⭐⭐⭐⭐       | ⚠️     | 4/5       |
| Hibrid OCR    | ⭐⭐⭐⭐    | ⭐⭐⭐⭐         | ⭐⭐⭐⭐       | ⚠️     | 4.5/5     |
| Speech        | ⭐⭐      | ⭐⭐           | ⭐⭐⭐        | ❌      | 2/5       |
| Vágólap       | ⭐⭐⭐⭐    | ⭐⭐⭐⭐⭐        | ⭐          | ✅      | 1.5/5     |

## Javasolt megoldás

**Többszintű beviteli rendszer:** több módszer, a felhasználó választhat.

**Prioritás:** 1) Combo box autocomplete (megbízható), 2) Kamera OCR kézi javítással (gyors), 3) Egyszerű szövegbevitel (tartalék).

### UI mockup

![UI-Mockup]

## Implementációs terv, technikai megfontolások

Technológiai stack, projekt struktúra, fejlesztési fázisok, OCR előfeldolgozás, szó szűrés, szólista optimalizáció: lásd angol [InputMethodAnalysis.md](InputMethodAnalysis.md).

## Következtetés

A combo box megbízhatóságát és az OCR sebességét összekapcsolva rugalmas bevitelt kap a felhasználó. A combo box implementációval indulj; az OCR később prémium bővítés lehet.

[//]: #References-and-image-links
[Terminal Screen]: ../Images/Terminal.png
[UI-Mockup]: ../Images/UI-elements/UI-mockup.drawio.svg
