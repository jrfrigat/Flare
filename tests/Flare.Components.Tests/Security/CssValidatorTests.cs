using Flare.Components.Security;

namespace Flare.Components.Tests;

/// <summary>
/// The CSS sanitizer is a security boundary: a custom token or color reaches an inline style, and a
/// value that slips through can run script in old engines. These tests pin the contract the
/// hand-written scanner must keep after the regular expressions were removed from the startup path.
/// </summary>
public class CssValidatorTests
{
    [Theory]
    [InlineData("width:expression(alert(1))")]
    [InlineData("width:EXPRESSION (alert(1))")]
    [InlineData("width:expression \t (alert(1))")]
    public void StripDangerous_RemovesExpressionCalls(string css)
    {
        var result = CssValidator.StripDangerous(css);
        Assert.DoesNotContain("expression", result, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("background:url(javascript:alert(1))")]
    [InlineData("background:url( 'javascript:alert(1)' )")]
    [InlineData("background:URL(  \"JavaScript:alert(1)\")")]
    [InlineData("1rem;background:url(javascript:alert(1))")]
    public void StripDangerous_NeutralizesJavascriptUrls(string css)
    {
        var result = CssValidator.StripDangerous(css)!;
        var open = result.IndexOf("url(", StringComparison.OrdinalIgnoreCase);
        Assert.True(open >= 0);
        var close = result.IndexOf(')', open);
        Assert.True(close >= 0);
        Assert.Equal("url(about:blank)", result[open..(close + 1)]);
        // The scheme is consumed with the prefix; only the payload may stay after url(about:blank).
        Assert.DoesNotContain("javascript:", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StripDangerous_NeutralizesDataUrlsThatAreNotImages()
    {
        var result = CssValidator.StripDangerous("background:url(data:text/html;base64,PHNjcmlwdD4=)");
        Assert.Contains("url(about:blank)", result);
        Assert.DoesNotContain("data:", result, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("background:url(data:image/png;base64,AAAA)")]
    [InlineData("background:url(data:IMAGE/PNG;base64,AAAA)")]
    public void StripDangerous_KeepsImageDataUrls(string css)
    {
        Assert.Equal(css, CssValidator.StripDangerous(css));
    }

    [Theory]
    [InlineData("x:behavior:url(#default#time2)", "behavior")]
    [InlineData("x:BEHAVIOR : url(#x)", "behavior")]
    [InlineData("x:-moz-binding:url(#x)", "-moz-binding")]
    [InlineData("x:-MOZ-BINDING : url(#x)", "-moz-binding")]
    public void StripDangerous_RemovesLegacyBindingSyntax(string css, string keyword)
    {
        Assert.DoesNotContain(keyword, CssValidator.StripDangerous(css), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("#fff")]
    [InlineData("rgba(0, 0, 0, 0.5)")]
    [InlineData("var(" + Flare.Css.Tokens.Color.Primary + ")")]
    [InlineData("color-mix(in srgb, var(" + Flare.Css.Tokens.Color.Primary + ") 8%, transparent)")]
    public void SanitizeColor_AcceptsSafeValues(string color)
    {
        Assert.Equal(color, CssValidator.SanitizeColor(color));
    }

    [Theory]
    [InlineData("red;background:blue")]
    [InlineData("<script>")]
    [InlineData("url(javascript:alert(1))")]
    [InlineData("#fff\"")]
    public void SanitizeColor_RejectsUnsafeValues(string color)
    {
        Assert.Null(CssValidator.SanitizeColor(color));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SanitizeColor_RejectsEmptyValues(string? color)
    {
        Assert.Null(CssValidator.SanitizeColor(color));
    }

    [Theory]
    [InlineData("/x")]
    [InlineData("#x")]
    [InlineData("?q=1")]
    [InlineData("./x")]
    [InlineData("../x")]
    [InlineData("https://example.com")]
    [InlineData("mailto:a@b")]
    [InlineData(null)]
    public void IsHrefSafe_AllowsRelativeAndSafeSchemes(string? href)
    {
        Assert.True(CssValidator.IsHrefSafe(href));
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData(" java\nscript:alert(1)")]
    [InlineData("vbscript:msgbox(1)")]
    [InlineData("data:text/html,<script>")]
    public void IsHrefSafe_RejectsScriptSchemes(string href)
    {
        Assert.False(CssValidator.IsHrefSafe(href));
    }

    [Theory]
    [InlineData("data:image/png;base64,AAA", true)]
    [InlineData("data:text/html,<script>", false)]
    [InlineData("javascript:alert(1)", false)]
    [InlineData("https://example.com/x.png", true)]
    [InlineData("blob:https://example.com/1", true)]
    public void IsImageSrcSafe_UsesTheImageAllowList(string src, bool expected)
    {
        Assert.Equal(expected, CssValidator.IsImageSrcSafe(src));
    }
}
