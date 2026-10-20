using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

/// <summary>
/// The selected state lived only in CSS. A chip that can toggle now announces it on the action
/// button, while an action-only chip and a static tag stay plain buttons/read-only tags.
/// </summary>
public class FlareChipPressedStateTests : FlareTestContext
{
    [Fact]
    public void StandaloneToggle_AnnouncesItsStateAsText()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Filter")
            .Add(x => x.SelectedChanged, (bool _) => { }));

        Assert.Equal("false", cut.Find($"button.{Css.Classes.Chip.Action}").GetAttribute("aria-pressed"));

        cut.Render(p => p
            .Add(x => x.Label, "Filter")
            .Add(x => x.Selected, true)
            .Add(x => x.SelectedChanged, (bool _) => { }));

        Assert.Equal("true", cut.Find($"button.{Css.Classes.Chip.Action}").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void ActionOnlyChip_IsAButtonWithoutAToggleState()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "Show")
            .Add(x => x.OnClick, () => { }));

        Assert.Null(cut.Find($"button.{Css.Classes.Chip.Action}").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void StaticChip_HasNoActionAndNoToggleState()
    {
        var cut = Render<FlareChip>(p => p
            .Add(x => x.Label, "STORY-01")
            .Add(x => x.Selected, true));

        Assert.Empty(cut.FindAll($".{Css.Classes.Chip.Action}"));
        Assert.DoesNotContain("aria-pressed", cut.Markup);
    }

    [Fact]
    public void GroupToggle_AnnouncesAndUpdatesTheState()
    {
        var cut = Render<BoundGroup>();

        var action = cut.Find($"button.{Css.Classes.Chip.Action}");
        Assert.Equal("false", action.GetAttribute("aria-pressed"));

        action.Click();

        Assert.Equal("true", cut.Find($"button.{Css.Classes.Chip.Action}").GetAttribute("aria-pressed"));
    }

    private sealed class BoundGroup : ComponentBase
    {
        private IReadOnlyCollection<string> _selected = Array.Empty<string>();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<FlareChipGroup>(0);
            builder.AddAttribute(1, nameof(FlareChipGroup.SelectedValues), _selected);
            builder.AddAttribute(2, nameof(FlareChipGroup.SelectedValuesChanged),
                EventCallback.Factory.Create<IReadOnlyCollection<string>>(this, values =>
                {
                    _selected = values;
                    StateHasChanged();
                }));
            builder.AddAttribute(3, nameof(FlareChipGroup.ChildContent), (RenderFragment)(chips =>
            {
                chips.OpenComponent<FlareChip>(0);
                chips.AddAttribute(1, nameof(FlareChip.Label), "One");
                chips.AddAttribute(2, nameof(FlareChip.Value), "one");
                chips.CloseComponent();
            }));
            builder.CloseComponent();
        }
    }
}
