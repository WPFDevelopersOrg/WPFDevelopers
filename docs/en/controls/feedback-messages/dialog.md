# Dialog

> Category: [Notifications & Messages](../index.md#feedback-messages) · Available in the WPFDevelopers NuGet package.

A general-purpose dialog for custom content, input validation, and MVVM workflows.

## Example Source

[DialogExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Dialog/DialogExample.xaml)

## Detailed Usage

`Dialog` displays custom forms or confirmation content using a `UserControl` or `Page`, with a mask, title, and confirm/cancel buttons.

### Opening and validating input

```csharp
var editor = new UserEditor(); // Your custom UserControl
var dialog = new WPFDevelopers.Controls.Dialog
{
    Title = "Edit user",
    Content = editor,
    Owner = this,
    DialogWidth = 520
};
dialog.Closing += (sender, e) =>
{
    if (e.Reason == WPFDevelopers.Controls.DialogCloseReason.Confirm)
        e.Cancel = !editor.Validate(); // Keep open when validation fails
};
if (await dialog.ShowAsync() == WPFDevelopers.Controls.DialogResult.Confirmed)
{
    Save(editor.DataContext);
}
```

Implement `Validate()` and `Save()` in your application. Validate input in `Closing` when confirming, set `e.Cancel = true` to keep the dialog open, and display an error in the content. WPF binding validation errors also prevent confirmation automatically.

### Common options

| Property/method | Default | Description |
|---|---|---|
| `Content` | null | Required; a `UserControl` or `Page` not placed in another container |
| `Title` | Empty string | Dialog title |
| `CornerRadius` | `10` | Corner radius; use `new CornerRadius(0)` for square corners |
| `Owner` | null | Parent window; finds the current window if omitted |
| `DialogWidth` / `DialogHeight` | `440` / `double.NaN` | Width and height; `double.NaN` sizes automatically |
| `ShowFooter` | true | Whether to show the footer buttons |
| `ConfirmText` / `CancelText` | null | Button labels; follow language settings by default |
| `IsConfirmEnabled` | true | Bindable confirmation button availability |
| `IsOpen` | false | Two-way open state; false requests cancellation |
| `ClosedCommand` | null | Runs after closure with a `DialogResult` parameter |
| `ShowAsync(owner = null)` | — | Opens the dialog and awaits `Confirmed` or `Canceled` |
| `Close(reason)` | `Cancel` | Requests closure, still running `Closing` validation |

- Call on the UI thread. Only one dialog per owner is supported; do not change `Content` while open.
- Cancel does not roll back ViewModel changes. Use a working copy if edits must be discarded.
- Custom window templates need an `AdornerDecorator`. For a draggable independent window, use a custom `Window` with `MaskShowDialog()`.

The example uses `await` (.NET 4.5+). On .NET 4.0 use the same API with a UI scheduler:

```csharp
dialog.ShowAsync().ContinueWith(task =>
{
    if (task.Result == WPFDevelopers.Controls.DialogResult.Confirmed)
        Save(editor.DataContext);
}, TaskScheduler.FromCurrentSynchronizationContext());
```

### MVVM

Declare the dialog in `Resources`, not directly in the layout. Assign the ViewModel explicitly (`DialogViewModel` below is a previously declared resource):

```xml
<wd:Dialog x:Key="EditorDialog"
           DataContext="{StaticResource DialogViewModel}"
           Title="Edit user"
           IsOpen="{Binding IsDialogOpen, Mode=TwoWay}"
           IsConfirmEnabled="{Binding CanConfirm}"
           ClosedCommand="{Binding DialogClosedCommand}">
    <UserControl>
        <TextBox Text="{Binding Name, UpdateSourceTrigger=PropertyChanged}" />
    </UserControl>
</wd:Dialog>
```

Reference the dialog's DataContext from the opening button:

```xml
<Button Content="Open"
        DataContext="{Binding DataContext, Source={StaticResource EditorDialog}}"
        Command="{Binding OpenDialogCommand}" />
```

`OpenDialogCommand` sets `IsDialogOpen` to true; `CanConfirm` controls the confirm button. After closure, `DialogClosedCommand` receives `DialogResult.Confirmed` or `Canceled`.

Full examples: [C# / MVVM](https://github.com/WPFDevelopersOrg/WPFDevelopers/tree/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Dialog).

---

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

