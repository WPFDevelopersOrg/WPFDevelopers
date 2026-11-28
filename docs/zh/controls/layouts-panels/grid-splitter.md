# GridSplitter

> 分类：[布局与面板](../index.md#layouts-panels) · 这是 WPF 标准控件；完成快速开始中的主题配置后即可使用 WD 样式。

使用 WD 主题样式的标准 WPF 网格分隔条，支持拖动调整行列尺寸。

## 示例源码

[GridSplitterExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/GridSplitterExample.xaml)

## 详细用法

`GridSplitter` 是标准 WPF 控件，不需要 `wd:` 前缀。完成上述主题配置后即可使用默认样式：背景透明、居中显示三个圆点，鼠标进入时隐藏圆点并显示主题色分隔条，移出后恢复。

> 三点手柄样式需要使用包含此改动的控件库版本；较早的 NuGet 包尚未包含该样式。

### 垂直分隔条：左右分栏

使用独立的 `Auto` 列放置分隔条，设置 `Width` 和 `ResizeDirection="Columns"`：

```xml
<Grid Height="180">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" MinWidth="60" />
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="*" MinWidth="60" />
    </Grid.ColumnDefinitions>
    <Border Background="{DynamicResource WD.PrimaryMouseOverBrush}">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="左侧" />
    </Border>
    <GridSplitter
        Grid.Column="1"
        Width="5"
        Margin="2,0"
        HorizontalAlignment="Stretch"
        VerticalAlignment="Stretch"
        ResizeDirection="Columns"
        ResizeBehavior="PreviousAndNext"
        ShowsPreview="True" />
    <Border Grid.Column="2" Background="{DynamicResource WD.PrimaryMouseOverBrush}">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="右侧" />
    </Border>
</Grid>
```

### 水平分隔条：上下分栏

使用独立的 `Auto` 行放置分隔条，设置 `Height` 和 `ResizeDirection="Rows"`；此时三个圆点会旋转为水平排列：

```xml
<Grid Height="240">
    <Grid.RowDefinitions>
        <RowDefinition Height="*" MinHeight="40" />
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" MinHeight="40" />
    </Grid.RowDefinitions>
    <Border Background="{DynamicResource WD.PrimaryMouseOverBrush}">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="上方" />
    </Border>
    <GridSplitter
        Grid.Row="1"
        Height="5"
        Margin="0,2"
        HorizontalAlignment="Stretch"
        VerticalAlignment="Stretch"
        ResizeDirection="Rows"
        ResizeBehavior="PreviousAndNext"
        ShowsPreview="False" />
    <Border Grid.Row="2" Background="{DynamicResource WD.PrimaryMouseOverBrush}">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="下方" />
    </Border>
</Grid>
```

配置要点：

- 显式设置 `ResizeDirection`：`Columns` 调整左右列宽，`Rows` 调整上下行高。当前手柄样式通过 `Rows` 触发器旋转三点，不会根据 `Auto` 的实际拖动方向自动旋转。
- `ResizeBehavior="PreviousAndNext"` 调整分隔条两侧的行或列，适合上述独立行/列布局。
- `ShowsPreview="True"` 拖动时显示预览，松开后应用尺寸；`False` 则实时调整。预览模式需要所在的可视树中存在 `AdornerLayer`，标准 WPF 窗口通常已提供。
- 为内容行/列设置 `MinWidth` / `MinHeight`，避免拖动后内容区过小。
- `Margin="2,0"` 表示左右各留 2 DIP；`Margin="0,2"` 表示上下各留 2 DIP。相邻容器的外边距会叠加，建议统一在分隔条上配置间距。透明背景仍保留拖动命中区域。
- `Foreground` 控制圆点颜色；悬停背景使用主题资源 `WD.PrimaryBrush`。

交互示例位于示例程序的 **基础控件 → GridSplitter（网格分隔条）**，源码参见 [GridSplitterExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/GridSplitterExample.xaml)。

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
