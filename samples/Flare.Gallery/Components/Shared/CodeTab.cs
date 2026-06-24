namespace Flare.Gallery.Components.Shared;

public class CodeTab
{
    public string Title { get; set; } = string.Empty; // Название вкладки (Razor, C#, CSS, JSON)
    public string Language { get; set; } = string.Empty; // Класс для подсветки (razor, csharp, css)
    public string Code { get; set; } = string.Empty; // Сам текст кода
}
