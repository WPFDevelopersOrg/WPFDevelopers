# StreamerLoading

> Category: [Loading Animations](../index.md#loading-animations) · Sample-only: this implementation is not included in the NuGet control package.

Streamer animation

## Example Source

[StreamerLoadingExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/StreamerLoadingExample.xaml)

## XAML Usage

```xml
<controls:StreamerLoading Visibility="{Binding ElementName=MyCheckBox, Path=IsChecked, Converter={StaticResource WD.Bool2VisibilityConverter}}">
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

