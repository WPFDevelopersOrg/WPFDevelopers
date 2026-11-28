# wd:CycleLoading

> Category: [Loading Animations](../index.md#loading-animations) · Available in the WPFDevelopers NuGet package.

Cycle animation

## Example Source

[CycleLoadingExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/CycleLoadingExample.xaml)

## XAML Usage

```xml
<wd:CycleLoading
                Width="200"
                IsLoading="{Binding ElementName=MyCheckBox, Path=IsChecked}"
                LoadTitle="Loading...."
                Value="50">
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

