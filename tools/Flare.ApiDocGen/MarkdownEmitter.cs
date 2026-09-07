using System.Text;

namespace Flare.ApiDocGen;

/// <summary>Writes the extracted API model as a navigable, repository-friendly Markdown reference.</summary>
internal static class MarkdownEmitter
{
    public static void Write(string outputDirectory, IReadOnlyList<ComponentDoc> components, IReadOnlyList<EnumDoc> enums)
    {
        var root = Path.GetFullPath(outputDirectory);
        var componentDirectory = Path.Combine(root, "components");
        var enumDirectory = Path.Combine(root, "enums");
        Directory.CreateDirectory(componentDirectory);
        Directory.CreateDirectory(enumDirectory);

        var sortedComponents = components
            .OrderBy(component => component.Name, StringComparer.Ordinal)
            .ThenBy(component => component.FullName, StringComparer.Ordinal)
            .ToList();
        var sortedEnums = enums
            .OrderBy(enumDoc => enumDoc.Name, StringComparer.Ordinal)
            .ThenBy(enumDoc => enumDoc.FullName, StringComparer.Ordinal)
            .ToList();

        foreach (var component in sortedComponents)
            WriteIfChanged(Path.Combine(componentDirectory, FileName(component.FullName)), ComponentPage(component));
        foreach (var enumDoc in sortedEnums)
            WriteIfChanged(Path.Combine(enumDirectory, FileName(enumDoc.FullName)), EnumPage(enumDoc));

        WriteIfChanged(Path.Combine(root, "README.md"), IndexPage(sortedComponents, sortedEnums));
        Console.WriteLine($"[ApiDocGen] Wrote Markdown reference: {root} " +
            $"({sortedComponents.Count} components, {sortedEnums.Count} enums).");
    }

    private static string IndexPage(IReadOnlyList<ComponentDoc> components, IReadOnlyList<EnumDoc> enums)
    {
        var sb = Header();
        sb.AppendLine("# Flare API reference");
        sb.AppendLine();
        sb.AppendLine("Generated from the public Flare assemblies and XML documentation. Do not edit by hand.");
        sb.AppendLine();
        sb.AppendLine("## Components");
        sb.AppendLine();
        foreach (var component in components)
            sb.AppendLine($"- [`{component.Name}`](components/{FileName(component.FullName)}){SummarySuffix(component.Summary)}");

        sb.AppendLine();
        sb.AppendLine("## Enums");
        sb.AppendLine();
        foreach (var enumDoc in enums)
            sb.AppendLine($"- [`{enumDoc.FullName}`](enums/{FileName(enumDoc.FullName)}){SummarySuffix(enumDoc.Summary)}");

        return Normalize(sb);
    }

    private static string ComponentPage(ComponentDoc component)
    {
        var sb = Header();
        sb.AppendLine($"# {component.Name}");
        sb.AppendLine();
        sb.AppendLine($"`{component.FullName}`");
        AppendDescription(sb, component.Summary, component.Remarks);

        sb.AppendLine("## Parameters");
        sb.AppendLine();
        if (component.Parameters.Count == 0)
        {
            sb.AppendLine("This component has no public parameters.");
        }
        else
        {
            sb.AppendLine("| Name | Type | Default | Kind | Required | Description |");
            sb.AppendLine("| --- | --- | --- | --- | --- | --- |");
            foreach (var parameter in component.Parameters)
            {
                var kind = parameter.IsCascading ? "Cascading" : parameter.IsEventCallback ? "Callback" : "Parameter";
                var required = parameter.IsRequired ? "Yes" : "";
                var inherited = parameter.DeclaringType == component.Name ? "" : $" Inherited from `{parameter.DeclaringType}`.";
                sb.AppendLine($"| `{parameter.Name}` | `{parameter.Type}` | `{parameter.Default ?? string.Empty}` | {kind} | {required} | {Cell(parameter.Summary)}{inherited} |");
                if (!string.IsNullOrWhiteSpace(parameter.Remarks))
                    sb.AppendLine($"|  |  |  |  |  | {Cell(parameter.Remarks)} |");
            }
        }

        sb.AppendLine();
        sb.AppendLine("## Methods");
        sb.AppendLine();
        if (component.Methods.Count == 0)
        {
            sb.AppendLine("This component exposes no documented public methods.");
        }
        else
        {
            foreach (var method in component.Methods)
            {
                sb.AppendLine($"### `{method.Signature}`");
                sb.AppendLine();
                if (!string.IsNullOrWhiteSpace(method.Summary))
                    sb.AppendLine(Text(method.Summary));
                sb.AppendLine($"Returns: `{method.ReturnType}`{Suffix(method.ReturnSummary)}");
                if (method.Parameters.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("| Parameter | Type | Description |");
                    sb.AppendLine("| --- | --- | --- |");
                    foreach (var parameter in method.Parameters)
                        sb.AppendLine($"| `{parameter.Name}` | `{parameter.Type}` | {Cell(parameter.Summary)} |");
                }
                sb.AppendLine();
            }
        }

        AppendNameList(sb, "Inheritance", component.Inherits);
        AppendNameList(sb, "Derived components", component.DerivedBy);
        return Normalize(sb);
    }

    private static string EnumPage(EnumDoc enumDoc)
    {
        var sb = Header();
        sb.AppendLine($"# {enumDoc.Name}");
        sb.AppendLine();
        sb.AppendLine($"`{enumDoc.FullName}`");
        AppendDescription(sb, enumDoc.Summary, enumDoc.Remarks);

        sb.AppendLine("## Values");
        sb.AppendLine();
        sb.AppendLine("| Name | Value | Description |");
        sb.AppendLine("| --- | --- | --- |");
        foreach (var member in enumDoc.Members)
            sb.AppendLine($"| `{member.Name}` | `{member.Value}` | {Cell(member.Summary)} |");

        AppendNameList(sb, "Used by components", enumDoc.UsedBy);
        return Normalize(sb);
    }

    private static StringBuilder Header()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!-- <auto-generated/> Generated by tools/Flare.ApiDocGen. Do not edit by hand. -->");
        sb.AppendLine();
        return sb;
    }

    private static void AppendDescription(StringBuilder sb, string? summary, string? remarks)
    {
        if (!string.IsNullOrWhiteSpace(summary))
        {
            sb.AppendLine();
            sb.AppendLine(Text(summary));
        }
        if (!string.IsNullOrWhiteSpace(remarks))
        {
            sb.AppendLine();
            sb.AppendLine("## Remarks");
            sb.AppendLine();
            sb.AppendLine(Text(remarks));
        }
        sb.AppendLine();
    }

    private static void AppendNameList(StringBuilder sb, string title, IReadOnlyList<string> names)
    {
        if (names.Count == 0)
            return;

        sb.AppendLine();
        sb.AppendLine($"## {title}");
        sb.AppendLine();
        foreach (var name in names)
            sb.AppendLine($"- `{name}`");
    }

    private static string FileName(string fullName) =>
        fullName.Replace('.', '-').ToLowerInvariant() + ".md";

    private static void WriteIfChanged(string path, string content)
    {
        if (File.Exists(path) && File.ReadAllText(path) == content)
            return;
        File.WriteAllText(path, content);
    }

    private static string Normalize(StringBuilder builder) => builder.ToString().ReplaceLineEndings("\n");

    private static string Text(string? value) => string.IsNullOrWhiteSpace(value) ? "—" : value.Trim();

    private static string Cell(string? value) => Text(value)
        .Replace("|", "\\|")
        .ReplaceLineEndings(" ");

    private static string Suffix(string? value) => string.IsNullOrWhiteSpace(value) ? "." : $" — {Text(value)}";

    private static string SummarySuffix(string? value) => string.IsNullOrWhiteSpace(value) ? string.Empty : $" — {Cell(value)}";
}
