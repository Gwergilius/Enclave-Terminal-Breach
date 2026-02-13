#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generates a code coverage report for the Enclave Terminal Breach solution.

.DESCRIPTION
    Runs tests with coverage collection and generates an HTML report.
    All test projects in the solution (.sln or .slnx) are included; no hardcoded project list.
    The report excludes test projects and test framework assemblies.

.PARAMETER Filter
    Test filter for dotnet test. Default: Category=UnitTest (only [UnitTest] tests run for coverage).

.PARAMETER OutputPath
    Output directory for reports. Default: TestResults

.PARAMETER SolutionPath
    Directory containing the solution, or full path to the solution file. Default: current directory, then src/ (relative to repo root).

.PARAMETER Module
    If specified, only unit tests marked with TestOf(module name) run, and the report shows coverage only for those types. Multiple modules: comma-separated. Alias: Modules. Default: empty (all unit tests run, full report).

.EXAMPLE
    .\run-coverage.ps1
    # Coverage from unit tests only ([UnitTest] attribute); solution: current dir or src/

.EXAMPLE
    .\run-coverage.ps1 -Module Password
    # Only tests with TestOf(Password) run; report shows coverage for the Password type only

.EXAMPLE
    .\run-coverage.ps1 -Module Password,PasswordValidator
    .\run-coverage.ps1 -Modules Password,PasswordValidator
    # Multiple modules: only their unit tests run; report shows coverage for those types only

.EXAMPLE
    .\run-coverage.ps1 -Filter ""
    # Run all tests for coverage (no filter)

.EXAMPLE
    .\run-coverage.ps1 -OutputPath "CustomTestResults"

.EXAMPLE
    cd src; ..\tools\coverage\run-coverage.ps1 -SolutionPath .
    # Run from src/ (global.json and solution in scope)

.NOTES
    - The script deletes the output folder (e.g. TestResults) before each run
    - dotnet-coverage and reportgenerator global tools are required
    - Supports both .sln and .slnx files
#>

param(
    [string]$Filter = "Category=UnitTest",
    [string]$OutputPath = "TestResults",
    [string]$SolutionPath = "",
    [Alias("Modules")]
    [string]$Module = ""
)

$ErrorActionPreference = 'Stop'

# Resolve output path as absolute (relative to current directory when script is invoked).
# This way it does not depend on drive or later Push-Location.
if (-not [System.IO.Path]::IsPathRooted($OutputPath)) {
    $OutputPath = [System.IO.Path]::GetFullPath((Join-Path (Get-Location).Path $OutputPath))
}

# Console output colors
$Colors = @{
    Info    = "Cyan"
    Success = "Green"
    Warning = "Yellow"
    Error   = "Red"
}

function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Colors[$Color]
}

function Test-Command {
    param([string]$Command)
    try {
        Get-Command $Command -ErrorAction Stop | Out-Null
        return $true
    }
    catch {
        return $false
    }
}

# Repo root: tools/coverage -> tools -> repo root
$ScriptDir = $PSScriptRoot
$RepoRoot = (Get-Item $ScriptDir).Parent.Parent.FullName
$SrcDir = Join-Path $RepoRoot "src"

# Check required tools
Write-ColorOutput "🔍 Checking required tools..." "Info"

if (-not (Test-Command "dotnet")) {
    Write-ColorOutput "❌ dotnet CLI not found!" "Error"
    exit 1
}

if (-not (Test-Command "dotnet-coverage")) {
    Write-ColorOutput "❌ dotnet-coverage not found! Install: dotnet tool install -g dotnet-coverage" "Error"
    exit 1
}

if (-not (Test-Command "reportgenerator")) {
    Write-ColorOutput "❌ reportgenerator not found! Install: dotnet tool install -g ReportGenerator" "Error"
    exit 1
}

Write-ColorOutput "✅ All required tools available" "Success"

# Resolve solution: explicit path, or current directory, or src/
$solutionFile = $null
if ($SolutionPath) {
    $p = $SolutionPath
    if ([System.IO.Path]::GetExtension($p) -in '.sln', '.slnx') {
        $solutionFile = if ([System.IO.Path]::IsPathRooted($p)) { Get-Item $p -ErrorAction Stop } else { Get-Item (Join-Path $RepoRoot $p) -ErrorAction Stop }
    }
    else {
        $dir = if ([System.IO.Path]::IsPathRooted($p)) { $p } else { Join-Path $RepoRoot $p }
        $dir = Get-Item $dir -ErrorAction Stop
        $solutionFile = Get-ChildItem -Path $dir.FullName -Filter "*.slnx" -ErrorAction SilentlyContinue | Select-Object -First 1
        if (-not $solutionFile) { $solutionFile = Get-ChildItem -Path $dir.FullName -Filter "*.sln" -ErrorAction SilentlyContinue | Select-Object -First 1 }
        if (-not $solutionFile) { throw "No .sln or .slnx found in the specified directory: $($dir.FullName)" }
    }
}
else {
    # First: current directory
    $solutionFile = Get-ChildItem -Path . -Filter "*.slnx" -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $solutionFile) { $solutionFile = Get-ChildItem -Path . -Filter "*.sln" -ErrorAction SilentlyContinue | Select-Object -First 1 }
    # Then: src/ relative to repo root
    if (-not $solutionFile) {
        $solutionFile = Get-ChildItem -Path $SrcDir -Filter "*.slnx" -ErrorAction SilentlyContinue | Select-Object -First 1
        if (-not $solutionFile) { $solutionFile = Get-ChildItem -Path $SrcDir -Filter "*.sln" -ErrorAction SilentlyContinue | Select-Object -First 1 }
    }
    if (-not $solutionFile) {
        Write-ColorOutput "❌ No solution file found (current directory or $SrcDir)!" "Error"
        exit 1
    }
}

$solutionFullPath = $solutionFile.FullName
Write-ColorOutput "📁 Solution: $solutionFullPath" "Info"

# Delete output folder
Write-ColorOutput "🧹 Deleting output folder: $OutputPath..." "Info"
if (Test-Path $OutputPath) {
    Remove-Item -Path $OutputPath -Recurse -Force
    Write-ColorOutput "✅ Output folder deleted" "Success"
}
New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null

# When -Module/-Modules is specified, run only unit tests for those modules (TestOf) and restrict report to those types.
# Module can be comma-separated, e.g. -Module=Password,PasswordValidator or -Modules=Password,PasswordValidator
$moduleList = @()
if ($Module) {
    $moduleList = $Module -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ }
}
$effectiveFilter = $Filter
if ($moduleList.Count -gt 0) {
    if ($moduleList.Count -eq 1) {
        $effectiveFilter = "Category=UnitTest&TestOf=$($moduleList[0])"
    }
    else {
        $testOfOr = ($moduleList | ForEach-Object { "TestOf=$_" }) -join '|'
        $effectiveFilter = "Category=UnitTest&($testOfOr)"
    }
    $moduleSummary = $moduleList -join ', '
    Write-ColorOutput "   Module(s): $moduleSummary (only unit tests with TestOf(...) and coverage for these types)" "Info"
}

# Single coverage run for the whole solution (all test projects included)
Write-ColorOutput "📊 Collecting coverage data (solution: all test projects)..." "Info"

$coverageFile = Join-Path $OutputPath "coverage.xml"
$coverageArgs = @(
    "collect",
    "dotnet",
    "test",
    $solutionFullPath,
    "--output", $coverageFile,
    "--output-format", "xml"
)
if ($effectiveFilter) {
    $coverageArgs += "--filter", $effectiveFilter
    Write-ColorOutput "   Filter: $effectiveFilter (only these tests will run)" "Info"
}

Write-ColorOutput "🚀 Running: dotnet-coverage collect dotnet test $solutionFullPath ..." "Info"

Push-Location (Split-Path $solutionFullPath -Parent)
try {
    & dotnet-coverage @coverageArgs
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "❌ Coverage collection failed!" "Error"
        exit $LASTEXITCODE
    }
    Write-ColorOutput "✅ Coverage data collected successfully" "Success"
}
finally {
    Pop-Location
}

if (-not (Test-Path $coverageFile)) {
    Write-ColorOutput "❌ Coverage file was not created: $coverageFile" "Error"
    exit 1
}

# Generate HTML report
Write-ColorOutput "📄 Generating HTML report..." "Info"

# Only include our production assemblies in the report (excludes test projects, test infra, and third-party DLLs).
# Exclude *.Tests and *.Test.* assemblies so only production code appears in the report.
$assemblyFilters = "+Enclave*;-*.Tests;-*.Test.*"
$reportArgs = @(
    "-reports:$coverageFile",
    "-targetdir:$OutputPath/html",
    "-reporttypes:Html",
    "-assemblyfilters:$assemblyFilters",
    "-verbosity:Info"
)
# When -Module/-Modules is specified, restrict report to those types (classes) only.
if ($moduleList.Count -gt 0) {
    $classFilterInclude = ($moduleList | ForEach-Object { "+*.$_" }) -join ';'
    $reportArgs += "-classfilters:$classFilterInclude"
}

Write-ColorOutput "🚀 Running: reportgenerator ..." "Info"

try {
    & reportgenerator @reportArgs
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "❌ HTML report generation failed!" "Error"
        exit 1
    }
    Write-ColorOutput "✅ HTML report generated successfully" "Success"
}
catch {
    Write-ColorOutput "❌ Error generating HTML report: $($_.Exception.Message)" "Error"
    exit 1
}

# Show results
Write-ColorOutput "📋 Results:" "Info"
Write-ColorOutput "   📁 Coverage XML: $coverageFile" "Success"
Write-ColorOutput "   🌐 HTML report: $OutputPath/html/index.html" "Success"

# Open HTML report
$htmlPath = Join-Path $OutputPath "html/index.html"
if (Test-Path $htmlPath) {
    Write-ColorOutput "🌐 Opening HTML report..." "Info"
    Start-Process $htmlPath
}

Write-ColorOutput "🎉 Coverage report generation complete!" "Success"
