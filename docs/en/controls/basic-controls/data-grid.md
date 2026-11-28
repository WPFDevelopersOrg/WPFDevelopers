# DataGrid

> Category: [Basic Controls & Styles](../index.md#basic-controls) · A standard WPF control; the linked sample demonstrates the WD theme style.

Data grid (row header selection / template columns / grid lines)

## Example Source

[DataGridExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/DataGridExample.xaml)

## XAML Usage

```xml
<DataGrid
                        wd:ElementHelper.CornerRadius="3"
                        wd:ScrollViewerBehavior.IsBackToTopEnabled="True"
                        AutoGenerateColumns="False"
                        FontSize="20"
                        HeadersVisibility="All"
                        ItemsSource="{Binding UserCollection, RelativeSource={RelativeSource AncestorType=UserControl}}"
                        RowHeaderWidth="40">
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

