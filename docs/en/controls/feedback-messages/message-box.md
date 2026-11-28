# MessageBox (static class)

> Category: [Notifications & Messages](../index.md#feedback-messages) · Available in the WPFDevelopers NuGet package.

Message dialog

## Example Source

[MessageBoxExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MessageBoxExample.xaml)

## Detailed Usage

`MessageBox` is a static message dialog class provided by WPFDevelopers, designed as a drop-in replacement for `System.Windows.MessageBox` with a more polished and customizable appearance.

> **Note**: `MessageBox` is a **static class**, not a XAML control. It can only be called from C# code.

### Import the Namespace

To avoid conflicts with `System.Windows.MessageBox`, use an alias:

```csharp
using MessageBox = WPFDevelopers.Controls.MessageBox;
```

### Method Signatures

`MessageBox.Show()` provides 5 overloads:

```csharp
// 1. Message text only (default OK button, no icon)
MessageBoxResult Show(string messageBoxText, Window owner = null, bool isDefault = true)

// 2. Message text + caption
MessageBoxResult Show(string messageBoxText, string caption, Window owner = null, bool isDefault = true)

// 3. Message text + caption + buttons
MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, Window owner = null, bool isDefault = true)

// 4. Message text + caption + icon
MessageBoxResult Show(string messageBoxText, string caption, MessageBoxImage icon, Window owner = null, bool isDefault = true)

// 5. Message text + caption + buttons + icon (full signature)
MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, Window owner = null, bool isDefault = true)
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `messageBoxText` | string | — | Message content |
| `caption` | string | — | Dialog title |
| `button` | MessageBoxButton | OK | Button set: `OK` / `OKCancel` / `YesNo` / `YesNoCancel` |
| `icon` | MessageBoxImage | None | Icon type: `Information` / `Warning` / `Error` / `Question` |
| `owner` | Window | null | Parent window. When provided, the dialog centers on the owner with an overlay mask |
| `isDefault` | bool | true | Whether the first button is the default button (triggered by Enter key) |

### Icon and Color Mapping

| MessageBoxImage | Icon | Color |
|-----------------|------|-------|
| `Information` | Info icon | Success (green) |
| `Warning` | Warning icon | Warning (orange) |
| `Error` | Error icon | Danger (red) |
| `Question` | Question icon | Primary (blue) |

### Usage Examples

### 1. Information Dialog

```csharp
// File deleted successfully
MessageBox.Show("File deleted successfully.", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
```

### 2. Warning Dialog

```csharp
// Uses default OK button
MessageBox.Show("Performing this action may cause the file to become inaccessible!", "Warning", MessageBoxImage.Warning);
```

### 3. Error Dialog

```csharp
MessageBox.Show("The file does not exist.", "Error", MessageBoxImage.Error);
```

### 4. Confirmation Dialog (with Result Handling)

```csharp
var result = MessageBox.Show("The file does not exist. Continue?", "Confirm", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

switch (result)
{
    case MessageBoxResult.Yes:
        // User clicked "Yes"
        break;
    case MessageBoxResult.No:
        // User clicked "No"
        break;
    case MessageBoxResult.Cancel:
        // User clicked "Cancel" or closed the dialog
        break;
}
```

### 5. With Owner Window (with Overlay Mask)

```csharp
// When owner is passed, the dialog centers on the parent window and shows an overlay mask
MessageBox.Show("Operation successful!", "Info", MessageBoxButton.OK, MessageBoxImage.Information, owner: this);
```

### Interaction Behavior

- **Close methods**: Click the close button (top-right), press `Escape`, or click any button
- **Owner window**: When `owner` is provided, the dialog centers on the parent window and displays a mask overlay. Without an owner, it auto-detects the current window or centers on screen
- **Button text**: Automatically localized via `LanguageManager` (follows system language)

---

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

