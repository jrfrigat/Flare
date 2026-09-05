using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareNavAutoExpandTests : FlareTestContext
{
    [Fact]
    public void NavLink_OnActiveChanged_FiresWhenActive()
    {
        bool? reported = null;
        Render<FlareNavLink>(p => p
            .Add(x => x.Href, "/x")
            .Add(x => x.Active, true)
            .Add(x => x.OnActiveChanged, (bool a) => reported = a)
            .AddChildContent("X"));

        Assert.True(reported);
    }

    [Fact]
    public void NavGroup_AutoExpands_WhenChildLinkActive()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Label, "Group")
            .Add(x => x.Expanded, false)
            .AddChildContent<FlareNavLink>(link => link
                .Add(l => l.Href, "/deep")
                .Add(l => l.Active, true)
                .AddChildContent("Deep")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Navigation.GroupItemsOpen}"));
        Assert.Contains(Css.Classes.Navigation.NavGroupExpanded, cut.Find($".{Css.Classes.Navigation.NavGroup}").ClassName);
    }

    [Fact]
    public void NavGroup_StaysCollapsed_WhenNoChildActive()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Label, "Group")
            .Add(x => x.Expanded, false)
            .AddChildContent<FlareNavLink>(link => link
                .Add(l => l.Href, "/inactive")
                .AddChildContent("Inactive")));

        Assert.Empty(cut.FindAll($".{Css.Classes.Navigation.GroupItemsOpen}"));
    }

    [Fact]
    public void NestedGroups_BothExpand_WhenDeepLinkActive()
    {
        var cut = Render<FlareNavGroup>(p => p
            .Add(x => x.Label, "Outer")
            .Add(x => x.Expanded, false)
            .AddChildContent<FlareNavGroup>(inner => inner
                .Add(g => g.Label, "Inner")
                .Add(g => g.Expanded, false)
                .AddChildContent<FlareNavLink>(link => link
                    .Add(l => l.Href, "/deep")
                    .Add(l => l.Active, true)
                    .AddChildContent("Deep"))));

        // both the outer and inner groups end up expanded
        Assert.Equal(2, cut.FindAll($".{Css.Classes.Navigation.GroupItemsOpen}").Count);
        Assert.Equal(2, cut.FindAll($".{Css.Classes.Navigation.NavGroupExpanded}").Count);
    }
}

// ------------------------------------------------------------------------------
// Href safety: relative links must survive, script-bearing schemes must not
// ------------------------------------------------------------------------------
