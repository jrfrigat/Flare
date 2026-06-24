using System.Reflection;
using System.Text;

// Flare.DemoGen: reflect a Flare component's [Parameter]s and emit an INTERACTIVE Gallery
// playground (.razor): a live preview + one control per parameter (FlareSelect for enums,
// FlareSwitch for bool, FlareInput for strings, FlareColorPicker for FlareColor).
// Usage: dotnet run --project tools/Flare.DemoGen -- <Component> [galleryFolder]

if (args.Length == 0 || args[0] is "-h" or "--help")
{
    Console.WriteLine("Usage: dotnet run --project tools/Flare.DemoGen -- <Component> [galleryFolder]");
    return 0;
}

var compArg = args[0];
var root = FindRepoRoot();
if (root is null) { Console.Error.WriteLine("Could not find repo root (src/Flare.Components)."); return 2; }

var dlls = ComponentDlls(root);
if (dlls.Count == 0) { Console.Error.WriteLine("Flare.Components*.dll not found - build the solution first."); return 2; }

// Resolve sibling dependencies from any of the component bin folders.
var binDirs = dlls.Select(Path.GetDirectoryName).Distinct().ToList();
AppDomain.CurrentDomain.AssemblyResolve += (_, e) =>
{
    var name = new AssemblyName(e.Name).Name + ".dll";
    foreach (var b in binDirs)
    {
        var path = Path.Combine(b!, name);
        if (File.Exists(path)) return Assembly.LoadFrom(path);
    }
    return null;
};

var typeName = compArg.StartsWith("Flare", StringComparison.Ordinal) ? compArg : "Flare" + compArg;
var type = dlls.Select(Assembly.LoadFrom)
    .SelectMany(a => a.GetTypes())
    .FirstOrDefault(t => t.Name == typeName);
if (type is null) { Console.Error.WriteLine($"Component type '{typeName}' not found."); return 2; }
if (type.IsGenericTypeDefinition)
{
    Console.Error.WriteLine($"Skipped '{typeName}': generic component (needs a type argument) - generate manually.");
    return 3;
}

var folder = args.Length > 1 ? args[1] : compArg + "s";
var tag = type.Name;

var parameters = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
    .Where(p => p.GetCustomAttributes().Any(a => a.GetType().Name == "ParameterAttribute"))
    .Where(p => p.Name != "AdditionalAttributes")
    .ToList();

// ---- plan each parameter ---------------------------------------------------------------------
var controls = new List<string>();          // control markup lines (the FlareGrid children)
var previewAttrs = new List<string>();       // attributes on the preview <tag>
string? childContent = null;                 // inner content expression (from ChildContent param)
var stateLines = new List<string>();         // _field declarations
var optionLines = new List<string>();        // static option arrays
var derivedLines = new List<string>();       // derived properties (FlareColor, helpers)
var needSnackbar = false;
var needIconHelper = false;
var needNullHelper = false;

foreach (var p in parameters)
{
    var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
    var f = "_" + Camel(p.Name);

    // ChildContent -> the preview's inner text, edited via an input.
    if (p.Name == "ChildContent" && IsRenderFragment(t))
    {
        stateLines.Add($"private string {f} = \"{tag.Replace("Flare", "")}\";");
        controls.Add($"<FlareInput @bind-Value=\"{f}\" Label=\"ChildContent\" />");
        childContent = $"@NullIfEmpty({f})";
        needNullHelper = true;
        continue;
    }

    // Enum -> FlareSelect. Nested enums (e.g. FlareResizable.ResizableEdge) need qualifying.
    if (t.IsEnum)
    {
        var en = t.IsNested ? $"{t.DeclaringType!.Name}.{t.Name}" : t.Name;
        var first = Enum.GetNames(t).First();
        stateLines.Add($"private {en} {f} = {en}.{first};");
        optionLines.Add($"private static readonly {en}[] {f}Values = Enum.GetValues<{en}>();");
        controls.Add($"<FlareSelect TValue=\"{en}\" @bind-Value=\"{f}\" Items=\"{f}Values\" Label=\"{p.Name}\" />");
        previewAttrs.Add($"{p.Name}=\"{f}\"");
        continue;
    }

    // bool -> FlareSwitch.
    if (t == typeof(bool))
    {
        stateLines.Add($"private bool {f};");
        controls.Add($"<FlareSwitch @bind-Value=\"{f}\" Label=\"{p.Name}\" />");
        previewAttrs.Add($"{p.Name}=\"{f}\"");
        continue;
    }

    // FlareColor -> preset select + custom color picker.
    if (t.Name == "FlareColor")
    {
        var presets = ColorPresets(t).ToList();
        stateLines.Add($"private string {f}Preset = \"{presets.FirstOrDefault() ?? "Default"}\";");
        stateLines.Add($"private string {f}Custom = \"\";");
        optionLines.Add($"private static readonly string[] {f}Presets = {{ {string.Join(", ", presets.Select(x => $"\"{x}\""))} }};");
        controls.Add($"<FlareSelect TValue=\"string\" @bind-Value=\"{f}Preset\" Items=\"{f}Presets\" Label=\"{p.Name}\" />");
        controls.Add($"<FlareColorPicker @bind-Value=\"{f}Custom\" Label=\"{p.Name} (custom)\" />");
        derivedLines.Add($"private FlareColor {f} => !string.IsNullOrWhiteSpace({f}Custom)");
        derivedLines.Add($"    ? FlareColor.Custom({f}Custom)");
        derivedLines.Add($"    : {f}Preset switch");
        derivedLines.Add("    {");
        foreach (var c in presets.Where(c => c != "Default"))
            derivedLines.Add($"        \"{c}\" => FlareColor.{c},");
        derivedLines.Add("        _ => FlareColor.Default,");
        derivedLines.Add("    };");
        previewAttrs.Add($"{p.Name}=\"{f}\"");
        continue;
    }

    // RenderFragment slot (icons etc.) -> icon-name input.
    if (IsRenderFragment(t))
    {
        stateLines.Add($"private string {f} = \"\";");
        controls.Add($"<FlareInput @bind-Value=\"{f}\" Label=\"{p.Name} (icon name)\" />");
        previewAttrs.Add($"{p.Name}=\"@IconFragment({f})\"");
        needIconHelper = true;
        continue;
    }

    // EventCallback -> wire a snackbar (but skip "*Changed" - those are @bind pairs).
    if (IsEventCallback(t))
    {
        if (!p.Name.EndsWith("Changed", StringComparison.Ordinal))
        {
            previewAttrs.Add($"{p.Name}=\"@(() => Snackbar.Show(\"{p.Name}\"))\"");
            needSnackbar = true;
        }
        continue;
    }

    // string -> FlareInput.
    if (t == typeof(string))
    {
        stateLines.Add($"private string {f} = \"\";");
        controls.Add($"<FlareInput @bind-Value=\"{f}\" Label=\"{p.Name}\" />");
        previewAttrs.Add($"{p.Name}=\"@NullIfEmpty({f})\"");
        needNullHelper = true;
        continue;
    }

    // Anything else: leave a TODO control so the author can wire it.
    controls.Add($"@* TODO control for {p.Name} ({t.Name}) *@");
}

// ---- assemble razor --------------------------------------------------------------------------
var sb = new StringBuilder();
sb.AppendLine($"@* AUTO-GENERATED interactive playground for {tag} (Flare.DemoGen). Review and edit. *@");
if (needSnackbar) sb.AppendLine("@inject ISnackbarService Snackbar");
sb.AppendLine();
sb.AppendLine("<FlareStack Gap=\"FlareSpacing.Large\" Align=\"FlareAlignItems.Stretch\">");
sb.AppendLine();
sb.AppendLine("    <div style=\"display:flex;align-items:center;justify-content:center;min-height:6rem;padding:1.5rem;");
sb.AppendLine("                border:1px solid var(--flare-color-outline-variant);border-radius:var(--flare-shape-medium);");
sb.AppendLine("                background:var(--flare-color-surface-container-low);\">");
var attrsStr = previewAttrs.Count > 0 ? " " + string.Join(" ", previewAttrs) : "";
if (childContent is null)
{
    // No ChildContent parameter -> self-close (avoids "does not accept child content").
    sb.AppendLine($"        <{tag}{attrsStr} />");
}
else
{
    sb.AppendLine($"        <{tag}{attrsStr}>");
    sb.AppendLine($"            {childContent}");
    sb.AppendLine($"        </{tag}>");
}
sb.AppendLine("    </div>");
sb.AppendLine();
sb.AppendLine("    <FlareGrid Gap=\"FlareSpacing.Medium\" Columns=\"2\">");
foreach (var c in controls) sb.AppendLine($"        {c}");
sb.AppendLine("    </FlareGrid>");
sb.AppendLine();
sb.AppendLine("</FlareStack>");
sb.AppendLine();
sb.AppendLine("@code {");
foreach (var s in stateLines) sb.AppendLine($"    {s}");
if (optionLines.Count > 0) { sb.AppendLine(); foreach (var s in optionLines) sb.AppendLine($"    {s}"); }
if (derivedLines.Count > 0) { sb.AppendLine(); foreach (var s in derivedLines) sb.AppendLine($"    {s}"); }
if (needNullHelper) { sb.AppendLine(); sb.AppendLine("    private static string? NullIfEmpty(string? s) => string.IsNullOrWhiteSpace(s) ? null : s;"); }
if (needIconHelper) { sb.AppendLine(); sb.AppendLine("    private RenderFragment? IconFragment(string name)"); sb.AppendLine("        => string.IsNullOrWhiteSpace(name) ? null : @<FlareIcon Icon=\"@name\" />;"); }
sb.AppendLine("}");

var outDir = Path.Combine(root, "samples", "Flare.Gallery", "Pages", "Components", folder, "Examples");
Directory.CreateDirectory(outDir);
var outFile = Path.Combine(outDir, $"{folder}Example.razor");
File.WriteAllText(outFile, sb.ToString());
Console.WriteLine($"Generated playground ({controls.Count} controls) -> {outFile}");
return 0;

// ---- helpers ---------------------------------------------------------------------------------
static string Camel(string s) => s.Length == 0 ? s : char.ToLowerInvariant(s[0]) + s[1..];

static bool IsRenderFragment(Type t)
    => t.FullName == "Microsoft.AspNetCore.Components.RenderFragment";

static bool IsEventCallback(Type t)
    => t.FullName == "Microsoft.AspNetCore.Components.EventCallback"
       || (t.IsGenericType && t.GetGenericTypeDefinition().FullName == "Microsoft.AspNetCore.Components.EventCallback`1");

static IEnumerable<string> ColorPresets(Type flareColor)
    => flareColor.GetProperties(BindingFlags.Public | BindingFlags.Static)
        .Where(p => p.PropertyType == flareColor)
        .Select(p => p.Name);

static string? FindRepoRoot()
{
    foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
    {
        var d = new DirectoryInfo(start);
        while (d is not null)
        {
            if (Directory.Exists(Path.Combine(d.FullName, "src", "Flare.Components"))) return d.FullName;
            d = d.Parent;
        }
    }
    return null;
}

static List<string> ComponentDlls(string root)
{
    var names = new[] { "Flare.Components", "Flare.Components.Extended", "Flare.Components.Media" };
    var result = new List<string>();
    foreach (var n in names)
    {
        var binRoot = Path.Combine(root, "src", n, "bin");
        if (!Directory.Exists(binRoot)) continue;
        var dll = Directory.EnumerateFiles(binRoot, n + ".dll", SearchOption.AllDirectories)
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .FirstOrDefault();
        if (dll is not null) result.Add(dll);
    }
    return result;
}
