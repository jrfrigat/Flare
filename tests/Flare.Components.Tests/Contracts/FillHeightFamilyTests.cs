using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

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
        { typeof(FlareCard), $".{Css.Classes.Card.Root}" },
        { typeof(FlarePaper), $".{Css.Classes.Paper.Root}" },
        { typeof(FlareStack), $".{Css.Classes.Stack.Root}" },
        { typeof(FlareGrid), $".{Css.Classes.Grid.Root}" },
        { typeof(FlareCol), $".{Css.Classes.Col.Root}" },
    };

    [Theory]
    [MemberData(nameof(Family))]
    public void Container_MarksItsRootWhenFilling(Type container, string rootSelector)
    {
        var plain = RenderContainer(container, fill: false);
        Assert.DoesNotContain(Css.Classes.Fill.Root, plain.Find(rootSelector).ClassName, StringComparison.Ordinal);

        var filled = RenderContainer(container, fill: true);
        Assert.Contains(Css.Classes.Fill.Root, filled.Find(rootSelector).ClassName, StringComparison.Ordinal);
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
        var tabsClass = tabs.Find($".{Css.Classes.Tabs.Root}").ClassName;
        Assert.Contains(Css.Classes.Fill.Root, tabsClass, StringComparison.Ordinal);
        Assert.Contains(Css.Classes.Tabs.Fill, tabsClass, StringComparison.Ordinal);

        var grid = Render<FlareDataGrid<string>>(p => p
            .Add(x => x.Items, new[] { "a" }.AsEnumerable())
            .Add(x => x.FillHeight, true));
        var gridClass = grid.Find($".{Css.Classes.DataGrid.Root}").ClassName;
        Assert.Contains(Css.Classes.Fill.Root, gridClass, StringComparison.Ordinal);
        Assert.Contains(Css.Classes.DataGrid.Fill, gridClass, StringComparison.Ordinal);
    }

    // The dialog is the second member with a mechanism of its own: it is centred on a scrim that is a
    // ROW flex container, so the shared `flex: 1 1 0` would set its flex-basis on the HORIZONTAL axis
    // and take over the panel's width. Height is the only axis a dialog fills.
    [Fact]
    public void Dialog_FillsWithItsOwnRule()
    {
        var plain = Render<FlareDialog>(p => p.Add(x => x.Visible, true));
        Assert.DoesNotContain("--fill", plain.Find($".{Css.Classes.Dialog.Root}").ClassName, StringComparison.Ordinal);

        var filled = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.FillHeight, true));

        var cls = filled.Find($".{Css.Classes.Dialog.Root}").ClassName;
        Assert.Contains(Css.Classes.Dialog.Fill, cls, StringComparison.Ordinal);
        Assert.DoesNotContain(Css.Classes.Fill.Root, cls, StringComparison.Ordinal);
    }

    [Fact]
    public void Dialog_InheritsTheSharedContract()
    {
        var declaredBy = typeof(FlareDialog)
            .GetProperty(nameof(FlareContainerBase.FillHeight))!.DeclaringType;

        Assert.True(typeof(FlareContainerBase).IsAssignableFrom(typeof(FlareDialog)));
        Assert.Equal(typeof(FlareContainerBase), declaredBy);
    }

    // A container with a definite height of its own passes it down without joining the family at all:
    // it is a plain block box, so a filling child's `block-size: 100%` resolves against it. Measured in
    // Chrome on Flare's own stylesheet - a 300px resizable box, a filling grid at 300px, its table
    // container scrolling 2699px of rows in 218px - which is why FlareResizable does NOT get the
    // parameter. Adding one there would be a switch for something that already happens.
    [Fact]
    public void AContainerWithItsOwnHeightNeedsNoSwitch()
    {
        Assert.False(typeof(FlareContainerBase).IsAssignableFrom(typeof(FlareResizable)),
            "FlareResizable sizes itself from InitialSize and the user's drag, and hands that height to "
            + "its content as an ordinary block box. FillHeight there would contradict its own purpose.");
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
        Assert.Contains(Css.Classes.Layout.ContentFill, cls, StringComparison.Ordinal);
        Assert.DoesNotContain(Css.Classes.Fill.Root, cls, StringComparison.Ordinal);
    }

    // "Be 24rem tall" and "fill your parent" are contradictory, and the contradiction used to resolve
    // into a third thing that is neither: filling sets `flex-basis: 0`, which replaces the height on a
    // flex parent's main axis, so the box came out at its content height and every link below it
    // collapsed with it. The written number wins now, and the fill is dropped.
    [Theory]
    [InlineData("height:24rem", false)]
    [InlineData("height : 24rem", false)]
    [InlineData("block-size:24rem", false)]
    [InlineData("margin:0;height:100%", false)]
    [InlineData("max-height:24rem", true)]
    [InlineData("min-height:24rem", true)]
    [InlineData("line-height:1.5", true)]
    [InlineData("--height:24rem", true)]
    [InlineData("padding:8px", true)]
    [InlineData("", true)]
    public void AnOwnHeightWinsOverFilling(string style, bool fills)
    {
        var cut = Render<FlareCard>(p => p
            .Add(x => x.FillHeight, true)
            .Add(x => x.Style, style)
            .AddChildContent("content"));

        var cls = cut.Find($".{Css.Classes.Card.Root}").ClassName ?? "";
        Assert.Equal(fills, cls.Contains(Css.Classes.Fill.Root, StringComparison.Ordinal));
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
