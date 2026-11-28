# wd:WaterfallPanel

> Category: [Layout & Panels](../index.md#layouts-panels) · Available in the WPFDevelopers NuGet package.

Waterfall / masonry panel

## Example Source

[WaterfallPanelExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/WaterfallPanelExample.xaml)

## Detailed Usage

Scrolling reuses cached item positions and uses per-column binary searches to select nearby items.
Positions are rebuilt when width, data, or measured realized-item heights change.
`WDScrollViewer` retains smooth scrolling: consecutive wheel input accumulates its target,
while direction changes restart from the current offset. Animations last 250ms without a fixed
40 FPS cap. Actual smoothness depends on item templates and hardware.
These changes use APIs available in .NET Framework 4.0. Windows 7 requires a compatible
.NET Framework build; this does not imply Windows 7 support for modern .NET targets.

For data binding, use `WaterfallItemsControl` without an outer `ScrollViewer`:

```xml
<wd:WaterfallItemsControl MinColumnWidth="200" Spacing="16" ItemsSource="{Binding Items}">
    <wd:WaterfallItemsControl.ItemTemplate>
        <DataTemplate>
            <Image Source="{Binding ImageUri}" Stretch="Uniform" />
        </DataTemplate>
    </wd:WaterfallItemsControl.ItemTemplate>
</wd:WaterfallItemsControl>
```

It inherits `ItemsSource`, `ItemTemplate`, and `ItemTemplateSelector`, enables recycling by default,
and exposes `Columns`, `MinColumnWidth`, `Spacing`, and `EstimatedItemHeight`.
Use `ScrollViewer.VerticalScrollBarVisibility` to control the scrollbar.
A finite host height is still required for virtualization. Use `WaterfallPanel` for direct XAML
children or custom hosts.

`Columns` defaults to 3. `MinColumnWidth` defaults to 0 (fixed columns).
A positive `MinColumnWidth` selects `max(1, floor((availableWidth + Spacing) / (MinColumnWidth + Spacing)))`
columns. A narrow container still has one column, which may be narrower than the minimum.
`ActualColumns` reports the selected count. With infinite available width, `Columns` is used,
and the widest natural item width (at least `MinColumnWidth`) determines the column width.

The same panel supports direct XAML children and `ItemsControl.ItemsSource`.
For UI virtualization, give the host a finite viewport and use an `ItemsControl` template containing
`ScrollViewer CanContentScroll="True"` with an `ItemsPresenter` as its content.
Set `VirtualizingStackPanel.IsVirtualizing="True"` and `VirtualizingStackPanel.VirtualizationMode="Recycling"`
on the `ItemsControl`, and use `WaterfallPanel` as its `ItemsPanel`.
Do not wrap this host in a vertical `StackPanel` or an outer scroller providing infinite height.
Without a finite scrolling viewport the panel generates all containers; setting `IsVirtualizing="False"`
also generates all containers.

Unmeasured items use `EstimatedItemHeight` (default 200). Actual measurements update positions and
the scroll extent, so distant jumps and asynchronously loaded images may cause layout shifts.
Reserve a suitable initial height for asynchronous content.
This is UI, not data, virtualization: metadata storage is O(item count), and layout calculation is
O(item count × columns). The example includes XAML, MVVM, and a 5000-card virtualization tab.

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

