using Flare.Abstractions;

namespace Flare.Infrastructure;

internal sealed class NullThemeStorage : IThemeStorageService
{
    public Task<ThemeSelection?> GetAsync() => Task.FromResult<ThemeSelection?>(null);
    public Task SaveAsync(ThemeSelection selection) => Task.CompletedTask;
}
