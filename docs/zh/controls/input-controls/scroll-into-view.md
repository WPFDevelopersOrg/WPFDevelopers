# wd:ScrollIntoView

> 分类：[输入控件](../index.md#input-controls) · 可通过 WPFDevelopers NuGet 包使用。

列表定位辅助功能

## 示例源码

[SelectorExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SelectorExample.xaml)

## XAML 用法

```xml
<DataGrid ItemsSource="{Binding ListArrays,RelativeSource={RelativeSource AncestorType=UserControl}}"
                      SelectedItem="{Binding SelectItem,RelativeSource={RelativeSource AncestorType=UserControl}}"
                      wd:ScrollIntoView.IsPosition="true"
                      Margin="4" IsReadOnly="True">
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
