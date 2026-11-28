# wd:ScrollIntoView

> Category: [Input Controls](../index.md#input-controls) · Available in the WPFDevelopers NuGet package.

List positioning helper

## Example Source

[SelectorExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SelectorExample.xaml)

## XAML Usage

```xml
<DataGrid ItemsSource="{Binding ListArrays,RelativeSource={RelativeSource AncestorType=UserControl}}"
                      SelectedItem="{Binding SelectItem,RelativeSource={RelativeSource AncestorType=UserControl}}"
                      wd:ScrollIntoView.IsPosition="true"
                      Margin="4" IsReadOnly="True">
```

## Related Pages

- [Control Catalog](../index.md)
- [Getting Started](../../getting-started.md)

