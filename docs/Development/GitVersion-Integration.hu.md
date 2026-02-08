# GitVersion integráció

**[English]** | Magyar

Automatizált szemantikus verziókezelés a git history, branchek és tag-ek alapján.

**Telepítés:** `dotnet tool install --global GitVersion.Tool` vagy repo lokális tool. **Konfig:** GitVersion.yml a gyökérben: mode Mainline, tag-prefix `[a-z]+-v`, version bump üzenetek (+semver: major/minor/patch/none), branch konfig (main, feature, phase, docs, hotfix). Main: Patch; feature: alpha, Minor; phase: beta, Minor; docs: None; hotfix: beta, Patch.

**Több platform:** platformonként egyedi tag (sparrow-v, raven-v, stb.). Verzió kimenet: dotnet-gitversion (JSON: SemVer, FullSemVer, AssemblySemVer, stb.). MSBuild: GitVersion.MsBuild csomag. CI (GitHub Actions): checkout fetch-depth: 0, GitVersion action, Version a buildben. Commit üzenet: +semver: major/minor/patch/none. Hibaelhárítás: nincs tag / shallow clone / rossz prefix.

[English]: ./GitVersion-Integration.md
