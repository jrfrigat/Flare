#!/usr/bin/env pwsh
# Tests packed libraries in an isolated consumer, without this repository's build/package settings.
# Example: ./scripts/test-package-compatibility.ps1 -Packages artifacts -RunExactRuntime
param(
    [Parameter(Mandatory)][string]$Packages,
    [string[]]$MicrosoftVersions = @('10.0.0', '10.0.13', '10.0.14'),
    [switch]$RunExactRuntime,
    [string]$RuntimeIdentifier = [System.Runtime.InteropServices.RuntimeInformation]::RuntimeIdentifier,
    [switch]$RestoreOnly
)

$ErrorActionPreference = 'Stop'
$packagePath = (Resolve-Path -LiteralPath $Packages).Path
$work = Join-Path ([System.IO.Path]::GetTempPath()) ('flare-package-test-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $work | Out-Null
Write-Host "Consumer and logs: $work"

function Invoke-Dotnet([string[]]$Arguments, [string]$Log) {
    & dotnet @Arguments --tl:off -m:1 -nr:false *> $Log
    if ($LASTEXITCODE -ne 0) {
        Get-Content -LiteralPath $Log | Write-Host
        throw "dotnet $($Arguments[0]) failed; see $Log"
    }
}

# Read ids/versions from the packages themselves, including Flare.Blazor's distinct package id.
$references = foreach ($file in Get-ChildItem -LiteralPath $packagePath -Filter '*.nupkg') {
    $zip = [System.IO.Compression.ZipFile]::OpenRead($file.FullName)
    try {
        $entry = $zip.Entries | Where-Object FullName -Like '*.nuspec' | Select-Object -First 1
        $reader = [System.IO.StreamReader]::new($entry.Open())
        try { [xml]$spec = $reader.ReadToEnd() } finally { $reader.Dispose() }
        $id = $spec.package.metadata.id
        $version = $spec.package.metadata.version
        if ($id -notlike 'Flare.*') { continue }
        "    <PackageReference Include=`"$id`" Version=`"[$version]`" />"
    } finally { $zip.Dispose() }
}
if (-not $references) { throw "No Flare packages found in $packagePath" }

# An explicit empty build/package boundary also makes this safe when TEMP is inside another repo.
'<Project />' | Set-Content -LiteralPath (Join-Path $work 'Directory.Build.props')
'<Project />' | Set-Content -LiteralPath (Join-Path $work 'Directory.Build.targets')
'<Project />' | Set-Content -LiteralPath (Join-Path $work 'Directory.Packages.props')
@'
using System.Globalization;
using System.Reflection;
using Flare.Abstractions;
using Flare.Components;
using Flare.Extensions;
using Flare.Components.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

if (args.Length > 0 && Environment.Version.ToString() != args[0])
    throw new InvalidOperationException($"Expected runtime {args[0]}, got {Environment.Version}");

var services = new ServiceCollection();
services.AddLogging();
services.AddFlare();
await using var provider = services.BuildServiceProvider();
await using var scope = provider.CreateAsyncScope();
var localization = scope.ServiceProvider.GetRequiredService<IFlareLocalization>();
localization.SetCulture(CultureInfo.GetCultureInfo("en-US"));
var localizer = localization.GetLocalizer<FlareStrings>();
if (localizer["Dialog_Cancel"].Value != "Cancel")
    throw new InvalidOperationException("English resource lookup failed");
localization.SetCulture(CultureInfo.GetCultureInfo("ru-RU"));
if (localizer["Dialog_Cancel"].Value == "Cancel" || localizer["Dialog_Cancel"].ResourceNotFound)
    throw new InvalidOperationException("Live localized resource lookup failed");

await using var renderer = new HtmlRenderer(scope.ServiceProvider,
    scope.ServiceProvider.GetRequiredService<ILoggerFactory>());
await renderer.Dispatcher.InvokeAsync(async () =>
{
    var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
    {
        [nameof(FlareText.ChildContent)] = (RenderFragment)(b => b.AddContent(0, "Package consumer")),
        [nameof(FlareText.Element)] = "h1"
    });
    var rendered = await renderer.RenderComponentAsync<FlareText>(parameters);
    var html = rendered.ToHtmlString();
    if (!html.StartsWith("<h1 ") || !html.Contains(">Package consumer</h1>"))
        throw new InvalidOperationException($"Unexpected component output: {html}");
});

// Load every shipped assembly, so an optional package cannot hide a missing runtime dependency.
foreach (var file in Directory.EnumerateFiles(AppContext.BaseDirectory, "Flare*.dll"))
    _ = Assembly.LoadFrom(file).GetTypes();
Console.WriteLine($"PASS: runtime {Environment.Version}; component rendering, localization, package loading");
'@ | Set-Content -LiteralPath (Join-Path $work 'Program.cs')

foreach ($version in $MicrosoftVersions) {
    $consumer = Join-Path $work $version
    New-Item -ItemType Directory -Path $consumer | Out-Null
    Copy-Item -LiteralPath (Join-Path $work 'Program.cs') -Destination $consumer
    $project = Join-Path $consumer 'Consumer.csproj'
    @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <NuGetAudit>false</NuGetAudit>
    <RestorePackagesPath>$work/cache</RestorePackagesPath>
  </PropertyGroup>
  <ItemGroup>
$($references -join "`n")
    <PackageReference Include="Microsoft.AspNetCore.Components.Web" Version="[$version]" />
    <PackageReference Include="Microsoft.Extensions.Localization" Version="[$version]" />
    <PackageReference Include="Microsoft.Extensions.Localization.Abstractions" Version="[$version]" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="[$version]" />
  </ItemGroup>
</Project>
"@ | Set-Content -LiteralPath $project
    $config = Join-Path $consumer 'NuGet.Config'
    $escapedPath = [System.Security.SecurityElement]::Escape($packagePath)
    @"
<configuration>
  <packageSources>
    <clear />
    <add key="packed-flare" value="$escapedPath" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"@ | Set-Content -LiteralPath $config
    Invoke-Dotnet @('restore', $project, '--configfile', $config) (Join-Path $consumer 'restore.log')
    $assets = Get-Content -LiteralPath (Join-Path $consumer 'obj/project.assets.json') -Raw | ConvertFrom-Json -AsHashtable
    foreach ($package in $assets.libraries.Keys) {
        if ($package -match '^Microsoft\.(AspNetCore|Extensions)\.[^/]+/(10\.0\.\d+)$' -and
            [version]$Matches[2] -gt [version]$version) {
            throw "Flare raised the consumer's $version baseline: $package"
        }
    }
    if ($RestoreOnly) {
        Write-Host "PASS: package restore at $version"
        continue
    }
    Invoke-Dotnet @('build', $project, '-c', 'Release', '--no-restore') (Join-Path $consumer 'build.log')
    & dotnet (Join-Path $consumer 'bin/Release/net10.0/Consumer.dll')
    if ($LASTEXITCODE -ne 0) { throw "Consumer execution failed at $version" }
    if ($RunExactRuntime) {
        $output = Join-Path $consumer 'publish'
        Invoke-Dotnet @('publish', $project, '-c', 'Release', '-r', $RuntimeIdentifier,
            '--self-contained', 'true', "-p:RuntimeFrameworkVersion=$version",
            '-p:TargetLatestRuntimePatch=false', '-o', $output) (Join-Path $consumer 'publish.log')
        $exe = if ($IsWindows) { 'Consumer.exe' } else { 'Consumer' }
        & (Join-Path $output $exe) $version
        if ($LASTEXITCODE -ne 0) { throw "Consumer execution failed on exact runtime $version" }
    }
    Write-Host "PASS: packed libraries with Microsoft packages $version"
}
