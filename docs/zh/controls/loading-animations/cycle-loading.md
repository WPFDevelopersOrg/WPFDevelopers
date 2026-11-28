# wd:CycleLoading

> 分类：[加载动画](../index.md#loading-animations) · 可通过 WPFDevelopers NuGet 包使用。

循环加载动画

## 示例源码

[CycleLoadingExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/CycleLoadingExample.xaml)

## XAML 用法

```xml
<wd:CycleLoading
                Width="200"
                IsLoading="{Binding ElementName=MyCheckBox, Path=IsChecked}"
                LoadTitle="Loading...."
                Value="50">
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
