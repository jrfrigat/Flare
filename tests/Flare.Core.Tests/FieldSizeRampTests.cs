using System.Globalization;
using Flare.Abstractions.Tokens.Components;
using Flare.Theme.FluentUI2.Tokens;
using Flare.Theme.MaterialDesign3.Tokens;

namespace Flare.Core.Tests;

/// <summary>
/// The Xs..Xl field ramp is five independent padding tokens, one per size, and the block half of each
/// sets the field height. Nothing in the stylesheet relates them, so a theme can hand out a Large step
/// shorter than its Medium one - which is exactly what happened while four of the five lived as
/// literals in core CSS and only Medium came from the theme.
/// </summary>
public class FieldSizeRampTests
{
    public static TheoryData<string, InputTokens> Themes => new()
    {
        { "MaterialDesign3", MaterialDesignTokens.Design.Input },
        { "FluentUI2", FluentUI2Tokens.Design.Input },
    };

    [Theory]
    [MemberData(nameof(Themes))]
    public void Field_block_padding_grows_from_xs_to_xl(string theme, InputTokens input)
    {
        var steps = new (string Name, string Value)[]
        {
            ("Xs", input.PaddingXs),
            ("Sm", input.PaddingSm),
            ("Md", input.Padding),
            ("Lg", input.PaddingLg),
            ("Xl", input.PaddingXl),
        };

        var previous = (Name: string.Empty, Rem: double.NegativeInfinity);
        foreach (var (name, value) in steps)
        {
            var rem = BlockRem(value);
            Assert.True(
                rem > previous.Rem,
                $"{theme}: field size {name} has {rem}rem of block padding, which is not more than " +
                $"{previous.Name} at {previous.Rem}rem. The ramp must grow, or a larger size renders " +
                "a shorter field.");
            previous = (name, rem);
        }
    }

    [Theory]
    [MemberData(nameof(Themes))]
    public void Field_padding_carries_both_axes(string theme, InputTokens input)
    {
        foreach (var value in new[] { input.PaddingXs, input.PaddingSm, input.Padding, input.PaddingLg, input.PaddingXl })
        {
            Assert.True(
                value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length is 2,
                $"{theme}: '{value}' is not a two-value block/inline shorthand. The size grid reads the " +
                "block half as the field height and the inline half as the text inset; a one- or " +
                "four-value form silently changes one of them.");
        }
    }

    // The block half of a "<block> <inline>" shorthand, in rem. The themes author these as rem literals,
    // which is what makes the ramp comparable at all - a var() reference would have to be resolved
    // against a rendered document.
    private static double BlockRem(string padding)
    {
        var block = padding.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        Assert.EndsWith("rem", block, StringComparison.Ordinal);
        return double.Parse(block[..^3], CultureInfo.InvariantCulture);
    }
}
