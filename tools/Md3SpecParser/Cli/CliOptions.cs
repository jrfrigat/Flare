using Flare.Tools.Md3SpecParser.Generation;

namespace Flare.Tools.Md3SpecParser.Cli;

/// <summary>Parsed command-line options.</summary>
public sealed class CliOptions
{
    /// <summary>Path to the JSON config file.</summary>
    public string ConfigPath { get; private set; } = "spec-config.json";

    /// <summary>Generation mode, or null to ask interactively.</summary>
    public GenerationMode? Mode { get; private set; }

    /// <summary>Explicitly requested types (case-insensitive), empty = ask/all.</summary>
    public IReadOnlyList<string> Types => _types;

    /// <summary>Select all eligible types without prompting.</summary>
    public bool All { get; private set; }

    /// <summary>Optional override for the config's output root.</summary>
    public string? OutputRoot { get; private set; }

    /// <summary>Run without any interactive prompts.</summary>
    public bool NonInteractive { get; private set; }

    /// <summary>Commit written files at the end without asking.</summary>
    public bool Commit { get; private set; }

    /// <summary>Show help and exit.</summary>
    public bool ShowHelp { get; private set; }

    private readonly List<string> _types = new();

    /// <summary>Parses <paramref name="args"/>; throws on malformed input.</summary>
    public static CliOptions Parse(string[] args)
    {
        var o = new CliOptions();

        for (var i = 0; i < args.Length; i++)
        {
            var a = args[i];
            switch (a)
            {
                case "-h" or "--help":
                    o.ShowHelp = true;
                    break;
                case "--config" or "-c":
                    o.ConfigPath = RequireValue(args, ref i, a);
                    break;
                case "--mode" or "-m":
                    o.Mode = ParseMode(RequireValue(args, ref i, a));
                    break;
                case "--full":
                    o.Mode = GenerationMode.Full;
                    break;
                case "--new":
                    o.Mode = GenerationMode.NewOnly;
                    break;
                case "--types" or "-t":
                    foreach (var t in RequireValue(args, ref i, a).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                        o._types.Add(t);
                    break;
                case "--all" or "-a":
                    o.All = true;
                    break;
                case "--output" or "-o":
                    o.OutputRoot = RequireValue(args, ref i, a);
                    break;
                case "--yes" or "-y":
                    o.NonInteractive = true;
                    break;
                case "--commit":
                    o.Commit = true;
                    break;
                default:
                    throw new ArgumentException($"Unknown argument: {a}");
            }
        }

        // Any explicit selection implies a non-interactive run unless prompts
        // are still needed (e.g. mode not given).
        if (o.All || o._types.Count > 0)
            o.NonInteractive = o.NonInteractive || o.Mode is not null;

        return o;
    }

    private static GenerationMode ParseMode(string value) => value.ToLowerInvariant() switch
    {
        "full" or "all" or "recreate" => GenerationMode.Full,
        "new" or "newonly" or "new-only" => GenerationMode.NewOnly,
        _ => throw new ArgumentException($"Invalid --mode '{value}'. Use 'full' or 'new'."),
    };

    private static string RequireValue(string[] args, ref int i, string flag)
    {
        if (i + 1 >= args.Length)
            throw new ArgumentException($"Missing value for {flag}.");
        return args[++i];
    }

    /// <summary>Usage text.</summary>
    public static string HelpText =>
        """
        md3spec - Material Design 3 Expressive spec generator

        Usage:
          md3spec [options]

        Options:
          -c, --config <path>    Config file (default: spec-config.json)
          -m, --mode <full|new>  full = recreate selected; new = only missing files
              --full / --new     Shortcuts for --mode
          -t, --types <a,b,c>    Comma-separated component types to process
          -a, --all              Process all eligible types
          -o, --output <dir>     Override output root from config
          -y, --yes              Non-interactive (no prompts)
              --commit           Git-commit written files (auto with -y; else asks)
          -h, --help             Show this help

        Examples:
          md3spec                         Interactive (choose mode + types)
          md3spec --new                   Generate only missing spec files (prompts types)
          md3spec --full --all -y         Recreate every type, no prompts
          md3spec --new --types Button,Menu -y
        """;
}
