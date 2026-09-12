using System.Text.RegularExpressions;

namespace Flare.Tools.Md2SpecParser.Parsing;

/// <summary>
/// The <c>$name: value !default;</c> declarations out of a Material Components stylesheet.
/// <para>
/// Material 2 published redline drawings for some components and nothing measurable for others -
/// there is no tab height, list-row height or app-bar height anywhere on the guidelines site. Google's
/// own implementation of the same specification does carry them, as Sass variables, so it serves as
/// the second source for exactly the values the first one omits. Each is labelled with where it came
/// from, because an implementation is evidence of the spec, not the spec itself.
/// </para>
/// </summary>
public static partial class ScssVariables
{
    /// <summary>One declared variable.</summary>
    /// <param name="Name">Its name without the leading <c>$</c>.</param>
    /// <param name="Value">The literal as written.</param>
    /// <param name="Comment">The trailing <c>//</c> comment, when it has one.</param>
    public sealed record Variable(string Name, string Value, string? Comment);

    /// <summary>Reads every top-level variable declaration, in file order.</summary>
    public static IReadOnlyList<Variable> Parse(string scss)
    {
        var found = new List<Variable>();
        foreach (Match m in Declaration().Matches(scss))
        {
            var value = m.Groups["value"].Value.Trim();
            // A map spanning several lines is structure, not a measurement; keep it out of the table.
            if (value.StartsWith('(') && !value.EndsWith(')')) continue;
            found.Add(new Variable(
                m.Groups["name"].Value,
                Regex.Replace(value, @"\s+", " "),
                m.Groups["comment"].Success ? m.Groups["comment"].Value.Trim() : null));
        }
        return found;
    }

    // Anchored at the line start so a $variable READ inside a rule is not mistaken for a declaration.
    [GeneratedRegex(@"^\$(?<name>[\w-]+)\s*:\s*(?<value>[^;]+?)\s*(?:!default)?\s*;(?:\s*//\s*(?<comment>[^\r\n]*))?",
        RegexOptions.Multiline)]
    private static partial Regex Declaration();
}
