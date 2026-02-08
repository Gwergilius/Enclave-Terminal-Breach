# Unified Operating System

**English** | [Magyar]

> This document is a digest of the article [Unified Operating System] at fallout.fandom.com. For the original source, see: [Unified Operating System]

The __Unified Operating System__ (UOS) is a text-based user interface  operating system developed by [RobCo Industries] which manages the hardware of a computer and acts as a software environment in which computer programs can be run efficiently. It runs on a computer mainframe.

[[_TOC_]]

## Background
The [UOS][Unified Operating System] was published and copyrighted by RobCo Industries in [2075] with an expiration date in [2077]. The Unified Operating System relies on the MF Boot Agent and the RETROS BIOS, which initialize the hardware and run tests (see terminal commands below). Before the unified operating system starts, the protocol RobCo Industries Termlink is used to log into the system and recover/reset lost passwords. It is also the main tool used in [hacking] terminals to gain access to otherwise restricted content and files.

## Commands

### System header

```
ROBCO INDUSTRIES UNIFIED OPERATING SYSTEM
COPYRIGHT 2075-2077 ROBCO INDUSTRIES
```

### Logon
```
WELCOME TO ROBCO INDUSTRIES (TM) TERMLINK

> LOGON ADMIN

ENTER PASSWORD NOW

> ******
```

### Hacking
```
WELCOME TO ROBCO INDUSTRIES (TM) TERMLINK

>SET TERMINAL/INQUIRE

RIT-V300

SET FILE/PROTECTION=OWNER:RWED ACCOUNTS.F

SET HALT RESTART/MAINT

Initializing Robco Industries(TM) MF Boot Agent v2.3.0
RETROS BIOS
RBIOS-4.02.08.00 52EE5.E7.E8
Copyright 2201-2203 Robco Ind.
Uppermem: 64 KB
Root (5A8)
Maintenance Mode

>RUN DEBUG/ACCOUNTS.F
```

### Minigame
```
ROBCO INDUSTRIES (TM) TERMLINK PROTOCOL
ENTER PASSWORD NOW
```

### Termlink
Used to log into the system and recover lost passwords and serves as the main tool used in hacking terminals to gain access to otherwise restricted content and files. Error codes may occur, prompted by 0x and then eight additional variables signifying the likely error cause which is then printed in a plain text message for the user. 

External error codes are prompted when an external device is malfunctioning when connected to a functional terminal. Termlink error codes take precedence over external devices and external error codes are only given when the device is faulty, such as a holotape or robot. External error codes are much shorter than terminal error codes stopping at three variables past 0x. 

## Appearances
The Unified Operating System appears in [Fallout 3][FO3], [Fallout: New Vegas][FNV], [Fallout 4][FO4], and [Fallout 76][FO76].

|Gallery|
|:---:|
|![Uos 1]<br />User interface, file options |
|![Uos 2]<br />User message system|
|![Trespasser screen]<br />RobCo Trespasser Management System|
|![Fo3 Admin Logon]<br />Login prompt|
|![Fo3 Correct Guess]<br />Termlink abuse|


[//]: #References-and-images
[Uos 1]: ../Images/UOS/Uos_1.webp "User interface, file options"
[Uos 2]: ../Images/UOS/Uos_2.webp "User message system"
[Trespasser screen]: ../Images/UOS/Trespasser_screen.webp "RobCo Trespasser Management System"
[Fo3 Admin Logon]: ../Images/UOS/Fo3_Admin_Logon.webp "Login prompt"
[Fo3 Correct Guess]: ../Images/UOS/Fo3_Correct_Guess.webp "Termlink abuse"

[2075]: https://fallout.fandom.com/wiki/Timeline#2075
[2077]: https://fallout.fandom.com/wiki/Timeline#2077
[FO3]: https://fallout.fandom.com/wiki/Fallout_3
[FNV]: https://fallout.fandom.com/wiki/Fallout:_New_Vegas
[FO4]: https://fallout.fandom.com/wiki/Fallout_4
[FO76]: https://fallout.fandom.com/wiki/Fallout_76
[hacking]: Minigame.md
[RobCo Industries]: https://fallout.fandom.com/wiki/RobCo_Industries
[Unified Operating System]: https://fallout.fandom.com/wiki/Unified_Operating_System
[Magyar]: ./UOS.hu.md