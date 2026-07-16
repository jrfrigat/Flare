using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

// Guard against the "wrapper silently drops the inner component's parameters" regression class.
// A composing wrapper forwards a hand-picked subset of the inner component's parameters, and it is
// easy to omit one (FlarePasswordField once shipped without OnKeyDown/OnKeyUp; FlareClipboard without
// Color). These tests pin the interaction surface so a new wrapper cannot regress it unnoticed.

public class FieldPassthroughGuardTests
{
    // The interaction callbacks the canonical FlareField exposes. Every editable field wrapper that
    // composes it must expose the same set, so "press Enter to submit" etc. works on any field.
    private static readonly string[] RequiredCallbacks = { "OnKeyDown", "OnKeyUp", "OnFocus", "OnBlur" };

    public static IEnumerable<object[]> EditableFieldTypes =>
        typeof(FlareEditableFieldBase).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic
                        && typeof(FlareEditableFieldBase).IsAssignableFrom(t))
            .Select(t => new object[] { t });

    [Theory]
    [MemberData(nameof(EditableFieldTypes))]
    public void EditableField_ExposesInteractionCallbacks(Type componentType)
    {
        foreach (var name in RequiredCallbacks)
        {
            var prop = componentType.GetProperty(name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            Assert.True(prop is not null,
                $"{componentType.Name} is missing a public '{name}' parameter (wrappers must forward it to the inner field).");
            Assert.True(prop!.GetCustomAttribute<ParameterAttribute>() is not null,
                $"{componentType.Name}.{name} must be a [Parameter].");

            var isEventCallback = prop.PropertyType == typeof(EventCallback)
                || (prop.PropertyType.IsGenericType
                    && prop.PropertyType.GetGenericTypeDefinition() == typeof(EventCallback<>));
            Assert.True(isEventCallback, $"{componentType.Name}.{name} must be an EventCallback.");
        }
    }
}

// ------------------------------------------------------------------------------
// FlareClipboard forwards Color to the inner button (emphasized copy control)
// ------------------------------------------------------------------------------
public class C_FlareClipboardColorTests : FlareTestContext
{
    [Fact]
    public void Color_IsForwardedToInnerButton()
    {
        var cut = Render<FlareClipboard>(p => p
            .Add(x => x.Text, "secret")
            .Add(x => x.Color, FlareColor.Primary));

        Assert.Contains("flare-color-primary", cut.Find("button.flare-btn").ClassName);
    }

    [Fact]
    public void DefaultColor_AddsNoColorClass()
    {
        var cut = Render<FlareClipboard>(p => p.Add(x => x.Text, "secret"));

        Assert.DoesNotContain("flare-color-", cut.Find("button.flare-btn").ClassName);
    }
}
