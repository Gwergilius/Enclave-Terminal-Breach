# GitVersion Integration

Automated semantic versioning using GitVersion for the Enclave Terminal Breach project.

## Overview

[GitVersion] is a tool that generates semantic version numbers based on git history, branches, and tags. This project uses GitVersion to:

- Automate version numbering across multiple platforms (SPARROW, RAVEN, GHOST, ECHELON)
- Maintain lore-aligned version numbers (v0.x, v1.x, v2.x)
- Generate pre-release versions for feature branches
- Include build metadata (commit count, SHA, branch name)
- Ensure consistent versioning in CI/CD pipelines

## Why GitVersion?

**Manual versioning challenges:**
- Easy to forget version bumps
- Inconsistent pre-release numbering
- Build numbers out of sync
- Merge conflicts in version files

**GitVersion benefits:**
- ✅ Automatic version calculation from git history
- ✅ Deterministic (same git state = same version)
- ✅ Pre-release and build metadata support
- ✅ CI/CD integration
- ✅ Works with multiple platforms via tag prefixes

## Installation

### Global Tool (Recommended)

```bash
dotnet tool install --global GitVersion.Tool
```

Verify installation:
```bash
dotnet-gitversion --version
```

### Local Tool (Per-repository)

```bash
dotnet new tool-manifest
dotnet tool install GitVersion.Tool
```

Usage:
```bash
dotnet tool run dotnet-gitversion
```

## Configuration

### GitVersion.yml

The `GitVersion.yml` file at the repository root configures versioning behavior:

```yaml
# GitVersion.yml
mode: Mainline
tag-prefix: '[a-z]+-v'
major-version-bump-message: '\+semver:\s?(breaking|major)'
minor-version-bump-message: '\+semver:\s?(feature|minor)'
patch-version-bump-message: '\+semver:\s?(fix|patch)'
no-bump-message: '\+semver:\s?none'
legacy-semver-padding: 4
build-metadata-padding: 4
commits-since-version-source-padding: 4
tag-pre-release-weight: 60000
commit-message-incrementing: Enabled

branches:
  main:
    regex: ^main$
    mode: ContinuousDelivery
    tag: ''
    increment: Patch
    prevent-increment-of-merged-branch-version: true
    track-merge-target: false
    is-release-branch: true
    is-mainline: true

  feature:
    regex: ^feature[/-]
    mode: ContinuousDelivery
    tag: alpha
    increment: Minor
    prevent-increment-of-merged-branch-version: false
    is-release-branch: false

  phase:
    regex: ^phase[/-]
    mode: ContinuousDelivery
    tag: beta
    increment: Minor
    prevent-increment-of-merged-branch-version: false
    is-release-branch: false

  docs:
    regex: ^docs[/-]
    mode: ContinuousDelivery
    tag: ''
    increment: None
    prevent-increment-of-merged-branch-version: true
    is-release-branch: false

  hotfix:
    regex: ^hotfix[/-]
    mode: ContinuousDelivery
    tag: beta
    increment: Patch
    prevent-increment-of-merged-branch-version: false
    is-release-branch: false

ignore:
  sha: []
  # Ignore commits before project migration to GitHub
  commits-before: 2026-02-01T00:00:00

merge-message-formats: {}
```

### Configuration Breakdown

**Mode: Mainline**
- Simplified versioning model
- main branch is always release-ready
- Feature branches get pre-release tags

**Tag Prefix Pattern**
```yaml
tag-prefix: '[a-z]+-v'
```
Matches our phase tags:
- `sparrow-v0.1.0`
- `raven-v0.4.0`
- `ghost-v1.2.4`
- `echelon-v2.1.7`

**Version Bump Messages**
Control version increments via commit messages:
```yaml
major-version-bump-message: '\+semver:\s?(breaking|major)'
minor-version-bump-message: '\+semver:\s?(feature|minor)'
patch-version-bump-message: '\+semver:\s?(fix|patch)'
no-bump-message: '\+semver:\s?none'
```

**Branch-Specific Configuration**

| Branch Pattern | Pre-release Tag | Increment | Example Version |
|----------------|-----------------|-----------|-----------------|
| `main` | None | Patch | `0.1.1` |
| `feature/*` | `alpha` | Minor | `0.2.0-alpha.3+5` |
| `phase/*` | `beta` | Minor | `1.1.0-beta.1+2` |
| `docs/*` | None | None | `0.1.0` (unchanged) |
| `hotfix/*` | `beta` | Patch | `0.1.1-beta.1` |

## Multi-Platform Versioning

### Tag Prefix per Platform

Each platform uses a unique tag prefix:

```bash
# SPARROW
git tag sparrow-v0.1.0

# RAVEN  
git tag raven-v0.4.0

# GHOST
git tag ghost-v1.2.4

# ECHELON
git tag echelon-v2.1.7
```

### Determining Version for Specific Platform

GitVersion finds the **most recent tag** matching the pattern:

```bash
# On main branch with these tags:
# sparrow-v0.1.0 (5 commits ago)
# raven-v0.4.0 (2 commits ago)

dotnet-gitversion
# Returns version based on raven-v0.4.0 (most recent)
# Output: 0.4.1
```

### Per-Project Version (Advanced)

For more control, specify tag prefix per project:

**Directory.Build.props** (in project directory):
```xml
<Project>
  <PropertyGroup>
    <GitVersionTagPrefix>sparrow-v</GitVersionTagPrefix>
  </PropertyGroup>
</Project>
```

Or use environment variable:
```bash
GITVERSION_TAG_PREFIX=sparrow-v dotnet-gitversion
```

## Version Output

### JSON Output

```bash
dotnet-gitversion
```

Output:
```json
{
  "Major": 0,
  "Minor": 1,
  "Patch": 1,
  "PreReleaseTag": "alpha.3",
  "PreReleaseTagWithDash": "-alpha.3",
  "PreReleaseLabel": "alpha",
  "PreReleaseLabelWithDash": "-alpha",
  "PreReleaseNumber": 3,
  "WeightedPreReleaseNumber": 30003,
  "BuildMetaData": "5.Branch.feature-stdin.Sha.a1b2c3d4",
  "BuildMetaDataPadded": "0005",
  "FullBuildMetaData": "5.Branch.feature-stdin.Sha.a1b2c3d4e5f67890",
  "MajorMinorPatch": "0.1.1",
  "SemVer": "0.1.1-alpha.3",
  "LegacySemVer": "0.1.1-alpha3",
  "LegacySemVerPadded": "0.1.1-alpha0003",
  "AssemblySemVer": "0.1.1.0",
  "AssemblySemFileVer": "0.1.1.0",
  "FullSemVer": "0.1.1-alpha.3+5",
  "InformationalVersion": "0.1.1-alpha.3+5.Branch.feature-stdin.Sha.a1b2c3d4e5f67890",
  "BranchName": "feature/stdin-input",
  "EscapedBranchName": "feature-stdin-input",
  "Sha": "a1b2c3d4e5f67890abcdef1234567890abcdef12",
  "ShortSha": "a1b2c3d4",
  "CommitDate": "2026-02-15"
}
```

### Common Version Properties

**SemVer** (`0.1.1-alpha.3`)
- Semantic version without build metadata
- Use for NuGet packages

**FullSemVer** (`0.1.1-alpha.3+5`)
- Full semantic version with build metadata
- Use for display purposes

**InformationalVersion** (`0.1.1-alpha.3+5.Branch.feature-stdin.Sha.a1b2c3d4`)
- Complete version with all metadata
- Use for assembly informational version

**AssemblySemVer** (`0.1.1.0`)
- .NET assembly version (4 parts)
- Use for AssemblyVersion attribute

**MajorMinorPatch** (`0.1.1`)
- Base version without pre-release or metadata
- Use for simple version comparisons

### Query Specific Variable

```bash
# Get SemVer only
dotnet-gitversion /showvariable SemVer
# Output: 0.1.1-alpha.3

# Get InformationalVersion
dotnet-gitversion /showvariable InformationalVersion
# Output: 0.1.1-alpha.3+5.Branch.feature-stdin.Sha.a1b2c3d4
```

## MSBuild Integration

### Automatic Assembly Version

GitVersion can automatically set assembly versions during build:

**Install GitVersion MSBuild package:**

```bash
dotnet add package GitVersion.MsBuild
```

**Project file:**

```xml
<!-- Enclave.Echelon.SPARROW.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <!-- GitVersion will populate these automatically -->
    <Version>0.0.0</Version>
    <AssemblyVersion>0.0.0.0</AssemblyVersion>
    <FileVersion>0.0.0.0</FileVersion>
    <InformationalVersion>0.0.0</InformationalVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="GitVersion.MsBuild" Version="5.12.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

During build, GitVersion replaces version properties:
```xml
<Version>0.1.1-alpha.3</Version>
<AssemblyVersion>0.1.1.0</AssemblyVersion>
<FileVersion>0.1.1.0</FileVersion>
<InformationalVersion>0.1.1-alpha.3+5.Branch.feature-stdin.Sha.a1b2c3d4</InformationalVersion>
```

### Solution-Wide Version (Directory.Build.props)

Share version across all projects:

```xml
<!-- Directory.Build.props (repository root) -->
<Project>
  <ItemGroup>
    <PackageReference Include="GitVersion.MsBuild" Version="5.12.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

All projects automatically get GitVersion integration.

## CI/CD Integration

### GitHub Actions

**Install and execute GitVersion:**

```yaml
# .github/workflows/build.yml
name: Build and Test

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout
      uses: actions/checkout@v4
      with:
        fetch-depth: 0  # Required for GitVersion
    
    - name: Install GitVersion
      uses: gittools/actions/gitversion/setup@v0.10.2
      with:
        versionSpec: '5.x'
    
    - name: Determine Version
      id: gitversion
      uses: gittools/actions/gitversion/execute@v0.10.2
    
    - name: Display Version
      run: |
        echo "SemVer: ${{ steps.gitversion.outputs.semVer }}"
        echo "FullSemVer: ${{ steps.gitversion.outputs.fullSemVer }}"
        echo "InformationalVersion: ${{ steps.gitversion.outputs.informationalVersion }}"
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 10.0.x
    
    - name: Build
      run: dotnet build -c Release /p:Version=${{ steps.gitversion.outputs.semVer }}
    
    - name: Test
      run: dotnet test -c Release --no-build
```

**Use version in artifact names:**

```yaml
    - name: Publish
      run: dotnet publish -c Release -o ./publish
    
    - name: Upload Artifact
      uses: actions/upload-artifact@v4
      with:
        name: sparrow-${{ steps.gitversion.outputs.semVer }}
        path: ./publish
```

### Version in Build Logs

GitVersion outputs to build logs:

```
GitVersion: 5.12.0
Calculated version: 0.1.1-alpha.3+5
  Major: 0
  Minor: 1
  Patch: 1
  PreReleaseTag: alpha.3
  BuildMetaData: 5.Branch.feature-stdin.Sha.a1b2c3d4
```

## Version Control with Commit Messages

### Semantic Commit Messages

Control version bumps via commit message footer:

**MAJOR version bump:**
```bash
git commit -m "feat(sparrow)!: redesign solver interface

BREAKING CHANGE: ISolver.GetBestGuess() signature changed.

+semver: major"
```

**MINOR version bump:**
```bash
git commit -m "feat(sparrow): add tie-breaker logic

Implements worst-case bucket minimization.

+semver: minor"
```

**PATCH version bump:**
```bash
git commit -m "fix(sparrow): resolve buffer overflow

+semver: patch"
```

**No version bump:**
```bash
git commit -m "docs(sparrow): update README

Documentation only, no code changes.

+semver: none"
```

### Automatic Detection

GitVersion can also detect version bumps from conventional commit types:

- `feat:` → MINOR bump
- `fix:` → PATCH bump
- `!` or `BREAKING CHANGE:` → MAJOR bump

**Enable in GitVersion.yml:**
```yaml
commit-message-incrementing: Enabled
```

## Workflow Examples

### Feature Development

```bash
# Start feature
git checkout -b feature/stdin-input

# Make changes
git commit -m "feat(sparrow): add stdin password input

+semver: minor"

# Build shows version
dotnet build
# Output: Building SPARROW v0.2.0-alpha.1+1

# More commits
git commit -m "test(sparrow): add stdin validation tests"

dotnet build
# Output: Building SPARROW v0.2.0-alpha.1+2
```

### Release Workflow

```bash
# On main branch, ready to release
dotnet-gitversion /showvariable MajorMinorPatch
# Output: 0.1.1

# Create release
git tag sparrow-v0.1.1 -m "SPARROW v0.1.1 - Bug fixes"
git push origin sparrow-v0.1.1

# Next commit on main
git commit -m "chore: update dependencies"
dotnet-gitversion /showvariable SemVer
# Output: 0.1.2 (next patch version)
```

### Hotfix Workflow

```bash
# Create hotfix branch from tag
git checkout -b hotfix/sparrow-buffer-fix sparrow-v0.1.0

# Fix bug
git commit -m "fix(sparrow): resolve buffer overflow

+semver: patch"

dotnet build
# Output: Building SPARROW v0.1.1-beta.1+1

# Merge to main and tag
git checkout main
git merge hotfix/sparrow-buffer-fix
git tag sparrow-v0.1.1 -m "Hotfix: buffer overflow"
```

## Troubleshooting

### No Version Calculated

**Problem:** GitVersion returns `0.0.0`

**Causes:**
- No tags in repository
- Tag doesn't match `tag-prefix` pattern
- Shallow clone (missing git history)

**Solutions:**
```bash
# Check for tags
git tag -l

# Create initial tag if none exist
git tag sparrow-v0.1.0

# Ensure full clone (not shallow)
git fetch --unshallow
```

### Wrong Version Calculated

**Problem:** Version doesn't match expectation

**Diagnosis:**
```bash
# Show verbose output
dotnet-gitversion /diag

# Check which tag is being used
dotnet-gitversion /showvariable VersionSourceSha
```

**Common causes:**
- Multiple tags on same commit
- Tag on wrong branch
- Incorrect tag prefix pattern

### CI/CD Version Mismatch

**Problem:** CI shows different version than local

**Solution:**
```yaml
# Ensure full fetch in CI
- uses: actions/checkout@v4
  with:
    fetch-depth: 0  # Get all history
```

### Version Not Updating

**Problem:** Version stays same after commits

**Check:**
```bash
# Verify commits-since-version
dotnet-gitversion /showvariable CommitsSinceVersionSource

# Should be > 0
```

If 0, check:
- Commit message might have `+semver: none`
- Branch configuration might have `increment: None`

## Best Practices

### Do:
- ✅ Use annotated tags for releases (`git tag -a`)
- ✅ Include `fetch-depth: 0` in CI checkout
- ✅ Commit GitVersion.yml to repository
- ✅ Test GitVersion locally before CI
- ✅ Use semantic commit messages

### Don't:
- ❌ Mix lightweight and annotated tags
- ❌ Use same tag on multiple branches
- ❌ Rely on default configuration (always use GitVersion.yml)
- ❌ Force-push tagged commits
- ❌ Tag every commit (only releases)

## Migration from Manual Versioning

### Current State

If you have projects with hardcoded versions:

```xml
<Version>0.1.0</Version>
<AssemblyVersion>0.1.0.0</AssemblyVersion>
```

### Migration Steps

1. **Tag current release:**
```bash
git tag sparrow-v0.1.0
```

2. **Add GitVersion.MsBuild:**
```bash
dotnet add package GitVersion.MsBuild
```

3. **Update project file:**
```xml
<!-- Before -->
<Version>0.1.0</Version>

<!-- After -->
<Version>0.0.0</Version> <!-- GitVersion will override -->
```

4. **Verify:**
```bash
dotnet build
# Check build output for calculated version
```

5. **Commit:**
```bash
git commit -m "chore(build): migrate to GitVersion

+semver: none"
```

## References

- [GitVersion Documentation] - Official documentation
- [Semantic Versioning] - Version numbering specification
- [Conventional Commits] - Commit message standard
- [GitVersion Configuration] - Configuration reference

[GitVersion]: https://gitversion.net/
[GitVersion Documentation]: https://gitversion.net/docs/
[GitVersion Configuration]: https://gitversion.net/docs/reference/configuration
[Semantic Versioning]: https://semver.org/
[Conventional Commits]: https://www.conventionalcommits.org/
