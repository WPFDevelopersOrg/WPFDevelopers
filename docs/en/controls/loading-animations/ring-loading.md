# wd:RingLoading

> Category: [Loading Animations](../index.md#loading-animations) · Available in the WPFDevelopers NuGet package.

Ring loading animation

## Example Source

[RingLoadingExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/RingLoadingExample.xaml)

## XAML Usage

```xml
<wd:RingLoading
                Width="400"
                Height="400"
                Description="WPFDevelopers"
                Foreground="Black"
                IsLoading="{Binding ElementName=MyCheckBox, Path=IsChecked}"
                ProgressValue="50" />
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

