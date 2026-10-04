using Flare.Abstractions;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-187: the components under a theme provider or scope follow the theme, but a render of the scope's parent
/// that changes nothing about the theme does not render all of them again.
/// </summary>
public class ThemeCascadeRenderTests : FlareTestContext
{
    // A Flare component with no parameters of its own: only the theme cascade can make it render again.
    private sealed class Probe : FlareComponentBase
    {
        public int Renders;
        public bool? SeenDark;
        protected override string ComponentCssClass => "probe";
        protected override void BuildRenderTree(RenderTreeBuilder b)
        {
            Renders++;
            SeenDark = Theme?.IsDark;
            b.AddContent(0, "p");
        }
    }

    private sealed class Host : ComponentBase
    {
        [Parameter] public ThemeMode? Mode { get; set; }
        [Parameter] public int Tick { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder b)
        {
            b.OpenComponent<FlareThemeScope>(0);
            b.AddAttribute(1, nameof(FlareThemeScope.Mode), Mode);
            b.AddAttribute(2, nameof(FlareThemeScope.ChildContent), (RenderFragment)(c =>
            {
                c.AddContent(0, Tick);
                c.OpenComponent<Probe>(1);
                c.CloseComponent();
            }));
            b.CloseComponent();
        }
    }

    private IRenderedComponent<Host> RenderHost(ThemeMode? mode) => Render<Host>(p => p
        .AddCascadingValue<IThemeService>(new StubThemeService())
        .Add(x => x.Mode, mode).Add(x => x.Tick, 0));

    [Fact]
    public void ParentRender_LeavesThemedComponentsAlone()
    {
        var host = RenderHost(ThemeMode.Light);
        var probe = host.FindComponent<Probe>().Instance;
        var renders = probe.Renders;

        host.Render(p => p.Add(x => x.Mode, ThemeMode.Light).Add(x => x.Tick, 1));
        host.Render(p => p.Add(x => x.Mode, ThemeMode.Light).Add(x => x.Tick, 2));

        Assert.Contains("2", host.Markup);
        Assert.Equal(renders, probe.Renders);
    }

    [Fact]
    public void ThemeChange_RendersThemedComponents_WithTheNewTheme()
    {
        var host = RenderHost(ThemeMode.Light);
        var probe = host.FindComponent<Probe>().Instance;
        Assert.False(probe.SeenDark);
        var renders = probe.Renders;

        host.Render(p => p.Add(x => x.Mode, ThemeMode.Dark).Add(x => x.Tick, 0));

        Assert.True(probe.Renders > renders);
        Assert.True(probe.SeenDark);
    }
}
