# wd:Badge

> 分类：[提示与消息](../index.md#feedback-messages) · 可通过 WPFDevelopers NuGet 包使用。

角标通知

## 示例源码

[BadgeExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BadgeExample.xaml)

## XAML 用法

```xml
<Button
                    wd:Badge.IsShow="{Binding ElementName=MyBadgeToggleButton, Path=IsChecked}"
                    wd:Badge.Text="new"
                    Content="Default"
                    Style="{DynamicResource WD.PrimaryButton}" />
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
