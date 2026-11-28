# wd:IPEditBox

> 分类：[输入控件](../index.md#input-controls) · 可通过 WPFDevelopers NuGet 包使用。

IP 地址输入框

## 示例源码

[IPEditBoxExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/IPEditBoxExample.xaml)

## XAML 用法

```xml
<wd:IPEditBox
                x:Name="myIPEditBox"
                wd:ElementHelper.CornerRadius="4"
                Text="{Binding ElementName=MyIp, Path=Text, Mode=TwoWay}" />
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
