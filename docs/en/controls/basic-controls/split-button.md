# wd:SplitButton

> Category: [Basic Controls & Styles](../index.md#basic-controls) · Available in the WPFDevelopers NuGet package.

Split button (main button + dropdown menu, supports text/icon options)

## Example Source

[SplitButtonExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SplitButtonExample.xaml)

## Detailed Usage

`SplitButton` is a split button control provided by WPFDevelopers. The left side is a clickable main button area, and the right side is a dropdown toggle button. It supports both text options and rich content options with icons.

### Basic Usage (ItemsSource Data Binding)

```xml
<wd:SplitButton
    Width="150"
    Content="File"
    ItemsSource="{Binding MenuItems}"
    SelectionChanged="SplitButton_SelectionChanged" />
```

```csharp
// ViewModel or Code-behind
public ObservableCollection<string> MenuItems { get; } = new ObservableCollection<string>
{
    "Save in PDF",
    "Save in Word",
    "Save in Excel",
    "Save in Image"
};

private void SplitButton_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
{
    // e.NewValue is the selected item
}
```

### Default Style Variants (Border + Background)

```xml
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}" />
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonSuccess}" />
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonWarning}" />
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonDanger}" />
```

### Primary Style Variants (Solid Background)

```xml
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonPrimary}" />
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonSuccessPrimary}" />
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonWarningPrimary}" />
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonDangerPrimary}" />
```

### XAML Children (Supports MenuItem with Icons)

```xml
<wd:SplitButton
    Width="150"
    Content="File"
    ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonSuccess}">
    <MenuItem Header="Copy">
        <MenuItem.Icon>
            <wd:PathIcon Kind="Copy" />
        </MenuItem.Icon>
    </MenuItem>
    <MenuItem Header="Paste" />
    <MenuItem Header="Cut" />
</wd:SplitButton>
```

### Key Properties

| Property | Type | Default | Description |
|------|------|--------|------|
| `Content` | object | null | Main button display content |
| `ItemsSource` | object | null | Dropdown menu data source (IEnumerable) |
| `IsDropDownOpen` | bool | false | Whether dropdown is open (two-way binding) |
| `ContextMenuStyle` | Style | null | Custom ContextMenu style |

### Events

| Event | Description |
|------|------|
| `Click` | Fired when the main button area is clicked (clicking ToggleButton does NOT fire this) |
| `SelectionChanged` | Fired when a dropdown menu item is selected |

### Available Styles

| Style Key | Description |
|--------|------|
| `WD.SplitButton` (default) | Border + transparent background, border turns primary color on hover |
| `WD.SplitButtonDefault` | Border + background, full hover/pressed/focused feedback with primary color |
| `WD.SplitButtonPrimary` | No border + solid primary color background + white text, opacity 0.9 on hover |
| `WD.SplitButtonSuccess` | Border + background, full hover/pressed/focused feedback with success color |
| `WD.SplitButtonSuccessPrimary` | No border + solid success background + white text |
| `WD.SplitButtonWarning` | Border + background, full hover/pressed/focused feedback with warning color |
| `WD.SplitButtonWarningPrimary` | No border + solid warning background + white text |
| `WD.SplitButtonDanger` | Border + background, full hover/pressed/focused feedback with danger color |
| `WD.SplitButtonDangerPrimary` | No border + solid danger background + white text |

### Interaction Behavior

| Action | Behavior |
|------|------|
| Click main button area | Fires `Click` event |
| Click ToggleButton | Expands/collapses dropdown menu |
| Click menu item | Button Content updates to selected item, fires `SelectionChanged` |
| Click outside menu | Automatically closes dropdown |

---

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

