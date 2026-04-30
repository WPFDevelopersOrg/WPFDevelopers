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

- Visual Studio 2022
- .NET Framework 4.0 或更高 / .NET Core 3.0 或更高

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

---

## 第二步：配置 App.xaml

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
                <wd:Resources />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### 主题配置说明

`<wd:Resources />` 支持以下模式：

| 配置方式 | 效果 |
|----------|------|
| `<wd:Resources />` | 跟随系统主题（Windows 10+ 自动切换 Light/Dark） |
| `<wd:Resources Theme="Light" />` | 固定浅色主题 |
| `<wd:Resources Theme="Dark" />` | 固定深色主题 |
| `<wd:Resources Color="Fuchsia" />` | 自定义主题色 |

> **重要**：`wd:Resources` 必须放在 `Theme.xaml` 之后，否则主题无法正确加载。

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

---

## 三种使用方式

### 1. 直接使用 WD 控件

```xml
<!-- 加载动画 -->
<wd:BallLoading Width="100" IsLoading="{Binding IsLoading}" />

<!-- 抽屉面板 -->
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

<!-- 仪表盘 -->
<wd:Dashboard Value="75" MaxValue="100" />

<!-- 饼图 -->
<wd:PieControl Datas="{Binding PieDatas}" />

<!-- 时间轴 -->
<wd:TimeLineControl ItemsSource="{Binding TimelineItems}" />

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

## 控件全览

> **重要说明**：以下列表中带有 <span style="color:#e6a23c">🟡 示例专属</span> 标记的控件定义在 `WPFDevelopers.Samples.Shared/Controls/` 目录中，**不**属于 NuGet 包（`wd:` 命名空间），仅在示例项目中可用。其余控件均可通过 NuGet 安装后以 `wd:` 前缀直接使用。

### 窗口与导航

| 控件名 | 来源 | 示例文件 | 说明 |
|--------|------|----------|------|
| `wd:Window` | NuGet | [MainWindow.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/MainWindow.xaml) | 自定义窗体（ToolWindow/NoneTitleBar/Normal/HighTitleBar） |
| `wd:NotifyIcon` | NuGet | [NotifyIconExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/NotifyIconExample.xaml) | 系统托盘图标 |
| `wd:DrawerMenu` | NuGet | [DrawerMenuExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DrawerMenu/DrawerMenuExample.xaml) | Win10 风格抽屉导航菜单 |
| `wd:NavMenu3D` | NuGet | [NavMenu3DExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/NavMenu3DExample.xaml) | 3D 动画导航菜单 |
| `wd:NavScrollPanel` | NuGet | [NavScrollPanelExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/NavScrollPanel) | Win10 设置风格导航面板 |
| <span style="color:#e6a23c">🟡 TransitionPanel</span> | 示例专属 | [TransitionPanelExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/TransitionPanelExample.xaml) | 过渡动画面板（定义在 `Samples.Shared/Controls/`） |
| `wd:Drawer` | NuGet | [DrawerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DrawerExample.xaml) | 滑出式面板（上/下/左/右） |
| `wd:Mask` | NuGet | [MaskExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/MaskExample.xaml) | 遮罩层（窗体级/控件级） |
| `wd:AcrylicBlur` | NuGet | [AcrylicBlurExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/AcrylicBlurExample.xaml) | 亚克力模糊窗体 |
| `wd:TaskbarItemInfo` | NuGet | [TaskbarItemInfoExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/TaskbarItemInfoExample.xaml) | 任务栏角徽 |

### 基础控件与样式

| 控件名 | 示例文件 | 说明 |
|--------|----------|------|
| `TextBox` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 文本框（水印/清除按钮/圆角） |
| `PasswordBox` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 密码框（水印/清除/监控/明文切换） |
| `ComboBox` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 下拉框（水印/可编辑/圆角） |
| `CheckBox` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 复选框 |
| `RadioButton` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 单选按钮 |
| `ToggleButton` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 切换按钮 |
| `Slider` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 滑块（水平/垂直） |
| `ProgressBar` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 进度条（条纹/不确定/垂直） |
| `DataGrid` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 数据表格（多选行头/模板列/网格线） |
| `ListBox` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 列表框 |
| `ListView` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 列表视图（GridView） |
| `TreeView` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 树形视图（滚动动画） |
| `Expander` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 折叠面板（四方向） |
| `GroupBox` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 分组框 |
| `TabControl` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 标签控件（四方向/可关闭） |
| `Menu` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 菜单 |
| `Calendar` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 日历控件 |
| `DatePicker` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | 日期选择器（含时间选择） |

### 输入控件

| 控件名 | 来源 | 示例文件 | 说明 |
|--------|------|----------|------|
| `wd:ColorPicker` | NuGet | [ColorPickerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ColorPickerExample.xaml) | 颜色选择器 |
| `wd:TimePicker` | NuGet | [TimePickerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/TimePickerExample.xaml) | 时间选择器 |
| `wd:DateRangePicker` | NuGet | [DateRangePickerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DateRangePickerExample.xaml) | 日期范围选择器 |
| `wd:NumericBox` | NuGet | [NumericBoxExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/NumericBoxExample.xaml) | 数字输入框 |
| `wd:IPEditBox` | NuGet | [IPEditBoxExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/IPEditBoxExample.xaml) | IP 地址输入框 |
| `wd:MultiSelectComboBox` | NuGet | [MultiSelectComboBoxExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/MultiSelectComboBoxExample.xaml) | 下拉多选框 |
| <span style="color:#e6a23c">🟡 MultiSelectSearchComboBox</span> | 示例专属 | [MultiSelectSearchComboBoxExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/MultiSelectSearchComboBoxExample.xaml) | 下拉搜索多选框 |
| `wd:Password` | NuGet | [PasswordExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Passwrod/PasswordExample.xaml) | 密码显示/隐藏切换 |
| `wd:VerifyCode` | NuGet | [VerifyCodeExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/VerifyCodeExample.xaml) | 验证码绘制 |
| <span style="color:#e6a23c">🟡 RoundPicker</span> | 示例专属 | [RoundPickerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/RoundPickerExample.xaml) | 圆形颜色选择器 |
| `wd:RulerControl` | NuGet | [RulerControlExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/RulerControlExample.xaml) | 刻度尺控件 |
| `wd:Selector` | NuGet | [SelectorExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SelectorExample.xaml) | 选择器控件 |
| `wd:Dial` | NuGet | [DialExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DialExample.xaml) | 拨号盘控件 |
| `wd:GestureUnlock` | NuGet | [GestureUnlockExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/GestureUnlockExample.xaml) | 手势解锁 |
| `wd:SvgViewer` | NuGet | [SvgViewerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SvgViewerExample.xaml) | SVG 查看器 |

### 加载动画

| 控件名 | 来源 | 示例文件 | 说明 |
|--------|------|----------|------|
| `wd:RingLoading` | NuGet | [RingLoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/RingLoadingExample.xaml) | 环形加载动画 |
| `wd:BallLoading` | NuGet | [BallLoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/BallLoadingExample.xaml) | 弹球加载动画 |
| <span style="color:#e6a23c">🟡 StreamerLoading</span> | 示例专属 | [StreamerLoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/StreamerLoadingExample.xaml) | 流光加载动画 |
| `wd:WaitLoading` | NuGet | [WaitLoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/WaitLoadingExample.xaml) | 等待加载动画 |
| `wd:CycleLoading` | NuGet | [CycleLoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/CycleLoadingExample.xaml) | 循环加载动画 |
| `wd:RollLoading` | NuGet | [RollLoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/RollLoadingExample.xaml) | 滚动加载动画 |
| `wd:Loading`（附加属性） | NuGet | [LoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading) | 按钮/控件加载状态 |

### 图表与数据可视化

| 控件名 | 来源 | 示例文件 | 说明 |
|--------|------|----------|------|
| `wd:ChartRadar` | NuGet | [ChartRadarExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ChartRadarExample.xaml) | 雷达图 |
| `wd:ChartBar` | NuGet | [ChartBarExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ChartBarExample.xaml) | 柱状图 |
| `wd:ChartLine` | NuGet | [ChartLineExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ChartLineExample.xaml) | 折线图 |
| `wd:ChartPie` | NuGet | [ChartPieExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ChartPieExample.xaml) | 饼图 |
| <span style="color:#e6a23c">🟡 Dashboard</span> | 示例专属 | [DashboardExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DashboardExample.xaml) | 仪表盘（刻度跟随进度） |
| <span style="color:#e6a23c">🟡 PieControl</span> | 示例专属 | [PieControlExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/PieControlExample.xaml) | 统计饼图 |
| `wd:Gauge` | NuGet | [GaugeExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/GaugeExample.xaml) | 仪表盘 |
| `wd:LineChart` | NuGet | [LineChartExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/LineChartExample.xaml) | 折线图（另一实现） |
| `wd:CircleProgressBar` | NuGet | [CircleProgressBarExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CircleProgressBarExample.xaml) | 环形进度条 |
| `wd:Step` | NuGet | [StepExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/StepExample.xaml) | 步骤条向导 |
| `wd:BreadCrumbBar` | NuGet | [BreadCrumbBarExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BreadCrumbBarExample.xaml) | 面包屑导航栏 |
| `wd:Pagination` | NuGet | [PaginationExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/PaginationExample.xaml) | 分页控件 |
| <span style="color:#e6a23c">🟡 TimeLineControl</span> | 示例专属 | [TimeLineExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/TimeLineExample.xaml) | 时间轴（仿 Gitee） |
| `wd:Thermometer` | NuGet | [ThermometerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ThermometerExample.xaml) | 温度计 |
| `wd:DataGridFilter` | NuGet | [DataGridFilterExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DataGridFilterExample.xaml) | DataGrid 列过滤引擎 |

### 布局与面板

| 控件名 | 来源 | 示例文件 | 说明 |
|--------|------|----------|------|
| `wd:Carousel` | NuGet | [CarouselExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CarouselExample.xaml) | 轮播图 |
| `wd:CarouselEx`（EmphasizerCarousel） | NuGet | [CarouselExampleEx.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CarouselExampleEx.xaml) | 强调型轮播图 |
| `wd:CircleMenu` | NuGet | [CircleMenuExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CircleMenuExample.xaml) | 圆形菜单 |
| `wd:SixGridView` | NuGet | [SixGirdViewExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SixGirdViewExample.xaml) | 六宫格布局 |
| `wd:WaterfallPanel` | NuGet | [WaterfallPanelExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/WaterfallPanelExample.xaml) | 瀑布流面板 |
| `wd:VirtualizingWrapPanel` | NuGet | [VirtualizingWrapPanelExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/VirtualizingWrapPanelExample.xaml) | 虚拟化 WrapPanel |
| <span style="color:#e6a23c">🟡 TransformLayout</span> | 示例专属 | [TransformLayoutExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/TransformLayoutExample.xaml) | 可拖拽旋转缩放控件 |
| `wd:DrapView` | NuGet | [DrapViewExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DrapView/DrapViewExample.xaml) | 拖拽视图控件 |
| `wd:Spacing`（附加属性） | NuGet | [SpacingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SpacingExample.xaml) | 面板子元素间距 |
| `wd:PanningItems` | NuGet | [PanningItemsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/PanningItemsExample.xaml) | 平移控件 |
| <span style="color:#e6a23c">🟡 ScrollViewerAnimation</span> | 示例专属 | [ScrollViewerAnimationExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ScrollViewerAnimationExample.xaml) | 带动画的滚动条 |
| `wd:IconicThumbnail` | NuGet | [IconicThumbnailExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/IconicThumbnailExample.xaml) | 图标缩略图 |

### 动画与特效

| 控件名 | 来源 | 示例文件 | 说明 |
|--------|------|----------|------|
| `wd:BreatheLight` | NuGet | [BreatheLightExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BreatheLightExample.xaml) | 呼吸灯动画 |
| `wd:SpotLight` | NuGet | [SpotLightExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SpotLightExample.xaml) | 聚光灯效果 |
| `wd:EdgeLight` | NuGet | [EdgeLightExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/EdgeLightExample.xaml) | 边框跑马灯 |
| <span style="color:#e6a23c">🟡 RainbowButtons</span> | 示例专属 | [RainbowButtonsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/RainbowButtonsExample.xaml) | 彩虹按钮 |
| `wd:Shake` | NuGet | [ShakeExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ShakeExample.xaml) | 窗体抖动 |
| <span style="color:#e6a23c">🟡 BubblleControl</span> | 示例专属 | [BubblleControlExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BubblleControlExample.xaml) | 泡泡动画控件 |
| `wd:StarrySky` | NuGet | [StarrySkyExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/StarrySkyExample.xaml) | 星空动画 |
| <span style="color:#e6a23c">🟡 SnowCanvas</span> | 示例专属 | [SnowCanvasExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SnowCanvasExample.xaml) | 圣诞树雪花 Canvas |
| `wd:SpeedRockets` | NuGet | [SpeedRocketsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SpeedRockets/SpeedRocketsExample.xaml) | 加速火箭动画 |
| <span style="color:#e6a23c">🟡 CountdownTimer</span> | 示例专属 | [CountdownTimerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CountdownTimerExample.xaml) | 倒计时动画 |
| `wd:NumberCard` | NuGet | [NumberCardExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/NumberCard/NumberCardExample.xaml) | 3D 翻转倒计时卡片 |
| `wd:AnimationGrid` | NuGet | [AnimationGridExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/AnimationGridExample.xaml) | 动画网格 |
| `wd:LogoAnimation` | NuGet | [LogoAnimationExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/LogoAnimationExample.xaml) | 登录 Logo 动画 |
| <span style="color:#e6a23c">🟡 SongWords</span> | 示例专属 | [SongWordsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SongWordsExample.xaml) | 歌词滚动动画 |
| `wd:AnimationAudio` | NuGet | [AnimationAudioExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/AnimationAudioExample.xaml) | 音频波形可视化 |
| `wd:Barrage` | NuGet | [BarrageExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BarrageExample.xaml) | 弹幕控件 |
| `wd:CanvasHandWriting` | NuGet | [CanvasHandWritingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CanvasHandWriting/CanvasHandWritingExample.xaml) | Canvas 平滑笔迹绘制 |
| <span style="color:#e6a23c">🟡 Drawing</span> | 示例专属 | [DrawingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DrawingExample.xaml) | 自由手绘 |
| <span style="color:#e6a23c">🟡 DrawPrize</span> | 示例专属 | [DrawPrizeExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DrawPrizeExample.xaml) | 大转盘抽奖 |

### 媒体与图像处理

| 控件名 | 来源 | 示例文件 | 说明 |
|--------|------|----------|------|
| `wd:ScreenCut` | NuGet | [ScreenCutExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ScreenCutExample.xaml) | 截屏工具（画笔/箭头标注） |
| `wd:CropImage` | NuGet | [CropImageExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CropImageExample.xaml) | 图片裁剪 |
| `wd:CropAvatar` | NuGet | [CropAvatarExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CropAvatar/CropAvatarExample.xaml) | 头像裁剪选择器 |
| <span style="color:#e6a23c">🟡 CropControl</span> | 示例专属 | [CropControlExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CropControlExample.xaml) | 图片切九宫格控件 |
| <span style="color:#e6a23c">🟡 CutImage</span> | 示例专属 | [CutImageExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CutImageExample.xaml) | 用户头像裁剪方案 |
| `wd:Magnifier` | NuGet | [MagnifierExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/MagnifierExample.xaml) | 放大镜 |

### 提示与消息

| 控件名 | 来源 | 示例文件 | 说明 |
|--------|------|----------|------|
| `wd:Badge`（附加属性） | NuGet | [BadgeExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BadgeExample.xaml) | 角标通知 |
| `wd:Toast` | NuGet | [ToastExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ToastExample.xaml) | 轻提示/Toast 消息 |
| `wd:Tag` | NuGet | [TagExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/TagExample.xaml) | 标签控件 |
| `wd:PathIcon` | NuGet | [PathIconExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/PathIconExample.xaml) | 矢量路径图标 |
| `wd:AllPathIcon` | NuGet | [AllPathIconExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/AllPathIconExample.xaml) | 全部内置图标浏览 |

### 其他控件

| 控件名 | 来源 | 示例文件 | 说明 |
|--------|------|----------|------|
| `wd:ZooSemy` | NuGet | [ZooSemyExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ZooSemy/ZooSemyExample.xaml) | 拟物旋转按钮（音量旋钮） |
| `wd:OtherControl` | NuGet | [OtherControlExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/OtherControlExample.xaml) | 火炬等趣味控件 |
| `wd:Desktop` | NuGet | [DesktopBackground.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Desktop/DesktopBackground.xaml) | 动态桌面壁纸（视频播放） |
| `wd:AMap` | NuGet | [BingAMapExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/BingAMapExample.xaml) | 地图集成（Bing/高德） |
| `wd:LoginWindow` | NuGet | [LoginExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/LoginWindow/LoginExample.xaml) | 登录窗口模板 |
| <span style="color:#e6a23c">🟡 ChatEmoji</span> | 示例专属 | [ChatEmojiExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ChatEmojiExample.xaml) | Emoji + 文本聊天控件 |

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
│   ├── BasicControlsExample.xaml         # 基础控件示例
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
│   │   └── BingAMapExample.xaml
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
