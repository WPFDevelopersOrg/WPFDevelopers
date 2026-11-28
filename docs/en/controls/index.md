# Controls by Category

Browse 131 control guides, attached properties, and sample implementations across 11 categories. Select a control for its guide or follow the source link to the sample.

<div class="wd-control-search" role="search" data-language="en">
  <label class="wd-control-search__label" for="wd-control-filter">Search controls</label>
  <input id="wd-control-filter" type="search" placeholder="Search by control, category, or purpose, e.g. Button, date, navigation" autocomplete="off">
  <div class="wd-control-search__dropdown">
    <p id="wd-control-search-status" aria-live="polite">Enter a keyword to filter controls and jump to a catalog entry.</p>
    <ol id="wd-control-search-results" class="wd-control-search__results"></ol>
  </div>
</div>

## Control Catalog { #control-catalog }

The catalog distinguishes NuGet controls, sample-only implementations, and standard WPF controls shown with WD styles. Search matches names, categories, sources, and descriptions.

> **Sample-only** entries are not shipped in the NuGet package. The sample navigation marks them with a `DEMO` badge; `NEW` and `UPDATE` badges indicate separate change statuses.

### Windows & Navigation { #window-navigation }

| Control Guide | Source | Description & Example |
|---|---|---|
| [wd:Window](window-navigation/window.md) | NuGet | Custom window (ToolWindow / NoneTitleBar / Normal / HighTitleBar) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MainWindow.xaml) |
| [wd:NotifyIcon](window-navigation/notify-icon.md) | NuGet | System tray icon · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/NotifyIconExample.xaml) |
| [wd:DrawerMenu](window-navigation/drawer-menu.md) | NuGet | Win10-style drawer navigation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrawerMenu/DrawerMenuExample.xaml) |
| [wd:SidePanel](window-navigation/side-panel.md) | NuGet | Collapsible sidebar with custom content · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SidePanelExample.xaml) |
| [wd:NavMenu3D](window-navigation/nav-menu3-d.md) | NuGet | 3D animated navigation menu · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/NavMenu3DExample.xaml) |
| [wd:NavScrollPanel](window-navigation/nav-scroll-panel.md) | NuGet | Win10 settings-style nav panel · [Example folder](https://github.com/WPFDevelopersOrg/WPFDevelopers/tree/master/src/WPFDevelopers.Samples.Shared/ExampleViews/NavScrollPanel) |
| [TransitionPanel](window-navigation/transition-panel.md) | Sample-only | Transition animation panel · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/TransitionPanelExample.xaml) |
| [wd:Drawer](window-navigation/drawer.md) | NuGet | Slide-out panel (Top/Bottom/Left/Right) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrawerExample.xaml) |
| [wd:Mask](window-navigation/mask.md) | NuGet | Modal overlay · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MaskExample.xaml) |
| [wd:AcrylicBlur](window-navigation/acrylic-blur.md) | NuGet | Acrylic blur window · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/AcrylicBlurExample.xaml) |
| [TaskbarItemInfo](window-navigation/taskbar-item-info.md) | Sample-only | Taskbar badge · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/TaskbarItemInfoExample.xaml) |

### Basic Controls & Styles { #basic-controls }

| Control Guide | Source | Description & Example |
|---|---|---|
| [wd:WDBorder](basic-controls/wd-border.md) | NuGet | Inherits from Border and exposes a ContentClip geometry for rounded child clipping · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Net40/Styles/Styles.TreeView.xaml) |
| [Theme](basic-controls/theme.md) | WPF built-in (WD style sample) | Theme switching (Light/Dark/Languages) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/ThemeExample.xaml) |
| [Window (WPF)](basic-controls/window.md) | WPF built-in (WD style sample) | Basic windows (ToolWindow/NoneTitleBar) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/WindowBasicExample.xaml) |
| [Loading](basic-controls/loading.md) | WPF built-in (WD style sample) | Loading states (button loading/loading task) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/LoadingBasicExample.xaml) |
| [Button](basic-controls/button.md) | WPF built-in (WD style sample) | Button styles · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/ButtonExample.xaml) |
| [TextBox](basic-controls/text-box.md) | WPF built-in (WD style sample) | TextBox (watermark / clear button / corner radius) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/TextBoxExample.xaml) |
| [PasswordBox](basic-controls/password-box.md) | WPF built-in (WD style sample) | PasswordBox (watermark / clear / monitor / plain text toggle) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/PasswordBoxExample.xaml) |
| [ComboBox](basic-controls/combo-box.md) | WPF built-in (WD style sample) | ComboBox (watermark / editable / corner radius) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/ComboBoxExample.xaml) |
| [CheckBox](basic-controls/check-box.md) | WPF built-in (WD style sample) | Checkbox · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/CheckboxExample.xaml) |
| [RadioButton](basic-controls/radio-button.md) | WPF built-in (WD style sample) | Radio button · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/RadioButtonExample.xaml) |
| [ToggleButton](basic-controls/toggle-button.md) | WPF built-in (WD style sample) | Toggle button · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/ToggleButtonExample.xaml) |
| [DatePicker](basic-controls/date-picker.md) | WPF built-in (WD style sample) | Date picker (with time selection) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/DatePickerExample.xaml) |
| [DateTimePicker](basic-controls/date-time-picker.md) | WPF built-in (WD style sample) | DateTime picker · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/DateTimePickerExample.xaml) |
| [Slider](basic-controls/slider.md) | WPF built-in (WD style sample) | Slider (horizontal / vertical) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/SliderExample.xaml) |
| [ProgressBar](basic-controls/progress-bar.md) | WPF built-in (WD style sample) | Progress bar (striped / indeterminate / vertical) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/ProgressBarExample.xaml) |
| [DataGrid](basic-controls/data-grid.md) | WPF built-in (WD style sample) | Data grid (row header selection / template columns / grid lines) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/DataGridExample.xaml) |
| [ListBox](basic-controls/list-box.md) | WPF built-in (WD style sample) | List box · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/ListBoxExample.xaml) |
| [ListView](basic-controls/list-view.md) | WPF built-in (WD style sample) | List view (GridView) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/ListViewExample.xaml) |
| [TreeView](basic-controls/tree-view.md) | WPF built-in (WD style sample) | Tree view (scroll animation) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/TreeViewExample.xaml) |
| [Expander](basic-controls/expander.md) | WPF built-in (WD style sample) | Expander (4 directions) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/ExpanderExample.xaml) |
| [GroupBox](basic-controls/group-box.md) | WPF built-in (WD style sample) | Group box · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/GroupBoxExample.xaml) |
| [TabControl](basic-controls/tab-control.md) | WPF built-in (WD style sample) | Tab control (4 directions / closable) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/TabControlExample.xaml) |
| [Menu](basic-controls/menu.md) | WPF built-in (WD style sample) | Menu bar · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/MenuExample.xaml) |
| [wd:SplitButton](basic-controls/split-button.md) | NuGet | Split button (main button + dropdown menu, supports text/icon options) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SplitButtonExample.xaml) |
| [Calendar](basic-controls/calendar.md) | WPF built-in (WD style sample) | Calendar · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/CalendarExample.xaml) |
| [wd:Segmented](basic-controls/segmented.md) | NuGet | Segmented control (multi-option toggle) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SegmentedExample.xaml) |

### Input Controls { #input-controls }

| Control Guide | Source | Description & Example |
|---|---|---|
| [wd:ColorPicker](input-controls/color-picker.md) | NuGet | Color picker · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ColorPickerExample.xaml) |
| [wd:TimePicker](input-controls/time-picker.md) | NuGet | Time picker · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/TimePickerExample.xaml) |
| [wd:DateRangePicker](input-controls/date-range-picker.md) | NuGet | Date range picker · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DateRangePickerExample.xaml) |
| [wd:NumericBox](input-controls/numeric-box.md) | NuGet | Numeric input · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/NumericBoxExample.xaml) |
| [wd:IPEditBox](input-controls/ip-edit-box.md) | NuGet | IP address input · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/IPEditBoxExample.xaml) |
| [wd:MultiSelectComboBox](input-controls/multi-select-combo-box.md) | NuGet | Multi-select combo box · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MultiSelectComboBoxExample.xaml) |
| [MultiSelectSearchComboBox](input-controls/multi-select-search-combo-box.md) | Sample-only | Searchable multi-select combo box · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MultiSelectSearchComboBoxExample.xaml) |
| [Password](input-controls/password.md) | Sample-only | Composed password show/hide sample · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Passwrod/PasswordExample.xaml) |
| [wd:VerifyCode](input-controls/verify-code.md) | NuGet | CAPTCHA drawing · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/VerifyCodeExample.xaml) |
| [RoundPicker](input-controls/round-picker.md) | Sample-only | Circular color picker · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/RoundPickerExample.xaml) |
| [wd:RulerControl](input-controls/ruler-control.md) | NuGet | Ruler control · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/RulerExample.xaml) |
| [wd:ScrollIntoView](input-controls/scroll-into-view.md) | NuGet | List positioning helper · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SelectorExample.xaml) |
| [wd:Dial](input-controls/dial.md) | NuGet | Dial pad · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DialExample.xaml) |
| [wd:GestureUnlock](input-controls/gesture-unlock.md) | NuGet | Gesture pattern unlock · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/GestureUnlockExample.xaml) |
| [wd:SvgViewer](input-controls/svg-viewer.md) | NuGet | SVG viewer · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SvgViewerExample.xaml) |
| [wd:OtpBox](input-controls/otp-box.md) | NuGet | OTP verification code input (auto-jump / backspace / paste) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/OtpBoxExample.xaml) |

### Loading Animations { #loading-animations }

| Control Guide | Source | Description & Example |
|---|---|---|
| [wd:RingLoading](loading-animations/ring-loading.md) | NuGet | Ring loading animation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/RingLoadingExample.xaml) |
| [wd:BallLoading](loading-animations/ball-loading.md) | NuGet | Bouncing ball animation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/BallLoadingExample.xaml) |
| [StreamerLoading](loading-animations/streamer-loading.md) | Sample-only | Streamer animation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/StreamerLoadingExample.xaml) |
| [wd:WaitLoading](loading-animations/wait-loading.md) | NuGet | Wait animation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/WaitLoadingExample.xaml) |
| [wd:CycleLoading](loading-animations/cycle-loading.md) | NuGet | Cycle animation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/CycleLoadingExample.xaml) |
| [wd:RollLoading](loading-animations/roll-loading.md) | NuGet | Rolling animation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/RollLoadingExample.xaml) |
| [wd:Loading (attached property)](loading-animations/loading.md) | NuGet | Button/control loading state · [Example folder](https://github.com/WPFDevelopersOrg/WPFDevelopers/tree/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading) |

### Charts & Data Visualization { #charts-data-visualization }

| Control Guide | Source | Description & Example |
|---|---|---|
| [wd:ChartRadar](charts-data-visualization/chart-radar.md) | NuGet | Radar chart · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ChartRadarExample.xaml) |
| [wd:ChartBar](charts-data-visualization/chart-bar.md) | NuGet | Bar chart · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ChartBarExample.xaml) |
| [wd:ChartLine](charts-data-visualization/chart-line.md) | NuGet | Line chart · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ChartLineExample.xaml) |
| [wd:ChartPie](charts-data-visualization/chart-pie.md) | NuGet | Pie chart · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ChartPieExample.xaml) |
| [Dashboard](charts-data-visualization/dashboard.md) | Sample-only | Dashboard gauge (tick marks follow progress) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DashboardExample.xaml) |
| [PieSegment](charts-data-visualization/pie-segment.md) | Sample-only | Segmented pie chart sample (distinct from wd:ChartPie) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/PieSegmentExample.xaml) |
| [wd:Gauge](charts-data-visualization/gauge.md) | NuGet | Gauge control · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/GaugeExample.xaml) |
| [LineChart](charts-data-visualization/line-chart.md) | Sample-only | Line chart (alternate implementation) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/LineChartExample.xaml) |
| [wd:CircleProgressBar](charts-data-visualization/circle-progress-bar.md) | NuGet | Circular progress bar · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CircleProgressBarExample.xaml) |
| [wd:Step](charts-data-visualization/step.md) | NuGet | Step wizard · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/StepExample.xaml) |
| [wd:BreadCrumbBar](charts-data-visualization/bread-crumb-bar.md) | NuGet | Breadcrumb navigation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BreadCrumbBarExample.xaml) |
| [wd:Pagination](charts-data-visualization/pagination.md) | NuGet | Pagination control · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/PaginationExample.xaml) |
| [ActivityFeed](charts-data-visualization/activity-feed.md) | Sample-only | Activity feed (Gitee-style) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ActivityFeedExample.xaml) |
| [wd:Thermometer](charts-data-visualization/thermometer.md) | NuGet | Thermometer · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ThermometerExample.xaml) |
| [wd:DataGridFilter](charts-data-visualization/data-grid-filter.md) | NuGet | DataGrid column filter engine · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DataGridFilterExample.xaml) |

### Layout & Panels { #layouts-panels }

| Control Guide | Source | Description & Example |
|---|---|---|
| [wd:Carousel](layouts-panels/carousel.md) | NuGet | Carousel (auto-play / dot indicators / arrows / click navigation) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CarouselExample.xaml) |
| [wd:FocusCarousel](layouts-panels/focus-carousel.md) | NuGet | Emphasizer carousel (3D flip + zoom effect) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/FocusCarouselExample.xaml) |
| [wd:CardCarousel](layouts-panels/card-carousel.md) | NuGet | Master carousel (multi-layer overlay animation) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CardCarouselExample.xaml) |
| [wd:CircleMenu](layouts-panels/circle-menu.md) | NuGet | Circular menu · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CircleMenuExample.xaml) |
| [wd:SixGridView](layouts-panels/six-grid-view.md) | NuGet | Six-column grid layout · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SixGirdViewExample.xaml) |
| [wd:WaterfallPanel](layouts-panels/waterfall-panel.md) | NuGet | Waterfall / masonry panel · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/WaterfallPanelExample.xaml) |
| [wd:WaterfallItemsControl](layouts-panels/waterfall-items-control.md) | NuGet | Masonry items control with a scrolling template · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/WaterfallPanelExample.xaml) |
| [wd:VirtualizingWrapPanel](layouts-panels/virtualizing-wrap-panel.md) | NuGet | Virtualizing WrapPanel · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/VirtualizingWrapPanelExample.xaml) |
| [TransformLayout](layouts-panels/transform-layout.md) | Sample-only | Draggable, resizable, rotatable control · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/TransformLayoutExample.xaml) |
| [wd:DrapView](layouts-panels/drap-view.md) | NuGet | Drag-and-drop view · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrapView/DrapViewExample.xaml) |
| [wd:Spacing (attached property)](layouts-panels/spacing.md) | NuGet | Panel child spacing · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SpacingExample.xaml) |
| [wd:PanningItems](layouts-panels/panning-items.md) | NuGet | Panning control (touch swipe) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/PanningItemsExample.xaml) |
| [wd:WDScrollViewer](layouts-panels/wd-scroll-viewer.md) | NuGet | A ScrollViewer control with smooth scrolling animation. · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ScrollViewerAnimationExample.xaml) |
| [wd:ScrollViewerBehavior](layouts-panels/scroll-viewer-behavior.md) | NuGet | Attached scroll-viewer behavior for animation and back-to-top support. · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ScrollViewerAnimationExample.xaml) |
| [wd:IconicThumbnail](layouts-panels/iconic-thumbnail.md) | NuGet | Iconic thumbnail · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/IconicThumbnailExample.xaml) |
| [GridSplitter](layouts-panels/grid-splitter.md) | WPF built-in (WD theme style) | The standard WPF grid splitter using the WD theme style to resize rows or columns. · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/GridSplitterExample.xaml) |

### Animation & Effects { #animation-effects }

| Control Guide | Source | Description & Example |
|---|---|---|
| [wd:BreatheLight](animation-effects/breathe-light.md) | NuGet | Breathing light animation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BreatheLightExample.xaml) |
| [wd:SpotLight](animation-effects/spot-light.md) | NuGet | Spotlight effect · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SpotLightExample.xaml) |
| [wd:EdgeLight](animation-effects/edge-light.md) | NuGet | Edge marquee light · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/EdgeLightExample.xaml) |
| [RainbowButtons](animation-effects/rainbow-buttons.md) | Sample-only | Rainbow buttons · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/RainbowButtonsExample.xaml) |
| [wd:Shake](animation-effects/shake.md) | NuGet | Window shake · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ShakeExample.xaml) |
| [BubbleSelection](animation-effects/bubble-selection.md) | Sample-only | Bubble selection sample · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BubbleSelectionExample.xaml) |
| [wd:StarrySky](animation-effects/starry-sky.md) | NuGet | Starry sky animation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/StarrySkyExample.xaml) |
| [SnowCanvas](animation-effects/snow-canvas.md) | Sample-only | Christmas tree & snow canvas · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SnowCanvasExample.xaml) |
| [SpeedRockets](animation-effects/speed-rockets.md) | Sample-only | Speed rocket animation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SpeedRockets/SpeedRocketsExample.xaml) |
| [CountdownTimer](animation-effects/countdown-timer.md) | Sample-only | Countdown timer animation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CountdownTimerExample.xaml) |
| [NumberCard](animation-effects/number-card.md) | Sample-only | 3D flip countdown cards · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/NumberCard/NumberCardExample.xaml) |
| [wd:AnimationGrid](animation-effects/animation-grid.md) | NuGet | Animation grid · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/AnimationGridExample.xaml) |
| [LogoAnimation](animation-effects/logo-animation.md) | Sample-only | Login logo animation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/LogoAnimationExample.xaml) |
| [SongWords](animation-effects/song-words.md) | Sample-only | Lyrics scroll animation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SongWordsExample.xaml) |
| [wd:AnimationAudio](animation-effects/animation-audio.md) | NuGet | Audio waveform visualization · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/AnimationAudioExample.xaml) |
| [Barrage](animation-effects/barrage.md) | Sample-only | Danmaku / barrage control · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BarrageExample.xaml) |
| [CanvasHandWriting](animation-effects/canvas-hand-writing.md) | Sample-only | Smooth canvas handwriting · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CanvasHandWriting/CanvasHandWritingExample.xaml) |
| [Drawing](animation-effects/drawing.md) | Sample-only | Freehand drawing · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrawingExample.xaml) |
| [DrawPrize](animation-effects/draw-prize.md) | Sample-only | Lottery wheel · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrawPrizeExample.xaml) |

### Effects & Filters { #effects-filters }

| Control Guide | Source | Description & Example |
|---|---|---|
| [wd:GrayscaleEffect](effects-filters/grayscale-effect.md) | NuGet | Grayscale filter effect (adjustable intensity) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/GrayscaleEffectExample.xaml) |

### Media & Image Processing { #media-image-processing }

| Control Guide | Source | Description & Example |
|---|---|---|
| [wd:ScreenCut](media-image-processing/screen-cut.md) | NuGet | Screen capture (pen / arrow annotation) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ScreenCutExample.xaml) |
| [wd:CropImage](media-image-processing/crop-image.md) | NuGet | Image cropping · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CropImageExample.xaml) |
| [wd:CropAvatar](media-image-processing/crop-avatar.md) | NuGet | Avatar cropping selector · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CropAvatar/CropAvatarExample.xaml) |
| [ImageCrop](media-image-processing/image-crop.md) | Sample-only | Image nine-grid tiling effect sample · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ImageCropExample.xaml) |
| [CutImage](media-image-processing/cut-image.md) | Sample-only | User avatar cropping solution · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CutImageExample.xaml) |
| [wd:Magnifier](media-image-processing/magnifier.md) | NuGet | Magnifier · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MagnifierExample.xaml) |

### Notifications & Messages { #feedback-messages }

| Control Guide | Source | Description & Example |
|---|---|---|
| [wd:Badge (attached property)](feedback-messages/badge.md) | NuGet | Badge notification · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BadgeExample.xaml) |
| [wd:Toast](feedback-messages/toast.md) | NuGet | Toast message popup · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ToastExample.xaml) |
| [wd:Tag](feedback-messages/tag.md) | NuGet | Tag control · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/TagExample.xaml) |
| [wd:PathIcon](feedback-messages/path-icon.md) | NuGet | Vector path icon · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/PathIconExample.xaml) |
| [wd:AllPathIcon](feedback-messages/all-path-icon.md) | NuGet | Built-in icon browser · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/AllPathIconExample.xaml) |
| [MessageBox (static class)](feedback-messages/message-box.md) | NuGet | Message dialog · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MessageBoxExample.xaml) |
| [Dialog](feedback-messages/dialog.md) | NuGet | A general-purpose dialog for custom content, input validation, and MVVM workflows. · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Dialog/DialogExample.xaml) |

### Other Controls { #other-controls }

| Control Guide | Source | Description & Example |
|---|---|---|
| [ZooSemy](other-controls/zoo-semy.md) | Sample-only | Skeuomorphic rotary knob (volume) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ZooSemy/ZooSemyExample.xaml) |
| [Flame](other-controls/flame.md) | Sample-only | Flame animation · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/FlameExample.xaml) |
| [Desktop](other-controls/desktop.md) | Sample-only | Dynamic desktop wallpaper · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Desktop/DesktopBackground.xaml) |
| [AMap](other-controls/a-map.md) | Sample-only | Map integration (Bing / AutoNavi) · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Map/BingAMapExample.xaml) |
| [LoginWindow](other-controls/login-window.md) | Sample-only | Login window template · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/LoginWindow/LoginExample.xaml) |
| [ChatEmoji](other-controls/chat-emoji.md) | Sample-only | Emoji + text chat · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ChatEmojiExample.xaml) |
| [wd:MarkdownViewer](other-controls/markdown-viewer.md) | NuGet | Renders Markdown as rich text in WPF, useful for guides, README content, and logs. · [Example source](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MarkdownViewerExample.xaml) |
