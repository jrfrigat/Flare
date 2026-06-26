namespace Flare.Css.Classes;

/// <summary>
/// Column-span classes for the simple 12-column utility grid (place inside a <c>.flare-row</c> /
/// <see cref="Row"/>). Each class sets the column width as a fraction of 12. Base classes apply
/// at every breakpoint; the <c>Sm</c>/<c>Md</c>/<c>Lg</c> sets apply from that breakpoint up.
/// This utility grid is independent of the <c>FlareCol</c>/<c>FlareGrid</c> components, which use
/// CSS-variable spans rather than these fixed classes.
/// </summary>
public static class Col
{
    /// <summary>The <c>flare-col</c> base column class.</summary>
    public const string Root = "flare-col";

    /// <summary>Spans 1/12 of the row at all breakpoints.</summary>
    public const string N1 = "flare-col-1";
    /// <summary>Spans 2/12 of the row at all breakpoints.</summary>
    public const string N2 = "flare-col-2";
    /// <summary>Spans 3/12 of the row at all breakpoints.</summary>
    public const string N3 = "flare-col-3";
    /// <summary>Spans 4/12 of the row at all breakpoints.</summary>
    public const string N4 = "flare-col-4";
    /// <summary>Spans 5/12 of the row at all breakpoints.</summary>
    public const string N5 = "flare-col-5";
    /// <summary>Spans 6/12 of the row at all breakpoints.</summary>
    public const string N6 = "flare-col-6";
    /// <summary>Spans 7/12 of the row at all breakpoints.</summary>
    public const string N7 = "flare-col-7";
    /// <summary>Spans 8/12 of the row at all breakpoints.</summary>
    public const string N8 = "flare-col-8";
    /// <summary>Spans 9/12 of the row at all breakpoints.</summary>
    public const string N9 = "flare-col-9";
    /// <summary>Spans 10/12 of the row at all breakpoints.</summary>
    public const string N10 = "flare-col-10";
    /// <summary>Spans 11/12 of the row at all breakpoints.</summary>
    public const string N11 = "flare-col-11";
    /// <summary>Spans 12/12 (full width) at all breakpoints.</summary>
    public const string N12 = "flare-col-12";

    /// <summary>Spans 1/12 from the sm breakpoint (>=600px) up.</summary>
    public const string Sm1 = "flare-col-sm-1";
    /// <summary>Spans 2/12 from the sm breakpoint (>=600px) up.</summary>
    public const string Sm2 = "flare-col-sm-2";
    /// <summary>Spans 3/12 from the sm breakpoint (>=600px) up.</summary>
    public const string Sm3 = "flare-col-sm-3";
    /// <summary>Spans 4/12 from the sm breakpoint (>=600px) up.</summary>
    public const string Sm4 = "flare-col-sm-4";
    /// <summary>Spans 5/12 from the sm breakpoint (>=600px) up.</summary>
    public const string Sm5 = "flare-col-sm-5";
    /// <summary>Spans 6/12 from the sm breakpoint (>=600px) up.</summary>
    public const string Sm6 = "flare-col-sm-6";
    /// <summary>Spans 7/12 from the sm breakpoint (>=600px) up.</summary>
    public const string Sm7 = "flare-col-sm-7";
    /// <summary>Spans 8/12 from the sm breakpoint (>=600px) up.</summary>
    public const string Sm8 = "flare-col-sm-8";
    /// <summary>Spans 9/12 from the sm breakpoint (>=600px) up.</summary>
    public const string Sm9 = "flare-col-sm-9";
    /// <summary>Spans 10/12 from the sm breakpoint (>=600px) up.</summary>
    public const string Sm10 = "flare-col-sm-10";
    /// <summary>Spans 11/12 from the sm breakpoint (>=600px) up.</summary>
    public const string Sm11 = "flare-col-sm-11";
    /// <summary>Spans 12/12 (full width) from the sm breakpoint (>=600px) up.</summary>
    public const string Sm12 = "flare-col-sm-12";

    /// <summary>Spans 1/12 from the md breakpoint (>=960px) up.</summary>
    public const string Md1 = "flare-col-md-1";
    /// <summary>Spans 2/12 from the md breakpoint (>=960px) up.</summary>
    public const string Md2 = "flare-col-md-2";
    /// <summary>Spans 3/12 from the md breakpoint (>=960px) up.</summary>
    public const string Md3 = "flare-col-md-3";
    /// <summary>Spans 4/12 from the md breakpoint (>=960px) up.</summary>
    public const string Md4 = "flare-col-md-4";
    /// <summary>Spans 5/12 from the md breakpoint (>=960px) up.</summary>
    public const string Md5 = "flare-col-md-5";
    /// <summary>Spans 6/12 from the md breakpoint (>=960px) up.</summary>
    public const string Md6 = "flare-col-md-6";
    /// <summary>Spans 7/12 from the md breakpoint (>=960px) up.</summary>
    public const string Md7 = "flare-col-md-7";
    /// <summary>Spans 8/12 from the md breakpoint (>=960px) up.</summary>
    public const string Md8 = "flare-col-md-8";
    /// <summary>Spans 9/12 from the md breakpoint (>=960px) up.</summary>
    public const string Md9 = "flare-col-md-9";
    /// <summary>Spans 10/12 from the md breakpoint (>=960px) up.</summary>
    public const string Md10 = "flare-col-md-10";
    /// <summary>Spans 11/12 from the md breakpoint (>=960px) up.</summary>
    public const string Md11 = "flare-col-md-11";
    /// <summary>Spans 12/12 (full width) from the md breakpoint (>=960px) up.</summary>
    public const string Md12 = "flare-col-md-12";

    /// <summary>Spans 1/12 from the lg breakpoint (>=1280px) up.</summary>
    public const string Lg1 = "flare-col-lg-1";
    /// <summary>Spans 2/12 from the lg breakpoint (>=1280px) up.</summary>
    public const string Lg2 = "flare-col-lg-2";
    /// <summary>Spans 3/12 from the lg breakpoint (>=1280px) up.</summary>
    public const string Lg3 = "flare-col-lg-3";
    /// <summary>Spans 4/12 from the lg breakpoint (>=1280px) up.</summary>
    public const string Lg4 = "flare-col-lg-4";
    /// <summary>Spans 5/12 from the lg breakpoint (>=1280px) up.</summary>
    public const string Lg5 = "flare-col-lg-5";
    /// <summary>Spans 6/12 from the lg breakpoint (>=1280px) up.</summary>
    public const string Lg6 = "flare-col-lg-6";
    /// <summary>Spans 7/12 from the lg breakpoint (>=1280px) up.</summary>
    public const string Lg7 = "flare-col-lg-7";
    /// <summary>Spans 8/12 from the lg breakpoint (>=1280px) up.</summary>
    public const string Lg8 = "flare-col-lg-8";
    /// <summary>Spans 9/12 from the lg breakpoint (>=1280px) up.</summary>
    public const string Lg9 = "flare-col-lg-9";
    /// <summary>Spans 10/12 from the lg breakpoint (>=1280px) up.</summary>
    public const string Lg10 = "flare-col-lg-10";
    /// <summary>Spans 11/12 from the lg breakpoint (>=1280px) up.</summary>
    public const string Lg11 = "flare-col-lg-11";
    /// <summary>Spans 12/12 (full width) from the lg breakpoint (>=1280px) up.</summary>
    public const string Lg12 = "flare-col-lg-12";
}
