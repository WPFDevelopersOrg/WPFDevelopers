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

## MapView Guide

If you need to use the map control, start with:

- [MapView Usage Guide](./mapview.md)

This guide is separated from the general quick-start content because MapView is a larger, feature-rich component with dedicated examples and configuration guidance.

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

To download a published release directly, use any of these release pages: [GitHub Releases](https://github.com/WPFDevelopersOrg/WPFDevelopers/releases) · [AtomGit Releases](https://atomgit.com/WPFDevelopersOrg/WPFDevelopers/releases) · [Gitee Releases](https://gitee.com/WPFDevelopersOrg/WPFDevelopers/releases).

---

## Step 2: Configure App.xaml

When installing a WD package that includes the configuration script into a `packages.config` application project in Visual Studio, the script attempts to configure `App.xaml` automatically; library projects are not modified. For `PackageReference` or if automatic configuration does not complete, follow the steps below.

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
                <wd:Resources Radius="4" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### Theme Configuration

`<wd:Resources />` supports the following modes:

| Configuration | Effect |
|---------------|--------|
| `<wd:Resources Radius="4" />` | Restores the previous theme mode; follows the system on first use, 4px control corner radius |
| `<wd:Resources Theme="System" Radius="4" />` | Follows the system theme, 4px corner radius |
| `<wd:Resources Theme="Light" Radius="4" />` | Fixed light theme, 4px corner radius |
| `<wd:Resources Theme="Dark" Radius="4" />` | Fixed dark theme, 4px corner radius |
| `<wd:Resources Color="Fuchsia" Radius="4" />` | Custom theme color, 4px corner radius |

> **Tip**: The `Radius` property controls the corner radius of controls. Windows 11 defaults to 4px, Windows 10 defaults to 0px. Adjust as needed.

> **Important**: `wd:Resources` must be placed **after** `Theme.xaml` in MergedDictionaries, otherwise the theme will not load correctly.

### High-DPI Border Clipping on Older .NET Framework Targets

Applications targeting .NET Framework versions below 4.6 can clip borders when fixed-width controls combine `Margin` with `UseLayoutRounding` at scaling factors such as 125%. `PanelHelper.Spacing` adjusts the margins of direct children, so it can also trigger this behavior.

Enable the layout-rounding fix introduced in .NET Framework 4.6 in the application's `App.config`, without removing `Width` or `Spacing`:

```xml
<configuration>
    <runtime>
        <AppContextSwitchOverrides value="Switch.MS.Internal.DoNotApplyLayoutRoundingToMarginsAndBorderThickness=false" />
    </runtime>
</configuration>
```

If `AppContextSwitchOverrides` already exists, append this switch to its `value`, separated by a semicolon, rather than adding another element. The fix requires .NET Framework 4.6 or later at runtime and an application restart. On earlier runtimes, setting `UseLayoutRounding="False"` on the affected control is a local workaround.

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

Browse the [control catalog](controls/index.md) for categorized guides, sample source, and searchable examples.

## Three Usage Patterns

### 1. Direct WD Controls

```xml
<!-- Loading animation -->
<wd:BallLoading Width="100" IsLoading="{Binding IsLoading}" />

<!-- Top and bottom drawers span the available width. HeaderWidth only limits left and right drawers. -->
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

#### Back-to-top and back-to-bottom

Set `ScrollViewerBehavior.IsBackToTopEnabled` and/or `ScrollViewerBehavior.IsBackToBottomEnabled` to `True` on a scrollable control. The corresponding button appears at the bottom-right while the pointer or keyboard focus is within the control, and hides when the content reaches its destination.

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
    <!-- long content -->
</ScrollViewer>
```

The behavior can also be set directly on `ScrollViewer`, `WDScrollViewer`, or `TreeView`. For a `Panel`, set it on the containing `ScrollViewer` because panels do not provide scrolling themselves. It does not replace the `ItemsPanel`, so list virtualization is preserved.

Open the **Attached Properties** menu in the sample application to see an interactive example.

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

## Full Sample Project

If you prefer not to configure from scratch, reference the sample project in this repository:

```
src/WPFDevelopers.Samples.Shared/
├── App.xaml                              # App entry point & theme config
├── ExampleViews/                         # Full control example pages
│   ├── MainWindow.xaml                   # Main window (left menu + right content)
│   ├── UsageGuide.xaml                   # Usage guide page
│   ├── UsageColor.xaml                   # Color usage guide
│   ├── Basics/                           # Basic control examples (23 individual pages)
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
│   │   ├── MapViewExample.xaml
│   │   ├── MapViewExample.xaml.cs
│   │   ├── MapViewFeaturesWindow.xaml
│   │   ├── MapViewFeaturesWindow.xaml.cs
│   │   ├── TiandituTileSource.cs
│   │   └── AMapTileSource.cs
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
