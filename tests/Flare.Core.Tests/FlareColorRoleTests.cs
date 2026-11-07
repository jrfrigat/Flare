using Flare.Components;

namespace Flare.Core.Tests;

public class FlareColorRoleTests
{
    public static TheoryData<FlareColor, string> Roles => new()
    {
        { FlareColor.Primary, "primary" },
        { FlareColor.Secondary, "secondary" },
        { FlareColor.Tertiary, "tertiary" },
        { FlareColor.Success, "success" },
        { FlareColor.Warning, "warning" },
        { FlareColor.Error, "error" },
        { FlareColor.Info, "info" },
        { FlareColor.OnSurface, "on-surface" },
        { FlareColor.OnSurfaceVariant, "on-surface-variant" },
        { FlareColor.OnSurfaceVariant2, "on-surface-variant2" },
    };

    [Theory]
    [MemberData(nameof(Roles))]
    public void RoleClassesAreShared(FlareColor color, string role)
    {
        var css = color.CssClass;
        Assert.Equal($"flare-color-{role}", css);
        Assert.Same(css, FlareColor.Custom($"var(--flare-color-{role})").CssClass);
    }

    [Fact]
    public void DefaultAndCustomColorsHaveNoClass()
    {
        Assert.Null(FlareColor.Default.CssClass);
        Assert.Null(FlareColor.Custom("#123456").CssClass);
    }
}
