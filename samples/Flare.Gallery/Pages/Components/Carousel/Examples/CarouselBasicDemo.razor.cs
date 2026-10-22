using Flare.Gallery.Resources;

namespace Flare.Gallery.Pages.Components.Carousel.Examples;

public partial class CarouselBasicDemo
{
    private readonly List<SlideData> _slides =
    [
        new(GalleryStrings.Carousel_SlideDashboard, "var(--flare-color-primary-container)", "bar_chart"),
        new(GalleryStrings.Carousel_SlideUsers, "var(--flare-color-secondary-container)", "people"),
        new(GalleryStrings.Carousel_SlideSettings, "var(--flare-color-tertiary-container)", "settings"),
        new(GalleryStrings.Carousel_SlideNotifications, "var(--flare-color-error-container)", "notifications"),
    ];
}