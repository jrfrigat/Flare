using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareHrefSafetyTests : FlareTestContext
{
    private string RenderedHref(string href) =>
        Render<FlareNavLink>(p => p.Add(x => x.Href, href).AddChildContent("link"))
            .Find("a").GetAttribute("href") ?? string.Empty;

    // A Blazor app hosted under a sub-path (GitHub Pages serves the gallery from /Flare/) has to
    // write its internal links base-relative; "/x" would resolve against the origin instead.
    [Theory]
    [InlineData("components/buttons")]
    [InlineData("")]
    [InlineData("./x")]
    [InlineData("../x")]
    [InlineData("/x")]
    [InlineData("#anchor")]
    [InlineData("?q=1")]
    [InlineData("a/b:c")]                       // the ':' is in the path, not a scheme
    [InlineData("https://example.com")]
    [InlineData("mailto:a@b.c")]
    [InlineData("tel:+123")]
    public void SafeHref_RendersUnchanged(string href) => Assert.Equal(href, RenderedHref(href));

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("JaVaScRiPt:alert(1)")]
    [InlineData(" javascript:alert(1)")]        // browsers skip leading whitespace
    [InlineData("\njavascript:alert(1)")]
    [InlineData("java\tscript:alert(1)")]       // ...and interior control characters
    [InlineData("vbscript:msgbox(1)")]
    [InlineData("data:text/html,<script>alert(1)</script>")]
    [InlineData("javascriptlonger:alert(1)")]   // longer than any allowed scheme
    public void UnsafeHref_BecomesAboutBlank(string href) => Assert.Equal("about:blank", RenderedHref(href));
}
