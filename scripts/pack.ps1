#!/usr/bin/env pwsh
# Packs the publishable Flare library packages into ./artifacts.
#
# Only the library projects under src/ are published (Flare/Flare.Blazor, Flare.Abstractions,
# Flare.Theming, Flare.Infrastructure, Flare.Components, the Flare.Components.* add-on packages, and
# the Flare.Theme.* packages). Gallery, Legacy, tests and tools are marked <IsPackable>false</IsPackable>,
# so `dotnet pack` skips them automatically -- this script just packs every src/*.csproj and lets that
# flag filter. The library packages multi-target net8.0/net9.0/net10.0.
#
# Versioning is driven by MinVer from the git tag (prefix "v"): tag the commit you publish, e.g.
#   git tag v0.0.1
# Without a tag you get a 0.0.0-preview.* version. Pass -Version to override explicitly.
#
# Usage:
#   ./scripts/pack.ps1                 # Release pack to ./artifacts (version from git tag)
#   ./scripts/pack.ps1 -Version 0.0.1  # force the package version
#   ./scripts/pack.ps1 -Configuration Debug

param(
    [string]$Configuration = "Release",
    [string]$Version,
    [string]$Output = "artifacts"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$outDir = Join-Path $root $Output

if (Test-Path $outDir) { Remove-Item "$outDir/*.nupkg", "$outDir/*.snupkg" -ErrorAction SilentlyContinue }
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

# MinVer owns the version, so a plain -p:Version is ignored; override it via MinVerVersionOverride.
$args = @("-c", $Configuration, "-o", $outDir)
if ($Version) { $args += "-p:MinVerVersionOverride=$Version" }

Get-ChildItem -Path (Join-Path $root "src") -Recurse -Filter *.csproj | ForEach-Object {
    Write-Host "==> pack $($_.Name)" -ForegroundColor Cyan
    & dotnet pack $_.FullName @args
    if ($LASTEXITCODE -ne 0) { throw "pack failed for $($_.Name)" }
}

Write-Host "`nPackages written to $outDir:" -ForegroundColor Green
Get-ChildItem "$outDir/*.nupkg" | ForEach-Object { Write-Host "  $($_.Name)" }
