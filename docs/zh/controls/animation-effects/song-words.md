# SongWords

> 分类：[动画与特效](../index.md#animation-effects) · 该项目目前仅在示例程序中提供，不属于 NuGet 控件包。

歌词滚动动画

## 示例源码

[SongWordsExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SongWordsExample.xaml)

## XAML 用法

```xml
<controls:SongWords
                                Margin="0,4"
                                Block.TextAlignment="Center"
                                FontSize="15"
                                StartDuration="{Binding StarTime}"
                                Text="{Binding SongWords}"
                                Duration="{Binding RunTime}" />
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
