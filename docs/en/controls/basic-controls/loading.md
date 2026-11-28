# Loading

> Category: [Basic Controls & Styles](../index.md#basic-controls) · A standard WPF control; the linked sample demonstrates the WD theme style.

Loading states (button loading/loading task)

## Example Source

[LoadingBasicExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/LoadingBasicExample.xaml)

## XAML Usage

```xml
<Button Name="btnLoading" wd:Loading.LoadingType="Normal" Click="BtnLoading_Click"
                        Content="AddLoading" Style="{DynamicResource WD.WarningPrimaryButton}" />
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

