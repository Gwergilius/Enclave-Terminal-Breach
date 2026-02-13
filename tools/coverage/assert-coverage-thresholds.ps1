#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Asserts that line and branch coverage meet minimum thresholds (e.g. for CI).

.DESCRIPTION
    Reads a coverage XML (OpenCover or Cobertura), optionally generates Cobertura
    via ReportGenerator, then checks line-rate and branch-rate against thresholds.
    Exits 0 if both pass, 1 otherwise.

.PARAMETER CoverageXmlPath
    Path to the coverage XML file (e.g. TestResults/coverage.xml from run-coverage.ps1).

.PARAMETER LineMinimum
    Minimum line coverage percentage (0-100). Default: 80.

.PARAMETER BranchMinimum
    Minimum branch coverage percentage (0-100). Default: 95.

.PARAMETER ReportGenerator
    Optional path to reportgenerator. If not set, uses reportgenerator from PATH.
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$CoverageXmlPath,
    [ValidateRange(0, 100)]
    [int]$LineMinimum = 80,
    [ValidateRange(0, 100)]
    [int]$BranchMinimum = 95,
    [string]$ReportGenerator = "reportgenerator"
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path $CoverageXmlPath)) {
    Write-Error "Coverage file not found: $CoverageXmlPath"
    exit 1
}

[xml]$doc = Get-Content -Path $CoverageXmlPath -Raw
$coverage = $doc.coverage

# If not already Cobertura (has line-rate on root), convert via ReportGenerator
if (-not $coverage -or $coverage.PSObject.Properties['line-rate'] -eq $null) {
    $tempDir = Join-Path ([System.IO.Path]::GetTempPath()) "coverage-threshold-check-$(Get-Random)"
    New-Item -ItemType Directory -Path $tempDir -Force | Out-Null
    try {
        & $ReportGenerator "-reports:$CoverageXmlPath" "-targetdir:$tempDir" "-reporttypes:Cobertura" "-assemblyfilters:+Enclave*;-*.Tests;-*.Test.*" "-verbosity:Warning" 2>&1 | Out-Null
        if ($LASTEXITCODE -ne 0) {
            Write-Error "ReportGenerator failed (exit $LASTEXITCODE)"
            exit 1
        }
        $coberturaFile = Get-ChildItem -Path $tempDir -Filter "*.xml" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
        if (-not $coberturaFile) {
            Write-Error "No Cobertura XML produced under $tempDir"
            exit 1
        }
        [xml]$doc = Get-Content -Path $coberturaFile.FullName -Raw
        $coverage = $doc.coverage
    }
    finally {
        if (Test-Path $tempDir) { Remove-Item -Path $tempDir -Recurse -Force -ErrorAction SilentlyContinue }
    }
}

if (-not $coverage) {
    Write-Error "Cobertura XML has no 'coverage' root element (or ReportGenerator could not convert input)"
    exit 1
}

$lineRate = [double]$coverage.GetAttribute('line-rate')
$branchRate = [double]$coverage.GetAttribute('branch-rate')
$linePct = [math]::Round($lineRate * 100, 2)
$branchPct = [math]::Round($branchRate * 100, 2)

$lineOk = $linePct -ge $LineMinimum
$branchOk = $branchPct -ge $BranchMinimum

Write-Host "Line coverage:   $linePct% (minimum: $LineMinimum%) - $(if ($lineOk) { 'PASS' } else { 'FAIL' })"
Write-Host "Branch coverage: $branchPct% (minimum: $BranchMinimum%) - $(if ($branchOk) { 'PASS' } else { 'FAIL' })"

if (-not $lineOk -or -not $branchOk) {
    Write-Host "Coverage thresholds not met." -ForegroundColor Red
    exit 1
}
Write-Host "Coverage thresholds met." -ForegroundColor Green
exit 0
