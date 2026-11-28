# TreeView

> Category: [Basic Controls & Styles](../index.md#basic-controls) · A standard WPF control; the linked sample demonstrates the WD theme style.

Tree view (scroll animation)

## Example Source

[TreeViewExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/TreeViewExample.xaml)

## XAML Usage

```xml
<TreeView
                    wd:ElementHelper.CornerRadius="3"
                    wd:TreeViewHelper.IsScrollAnimation="true"
                    FontSize="20"
                    ItemsSource="{Binding UserCollection, RelativeSource={RelativeSource AncestorType=UserControl}}">
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

