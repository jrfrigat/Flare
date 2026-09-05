using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests.Component;

// ------------------------------------------------------------------------------
// FlareDialog  (5 tests from Wave6)
// ------------------------------------------------------------------------------

public class C_FlareDialogTests : FlareTestContext
{
    [Fact]
    public void HiddenWhenVisibleFalse()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, false));

        Assert.Empty(cut.FindAll($".{Css.Classes.Dialog.Scrim}"));
    }

    [Fact]
    public void RendersWhenVisibleTrue()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Dialog.Scrim}"));
    }

    [Fact]
    public void RendersTitle_WhenProvided()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.Title, "Confirm Action"));

        Assert.Contains("Confirm Action", cut.Find($".{Css.Classes.Dialog.Title}").TextContent);
    }

    [Fact]
    public void RendersChildContent_WhenVisible()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .AddChildContent("<p id=\"dialog-body\">Body</p>"));

        Assert.NotEmpty(cut.FindAll("#dialog-body"));
    }

    [Fact]
    public void TitledDialog_LabelledByTitle_NoAriaLabel()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.Title, "Confirm"));

        var panel = cut.Find("[role=dialog]");
        Assert.True(panel.HasAttribute("aria-labelledby"));
        Assert.False(panel.HasAttribute("aria-label"));
    }

    [Fact]
    public void HeaderlessDialog_HasNoDanglingLabelledBy_UsesAriaLabel()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.AriaLabel, "Quick action")
            .AddChildContent("<p>Body</p>"));

        var panel = cut.Find("[role=dialog]");
        // No title -> must not point aria-labelledby at a non-existent element
        Assert.False(panel.HasAttribute("aria-labelledby"));
        Assert.Equal("Quick action", panel.GetAttribute("aria-label"));
    }

    [Fact]
    public void DefaultSize_HasMdClass()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Dialog.SizeMd}"));
    }
}

// ------------------------------------------------------------------------------
// FlareAlert  (7 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareAlertTests : FlareTestContext
{
    [Fact]
    public void RendersRootDiv()
    {
        var cut = Render<FlareAlert>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Alert.Root}"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareAlert>(p => p
            .AddChildContent("Alert body text"));

        Assert.Contains("Alert body text", cut.Find($".{Css.Classes.Alert.Body}").TextContent);
    }

    [Fact]
    public void RendersTitle()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.Title, "Important!"));

        Assert.Contains("Important!", cut.Find($".{Css.Classes.Alert.Title}").TextContent);
    }

    [Fact]
    public void SeverityInfo_HasInfoClass()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.Severity, AlertSeverity.Info));

        Assert.Contains(Css.Classes.Alert.Info, cut.Find($".{Css.Classes.Alert.Root}").ClassName);
    }

    [Fact]
    public void SeveritySuccess_HasSuccessClass()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.Severity, AlertSeverity.Success));

        Assert.Contains(Css.Classes.Alert.Success, cut.Find($".{Css.Classes.Alert.Root}").ClassName);
    }

    [Fact]
    public void SeverityWarning_HasWarningClass()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.Severity, AlertSeverity.Warning));

        Assert.Contains(Css.Classes.Alert.Warning, cut.Find($".{Css.Classes.Alert.Root}").ClassName);
    }

    [Fact]
    public void SeverityError_HasErrorClass()
    {
        var cut = Render<FlareAlert>(p => p
            .Add(x => x.Severity, AlertSeverity.Error));

        Assert.Contains(Css.Classes.Alert.Error, cut.Find($".{Css.Classes.Alert.Root}").ClassName);
    }
}

// ------------------------------------------------------------------------------
// FlareSnackbarProvider  (6 tests from Wave5)
// ------------------------------------------------------------------------------

public class C_FlareSnackbarProviderTests : FlareTestContext
{
    public C_FlareSnackbarProviderTests()
    {
        Services.AddSingleton<ISnackbarService, SnackbarService>();
    }

    [Fact]
    public void RendersProviderDiv()
    {
        var cut = Render<FlareSnackbarProvider>();

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Snackbar.Provider}"));
    }

    [Fact]
    public void NonErrorMessage_HasPoliteStatusRole()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Saved", SnackbarSeverity.Success);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count > 0);

        // Live semantics live on the toast, not the container: a stack mixes politeness levels.
        Assert.Equal("status", cut.Find($".{Css.Classes.Snackbar.Root}").GetAttribute("role"));
    }

    [Fact]
    public void ErrorMessage_HasAssertiveAlertRole()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Boom", SnackbarSeverity.Error);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Error}").Count > 0);

        // An error interrupts the screen reader; role="alert" is an assertive live region.
        Assert.Equal("alert", cut.Find($".{Css.Classes.Snackbar.Error}").GetAttribute("role"));
    }

    // The severity class used to be assembled from the enum name, and Normal has no accent rule at
    // all - so the neutral toast carried flare-snackbar--normal, a class the stylesheet never defines.
    [Fact]
    public void NormalSeverity_CarriesNoAccentClass()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Just so you know", SnackbarSeverity.Normal);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count > 0);

        Assert.DoesNotContain("flare-snackbar--normal", cut.Markup);
    }

    [Fact]
    public void NoMessagesInitially_NoSnackbarDivs()
    {
        var cut = Render<FlareSnackbarProvider>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Snackbar.Root}"));
    }

    [Fact]
    public void ShowMessage_SnackbarDivAppears()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Hello Snackbar");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count > 0);

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Snackbar.Root}"));
    }

    [Fact]
    public void ShowMessage_TextIsRendered()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Important message");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count > 0);

        Assert.Contains("Important message", cut.Markup);
    }

    [Fact]
    public void ShowErrorMessage_HasErrorClass()
    {
        var cut = Render<FlareSnackbarProvider>();
        var service = Services.GetRequiredService<ISnackbarService>();

        service.Show("Error occurred", SnackbarSeverity.Error);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Error}").Count > 0);

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Snackbar.Error}"));
    }
}

// ------------------------------------------------------------------------------
// FlareProgress  (6 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareProgressTests : FlareTestContext
{
    [Fact]
    public void RendersLinearRootElement()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Linear));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Progress.Root}"));
    }

    [Fact]
    public void LinearVariant_HasLinearClass()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Linear)
            .Add(x => x.Value, 50.0));

        Assert.Contains(Css.Classes.Progress.Linear, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }

    [Fact]
    public void CircularVariant_HasCircularClass()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Circular)
            .Add(x => x.Value, 50.0));

        Assert.Contains(Css.Classes.Progress.Circular, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }

    [Fact]
    public void IndeterminateMode_WhenValueIsNull()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Value, (double?)null));

        Assert.Contains(Css.Classes.Progress.Indeterminate, cut.Find($".{Css.Classes.Progress.Root}").ClassName);
    }

    [Fact]
    public void AriaValueNow_ReflectsValue()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Value, 75.0));

        Assert.Equal("75", cut.Find("[role='progressbar']").GetAttribute("aria-valuenow"));
    }

    [Fact]
    public void CircularVariant_RendersSvg()
    {
        var cut = Render<FlareProgress>(p => p
            .Add(x => x.Variant, ProgressVariant.Circular)
            .Add(x => x.Value, 50.0));

        Assert.NotEmpty(cut.FindAll($"svg.{Css.Classes.Progress.Svg}"));
    }
}

// ------------------------------------------------------------------------------
// FlareSkeleton  (6 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareSkeletonTests : FlareTestContext
{

    [Fact]
    public void VariantRect_HasRectClass()
    {
        var cut = Render<FlareSkeleton>(p => p
            .Add(x => x.Variant, SkeletonVariant.Rect));

        Assert.Contains(Css.Classes.Skeleton.Rect, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void VariantCircle_HasCircleClass()
    {
        var cut = Render<FlareSkeleton>(p => p
            .Add(x => x.Variant, SkeletonVariant.Circle));

        Assert.Contains(Css.Classes.Skeleton.Circle, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void VariantText_HasTextClass()
    {
        var cut = Render<FlareSkeleton>(p => p
            .Add(x => x.Variant, SkeletonVariant.Text));

        Assert.Contains(Css.Classes.Skeleton.Text, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void AnimationWave_HasWaveClass()
    {
        var cut = Render<FlareSkeleton>(p => p
            .Add(x => x.Animation, SkeletonAnimation.Wave));

        Assert.Contains(Css.Classes.Skeleton.Wave, cut.Find($".{Css.Classes.Skeleton.Root}").ClassName);
    }

    [Fact]
    public void WidthAndHeightAppliedAsStyle()
    {
        var cut = Render<FlareSkeleton>(p => p
            .Add(x => x.Width, "200px")
            .Add(x => x.Height, "50px"));

        var style = cut.Find($".{Css.Classes.Skeleton.Root}").GetAttribute("style") ?? string.Empty;
        Assert.Contains("200px", style);
        Assert.Contains("50px", style);
    }
}

// ------------------------------------------------------------------------------
// FlareOverlay  (7 tests from Wave5)
// ------------------------------------------------------------------------------

public class C_FlareOverlayTests : FlareTestContext
{
    [Fact]
    public void HiddenWhenOpenFalse()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, false));

        Assert.Empty(cut.FindAll($".{Css.Classes.Overlay.Root}"));
    }

    [Fact]
    public void RendersWhenOpenTrue()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Overlay.Root}"));
    }

    [Fact]
    public void RendersChildContent_WhenOpen()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, true)
            .AddChildContent("<span id=\"overlay-child\">Content</span>"));

        Assert.NotEmpty(cut.FindAll("#overlay-child"));
    }

    [Fact]
    public void ZIndex_AppliedInStyle()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.ZIndex, 9999));

        var style = cut.Find($".{Css.Classes.Overlay.Root}").GetAttribute("style") ?? "";
        Assert.Contains("9999", style);
    }

    [Fact]
    public void Opacity_AppliedInStyle()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Opacity, 0.75));

        var style = cut.Find($".{Css.Classes.Overlay.Root}").GetAttribute("style") ?? "";
        Assert.Contains("0.75", style);
    }

    [Fact]
    public void Absolute_HasAbsoluteClass()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Absolute, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Overlay.Absolute}"));
    }

    [Fact]
    public void NotAbsolute_NoAbsoluteClass()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Absolute, false));

        Assert.Empty(cut.FindAll($".{Css.Classes.Overlay.Absolute}"));
    }
}

// ------------------------------------------------------------------------------
// FlarePopover  (7 tests from Wave5)
// ------------------------------------------------------------------------------

public class C_FlarePopoverTests : FlareTestContext
{
    [Fact]
    public void RendersAnchorElement()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, false));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Popover.Anchor}"));
    }

    [Fact]
    public void PopoverPaperHiddenWhenOpenFalse()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, false));

        Assert.Empty(cut.FindAll($".{Css.Classes.Popover.Paper}"));
    }

    [Fact]
    public void PopoverPaperRenderedWhenOpenTrue()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Popover.Paper}"));
    }

    [Fact]
    public void RendersChildContent_WhenOpen()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, true)
            .AddChildContent("<span id=\"pop-child\">Pop Content</span>"));

        Assert.NotEmpty(cut.FindAll("#pop-child"));
    }

    [Fact]
    public void RendersAnchorContent()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, false)
            .Add(x => x.AnchorContent, b =>
                b.AddMarkupContent(0, "<button id=\"trigger-btn\">Open</button>")));

        Assert.NotEmpty(cut.FindAll("#trigger-btn"));
    }

    [Fact]
    public void PlacementBottomStart_HasBottomStartClass()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Placement, PopoverPlacement.BottomStart));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Popover.PaperBottomStart}"));
    }

    [Fact]
    public void PlacementTop_HasTopClass()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Placement, PopoverPlacement.Top));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Popover.PaperTop}"));
    }
}

// ------------------------------------------------------------------------------
// FlareTooltip  (5 tests from Wave3)
// ------------------------------------------------------------------------------

public class C_FlareTooltipTests : FlareTestContext
{
    [Fact]
    public void RendersRootElement()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "Tooltip text"));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Tooltip.Root}"));
    }

    [Fact]
    public void RendersTooltipContent()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "Helpful tip"));

        Assert.Contains("Helpful tip", cut.Find($".{Css.Classes.Tooltip.Content}").TextContent);
    }

    [Fact]
    public void PlacementTop_HasTopClass()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "tip")
            .Add(x => x.Placement, TooltipPlacement.Top));

        Assert.Contains(Css.Classes.Tooltip.Top, cut.Find($".{Css.Classes.Tooltip.Root}").ClassName);
    }

    [Fact]
    public void PlacementBottom_HasBottomClass()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "tip")
            .Add(x => x.Placement, TooltipPlacement.Bottom));

        Assert.Contains(Css.Classes.Tooltip.Bottom, cut.Find($".{Css.Classes.Tooltip.Root}").ClassName);
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "tip")
            .AddChildContent("<button class=\"trigger-btn\">Hover me</button>"));

        Assert.NotEmpty(cut.FindAll(".trigger-btn"));
    }
}

// ------------------------------------------------------------------------------
// FlareEmptyState  (6 tests from Wave1)
// ------------------------------------------------------------------------------

public class C_FlareEmptyStateTests : FlareTestContext
{
    [Fact]
    public void RendersIcon()
    {
        var cut = Render<FlareEmptyState>(p => p
            .Add(x => x.IconContent, b => b.AddMarkupContent(0, "<span class=\"test-icon\">★</span>")));

        Assert.NotNull(cut.Find($".{Css.Classes.Empty.StateIcon}"));
        Assert.NotEmpty(cut.FindAll(".test-icon"));
    }

    [Fact]
    public void RendersTitle()
    {
        var cut = Render<FlareEmptyState>(p => p
            .Add(x => x.Title, "Nothing here"));

        var title = cut.Find($".{Css.Classes.Empty.StateTitle}");
        Assert.Equal("Nothing here", title.TextContent);
    }

    [Fact]
    public void RendersDescription()
    {
        var cut = Render<FlareEmptyState>(p => p
            .Add(x => x.Description, "Try adjusting your filters."));

        var desc = cut.Find($".{Css.Classes.Empty.StateDescription}");
        Assert.Equal("Try adjusting your filters.", desc.TextContent);
    }

    [Fact]
    public void RendersActionContent()
    {
        var cut = Render<FlareEmptyState>(p => p
            .Add(x => x.ActionContent,
                b => b.AddMarkupContent(0, "<button class=\"action-btn\">Retry</button>")));

        Assert.NotNull(cut.Find($".{Css.Classes.Empty.StateAction}"));
        Assert.NotEmpty(cut.FindAll(".action-btn"));
    }

    [Fact]
    public void RendersMinimal_TitleOnly()
    {
        var cut = Render<FlareEmptyState>(p => p
            .Add(x => x.Title, "Empty"));

        Assert.NotNull(cut.Find($".{Css.Classes.Empty.StateTitle}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Empty.StateIcon}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Empty.StateDescription}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Empty.StateAction}"));
    }

    [Fact]
    public void RendersWithAllSlots()
    {
        var cut = Render<FlareEmptyState>(p => p
            .Add(x => x.IconContent, b => b.AddMarkupContent(0, "<span>icon</span>"))
            .Add(x => x.Title, "No Results")
            .Add(x => x.Description, "Clear your search to see results.")
            .Add(x => x.ActionContent, b => b.AddMarkupContent(0, "<button>Clear</button>")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Empty.StateIcon}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Empty.StateTitle}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Empty.StateDescription}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Empty.StateAction}"));
    }
}

// ------------------------------------------------------------------------------
// FlareConfirmDialogProvider  (8 tests from Wave6)
// ------------------------------------------------------------------------------

public class C_FlareConfirmDialogProviderTests : FlareTestContext
{
    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareConfirmDialogProvider>(p => p
            .AddChildContent("<span id=\"child-of-confirm\">Content</span>"));

        Assert.NotEmpty(cut.FindAll("#child-of-confirm"));
    }

    [Fact]
    public void DialogNotVisibleInitially()
    {
        var cut = Render<FlareConfirmDialogProvider>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Confirmdialog.Backdrop}"));
    }

    [Fact]
    public void ConfirmButtonNotVisibleInitially()
    {
        var cut = Render<FlareConfirmDialogProvider>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Confirmdialog.BtnConfirm}"));
    }

    [Fact]
    public void CancelButtonNotVisibleInitially()
    {
        var cut = Render<FlareConfirmDialogProvider>();

        Assert.Empty(cut.FindAll($".{Css.Classes.Confirmdialog.BtnCancel}"));
    }

    [Fact]
    public void AfterConfirmAsync_DialogVisible()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var provider = cut.Instance;

        _ = provider.ConfirmAsync("Delete?", "Are you sure?");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Confirmdialog.Backdrop}").Count > 0);

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Confirmdialog.Backdrop}"));
    }

    [Fact]
    public void AfterConfirmAsync_TitleRendered()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var provider = cut.Instance;

        _ = provider.ConfirmAsync("Delete item", "Cannot be undone.");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Confirmdialog.Title}").Count > 0);

        Assert.Contains("Delete item", cut.Find($".{Css.Classes.Confirmdialog.Title}").TextContent);
    }

    [Fact]
    public void AfterConfirmAsync_MessageRendered()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var provider = cut.Instance;

        _ = provider.ConfirmAsync("Title", "Please confirm this action.");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Confirmdialog.Body}").Count > 0);

        Assert.Contains("Please confirm this action.", cut.Find($".{Css.Classes.Confirmdialog.Body}").TextContent);
    }

    [Fact]
    public void ClickConfirm_DialogDismisses()
    {
        var cut = Render<FlareConfirmDialogProvider>();
        var provider = cut.Instance;

        _ = provider.ConfirmAsync("Sure?", "Yes or No");
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Confirmdialog.BtnConfirm}").Count > 0);

        cut.Find($".{Css.Classes.Confirmdialog.BtnConfirm}").Click();

        Assert.Empty(cut.FindAll($".{Css.Classes.Confirmdialog.Backdrop}"));
    }
}
