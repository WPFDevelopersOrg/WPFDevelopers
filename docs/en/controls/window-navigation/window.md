# wd:Window

> Category: [Windows & Navigation](../index.md#window-navigation) · Available in the WPFDevelopers NuGet package.

Custom window (ToolWindow / NoneTitleBar / Normal / HighTitleBar)

## Example Source

[MainWindow.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MainWindow.xaml)

## Detailed Usage

Set `ShowTheme="True"` to display the window theme menu (hidden by default), offering Follow system / Light / Dark in `Normal` and `High` title bars:

```xml
<wd:Window ShowTheme="True" TitleBarMode="High">
    <!-- Window content -->
</wd:Window>
```

Use `Theme` to select a mode: `System` follows the system, `Light` uses light mode, and `Dark` uses dark mode.

```xml
<wd:Resources Color="DarkSlateGray" Theme="System" />
```

Omitting `Theme` restores the previous choice, or follows the system on first use. Setting it explicitly uses the specified mode.

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

