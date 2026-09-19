using Bunit;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// Two contracts that meet in one place - the single door markup uses to reach the editor.
/// <para>
/// The first is that the door stays open. <c>setContent</c> ran on the first render alone, so the only
/// route from <c>Value</c> to the DOM closed after the first paint: a controlled binding or a form
/// reset re-rendered the component and left the old HTML on screen.
/// </para>
/// <para>
/// The second is that what goes through it is filtered. <c>Value</c> is public API, so a stored draft
/// or a server response arrives untrusted; it used to be assigned to <c>innerHTML</c> as given.
/// The filtering itself is in the browser, where the parser is - what these tests hold is that the
/// component asks for it, and stops asking only when a caller has said the markup is trusted.
/// </para>
/// </summary>
public class FlareRichTextEditorValueTests : FlareTestContext
{
    private IReadOnlyList<string> PushesOf(string function) =>
        JSInterop.Invocations
            .Where(i => i.Identifier == function)
            .Select(i => i.Arguments.Count > 1 ? i.Arguments[1]?.ToString() ?? string.Empty : string.Empty)
            .ToList();

    private IReadOnlyList<string> SafePushes => PushesOf("setContent");
    private IReadOnlyList<string> UnsafePushes => PushesOf("setContentUnsafe");

    [Fact]
    public void InitialValue_GoesThroughTheFilter()
    {
        Render<FlareRichTextEditor>(p => p.Add(x => x.Value, "<p>hello</p>"));

        Assert.Equal(["<p>hello</p>"], SafePushes);
        Assert.Empty(UnsafePushes);
    }

    // The defect this suite is named for: the value moved and the editor never heard about it.
    [Fact]
    public void ExternalChangeAfterFirstRender_ReachesTheEditor()
    {
        var cut = Render<FlareRichTextEditor>(p => p.Add(x => x.Value, "<p>first</p>"));

        cut.Render(p => p.Add(x => x.Value, "<p>second</p>"));

        Assert.Equal(["<p>first</p>", "<p>second</p>"], SafePushes);
    }

    // A form reset is a change to the empty string, which is exactly the value a truthiness check drops.
    [Fact]
    public void ResetToEmpty_ReachesTheEditor()
    {
        var cut = Render<FlareRichTextEditor>(p => p.Add(x => x.Value, "<p>draft</p>"));

        cut.Render(p => p.Add(x => x.Value, string.Empty));

        Assert.Equal(["<p>draft</p>", ""], SafePushes);
    }

    // The surface holds a caret. Re-rendering with the value it already has must not rewrite it.
    [Fact]
    public void SameValueAgain_IsNotPushedTwice()
    {
        var cut = Render<FlareRichTextEditor>(p => p.Add(x => x.Value, "<p>same</p>"));

        cut.Render(p => p.Add(x => x.Value, "<p>same</p>"));

        Assert.Equal(["<p>same</p>"], SafePushes);
    }

    // The editor answers every keystroke with OnContentChanged, which sets Value. Without the record of
    // what was last agreed, that would come straight back as an "external" change and collapse the caret
    // to the start on every letter typed.
    [Fact]
    public async Task EditorsOwnEcho_DoesNotComeBack()
    {
        var cut = Render<FlareRichTextEditor>(p => p.Add(x => x.Value, "<p>a</p>"));

        await cut.Instance.OnContentChanged("<p>ab</p>");
        cut.Render(p => p.Add(x => x.Value, "<p>ab</p>"));

        Assert.Equal(["<p>a</p>"], SafePushes);
    }

    [Fact]
    public void SanitizeIsOnByDefault()
    {
        Render<FlareRichTextEditor>(p => p.Add(x => x.Value, "<img src=x onerror=alert(1)>"));

        Assert.Single(SafePushes);
        Assert.Empty(UnsafePushes);
    }

    // The opt-out is a statement the caller makes, and it has to actually change the route taken.
    [Fact]
    public void SanitizeFalse_TakesTheUnfilteredRoute()
    {
        Render<FlareRichTextEditor>(p => p
            .Add(x => x.Sanitize, false)
            .Add(x => x.Value, "<p>trusted</p>"));

        Assert.Equal(["<p>trusted</p>"], UnsafePushes);
        Assert.Empty(SafePushes);
    }
}
