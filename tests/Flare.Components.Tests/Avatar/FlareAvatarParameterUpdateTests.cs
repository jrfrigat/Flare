using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

/// <summary>
/// Everything the avatar draws from its parameters has to follow a later parameter set, not only the
/// content text: class, style, unmatched attributes and the visibility a group decides are all part of
/// what the element shows.
/// </summary>
public class FlareAvatarParameterUpdateTests : FlareTestContext
{
    // The avatar is rendered as a child here: a root-level parameter change rebuilds the fragment
    // directly, while a child goes through the normal diff - the path a ShouldRender memo used to
    // suppress.
    private sealed class AvatarHost : ComponentBase
    {
        [Parameter] public string Text { get; set; } = "Ann Lee";
        [Parameter] public string? CssClass { get; set; }
        [Parameter] public string? InlineStyle { get; set; }
        [Parameter] public string? TestId { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<FlareAvatar>(0);
            builder.AddAttribute(1, nameof(FlareAvatar.Text), Text);
            builder.AddAttribute(2, nameof(FlareAvatar.Class), CssClass);
            builder.AddAttribute(3, nameof(FlareAvatar.Style), InlineStyle);
            builder.AddAttribute(4, "data-testid", TestId);
            builder.CloseComponent();
        }
    }

    [Fact]
    public void ALaterClassAndStyleAndAttributeChange_ReachTheElement()
    {
        var cut = Render<AvatarHost>(p => p
            .Add(x => x.CssClass, "first")
            .Add(x => x.InlineStyle, "opacity: 0.5")
            .Add(x => x.TestId, "one"));

        // Two updates, not one: ShouldRender is not consulted on the first render of a component, so
        // a single change would pass even with a memo in place.
        cut.Render(p => p
            .Add(x => x.CssClass, "middle")
            .Add(x => x.InlineStyle, "opacity: 0.75")
            .Add(x => x.TestId, "one-and-a-half"));
        cut.Render(p => p
            .Add(x => x.CssClass, "second")
            .Add(x => x.InlineStyle, "opacity: 1")
            .Add(x => x.TestId, "two"));

        var root = cut.Find($".{Css.Classes.Avatar.Root}");
        Assert.DoesNotContain("first", root.ClassName);
        Assert.Contains("second", root.ClassName);
        // The style attribute also carries the palette tokens the avatar derives from its text.
        var style = root.GetAttribute("style") ?? string.Empty;
        Assert.Contains("opacity: 1", style);
        Assert.DoesNotContain("opacity: 0.5", style);
        Assert.Equal("two", root.GetAttribute("data-testid"));
    }

    [Fact]
    public void ALaterAltChange_UpdatesTheAccessibleName()
    {
        var cut = Render<FlareAvatar>(p => p.Add(x => x.Text, "Ann Lee").Add(x => x.Alt, "First"));
        Assert.Equal("First", cut.Find($".{Css.Classes.Avatar.Root}").GetAttribute("aria-label"));

        cut.Render(p => p.Add(x => x.Text, "Ann Lee").Add(x => x.Alt, "Second"));

        Assert.Equal("Second", cut.Find($".{Css.Classes.Avatar.Root}").GetAttribute("aria-label"));
    }

    private static void AddAvatar(RenderTreeBuilder builder, int seq, string text)
    {
        builder.OpenComponent<FlareAvatar>(seq);
        builder.AddAttribute(seq + 1, nameof(FlareAvatar.Text), text);
        builder.CloseComponent();
    }

    private void AddGroupParameters(ComponentParameterCollectionBuilder<FlareAvatarGroup> p)
    {
        p.Add(x => x.Max, _max);
        p.AddChildContent(builder =>
        {
            AddAvatar(builder, 0, "Ann Lee");
            AddAvatar(builder, 10, "Bob Ray");
        });
    }

    private int _max = 1;

    // The overflow bubble carries the avatar root class too; the count is about real avatars only.
    private string AvatarSelector =>
        $".{Css.Classes.Avatar.Root}:not(.{Css.Classes.Avatar.GroupOverflow})";

    [Fact]
    public void AnAvatarHiddenByTheGroup_AppearsWhenMaxGrows()
    {
        _max = 1;
        var cut = Render<FlareAvatarGroup>(AddGroupParameters);
        Assert.Single(cut.FindAll(AvatarSelector));

        _max = 2;
        cut.Render(AddGroupParameters);

        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll(AvatarSelector).Count));
    }
}
