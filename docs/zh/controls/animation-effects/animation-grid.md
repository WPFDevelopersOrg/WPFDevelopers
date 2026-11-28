# wd:AnimationGrid

> 分类：[动画与特效](../index.md#animation-effects) · 可通过 WPFDevelopers NuGet 包使用。

动画网格

## 示例源码

[AnimationGridExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/AnimationGridExample.xaml)

## XAML 用法

```xml
<wd:AnimationGrid
                x:Name="MyPanel"
                ItemTemplate="{StaticResource GridItemTemplate}"
                ItemsSource="{Binding GridItems, RelativeSource={RelativeSource AncestorType=UserControl}}" />
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
