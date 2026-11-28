# wd:GrayscaleEffect

> Category: [Effects & Filters](../index.md#effects-filters) · Available in the WPFDevelopers NuGet package.

Grayscale filter effect (adjustable intensity)

## Example Source

[GrayscaleEffectExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/GrayscaleEffectExample.xaml)

## Detailed Usage

`GrayscaleEffect` is a pixel shader effect that converts any visual element to grayscale display, commonly used for global grayscale mode (e.g., memorial days).

### Apply to a Single Control

```xml
<Image Source="photo.jpg">
    <Image.Effect>
        <wd:GrayscaleEffect Factor="1" />
    </Image.Effect>
</Image>
```

### Apply to the Entire Window

```xml
<wd:Window>
    <wd:Window.Effect>
        <wd:GrayscaleEffect x:Name="grayscaleEffect" Factor="0" />
    </wd:Window.Effect>
</wd:Window>
```

```csharp
// Enable grayscale
var animation = new DoubleAnimation
{
    To = 1,
    Duration = TimeSpan.FromMilliseconds(1000),
    EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut }
};
grayscaleEffect.BeginAnimation(GrayscaleEffect.FactorProperty, animation);

// Disable grayscale
animation.To = 0;
grayscaleEffect.BeginAnimation(GrayscaleEffect.FactorProperty, animation);
```

### Key Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Factor` | double | 0 | Grayscale intensity, 0 = original color, 1 = full grayscale |
| `Brightness` | double | 0 | Brightness adjustment |

---

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

