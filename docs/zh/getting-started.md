# WPFDevelopers 快速开始

> 三步即可在你的 WPF 项目中使用 WPFDevelopers 控件库

## 支持的 .NET 版本

WPFDevelopers 覆盖从 .NET Framework 4.0 到 .NET 10 的完整版本线：

| 框架系列 | 具体版本 |
|----------|----------|
| .NET Framework | net40, net45, net451, net452, net46, net461, net462, net47, net471, net472, net48, net481 |
| .NET Core | netcoreapp3.0, netcoreapp3.1 |
| .NET 5+ | net5.0-windows, net6.0-windows, net7.0-windows, net8.0-windows, net9.0-windows, net10.0-windows |

## 环境要求

- Visual Studio 2026
- .NET Framework 4.0 或更高 / .NET Core 3.0 或更高

## MapView 专题文档

如果你要使用地图控件，建议直接阅读：

- [MapView 使用指南](./mapview.md)

该文档单独拆分，重点说明 MapView 的使用方式、底图配置、图层和聚合示例，以及车辆轨迹场景。

---

## 第一步：安装 NuGet 包

通过 **NuGet 包管理器** 或 **包管理器控制台** 安装：

```powershell
Install-Package WPFDevelopers
```

> **注意**：如需最新功能，请使用预览版：
> ```powershell
> Install-Package WPFDevelopers -Pre
> ```

或在 Visual Studio 中：右键项目 → **管理 NuGet 程序包** → 搜索 `WPFDevelopers` → 安装。

如需直接下载已发布版本，可从以下任一 Release 页面获取：[GitHub Releases](https://github.com/WPFDevelopersOrg/WPFDevelopers/releases) · [AtomGit Releases](https://atomgit.com/WPFDevelopersOrg/WPFDevelopers/releases) · [Gitee Releases](https://gitee.com/WPFDevelopersOrg/WPFDevelopers/releases)。

---

## 第二步：配置 App.xaml

在 Visual Studio 中为 `packages.config` 应用项目安装包含配置脚本的 WD 包时，脚本会尝试自动配置 `App.xaml`；类库项目不会自动修改。使用 `PackageReference` 或自动配置未完成时，请按下方步骤手动配置。

在 `App.xaml` 中添加 WD 命名空间及主题资源：

```xml
<Application x:Class="YourApp.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:wd="https://github.com/WPFDevelopersOrg/WPFDevelopers"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- 1. 必须先引入主题 -->
                <ResourceDictionary Source="pack://application:,,,/WPFDevelopers;component/Themes/Theme.xaml" />
                <!-- 2. wd:Resources 必须在 Theme.xaml 之后 -->
                <wd:Resources Radius="4" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### 主题配置说明

`<wd:Resources />` 支持以下模式：

| 配置方式 | 效果 |
|----------|------|
| `<wd:Resources Radius="4" />` | 恢复上次主题模式，首次使用跟随系统，控件圆角 4px |
| `<wd:Resources Theme="System" Radius="4" />` | 跟随系统主题，控件圆角 4px |
| `<wd:Resources Theme="Light" Radius="4" />` | 固定浅色主题，控件圆角 4px |
| `<wd:Resources Theme="Dark" Radius="4" />` | 固定深色主题，控件圆角 4px |
| `<wd:Resources Color="Fuchsia" Radius="4" />` | 自定义主题色，控件圆角 4px |

> **提示**：`Radius` 属性控制控件圆角大小，Windows 11 默认为 4px，Windows 10 默认为 0px。可根据需要调整。

> **重要**：`wd:Resources` 必须放在 `Theme.xaml` 之后，否则主题无法正确加载。


### 旧版 .NET Framework 的高 DPI 边框裁剪

目标框架低于 .NET Framework 4.6 的应用，在 125% 等缩放比例下，启用 `UseLayoutRounding` 后，固定宽度控件配合 `Margin` 可能出现边框裁剪。`PanelHelper.Spacing` 通过修改直接子元素的 `Margin` 实现间距，因此也可能触发此问题。

在应用的 `App.config` 中启用 .NET Framework 4.6 引入的布局取整修复，无需删除 `Width` 或 `Spacing`：

```xml
<configuration>
    <runtime>
        <AppContextSwitchOverrides value="Switch.MS.Internal.DoNotApplyLayoutRoundingToMarginsAndBorderThickness=false" />
    </runtime>
</configuration>
```

如已有 `AppContextSwitchOverrides`，请将此开关以分号追加到现有 `value` 中，不要重复添加节点。该修复要求运行时为 .NET Framework 4.6 或更高，修改后需重新启动应用；更早的运行时可对受影响控件设置 `UseLayoutRounding="False"` 作为局部规避方案。

---

## 第三步：在 XAML 中使用控件

在需要使用的页面/窗口中添加命名空间声明：

```xml
xmlns:wd="https://github.com/WPFDevelopersOrg/WPFDevelopers"
```

然后即可使用 WD 控件：

```xml
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:wd="https://github.com/WPFDevelopersOrg/WPFDevelopers"
        Title="My App" Width="800" Height="600">
    <Grid>
        <wd:BallLoading Width="100" IsLoading="True" />
    </Grid>
</Window>
```

控件分类目录按控件分别提供使用说明、示例源码和已有专题内容：[浏览控件分类目录](controls/index.md)。

## 三种使用方式

### 1. 直接使用 WD 控件

```xml
<!-- 加载动画 -->
<wd:BallLoading Width="100" IsLoading="{Binding IsLoading}" />

<!-- 抽屉面板：顶部和底部抽屉占满可用宽度，HeaderWidth 只限制左侧和右侧抽屉。 -->
<wd:Drawer x:Name="DrawerTop" Position="Top">
    <wd:Drawer.Header><TextBlock Text="面板标题" FontSize="16" /></wd:Drawer.Header>
    <wd:Drawer.Content><TextBlock Text="面板内容" /></wd:Drawer.Content>
</wd:Drawer>

<!-- 下拉多选框 -->
<wd:MultiSelectComboBox ItemsSource="{Binding Items}" ShowType="Tag">
    <wd:MultiSelectComboBoxItem Content="选项一" />
    <wd:MultiSelectComboBoxItem Content="选项二" />
</wd:MultiSelectComboBox>

<!-- 步骤条 -->
<wd:Step StepIndex="1">
    <wd:StepItem Content="第一步" />
    <wd:StepItem Content="第二步" />
    <wd:StepItem Content="第三步" />
</wd:Step>

<!-- 轮播图 -->
<wd:Carousel ItemsSource="{Binding CarouselItems}" />

<!-- 雷达图 -->
<wd:ChartRadar Datas="{Binding RadarDatas}" />

<!-- 柱状图 -->
<wd:ChartBar Datas="{Binding BarDatas}" />

<!-- 折线图 -->
<wd:ChartLine Datas="{Binding LineDatas}" />

<!-- 饼图（仿LiveCharts） -->
<wd:ChartPie Datas="{Binding PieDatas}" />

<!-- 仪表盘（NuGet 版本） -->
<wd:Gauge Value="75" MaxValue="100" />

<!-- 环形进度条 -->
<wd:CircleProgressBar Value="75" />
```

### 2. 附加属性扩展标准 WPF 控件

```xml
<!-- 角标 -->
<Button wd:Badge.IsShow="True" wd:Badge.Text="new" Content="消息" />

<!-- 按钮加载状态 -->
<Button wd:Loading.IsShow="True" wd:Loading.LoadingType="Normal" Content="提交" />

<!-- TextBox 水印 + 清除按钮 -->
<TextBox wd:ElementHelper.Watermark="请输入用户名" wd:ElementHelper.IsClear="True" />

<!-- PasswordBox 监控 -->
<PasswordBox wd:PasswordBoxHelper.IsMonitoring="True" wd:ElementHelper.Watermark="密码" />

<!-- DatePicker 显示时间 -->
<DatePicker wd:DatePickerHelper.ShowTime="True" />
<DatePicker wd:DatePickerHelper.ShowTime="True" wd:DatePickerHelper.Watermark="日期时间" />

<!-- 面板间距 -->
<WrapPanel wd:PanelHelper.Spacing="10">
    <Button Content="按钮1" />
    <Button Content="按钮2" />
</WrapPanel>

<!-- 圆角 -->
<Button wd:ElementHelper.CornerRadius="5" Content="圆角按钮" />
<TextBox wd:ElementHelper.CornerRadius="3" />
<DataGrid wd:ElementHelper.CornerRadius="3" />

<!-- 条纹进度条 -->
<ProgressBar wd:ElementHelper.IsStripe="True" Value="80" />

<!-- TabItem 关闭按钮 -->
<TabItem wd:ElementHelper.IsClear="True" Header="可关闭的标签页" />

<!-- TreeView 滚动动画 -->
<TreeView wd:TreeViewHelper.IsScrollAnimation="true" />

<!-- ScrollIntoView 定位 -->
<ListBoxItem wd:ScrollIntoView.IsPosition="True" />
```

#### 回到顶部和回到底部

在可滚动控件上将 `ScrollViewerBehavior.IsBackToTopEnabled` 和/或 `ScrollViewerBehavior.IsBackToBottomEnabled` 设置为 `True`。当光标或键盘焦点进入控件时，对应按钮会显示在控件右下角；到达目标位置后自动隐藏。

```xml
<ListBox
    wd:ScrollViewerBehavior.IsBackToTopEnabled="True"
    ItemsSource="{Binding Items}" />

<ListView
    wd:ScrollViewerBehavior.IsBackToTopEnabled="True"
    ItemsSource="{Binding Items}" />

<DataGrid
    wd:ScrollViewerBehavior.IsBackToTopEnabled="True"
    wd:ScrollViewerBehavior.IsBackToBottomEnabled="True"
    ItemsSource="{Binding Items}" />

<ScrollViewer
    wd:ScrollViewerBehavior.IsBackToTopEnabled="True"
    wd:ScrollViewerBehavior.IsBackToBottomEnabled="True">
    <!-- 长内容 -->
</ScrollViewer>
```

该附加属性也可直接用于 `ScrollViewer`、`WDScrollViewer` 和 `TreeView`。`Panel` 本身不提供滚动能力，应将属性设置在包裹它的 `ScrollViewer` 上。该行为不会修改 `ItemsPanel`，因此不会影响列表虚拟化。

示例程序中可打开 **附加属性** 菜单查看交互示例。

### 3. 静态资源样式

```xml
<!-- 普通按钮 -->
<Button Style="{StaticResource WD.NormalButton}" Content="普通" />

<!-- 主要按钮（各色系） -->
<Button Style="{StaticResource WD.PrimaryButton}" Content="主要" />
<Button Style="{StaticResource WD.SuccessPrimaryButton}" Content="成功" />
<Button Style="{StaticResource WD.WarningPrimaryButton}" Content="警告" />
<Button Style="{StaticResource WD.DangerPrimaryButton}" Content="危险" />

<!-- 默认按钮（各色系） -->
<Button Style="{StaticResource WD.SuccessDefaultButton}" Content="成功" />
<Button Style="{StaticResource WD.WarningDefaultButton}" Content="警告" />
<Button Style="{StaticResource WD.DangerDefaultButton}" Content="危险" />
```

---

## 常用主题资源

WD 提供了一套主题资源，可在 XAML 中引用：

| 资源键 | 类型 | 说明 |
|--------|------|------|
| `WD.PrimaryBrush` | SolidColorBrush | 主题色 |
| `WD.SuccessBrush` | SolidColorBrush | 成功色 |
| `WD.WarningBrush` | SolidColorBrush | 警告色 |
| `WD.DangerBrush` | SolidColorBrush | 危险色 |
| `WD.InfoSolidColorBrush` | SolidColorBrush | 信息色 |
| `WD.LightBrush` | SolidColorBrush | 浅色 |
| `WD.CircularSingularSolidColorBrush` | SolidColorBrush | 环形色 |
| `WD.BackgroundBrush` | SolidColorBrush | 背景色 |
| `WD.PrimaryTextBrush` | SolidColorBrush | 主文本色 |
| `WD.WarningGeometry` | Geometry | 警告图标 |

---

## 完整示例项目

如果你不想从头配置，可以直接参考本仓库的示例项目：

```
src/WPFDevelopers.Samples.Shared/
├── App.xaml                              # 应用入口与主题配置
├── ExampleViews/                         # 完整控件示例页面
│   ├── MainWindow.xaml                   # 主窗口（左侧菜单 + 右侧内容）
│   ├── UsageGuide.xaml                   # 使用指南页
│   ├── UsageColor.xaml                   # 颜色使用指南
│   ├── Basics/                           # 基础控件示例（23 个独立页面）
│   │   ├── ThemeExample.xaml
│   │   ├── MenuExample.xaml
│   │   ├── WindowBasicExample.xaml
│   │   ├── LoadingBasicExample.xaml
│   │   ├── ButtonExample.xaml
│   │   ├── RadioButtonExample.xaml
│   │   ├── CheckboxExample.xaml
│   │   ├── TextBoxExample.xaml
│   │   ├── PasswordBoxExample.xaml
│   │   ├── ComboBoxExample.xaml
│   │   ├── ToggleButtonExample.xaml
│   │   ├── DatePickerExample.xaml
│   │   ├── DateTimePickerExample.xaml
│   │   ├── CalendarExample.xaml
│   │   ├── SliderExample.xaml
│   │   ├── ProgressBarExample.xaml
│   │   ├── DataGridExample.xaml
│   │   ├── ListBoxExample.xaml
│   │   ├── ListViewExample.xaml
│   │   ├── TreeViewExample.xaml
│   │   ├── ExpanderExample.xaml
│   │   ├── GroupBoxExample.xaml
│   │   └── TabControlExample.xaml
│   ├── Loading/                          # 加载动画系列
│   │   ├── BallLoadingExample.xaml
│   │   ├── RingLoadingExample.xaml
│   │   ├── StreamerLoadingExample.xaml
│   │   ├── WaitLoadingExample.xaml
│   │   ├── CycleLoadingExample.xaml
│   │   └── RollLoadingExample.xaml
│   ├── DrawerMenu/                       # 抽屉菜单子页面
│   │   ├── HomePage.xaml
│   │   ├── EmailPage.xaml
│   │   └── EdgePage.xaml
│   ├── NavScrollPanel/                   # 导航设置面板子页面
│   │   ├── About.xaml
│   │   ├── PrivacySettings.xaml
│   │   ├── PlaybackSettings.xaml
│   │   ├── ShortcutKeys.xaml
│   │   └── DesktopLyrics.xaml
│   ├── LoginWindow/                      # 登录窗口模板
│   │   ├── CustomControl/
│   │   ├── CustomStyle/
│   │   └── LoginExample.xaml
│   ├── CropAvatar/                       # 头像裁剪
│   │   ├── CropAvatarExample.xaml
│   │   └── CropAvatarWindow.xaml
│   ├── NumberCard/                       # 倒计时卡片
│   │   ├── NumberCardExample.xaml
│   │   └── NumberCardControl.xaml
│   ├── SpeedRockets/                     # 火箭动画
│   │   ├── SpeedRocketsExample.xaml
│   │   └── SpeedRocketsMini.xaml
│   ├── ZooSemy/                          # 拟物旋钮
│   │   ├── ZooSemyExample.xaml
│   │   └── VolumeControl.xaml
│   ├── CanvasHandWriting/                # 手写笔迹
│   │   └── CanvasHandWritingExample.xaml
│   ├── Desktop/                          # 桌面壁纸
│   │   ├── DesktopBackground.xaml
│   │   └── DesktopPlayVideo.xaml
│   ├── Passwrod/                         # 密码控件
│   │   ├── PasswordExample.xaml
│   │   └── PasswordWithPlainText.xaml
│   ├── Map/                              # 地图
│   │   ├── MapViewExample.xaml
│   │   ├── MapViewExample.xaml.cs
│   │   ├── MapViewFeaturesWindow.xaml
│   │   ├── MapViewFeaturesWindow.xaml.cs
│   │   ├── TiandituTileSource.cs
│   │   └── AMapTileSource.cs
│   ├── DrapView/                         # 拖拽视图
│   │   └── DrapViewExample.xaml
│   └── ...（更多示例文件）
├── Controls/                             # 示例辅助控件（CodeViewer、NavigateMenu等）
├── ViewModels/                           # 示例 ViewModel
├── Models/                               # 数据模型
├── Helpers/MenuEnum.cs                   # 所有示例分类枚举
└── Converts/                             # 值转换器
```

每个示例页面都包含内置的代码查看器（`CodeViewer`），可直接查看对应 XAML/C# 源码。

---
