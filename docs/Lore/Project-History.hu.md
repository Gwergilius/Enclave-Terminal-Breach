# ECHELON projekt – Történeti háttér

**[English]** | Magyar

## Háború előtti kontextus: RobCo dominancia

### RobCo UOS piaci jelenlét (2075–2077)

A RobCo Industries [Egységes operációs rendszere][UOS] 2075-re közel teljes piaci lefedettséget ért el: katonai infrastruktúra (87% katonai bázis terminál, DEFCON, nukleáris silók, stb.), kormányzati létesítmények (Vault-Tec, kutató laborok, elnöki bunker), kritikus infrastruktúra (erőművek, víz, kommunikáció).

### Az Enclave sebezhetősége

2076-ra a NEST elemzők kritikus fenyegetést azonosítottak: egyetlen bukási pont (RobCo monopólium), Robert House autonómiája, kínai kiberháború az UOS ellen, háború utáni forgatókönyv. 2076 február: titkos NSA lehallgatás – kínai ügynökök három katonai létesítményt feltörtek RobCo gyengeségekkel; 73 napig észrevétlen.

## Az ECHELON projekt kezdete

2076 március: Presidential Finding 76-14 (OMEGA) – „védelmi ellen-behatolási képességek”, 47 M$, Enclave SIGINT, fedő megnevezés „Terminal Maintenance Protocol Enhancement”. Site-R (Raven Rock), Dr. Elizabeth Krane projektvezető, 23 szakember. „Know Your Enemy” doktrína: RobCo mérnökök toborzása, UOS reverse engineering, árnyék specifikáció. Az első futtatható eredmény a **SPARROW** (DOS konzol POC, stdin/stdout), amely validálta a jelszó-eliminációs algoritmust a hardver integráció előtt.

## SIGNET – Signal Intelligence Network

SIGNET: az Enclave titkos belső hálózata (2074), OMEGA személyzetnek. **Looking Glass** böngésző: SIGNET elsődleges felülete, ECHELON eszközök hálózati eléréséhez. Infrastruktúra: Raven Rock, Poseidon Oil Rig, Control Station ENCLAVE, NEST; 256 bites titkosítás, biometrikus hitelesítés, levegőrésznyi elválasztás. GHOST v2.0.0 első SIGNET bevezetés; v2.2.4 SIGNET standard. Kettős bevezetés: SIGNET (GHOST) létesítményekhez, Pip-Boy (ECHELON) terepre. 2287-ben még üzemel.

## Verziótörténet

#### SPARROW (2076 márc.–ápr.):
- v1.1.0 első futtatható POC (DOS konzol, stdin/stdout); **HOUSE gambit** (statisztikai / véletlen tippválasztás), ~55% siker, ~15% legrosszabb eset.
- v1.2.0 bevitel finomítás (case-insensitive, többoszlopos lista). RAVEN és a későbbi fázisok a SPARROW-alapú magot örökölték.   
#### RAVEN (2076 ápr.–aug.):
- v1.3.1 best-bucket megoldás (információs pontszám + legkisebb legrosszabb bucket), első sikeres UOS feltörés NX-12-re, 34% siker, 47 perc; 
- v1.4.0 PHOSPHOR 1.0 (hardver absztrakció).   
#### GHOST (2076 szept.–2077 jan.):
- v2.0.0 Pip-Boy + Looking Glass (SIGNET), 67%, 8 perc; 
- v2.2.0 PHOSPHOR 2.0 (16 szín); v2.2.4 „Ghost Revised” DIVERGENCE szabály (döntetlenben az előző tipptől legtávolabbi jelölt), neurális minta felismerés, 81%.   
#### ECHELON (2077 febr.–okt.):
- v3.0.0 tie-breaker stratégia (determinisztikus döntetlenfeloldás), teljes átdolgozás, 94%, 2–4 perc, PHOSPHOR 3.0; 
- v3.1.7 végső háború előtti, 247 egység OMEGA operatíveknek.   

## Háború utáni örökség

2077. október 23.: A nagy háború. 247 egységből 204 túlélt. Charleston tragédia: Dr. Krane elvesztése; Dr. Marcus Aldridge veszi át. PHOSPHOR háború utáni előny: Brotherhood, Institute, degradált terminálok – mind kezelve. v3.2.x (2080-as évek) degradált terminálok; v3.2.5 IRONCLAD (Power Armor HUD); v3.3.x Brotherhood ellenintézkedések; v3.4.x Institute integráció. 2287: ECHELON v3.1.7 „arany standard”.

## Technikai filozófia

„Az UOS-t úgy tervezték, hogy feltörjék”: x/y correct visszajelzés, zárójel segédletek, négy próba reset – kereskedelmi kényelem biztonság helyett. Konspiráció: szándékos gyengesítés kormányzati kérésre. Dr. Krane: „Nem törtük meg a RobCo biztonságát. Csak elolvastuk a kézikönyvet, amit nem merték kiadni.”

[English]: ./Project-History.md
[UOS]: ./UOS.hu.md
