using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// A screen-fit page is a chain of definite heights, and one link left at <c>auto</c> collapses the
/// whole chain - silently, and worse than silently, because a collapsed box with <c>overflow</c> clips
/// its content away instead of showing less of it. The chain used to exist on three components in the
/// library, so any page whose grid sat inside a card, a stack or a layout column had no way to build it
/// without app CSS.
///
/// These are structural guards: every container in the family declares the same parameter and marks its
/// root with the same shared class. What they cannot check is the measured box, which needs a browser -
/// that is verified in the Gallery, not here.
/// </summary>
public sealed class FillHeightFamilyTests : FlareTestContext
{
    private static RenderFragment Content() => b => b.AddContent(0, "content");

    public static TheoryData<Type, string> Family => new()
    {
        { typeof(FlareCard), ".flare-card" },
        { typeof(FlarePaper), ".flare-paper" },
        { typeof(FlareStack), ".flare-stack" },
        { typeof(FlareGrid), ".flare-grid" },
        { typeof(FlareCol), ".flare-col" },
    };

    [Theory]
    [MemberData(nameof(Family))]
    public void Container_MarksItsRootWhenFilling(Type container, string rootSelector)
    {
        var plain = RenderContainer(container, fill: false);
        Assert.DoesNotContain("flare-fill", plain.Find(rootSelector).ClassName, StringComparison.Ordinal);

        var filled = RenderContainer(container, fill: true);
        Assert.Contains("flare-fill", filled.Find(rootSelector).ClassName, StringComparison.Ordinal);
    }

    // The two that had a FillHeight of their own before there was a shared one keep their component
    // rule - a tab set's panels and a grid's table container are theirs to size - and now carry the
    // shared class as well, so the "spend the height you were given" half is declared once.
    [Fact]
    public void TabsAndDataGrid_CarryBothTheSharedClassAndTheirOwn()
    {
        var tabs = Render<FlareTabs>(p => p
            .Add(x => x.FillHeight, true)
            .Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<FlareTab>(0);
                b.AddAttribute(1, "Label", "One");
                b.AddAttribute(2, "ChildContent", Content());
                b.CloseComponent();
            })));
        var tabsClass = tabs.Find(".flare-tabs").ClassName;
        Assert.Contains("flare-fill", tabsClass, StringComparison.Ordinal);
        Assert.Contains("flare-tabs--fill", tabsClass, StringComparison.Ordinal);

        var grid = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, new[] { "a" }.AsEnumerable())
            .Add(x => x.FillHeight, true));
        var gridClass = grid.Find(".flare-datagrid").ClassName;
        Assert.Contains("flare-fill", gridClass, StringComparison.Ordinal);
        Assert.Contains("flare-datagrid--fill", gridClass, StringComparison.Ordinal);
    }

    // FlareLayoutContent is the one member whose mechanism differs: `main` is a row of the shell's grid
    // rather than a flex item, so the shared triple would be wrong there and it hands the height down
    // through its own frame rule instead. The parameter is still the same parameter.
    [Fact]
    public void LayoutContent_UsesItsOwnMechanism()
    {
        var cut = Render<FlareLayoutContent>(p => p
            .Add(x => x.FillHeight, true)
            .Add(x => x.ChildContent, Content()));

        var cls = cut.Find("main").ClassName;
        Assert.Contains("flare-layout-content--fill", cls, StringComparison.Ordinal);
        Assert.DoesNotContain("flare-fill", cls, StringComparison.Ordinal);
    }

    // The parameter itself is declared once, on the base every container shares. A component that
    // re-declared it would drift in name, default or documentation - which is how the chain came to
    // cover three components and no more.
    [Theory]
    [MemberData(nameof(Family))]
    public void Container_InheritsTheSharedContract(Type container, string rootSelector)
    {
        _ = rootSelector;
        Assert.True(typeof(FlareContainerBase).IsAssignableFrom(container),
            $"{container.Name} holds other components, so it belongs to the FillHeight chain.");

        var declaredBy = container.GetProperty(nameof(FlareContainerBase.FillHeight))!.DeclaringType;
        Assert.True(declaredBy == typeof(FlareContainerBase),
            $"{container.Name} re-declares FillHeight on itself ({declaredBy?.Name}) instead of "
            + "inheriting it, which is how the parameter drifts in default or documentation.");
    }

    private IRenderedComponent<IComponent> RenderContainer(Type container, bool fill)
    {
        var method = typeof(FillHeightFamilyTests)
            .GetMethod(nameof(RenderTyped), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .MakeGenericMethod(container);
        return (IRenderedComponent<IComponent>)method.Invoke(this, [fill])!;
    }

    private IRenderedComponent<IComponent> RenderTyped<T>(bool fill) where T : FlareContainerBase =>
        (IRenderedComponent<IComponent>)(object)Render<T>(p => p
            .Add(x => x.FillHeight, fill)
            .AddChildContent("content"));
}
