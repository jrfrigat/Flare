namespace Flare;

/// <summary>Theme color roles exposed as ready-to-use CSS <c>var()</c> expressions.</summary>
public static class Colors
{
    /// <summary>Primary color accessor.</summary>
    public static string Primary => Css.Tokens.Vars.Var(Css.Tokens.Color.Primary);
    /// <summary>On primary color accessor.</summary>
    public static string OnPrimary => Css.Tokens.Vars.Var(Css.Tokens.Color.OnPrimary);
    /// <summary>Primary container color accessor.</summary>
    public static string PrimaryContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.PrimaryContainer);
    /// <summary>On primary container color accessor.</summary>
    public static string OnPrimaryContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.OnPrimaryContainer);

    /// <summary>Secondary color accessor.</summary>
    public static string Secondary => Css.Tokens.Vars.Var(Css.Tokens.Color.Secondary);
    /// <summary>On secondary color accessor.</summary>
    public static string OnSecondary => Css.Tokens.Vars.Var(Css.Tokens.Color.OnSecondary);
    /// <summary>Secondary container color accessor.</summary>
    public static string SecondaryContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.SecondaryContainer);
    /// <summary>On secondary container color accessor.</summary>
    public static string OnSecondaryContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.OnSecondaryContainer);

    /// <summary>Tertiary color accessor.</summary>
    public static string Tertiary => Css.Tokens.Vars.Var(Css.Tokens.Color.Tertiary);
    /// <summary>On tertiary color accessor.</summary>
    public static string OnTertiary => Css.Tokens.Vars.Var(Css.Tokens.Color.OnTertiary);
    /// <summary>Tertiary container color accessor.</summary>
    public static string TertiaryContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.TertiaryContainer);
    /// <summary>On tertiary container color accessor.</summary>
    public static string OnTertiaryContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.OnTertiaryContainer);

    /// <summary>Success color accessor.</summary>
    public static string Success => Css.Tokens.Vars.Var(Css.Tokens.Color.Success);
    /// <summary>On success color accessor.</summary>
    public static string OnSuccess => Css.Tokens.Vars.Var(Css.Tokens.Color.OnSuccess);
    /// <summary>Success container color accessor.</summary>
    public static string SuccessContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.SuccessContainer);
    /// <summary>On success container color accessor.</summary>
    public static string OnSuccessContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.OnSuccessContainer);

    /// <summary>Warning color accessor.</summary>
    public static string Warning => Css.Tokens.Vars.Var(Css.Tokens.Color.Warning);
    /// <summary>On warning color accessor.</summary>
    public static string OnWarning => Css.Tokens.Vars.Var(Css.Tokens.Color.OnWarning);
    /// <summary>Warning container color accessor.</summary>
    public static string WarningContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.WarningContainer);
    /// <summary>On warning container color accessor.</summary>
    public static string OnWarningContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.OnWarningContainer);

    /// <summary>Error color accessor.</summary>
    public static string Error => Css.Tokens.Vars.Var(Css.Tokens.Color.Error);
    /// <summary>On error color accessor.</summary>
    public static string OnError => Css.Tokens.Vars.Var(Css.Tokens.Color.OnError);
    /// <summary>Error container color accessor.</summary>
    public static string ErrorContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.ErrorContainer);
    /// <summary>On error container color accessor.</summary>
    public static string OnErrorContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.OnErrorContainer);

    /// <summary>Info color accessor.</summary>
    public static string Info => Css.Tokens.Vars.Var(Css.Tokens.Color.Info);
    /// <summary>On info color accessor.</summary>
    public static string OnInfo => Css.Tokens.Vars.Var(Css.Tokens.Color.OnInfo);
    /// <summary>Info container color accessor.</summary>
    public static string InfoContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.InfoContainer);
    /// <summary>On info container color accessor.</summary>
    public static string OnInfoContainer => Css.Tokens.Vars.Var(Css.Tokens.Color.OnInfoContainer);

    /// <summary>Surface color accessor.</summary>
    public static string Surface => Css.Tokens.Vars.Var(Css.Tokens.Color.Surface);
    /// <summary>On surface color accessor.</summary>
    public static string OnSurface => Css.Tokens.Vars.Var(Css.Tokens.Color.OnSurface);
    /// <summary>Surface variant color accessor.</summary>
    public static string SurfaceVariant => Css.Tokens.Vars.Var(Css.Tokens.Color.SurfaceVariant);
    /// <summary>On surface variant color accessor.</summary>
    public static string OnSurfaceVariant => Css.Tokens.Vars.Var(Css.Tokens.Color.OnSurfaceVariant);
    /// <summary>On surface variant 2 (fainter tertiary text tone) color accessor.</summary>
    public static string OnSurfaceVariant2 => Css.Tokens.Vars.Var(Css.Tokens.Color.OnSurfaceVariant2);

    /// <summary>Background color accessor.</summary>
    public static string Background => Css.Tokens.Vars.Var(Css.Tokens.Color.Background);
    /// <summary>On background color accessor.</summary>
    public static string OnBackground => Css.Tokens.Vars.Var(Css.Tokens.Color.OnBackground);
    /// <summary>Outline color accessor.</summary>
    public static string Outline => Css.Tokens.Vars.Var(Css.Tokens.Color.Outline);
    /// <summary>Outline variant color accessor.</summary>
    public static string OutlineVariant => Css.Tokens.Vars.Var(Css.Tokens.Color.OutlineVariant);
}
