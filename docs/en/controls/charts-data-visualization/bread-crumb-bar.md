# wd:BreadCrumbBar

> Category: [Charts & Data Visualization](../index.md#charts-data-visualization) · Available in the WPFDevelopers NuGet package.

Breadcrumb navigation

## Example Source

[BreadCrumbBarExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BreadCrumbBarExample.xaml)

## XAML Usage

```xml
<wd:BreadCrumbBar
                DisplayMemberPath="Text"
                ItemsSource="{Binding BreadcrumbItems, RelativeSource={RelativeSource AncestorType=UserControl}}"
                SelectedItem="{Binding BreadcrumbItem, RelativeSource={RelativeSource AncestorType=UserControl}}"
                SelectionChanged="BreadCrumbBar_SelectionChanged" />
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

