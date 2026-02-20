# ECHELON projekt – Történeti háttér

**[English]** | Magyar

## Háború előtti kontextus: RobCo dominancia

### RobCo UOS piaci jelenlét (2075–2077)

A RobCo Industries [Egységes operációs rendszere][UOS] (_Unified Operating System - UOS_) 2075-re közel teljes piaci lefedettséget ért el: katonai infrastruktúra (87% katonai bázis terminál, DEFCON, nukleáris silók, stb.), kormányzati létesítmények (Vault-Tec, kutató laborok, elnöki bunker), kritikus infrastruktúra (erőművek, víz, kommunikáció).

### Az Enclave sebezhetősége

2076-ra a NEST elemzők kritikus fenyegetést azonosítottak: egyetlen bukási pont (RobCo monopólium), Robert House autonómiája, kínai kiberháború az UOS ellen, háború utáni forgatókönyv. 2076 február: titkos NSA lehallgatás – kínai ügynökök három katonai létesítményt feltörtek RobCo gyengeségekkel; 73 napig észrevétlen.

## Az ECHELON projekt kezdete

2076 március: Presidential Finding 76-14 (OMEGA) – „védelmi ellen-behatolási képességek”, 47 M$, Enclave SIGINT, fedő megnevezés „Terminal Maintenance Protocol Enhancement”. Site-R (Raven Rock), Dr. Elizabeth Krane projektvezető, 23 szakember. „Know Your Enemy” doktrína: RobCo mérnökök toborzása, UOS reverse engineering, árnyék specifikáció. Az első futtatható eredmény a **SPARROW** (DOS konzol POC, stdin/stdout), amely validálta a jelszó-eliminációs algoritmust a hardver integráció előtt.

## SIGNET – Signal Intelligence Network

SIGNET: az Enclave titkos belső hálózata (2074), OMEGA személyzetnek. **Looking Glass** böngésző: SIGNET elsődleges felülete, ECHELON eszközök hálózati eléréséhez. Infrastruktúra: Raven Rock, Poseidon Oil Rig, Control Station ENCLAVE, NEST; 256 bites titkosítás, biometrikus hitelesítés, levegőrésznyi elválasztás. GHOST v3.0.0 első SIGNET bevezetés; v3.2.4 SIGNET standard. Kettős bevezetés: SIGNET (GHOST) létesítményekhez, Pip-Boy (ECHELON) terepre. 2287-ben még üzemel.

## Verziótörténet

#### SPARROW (2076 márc.–ápr.):
- v1.1.0 ![SPARROW v1.1.0](https://img.shields.io/badge/SPARROW-v1.1.0-38bdf8) első futtatható POC (DOS konzol, stdin/stdout); **HOUSE gambit** (statisztikai / véletlen tippválasztás), ~55% siker, ~15% legrosszabb eset.
- v1.2.0 ![SPARROW v1.2.0](https://img.shields.io/badge/SPARROW-v1.2.0-38bdf8) bevitel finomítás (case-insensitive, többoszlopos lista). RAVEN és a későbbi fázisok a SPARROW-alapú magot örökölték.   
#### RAVEN (2076 ápr.–aug.):
- v2.0.0 ![RAVEN v2.0.0](https://img.shields.io/badge/RAVEN-v2.0.0-1e40af) best-bucket megoldás (információs pontszám + legkisebb legrosszabb bucket), első sikeres UOS feltörés NX-12-re, 34% siker, 47 perc; 
- v2.1.0 ![RAVEN v2.1.0](https://img.shields.io/badge/RAVEN-v2.1.0-1e40af) ![PHOSPHOR 1.0](https://img.shields.io/badge/PHOSPHOR-1.0-7c3aed) (hardver absztrakció).   
#### GHOST (2076 szept.–2077 jan.):
- v3.0.0 ![GHOST v3.0.0](https://img.shields.io/badge/GHOST-v3.0.0-06b6d4) Pip-Boy + Looking Glass (SIGNET), 67%, 8 perc;
- v3.2.0 ![GHOST v3.2.0](https://img.shields.io/badge/GHOST-v3.2.0-06b6d4) ![PHOSPHOR 2.0](https://img.shields.io/badge/PHOSPHOR-2.0-7c3aed) (16 szín); v3.2.4 ![GHOST v3.2.4](https://img.shields.io/badge/GHOST-v3.2.4-06b6d4) „Ghost Revised” DIVERGENCE szabály (döntetlenben az előző tipptől legtávolabbi jelölt), neurális minta felismerés, 81%.
#### ECHELON (2077 febr.–okt.):
- v4.0.0 ![ECHELON v4.0.0](https://img.shields.io/badge/ECHELON-v4.0.0-059669) tie-breaker stratégia (determinisztikus döntetlenfeloldás), teljes átdolgozás, 94%, 2–4 perc, ![PHOSPHOR 3.0](https://img.shields.io/badge/PHOSPHOR-3.0-7c3aed);
- v4.1.7 ![ECHELON v4.1.7](https://img.shields.io/badge/ECHELON-v4.1.7-059669) végső háború előtti, 247 egység OMEGA operatíveknek.

## Háború utáni örökség

2077. október 23.: A nagy háború. 247 egységből 204 túlélt. Charleston tragédia: Dr. Krane elvesztése; Dr. Marcus Aldridge veszi át. ![PHOSPHOR](https://img.shields.io/badge/PHOSPHOR-system-7c3aed) háború utáni előny: Brotherhood, Institute, degradált terminálok – mind kezelve. ![ECHELON v4.2.x](https://img.shields.io/badge/ECHELON-v4.2.x-059669) (2080-as évek) degradált terminálok; ![ECHELON v4.2.5](https://img.shields.io/badge/ECHELON-v4.2.5-059669) IRONCLAD (Power Armor HUD); ![ECHELON v4.3.x](https://img.shields.io/badge/ECHELON-v4.3.x-059669) Brotherhood ellenintézkedések; ![ECHELON v4.4.x](https://img.shields.io/badge/ECHELON-v4.4.x-059669) Institute integráció. 2287: ![ECHELON v4.1.7](https://img.shields.io/badge/ECHELON-v4.1.7-059669) „arany standard”.

## Technikai filozófia

„Az UOS-t úgy tervezték, hogy feltörjék”: x/y correct visszajelzés, zárójel segédletek, négy próba reset – kereskedelmi kényelem biztonság helyett. Konspiráció: szándékos gyengesítés kormányzati kérésre. Dr. Krane: „Nem törtük meg a RobCo biztonságát. Csak elolvastuk a kézikönyvet, amit nem merték kiadni.”

[English]: ./Project-History.md
[UOS]: ./UOS.hu.md
