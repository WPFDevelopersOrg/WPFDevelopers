# ListView

> Category: [Basic Controls & Styles](../index.md#basic-controls) · A standard WPF control; the linked sample demonstrates the WD theme style.

List view (GridView)

## Example Source

[ListViewExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/ListViewExample.xaml)

## XAML Usage

```xml
<ListView
                    wd:ElementHelper.CornerRadius="3"
                    wd:ScrollViewerBehavior.IsBackToTopEnabled="True"
                    FontSize="20"
                    ItemsSource="{Binding UserCollection, RelativeSource={RelativeSource AncestorType=UserControl}}">
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

