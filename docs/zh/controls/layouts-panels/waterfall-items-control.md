# wd:WaterfallItemsControl

> 分类：[布局与面板](../index.md#layouts-panels) · 可通过 WPFDevelopers NuGet 包使用。

内置滚动模板的瀑布流集合控件

## 示例源码

[WaterfallPanelExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/WaterfallPanelExample.xaml)

## XAML 用法

```xml
<wd:WaterfallItemsControl
                    Margin="16"
                    ItemsSource="{Binding Items, Source={StaticResource WaterfallPanelViewModel}}"
                    MinColumnWidth="200"
                    Spacing="16">
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
