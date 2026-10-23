#!/usr/bin/env pwsh
# Moves each package's unshipped public API into its shipped list when a version is cut.
#
# Every package under src/ that ships keeps PublicAPI.Shipped.txt (what released versions carry) and
# PublicAPI.Unshipped.txt (what the next version adds). The build fails on a public symbol in neither
# file (RS0016) and on a listed symbol that is gone (RS0017). Cutting a version moves the unshipped
# lines into the shipped file: an added line is appended, a "*REMOVED*" line deletes its symbol from
# the shipped file, and the unshipped file is left with its nullable header only.
#
# Usage:
#   ./scripts/ship-public-api.ps1           # ship every package's unshipped API
#   ./scripts/ship-public-api.ps1 -WhatIf   # report the changes without writing

[CmdletBinding(SupportsShouldProcess)]
param()

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$header = "#nullable enable"
$removedPrefix = "*REMOVED*"

foreach ($unshippedFile in Get-ChildItem -Path (Join-Path $root "src") -Filter "PublicAPI.Unshipped.txt" -Recurse) {
    $shippedFile = Join-Path $unshippedFile.DirectoryName "PublicAPI.Shipped.txt"
    $unshipped = @(Get-Content $unshippedFile.FullName | Where-Object { $_ -and $_ -ne $header })
    if ($unshipped.Count -eq 0) { continue }

    $shipped = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    if (Test-Path $shippedFile) {
        Get-Content $shippedFile | Where-Object { $_ -and $_ -ne $header } | ForEach-Object { [void]$shipped.Add($_) }
    }

    $added = 0
    $removed = 0
    foreach ($line in $unshipped) {
        if ($line.StartsWith($removedPrefix, [System.StringComparison]::Ordinal)) {
            if ($shipped.Remove($line.Substring($removedPrefix.Length))) { $removed++ }
        }
        elseif ($shipped.Add($line)) { $added++ }
    }

    $package = Split-Path -Leaf $unshippedFile.DirectoryName
    if ($PSCmdlet.ShouldProcess($package, "ship $added added and $removed removed API lines")) {
        $sorted = [string[]]@($shipped)
        [System.Array]::Sort($sorted, [System.StringComparer]::Ordinal)
        Set-Content -Path $shippedFile -Value (@($header) + $sorted) -Encoding utf8NoBOM
        Set-Content -Path $unshippedFile.FullName -Value $header -Encoding utf8NoBOM
    }
    Write-Host "$package : +$added -$removed"
}
