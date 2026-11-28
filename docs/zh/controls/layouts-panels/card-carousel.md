# wd:CardCarousel

> 分类：[布局与面板](../index.md#layouts-panels) · 可通过 WPFDevelopers NuGet 包使用。

高级轮播图（多层叠加动画）

## 示例源码

[CardCarouselExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CardCarouselExample.xaml)

## 详细用法

多层叠加动画效果的轮播图，支持自动播放和间隔控制：

```xml
<wd:CardCarousel AutoPlay="True" AutoPlayInterval="0:0:5">
    <Image Source="pack://application:,,,/Images/photo1.jpg" />
    <Image Source="pack://application:,,,/Images/photo2.jpg" />
    <Image Source="pack://application:,,,/Images/photo3.jpg" />
</wd:CardCarousel>
```

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `AutoPlay` | bool | false | 是否自动播放 |
| `AutoPlayInterval` | TimeSpan | 0:0:3 | 播放间隔（TimeSpan） |

---

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
