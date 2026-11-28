# ToggleButton

> Category: [Basic Controls & Styles](../index.md#basic-controls) · A standard WPF control; the linked sample demonstrates the WD theme style.

Toggle button

## Example Source

[ToggleButtonExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/ToggleButtonExample.xaml)

## XAML Usage

```xml
<ToggleButton Content="IsOpen" />
```

For a capsule-shaped switch with a diagonal divider, use `SwitchButton`:

[SwitchButtonExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SwitchButtonExample.xaml)

```xml
<wd:SwitchButton UncheckedContent="OFF"
                 CheckedContent="ON"
                 IsChecked="{Binding IsEnabled, Mode=TwoWay}"
                 Style="{StaticResource WD.DiagonalSwitchButton}" />
```

Keep the width at least as large as the height for semicircular ends. `Foreground`
sets the selected side's brush and `Background` sets the other side's brush.
The default brushes are `WD.PrimaryBrush` and `WD.BaseBrush`, respectively,
so the inactive background and content follow light and dark themes.
The selected region slides between sides when the switch changes state.
`UncheckedContent` appears in the center of the left half, and `CheckedContent`
appears in the center of the right half.
When unchecked, the left OFF side is highlighted; when checked, the right ON
side is highlighted.
Both properties accept content elements such as `Path`, `wd:PathIcon`, or `Image`,
not only strings. Set the element's size explicitly.
Text, `Path`, and `wd:PathIcon` are white on the selected side and use
`WD.PrimaryTextBrush` on the other side. Explicit `Fill` or `Foreground` values
override the default colors. Images retain their original colors.
For SVG, supply a rendered image source or use an SVG-capable element.
The default size follows the content, with `WD.Padding` applied to each half.
Override `Padding` to adjust spacing, or set `Width` and `Height` explicitly.

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

