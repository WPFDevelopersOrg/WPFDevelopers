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

- Visual Studio 2026
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
| `<wd:Resources />` | Default Light theme (follows system Light/Dark on Windows 10+) |
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

<!-- Dashboard gauge (NuGet version) -->
<wd:Gauge Value="75" MaxValue="100" />

<!-- Circular progress bar -->
<wd:CircleProgressBar Value="75" />
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
| <span style="color:#e6a23c">🟡 TaskbarItemInfo</span> | Sample-only | [TaskbarItemInfoExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/TaskbarItemInfoExample.xaml) | Taskbar badge |

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
| `wd:SplitButton` | NuGet | [SplitButtonExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SplitButtonExample.xaml) | Split button (main button + dropdown menu, supports text/icon options) |
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
| <span style="color:#e6a23c">🟡 Selector</span> | Sample-only | [SelectorExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SelectorExample.xaml) | Selector control |
| `wd:Dial` | NuGet | [DialExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DialExample.xaml) | Dial pad |
| `wd:GestureUnlock` | NuGet | [GestureUnlockExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/GestureUnlockExample.xaml) | Gesture pattern unlock |
| `wd:SvgViewer` | NuGet | [SvgViewerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SvgViewerExample.xaml) | SVG viewer |
| `wd:OtpBox` | NuGet | [OtpBoxExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/OtpBoxExample.xaml) | OTP verification code input (auto-jump / backspace / paste) |

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
| <span style="color:#e6a23c">🟡 LineChart</span> | Sample-only | [LineChartExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/LineChartExample.xaml) | Line chart (alternate implementation) |
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
| `wd:Carousel` | NuGet | [CarouselExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CarouselExample.xaml) | Carousel (auto-play / dot indicators / arrows / click navigation) |
| `wd:FocusCarousel` | NuGet | [FocusCarouselExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/FocusCarouselExample.xaml) | Emphasizer carousel (3D flip + zoom effect) |
| `wd:CardCarousel` | NuGet | [CardCarouselExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CardCarouselExample.xaml) | Master carousel (multi-layer overlay animation) |
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
| <span style="color:#e6a23c">🟡 SpeedRockets</span> | Sample-only | [SpeedRocketsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SpeedRockets/SpeedRocketsExample.xaml) | Speed rocket animation |
| <span style="color:#e6a23c">🟡 CountdownTimer</span> | Sample-only | [CountdownTimerExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CountdownTimerExample.xaml) | Countdown timer animation |
| <span style="color:#e6a23c">🟡 NumberCard</span> | Sample-only | [NumberCardExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/NumberCard/NumberCardExample.xaml) | 3D flip countdown cards |
| `wd:AnimationGrid` | NuGet | [AnimationGridExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/AnimationGridExample.xaml) | Animation grid |
| <span style="color:#e6a23c">🟡 LogoAnimation</span> | Sample-only | [LogoAnimationExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/LogoAnimationExample.xaml) | Login logo animation |
| <span style="color:#e6a23c">🟡 SongWords</span> | Sample-only | [SongWordsExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/SongWordsExample.xaml) | Lyrics scroll animation |
| `wd:AnimationAudio` | NuGet | [AnimationAudioExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/AnimationAudioExample.xaml) | Audio waveform visualization |
| <span style="color:#e6a23c">🟡 Barrage</span> | Sample-only | [BarrageExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/BarrageExample.xaml) | Danmaku / barrage control |
| <span style="color:#e6a23c">🟡 CanvasHandWriting</span> | Sample-only | [CanvasHandWritingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/CanvasHandWriting/CanvasHandWritingExample.xaml) | Smooth canvas handwriting |
| <span style="color:#e6a23c">🟡 Drawing</span> | Sample-only | [DrawingExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DrawingExample.xaml) | Freehand drawing |
| <span style="color:#e6a23c">🟡 DrawPrize</span> | Sample-only | [DrawPrizeExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/DrawPrizeExample.xaml) | Lottery wheel |

### Effects & Filters

| Control | Source | Example File | Description |
|---------|--------|-------------|-------------|
| `wd:GrayscaleEffect` | NuGet | [GrayscaleEffectExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/GrayscaleEffectExample.xaml) | Grayscale filter effect (adjustable intensity) |

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
| `MessageBox` (static class) | NuGet | [MessageBoxExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/MessageBoxExample.xaml) | Message dialog |

### Other Controls

| Control | Source | Example File | Description |
|---------|--------|-------------|-------------|
| <span style="color:#e6a23c">🟡 ZooSemy</span> | Sample-only | [ZooSemyExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ZooSemy/ZooSemyExample.xaml) | Skeuomorphic rotary knob (volume) |
| <span style="color:#e6a23c">🟡 OtherControl</span> | Sample-only | [OtherControlExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/OtherControlExample.xaml) | Torch & other fun controls |
| <span style="color:#e6a23c">🟡 Desktop</span> | Sample-only | [DesktopBackground.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Desktop/DesktopBackground.xaml) | Dynamic desktop wallpaper |
| <span style="color:#e6a23c">🟡 AMap</span> | Sample-only | [BingAMapExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/BingAMapExample.xaml) | Map integration (Bing / AutoNavi) |
| <span style="color:#e6a23c">🟡 LoginWindow</span> | Sample-only | [LoginExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/LoginWindow/LoginExample.xaml) | Login window template |
| <span style="color:#e6a23c">🟡 ChatEmoji</span> | Sample-only | [ChatEmojiExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/ChatEmojiExample.xaml) | Emoji + text chat |

---

## MessageBox Usage Tutorial

`MessageBox` is a static message dialog class provided by WPFDevelopers, designed as a drop-in replacement for `System.Windows.MessageBox` with a more polished and customizable appearance.

> **Note**: `MessageBox` is a **static class**, not a XAML control. It can only be called from C# code.

### Import the Namespace

To avoid conflicts with `System.Windows.MessageBox`, use an alias:

```csharp
using MessageBox = WPFDevelopers.Controls.MessageBox;
```

### Method Signatures

`MessageBox.Show()` provides 5 overloads:

```csharp
// 1. Message text only (default OK button, no icon)
MessageBoxResult Show(string messageBoxText, Window owner = null, double? buttonRadius = null, bool isDefault = true)

// 2. Message text + caption
MessageBoxResult Show(string messageBoxText, string caption, Window owner = null, double? buttonRadius = null, bool isDefault = true)

// 3. Message text + caption + buttons
MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, Window owner = null, double? buttonRadius = null, bool isDefault = true)

// 4. Message text + caption + icon
MessageBoxResult Show(string messageBoxText, string caption, MessageBoxImage icon, Window owner = null, double? buttonRadius = null, bool isDefault = true)

// 5. Message text + caption + buttons + icon (full signature)
MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, Window owner = null, double? buttonRadius = null, bool isDefault = true)
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `messageBoxText` | string | — | Message content |
| `caption` | string | — | Dialog title |
| `button` | MessageBoxButton | OK | Button set: `OK` / `OKCancel` / `YesNo` / `YesNoCancel` |
| `icon` | MessageBoxImage | None | Icon type: `Information` / `Warning` / `Error` / `Question` |
| `owner` | Window | null | Parent window. When provided, the dialog centers on the owner with an overlay mask |
| `buttonRadius` | double? | null | Button corner radius in pixels. When null, auto-detects OS: Windows 11 defaults to 4px, Windows 10 defaults to 0px |
| `isDefault` | bool | true | Whether the first button is the default button (triggered by Enter key) |

### Icon and Color Mapping

| MessageBoxImage | Icon | Color |
|-----------------|------|-------|
| `Information` | Info icon | Success (green) |
| `Warning` | Warning icon | Warning (orange) |
| `Error` | Error icon | Danger (red) |
| `Question` | Question icon | Primary (blue) |

### Usage Examples

#### 1. Information Dialog

```csharp
// File deleted successfully, with rounded buttons
MessageBox.Show("File deleted successfully.", "Message", MessageBoxButton.OK, MessageBoxImage.Information, buttonRadius: 4);
```

#### 2. Warning Dialog

```csharp
// Uses default OK button
MessageBox.Show("Performing this action may cause the file to become inaccessible!", "Warning", MessageBoxImage.Warning);
```

#### 3. Error Dialog

```csharp
MessageBox.Show("The file does not exist.", "Error", MessageBoxImage.Error);
```

#### 4. Confirmation Dialog (with Result Handling)

```csharp
var result = MessageBox.Show("The file does not exist. Continue?", "Confirm", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

switch (result)
{
    case MessageBoxResult.Yes:
        // User clicked "Yes"
        break;
    case MessageBoxResult.No:
        // User clicked "No"
        break;
    case MessageBoxResult.Cancel:
        // User clicked "Cancel" or closed the dialog
        break;
}
```

#### 5. With Owner Window (with Overlay Mask)

```csharp
// When owner is passed, the dialog centers on the parent window and shows an overlay mask
MessageBox.Show("Operation successful!", "Info", MessageBoxButton.OK, MessageBoxImage.Information, owner: this, buttonRadius: 4);
```

### Interaction Behavior

- **Close methods**: Click the close button (top-right), press `Escape`, or click any button
- **Owner window**: When `owner` is provided, the dialog centers on the parent window and displays a mask overlay. Without an owner, it auto-detects the current window or centers on screen
- **Button text**: Automatically localized via `LanguageManager` (follows system language)

---

## SplitButton Usage Tutorial

`SplitButton` is a split button control provided by WPFDevelopers. The left side is a clickable main button area, and the right side is a dropdown toggle button. It supports both text options and rich content options with icons.

### Basic Usage (ItemsSource Data Binding)

```xml
<wd:SplitButton
    Width="150"
    Content="File"
    ItemsSource="{Binding MenuItems}"
    SelectionChanged="SplitButton_SelectionChanged" />
```

```csharp
// ViewModel or Code-behind
public ObservableCollection<string> MenuItems { get; } = new ObservableCollection<string>
{
    "Save in PDF",
    "Save in Word",
    "Save in Excel",
    "Save in Image"
};

private void SplitButton_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
{
    // e.NewValue is the selected item
}
```

### Default Style (Border + Transparent Background)

```xml
<wd:SplitButton Width="150" Content="File" ItemsSource="{Binding MenuItems}" />
```

### Primary Style (Solid Primary Color Background)

```xml
<wd:SplitButton
    Width="150"
    Content="File"
    ItemsSource="{Binding MenuItems}"
    Style="{StaticResource WD.SplitButtonPrimary}" />
```

### Custom Color + XAML Children (Supports MenuItem with Icons)

```xml
<wd:SplitButton
    Width="150"
    Background="{StaticResource WD.SuccessBrush}"
    BorderBrush="{StaticResource WD.SuccessBrush}"
    Content="File"
    Foreground="White">
    <MenuItem Header="Copy">
        <MenuItem.Icon>
            <wd:PathIcon Kind="Copy" />
        </MenuItem.Icon>
    </MenuItem>
    <MenuItem Header="Paste" />
    <MenuItem Header="Cut" />
</wd:SplitButton>
```

### Key Properties

| Property | Type | Default | Description |
|------|------|--------|------|
| `Content` | object | null | Main button display content |
| `ItemsSource` | object | null | Dropdown menu data source (IEnumerable) |
| `IsDropDownOpen` | bool | false | Whether dropdown is open (two-way binding) |
| `ContextMenuStyle` | Style | null | Custom ContextMenu style |

### Events

| Event | Description |
|------|------|
| `Click` | Fired when the main button area is clicked (clicking ToggleButton does NOT fire this) |
| `SelectionChanged` | Fired when a dropdown menu item is selected |

### Available Styles

| Style Key | Description |
|--------|------|
| `WD.SplitButton` (default) | Border + transparent background, border turns primary color on hover |
| `WD.SplitButtonPrimary` | No border + solid primary color background + white text |

### Interaction Behavior

| Action | Behavior |
|------|------|
| Click main button area | Fires `Click` event |
| Click ToggleButton | Expands/collapses dropdown menu |
| Click menu item | Button Content updates to selected item, fires `SelectionChanged` |
| Click outside menu | Automatically closes dropdown |

---

## Carousel Usage Tutorial

`Carousel` is a carousel control provided by WPFDevelopers, supporting auto-play, dot indicators, arrow navigation, click navigation, and more.

### Basic Usage (XAML Declaration)

```xml
<wd:Carousel
    Width="400" Height="200"
    AutoPlay="True"
    AutoPlayInterval="0:0:4"
    ShowArrows="True"
    ItemClick="Carousel_ItemClick">
    <Border Background="#722ed1">
        <TextBlock Text="Slide 1" Foreground="White" FontSize="24" />
    </Border>
    <Border Background="#eb2f96">
        <TextBlock Text="Slide 2" Foreground="White" FontSize="24" />
    </Border>
    <Border Background="#1890ff">
        <TextBlock Text="Slide 3" Foreground="White" FontSize="24" />
    </Border>
</wd:Carousel>
```

### Data Binding Usage (MVVM)

```xml
<wd:Carousel
    ItemsSource="{Binding ImagePaths}"
    DisplayMemberPath="URL"
    AutoPlay="True"
    ItemClickCommand="{Binding CarouselClickCommand}" />
```

```csharp
// Data model
public class CarouselSlideModel
{
    public string Title { get; set; }
    public string URL { get; set; }
}

// ViewModel
public ObservableCollection<CarouselSlideModel> ImagePaths { get; set; }

public ICommand CarouselClickCommand => new RelayCommand(param =>
{
    if (param is CarouselSlideModel model)
    {
        Toast.Push($"Clicked image - {model.Title}", ToastImage.Success, true);
    }
});
```

### Key Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ItemsSource` | IEnumerable | null | Data source |
| `SelectedIndex` | int | 0 | Current selected item index |
| `SelectedItem` | object | null | Current selected item |
| `AutoPlay` | bool | false | Enable auto-play |
| `AutoPlayInterval` | TimeSpan | 3 seconds | Auto-play interval |
| `AnimationDuration` | double | 0.5 | Transition animation duration (seconds) |
| `ShowDots` | bool | true | Show dot indicators |
| `ShowArrows` | bool | true | Show left/right arrows |
| `ItemTemplate` | DataTemplate | null | Data template |
| `DisplayMemberPath` | string | null | Display member path (shows property value directly) |

### Events

| Event | Description |
|-------|-------------|
| `ItemClick` | Fired when a slide is clicked |
| `SelectedItemChanged` | Fired when selected item changes |

### Methods

| Method | Description |
|--------|-------------|
| `GoToNext()` | Navigate to next slide |
| `GoToPrevious()` | Navigate to previous slide |

---

## FocusCarousel / CardCarousel

### FocusCarousel

A carousel with 3D flip + zoom effects, where the center item is enlarged and highlighted:

```xml
<wd:FocusCarousel>
    <Image Source="pack://application:,,,/Images/photo1.jpg" />
    <Image Source="pack://application:,,,/Images/photo2.jpg" />
    <Image Source="pack://application:,,,/Images/photo3.jpg" />
</wd:FocusCarousel>
```

### CardCarousel

A carousel with multi-layer overlay animation and auto-play/interval control:

```xml
<wd:CardCarousel AutoPlay="True" AutoPlayInterval="0:0:5">
    <Image Source="pack://application:,,,/Images/photo1.jpg" />
    <Image Source="pack://application:,,,/Images/photo2.jpg" />
    <Image Source="pack://application:,,,/Images/photo3.jpg" />
</wd:CardCarousel>
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AutoPlay` | bool | false | Enable auto-play |
| `AutoPlayInterval` | TimeSpan | 0:0:3 | Playback interval (TimeSpan) |

---

## GrayscaleEffect Usage Tutorial

`GrayscaleEffect` is a pixel shader effect that converts any visual element to grayscale display, commonly used for global grayscale mode (e.g., memorial days).

### Apply to a Single Control

```xml
<Image Source="photo.jpg">
    <Image.Effect>
        <wd:GrayscaleEffect Factor="1" />
    </Image.Effect>
</Image>
```

### Apply to the Entire Window

```xml
<wd:Window>
    <wd:Window.Effect>
        <wd:GrayscaleEffect x:Name="grayscaleEffect" Factor="0" />
    </wd:Window.Effect>
</wd:Window>
```

```csharp
// Enable grayscale
var animation = new DoubleAnimation
{
    To = 1,
    Duration = TimeSpan.FromMilliseconds(1000),
    EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut }
};
grayscaleEffect.BeginAnimation(GrayscaleEffect.FactorProperty, animation);

// Disable grayscale
animation.To = 0;
grayscaleEffect.BeginAnimation(GrayscaleEffect.FactorProperty, animation);
```

### Key Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Factor` | double | 0 | Grayscale intensity, 0 = original color, 1 = full grayscale |
| `Brightness` | double | 0 | Brightness adjustment |

---

## OtpBox OTP Input Control Usage Tutorial

`OtpBox` is a control designed for OTP (One-Time Password) input scenarios, supporting auto-focus jump, backspace retreat, paste fill, and more.

### Basic Usage

```xml
<wd:OtpBox Length="6" Completed="OtpBoxCompleted" />
```

### MVVM Binding

```xml
<wd:OtpBox
    Length="6"
    Value="{Binding OtpCode, Mode=TwoWay}"
    CompletedCommand="{Binding VerifyCommand}" />
```

```csharp
// Event approach
private void OtpBoxCompleted(object sender, RoutedEventArgs e)
{
    var otpBox = e.OriginalSource as OtpBox;
    var pwd = otpBox?.Value ?? string.Empty;

    if (pwd != _otpPassword)
    {
        myOtpBox.State = ControlState.Error;
        return;
    }

    myOtpBox.State = ControlState.Success;
}

// Command approach
public ICommand CompletedCommand => new RelayCommand(param =>
{
    var pwd = param.ToString();
    // Validation logic...
});
```

### Key Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Length` | int | 4 | Number of OTP digits |
| `Value` | string | "" | Current value (supports two-way binding) |
| `State` | ControlState | None | Validation state (None/Success/Error) |
| `CompletedCommand` | ICommand | null | Command executed on completion |

### Events

| Event | Description |
|-------|-------------|
| `Completed` | Fired when all digits are filled |

### Interaction Features

| Action | Behavior |
|--------|----------|
| Enter a digit | Auto-jump to next input box |
| Backspace (empty box) | Jump to previous box and delete last character |
| Ctrl+V Paste | Auto-fill digit by digit, filter non-numeric characters |
| Arrow keys ← → | Switch between input boxes |
| Enter / Tab | Jump to next input box |
| Error state | Auto-clear and refocus after 1.5 seconds |
| Success state | Auto-reset to default state after 1.5 seconds |

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
