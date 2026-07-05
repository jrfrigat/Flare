using System.Runtime.CompilerServices;

namespace Flare.Core.Tests;

/// <summary>
/// Architecture guard for the field-family chrome consolidation: the supporting-text row
/// (<c>flare-input__support</c>, which wraps the helper/error text + counter) must be rendered ONLY by
/// the shared <c>FlareFieldChrome</c> frame. Every field (text/numeric/mask/password/textarea,
/// select/multiselect/combobox/tagfield, and the date/time pickers) composes that frame instead of
/// re-emitting its own support row, so a field that grows its own helper/error stack - re-introducing the
/// duplication this refactor removed - fails here.
///
/// Only <c>__support</c> is asserted, deliberately: <c>flare-input__label</c> and
/// <c>flare-input__helper</c> have legitimate non-frame emitters (FlareField's floating label,
/// FlareFormField, the intentionally-unconverted FlareDateRangePicker, the color customizer), whereas the
/// support row is genuinely frame-exclusive, so this needs no allowlist that could rot.
/// </summary>
public sealed class FieldChromeGuardTests
{
    // Razor compiles the token reference, not the raw class literal, so match BOTH forms.
    private static readonly string[] SupportMarkers = ["flare-input__support", "Css.Classes.Input.Support"];
    private const string FrameFile = "FlareFieldChrome.razor";

    [Fact]
    public void SupportRow_IsRenderedOnlyByTheSharedFrame()
    {
        var componentsDir = Path.Combine(FindRepoRoot(), "src", "Flare.Components");
        Assert.True(Directory.Exists(componentsDir), $"Missing components dir: {componentsDir}");

        var offenders = new List<string>();
        var emitters = new List<string>();

        foreach (var file in Directory.EnumerateFiles(componentsDir, "*.razor", SearchOption.AllDirectories))
        {
            if (IsBuildArtifact(file)) continue;
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                if (!SupportMarkers.Any(m => lines[i].Contains(m, StringComparison.Ordinal))) continue;
                emitters.Add(Path.GetFileName(file));
                if (!string.Equals(Path.GetFileName(file), FrameFile, StringComparison.Ordinal))
                    offenders.Add($"{Path.GetRelativePath(componentsDir, file)}:{i + 1}");
            }
        }

        Assert.True(offenders.Count == 0,
            "The field supporting-text row (flare-input__support) is frame-exclusive; render helper/error " +
            $"through FlareFieldChrome instead of re-emitting the support row. Offenders:\n  " +
            string.Join("\n  ", offenders));

        // The frame must still emit it, or the guard above would pass vacuously after an accidental deletion.
        Assert.Contains(FrameFile, emitters);
    }

    private static bool IsBuildArtifact(string path)
    {
        var sep = Path.DirectorySeparatorChar;
        return path.Contains($"{sep}bin{sep}", StringComparison.Ordinal)
            || path.Contains($"{sep}obj{sep}", StringComparison.Ordinal);
    }

    // The compile-time path of THIS test file, walked up until the folder that contains "src".
    private static string FindRepoRoot([CallerFilePath] string thisFile = "")
    {
        var dir = Path.GetDirectoryName(thisFile);
        while (dir is not null && !Directory.Exists(Path.Combine(dir, "src")))
            dir = Path.GetDirectoryName(dir);
        Assert.False(dir is null, "Could not locate the repository root (no ancestor 'src' folder).");
        return dir!;
    }
}
