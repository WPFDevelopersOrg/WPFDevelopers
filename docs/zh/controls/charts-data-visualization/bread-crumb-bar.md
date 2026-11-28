# wd:BreadCrumbBar

> 分类：[图表与数据可视化](../index.md#charts-data-visualization) · 可通过 WPFDevelopers NuGet 包使用。

面包屑导航栏

## 示例源码

[BreadCrumbBarExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BreadCrumbBarExample.xaml)

## XAML 用法

```xml
<wd:BreadCrumbBar
                DisplayMemberPath="Text"
                ItemsSource="{Binding BreadcrumbItems, RelativeSource={RelativeSource AncestorType=UserControl}}"
                SelectedItem="{Binding BreadcrumbItem, RelativeSource={RelativeSource AncestorType=UserControl}}"
                SelectionChanged="BreadCrumbBar_SelectionChanged" />
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
