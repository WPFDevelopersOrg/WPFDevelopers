# wd:AnimationGrid

> Category: [Animation & Effects](../index.md#animation-effects) · Available in the WPFDevelopers NuGet package.

Animation grid

## Example Source

[AnimationGridExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/AnimationGridExample.xaml)

## XAML Usage

```xml
<wd:AnimationGrid
                x:Name="MyPanel"
                ItemTemplate="{StaticResource GridItemTemplate}"
                ItemsSource="{Binding GridItems, RelativeSource={RelativeSource AncestorType=UserControl}}" />
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

