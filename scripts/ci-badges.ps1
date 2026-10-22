<#
.SYNOPSIS
    Writes the shields.io endpoint JSON for the CI test and coverage badges.

.DESCRIPTION
    The CI runs four test suites and publishes one TRX per project and TFM plus one cobertura file per
    project and TFM. This script reads them, groups by suite, and writes eight small JSON files that
    the README badges point at through raw.githubusercontent.com. It never runs tests and never talks
    to a network: parsing, grouping and colour are all local.

.PARAMETER Results
    Directory holding the downloaded CI artifacts: TRX files at the top and coverage/<project>/*.xml.

.PARAMETER Out
    Directory the badge JSON files are written to.

.EXAMPLE
    ./scripts/ci-badges.ps1 -Results TestResults -Out badges
#>
param(
    [string]$Results = "TestResults",
    [string]$Out = "badges"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# The groups are the four CI matrix suites. Every test project belongs to exactly one of them, so a
# red badge and a red check name the same thing.
$Groups = [ordered]@{
    components = @("Flare.Components.Tests")
    core       = @("Flare.Core.Tests")
    guards     = @("Flare.Guards.Tests")
    packages   = @(
        "Flare.ApiDocGen.Tests",
        "Flare.Components.Barcode.Tests",
        "Flare.Components.Carousel.Tests",
        "Flare.Components.Gantt.Tests",
        "Flare.Components.IDE.Tests",
        "Flare.Components.Kanban.Tests",
        "Flare.Components.Media.Tests",
        "Flare.Components.QrCode.Tests",
        "Flare.Components.Query.Tests",
        "Flare.Components.RichTextEditor.Tests",
        "Flare.Components.Transfer.Tests",
        "Flare.Integration.Tests"
    )
}

# A coverage number nobody should trust is worse than no number: these are the bands the badge paints.
$CoverageColors = @(
    @{ Floor = 90; Color = "brightgreen" },
    @{ Floor = 75; Color = "green" },
    @{ Floor = 60; Color = "yellowgreen" },
    @{ Floor = 40; Color = "yellow" },
    @{ Floor = 0;  Color = "orangered" }
)

function Get-ShortName([string]$Project) {
    $name = $Project -replace "^Flare\.", "" -replace "\.Tests$", "" -replace "^Components\.", ""
    return $name.ToLowerInvariant()
}

function Get-TrxCounts([string]$Path) {
    [xml]$document = Get-Content -LiteralPath $Path -Raw
    $counters = $document.SelectSingleNode("//*[local-name()='Counters']")
    if ($null -eq $counters) { return @{ Total = 0; Passed = 0; Failed = 0 } }
    return @{
        Total  = [int]$counters.total
        Passed = [int]$counters.passed
        Failed = [int]$counters.failed
    }
}

function New-Badge([string]$Label, [string]$Message, [string]$Color) {
    return [ordered]@{
        schemaVersion = 1
        label         = $Label
        message       = $Message
        color         = $Color
        # raw.githubusercontent and shields both cache; this is the ceiling, not a guarantee.
        cacheSeconds  = 300
    }
}

function Get-TestBadge([string]$Group, [string[]]$Projects) {
    $total = 0; $passed = 0; $failed = 0
    $failing = @()

    foreach ($project in $Projects) {
        $projectFailed = 0
        foreach ($trx in @(Get-ChildItem -LiteralPath $Results -Filter "$project`_*.trx" -File -ErrorAction SilentlyContinue)) {
            $counts = Get-TrxCounts $trx.FullName
            $total += $counts.Total
            $passed += $counts.Passed
            $failed += $counts.Failed
            $projectFailed += $counts.Failed
        }
        if ($projectFailed -gt 0) { $failing += Get-ShortName $project }
    }

    $label = "$Group tests"
    if ($total -eq 0) { return New-Badge $label "no results" "lightgrey" }
    if ($failed -gt 0) {
        # One broken project is named, because "3 failed" does not say which suite to open.
        $message = if ($failing.Count -eq 1) { "$($failing[0]) failed" } else { "$failed failed / $total" }
        return New-Badge $label $message "red"
    }
    return New-Badge $label "$passed passed" "brightgreen"
}

function Get-CoverageBadge([string]$Group, [string[]]$Projects) {
    $covered = 0; $valid = 0
    foreach ($project in $Projects) {
        $directory = Join-Path (Join-Path $Results "coverage") $project
        foreach ($xmlFile in @(Get-ChildItem -LiteralPath $directory -Filter "*.xml" -File -ErrorAction SilentlyContinue)) {
            [xml]$document = Get-Content -LiteralPath $xmlFile.FullName -Raw
            $root = $document.DocumentElement
            $covered += [int]$root.GetAttribute("lines-covered")
            $valid += [int]$root.GetAttribute("lines-valid")
        }
    }

    if ($valid -eq 0) { return New-Badge "$Group coverage" "no data" "lightgrey" }

    $percent = 100.0 * $covered / $valid
    $color = ($CoverageColors | Where-Object { $percent -ge $_.Floor } | Select-Object -First 1).Color
    return New-Badge "$Group coverage" ("{0:0}%" -f $percent) $color
}

New-Item -ItemType Directory -Path $Out -Force | Out-Null
$Utf8NoBom = [System.Text.UTF8Encoding]::new($false)
foreach ($group in $Groups.Keys) {
    $projects = $Groups[$group]
    $test = Get-TestBadge $group $projects
    $coverage = Get-CoverageBadge $group $projects
    # shields reads the file byte for byte; a BOM would make it invalid JSON.
    [System.IO.File]::WriteAllText(
        (Join-Path $Out "tests-$group.json"), ($test | ConvertTo-Json -Compress), $Utf8NoBom)
    [System.IO.File]::WriteAllText(
        (Join-Path $Out "coverage-$group.json"), ($coverage | ConvertTo-Json -Compress), $Utf8NoBom)
}
