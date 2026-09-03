using System.Globalization;
using System.Text.RegularExpressions;
using Flare.Abstractions.Tokens.Components;
using Flare.Theme.FluentUI2.Tokens;
using Flare.Theme.MaterialDesign3.Tokens;

namespace Flare.Core.Tests;

/// <summary>
/// The field family's height is one token per size, applied to the shared well, and this fixes the class
/// of defect that let the same size render at different heights on different controls. The well used to
/// be measured from its content, so whatever sat inside it won: a combobox trigger stood 2px OVER the
/// text field under Material and 4px UNDER it under Fluent, the Xl step inverted that sign, a Select with
/// a leading icon was 4px taller than one without, and the tag well named a 2.75rem literal of its own
/// that landed 10px short. Heights are equal by construction now - a definite height is not measured from
/// content - so what is left to guard is the ramp the themes supply and the ways core CSS could start
/// measuring content again.
/// </summary>
public class FieldHeightRampTests
{
    public static TheoryData<string, InputTokens> Themes => new()
    {
        { "MaterialDesign3", MaterialDesignTokens.Design.Input },
        { "FluentUI2", FluentUI2Tokens.Design.Input },
    };

    private static (string Name, string Value)[] Steps(InputTokens i) =>
    [
        ("Xs", i.HeightXs), ("Sm", i.HeightSm), ("Md", i.HeightMd), ("Lg", i.HeightLg), ("Xl", i.HeightXl),
    ];

    [Theory]
    [MemberData(nameof(Themes))]
    public void Height_ramp_grows_from_xs_to_xl(string theme, InputTokens input)
    {
        var previous = (Name: string.Empty, Rem: double.NegativeInfinity);
        foreach (var (name, value) in Steps(input))
        {
            var rem = Rem(value);
            Assert.True(
                rem > previous.Rem,
                $"{theme}: field height {name} is {rem}rem, which is not more than {previous.Name} at " +
                $"{previous.Rem}rem. Nothing in the stylesheet relates the five steps, so an out-of-order " +
                "ramp renders a larger size as a shorter field.");
            previous = (name, rem);
        }
    }

    // A definite height does not grow to fit, so a step shorter than the padding it has to hold makes the
    // control's own box overflow the well it is centred in. This is the one arithmetic relationship
    // between the two ramps, and it is the theme's to keep: core cannot correct it.
    [Theory]
    [MemberData(nameof(Themes))]
    public void Height_leaves_room_for_the_padding_at_that_step(string theme, InputTokens input)
    {
        var padding = new[] { input.PaddingXs, input.PaddingSm, input.PaddingMd, input.PaddingLg, input.PaddingXl };
        var steps = Steps(input);

        for (var i = 0; i < steps.Length; i++)
        {
            var block = BlockRem(padding[i]) * 2;
            var height = Rem(steps[i].Value);
            Assert.True(
                height > block,
                $"{theme}: field height {steps[i].Name} is {height}rem but its padding alone is {block}rem. " +
                "The well is exactly this tall, so there is no room left for a line of text.");
        }
    }

    // Every step is a rem literal, which is what makes the ramp comparable here at all - a var() would
    // have to be resolved against a rendered document, and the ordering guard above would go quiet.
    [Theory]
    [MemberData(nameof(Themes))]
    public void Height_steps_are_absolute_lengths(string theme, InputTokens input)
    {
        foreach (var (name, value) in Steps(input))
        {
            Assert.True(
                value.EndsWith("rem", StringComparison.Ordinal) && double.TryParse(
                    value[..^3], NumberStyles.Float, CultureInfo.InvariantCulture, out _),
                $"{theme}: field height {name} is '{value}'. The ramp has to be comparable step to step, " +
                "so each one is a rem literal.");
        }
    }

    // The whole family resolves its height from one place. A second height on a well - in any of the
    // component stylesheets, or a literal beside the token - is how the family drifted apart before: the
    // tag well carried `min-height: 2.75rem` and sat 10px under every field beside it, inverted between
    // its own Sm and Md, and nothing failed.
    [Fact]
    public void Only_the_shared_well_rule_sizes_a_field_well()
    {
        var cssDir = Path.Combine(FindRepoRoot(), "src", "Flare.Components", "wwwroot", "css");
        var offenders = new List<string>();

        foreach (var file in Directory.EnumerateFiles(cssDir, "*.css"))
        {
            var css = StripComments(File.ReadAllText(file));
            foreach (Match rule in Regex.Matches(css, @"(?<selectors>[^{}]+)\{(?<body>[^{}]*)\}"))
            {
                var selectors = rule.Groups["selectors"].Value;
                // The subject of a selector is its LAST compound: a rule reading
                // ".flare-input__field .something { height }" sizes the something, not the well.
                if (!selectors.Split(',').Any(s => Subject(s).Contains("__field", StringComparison.Ordinal)))
                    continue;

                // The lookbehind is what keeps "line-height" and the private "--_flare-input-height" out:
                // both end in the property name being searched for.
                foreach (Match decl in Regex.Matches(
                    rule.Groups["body"].Value, @"(?<![-\w])(?<prop>(?:min-|max-)?(?:block-size|height))\s*:\s*(?<value>[^;]+)"))
                {
                    var value = decl.Groups["value"].Value.Trim();
                    // The one legitimate shape: the private var the shared rule resolves from the ramp,
                    // and the `auto` the grow wells use to hand their height back to their content.
                    if (value.Contains("--_flare-input-height", StringComparison.Ordinal) || value == "auto") continue;
                    offenders.Add($"{Path.GetFileName(file)}: '{selectors.Trim()}' sets {decl.Groups["prop"].Value}: {value}");
                }
            }
        }

        Assert.True(
            offenders.Count == 0,
            "A field well is sized outside the shared height ramp:\n  " + string.Join("\n  ", offenders) +
            "\nHeight comes from --flare-input-height-* through --_flare-input-height (input.css). A well " +
            "that names its own is the defect this ramp exists to remove.");
    }

    // The last compound of a selector - what the rule actually styles.
    private static string Subject(string selector)
    {
        var trimmed = selector.Trim();
        var cut = trimmed.LastIndexOfAny([' ', '>', '+', '~']);
        return cut < 0 ? trimmed : trimmed[(cut + 1)..];
    }

    private static double Rem(string value)
    {
        Assert.EndsWith("rem", value, StringComparison.Ordinal);
        return double.Parse(value[..^3], CultureInfo.InvariantCulture);
    }

    private static double BlockRem(string padding)
    {
        var block = padding.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        Assert.EndsWith("rem", block, StringComparison.Ordinal);
        return double.Parse(block[..^3], CultureInfo.InvariantCulture);
    }

    private static string StripComments(string css) => Regex.Replace(css, @"/\*.*?\*/", string.Empty, RegexOptions.Singleline);

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Flare.Components")))
                return dir.FullName;
            dir = dir.Parent;
        }

        throw new InvalidOperationException("Could not locate the repository root from " + AppContext.BaseDirectory);
    }
}
