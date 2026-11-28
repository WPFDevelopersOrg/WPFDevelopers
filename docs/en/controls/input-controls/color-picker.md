# wd:ColorPicker

> Category: [Input Controls](../index.md#input-controls) · Available in the WPFDevelopers NuGet package.

Color picker

## Example Source

[ColorPickerExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ColorPickerExample.xaml)

## Detailed Usage

`ColorPicker` remains an inline panel. Its format dropdown selects RGB, HSL, or HEX directly; set `ColorType` to choose the initial format.

```xml
<wd:ColorPicker SelectedColor="{Binding AccentColor}" ColorType="HEX" />
```

Set `DisplayMode="Popup"` on the same `ColorPicker` to show a clickable swatch that opens the panel. The default, `DisplayMode="Inline"`, preserves the existing inline display:

```xml
<wd:ColorPicker DisplayMode="Popup" SelectedColor="{Binding AccentColor}" />
<wd:ColorPicker DisplayMode="Popup" SelectedColor="{Binding AccentColor}"
                     ShowColorText="True"
                     SwatchSize="24"
                     ColorType="HEX" />
```

- `SelectedColor` is a `Color` and binds two-way by default. Changes are applied immediately, without confirmation.
- `ShowColorText` defaults to `False`. Enable it to display HEX beside the swatch: `#RRGGBB` for opaque colors, or `#AARRGGBB` otherwise.
- `SwatchSize` is the swatch side length, defaulting to 18. Use positive values such as 24 or 32; trigger dimensions also include padding and borders.
- `IsDropDownOpen` supports two-way binding. Click outside or press Esc to close; F4 or Alt+Down toggles the popup. Space opens it when the trigger has focus.
- Mouse activation opens the panel on release so the popup can capture outside clicks. Outside-click dismissal remains available after changing formats.
- `ShowColorText`, `SwatchSize`, and `IsDropDownOpen` apply to Popup mode; Inline mode cannot open a popup. Switching `DisplayMode` at runtime closes the popup and preserves the color and format.
- `SelectedColorChanged` is a bubbling color-change event. Both modes share the color panel and theme resources. In Popup mode, `Width`, `Height`, and `Padding` configure the trigger, not the inner panel.

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

