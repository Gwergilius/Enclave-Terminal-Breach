# Fejlesztési dokumentáció

**[English]** | Magyar

Folyamat, workflow és közreműködési irányelvek.

## Tartalom

- [Branching Strategy][Branching Strategy] – Git workflow és branch kezelés
- [Commit Conventions][Commit Conventions] – Commit üzenet szabványok és példák
- [Release Process][Release Process] – Verziókezelés, release-ek és changelog kezelés
- [GitVersion Integration][GitVersion Integration] – Automatikus szemantikus verziókezelés
- [Changelog Management][Changelog Management] – Changelog karbantartási stratégia

## Gyors referencia

### Új munka kezdése
```bash
git checkout main && git pull
git checkout -b feature/your-feature
```

### Commit
```bash
git add .
git commit -m "feat(scope): your change description

+semver: minor"
```
Részletek: [Commit Conventions][Commit Conventions].

### Changelog frissítés
```bash
# Szerkeszd a komponens changelogot (pl. src/Enclave.Echelon.SPARROW/CHANGELOG.md)
## [Unreleased]
### Added
- Your feature description
```
Részletek: [Changelog Management][Changelog Management].

### Verzió ellenőrzés
```bash
dotnet-gitversion /showvariable SemVer
```
Részletek: [GitVersion Integration][GitVersion Integration].

### Pull Request
```bash
git push -u origin feature/your-feature
gh pr create --title "feat(scope): Your feature"
```

### Merge után
```bash
git checkout main && git pull
git branch -d feature/your-feature
```

### Release
Changelogok frissítése (központi + komponens), commit, tag, push. Részletek: [Release Process][Release Process].

## Automatikus verziókezelés

A projekt [GitVersion][GitVersion]-t használ: verzió a git history, tag-ek, branch név és commit üzenetek alapján.

**Gyors ellenőrzés:** `dotnet-gitversion`  
**Branch-specifikus:** main → 0.1.1; feature/* → 0.2.0-alpha.1+3; phase/* → 1.0.0-beta.1+5  
Részletek: [GitVersion Integration][GitVersion Integration].

## Changelog stratégia

**Hibrid changelog:** központi CHANGELOG.md (gyökér) + komponens changelogok. Feature branchen csak komponens; release-nél mindkettő. Részletek: [Changelog Management][Changelog Management].

## Dokumentációs szabványok

Conventional Commits, Semantic Versioning, Keep a Changelog, GitVersion. Lásd az egyes dokumentumokat.

[English]: ./README.md
[Branching Strategy]: ./Branching-Strategy.hu.md
[Commit Conventions]: ./Commit-Conventions.hu.md
[Release Process]: ./Release-Process.hu.md
[GitVersion Integration]: ./GitVersion-Integration.hu.md
[Changelog Management]: ./Changelog-Management.hu.md
[GitVersion]: https://gitversion.net/
