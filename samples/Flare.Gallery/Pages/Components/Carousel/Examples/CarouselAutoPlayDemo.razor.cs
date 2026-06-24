namespace Flare.Gallery.Pages.Components.Carousel.Examples;

public partial class CarouselAutoPlayDemo
{
    private readonly List<SlideData> _slides =
    [
        new("Dashboard Analytics", "var(--flare-color-primary-container)", "bar_chart"),
        new("User Management", "var(--flare-color-secondary-container)", "people"),
        new("Settings & Config", "var(--flare-color-tertiary-container)", "settings"),
        new("Notifications", "var(--flare-color-error-container)", "notifications"),
    ];
}