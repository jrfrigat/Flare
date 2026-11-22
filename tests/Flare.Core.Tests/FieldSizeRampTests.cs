using System.Globalization;
using Flare.Abstractions.Tokens.Components;
using Flare.Theme.FluentUI2.Tokens;
using Flare.Theme.MaterialDesign3.Tokens;

namespace Flare.Core.Tests;

/// <summary>
/// The Xs..Xl field ramp is five independent padding tokens, one per size. The heights are the ramp
/// (guarded separately); block padding places the step's line box inside that definite height, so it
/// may shrink when a larger step brings a larger line - but it must never become negative, and the
/// shorthand must keep both halves readable.
/// </summary>
public class FieldSizeRampTests
{
    public static TheoryData<string, InputTokens> Themes => new()
    {
        { "MaterialDesign3", MaterialDesign3Tokens.Design.Input },
        { "FluentUI2", FluentUI2Tokens.Design.Input },
    };

    [Theory]
    [MemberData(nameof(Themes))]
    public void Field_block_padding_is_never_negative(string theme, InputTokens input)
    {
        var steps = new (string Name, string Value)[]
        {
            ("Xs", input.PaddingXs),
            ("Sm", input.PaddingSm),
            ("Md", input.PaddingMd),
            ("Lg", input.PaddingLg),
            ("Xl", input.PaddingXl),
        };

        foreach (var (name, value) in steps)
        {
            var rem = BlockRem(value);
            Assert.True(
                rem >= 0,
                $"{theme}: field size {name} has {rem}rem of block padding. Padding cannot be negative " +
                "and must leave the fixed height for the line box.");
        }
    }

    [Theory]
    [MemberData(nameof(Themes))]
    public void Field_padding_carries_both_axes(string theme, InputTokens input)
    {
        foreach (var value in new[] { input.PaddingXs, input.PaddingSm, input.PaddingMd, input.PaddingLg, input.PaddingXl })
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
