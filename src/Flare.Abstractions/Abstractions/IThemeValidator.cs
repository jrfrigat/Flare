namespace Flare.Abstractions;

/// <summary>
/// Validates that a theme provides all required tokens. Use to catch missing tokens
/// at registration time rather than at render time.
/// </summary>
public interface IThemeValidator
{
    /// <summary>
    /// Validates the theme and returns a list of validation errors.
    /// Empty list means the theme is valid.
    /// </summary>
    IReadOnlyList<string> Validate(ITheme theme);
}

/// <summary>
/// Default implementation that checks all required token groups are present.
/// </summary>
public sealed class ThemeValidator : IThemeValidator
{
    /// <summary>Validate.</summary>
    public IReadOnlyList<string> Validate(ITheme theme)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(theme.Id))
            errors.Add("Theme.Id must not be null or empty.");

        if (string.IsNullOrWhiteSpace(theme.DisplayName))
            errors.Add("Theme.DisplayName must not be null or empty.");

        if (theme.Design is null)
        {
            errors.Add("Theme.Design must not be null.");
            return errors;
        }

        // Typography
        if (theme.Design.Typography is null)
            errors.Add("Theme.Design.Typography is required.");
        else
        {
            if (theme.Design.Typography.BodyLarge is null) errors.Add("Typography.BodyLarge is required.");
            if (theme.Design.Typography.BodyMedium is null) errors.Add("Typography.BodyMedium is required.");
            if (theme.Design.Typography.BodySmall is null) errors.Add("Typography.BodySmall is required.");
            if (theme.Design.Typography.LabelLarge is null) errors.Add("Typography.LabelLarge is required.");
            if (theme.Design.Typography.LabelMedium is null) errors.Add("Typography.LabelMedium is required.");
            if (theme.Design.Typography.LabelSmall is null) errors.Add("Typography.LabelSmall is required.");
            if (theme.Design.Typography.HeadlineLarge is null) errors.Add("Typography.HeadlineLarge is required.");
            if (theme.Design.Typography.HeadlineMedium is null) errors.Add("Typography.HeadlineMedium is required.");
            if (theme.Design.Typography.HeadlineSmall is null) errors.Add("Typography.HeadlineSmall is required.");
            if (theme.Design.Typography.TitleLarge is null) errors.Add("Typography.TitleLarge is required.");
            if (theme.Design.Typography.TitleMedium is null) errors.Add("Typography.TitleMedium is required.");
            if (theme.Design.Typography.TitleSmall is null) errors.Add("Typography.TitleSmall is required.");
            if (theme.Design.Typography.DisplayLarge is null) errors.Add("Typography.DisplayLarge is required.");
            if (theme.Design.Typography.DisplayMedium is null) errors.Add("Typography.DisplayMedium is required.");
            if (theme.Design.Typography.DisplaySmall is null) errors.Add("Typography.DisplaySmall is required.");
        }

        // Shape
        if (theme.Design.Shape is null)
            errors.Add("Theme.Design.Shape is required.");

        // Elevation
        if (theme.Design.Elevation is null)
            errors.Add("Theme.Design.Elevation is required.");

        // Motion
        if (theme.Design.Motion is null)
            errors.Add("Theme.Design.Motion is required.");

        // State
        if (theme.Design.State is null)
            errors.Add("Theme.Design.State is required.");

        // StyleAssets
        if (theme.StyleAssets is null || !theme.StyleAssets.Any())
            errors.Add("Theme.StyleAssets must contain at least one stylesheet.");

        // DefaultPaletteId
        if (string.IsNullOrWhiteSpace(theme.DefaultPaletteId))
            errors.Add("Theme.DefaultPaletteId must not be null or empty.");

        return errors;
    }
}
