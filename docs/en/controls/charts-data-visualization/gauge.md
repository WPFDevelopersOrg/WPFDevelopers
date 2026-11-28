# wd:Gauge

> Category: [Charts & Data Visualization](../index.md#charts-data-visualization) · Available in the WPFDevelopers NuGet package.

Gauge control

## Example Source

[GaugeExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/GaugeExample.xaml)

## XAML Usage

```xml
<wd:Gauge
                    Title="Min"
                    Width="100"
                    Height="100"
                    Margin="10"
                    HorizontalAlignment="Center"
                    VerticalAlignment="Center"
                    Background="Black"
                    BorderBrush="Red"
                    FontSize="8"
                    Maximum="90"
                    Minimum="30"
                    Thickness="3"
                    ValueFormat="{}{0:0}值"
                    Value="{Binding ElementName=MySlider2, Path=Value}" />
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

