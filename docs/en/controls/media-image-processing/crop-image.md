# wd:CropImage

> Category: [Media & Image Processing](../index.md#media-image-processing) · Available in the WPFDevelopers NuGet package.

Image cropping

## Example Source

[CropImageExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CropImageExample.xaml)

## XAML Usage

```xml
<wd:CropImage
                Name="MyCropImage"
                Grid.Row="1"
                IsRatioScale="{Binding ElementName=myCheckBox, Path=IsChecked}"
                RectScale="{Binding ElementName=myRectScaleSlider, Path=Value}"
                ScaleSize="2.35,1" />
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

