# wd:Window

> 分类：[窗口与导航](../index.md#window-navigation) · 可通过 WPFDevelopers NuGet 包使用。

自定义窗体（ToolWindow/NoneTitleBar/Normal/HighTitleBar）

## 示例源码

[MainWindow.xaml](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MainWindow.xaml)

## XAML 用法

```xml
<wd:Window
    x:Class="WPFDevelopers.Samples.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:System="clr-namespace:System;assembly=mscorlib"
    xmlns:controls="clr-namespace:WPFDevelopers.Samples.Controls"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:example="clr-namespace:WPFDevelopers.Samples.ExampleViews"
    xmlns:helpers="clr-namespace:WPFDevelopers.Samples.Helpers"
    xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
    xmlns:local="clr-namespace:WPFDevelopers.Samples"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:vm="clr-namespace:WPFDevelopers.Samples.ViewModels"
    xmlns:wd="https://github.com/WPFDevelopersOrg/WPFDevelopers"
    Title="WPFDevelopers"
    Width="{x:Static local:App.Wdith}"
    Height="{x:Static local:App.Height}"
    Icon="pack://application:,,,/WPFDevelopers.Samples;component/WPFDevelopers.ico"
    ShowTheme="True"
    WindowStartupLocation="CenterScreen"
    mc:Ignorable="d">
```

## 继续阅读

- [控件分类目录](../index.md)
- [快速开始](../../getting-started.md)
