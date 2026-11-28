# StreamerLoading

> 分类：[加载动画](../index.md#loading-animations) · 该项目目前仅在示例程序中提供，不属于 NuGet 控件包。

流光加载动画

## 示例源码

[StreamerLoadingExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/StreamerLoadingExample.xaml)

## XAML 用法

```xml
<controls:StreamerLoading Visibility="{Binding ElementName=MyCheckBox, Path=IsChecked, Converter={StaticResource WD.Bool2VisibilityConverter}}">
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
