# wd:CircleProgressBar

> 分类：[图表与数据可视化](../index.md#charts-data-visualization) · 可通过 WPFDevelopers NuGet 包使用。

环形进度条

## 示例源码

[CircleProgressBarExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CircleProgressBarExample.xaml)

## XAML 用法

```xml
<wd:CircleProgressBar
                Width="80"
                Height="80"
                Background="#21BA9D"
                BorderBrush="LightGray"
                BrushStrokeThickness="2"
                StrokeThickness="5"
                Value="{Binding ElementName=CircleSlider, Path=Value}" />
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
