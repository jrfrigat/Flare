using System.Globalization;

namespace Flare.Theming;

/// <summary>
/// Small, dependency-free color helpers used to derive palette roles from a brand seed
/// (containers, on-colors). A principled HCT/tonal generator can replace this in a later
/// sprint; the role contract it produces stays the same.
/// </summary>
public static class ColorMath
{
    /// <summary>Parses #RGB / #RRGGBB (alpha ignored) into 0..255 components.</summary>
    public static (int R, int G, int B) Parse(string hex)
    {
        var h = hex.TrimStart('#').Trim();
        if (h.Length == 3)
            h = $"{h[0]}{h[0]}{h[1]}{h[1]}{h[2]}{h[2]}";
        if (h.Length >= 6 &&
            int.TryParse(h.AsSpan(0, 6), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb))
            return ((rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
        return (0, 0, 0);
    }

    /// <summary>Hex.</summary>
    public static string Hex(int r, int g, int b) =>
        $"#{Clamp(r):X2}{Clamp(g):X2}{Clamp(b):X2}";

    /// <summary>Relative luminance (0..1), used to pick a readable on-color.</summary>
    public static double Luminance(string hex)
    {
        var (r, g, b) = Parse(hex);
        return (0.2126 * r + 0.7152 * g + 0.0722 * b) / 255.0;
    }

    /// <summary>A readable foreground (#1A1A1A or #FFFFFF) for text/icons on the given color.</summary>
    public static string OnColor(string hex) => Luminance(hex) > 0.55 ? "#1A1A1A" : "#FFFFFF";

    /// <summary>WCAG 2.x relative luminance (gamma-corrected sRGB), the basis for contrast ratios.</summary>
    public static double WcagLuminance(string hex)
    {
        var (r, g, b) = Parse(hex);
        static double Channel(int c)
        {
            var cs = c / 255.0;
            return cs <= 0.03928 ? cs / 12.92 : Math.Pow((cs + 0.055) / 1.055, 2.4);
        }
        return 0.2126 * Channel(r) + 0.7152 * Channel(g) + 0.0722 * Channel(b);
    }

    /// <summary>WCAG contrast ratio between two colors, from 1.0 (identical) to 21.0 (black on white).</summary>
    public static double ContrastRatio(string a, string b)
    {
        var la = WcagLuminance(a);
        var lb = WcagLuminance(b);
        var (hi, lo) = la >= lb ? (la, lb) : (lb, la);
        return (hi + 0.05) / (lo + 0.05);
    }

    /// <summary>Mixes two colors; <paramref name="t"/>=0 returns a, 1 returns b.</summary>
    public static string Mix(string a, string b, double t)
    {
        var (ar, ag, ab) = Parse(a);
        var (br, bg, bb) = Parse(b);
        return Hex(
            (int)Math.Round(ar + (br - ar) * t),
            (int)Math.Round(ag + (bg - ag) * t),
            (int)Math.Round(ab + (bb - ab) * t));
    }

    /// <summary>Mixes toward white by <paramref name="t"/> (0..1).</summary>
    public static string Lighten(string hex, double t) => Mix(hex, "#FFFFFF", t);

    /// <summary>Mixes toward black by <paramref name="t"/> (0..1).</summary>
    public static string Darken(string hex, double t) => Mix(hex, "#000000", t);

    /// <summary>Converts a hex color to HSL (H in degrees 0..360, S/L in 0..1).</summary>
    public static (double H, double S, double L) ToHsl(string hex)
    {
        var (r8, g8, b8) = Parse(hex);
        double r = r8 / 255.0, g = g8 / 255.0, b = b8 / 255.0;
        double max = Math.Max(r, Math.Max(g, b)), min = Math.Min(r, Math.Min(g, b)), d = max - min;
        double l = (max + min) / 2.0;
        double h, s;
        if (d == 0) { h = 0; s = 0; }
        else
        {
            s = d / (1 - Math.Abs(2 * l - 1));
            if (max == r) h = 60 * (((g - b) / d) % 6);
            else if (max == g) h = 60 * ((b - r) / d + 2);
            else h = 60 * ((r - g) / d + 4);
            if (h < 0) h += 360;
        }
        return (h, s, l);
    }

    /// <summary>Builds a hex color from HSL (H in degrees, S/L in 0..1).</summary>
    public static string FromHsl(double h, double s, double l)
    {
        h = ((h % 360) + 360) % 360;
        s = Math.Clamp(s, 0, 1);
        l = Math.Clamp(l, 0, 1);
        double c = (1 - Math.Abs(2 * l - 1)) * s;
        double x = c * (1 - Math.Abs(h / 60 % 2 - 1));
        double m = l - c / 2;
        double r, g, b;
        if (h < 60) { r = c; g = x; b = 0; }
        else if (h < 120) { r = x; g = c; b = 0; }
        else if (h < 180) { r = 0; g = c; b = x; }
        else if (h < 240) { r = 0; g = x; b = c; }
        else if (h < 300) { r = x; g = 0; b = c; }
        else { r = c; g = 0; b = x; }
        return Hex((int)Math.Round((r + m) * 255), (int)Math.Round((g + m) * 255), (int)Math.Round((b + m) * 255));
    }

    /// <summary>Returns the color with its lightness replaced (0..1).</summary>
    public static string WithLightness(string hex, double l)
    {
        var (h, s, _) = ToHsl(hex);
        return FromHsl(h, s, l);
    }

    /// <summary>Returns the color with its saturation replaced (0..1).</summary>
    public static string WithSaturation(string hex, double s)
    {
        var (h, _, l) = ToHsl(hex);
        return FromHsl(h, s, l);
    }

    /// <summary>Returns the color with its hue rotated by <paramref name="degrees"/>.</summary>
    public static string RotateHue(string hex, double degrees)
    {
        var (h, s, l) = ToHsl(hex);
        return FromHsl(h + degrees, s, l);
    }

    private static int Clamp(int v) => v < 0 ? 0 : v > 255 ? 255 : v;
}
