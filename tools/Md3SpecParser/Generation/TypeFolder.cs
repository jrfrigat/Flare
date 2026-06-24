using System.Text;

namespace Flare.Tools.Md3SpecParser.Generation;

/// <summary>
/// Maps a component type name to its output sub-folder name.
/// </summary>
public static class TypeFolder
{
    /// <summary>
    /// Converts a display type ("Button Group", "ButtonGroup", "button group")
    /// into a kebab-case folder name ("button-group").
    /// </summary>
    public static string ToKebab(string type)
    {
        var sb = new StringBuilder(type.Length + 4);
        char? prev = null;

        foreach (var ch in type.Trim())
        {
            if (ch is ' ' or '_' or '-')
            {
                if (sb.Length > 0 && sb[^1] != '-')
                    sb.Append('-');
                prev = null;
                continue;
            }

            // Insert a separator at lower->Upper boundaries (camel/Pascal case).
            if (char.IsUpper(ch) && prev is char p && (char.IsLower(p) || char.IsDigit(p))
                && sb.Length > 0 && sb[^1] != '-')
            {
                sb.Append('-');
            }

            sb.Append(char.ToLowerInvariant(ch));
            prev = ch;
        }

        return sb.ToString().Trim('-');
    }
}
