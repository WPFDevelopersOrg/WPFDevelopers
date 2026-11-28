# wd:SplitButton

> 分类：[基础控件与样式](../index.md#basic-controls) · 可通过 WPFDevelopers NuGet 包使用。

分割按钮（主按钮+下拉菜单，支持文本/图标选项）

## 示例源码

[SplitButtonExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SplitButtonExample.xaml)

## 详细用法

`SplitButton` 是 WPFDevelopers 提供的分割按钮控件，左侧为可点击的主按钮区域，右侧为下拉菜单切换按钮，支持文本选项和带图标的富内容选项。

### 基础用法（ItemsSource 数据绑定）

```xml
<wd:SplitButton
    Width="150"
    Content="File"
    ItemsSource="{Binding MenuItems}"
    SelectionChanged="SplitButton_SelectionChanged" />
```

```csharp
// ViewModel 或 Code-behind
public ObservableCollection<string> MenuItems { get; } = new ObservableCollection<string>
{
    "Save in PDF",
    "Save in Word",
    "Save in Excel",
    "Save in Image"
};

private void SplitButton_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
{
    // e.NewValue 为选中的项
}
```

### Default 样式变体（边框 + 背景）

```xml
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}" />
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonSuccess}" />
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonWarning}" />
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonDanger}" />
```

### Primary 样式变体（实心背景）

```xml
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonPrimary}" />
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonSuccessPrimary}" />
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonWarningPrimary}" />
<wd:SplitButton Width="120" Content="File" ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonDangerPrimary}" />
```

### XAML 子元素（支持 MenuItem 带图标）

```xml
<wd:SplitButton
    Width="150"
    Content="File"
    ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonSuccess}">
    <MenuItem Header="Copy">
        <MenuItem.Icon>
            <wd:PathIcon Kind="Copy" />
        </MenuItem.Icon>
    </MenuItem>
    <MenuItem Header="Paste" />
    <MenuItem Header="Cut" />
</wd:SplitButton>
```

### 主要属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Content` | object | null | 主按钮显示内容 |
| `ItemsSource` | object | null | 下拉菜单数据源（IEnumerable） |
| `IsDropDownOpen` | bool | false | 下拉菜单是否展开（双向绑定） |
| `ContextMenuStyle` | Style | null | ContextMenu 自定义样式 |

### 事件

| 事件 | 说明 |
|------|------|
| `Click` | 点击主按钮区域时触发（点击 ToggleButton 不会触发） |
| `SelectionChanged` | 选择下拉菜单项时触发 |

### 可用样式

| 样式键 | 说明 |
|--------|------|
| `WD.SplitButton`（默认） | 边框 + 透明背景，鼠标悬停时边框变主题色 |
| `WD.SplitButtonDefault` | 边框 + 背景，完整的悬停/按下/聚焦主题色反馈 |
| `WD.SplitButtonPrimary` | 无边框 + 主色实心背景 + 白色文字，悬停时透明度 0.9 |
| `WD.SplitButtonSuccess` | 边框 + 背景，完整的悬停/按下/聚焦成功色反馈 |
| `WD.SplitButtonSuccessPrimary` | 无边框 + 成功色实心背景 + 白色文字 |
| `WD.SplitButtonWarning` | 边框 + 背景，完整的悬停/按下/聚焦警告色反馈 |
| `WD.SplitButtonWarningPrimary` | 无边框 + 警告色实心背景 + 白色文字 |
| `WD.SplitButtonDanger` | 边框 + 背景，完整的悬停/按下/聚焦危险色反馈 |
| `WD.SplitButtonDangerPrimary` | 无边框 + 危险色实心背景 + 白色文字 |

### 交互行为

| 操作 | 行为 |
|------|------|
| 点击主按钮区域 | 触发 `Click` 事件 |
| 点击右侧 ToggleButton | 展开/收起下拉菜单 |
| 点击菜单项 | 按钮 Content 更新为选中项，触发 `SelectionChanged` |
| 点击菜单外部区域 | 自动关闭下拉菜单 |

---

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
