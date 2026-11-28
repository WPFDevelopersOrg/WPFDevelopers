# wd:SidePanel

> Category: [Windows & Navigation](../index.md#window-navigation) · Available in the WPFDevelopers NuGet package.

Collapsible sidebar with custom content

## Example Source

[SidePanelExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SidePanelExample.xaml)

## Detailed Usage

`SidePanel` hosts arbitrary content. `Dock` can be `Left` (the default), `Right`, `Top`, or `Bottom`. Left and right use `ExpandedWidth`; top and bottom use `ExpandedHeight`. Both default to 220, and `IsExpanded` defaults to `True`.

The panel collapses by changing its size; it is not an overlay. Place it in an automatically sized row or column so the main content can fill the space. With a `DockPanel`, also set `DockPanel.Dock` to position it. Avoid a fixed size, a nonzero minimum size, or a margin along the collapse axis.

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>
    <ToggleButton x:Name="SideToggle" Grid.ColumnSpan="2" Content="Sidebar" IsChecked="True" />
    <wd:SidePanel Grid.Row="1" ExpandedWidth="220"
                  IsExpanded="{Binding IsChecked, ElementName=SideToggle}">
        <StackPanel>
            <TextBlock Text="Sidebar content" />
            <Button Content="New task" />
        </StackPanel>
    </wd:SidePanel>
    <TextBlock Grid.Row="1" Grid.Column="1" Text="Main content" />
</Grid>
```

For MVVM, bind `IsExpanded` two-way to a view model and use a command to toggle it. See [SidePanelExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SidePanelExample.xaml).

In the example's Directions tab, all panels start collapsed. Select a direction to open its panel, or select Collapse to close it.

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

