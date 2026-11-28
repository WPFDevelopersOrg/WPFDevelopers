# wd:DrawerMenu

> Category: [Windows & Navigation](../index.md#window-navigation) · Available in the WPFDevelopers NuGet package.

Win10-style drawer navigation

## Example Source

[DrawerMenuExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrawerMenu/DrawerMenuExample.xaml)

## Detailed Usage

`DrawerMenu.ShowExpandButton` defaults to `True`. Set it to `False` to hide the built-in expand/collapse button and collapse the menu to zero width. Control `IsExpanded` from an external control:

```xml
<ToggleButton x:Name="MenuToggle" Content="Menu" />
<wd:DrawerMenu ShowExpandButton="False"
               IsExpanded="{Binding IsChecked, ElementName=MenuToggle, Mode=TwoWay}" />
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

