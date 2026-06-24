using Flare.Tools.Md3SpecParser.Parsing.Model;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Flare.Tools.Md3SpecParser.Parsing;

/// <summary>The theme a value is resolved for.</summary>
public enum Theme
{
    /// <summary>Light theme.</summary>
    Light,

    /// <summary>Dark theme.</summary>
    Dark,
}

/// <summary>
/// Indexes a <see cref="SystemModel"/> and resolves each token to a display value
/// for a chosen context, following context defaults and alias references the way the
/// Material token table is rendered.
/// </summary>
/// <remarks>
/// The selected context pins one tag per context group: Theme = Light/Dark,
/// Audience = 3P (the Material baseline palette, e.g. primary = #6750A4 rather than
/// the first-party #0B57D0), and every other group to its declared default. A value
/// is eligible when all of its context tags are pinned; the highest-specificity
/// eligible value wins, falling back to the most generic value when none match.
/// </remarks>
public sealed partial class ValueResolver
{
    private const string AudienceGroup = "Audience";
    private const string AudienceSelection = "3P";
    private const string ThemeGroup = "Theme";

    private readonly Dictionary<string, TokenModel> _tokensById = new();
    private readonly Dictionary<string, TokenModel> _tokensByName = new();
    private readonly Dictionary<string, List<ValueModel>> _valuesByTokenId = new();
    private readonly Dictionary<string, DisplayGroupModel> _displayGroupsByName = new();

    // groupDisplayName -> (tagDisplayName -> tag resource name)
    private readonly Dictionary<string, Dictionary<string, string>> _tagsByGroup = new();
    private readonly Dictionary<string, string> _groupDefaultTag = new(); // groupDisplay -> default tag resource

    private readonly HashSet<string> _selectedLight;
    private readonly HashSet<string> _selectedDark;

    /// <summary>Builds all lookup tables and the Light/Dark context selections.</summary>
    public ValueResolver(SystemModel system)
    {
        foreach (var t in system.Tokens)
        {
            _tokensById[TokenId(t.Name)] = t;
            _tokensByName[t.TokenName] = t;
        }

        foreach (var v in system.Values)
        {
            var id = TokenIdFromValue(v.Name);
            if (id.Length == 0) continue;
            if (!_valuesByTokenId.TryGetValue(id, out var list))
                _valuesByTokenId[id] = list = new List<ValueModel>();
            list.Add(v);
        }

        foreach (var g in system.DisplayGroups)
            _displayGroupsByName[g.Name] = g;

        var groupDisplayByResource = system.ContextTagGroups
            .ToDictionary(g => g.Name, g => g.DisplayName, StringComparer.Ordinal);

        foreach (var tag in system.Tags)
        {
            var groupResource = GroupOfTag(tag.Name);
            if (groupResource is null || !groupDisplayByResource.TryGetValue(groupResource, out var groupDisplay))
                continue;
            if (!_tagsByGroup.TryGetValue(groupDisplay, out var map))
                _tagsByGroup[groupDisplay] = map = new Dictionary<string, string>(StringComparer.Ordinal);
            map[tag.DisplayName] = tag.Name;
        }

        foreach (var g in system.ContextTagGroups)
            if (g.DefaultTag is { Length: > 0 } d)
                _groupDefaultTag[g.DisplayName] = d;

        _selectedLight = BuildSelection(Theme.Light);
        _selectedDark = BuildSelection(Theme.Dark);
    }

    /// <summary>
    /// Returns the tokens belonging to <paramref name="tokenSetName"/> in render
    /// order: ungrouped tokens first, then display groups by
    /// <see cref="DisplayGroupModel.OrderInParentTokenSet"/>, each by
    /// <see cref="TokenModel.OrderInDisplayGroup"/>.
    /// </summary>
    public IReadOnlyList<TokenModel> OrderedTokens(string tokenSetName)
    {
        var setId = SetId(tokenSetName);
        var marker = "/tokenSets/" + setId + "/tokens/";

        return _tokensById.Values
            .Where(t => t.Name.Contains(marker, StringComparison.Ordinal))
            .OrderBy(GroupOrder)
            .ThenBy(t => t.OrderInDisplayGroup)
            .ToList();
    }

    /// <summary>The display-group name for a token, or null if ungrouped.</summary>
    public string? GroupNameOf(TokenModel token)
        => token.DisplayGroup is { Length: > 0 } dg && _displayGroupsByName.TryGetValue(dg, out var g)
            ? g.DisplayName
            : null;

    /// <summary>Resolves a token to its formatted display value for <paramref name="theme"/>.</summary>
    public string Resolve(TokenModel token, Theme theme)
        => FormatValue(PickValue(ValuesFor(token), theme), theme, new HashSet<string>(StringComparer.Ordinal));

    /// <summary>The reference tone backing a color and its final hex.</summary>
    public readonly record struct ColorChain(string? Tone, string Hex);

    private const string SysColorPrefix = "md.sys.color.";
    private const string RefPalettePrefix = "md.ref.palette.";

    /// <summary>
    /// The semantic color role (<c>md.sys.color.*</c>) a token resolves through, or null
    /// when it has no color or maps to a concrete color directly.
    /// </summary>
    public string? ColorRoleOf(TokenModel token, Theme theme)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var value = PickValue(ValuesFor(token), theme);
        for (var i = 0; i < 16 && value is not null; i++)
        {
            if (value.TokenName is not { Length: > 0 } alias) return null;
            if (alias.StartsWith(SysColorPrefix, StringComparison.Ordinal)) return alias;
            if (!visited.Add(alias) || !_tokensByName.TryGetValue(alias, out var next)) return null;
            value = PickValue(ValuesFor(next), theme);
        }
        return null;
    }

    /// <summary>
    /// Follows the alias chain from <paramref name="tokenName"/> to a concrete color,
    /// recording the last <c>md.ref.palette.*</c> tone seen along the way.
    /// </summary>
    public ColorChain ResolveColorChain(string tokenName, Theme theme)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);
        string? tone = null;
        var current = tokenName;

        for (var i = 0; i < 16; i++)
        {
            if (!_tokensByName.TryGetValue(current, out var token)) break;
            var value = PickValue(ValuesFor(token), theme);
            if (value is null) break;
            if (value.Color is not null) return new ColorChain(tone, FormatColor(value.Color));
            if (value.TokenName is not { Length: > 0 } alias) break;
            if (!visited.Add(alias)) break;
            if (alias.StartsWith(RefPalettePrefix, StringComparison.Ordinal))
                tone = alias[RefPalettePrefix.Length..];
            current = alias;
        }

        return new ColorChain(tone, string.Empty);
    }

    private HashSet<string> Selection(Theme theme) => theme == Theme.Dark ? _selectedDark : _selectedLight;

    private HashSet<string> BuildSelection(Theme theme)
    {
        var set = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (groupDisplay, defaultTag) in _groupDefaultTag)
        {
            string? chosen = groupDisplay switch
            {
                AudienceGroup => Tag(AudienceGroup, AudienceSelection) ?? defaultTag,
                ThemeGroup => Tag(ThemeGroup, theme == Theme.Dark ? "Dark" : "Light") ?? defaultTag,
                _ => defaultTag,
            };
            if (chosen is not null) set.Add(chosen);
        }
        return set;
    }

    private string? Tag(string group, string tagDisplay)
        => _tagsByGroup.TryGetValue(group, out var m) && m.TryGetValue(tagDisplay, out var t) ? t : null;

    private int GroupOrder(TokenModel token)
        => token.DisplayGroup is { Length: > 0 } dg && _displayGroupsByName.TryGetValue(dg, out var group)
            ? group.OrderInParentTokenSet
            : -1; // ungrouped tokens render first

    private List<ValueModel> ValuesFor(TokenModel token)
        => _valuesByTokenId.TryGetValue(TokenId(token.Name), out var v) ? v : new List<ValueModel>();

    /// <summary>
    /// Chooses the value for the selected context: skips undefined values, prefers the
    /// highest-specificity value whose context tags are all selected, and otherwise
    /// falls back to the most generic (fewest tags, then lowest specificity) value.
    /// </summary>
    private ValueModel? PickValue(List<ValueModel> values, Theme theme)
    {
        var defined = values.Where(v => !v.Undefined).ToList();
        if (defined.Count == 0) return null;

        var selected = Selection(theme);
        var eligible = defined
            .Where(v => v.ContextTags is null || v.ContextTags.All(selected.Contains))
            .ToList();

        if (eligible.Count > 0)
            return eligible.OrderByDescending(Specificity).First();

        return defined
            .OrderBy(v => v.ContextTags?.Count ?? 0)
            .ThenBy(Specificity)
            .First();
    }

    private string FormatValue(ValueModel? value, Theme theme, HashSet<string> visited)
    {
        if (value is null) return string.Empty;
        if (value.Color is not null) return FormatColor(value.Color);
        if (value.Length is not null) return FormatDimension(value.Length);
        if (value.Elevation is not null) return FormatDimension(value.Elevation);
        if (value.FontSize is not null) return FormatDimension(value.FontSize);
        if (value.LineHeight is not null) return FormatDimension(value.LineHeight);
        if (value.FontTracking is not null) return FormatDimension(value.FontTracking);
        if (value.Opacity is double o) return FormatNumber(o);
        if (value.Numeric is double n) return FormatNumber(n);
        if (value.FontWeight is double w) return FormatNumber(w);
        if (value.AxisValue is { Value: double a }) return FormatNumber(a);
        if (value.FontNames is { Values.Count: > 0 } fn) return fn.Values[0];
        if (value.Shape is not null) return FormatShape(value.Shape);
        if (value.Type is not null) return FormatTypography(value.Type, theme, visited);

        if (value.TokenName is { Length: > 0 } alias)
        {
            if (!visited.Add(alias)) return alias; // cycle guard
            if (_tokensByName.TryGetValue(alias, out var target))
                return FormatValue(PickValue(ValuesFor(target), theme), theme, visited);
            return alias; // referenced token absent from this document
        }

        return string.Empty;
    }

    private string ResolveAlias(string? tokenName, Theme theme, HashSet<string> visited)
    {
        if (string.IsNullOrEmpty(tokenName) || !_tokensByName.TryGetValue(tokenName, out var t))
            return string.Empty;
        return FormatValue(PickValue(ValuesFor(t), theme), theme, visited);
    }

    private string FormatTypography(TypographyModel t, Theme theme, HashSet<string> visited)
    {
        var font = ResolveAlias(t.FontNameTokenName, theme, new HashSet<string>(visited, StringComparer.Ordinal));
        var weight = ResolveAlias(t.FontWeightTokenName, theme, new HashSet<string>(visited, StringComparer.Ordinal));
        var size = ResolveAlias(t.FontSizeTokenName, theme, new HashSet<string>(visited, StringComparer.Ordinal));
        var tracking = ResolveAlias(t.FontTrackingTokenName, theme, new HashSet<string>(visited, StringComparer.Ordinal));
        var lineHeight = ResolveAlias(t.LineHeightTokenName, theme, new HashSet<string>(visited, StringComparer.Ordinal));
        return $"{font}, weight {weight}, size {size}, tracking {tracking}, line-height {lineHeight}";
    }

    private static string FormatColor(ColorModel c)
    {
        static int B(double x) => (int)Math.Round(Math.Clamp(x, 0, 1) * 255, MidpointRounding.AwayFromZero);
        return $"#{B(c.Red):X2}{B(c.Green):X2}{B(c.Blue):X2}";
    }

    private static string FormatDimension(LengthModel l)
    {
        var value = FormatNumber(l.Value ?? 0);
        return l.Unit switch
        {
            "DIPS" => value + "dp",
            "PERCENT" => value + "%",
            "POINTS" => value + "pt",
            _ => value + (l.Unit ?? string.Empty),
        };
    }

    private static string FormatShape(ShapeModel s)
    {
        if (s.Family == "SHAPE_FAMILY_CIRCULAR") return "Circular";
        if (s.DefaultSize is not null) return FormatDimension(s.DefaultSize);
        return s.Family ?? string.Empty;
    }

    private static string FormatNumber(double value) =>
        value == Math.Truncate(value)
            ? ((long)value).ToString(CultureInfo.InvariantCulture)
            : value.ToString(CultureInfo.InvariantCulture);

    private static int Specificity(ValueModel v) =>
        int.TryParse(v.SpecificityScore, NumberStyles.Integer, CultureInfo.InvariantCulture, out var s) ? s : 0;

    private static string? GroupOfTag(string tagResourceName)
    {
        var m = GroupRegex().Match(tagResourceName);
        return m.Success ? m.Value : null;
    }

    private static string TokenId(string tokenResourceName)
    {
        var i = tokenResourceName.IndexOf("/tokens/", StringComparison.Ordinal);
        return i < 0 ? tokenResourceName : tokenResourceName[(i + "/tokens/".Length)..];
    }

    private static string TokenIdFromValue(string valueResourceName)
    {
        var i = valueResourceName.IndexOf("/tokens/", StringComparison.Ordinal);
        if (i < 0) return string.Empty;
        var rest = valueResourceName[(i + "/tokens/".Length)..];
        var j = rest.IndexOf("/values/", StringComparison.Ordinal);
        return j < 0 ? rest : rest[..j];
    }

    private static string SetId(string tokenSetResourceName)
    {
        var i = tokenSetResourceName.IndexOf("/tokenSets/", StringComparison.Ordinal);
        return i < 0 ? tokenSetResourceName : tokenSetResourceName[(i + "/tokenSets/".Length)..];
    }

    [GeneratedRegex(@"^.*/contextTagGroups/[^/]+")]
    private static partial Regex GroupRegex();
}
