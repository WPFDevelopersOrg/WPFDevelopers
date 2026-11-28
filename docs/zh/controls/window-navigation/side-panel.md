# wd:SidePanel

> 分类：[窗口与导航](../index.md#window-navigation) · 可通过 WPFDevelopers NuGet 包使用。

可放置自定义内容的可折叠侧栏

## 示例源码

[SidePanelExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SidePanelExample.xaml)

## 使用方法

`Dock` 可设置为 `Left`、`Right`、`Top` 或 `Bottom`，默认是 `Left`。左右方向使用 `ExpandedWidth`（默认 220），上下方向使用 `ExpandedHeight`（默认 220）。`IsExpanded` 默认是 `True`。

`SidePanel` 通过改变尺寸折叠，并非覆盖在主内容上的弹出层。使用 `DockPanel` 时，需要同时设置 `DockPanel.Dock` 来指定面板的位置；其他布局容器也需要为面板预留可自动调整的空间。避免在折叠方向设置固定的 `Width` 或 `Height`、非零最小尺寸或外边距，否则可能无法完全收起。

```xml
<StackPanel>
    <ToggleButton x:Name="SidePanelToggle" Content="底部面板" IsChecked="True" />
    <DockPanel Height="300">
        <wd:SidePanel Dock="Bottom" DockPanel.Dock="Bottom"
                      ExpandedHeight="100"
                      IsExpanded="{Binding IsChecked, ElementName=SidePanelToggle}">
            <TextBlock Text="底部面板" />
        </wd:SidePanel>
        <TextBlock Text="主内容" />
    </DockPanel>
</StackPanel>
```

示例的“方向”选项卡默认收起所有面板；选择方向可展开对应面板，选择“收起”可关闭面板。四个方向的完整示例和 MVVM 用法见示例源码。

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
