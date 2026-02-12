# Jövőbeli architektúra: jelszó regiszter és gyorsítótár

**[English]** | Magyar

Lehetséges jövőbeli fejlesztés: memória- és teljesítményoptimalizálás Flyweight mintával és gyorsítással.

## Tartalomjegyzék

- [Jelenlegi architektúra][anchor-current]
- [Javasolt architektúra][anchor-proposed]
- [Komponens részletek][anchor-details]
  - [Password osztály (módosított)][anchor-password]
  - [PasswordRegistry (új)][anchor-registry]
  - [GameSession (módosított)][anchor-gamesession]
- [Architektúra diagram][anchor-arch-diagram]
- [Előnyök][anchor-benefits]
- [Teljesítményelemzés][anchor-perf]
- [Migrációs út][anchor-migration]
- [Mikor implementáljuk][anchor-when]

---

## Jelenlegi architektúra

A jelenlegi implementációban:

![Jelenlegi architektúra][img-current]

Diagram forrása: [FutureArchitecture-CurrentArchitecture.drawio][src-current]. További formátumok: [PlantUML][src-current-puml], [Mermaid][src-current-mmd], [DOT][src-current-gv].

**Problémák:**
- Minden `GameSession` saját `Password` objektumokat hoz létre
- `IsEliminated` a `Password` objektumon van (session-specifikus állapot megosztott koncepción)
- Nincs `GetMatchCount` gyorsítása a sessionek között
- 100k szós szótárral és több sessionnel a memóriahasználat lineárisan nő

---

## Javasolt architektúra

**Flyweight minta:** `Password` objektumok egy globális regiszterben, a `GameSession` referenciákat tárol.

![Javasolt architektúra][img-proposed]

Diagram forrása: [FutureArchitecture-ProposedArchitecture.drawio][src-proposed]. További formátumok: [PlantUML][src-proposed-puml], [Mermaid][src-proposed-mmd], [DOT][src-proposed-gv].

---

## Komponens részletek

**Password (módosított):** Word, nincs IsEliminated; `_matchCountCache` (GetMatchCount kétirányú gyorsítással); internal konstruktor.  
**PasswordRegistry (új):** GetOrCreate(word), Contains, Count, Preload; szálbiztos.  
**GameSession (módosított):** _passwords lista (referenciák), _eliminated HashSet; IsEliminated(Password), EliminateByMatchCount, Reset.

Teljes kódpéldák: [angol verzió – Component Details][English].

---

## Architektúra diagram

![Architektúra diagram][img-arch]

Diagram forrása: [FutureArchitecture.drawio][src-arch].

---

## Előnyök

| Szempont | Jelenlegi | Javasolt |
|----------|-----------|----------|
| **Memória / szó** | N × (Password) sessionenként | 1 × (Password) összesen |
| **GetMatchCount (első)** | O(word_length) | O(word_length) |
| **GetMatchCount (ismételt)** | O(word_length) | **O(1)** cache találat |
| **Új GameSession** | O(n) Password allokáció | O(n) szótár keresés |
| **IsEliminated tárolás** | Password-on (sessionek között kiszivárog) | GameSession-ben (izolált) |

---

## Teljesítményelemzés

**Match count cache:** Első hívás párra O(szó hossz), érték mindkét cache-be kerül; későbbi hívás O(1). 12 jelszóval 66 pár, mindegyik egyszer számolva, utána O(1). Ugyanazokkal a szavakkal több játék: **100% cache találat**.

**Memória:** 100k szó, 10 session – jelenlegi ~100 MB, javasolt ~10 MB.

---

## Migrációs út

1. **Fázis 1:** PasswordRegistry + GameSession overload (nem breaking).  
2. **Fázis 2:** IsEliminated áthelyezése GameSession-be (breaking).  
3. **Fázis 3:** _matchCountCache + GetMatchCount cache.  
4. **Fázis 4:** Régi konstruktor eltávolítása, Password internal.

---

## Mikor implementáljuk

Ha: szólista > 10k, gyakori több session, GetMatchCount bottleneck, mobil memória probléma.  
**Jelenlegi állapot:** Még nem szükséges; 10–16 jelszó játékonként, teljesítmény rendben.

---

## Kapcsolódó minták

- **[Flyweight]:** Password objektumok megosztása sessionek között   
- **[Object Pool]:** PasswordRegistry mint pool   
- **[Cache-Aside]:** GetMatchCount első hozzáférésre cache-el   
- **[Identity Map]:** Egy Password példány egyedi szóra 

[English]: ./FutureArchitecture.md
[anchor-current]: #jelenlegi-architektúra
[anchor-proposed]: #javasolt-architektúra
[anchor-details]: #komponens-részletek
[anchor-password]: #password-osztály-módosított
[anchor-registry]: #passwordregistry-új
[anchor-gamesession]: #gamesession-módosított
[anchor-arch-diagram]: #architektúra-diagram
[anchor-benefits]: #előnyök
[anchor-perf]: #teljesítményelemzés
[anchor-migration]: #migrációs-út
[anchor-when]: #mikor-implementáljuk
[img-current]: ../Images/FutureArchitecture-CurrentArchitecture.drawio.svg
[src-current]: ../Images/FutureArchitecture-CurrentArchitecture.drawio
[src-current-puml]: ../Images/FutureArchitecture-CurrentArchitecture.puml
[src-current-mmd]: ../Images/FutureArchitecture-CurrentArchitecture.mmd
[src-current-gv]: ../Images/FutureArchitecture-CurrentArchitecture.gv
[img-proposed]: ../Images/FutureArchitecture-ProposedArchitecture.drawio.svg
[src-proposed]: ../Images/FutureArchitecture-ProposedArchitecture.drawio
[src-proposed-puml]: ../Images/FutureArchitecture-ProposedArchitecture.puml
[src-proposed-mmd]: ../Images/FutureArchitecture-ProposedArchitecture.mmd
[src-proposed-gv]: ../Images/FutureArchitecture-ProposedArchitecture.gv
[img-arch]: ../Images/FutureArchitecture-Application.drawio.svg
[src-arch]: ../Images/FutureArchitecture.drawio
[Flyweight]: https://refactoring.guru/design-patterns/flyweight
[Object Pool]: https://medium.com/@ahsan.majeed086/object-pool-pattern-464f4dcc1c75
[Cache-Aside]: https://learn.microsoft.com/en-us/azure/architecture/patterns/cache-aside
[Identity Map]: https://martinfowler.com/eaaCatalog/identityMap.html
