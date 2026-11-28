# wd:WaterfallItemsControl

> Category: [Layout & Panels](../index.md#layouts-panels) · Available in the WPFDevelopers NuGet package.

Masonry items control with a scrolling template

## Example Source

[WaterfallPanelExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/WaterfallPanelExample.xaml)

## XAML Usage

```xml
<wd:WaterfallItemsControl
                    Margin="16"
                    ItemsSource="{Binding Items, Source={StaticResource WaterfallPanelViewModel}}"
                    MinColumnWidth="200"
                    Spacing="16">
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

