using Flare.Theming;

namespace Flare.Core.Tests;

public class TokensToCssMinifyTests
{
    [Fact]
    public void Minify_RemovesCommentsAndStructuralWhitespace()
    {
        var css = "/* note */\n.flare-theme-x {\n  " + Flare.Css.Tokens.Color.Primary + ": red;\n" +
                  "  --g: color-mix(in srgb, red 8%, transparent);\n}";

        Assert.Equal(
            ".flare-theme-x{" + Flare.Css.Tokens.Color.Primary +
            ":red;--g:color-mix(in srgb, red 8%, transparent)}",
            TokensToCss.Minify(css));
    }

    [Fact]
    public void Minify_KeepsTheDescendantSpaceBeforeAPseudoClass()
    {
        Assert.Equal(".a :hover{color:red}", TokensToCss.Minify(".a :hover { color: red; }"));
        Assert.Equal(".x :where(.y){color:red}", TokensToCss.Minify(".x :where(.y) {\n  color: red;\n}"));
    }

    [Fact]
    public void Minify_KeepsQuotedStrings()
    {
        var min = TokensToCss.Minify(".x { content: \"a  /*b*/ c\"; font-family: 'My  Font'; }");

        Assert.Contains("content:\"a  /*b*/ c\"", min);
        Assert.Contains("font-family:'My  Font'", min);
    }
}
