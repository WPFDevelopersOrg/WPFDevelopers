# wd:WaterfallPanel

> 分类：[布局与面板](../index.md#layouts-panels) · 可通过 WPFDevelopers NuGet 包使用。

瀑布流面板

## 示例源码

[WaterfallPanelExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/WaterfallPanelExample.xaml)

## 详细用法

瀑布流滚动复用项目位置缓存，并通过列内二分查找选择视口附近的项目；
宽度、数据或已实现项目的测量高度变化时才重建位置。
`WDScrollViewer` 保留平滑滚动，连续同向滚轮输入累计目标位置，反向输入从当前位置开始，
动画时长为 250ms，不再固定限制为 40 FPS。实际流畅度取决于项目模板和设备性能。
这些优化只使用 .NET Framework 4.0 已有 API；Win7 应选择兼容的 .NET Framework 构建，
不能因此认为现代 .NET 目标也支持 Win7。

数据绑定场景推荐使用 `WaterfallItemsControl`，不需要再套外层 `ScrollViewer`：

```xml
<wd:WaterfallItemsControl MinColumnWidth="200" Spacing="16" ItemsSource="{Binding Items}">
    <wd:WaterfallItemsControl.ItemTemplate>
        <DataTemplate>
            <Image Source="{Binding ImageUri}" Stretch="Uniform" />
        </DataTemplate>
    </wd:WaterfallItemsControl.ItemTemplate>
</wd:WaterfallItemsControl>
```

它继承 `ItemsControl` 的 `ItemsSource`、`ItemTemplate`、`ItemTemplateSelector`，
默认启用容器回收，并提供 `Columns`、`MinColumnWidth`、`Spacing`、`EstimatedItemHeight`。
通过 `ScrollViewer.VerticalScrollBarVisibility` 控制滚动条，仍需由父布局提供有限高度才能虚拟化。
直接 XAML 子项或自行配置宿主时继续使用 `WaterfallPanel`。

`Columns` 默认为 3；`MinColumnWidth` 默认为 0，保留固定列数。
设置 `MinColumnWidth="200"` 后，列数按 `floor((可用宽度 + Spacing) / (MinColumnWidth + Spacing))`
计算，最少一列。容器不足最小列宽时，单列会缩小到容器宽度；`ActualColumns` 可读取当前列数。
无限宽度下使用 `Columns`，按子项最大自然宽度（且不小于 `MinColumnWidth`）计算列宽。

同一个 `WaterfallPanel` 支持直接 XAML 子元素和 `ItemsControl.ItemsSource`。
真正的 UI 虚拟化需要有限视口，并由 `ScrollViewer` 通过 `CanContentScroll="True"`
将滚动交给面板。例如：

```xml
<ItemsControl ItemsSource="{Binding Items}"
              VirtualizingStackPanel.IsVirtualizing="True"
              VirtualizingStackPanel.VirtualizationMode="Recycling">
    <ItemsControl.Template>
        <ControlTemplate TargetType="ItemsControl">
            <ScrollViewer CanContentScroll="True"
                          HorizontalScrollBarVisibility="Disabled"
                          VerticalScrollBarVisibility="Auto">
                <ItemsPresenter />
            </ScrollViewer>
        </ControlTemplate>
    </ItemsControl.Template>
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <wd:WaterfallPanel MinColumnWidth="200" Spacing="16"
                               EstimatedItemHeight="210" ClipToBounds="True" />
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
    <!-- 通过 ItemTemplate 定义每个数据项的内容。 -->
</ItemsControl>
```

不要将该虚拟化宿主放入垂直 `StackPanel` 或另一个提供无限高度的外层滚动容器；
缺少有限视口或滚动协作时会退回全部容器布局。`IsVirtualizing="False"` 也会生成全部容器。
未测量项使用 `EstimatedItemHeight`（默认 200），测得真实高度后会修正布局与滚动范围，
因此远距离跳转、图片异步加载时位置可能变化。异步内容应预留合理高度。
这是容器虚拟化，不是数据虚拟化：布局元数据仍随项目数增长，布局计算为 O(项目数 × 列数)。
示例包含 XAML、MVVM 和 5000 个不同高度卡片的虚拟化演示。

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
