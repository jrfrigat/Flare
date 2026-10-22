using System.Reflection;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// Escape belongs to the dialog from the moment its content renders, but the JS registration that normally
/// owns it arrives one round trip later. This drives the in-tree fallback in that window: the browser test
/// cannot reproduce the delay locally, so the registered flag is put back to its pre-registration state.
/// </summary>
public sealed class DialogEscapeFallbackTests : FlareTestContext
{
    [Fact]
    public void EscapeClosesWhenTheJsRegistrationIsNotInPlace()
    {
        var visible = true;
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.Title, "Settings")
            .Add(x => x.CloseOnEsc, true)
            .Add(x => x.VisibleChanged, (bool value) => visible = value)
            .AddChildContent("<p>Body</p>"));

        var registered = typeof(FlareDialog).GetField("_escRegistered", BindingFlags.Instance | BindingFlags.NonPublic)!;
        registered.SetValue(cut.Instance, false);

        cut.Find("[role=dialog]").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.False(visible);
    }
}
