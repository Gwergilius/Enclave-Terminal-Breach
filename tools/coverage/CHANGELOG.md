# Coverage tools Changelog

All notable changes to the coverage scripts under `tools/coverage/`.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/). These tools do not use semantic versioning.

## [Unreleased]

## [1.1.0] - 2026-02-14
### Added
- **run-coverage.ps1** – Runs unit tests (by default `Category=UnitTest`), collects coverage with dotnet-coverage, generates HTML report via ReportGenerator, opens report in browser. Supports `-Filter`, `-Module`/`-Modules`, `-OutputPath`, `-SolutionPath`.
- **assert-coverage-thresholds.ps1** – Checks line and branch coverage against minimum percentages (e.g. for CI). Reads coverage XML (Cobertura or converts via ReportGenerator).
- **README.md** – Usage, requirements (PowerShell 7+, dotnet-coverage, ReportGenerator), filtered coverage, module-scoped runs, thresholds.
