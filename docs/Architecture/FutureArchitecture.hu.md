# Jövőbeli architektúra: jelszó regiszter és gyorsítótár

**[English]** | Magyar

Lehetséges jövőbeli fejlesztés: memória- és teljesítményoptimalizálás Flyweight mintával és gyorsítással.

## Jelenlegi probléma

Minden GameSession saját Password objektumokat hoz létre; IsEliminated a Password-on (session-specifikus állapot); nincs GetMatchCount gyorsítása; 100k szó és több session → lineáris memórianövekedés.

## Javasolt: Flyweight

Password objektumok egy globális **PasswordRegistry**-ben; GameSession referenciákat tárol. Session-specifikus állapot: _eliminated HashSet a GameSession-ben. Password: _matchCountCache (GetMatchCount eredmények); internal konstruktor; GetOrCreate a registry-n keresztül.

## Komponensek

**Password (módosított):** Word, nincs IsEliminated; GetMatchCount cache-pel; kétirányú gyorsítás.  
**PasswordRegistry (új):** GetOrCreate(word), Contains, Count, Preload; szálbiztos.  
**GameSession (módosított):** _passwords lista (referenciák), _eliminated HashSet; IsEliminated(Password), EliminateByMatchCount, Reset.

## Előnyök

Memória: 1× Password objektum szónként (nem N× sessionenként). GetMatchCount ismételt hívás O(1) cache-pel. Részletes diagramok, kódpéldák, migrációs fázisok és „mikor implementáljuk”: [angol verzió][English].

[English]: ./FutureArchitecture.md
