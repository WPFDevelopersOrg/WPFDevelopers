# wd:Gauge

> 分类：[图表与数据可视化](../index.md#charts-data-visualization) · 可通过 WPFDevelopers NuGet 包使用。

仪表盘

## 示例源码

[GaugeExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/GaugeExample.xaml)

## XAML 用法

```xml
<wd:Gauge
                    Title="Min"
                    Width="100"
                    Height="100"
                    Margin="10"
                    HorizontalAlignment="Center"
                    VerticalAlignment="Center"
                    Background="Black"
                    BorderBrush="Red"
                    FontSize="8"
                    Maximum="90"
                    Minimum="30"
                    Thickness="3"
                    ValueFormat="{}{0:0}值"
                    Value="{Binding ElementName=MySlider2, Path=Value}" />
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
