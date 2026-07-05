using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests.Combobox;

// bUnit tests for the rebuilt generic FlareTagField<T> (free-typing multi over the engine-driven dropdown).
public class FlareTagFieldTests : FlareTestContext
{
    [Fact]
    public void Enter_commits_a_custom_string_tag()
    {
        IReadOnlyList<string> tags = [];
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => tags = v)));

        var input = cut.Find("input.flare-tag-input__input");
        input.Input("Blazor");
        input.KeyDown(new KeyboardEventArgs { Key = "Enter" });

        Assert.Contains("Blazor", tags);
    }

    [Fact]
    public void Comma_separator_commits_a_tag()
    {
        IReadOnlyList<string> tags = [];
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => tags = v)));

        var input = cut.Find("input.flare-tag-input__input");
        input.Input("csharp");
        input.KeyDown(new KeyboardEventArgs { Key = "," });

        Assert.Contains("csharp", tags);
    }

    [Fact]
    public void Backspace_on_empty_removes_the_last_tag()
    {
        IReadOnlyList<string> tags = ["one", "two"];
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Values, tags)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => tags = v)));

        cut.Find("input.flare-tag-input__input").KeyDown(new KeyboardEventArgs { Key = "Backspace" });

        Assert.Equal(new[] { "one" }, tags);
    }

    [Fact]
    public void MaxTags_blocks_adding_beyond_the_limit()
    {
        IReadOnlyList<string> tags = ["a", "b"];
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.MaxTags, 2)
            .Add(x => x.Values, tags)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => tags = v)));

        // At the cap, the input is not even rendered.
        Assert.Empty(cut.FindAll("input.flare-tag-input__input"));
    }

    [Fact]
    public void Duplicate_tag_is_rejected_by_default()
    {
        IReadOnlyList<string> tags = ["dup"];
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Values, tags)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => tags = v)));

        var input = cut.Find("input.flare-tag-input__input");
        input.Input("dup");
        input.KeyDown(new KeyboardEventArgs { Key = "Enter" });

        Assert.Single(tags);   // no duplicate added
    }

    [Fact]
    public void Uncontrolled_keeps_tags_across_parent_rerender()
    {
        // Regression: uncontrolled (no @bind-Values) tags must survive a parent re-render.
        var cut = Render<FlareTagField<string>>();
        var input = cut.Find("input.flare-tag-input__input");
        input.Input("alpha");
        input.KeyDown(new KeyboardEventArgs { Key = "Enter" });
        Assert.Single(cut.FindAll(".flare-multiselect__chip"));

        cut.Render(p => p.Add(x => x.Placeholder, "add"));   // simulate a parent re-render
        Assert.Single(cut.FindAll(".flare-multiselect__chip"));
    }

    [Fact]
    public void Chip_remove_button_removes_the_tag()
    {
        IReadOnlyList<string> tags = ["keep", "drop"];
        var cut = Render<FlareTagField<string>>(p => p
            .Add(x => x.Values, tags)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IReadOnlyList<string>>(this, v => tags = v)));

        var removeButtons = cut.FindAll(".flare-multiselect__chip-remove");
        Assert.Equal(2, removeButtons.Count);
        removeButtons[1].Click();   // remove "drop"

        Assert.Equal(new[] { "keep" }, tags);
    }
}
