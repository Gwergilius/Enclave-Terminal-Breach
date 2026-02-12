# Excel prototípus

**[English]** | Magyar

A projekt első prototípusa: jelszó-kiszűrési megoldó Excelben (makrós munkafüzet). A mag-algoritmust mutatja be, még mielőtt bármely kódalapú fázis (SPARROW, RAVEN stb.) megkezdődne.

## Algoritmus

A logikát az [Algoritmus] dokumentum írja le. Röviden: minden megmaradt jelszó-jelölthez a megoldó egy információs pontszámot számol (a tipp és az összes jelölt összehasonlításakor kapott különböző egyezésszám-eredmények száma), és olyan tippet választ, ami ezt a pontszámot maximalizálja.

Ez az Excel prototípus **nem** implementálja a tie-breaker finomítást (a legnagyobb bucket méret minimalizálását, ha több tippnek ugyanaz az információs pontszáma). Ennek ellenére remekül használható a folyamat megértéséhez és a terminálhack minijáték kézi kipróbálásához.

## Tartalom

- **Prototype.xlsm** – Makrós munkafüzet. Excelben nyisd meg, engedélyezd a makrókat, és a munkalap utasításait követve futtathatod a megoldót.

## Hivatkozások

- [Algoritmus] – Információelméleti megoldó leírás és implementációs megjegyzések

[//]: #References
[Algoritmus]: ../../docs/Architecture/Algorithm.hu.md
[English]: ./README.md
