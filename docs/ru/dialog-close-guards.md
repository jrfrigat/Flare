# Защита несохраненных изменений в диалогах

Поместите `FlareDialogCloseGuard` внутрь `FlareForm` или `EditForm` диалога. Он читает
`EditContext.IsModified()` и спрашивает перед отменой изменений. Неизмененная форма закрывается
сразу. Стандартное подтверждение использует локализованные строки Flare и требует
`FlareDialogProvider` в раскладке, в том числе для встроенного диалога.

Для полей Flare задайте и `@bind-Value`, и `For="@(() => model.Property)"`: `For` связывает
изменения с `EditContext` формы. Одна привязка значения не помечает форму измененной.

## Форма с сохранением и отменой

Это содержимое сервисного диалога редактирует рабочую копию. После успешного сохранения форма
помечается неизмененной, и диалог закрывается с результатом. Отмена оставляет исходные данные
вызывающего кода нетронутыми.

```razor
@using Microsoft.AspNetCore.Components.Forms

<FlareForm Model="_model" OnValidSubmit="Save">
    <FlareDialogCloseGuard />
    <FlareTextField @bind-Value="_model.Name" For="@(() => _model.Name)" Label="Name" />
    <FlareButton OnClick="Cancel">Cancel</FlareButton>
    <FlareButton Type="ButtonType.Submit">Save</FlareButton>
</FlareForm>

@code {
    [CascadingParameter] public FlareDialogInstance Dialog { get; set; } = default!;
    [Parameter] public string Name { get; set; } = "";
    private readonly Profile _model = new();

    protected override void OnInitialized() => _model.Name = Name;

    private async Task Save(EditContext context)
    {
        // Await persistence here, if needed. Keep the form dirty if saving fails.
        context.MarkAsUnmodified();
        await Dialog.CloseAsync(_model.Name);
    }

    private Task Cancel() => Dialog.CancelAsync();

    private sealed class Profile
    {
        public string Name { get; set; } = "";
    }
}
```

Откройте его через `DialogService.ShowAsync<ProfileDialog>("Edit profile", parameters)` и прочитайте
сохраненное имя через `result.GetData<string>()`, если `result.Cancelled` равно false. Нижняя панель
sheet использует те же содержимое и защиту. Страница диалогов Gallery показывает сервисные диалоги
и встроенную форму.

Для встроенного диалога оставьте `@bind-Visible` и добавьте `@ref`:

```razor
<FlareDialog @ref="_dialog" @bind-Visible="_visible" Title="Edit profile">
    @* Form and guard go here. Cancel calls _dialog.CloseAsync(). *@
</FlareDialog>

@code {
    private FlareDialog _dialog = default!;
    private bool _visible;
}
```

## Явное состояние и свое подтверждение

Вне формы задайте `HasUnsavedChanges`. Значение по умолчанию `null` читает контекст формы;
явное `false` отключает эту защиту, даже если форма изменена.

```razor
<FlareDialogCloseGuard HasUnsavedChanges="_dirty"
                       Title="Discard this draft?"
                       Message="Your draft has not been saved."
                       ConfirmLabel="Discard" CancelLabel="Keep editing" />
```

После сохранения обновите состояние и дождитесь передачи параметра защите перед запросом закрытия.
В случае `EditContext` вызов `MarkAsUnmodified()` действует сразу, в том числе внутри обработчика
сохранения.

`ConfirmClose` заменяет стандартное подтверждение. Он получает причину закрытия и возвращает
`true`, чтобы разрешить закрытие, или `false`, чтобы оставить диалог открытым. Обработчик вызывается
только при наличии изменений:

```razor
<FlareDialogCloseGuard HasUnsavedChanges="_dirty" ConfirmClose="ConfirmDiscard" />

@code {
    private async ValueTask<bool> ConfirmDiscard(DialogCloseReason reason) =>
        await DialogService.ConfirmAsync("Discard draft?", "Continue without saving?") == true;
}
```

Свое содержимое может получить `[CascadingParameter] DialogCloseContext` и зарегистрировать
`Func<DialogCloseReason, ValueTask<bool>>` через `RegisterGuard`. Сохраните полученный `IDisposable`
и освободите его при удалении содержимого. Экземпляр диалога предоставляет тот же контекст через
`FlareDialogInstance.CloseContext`. Контекст принимает одну попытку закрытия за раз: повторные
нажатия не создают несколько вопросов. Сначала выполняется `BeforeClose`, затем зарегистрированные
защиты; любое `false` отменяет попытку.

## Способы закрытия и миграция

`DialogCloseReason` различает `ScrimClick`, `EscapeKey`, `CloseButton`, `Programmatic` и `Navigation`.
Закрытие через интерфейс, `CloseAsync` диалога, сервисные дескрипторы и внутреннюю навигацию
использует один контекст. Фон закрывает окно, только если нажатие и отпускание произошли на одном
и том же фоне; перетаскивание из диалога на фон не запрашивает закрытие.

Замените `FlareDialogInstance.Close(...)` / `Cancel()` и `DialogReference.Close(...)` / `Cancel()`
на `await CloseAsync(...)` / `await CancelAsync()`. Они возвращают `Task<bool>`: `true` означает
принятое закрытие, `false` - отказ защиты, уже выполняющуюся попытку или уже закрытый диалог.
Побеждает первый принятый результат. Дождитесь операции, прежде чем считать диалог закрытым.

Прямое присваивание `Visible = false` принудительно меняет состояние и обходит защиты.
Для закрытия по действию пользователя вызывайте `CloseAsync()` встроенного диалога. У сервисного
диалога защищены и успешный результат, и отмена; после сохранения сбросьте признак изменений
перед запросом закрытия.

При `CloseOnNavigation = true` внутренняя навигация спрашивает защиты до смены URL. Если другой
обработчик отменит переход, диалог останется открыт. Перезагрузка, внешний URL и закрытие вкладки
не могут ждать диалог Blazor: добавьте `FlareNavigationGuard IsDirty="_dirty"
GuardInAppNavigation="false"` для системного вопроса браузера при уходе. Его формулировку выбирает
браузер. Задайте `CloseOnNavigation = false`, если внутренняя навигация должна оставлять диалог
открытым.
