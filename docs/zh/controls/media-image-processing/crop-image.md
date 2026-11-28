# wd:CropImage

> 分类：[媒体与图像处理](../index.md#media-image-processing) · 可通过 WPFDevelopers NuGet 包使用。

图片裁剪

## 示例源码

[CropImageExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CropImageExample.xaml)

## XAML 用法

```xml
<wd:CropImage
                Name="MyCropImage"
                Grid.Row="1"
                IsRatioScale="{Binding ElementName=myCheckBox, Path=IsChecked}"
                RectScale="{Binding ElementName=myRectScaleSlider, Path=Value}"
                ScaleSize="2.35,1" />
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
