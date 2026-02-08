# Terminál Hacker állapotgép dokumentáció

**[English]** | Magyar

## Áttekintés

A Terminál Hacker alkalmazás tiszta állapotgépet használ a fő játékképernyő módjainak kezelésére. Állapotok, átmenetek és UI viselkedés minden állapothoz.

**Platform:** **MAUI** (MainPage, MainViewModel) és **Blazor** (Home.razor, MainViewModel). Mindkettő külön boot szekvencia képernyővel indul (MAUI: BootSequencePage, Blazor: BootPage.razor); a boot után a fő játékképernyőn ez az állapotgép érvényes.

## Alkalmazás folyamat

MAUI: App Start → BootSequencePage → MainPage → PasswordEntry (kezdeti).  
Blazor: App Start → BootPage.razor → Home.razor → PasswordEntry.  
A **Shared** projekt ugyanazt a MainViewModel és állapotgép logikát adja.

## Állapotok

### 1. PasswordEntry
Kezdeti állapot: jelszavak hozzáadása. PasswordList látható, de NEM kattintható; PasswordEntry látható és engedélyezett (max 20 jelszó); autocomplete words.txt-ből, kihagyva a már hozzáadottakat; „Add Password” kijelölve; „Remove Password” engedélyezett ha van jelszó; „Start Hack” engedélyezett ha van jelszó. Szűrés: words.txt, min 4 karakter (vagy első jelszó hossza), max 20.

### 2. PasswordDeletion
Jelszavak törlése. PasswordList látható és kattintható; PasswordEntry látható; autocomplete a listából; „Remove Password” kijelölve; listára kattintás kitölti a mezőt; submit törli a jelszót.

### 3. HackingGame
Aktív játék. PasswordList látható (tippek kiválasztására); PasswordEntry NEM látható; RecommendedGuesses, BestGuess, match count chipek láthatók; Add/Remove/Start gombok NEM; „Reset” látható. Csak nem kiesett jelszavak választhatók; tipp után match chipek; chipek választása beküldi a tippet és kieszt.

### 4. GameOver
Játék vége (győzelem/vereség). GameResult panel, győzelem/vereség üzenet, „Reset” újraindításhoz.

## Állapotátmenetek (MainViewModel)

PasswordEntry → PasswordDeletion (Remove Password, Passwords.Count > 0).  
PasswordEntry → HackingGame (Start Hack, Passwords.Count >= 2).  
PasswordDeletion → PasswordEntry (Add Password).  
PasswordDeletion → HackingGame (Start Hack, Passwords.Count >= 2).  
HackingGame → GameOver (győzelem vagy vereség).  
GameOver → PasswordEntry (Reset).

## Implementáció

**MainViewModel:** CurrentState, _isGameWon; ShowPasswordEntry, ShowActionButtons, ShowHackingGame, ShowGameResult; IsPasswordListEnabled, IsPasswordEntryEnabled; IsAddPasswordSelected, IsRemovePasswordSelected. **Átmenet:** TransitionToState, IsValidTransition, OnStateEntered. **Parancsok:** SetModeToAddPassword, SetModeToRemovePassword, StartGameAsync, SubmitGuess, ClearAll.

## Előnyök

Világos állapotdefiníciók; kényszerített átmenetek; egyetlen igazságforrás (CurrentState); bővíthető; tesztelhető; karbantartható.

## Migráció

**Előtte:** IsGameActive, IsGameWon, IsGameLost, CurrentMode (bool/enum) → érvénytelen kombinációk lehetségesek. **Utána:** CurrentState (PasswordEntry/PasswordDeletion/HackingGame/GameOver), _isGameWon csak GameOver-nál → csak érvényes kombinációk.

[English]: ./StateMachine.md
