using Flare.ApiDocGen;
using System.Reflection;

// Usage: Flare.ApiDocGen <outputFile.g.cs> [probeDirectory]
// Emits a generated C# registry of Flare component API docs for Flare.Gallery.

var outputPath = args.Length > 0
    ? args[0]
    : Path.Combine(AppContext.BaseDirectory, "ComponentApiRegistry.g.cs");

var probeDir = args.Length > 1 ? args[1] : AppContext.BaseDirectory;

Console.WriteLine($"[ApiDocGen] Probing assemblies in: {probeDir}");

var assemblies = LoadFlareAssemblies(probeDir);
Console.WriteLine($"[ApiDocGen] Loaded {assemblies.Count} Flare assemblies.");

var docs = new XmlDocReader();
docs.Load(Directory.EnumerateFiles(probeDir, "Flare.*.xml"));

var extractor = new ComponentExtractor(docs);
var components = new List<ComponentDoc>();
var enums = new List<EnumDoc>();

Type? baseComponentType = null;

foreach (var assembly in assemblies)
{
    foreach (var type in SafeGetTypes(assembly))
    {
        if (ComponentExtractor.IsComponent(type))
        {
            components.Add(extractor.Extract(type));
            baseComponentType ??= FindBaseComponent(type);
        }
        else if (ComponentExtractor.IsDocumentableEnum(type))
        {
            enums.Add(extractor.ExtractEnum(type));
        }
    }
}

// Document the shared base class itself so inheritance links resolve to a real page.
if (baseComponentType is not null)
    components.Add(extractor.Extract(baseComponentType));

// De-duplicate (umbrella + component assemblies can surface the same type) by full name.
components = components
    .GroupBy(c => c.FullName)
    .Select(g => g.First())
    .ToList();

// Build inheritance children: which components derive from each documented type.
var componentNames = new HashSet<string>(components.Select(c => c.Name), StringComparer.Ordinal);
var derived = new Dictionary<string, SortedSet<string>>(StringComparer.Ordinal);
foreach (var component in components)
{
    foreach (var baseName in component.Inherits)
    {
        if (!componentNames.Contains(baseName))
            continue;
        if (!derived.TryGetValue(baseName, out var set))
            derived[baseName] = set = new SortedSet<string>(StringComparer.Ordinal);
        set.Add(component.Name);
    }
}

components = components
    .Select(c => c with
    {
        DerivedBy = derived.TryGetValue(c.Name, out var kids)
            ? kids.ToList()
            : System.Array.Empty<string>()
    })
    .ToList();

enums = enums
    .GroupBy(e => e.FullName)
    .Select(g => g.First())
    .ToList();

// Build "used by" cross-references: which components reference each type name.
var usage = new Dictionary<string, SortedSet<string>>(StringComparer.Ordinal);
foreach (var component in components)
{
    foreach (var token in ReferencedTypeNames(component))
    {
        if (!usage.TryGetValue(token, out var set))
            usage[token] = set = new SortedSet<string>(StringComparer.Ordinal);
        set.Add(component.Name);
    }
}

enums = enums
    .Select(e => e with
    {
        UsedBy = usage.TryGetValue(e.Name, out var users)
            ? users.ToList()
            : System.Array.Empty<string>()
    })
    .ToList();

Console.WriteLine($"[ApiDocGen] Documented {components.Count} components, " +
    $"{components.Sum(c => c.Parameters.Count)} parameters, " +
    $"{components.Sum(c => c.Methods.Count)} methods, " +
    $"{enums.Count} enums.");

var source = RegistryEmitter.Emit(components, enums);

var outDir = Path.GetDirectoryName(Path.GetFullPath(outputPath));
if (!string.IsNullOrEmpty(outDir))
    Directory.CreateDirectory(outDir);

// Avoid rewriting (and retriggering builds) when nothing changed.
if (File.Exists(outputPath) && File.ReadAllText(outputPath) == source)
{
    Console.WriteLine($"[ApiDocGen] Up to date: {outputPath}");
    return 0;
}

File.WriteAllText(outputPath, source);
Console.WriteLine($"[ApiDocGen] Wrote: {outputPath}");
return 0;

static List<Assembly> LoadFlareAssemblies(string dir)
{
    var result = new List<Assembly>();
    foreach (var path in Directory.EnumerateFiles(dir, "Flare.*.dll"))
    {
        // Skip the docs tool's own helper assemblies, if any share the prefix.
        var name = Path.GetFileNameWithoutExtension(path);
        if (name.StartsWith("Flare.ApiDocGen", StringComparison.Ordinal))
            continue;

        try { result.Add(Assembly.LoadFrom(path)); }
        catch (Exception ex) { Console.WriteLine($"[ApiDocGen] Skipped {name}: {ex.Message}"); }
    }
    return result;
}

// Walks up to the FlareComponentBase type in a component's inheritance chain.
static Type? FindBaseComponent(Type type)
{
    for (var b = type.BaseType; b is not null; b = b.BaseType)
        if (b.FullName == "Flare.Core.Components.FlareComponentBase")
            return b;
    return null;
}

static IEnumerable<Type> SafeGetTypes(Assembly assembly)
{
    try { return assembly.GetTypes(); }
    catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t is not null)!; }
    catch { return Array.Empty<Type>(); }
}

// All identifier tokens used in a component's parameter/method type strings.
static IEnumerable<string> ReferencedTypeNames(ComponentDoc component)
{
    var seen = new HashSet<string>(StringComparer.Ordinal);

    foreach (var p in component.Parameters)
        foreach (var token in IdentifierTokens(p.Type))
            if (seen.Add(token)) yield return token;

    foreach (var m in component.Methods)
    {
        foreach (var token in IdentifierTokens(m.ReturnType))
            if (seen.Add(token)) yield return token;
        foreach (var mp in m.Parameters)
            foreach (var token in IdentifierTokens(mp.Type))
                if (seen.Add(token)) yield return token;
    }
}

static IEnumerable<string> IdentifierTokens(string text)
{
    var start = -1;
    for (var i = 0; i < text.Length; i++)
    {
        var isIdChar = char.IsLetterOrDigit(text[i]) || text[i] == '_';
        if (isIdChar && start < 0)
            start = i;
        else if (!isIdChar && start >= 0)
        {
            yield return text[start..i];
            start = -1;
        }
    }
    if (start >= 0)
        yield return text[start..];
}
