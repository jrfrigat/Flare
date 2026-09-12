using System.Text.Json;
using Flare.Tools.Md2SpecParser.Configuration;
using Flare.Tools.Md2SpecParser.Downloading;
using Flare.Tools.Md2SpecParser.Generation;

const string help = """
md2spec - renders the Material Design 2 guidelines into docs/spec as markdown.

Usage:
  md2spec [options]

Options:
  --config <path>    Config file (default: spec-config.json beside the binary).
  --out <path>       Override the output root.
  --types <a,b,...>  Only these types (matched case-insensitively).
  --list             Print the configured types and exit.
  --use-proxy        Go through HTTP(S)_PROXY instead of connecting directly.
  -h, --help         This text.

The config names site ROUTES, not page ids: the tool reads m2.material.io/site_meta.js, which maps
every route to the page-data document behind it, so adding a page is one readable line.
""";

var configPath = "spec-config.json";
string? outputRoot = null;
var types = new List<string>();
var listOnly = false;
var useProxy = false;

for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "-h" or "--help":
            Console.WriteLine(help);
            return 0;
        case "--list":
            listOnly = true;
            break;
        case "--use-proxy":
            useProxy = true;
            break;
        case "--config" when i + 1 < args.Length:
            configPath = args[++i];
            break;
        case "--out" when i + 1 < args.Length:
            outputRoot = args[++i];
            break;
        case "--types" when i + 1 < args.Length:
            types.AddRange(args[++i].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            break;
        default:
            Console.Error.WriteLine($"Unknown argument: {args[i]}");
            Console.Error.WriteLine(help);
            return 2;
    }
}

if (!File.Exists(configPath))
{
    var beside = Path.Combine(AppContext.BaseDirectory, configPath);
    if (File.Exists(beside)) configPath = beside;
}

Md2Config config;
try
{
    config = JsonSerializer.Deserialize<Md2Config>(await File.ReadAllTextAsync(configPath))
             ?? throw new InvalidOperationException("empty config");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Failed to load config '{configPath}': {ex.Message}");
    return 2;
}

if (outputRoot is not null) config.OutputRoot = Path.GetFullPath(outputRoot);

var selected = config.Pages.AsEnumerable();
if (types.Count > 0)
{
    var wanted = new HashSet<string>(types, StringComparer.OrdinalIgnoreCase);
    selected = config.Pages.Where(p => wanted.Contains(p.Type));
}
var pages = selected.ToList();

if (listOnly)
{
    foreach (var page in config.Pages)
        Console.WriteLine($"{page.Type,-22} {page.ResolveFolder(),-20} {string.Join(", ", page.Routes)}");
    return 0;
}

if (pages.Count == 0)
{
    Console.Error.WriteLine("Nothing selected.");
    return 1;
}

Console.WriteLine($"Config: {Path.GetFullPath(configPath)}");
Console.WriteLine($"Output: {config.OutputRoot}");

// Direct by default: the HTTP(S)_PROXY this machine exports answers 403 for everything outside its
// own allow-list, and the target is a public documentation site that needs no proxy at all.
using var handler = new HttpClientHandler { UseProxy = useProxy };
using var http = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(60) };
// The site answers a bare client with its shell; this is the same UA string a browser sends.
http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; FlareMd2SpecParser/1.0)");

SiteMetaIndex index;
try
{
    index = await SiteMetaIndex.LoadAsync(http, config.SiteMetaUrl);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Failed to read the route index: {ex.Message}");
    return 1;
}
Console.WriteLine($"Routes: {index.Count} resolved from {config.SiteMetaUrl}");
Console.WriteLine();

var results = await new SpecWriter(config, http, index).GenerateAsync(pages);

var failed = 0;
foreach (var r in results)
{
    if (r.Status == "failed") failed++;
    var marker = r.Status switch { "written" => "+", "skipped" => "-", _ => "!" };
    Console.WriteLine($"  {marker} {r.Type,-22} {r.Status,-8} {r.Detail}");
}

Console.WriteLine();
Console.WriteLine(failed == 0 ? "Done." : $"Done with {failed} failure(s).");
return failed == 0 ? 0 : 1;
