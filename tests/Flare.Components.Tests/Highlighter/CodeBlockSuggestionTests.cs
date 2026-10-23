using Flare.Components;
using Flare.Components.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

/// <summary>
/// The suggestion list is the editable half of FlareCodeBlock: which offers appear, which one is
/// chosen, and what accepting one does to the text the host is holding. The highlighter is faked so
/// the flow is deterministic and the JS boundary is a recorded contract.
/// </summary>
public class CodeBlockSuggestionTests : FlareTestContext
{
    private readonly FakeHighlighter _highlight = new();

    public CodeBlockSuggestionTests()
    {
        Services.AddScoped<IHighlightJsService>(_ => _highlight);
    }

    private IRenderedComponent<FlareCodeBlock> RenderEditor(
        FlareCodeSuggestionProvider? provider = null,
        string value = "abcd",
        int maxSuggestions = 8,
        Action<string>? changed = null)
        => Render<FlareCodeBlock>(p =>
        {
            p.Add(x => x.Value, value);
            p.Add(x => x.ReadOnly, false);
            p.Add(x => x.MaxSuggestions, maxSuggestions);
            if (provider is not null) p.Add(x => x.SuggestionProvider, provider);
            if (changed is not null)
                p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, changed));
        });

    private static FlareCodeSuggestionProvider Offers(params FlareCodeSuggestion[] suggestions)
        => _ => ValueTask.FromResult<IReadOnlyList<FlareCodeSuggestion>>(suggestions);

    private static int SelectedIndex(IRenderedComponent<FlareCodeBlock> cut)
    {
        var options = cut.FindAll("[role=option]");
        for (var i = 0; i < options.Count; i++)
            if (options[i].GetAttribute("aria-selected") == "true") return i;
        return -1;
    }

    private static async Task TypeAsync(IRenderedComponent<FlareCodeBlock> cut, string text)
        => await cut.InvokeAsync(() => cut.Find("textarea").Input(text));

    private static async Task PressAsync(IRenderedComponent<FlareCodeBlock> cut, string key)
        => await cut.InvokeAsync(() => cut.Find("textarea").KeyDown(new KeyboardEventArgs { Key = key }));

    [Fact]
    public async Task NoProvider_OffersNothing()
    {
        var cut = RenderEditor();

        await TypeAsync(cut, "abc");

        Assert.Empty(cut.FindAll("[role=option]"));
        Assert.Equal(0, _highlight.CaretCalls);
        Assert.Null(cut.Find("textarea").GetAttribute("aria-autocomplete"));
    }

    [Fact]
    public async Task NoCaret_ClosesTheList()
    {
        _highlight.Caret = null;
        var cut = RenderEditor(Offers(new FlareCodeSuggestion("Alpha", "Alpha")));

        await TypeAsync(cut, "abc");

        Assert.Empty(cut.FindAll("[role=option]"));
    }

    [Fact]
    public async Task MaxSuggestions_TrimsTheOffer()
    {
        var cut = RenderEditor(
            Offers(
                new FlareCodeSuggestion("A", "A"),
                new FlareCodeSuggestion("B", "B"),
                new FlareCodeSuggestion("C", "C")),
            maxSuggestions: 2);

        await TypeAsync(cut, "abcd");

        Assert.Equal(2, cut.FindAll("[role=option]").Count);
        Assert.Equal("true", cut.Find("textarea").GetAttribute("aria-expanded"));
    }

    [Fact]
    public async Task RepeatedOffer_KeepsTheChosenSuggestion()
    {
        var cut = RenderEditor(Offers(
            new FlareCodeSuggestion("A", "A"),
            new FlareCodeSuggestion("B", "B")));
        await TypeAsync(cut, "abcd");
        await PressAsync(cut, "ArrowDown");
        Assert.Equal(1, SelectedIndex(cut));

        await cut.InvokeAsync(() => cut.Find("textarea").Click());

        Assert.Equal(1, SelectedIndex(cut));
    }

    [Fact]
    public async Task AChangedOffer_ResetsTheChoice()
    {
        var calls = 0;
        var cut = RenderEditor(_ => ValueTask.FromResult<IReadOnlyList<FlareCodeSuggestion>>(
            ++calls == 1
                ? new[] { new FlareCodeSuggestion("A", "A"), new FlareCodeSuggestion("B", "B") }
                : new[] { new FlareCodeSuggestion("C", "C"), new FlareCodeSuggestion("D", "D") }));
        await TypeAsync(cut, "abcd");
        await PressAsync(cut, "ArrowDown");
        Assert.Equal(1, SelectedIndex(cut));

        await cut.InvokeAsync(() => cut.Find("textarea").Click());

        Assert.Equal(0, SelectedIndex(cut));
    }

    [Fact]
    public async Task ArrowKeys_WrapAroundTheList()
    {
        var cut = RenderEditor(Offers(
            new FlareCodeSuggestion("A", "A"),
            new FlareCodeSuggestion("B", "B")));
        await TypeAsync(cut, "abcd");
        Assert.Equal(0, SelectedIndex(cut));

        await PressAsync(cut, "ArrowDown");
        Assert.Equal(1, SelectedIndex(cut));
        await PressAsync(cut, "ArrowDown");
        Assert.Equal(0, SelectedIndex(cut));
        await PressAsync(cut, "ArrowUp");
        Assert.Equal(1, SelectedIndex(cut));
    }

    [Fact]
    public async Task Enter_AcceptsTheChosenSuggestion()
    {
        string? changed = null;
        var cut = RenderEditor(
            Offers(new FlareCodeSuggestion("Alpha", "Alpha") { ReplaceStart = 1, ReplaceLength = 2 }),
            value: "abcd",
            changed: v => changed = v);
        await TypeAsync(cut, "abcd");

        await PressAsync(cut, "Enter");

        Assert.Equal("aAlphad", changed);
        Assert.Equal("aAlphad", _highlight.LastText);
        Assert.Equal(6, _highlight.LastCaret);
        Assert.Empty(cut.FindAll("[role=option]"));
    }

    [Fact]
    public async Task Tab_AcceptsTheChosenSuggestion()
    {
        string? changed = null;
        var cut = RenderEditor(
            Offers(new FlareCodeSuggestion("Z", "Z") { ReplaceStart = 0, ReplaceLength = 0 }),
            value: "abcd",
            changed: v => changed = v);
        await TypeAsync(cut, "abcd");

        await PressAsync(cut, "Tab");

        Assert.Equal("Zabcd", changed);
    }

    [Fact]
    public async Task Accept_ClampsTheReplacementRange()
    {
        string? changed = null;
        var cut = RenderEditor(
            Offers(new FlareCodeSuggestion("S", "S") { ReplaceStart = 99, ReplaceLength = 5 }),
            value: "xy",
            changed: v => changed = v);
        await TypeAsync(cut, "xy");

        await PressAsync(cut, "Enter");

        Assert.Equal("xyS", changed);
        Assert.Equal(3, _highlight.LastCaret);
    }

    [Fact]
    public async Task Accept_WithoutAStart_InsertsAtTheCaret()
    {
        string? changed = null;
        var cut = RenderEditor(
            Offers(new FlareCodeSuggestion("Z", "Z")),
            value: "abcd",
            changed: v => changed = v);
        await TypeAsync(cut, "abcd");

        await PressAsync(cut, "Enter");

        // The fake caret sits at offset 4, which is the end of the text.
        Assert.Equal("abcdZ", changed);
        Assert.Equal(5, _highlight.LastCaret);
    }

    [Fact]
    public async Task Accept_StillDeliversTheValueWhenJavascriptFails()
    {
        _highlight.FailSetText = new InvalidOperationException("no dom");
        _highlight.FailSetCaret = new InvalidOperationException("no dom");
        string? changed = null;
        var cut = RenderEditor(
            Offers(new FlareCodeSuggestion("Z", "Z") { ReplaceStart = 0, ReplaceLength = 1 }),
            value: "abcd",
            changed: v => changed = v);
        await TypeAsync(cut, "abcd");

        await PressAsync(cut, "Enter");

        Assert.Equal("Zbcd", changed);
    }

    [Fact]
    public async Task NotReady_OffersNothingAndKeepsThePlainText()
    {
        _highlight.FailAttach = new InvalidOperationException("prerender");
        var cut = RenderEditor(Offers(new FlareCodeSuggestion("A", "A")), value: "<x>");

        await TypeAsync(cut, "abc");

        Assert.Empty(cut.FindAll("[role=option]"));
        Assert.Equal(0, _highlight.CaretCalls);
        Assert.Contains("&lt;x&gt;", cut.Find("pre").InnerHtml);
    }

    [Fact]
    public async Task KindAndDetail_AreShownBesideTheLabel()
    {
        var cut = RenderEditor(Offers(new FlareCodeSuggestion("Alpha", "Alpha")
        {
            Kind = "type",
            Detail = "the first letter",
        }));

        await TypeAsync(cut, "abcd");

        Assert.Contains("type", cut.Find("[role=option]").TextContent);
        Assert.Contains("the first letter", cut.Find("[role=option]").TextContent);
    }

    [Fact]
    public async Task ProviderFailure_LeavesTheEditorQuiet()
    {
        var cut = RenderEditor(_ => throw new InvalidOperationException("no suggestions"));

        await TypeAsync(cut, "abcd");

        Assert.Empty(cut.FindAll("[role=option]"));
    }

    [Fact]
    public async Task Blur_ClosesTheList()
    {
        var cut = RenderEditor(Offers(new FlareCodeSuggestion("A", "A")));
        await TypeAsync(cut, "abcd");
        Assert.NotEmpty(cut.FindAll("[role=option]"));

        await cut.InvokeAsync(() => cut.Find("textarea").Blur());

        Assert.Empty(cut.FindAll("[role=option]"));
    }

    [Fact]
    public async Task KeyUp_AsksForSuggestionsAtTheNewCaret()
    {
        var cut = RenderEditor(Offers(new FlareCodeSuggestion("A", "A")));
        await TypeAsync(cut, "abcd");
        var calls = _highlight.CaretCalls;

        await cut.InvokeAsync(() => cut.Find("textarea").KeyUp(new KeyboardEventArgs { Key = "a" }));

        Assert.True(_highlight.CaretCalls > calls);
    }

    [Fact]
    public async Task Escape_ClosesTheList()
    {
        var cut = RenderEditor(Offers(new FlareCodeSuggestion("A", "A")));
        await TypeAsync(cut, "abcd");
        Assert.NotEmpty(cut.FindAll("[role=option]"));

        await PressAsync(cut, "Escape");

        Assert.Empty(cut.FindAll("[role=option]"));
        Assert.Equal("false", cut.Find("textarea").GetAttribute("aria-expanded"));
    }

    private sealed class FakeHighlighter : IHighlightJsService
    {
        public FlareCaretInfo? Caret { get; set; } = new(4, 4, 10, 20, 16);
        public int CaretCalls { get; private set; }
        public Exception? FailAttach { get; set; }
        public Exception? FailSetText { get; set; }
        public Exception? FailSetCaret { get; set; }
        public string? LastText { get; private set; }
        public int? LastCaret { get; private set; }

        public ValueTask<string?> HighlightAsync(string code, string? language)
            => ValueTask.FromResult<string?>(null);

        public ValueTask EnableEditorKeysAsync(ElementReference textarea, int indentSize)
            => ValueTask.CompletedTask;

        public ValueTask DisableEditorKeysAsync(ElementReference textarea)
            => ValueTask.CompletedTask;

        public ValueTask<FlareCaretInfo?> GetCaretAsync(ElementReference textarea)
        {
            CaretCalls++;
            return ValueTask.FromResult(Caret);
        }

        public ValueTask SetCaretAsync(ElementReference textarea, int offset)
        {
            if (FailSetCaret is not null) throw FailSetCaret;
            LastCaret = offset;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetTextAsync(ElementReference textarea, string text)
        {
            if (FailSetText is not null) throw FailSetText;
            LastText = text;
            return ValueTask.CompletedTask;
        }

        public ValueTask AttachEditorSyncAsync(ElementReference textarea)
        {
            if (FailAttach is not null) throw FailAttach;
            return ValueTask.CompletedTask;
        }

        public ValueTask SyncEditorScrollAsync(ElementReference textarea)
            => ValueTask.CompletedTask;

        public ValueTask DetachEditorSyncAsync(ElementReference textarea)
            => ValueTask.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
