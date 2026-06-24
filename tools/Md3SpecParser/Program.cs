using Flare.Tools.Md3SpecParser.Cli;
using Flare.Tools.Md3SpecParser.Configuration;
using Flare.Tools.Md3SpecParser.Downloading;
using Flare.Tools.Md3SpecParser.Generation;
using Flare.Tools.Md3SpecParser.Parsing;

CliOptions options;
try
{
    options = CliOptions.Parse(args);
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    Console.Error.WriteLine();
    Console.Error.WriteLine(CliOptions.HelpText);
    return 2;
}

if (options.ShowHelp)
{
    Console.WriteLine(CliOptions.HelpText);
    return 0;
}

// Resolve config path relative to the working dir or the binary location.
var configPath = ResolveConfigPath(options.ConfigPath);

SpecConfig config;
try
{
    config = ConfigLoader.Load(configPath);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Failed to load config: {ex.Message}");
    return 2;
}

if (options.OutputRoot is not null)
    config.OutputRoot = Path.GetFullPath(options.OutputRoot);

Console.WriteLine($"Config:  {configPath}");
Console.WriteLine($"Output:  {config.OutputRoot}");

using var downloader = new JsonDownloader();
ISpecParser parser = new Md3ExpressiveParser();
var generator = new SpecGenerator(config, parser, downloader);

var allGroups = generator.GroupByType();

// Determine mode (defaults to New; override with --full / --new / --mode).
var mode = options.Mode ?? GenerationMode.NewOnly;

// Determine candidate groups (filter by explicit --types if given).
IReadOnlyList<TypeGroup> candidates = allGroups;
if (options.Types.Count > 0)
{
    var wanted = new HashSet<string>(options.Types, StringComparer.OrdinalIgnoreCase);
    candidates = allGroups.Where(g => wanted.Contains(g.Type)).ToList();

    var missing = options.Types
        .Where(t => !allGroups.Any(g => g.Type.Equals(t, StringComparison.OrdinalIgnoreCase)))
        .ToList();
    if (missing.Count > 0)
        Console.Error.WriteLine($"Warning: unknown type(s) ignored: {string.Join(", ", missing)}");

    if (candidates.Count == 0)
    {
        Console.Error.WriteLine("No matching types in config.");
        return 1;
    }
}

// Determine selection. An explicit interactive pick always regenerates the chosen
// types (the mode only seeds which boxes start checked); non-interactive runs honour
// the mode so --new still skips existing files.
var interactive = !(options.All || options.Types.Count > 0 || options.NonInteractive);
IReadOnlyList<TypeGroup> selected = interactive
    ? InteractivePrompt.AskTypes(candidates, mode)
    : candidates;

if (selected.Count == 0)
{
    Console.WriteLine("Nothing selected. Aborted.");
    return 0;
}

var effectiveMode = interactive ? GenerationMode.Full : mode;

Console.WriteLine();
Console.WriteLine($"Mode:    {effectiveMode}");
Console.WriteLine($"Types:   {string.Join(", ", selected.Select(g => g.Type))}");

Console.WriteLine();
Console.WriteLine("Progress:");
var reporter = new ProgressReporter();
var results = await generator.GenerateAsync(selected, effectiveMode, reporter);

Console.WriteLine();
Console.WriteLine("Results:");
var failed = 0;
var writtenPaths = new List<string>();
var writtenTypes = new List<string>();
foreach (var r in results)
{
    var marker = r.Status switch { "written" => "✓", "skipped" => "-", _ => "✗" };
    if (r.Status == "failed") failed++;
    if (r.Status == "written")
    {
        writtenPaths.Add(r.OutputPath);
        if (!r.Type.StartsWith("palette:", StringComparison.Ordinal))
            writtenTypes.Add(r.Type);
    }
    Console.WriteLine($"  {marker} {r.Type,-16} {r.Status,-8} {r.Detail}");
}

Console.WriteLine();
Console.WriteLine(failed == 0 ? "Done." : $"Done with {failed} failure(s).");

MaybeCommit(writtenPaths, writtenTypes, options, config.OutputRoot);

return failed == 0 ? 0 : 1;

// Offers (or, with --commit, performs) a git commit of the written files.
static void MaybeCommit(IReadOnlyList<string> paths, IReadOnlyList<string> types, CliOptions opts, string workingDir)
{
    if (paths.Count == 0) return;

    bool doCommit;
    if (opts.Commit)
        doCommit = true;
    else if (opts.NonInteractive || Console.IsInputRedirected)
        doCommit = false;
    else
        doCommit = InteractivePrompt.Confirm($"Commit {paths.Count} written file(s) to git?");

    if (!doCommit) return;

    var label = types.Count switch
    {
        0 => "spec",
        <= 6 => string.Join(", ", types.Distinct()),
        _ => $"{types.Distinct().Count()} components",
    };
    var message = $"docs(spec): update {label} via Md3SpecParser";

    var (ok, output) = Git.Commit(paths, message, workingDir);
    Console.WriteLine(ok ? $"Committed: {message}" : $"Commit failed: {output}");
}

// Looks for the config next to the CWD first, then beside the executable.
static string ResolveConfigPath(string path)
{
    if (Path.IsPathRooted(path) && File.Exists(path)) return path;
    if (File.Exists(path)) return Path.GetFullPath(path);

    var beside = Path.Combine(AppContext.BaseDirectory, path);
    return File.Exists(beside) ? beside : Path.GetFullPath(path);
}
