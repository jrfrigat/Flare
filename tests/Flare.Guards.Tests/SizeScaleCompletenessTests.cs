using System.Reflection;

namespace Flare.Guards.Tests;

/// <summary>
/// A size scale has to name every one of its steps, the middle one included. The default step used to
/// be the absence of a class: nothing in the DOM said what size the control was, and a stylesheet could
/// not reach it except by excluding all four other steps by name - <c>:not(--xs):not(--sm):not(--lg):not(--xl)</c>.
/// That is a consumer problem, not a tidiness one: <c>button.css</c> and <c>buttongroup.css</c> already
/// rely on <c>.flare-btn--md</c> existing (<c>.flare-btn-group--connected > .flare-btn--md</c>), and the
/// same selector could not be written for a checkbox.
/// </summary>
public sealed class SizeScaleCompletenessTests
{
    private static readonly string[] Steps = ["SizeXs", "SizeSm", "SizeMd", "SizeLg", "SizeXl"];

    /// <summary>
    /// Types whose Xs..Xl are NOT a control size scale, so the middle step means nothing there:
    /// <c>Container</c> names breakpoint max-widths and <c>Card</c>/<c>Dialog</c> are panel widths that
    /// already carry their own complete set.
    /// </summary>
    private static readonly string[] NotAControlScale = ["Container", "Utility"];

    [Fact]
    public void EverySizeScaleNamesItsMiddleStep()
    {
        var offenders = new List<string>();

        foreach (var type in ClassRegistryTypes())
        {
            var present = Steps.Where(s => Constant(type, s) is not null).ToList();
            // A scale, not a lone constant: at least the two steps either side of the middle.
            if (!present.Contains("SizeSm") || !present.Contains("SizeLg")) continue;
            if (present.Contains("SizeMd")) continue;

            offenders.Add($"{type.Name} (has {string.Join(", ", present)})");
        }

        Assert.True(offenders.Count == 0,
            "These size scales name every step but the middle one, so the default size has no class and "
            + "no selector can reach it: " + string.Join("; ", offenders));
    }

    // The middle step must be the same CSS name the others follow, or the constant is there and the
    // stylesheet still cannot be written against it.
    [Fact]
    public void TheMiddleStepFollowsItsOwnScalesNaming()
    {
        var offenders = new List<string>();

        foreach (var type in ClassRegistryTypes())
        {
            var sm = Constant(type, "SizeSm");
            var md = Constant(type, "SizeMd");
            if (sm is null || md is null) continue;

            var expected = sm[..^"sm".Length] + "md";
            if (md != expected) offenders.Add($"{type.Name}: SizeMd is \"{md}\", expected \"{expected}\"");
        }

        Assert.True(offenders.Count == 0, string.Join("; ", offenders));
    }

    private static string? Constant(Type type, string name) =>
        type.GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            is { IsLiteral: true } f && f.FieldType == typeof(string)
            ? (string?)f.GetRawConstantValue()
            : null;

    private static IEnumerable<Type> ClassRegistryTypes() =>
        typeof(Flare.Css.Classes.Button).Assembly.GetTypes()
            .Where(t => t.Namespace == "Flare.Css.Classes"
                     && !NotAControlScale.Contains(t.Name));
}
