# wd:Mask

> 分类：[窗口与导航](../index.md#window-navigation) · 可通过 WPFDevelopers NuGet 包使用。

遮罩层（窗体级/控件级）

## 示例源码

[MaskExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MaskExample.xaml)

## XAML 用法

```xml
<Border
                Height="200"
                Margin="10"
                wd:Mask.IsShow="{Binding ElementName=ToggleButtonMask, Path=IsChecked}"
                Background="LawnGreen">
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
