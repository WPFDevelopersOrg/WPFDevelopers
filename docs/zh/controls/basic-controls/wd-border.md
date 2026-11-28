# wd:WDBorder

> 分类：[基础控件与样式](../index.md#basic-controls) · 可通过 WPFDevelopers NuGet 包使用。

`WDBorder` 继承自 WPF `Border`，额外提供只读的 `ContentClip` 几何，用于将子元素裁剪到边框内部的圆角区域。控件本身不会自动裁剪内容，需要将 `ContentClip` 绑定到需要裁剪的子元素的 `Clip` 属性。

## 示例源码

- [TreeView 样式中的使用方式](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Net40/Styles/Styles.TreeView.xaml)
- [WDBorder 实现](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Shared/Controls/BaseControls/WDBorder.cs)

## 详细用法

声明 WPFDevelopers 命名空间，并将 `ContentClip` 绑定到内容元素：

```xml
<wd:WDBorder
    xmlns:wd="https://github.com/WPFDevelopersOrg/WPFDevelopers"
    Width="220"
    Height="140"
    Background="#E6F4FF"
    BorderBrush="#1677FF"
    BorderThickness="1"
    CornerRadius="16">
    <Grid Clip="{Binding Path=ContentClip, RelativeSource={RelativeSource AncestorType=wd:WDBorder}}">
        <TextBlock
            HorizontalAlignment="Center"
            VerticalAlignment="Center"
            Text="圆角区域内的内容" />
    </Grid>
</wd:WDBorder>
```

### 主要属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `ContentClip` | `Geometry` | 只读裁剪几何，根据控件尺寸、`CornerRadius` 和 `BorderThickness` 计算 |
| `CornerRadius` | `CornerRadius` | 从 `Border` 继承，决定圆角大小 |
| `BorderThickness` | `Thickness` | 从 `Border` 继承，决定边框厚度及内容裁剪区域的内缩量 |

`ContentClip` 主要用于自定义控件模板：将它绑定到内部 `Grid`、`Image` 或滚动内容的 `Clip` 属性，即可避免子内容超出圆角边缘。

---

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
