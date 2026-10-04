using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-188: the popup buttons are plain markup that has to look like the FlareButton they stand in for - the same
/// classes on the button and on its icon slot - and still act on a click.
/// </summary>
public class PickerButtonsTests : FlareTestContext
{
    private sealed class Host : ComponentBase
    {
        public int Clicks;
        [Parameter] public bool Text { get; set; }
        protected override void BuildRenderTree(RenderTreeBuilder b) =>
            b.AddContent(0, Text
                ? PickerButtons.Text(this, () => { Clicks++; return Task.CompletedTask; }, "OK")
                : PickerButtons.Icon(this, () => Clicks++, "Next", FlareIcons.ChevronRight));
    }

    [Fact]
    public void IconButton_HasTheClassesOfAnIconOnlyFlareButton()
    {
        var reference = Render<FlareButton>(p => p.Add(x => x.Variant, ButtonVariant.Text).Add(x => x.Size, ButtonSize.Sm)
            .Add(x => x.AriaLabel, "Next").Add(x => x.LeadingIcon, (RenderFragment)(b => b.AddContent(0, "i"))));
        var cut = Render<Host>();

        var button = cut.Find("button");
        Assert.Equal(reference.Find("button").ClassList.OrderBy(c => c), button.ClassList.OrderBy(c => c));
        Assert.Equal(reference.Find("button > span").ClassList.OrderBy(c => c), button.QuerySelector("span")!.ClassList.OrderBy(c => c));
        Assert.Equal("Next", button.GetAttribute("aria-label"));
        Assert.Equal("button", button.GetAttribute("type"));

        button.Click();
        Assert.Equal(1, cut.Instance.Clicks);
    }

    [Fact]
    public void TextButton_HasTheClassesOfATextFlareButton()
    {
        var reference = Render<FlareButton>(p => p.Add(x => x.Variant, ButtonVariant.Text).Add(x => x.Size, ButtonSize.Sm)
            .AddChildContent("OK"));
        var cut = Render<Host>(p => p.Add(x => x.Text, true));

        var button = cut.Find("button");
        Assert.Equal(reference.Find("button").ClassList.OrderBy(c => c), button.ClassList.OrderBy(c => c));
        Assert.Equal(reference.Find("button > span").ClassList.OrderBy(c => c), button.QuerySelector("span")!.ClassList.OrderBy(c => c));
        Assert.Equal("OK", button.TextContent.Trim());

        button.Click();
        Assert.Equal(1, cut.Instance.Clicks);
    }
}
