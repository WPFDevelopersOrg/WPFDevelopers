# wd:RingLoading

> 分类：[加载动画](../index.md#loading-animations) · 可通过 WPFDevelopers NuGet 包使用。

环形加载动画

## 示例源码

[RingLoadingExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/RingLoadingExample.xaml)

## XAML 用法

```xml
<wd:RingLoading
                Width="400"
                Height="400"
                Description="WPFDevelopers"
                Foreground="Black"
                IsLoading="{Binding ElementName=MyCheckBox, Path=IsChecked}"
                ProgressValue="50" />
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
