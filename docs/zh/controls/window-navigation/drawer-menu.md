# wd:DrawerMenu

> 分类：[窗口与导航](../index.md#window-navigation) · 可通过 WPFDevelopers NuGet 包使用。

Win10 风格抽屉导航菜单

## 示例源码

[DrawerMenuExample.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrawerMenu/DrawerMenuExample.xaml)

## XAML 用法

```xml
<wd:DrawerMenu
                            x:Name="NavigationMenu"
                            IsExpanded="{Binding ElementName=IsExpandedCheckBox, Path=IsChecked, Mode=TwoWay}"
                            SelectedItemChanged="DrawerMenu_SelectedItemChanged"
                            ShowExpandButton="{Binding ElementName=ShowExpandButtonCheckBox, Path=IsChecked}">
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
