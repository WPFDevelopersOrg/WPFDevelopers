# GridSplitter

> Category: [Layout & Panels](../index.md#layouts-panels) · A standard WPF control; the linked sample demonstrates the WD theme style.

The standard WPF grid splitter using the WD theme style to resize rows or columns.

## Example Source

[GridSplitterExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/GridSplitterExample.xaml)

## Detailed Usage

`GridSplitter` is a standard WPF control and does not require the `wd:` prefix. After configuring the theme above, its default style has a transparent background and a centered three-dot grip. Hovering hides the dots and highlights the splitter with the theme color; leaving restores the grip.

> The three-point joystick style requires the use of a control library version that includes this modification; The earlier NuGet packages did not include this style

### Vertical Splitter: Side-by-Side Panes

Place the splitter in a dedicated `Auto` column and set `Width` and `ResizeDirection="Columns"`:

```xml
<Grid Height="180">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" MinWidth="60" />
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="*" MinWidth="60" />
    </Grid.ColumnDefinitions>
    <Border Background="{DynamicResource WD.PrimaryMouseOverBrush}">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="Left" />
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
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="Right" />
    </Border>
</Grid>
```

### Horizontal Splitter: Stacked Panes

Place the splitter in a dedicated `Auto` row and set `Height` and `ResizeDirection="Rows"`. The three-dot grip rotates to a horizontal arrangement:

```xml
<Grid Height="240">
    <Grid.RowDefinitions>
        <RowDefinition Height="*" MinHeight="40" />
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" MinHeight="40" />
    </Grid.RowDefinitions>
    <Border Background="{DynamicResource WD.PrimaryMouseOverBrush}">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="Top" />
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
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="Bottom" />
    </Border>
</Grid>
```

Configuration notes:

- Set `ResizeDirection` explicitly: `Columns` resizes adjacent columns; `Rows` resizes adjacent rows. The current grip style rotates through the `Rows` trigger, not through the effective direction inferred by `Auto`.
- `ResizeBehavior="PreviousAndNext"` resizes the rows or columns on either side of the splitter, matching the dedicated row/column layouts above.
- `ShowsPreview="True"` previews the resize and applies it on release; `False` resizes immediately. Preview mode requires an `AdornerLayer` in the visual tree, which standard WPF windows normally provide.
- Set `MinWidth` / `MinHeight` on content columns/rows to prevent panes from becoming too small.
- `Margin="2,0"` adds 2 DIPs on each side; `Margin="0,2"` adds 2 DIPs above and below. Margins on neighboring containers add to this spacing, so prefer configuring the gap on the splitter alone. The transparent background remains hit-testable for dragging.
- `Foreground` controls the dot color; the hover background uses the theme resource `WD.PrimaryBrush`.

Find the interactive demo under **Basic → GridSplitter** in the sample application. See [GridSplitterExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/GridSplitterExample.xaml) for the source.

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

