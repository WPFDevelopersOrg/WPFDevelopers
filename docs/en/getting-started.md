# WPFDevelopers Getting Started

> Get started with WPFDevelopers controls in your WPF project in three steps.

## Supported .NET Versions

WPFDevelopers covers the full spectrum from .NET Framework 4.0 to .NET 10:

| Framework Family | Specific Versions |
|-----------------|-------------------|
| .NET Framework | net40, net45, net451, net452, net46, net461, net462, net47, net471, net472, net48, net481 |
| .NET Core | netcoreapp3.0, netcoreapp3.1 |
| .NET 5+ | net5.0-windows, net6.0-windows, net7.0-windows, net8.0-windows, net9.0-windows, net10.0-windows |

## Requirements

- Visual Studio 2022
- .NET Framework 4.0 or later / .NET Core 3.0 or later

---

## Step 1: Install the NuGet Package

Install via **NuGet Package Manager** or **Package Manager Console**:

```powershell
Install-Package WPFDevelopers
```

> **Note**: For the latest features, use the preview package:
> ```powershell
> Install-Package WPFDevelopers -Pre
> ```

Or in Visual Studio: right-click your project → **Manage NuGet Packages** → search `WPFDevelopers` → Install.

---

## Step 2: Configure App.xaml

Add the WD namespace and theme resources in `App.xaml`:

```xml
<Application x:Class="YourApp.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:wd="https://github.com/WPFDevelopersOrg/WPFDevelopers"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- 1. Theme must be imported first -->
                <ResourceDictionary Source="pack://application:,,,/WPFDevelopers;component/Themes/Theme.xaml" />
                <!-- 2. wd:Resources must come AFTER Theme.xaml -->
                <wd:Resources />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### Theme Configuration

`<wd:Resources />` supports the following modes:

| Configuration | Effect |
|---------------|--------|
| `<wd:Resources />` | Follows system theme (Windows 10+, auto Light/Dark switch) |
| `<wd:Resources Theme="Light" />` | Fixed light theme |
| `<wd:Resources Theme="Dark" />` | Fixed dark theme |
| `<wd:Resources Color="Fuchsia" />` | Custom theme color |

> **Important**: `wd:Resources` must be placed **after** `Theme.xaml` in MergedDictionaries, otherwise the theme will not load correctly.

---

## Step 3: Use Controls in XAML

Declare the namespace in any XAML file:

```xml
xmlns:wd="https://github.com/WPFDevelopersOrg/WPFDevelopers"
```

Then use WD controls:

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

## Three Usage Patterns

### 1. Direct WD Controls

```xml
<!-- Loading animation -->
<wd:BallLoading Width="100" IsLoading="{Binding IsLoading}" />

<!-- Drawer panel -->
<wd:Drawer x:Name="DrawerTop" Position="Top">
    <wd:Drawer.Header><TextBlock Text="Title" FontSize="16" /></wd:Drawer.Header>
    <wd:Drawer.Content><TextBlock Text="Content" /></wd:Drawer.Content>
</wd:Drawer>

<!-- Multi-select ComboBox -->
<wd:MultiSelectComboBox ItemsSource="{Binding Items}" ShowType="Tag">
    <wd:MultiSelectComboBoxItem Content="Option 1" />
    <wd:MultiSelectComboBoxItem Content="Option 2" />
</wd:MultiSelectComboBox>

<!-- Step wizard -->
<wd:Step StepIndex="1">
    <wd:StepItem Content="Step 1" />
    <wd:StepItem Content="Step 2" />
    <wd:StepItem Content="Step 3" />
</wd:Step>

<!-- Dashboard gauge -->
<wd:Dashboard Value="75" MaxValue="100" />

<!-- Pie chart -->
<wd:PieControl Datas="{Binding PieDatas}" />

<!-- Timeline -->
<wd:TimeLineControl ItemsSource="{Binding TimelineItems}" />

<!-- Carousel -->
<wd:Carousel ItemsSource="{Binding CarouselItems}" />

<!-- Radar chart -->
<wd:ChartRadar Datas="{Binding RadarDatas}" />

<!-- Bar chart -->
<wd:ChartBar Datas="{Binding BarDatas}" />

<!-- Line chart -->
<wd:ChartLine Datas="{Binding LineDatas}" />

<!-- Pie chart (LiveCharts-style) -->
<wd:ChartPie Datas="{Binding PieDatas}" />
```

### 2. Attached Properties on Standard WPF Controls

```xml
<!-- Badge notification -->
<Button wd:Badge.IsShow="True" wd:Badge.Text="new" Content="Messages" />

<!-- Button loading state -->
<Button wd:Loading.IsShow="True" wd:Loading.LoadingType="Normal" Content="Submit" />

<!-- TextBox watermark + clear button -->
<TextBox wd:ElementHelper.Watermark="Enter username" wd:ElementHelper.IsClear="True" />

<!-- PasswordBox monitoring -->
<PasswordBox wd:PasswordBoxHelper.IsMonitoring="True" wd:ElementHelper.Watermark="Password" />

<!-- DatePicker with time -->
<DatePicker wd:DatePickerHelper.ShowTime="True" />

<!-- Panel spacing -->
<WrapPanel wd:PanelHelper.Spacing="10">
    <Button Content="Button 1" />
    <Button Content="Button 2" />
</WrapPanel>

<!-- Corner radius -->
<Button wd:ElementHelper.CornerRadius="5" Content="Rounded" />
<TextBox wd:ElementHelper.CornerRadius="3" />
<DataGrid wd:ElementHelper.CornerRadius="3" />

<!-- Striped progress bar -->
<ProgressBar wd:ElementHelper.IsStripe="True" Value="80" />

<!-- Closable TabItem -->
<TabItem wd:ElementHelper.IsClear="True" Header="Closable Tab" />

<!-- TreeView scroll animation -->
<TreeView wd:TreeViewHelper.IsScrollAnimation="true" />
```

### 3. Static Resource Styles

```xml
<!-- Normal button -->
<Button Style="{StaticResource WD.NormalButton}" Content="Normal" />

<!-- Primary buttons (various themes) -->
<Button Style="{StaticResource WD.PrimaryButton}" Content="Primary" />
<Button Style="{StaticResource WD.SuccessPrimaryButton}" Content="Success" />
<Button Style="{StaticResource WD.WarningPrimaryButton}" Content="Warning" />
<Button Style="{StaticResource WD.DangerPrimaryButton}" Content="Danger" />

<!-- Default buttons (various themes) -->
<Button Style="{StaticResource WD.SuccessDefaultButton}" Content="Success" />
<Button Style="{StaticResource WD.WarningDefaultButton}" Content="Warning" />
<Button Style="{StaticResource WD.DangerDefaultButton}" Content="Danger" />
```

---

## Common Theme Resources

WD provides a set of theme resources you can reference in XAML:

| Resource Key | Type | Description |
|--------------|------|-------------|
| `WD.PrimaryBrush` | SolidColorBrush | Primary color |
| `WD.SuccessBrush` | SolidColorBrush | Success color |
| `WD.WarningBrush` | SolidColorBrush | Warning color |
| `WD.DangerBrush` | SolidColorBrush | Danger color |
| `WD.InfoSolidColorBrush` | SolidColorBrush | Info color |
| `WD.LightBrush` | SolidColorBrush | Light color |
| `WD.CircularSingularSolidColorBrush` | SolidColorBrush | Circular accent color |
| `WD.BackgroundBrush` | SolidColorBrush | Background color |
| `WD.PrimaryTextBrush` | SolidColorBrush | Primary text color |
| `WD.WarningGeometry` | Geometry | Warning icon |

---

## Complete Control Catalog

> **Important**: Controls marked <span style="color:#e6a23c">🟡 Sample-only</span> are defined in `WPFDevelopers.Samples.Shared/Controls/` and are **NOT** part of the NuGet package (`wd:` namespace). They are only available in the sample project. All other controls can be used directly via the `wd:` prefix after NuGet installation.

### Windows & Navigation

| Control | Source | Example File | Description |
|---------|--------|-------------|-------------|
| `wd:Window` | NuGet | [MainWindow.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/MainWindow.xaml) | Custom window (ToolWindow / NoneTitleBar / Normal / HighTitleBar) |
| `wd:NotifyIcon` | NuGet | [NotifyIconExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/NotifyIconExample.xaml) | System tray icon |
| `wd:DrawerMenu` | NuGet | [DrawerMenuExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DrawerMenu/DrawerMenuExample.xaml) | Win10-style drawer navigation |
| `wd:NavMenu3D` | NuGet | [NavMenu3DExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/NavMenu3DExample.xaml) | 3D animated navigation menu |
| `wd:NavScrollPanel` | NuGet | [NavScrollPanelExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/NavScrollPanel) | Win10 settings-style nav panel |
| <span style="color:#e6a23c">🟡 TransitionPanel</span> | Sample-only | [TransitionPanelExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/TransitionPanelExample.xaml) | Transition animation panel |
| `wd:Drawer` | NuGet | [DrawerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DrawerExample.xaml) | Slide-out panel (Top/Bottom/Left/Right) |
| `wd:Mask` | NuGet | [MaskExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/MaskExample.xaml) | Modal overlay |
| `wd:AcrylicBlur` | NuGet | [AcrylicBlurExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/AcrylicBlurExample.xaml) | Acrylic blur window |
| `wd:TaskbarItemInfo` | NuGet | [TaskbarItemInfoExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/TaskbarItemInfoExample.xaml) | Taskbar badge |

### Basic Controls & Styles

| Control | Example File | Description |
|---------|-------------|-------------|
| `TextBox` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | TextBox (watermark / clear button / corner radius) |
| `PasswordBox` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | PasswordBox (watermark / clear / monitor / plain text toggle) |
| `ComboBox` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | ComboBox (watermark / editable / corner radius) |
| `CheckBox` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | Checkbox |
| `RadioButton` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | Radio button |
| `ToggleButton` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | Toggle button |
| `Slider` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | Slider (horizontal / vertical) |
| `ProgressBar` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | Progress bar (striped / indeterminate / vertical) |
| `DataGrid` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | Data grid (row header selection / template columns / grid lines) |
| `ListBox` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | List box |
| `ListView` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | List view (GridView) |
| `TreeView` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | Tree view (scroll animation) |
| `Expander` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | Expander (4 directions) |
| `GroupBox` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | Group box |
| `TabControl` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | Tab control (4 directions / closable) |
| `Menu` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | Menu bar |
| `Calendar` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | Calendar |
| `DatePicker` | [BasicControlsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BasicControlsExample.xaml) | Date picker (with time selection) |

### Input Controls

| Control | Source | Example File | Description |
|---------|--------|-------------|-------------|
| `wd:ColorPicker` | NuGet | [ColorPickerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ColorPickerExample.xaml) | Color picker |
| `wd:TimePicker` | NuGet | [TimePickerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/TimePickerExample.xaml) | Time picker |
| `wd:DateRangePicker` | NuGet | [DateRangePickerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DateRangePickerExample.xaml) | Date range picker |
| `wd:NumericBox` | NuGet | [NumericBoxExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/NumericBoxExample.xaml) | Numeric input |
| `wd:IPEditBox` | NuGet | [IPEditBoxExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/IPEditBoxExample.xaml) | IP address input |
| `wd:MultiSelectComboBox` | NuGet | [MultiSelectComboBoxExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/MultiSelectComboBoxExample.xaml) | Multi-select combo box |
| <span style="color:#e6a23c">🟡 MultiSelectSearchComboBox</span> | Sample-only | [MultiSelectSearchComboBoxExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/MultiSelectSearchComboBoxExample.xaml) | Searchable multi-select combo box |
| `wd:Password` | NuGet | [PasswordExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Passwrod/PasswordExample.xaml) | Password show/hide toggle |
| `wd:VerifyCode` | NuGet | [VerifyCodeExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/VerifyCodeExample.xaml) | CAPTCHA drawing |
| <span style="color:#e6a23c">🟡 RoundPicker</span> | Sample-only | [RoundPickerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/RoundPickerExample.xaml) | Circular color picker |
| `wd:RulerControl` | NuGet | [RulerControlExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/RulerControlExample.xaml) | Ruler control |
| `wd:Selector` | NuGet | [SelectorExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SelectorExample.xaml) | Selector control |
| `wd:Dial` | NuGet | [DialExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DialExample.xaml) | Dial pad |
| `wd:GestureUnlock` | NuGet | [GestureUnlockExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/GestureUnlockExample.xaml) | Gesture pattern unlock |
| `wd:SvgViewer` | NuGet | [SvgViewerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SvgViewerExample.xaml) | SVG viewer |

### Loading Animations

| Control | Source | Example File | Description |
|---------|--------|-------------|-------------|
| `wd:RingLoading` | NuGet | [RingLoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/RingLoadingExample.xaml) | Ring loading animation |
| `wd:BallLoading` | NuGet | [BallLoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/BallLoadingExample.xaml) | Bouncing ball animation |
| <span style="color:#e6a23c">🟡 StreamerLoading</span> | Sample-only | [StreamerLoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/StreamerLoadingExample.xaml) | Streamer animation |
| `wd:WaitLoading` | NuGet | [WaitLoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/WaitLoadingExample.xaml) | Wait animation |
| `wd:CycleLoading` | NuGet | [CycleLoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/CycleLoadingExample.xaml) | Cycle animation |
| `wd:RollLoading` | NuGet | [RollLoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/RollLoadingExample.xaml) | Rolling animation |
| `wd:Loading` (attached) | NuGet | [LoadingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Loading) | Button/control loading state |

### Charts & Data Visualization

| Control | Source | Example File | Description |
|---------|--------|-------------|-------------|
| `wd:ChartRadar` | NuGet | [ChartRadarExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ChartRadarExample.xaml) | Radar chart |
| `wd:ChartBar` | NuGet | [ChartBarExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ChartBarExample.xaml) | Bar chart |
| `wd:ChartLine` | NuGet | [ChartLineExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ChartLineExample.xaml) | Line chart |
| `wd:ChartPie` | NuGet | [ChartPieExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ChartPieExample.xaml) | Pie chart |
| <span style="color:#e6a23c">🟡 Dashboard</span> | Sample-only | [DashboardExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DashboardExample.xaml) | Dashboard gauge (tick marks follow progress) |
| <span style="color:#e6a23c">🟡 PieControl</span> | Sample-only | [PieControlExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/PieControlExample.xaml) | Pie chart statistics |
| `wd:Gauge` | NuGet | [GaugeExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/GaugeExample.xaml) | Gauge control |
| `wd:LineChart` | NuGet | [LineChartExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/LineChartExample.xaml) | Line chart (alternate implementation) |
| `wd:CircleProgressBar` | NuGet | [CircleProgressBarExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CircleProgressBarExample.xaml) | Circular progress bar |
| `wd:Step` | NuGet | [StepExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/StepExample.xaml) | Step wizard |
| `wd:BreadCrumbBar` | NuGet | [BreadCrumbBarExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BreadCrumbBarExample.xaml) | Breadcrumb navigation |
| `wd:Pagination` | NuGet | [PaginationExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/PaginationExample.xaml) | Pagination control |
| <span style="color:#e6a23c">🟡 TimeLineControl</span> | Sample-only | [TimeLineExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/TimeLineExample.xaml) | Timeline (Gitee-style) |
| `wd:Thermometer` | NuGet | [ThermometerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ThermometerExample.xaml) | Thermometer |
| `wd:DataGridFilter` | NuGet | [DataGridFilterExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DataGridFilterExample.xaml) | DataGrid column filter engine |

### Layout & Panels

| Control | Source | Example File | Description |
|---------|--------|-------------|-------------|
| `wd:Carousel` | NuGet | [CarouselExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CarouselExample.xaml) | Carousel |
| `wd:CarouselEx` | NuGet | [CarouselExampleEx.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CarouselExampleEx.xaml) | Emphasizer carousel |
| `wd:CircleMenu` | NuGet | [CircleMenuExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CircleMenuExample.xaml) | Circular menu |
| `wd:SixGridView` | NuGet | [SixGirdViewExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SixGirdViewExample.xaml) | Six-column grid layout |
| `wd:WaterfallPanel` | NuGet | [WaterfallPanelExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/WaterfallPanelExample.xaml) | Waterfall / masonry panel |
| `wd:VirtualizingWrapPanel` | NuGet | [VirtualizingWrapPanelExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/VirtualizingWrapPanelExample.xaml) | Virtualizing WrapPanel |
| <span style="color:#e6a23c">🟡 TransformLayout</span> | Sample-only | [TransformLayoutExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/TransformLayoutExample.xaml) | Draggable, resizable, rotatable control |
| `wd:DrapView` | NuGet | [DrapViewExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DrapView/DrapViewExample.xaml) | Drag-and-drop view |
| `wd:Spacing` (attached) | NuGet | [SpacingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SpacingExample.xaml) | Panel child spacing |
| `wd:PanningItems` | NuGet | [PanningItemsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/PanningItemsExample.xaml) | Panning control (touch swipe) |
| <span style="color:#e6a23c">🟡 ScrollViewerAnimation</span> | Sample-only | [ScrollViewerAnimationExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ScrollViewerAnimationExample.xaml) | Animated scrollbar |
| `wd:IconicThumbnail` | NuGet | [IconicThumbnailExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/IconicThumbnailExample.xaml) | Iconic thumbnail |

### Animation & Effects

| Control | Source | Example File | Description |
|---------|--------|-------------|-------------|
| `wd:BreatheLight` | NuGet | [BreatheLightExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BreatheLightExample.xaml) | Breathing light animation |
| `wd:SpotLight` | NuGet | [SpotLightExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SpotLightExample.xaml) | Spotlight effect |
| `wd:EdgeLight` | NuGet | [EdgeLightExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/EdgeLightExample.xaml) | Edge marquee light |
| <span style="color:#e6a23c">🟡 RainbowButtons</span> | Sample-only | [RainbowButtonsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/RainbowButtonsExample.xaml) | Rainbow buttons |
| `wd:Shake` | NuGet | [ShakeExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ShakeExample.xaml) | Window shake |
| <span style="color:#e6a23c">🟡 BubblleControl</span> | Sample-only | [BubblleControlExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BubblleControlExample.xaml) | Bubble animation |
| `wd:StarrySky` | NuGet | [StarrySkyExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/StarrySkyExample.xaml) | Starry sky animation |
| <span style="color:#e6a23c">🟡 SnowCanvas</span> | Sample-only | [SnowCanvasExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SnowCanvasExample.xaml) | Christmas tree & snow canvas |
| `wd:SpeedRockets` | NuGet | [SpeedRocketsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SpeedRockets/SpeedRocketsExample.xaml) | Speed rocket animation |
| <span style="color:#e6a23c">🟡 CountdownTimer</span> | Sample-only | [CountdownTimerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CountdownTimerExample.xaml) | Countdown timer animation |
| `wd:NumberCard` | NuGet | [NumberCardExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/NumberCard/NumberCardExample.xaml) | 3D flip countdown cards |
| `wd:AnimationGrid` | NuGet | [AnimationGridExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/AnimationGridExample.xaml) | Animation grid |
| `wd:LogoAnimation` | NuGet | [LogoAnimationExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/LogoAnimationExample.xaml) | Login logo animation |
| <span style="color:#e6a23c">🟡 SongWords</span> | Sample-only | [SongWordsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SongWordsExample.xaml) | Lyrics scroll animation |
| `wd:AnimationAudio` | NuGet | [AnimationAudioExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/AnimationAudioExample.xaml) | Audio waveform visualization |
| `wd:Barrage` | NuGet | [BarrageExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BarrageExample.xaml) | Danmaku / barrage control |
| `wd:CanvasHandWriting` | NuGet | [CanvasHandWritingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CanvasHandWriting/CanvasHandWritingExample.xaml) | Smooth canvas handwriting |
| <span style="color:#e6a23c">🟡 Drawing</span> | Sample-only | [DrawingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DrawingExample.xaml) | Freehand drawing |
| <span style="color:#e6a23c">🟡 DrawPrize</span> | Sample-only | [DrawPrizeExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DrawPrizeExample.xaml) | Lottery wheel |

### Media & Image Processing

| Control | Source | Example File | Description |
|---------|--------|-------------|-------------|
| `wd:ScreenCut` | NuGet | [ScreenCutExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ScreenCutExample.xaml) | Screen capture (pen / arrow annotation) |
| `wd:CropImage` | NuGet | [CropImageExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CropImageExample.xaml) | Image cropping |
| `wd:CropAvatar` | NuGet | [CropAvatarExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CropAvatar/CropAvatarExample.xaml) | Avatar cropping selector |
| <span style="color:#e6a23c">🟡 CropControl</span> | Sample-only | [CropControlExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CropControlExample.xaml) | Image nine-grid cutter |
| <span style="color:#e6a23c">🟡 CutImage</span> | Sample-only | [CutImageExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CutImageExample.xaml) | User avatar cropping solution |
| `wd:Magnifier` | NuGet | [MagnifierExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/MagnifierExample.xaml) | Magnifier |

### Notifications & Messages

| Control | Source | Example File | Description |
|---------|--------|-------------|-------------|
| `wd:Badge` (attached) | NuGet | [BadgeExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BadgeExample.xaml) | Badge notification |
| `wd:Toast` | NuGet | [ToastExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ToastExample.xaml) | Toast message popup |
| `wd:Tag` | NuGet | [TagExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/TagExample.xaml) | Tag control |
| `wd:PathIcon` | NuGet | [PathIconExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/PathIconExample.xaml) | Vector path icon |
| `wd:AllPathIcon` | NuGet | [AllPathIconExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/AllPathIconExample.xaml) | Built-in icon browser |

### Other Controls

| Control | Source | Example File | Description |
|---------|--------|-------------|-------------|
| `wd:ZooSemy` | NuGet | [ZooSemyExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ZooSemy/ZooSemyExample.xaml) | Skeuomorphic rotary knob (volume) |
| `wd:OtherControl` | NuGet | [OtherControlExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/OtherControlExample.xaml) | Torch & other fun controls |
| `wd:Desktop` | NuGet | [DesktopBackground.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Desktop/DesktopBackground.xaml) | Dynamic desktop wallpaper |
| `wd:AMap` | NuGet | [BingAMapExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/BingAMapExample.xaml) | Map integration (Bing / AutoNavi) |
| `wd:LoginWindow` | NuGet | [LoginExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/LoginWindow/LoginExample.xaml) | Login window template |
| <span style="color:#e6a23c">🟡 ChatEmoji</span> | Sample-only | [ChatEmojiExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ChatEmojiExample.xaml) | Emoji + text chat |

---

## Full Sample Project

If you prefer not to configure from scratch, reference the sample project in this repository:

```
src/WPFDevelopers.Samples.Shared/
├── App.xaml                              # App entry point & theme config
├── ExampleViews/                         # Full control example pages
│   ├── MainWindow.xaml                   # Main window (left menu + right content)
│   ├── UsageGuide.xaml                   # Usage guide page
│   ├── UsageColor.xaml                   # Color usage guide
│   ├── BasicControlsExample.xaml         # Basic control examples
│   ├── Loading/                          # Loading animation series
│   │   ├── BallLoadingExample.xaml
│   │   ├── RingLoadingExample.xaml
│   │   ├── StreamerLoadingExample.xaml
│   │   ├── WaitLoadingExample.xaml
│   │   ├── CycleLoadingExample.xaml
│   │   └── RollLoadingExample.xaml
│   ├── DrawerMenu/                       # Drawer menu sub-pages
│   │   ├── HomePage.xaml
│   │   ├── EmailPage.xaml
│   │   └── EdgePage.xaml
│   ├── NavScrollPanel/                   # Nav settings panel sub-pages
│   │   ├── About.xaml
│   │   ├── PrivacySettings.xaml
│   │   ├── PlaybackSettings.xaml
│   │   ├── ShortcutKeys.xaml
│   │   └── DesktopLyrics.xaml
│   ├── LoginWindow/                      # Login window template
│   │   ├── CustomControl/
│   │   ├── CustomStyle/
│   │   └── LoginExample.xaml
│   ├── CropAvatar/                       # Avatar cropping
│   │   ├── CropAvatarExample.xaml
│   │   └── CropAvatarWindow.xaml
│   ├── NumberCard/                       # Countdown cards
│   │   ├── NumberCardExample.xaml
│   │   └── NumberCardControl.xaml
│   ├── SpeedRockets/                     # Rocket animation
│   │   ├── SpeedRocketsExample.xaml
│   │   └── SpeedRocketsMini.xaml
│   ├── ZooSemy/                          # Skeuomorphic knob
│   │   ├── ZooSemyExample.xaml
│   │   └── VolumeControl.xaml
│   ├── CanvasHandWriting/                # Handwriting
│   │   └── CanvasHandWritingExample.xaml
│   ├── Desktop/                          # Desktop wallpaper
│   │   ├── DesktopBackground.xaml
│   │   └── DesktopPlayVideo.xaml
│   ├── Passwrod/                         # Password controls
│   │   ├── PasswordExample.xaml
│   │   └── PasswordWithPlainText.xaml
│   ├── Map/                              # Map
│   │   └── BingAMapExample.xaml
│   ├── DrapView/                         # Drag view
│   │   └── DrapViewExample.xaml
│   └── ... (more example files)
├── Controls/                             # Sample helper controls (CodeViewer, NavigateMenu, etc.)
├── ViewModels/                           # Sample ViewModels
├── Models/                               # Data models
├── Helpers/MenuEnum.cs                   # All example category enum
└── Converts/                             # Value converters
```

Each example page includes a built-in code viewer (`CodeViewer`) that lets you inspect the corresponding XAML/C# source code directly.

---
