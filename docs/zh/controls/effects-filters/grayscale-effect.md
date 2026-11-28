# wd:GrayscaleEffect

> 分类：[特效与滤镜](../index.md#effects-filters) · 可通过 WPFDevelopers NuGet 包使用。

灰度滤镜效果（可控制灰度强度）

## 示例源码

[GrayscaleEffectExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/GrayscaleEffectExample.xaml)

## 详细用法

`GrayscaleEffect` 是一个像素着色器效果，可以将任何视觉元素转换为灰度显示，常用于全局灰度模式（如纪念日）。

### 应用于单个控件

```xml
<Image Source="photo.jpg">
    <Image.Effect>
        <wd:GrayscaleEffect Factor="1" />
    </Image.Effect>
</Image>
```

### 应用于整个窗口

```xml
<wd:Window>
    <wd:Window.Effect>
        <wd:GrayscaleEffect x:Name="grayscaleEffect" Factor="0" />
    </wd:Window.Effect>
</wd:Window>
```

```csharp
// 开启灰度
var animation = new DoubleAnimation
{
    To = 1,
    Duration = TimeSpan.FromMilliseconds(1000),
    EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut }
};
grayscaleEffect.BeginAnimation(GrayscaleEffect.FactorProperty, animation);

// 关闭灰度
animation.To = 0;
grayscaleEffect.BeginAnimation(GrayscaleEffect.FactorProperty, animation);
```

### 主要属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Factor` | double | 0 | 灰度强度，0 = 原始彩色，1 = 完全灰度 |
| `Brightness` | double | 0 | 亮度调节 |

---

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
