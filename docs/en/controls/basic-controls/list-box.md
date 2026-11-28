# ListBox

> Category: [Basic Controls & Styles](../index.md#basic-controls) · A standard WPF control; the linked sample demonstrates the WD theme style.

List box

## Example Source

[ListBoxExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/ListBoxExample.xaml)

## XAML Usage

```xml
<ListBox
                    wd:ElementHelper.CornerRadius="3"
                    wd:ScrollViewerBehavior.IsBackToTopEnabled="True"
                    DisplayMemberPath="Name"
                    FontSize="20"
                    ItemsSource="{Binding UserCollection, RelativeSource={RelativeSource AncestorType=UserControl}}" />
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

