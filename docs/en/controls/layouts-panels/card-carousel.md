# wd:CardCarousel

> Category: [Layout & Panels](../index.md#layouts-panels) · Available in the WPFDevelopers NuGet package.

Master carousel (multi-layer overlay animation)

## Example Source

[CardCarouselExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CardCarouselExample.xaml)

## Detailed Usage

A carousel with multi-layer overlay animation and auto-play/interval control:

```xml
<wd:CardCarousel AutoPlay="True" AutoPlayInterval="0:0:5">
    <Image Source="pack://application:,,,/Images/photo1.jpg" />
    <Image Source="pack://application:,,,/Images/photo2.jpg" />
    <Image Source="pack://application:,,,/Images/photo3.jpg" />
</wd:CardCarousel>
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AutoPlay` | bool | false | Enable auto-play |
| `AutoPlayInterval` | TimeSpan | 0:0:3 | Playback interval (TimeSpan) |

---

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

