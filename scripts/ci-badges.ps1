<#
.SYNOPSIS
    Writes the shields.io endpoint JSON for the CI test and coverage badges.

.DESCRIPTION
    The CI runs four test suites and publishes one TRX per project and TFM plus one cobertura file per
    project and TFM. This script reads them, groups by suite, and writes the badge JSON files the
    README points at through raw.githubusercontent.com, plus one coverage details file per suite. It
    never runs tests and never talks to a network: parsing, grouping and colour are all local.

    Coverage is a UNION, not a sum. The same shared assembly appears in several suites and in every
    TFM of each suite, so adding up the roots of the reports counted the same uncovered line once per
    report and made the badge read far below the real number. Lines are now keyed by module, source
    file and line number inside a TFM; the badge itself is the source-line union across TFMs, and the
    per-TFM numbers and the categories (authored, generated, tool, test) go to a details JSON.

.PARAMETER Results
    Directory holding the downloaded CI artifacts. TRX files may sit anywhere below it. Coverage is
    read from <project>/<tfm>/coverage.cobertura.*.xml (current CI) or from the older flat
    coverage/<project>/*.xml layout.

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

$RepoRoot = Split-Path -Parent $PSScriptRoot

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
        # The current CI writes one TRX per project and TFM below TestResults/<project>/<tfm>/.
        foreach ($trx in @(Get-ChildItem -LiteralPath $Results -Recurse -Filter "$project`_*.trx" -File -ErrorAction SilentlyContinue)) {
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

# A cobertura filename may be relative to the report's <source>, or absolute on the machine that
# produced it, and coverlet writes it relative to the src directory in some reports and to the repo
# root in others. Either way the key has to be the repository-relative path, or the same file counts
# twice under two spellings and the union doubles.
$script:SourceRootCache = @{}
function Get-SourcePath([string]$File, [string[]]$Sources) {
    $path = ($File -replace '\\', '/') -replace '^\./', ''
    foreach ($source in $Sources) {
        $root = ($source -replace '\\', '/').TrimEnd('/') + '/'
        if ($path.StartsWith($root, [System.StringComparison]::OrdinalIgnoreCase)) {
            $path = $path.Substring($root.Length)
            break
        }
    }
    if ($path -match '^[A-Za-z]:/|^/') {
        $match = [System.Text.RegularExpressions.Regex]::Match($path, '(?i)/(src|tests|tools|samples)/')
        if ($match.Success) { $path = $path.Substring($match.Index + 1) }
    }
    if ($path -notmatch '^(src|tests|tools|samples)/') {
        $first = $path.Split('/')[0]
        if ($first -and $path -notmatch '^/') {
            if (-not $script:SourceRootCache.ContainsKey($first)) {
                $script:SourceRootCache[$first] = Test-Path -LiteralPath (Join-Path $RepoRoot "src/$first") -PathType Container
            }
            if ($script:SourceRootCache[$first]) { return "src/$path" }
        }
    }
    return $path
}

function Get-CoverageCategory([string]$File, [string]$Module) {
    $path = $File -replace '\\', '/'
    if ($path.StartsWith('tests/')) { return 'test' }
    if ($path.StartsWith('tools/') -or $Module -notmatch '^Flare') { return 'tool' }
    if ($path -match '(?i)(^|/)(obj|bin)/' -or $path -match '(?i)\.(g|generated)\.cs$' -or $path -match '(?i)\.Designer\.cs$') {
        return 'generated'
    }
    $full = Join-Path $RepoRoot $path
    if (Test-Path -LiteralPath $full -PathType Leaf) {
        try {
            $text = [System.IO.File]::ReadAllText($full)
            $head = $text.Substring(0, [Math]::Min(1200, $text.Length))
            if ($head -match '(?i)<auto-generated') { return 'generated' }
        }
        catch { }
    }
    return 'authored'
}

function Get-BranchCounts([string]$Condition) {
    $match = [System.Text.RegularExpressions.Regex]::Match($Condition, '\((\d+)/(\d+)\)')
    if (-not $match.Success) { return $null }
    return @{ Covered = [int]$match.Groups[1].Value; Valid = [int]$match.Groups[2].Value }
}

# Current CI layout: <project>/<tfm>/coverage.cobertura.*.xml. Older artifacts kept the files flat
# under coverage/<project>/ with the TFM lost; those are still readable and report as "unknown".
function Get-ProjectCoverageFiles([string]$Project) {
    $entries = @()
    $newRoot = Join-Path $Results $Project
    if (Test-Path -LiteralPath $newRoot -PathType Container) {
        foreach ($tfmDirectory in @(Get-ChildItem -LiteralPath $newRoot -Directory -ErrorAction SilentlyContinue)) {
            foreach ($file in @(Get-ChildItem -LiteralPath $tfmDirectory.FullName -Filter "coverage.cobertura.*.xml" -File -ErrorAction SilentlyContinue)) {
                $entries += [pscustomobject]@{ File = $file.FullName; Tfm = $tfmDirectory.Name }
            }
        }
    }
    if ($entries.Count -eq 0) {
        $oldRoot = Join-Path (Join-Path $Results "coverage") $Project
        foreach ($file in @(Get-ChildItem -LiteralPath $oldRoot -Filter "*.xml" -File -ErrorAction SilentlyContinue)) {
            $entries += [pscustomobject]@{ File = $file.FullName; Tfm = "unknown" }
        }
    }
    return $entries
}

# The production assemblies a complete report should mention. Project names match assembly names;
# the Roslyn generator is build-time only and is not part of the measured surface.
function Get-ExpectedModules {
    $modules = @()
    $src = Join-Path $RepoRoot "src"
    if (Test-Path -LiteralPath $src -PathType Container) {
        foreach ($directory in @(Get-ChildItem -LiteralPath $src -Directory)) {
            $project = Join-Path $directory.FullName ($directory.Name + ".csproj")
            if ((Test-Path -LiteralPath $project -PathType Leaf) -and $directory.Name -notlike "*.Generators") {
                $modules += $directory.Name
            }
        }
    }
    return $modules
}

function Get-CoverageBadge([string]$Group, [string[]]$Projects) {
    $tfmFiles = @{}     # "tfm|module|file" -> lines of that file inside the TFM
    $unionFiles = @{}   # "module|file" -> lines of that file across TFMs
    $branches = @{}     # "tfm|category|module|file|line" -> covered/valid branches of that line
    $observed = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $missingProjects = @()
    $missingTfms = @()
    $inputFiles = 0

    foreach ($project in $Projects) {
        $files = @(Get-ProjectCoverageFiles $project)
        if ($files.Count -eq 0) { $missingProjects += $project; continue }
        # A run that died before writing coverage still leaves its TRX. Its TFM is missing from the
        # union, and a badge computed without it would be quietly narrower than the suite it names.
        $coveredTfms = @($files | ForEach-Object { $_.Tfm })
        $projectRoot = Join-Path $Results $project
        if (Test-Path -LiteralPath $projectRoot -PathType Container) {
            foreach ($tfmDirectory in @(Get-ChildItem -LiteralPath $projectRoot -Directory -ErrorAction SilentlyContinue)) {
                if ($coveredTfms -contains $tfmDirectory.Name) { continue }
                $trx = @(Get-ChildItem -LiteralPath $tfmDirectory.FullName -Filter "*.trx" -File -ErrorAction SilentlyContinue)
                if ($trx.Count -gt 0) { $missingTfms += "$project/$($tfmDirectory.Name)" }
            }
        }
        foreach ($entry in $files) {
            $inputFiles++
            [xml]$document = Get-Content -LiteralPath $entry.File -Raw
            $root = $document.DocumentElement
            $sources = @($root.SelectNodes("sources/source") | ForEach-Object { $_.InnerText })
            foreach ($package in @($root.SelectNodes("packages/package"))) {
                $module = $package.GetAttribute("name")
                [void]$observed.Add($module)
                foreach ($class in @($package.SelectNodes("classes/class"))) {
                    $file = Get-SourcePath $class.GetAttribute("filename") $sources
                    $category = Get-CoverageCategory $file $module
                    $tfmKey = "$($entry.Tfm)|$module|$file"
                    if (-not $tfmFiles.ContainsKey($tfmKey)) {
                        $tfmFiles[$tfmKey] = @{ Tfm = $entry.Tfm; Module = $module; File = $file; Category = $category; Lines = @{} }
                    }
                    $unionKey = "$module|$file"
                    if (-not $unionFiles.ContainsKey($unionKey)) {
                        $unionFiles[$unionKey] = @{ Module = $module; File = $file; Category = $category; Lines = @{} }
                    }
                    foreach ($line in @($class.SelectNodes("lines/line"))) {
                        $number = [int]$line.GetAttribute("number")
                        $hits = [int]$line.GetAttribute("hits")
                        $tfmBucket = $tfmFiles[$tfmKey]
                        if (-not $tfmBucket.Lines.ContainsKey($number) -or $tfmBucket.Lines[$number] -lt $hits) {
                            $tfmBucket.Lines[$number] = $hits
                        }
                        $unionBucket = $unionFiles[$unionKey]
                        if (-not $unionBucket.Lines.ContainsKey($number) -or $unionBucket.Lines[$number] -lt $hits) {
                            $unionBucket.Lines[$number] = $hits
                        }
                        if ($line.GetAttribute("branch") -eq "true") {
                            $counts = Get-BranchCounts $line.GetAttribute("condition-coverage")
                            if ($null -ne $counts) {
                                $branchKey = "$($entry.Tfm)|$category|$module|$file|$number"
                                $existing = $branches[$branchKey]
                                if ($null -eq $existing) {
                                    $branches[$branchKey] = @{ Covered = $counts.Covered; Valid = $counts.Valid }
                                }
                                else {
                                    if ($counts.Covered -gt $existing.Covered) { $existing.Covered = $counts.Covered }
                                    if ($counts.Valid -gt $existing.Valid) { $existing.Valid = $counts.Valid }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    $categories = @{}
    $unionCovered = 0; $unionValid = 0
    foreach ($bucket in $unionFiles.Values) {
        $valid = $bucket.Lines.Count
        $covered = @($bucket.Lines.Values | Where-Object { $_ -gt 0 }).Count
        if (-not $categories.ContainsKey($bucket.Category)) {
            $categories[$bucket.Category] = @{ Files = 0; Covered = 0; Valid = 0 }
        }
        $category = $categories[$bucket.Category]
        $category.Files++
        $category.Covered += $covered
        $category.Valid += $valid
        if ($bucket.Category -eq "authored") {
            $unionCovered += $covered
            $unionValid += $valid
        }
    }

    $perTfm = @{}
    foreach ($bucket in $tfmFiles.Values) {
        if ($bucket.Category -ne "authored") { continue }
        if (-not $perTfm.ContainsKey($bucket.Tfm)) {
            $perTfm[$bucket.Tfm] = @{ Covered = 0; Valid = 0; BranchCovered = 0; BranchValid = 0 }
        }
        $tfm = $perTfm[$bucket.Tfm]
        $tfm.Valid += $bucket.Lines.Count
        $tfm.Covered += @($bucket.Lines.Values | Where-Object { $_ -gt 0 }).Count
    }
    foreach ($branch in $branches.GetEnumerator()) {
        $parts = $branch.Key.Split('|')
        if ($parts[1] -ne "authored" -or -not $perTfm.ContainsKey($parts[0])) { continue }
        $tfm = $perTfm[$parts[0]]
        $tfm.BranchCovered += $branch.Value.Covered
        $tfm.BranchValid += $branch.Value.Valid
    }

    $perTfmOutput = [ordered]@{}
    foreach ($key in ($perTfm.Keys | Sort-Object)) {
        $value = $perTfm[$key]
        $percent = if ($value.Valid -gt 0) { [math]::Round(100.0 * $value.Covered / $value.Valid, 2) } else { 0 }
        $perTfmOutput[$key] = [ordered]@{
            covered = $value.Covered; valid = $value.Valid; percent = $percent
            branchCovered = $value.BranchCovered; branchValid = $value.BranchValid
        }
    }

    $categoriesOutput = [ordered]@{}
    foreach ($key in ($categories.Keys | Sort-Object)) {
        $categoriesOutput[$key] = $categories[$key]
    }

    $percent = if ($unionValid -gt 0) { 100.0 * $unionCovered / $unionValid } else { 0 }
    $missingModules = @(Get-ExpectedModules | Where-Object { -not $observed.Contains($_) })
    $partial = $missingProjects.Count -gt 0 -or $missingTfms.Count -gt 0
    $details = [ordered]@{
        group           = $Group
        sourceLineUnion = [ordered]@{ covered = $unionCovered; valid = $unionValid; percent = [math]::Round($percent, 2) }
        perTfm          = $perTfmOutput
        categories      = $categoriesOutput
        observedModules = @($observed | Sort-Object)
        missingModules  = $missingModules
        missingProjects = $missingProjects
        missingTfms     = $missingTfms
        partial         = $partial
        inputs          = [ordered]@{ files = $inputFiles; projects = $Projects.Count }
    }

    if ($unionValid -eq 0) {
        return [pscustomobject]@{ Badge = (New-Badge "$Group coverage" "no data" "lightgrey"); Details = $details }
    }

    $color = ($CoverageColors | Where-Object { $percent -ge $_.Floor } | Select-Object -First 1).Color
    $message = if ($partial) { "{0:0}% partial" -f $percent } else { "{0:0}%" -f $percent }
    return [pscustomobject]@{ Badge = (New-Badge "$Group coverage" $message $color); Details = $details }
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
        (Join-Path $Out "coverage-$group.json"), ($coverage.Badge | ConvertTo-Json -Compress), $Utf8NoBom)
    # The badge has room for one number; the breakdown lives beside it for a reader who needs it.
    [System.IO.File]::WriteAllText(
        (Join-Path $Out "coverage-$group-details.json"), ($coverage.Details | ConvertTo-Json -Depth 8), $Utf8NoBom)
}
