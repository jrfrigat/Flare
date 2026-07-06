using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

// The shared editable control primitive (FlareTextInput): a string-only <input>/<textarea>.
public class C_FlareTextInputTests : FlareTestContext
{
    [Fact]
    public void Renders_input_control_by_default()
    {
        var cut = Render<FlareTextInput>(p => p.Add(x => x.Value, "hi"));
        var input = cut.Find("input.flare-input__control");
        Assert.Equal("hi", input.GetAttribute("value"));
        Assert.Empty(cut.FindAll("textarea"));
    }

    [Fact]
    public void Multiline_renders_textarea()
    {
        var cut = Render<FlareTextInput>(p => p
            .Add(x => x.Multiline, true)
            .Add(x => x.Rows, 4));
        Assert.NotEmpty(cut.FindAll("textarea.flare-input__control"));
        Assert.Equal("4", cut.Find("textarea").GetAttribute("rows"));
        Assert.Empty(cut.FindAll("input"));
    }

    [Fact]
    public void Forwards_placeholder_disabled_readonly_required_type_maxlength()
    {
        var cut = Render<FlareTextInput>(p => p
            .Add(x => x.Placeholder, "type here")
            .Add(x => x.Disabled, true)
            .Add(x => x.ReadOnly, true)
            .Add(x => x.Required, true)
            .Add(x => x.Type, "email")
            .Add(x => x.MaxLength, 20));
        var input = cut.Find("input");
        Assert.Equal("type here", input.GetAttribute("placeholder"));
        Assert.True(input.HasAttribute("disabled"));
        Assert.True(input.HasAttribute("readonly"));
        Assert.True(input.HasAttribute("required"));
        Assert.Equal("email", input.GetAttribute("type"));
        Assert.Equal("20", input.GetAttribute("maxlength"));
    }

    [Fact]
    public void Change_emits_raw_string()
    {
        string? captured = null;
        var cut = Render<FlareTextInput>(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => captured = v)));
        cut.Find("input").Change("hello");
        Assert.Equal("hello", captured);
    }

    [Fact]
    public void Immediate_emits_on_input()
    {
        string? captured = null;
        var cut = Render<FlareTextInput>(p => p
            .Add(x => x.Immediate, true)
            .Add(x => x.DebounceInterval, 0)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => captured = v)));
        cut.Find("input").Input("abc");
        Assert.Equal("abc", captured);
    }

    [Fact]
    public void Non_immediate_does_not_emit_on_input()
    {
        string? captured = null;
        var cut = Render<FlareTextInput>(p => p
            .Add(x => x.Immediate, false)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => captured = v)));
        cut.Find("input").Input("abc");
        Assert.Null(captured);   // waits for change/blur
    }

    [Fact]
    public void Aria_invalid_and_describedby_are_wired()
    {
        var cut = Render<FlareTextInput>(p => p
            .Add(x => x.AriaInvalid, true)
            .Add(x => x.AriaDescribedBy, "err-1")
            .Add(x => x.Id, "ctl-1"));
        var input = cut.Find("input");
        Assert.Equal("true", input.GetAttribute("aria-invalid"));
        Assert.Equal("err-1", input.GetAttribute("aria-describedby"));
        Assert.Equal("ctl-1", input.GetAttribute("id"));
    }
}
