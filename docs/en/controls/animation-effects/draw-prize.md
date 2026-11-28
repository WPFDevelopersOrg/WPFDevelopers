# DrawPrize

> Category: [Animation & Effects](../index.md#animation-effects) · Sample-only: this implementation is not included in the NuGet control package.

Lottery wheel

## Example Source

[DrawPrizeExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrawPrizeExample.xaml)

## XAML Usage

```xml
<controls:DrawPrize
            x:Name="PART_DrawPrize"
            ItemsSource="{Binding MenuArray, RelativeSource={RelativeSource AncestorType=local:DrawPrizeExample}}"
            ListAngle="{Binding ListAngle, RelativeSource={RelativeSource AncestorType=local:DrawPrizeExample}}" />
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

