# SongWords

> Category: [Animation & Effects](../index.md#animation-effects) · Sample-only: this implementation is not included in the NuGet control package.

Lyrics scroll animation

## Example Source

[SongWordsExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SongWordsExample.xaml)

## XAML Usage

```xml
<controls:SongWords
                                Margin="0,4"
                                Block.TextAlignment="Center"
                                FontSize="15"
                                StartDuration="{Binding StarTime}"
                                Text="{Binding SongWords}"
                                Duration="{Binding RunTime}" />
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

