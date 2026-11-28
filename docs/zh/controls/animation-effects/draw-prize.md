# DrawPrize

> 分类：[动画与特效](../index.md#animation-effects) · 该项目目前仅在示例程序中提供，不属于 NuGet 控件包。

大转盘抽奖

## 示例源码

[DrawPrizeExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrawPrizeExample.xaml)

## XAML 用法

```xml
<controls:DrawPrize
            x:Name="PART_DrawPrize"
            ItemsSource="{Binding MenuArray, RelativeSource={RelativeSource AncestorType=local:DrawPrizeExample}}"
            ListAngle="{Binding ListAngle, RelativeSource={RelativeSource AncestorType=local:DrawPrizeExample}}" />
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
