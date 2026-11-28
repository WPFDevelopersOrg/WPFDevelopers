# wd:IPEditBox

> Category: [Input Controls](../index.md#input-controls) · Available in the WPFDevelopers NuGet package.

IP address input

## Example Source

[IPEditBoxExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/IPEditBoxExample.xaml)

## XAML Usage

```xml
<wd:IPEditBox
                x:Name="myIPEditBox"
                wd:ElementHelper.CornerRadius="4"
                Text="{Binding ElementName=MyIp, Path=Text, Mode=TwoWay}" />
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

