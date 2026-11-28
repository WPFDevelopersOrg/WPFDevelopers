# 控件分类与单项说明

WPFDevelopers 的 108 个控件、附加属性和示例项按功能分类。点击控件名称查看单项说明，示例链接直达对应源码。

<div class="wd-control-search" role="search" data-language="zh">
  <label class="wd-control-search__label" for="wd-control-filter">搜索控件</label>
  <input id="wd-control-filter" type="search" placeholder="输入控件名称、分类或用途，例如 Button、日期、导航" autocomplete="off">
  <div class="wd-control-search__dropdown">
    <p id="wd-control-search-status" aria-live="polite">输入关键字筛选控件，并点击结果定位到对应分类。</p>
    <ol id="wd-control-search-results" class="wd-control-search__results"></ol>
  </div>
</div>

## 控件总览 { #control-catalog }

目录覆盖 NuGet 控件、附加属性和示例专属项目。搜索可匹配名称、分类、来源及功能描述。

### 窗口与导航 { #window-navigation }

| 控件说明 | 来源 | 功能与示例源码 |
|---|---|---|
| [wd:Window](window-navigation/window.md) | NuGet | 自定义窗体（ToolWindow/NoneTitleBar/Normal/HighTitleBar） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MainWindow.xaml) |
| [wd:NotifyIcon](window-navigation/notify-icon.md) | NuGet | 系统托盘图标 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/NotifyIconExample.xaml) |
| [wd:DrawerMenu](window-navigation/drawer-menu.md) | NuGet | Win10 风格抽屉导航菜单 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrawerMenu/DrawerMenuExample.xaml) |
| [wd:SidePanel](window-navigation/side-panel.md) | NuGet | 可放置自定义内容的可折叠侧栏 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SidePanelExample.xaml) |
| [wd:NavMenu3D](window-navigation/nav-menu3-d.md) | NuGet | 3D 动画导航菜单 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/NavMenu3DExample.xaml) |
| [wd:NavScrollPanel](window-navigation/nav-scroll-panel.md) | NuGet | Win10 设置风格导航面板 · [示例目录](https://github.com/WPFDevelopersOrg/WPFDevelopers/tree/master/src/WPFDevelopers.Samples.Shared/ExampleViews/NavScrollPanel) |
| [TransitionPanel](window-navigation/transition-panel.md) | 示例专属 | 过渡动画面板（定义在 Samples.Shared/Controls/） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/TransitionPanelExample.xaml) |
| [wd:Drawer](window-navigation/drawer.md) | NuGet | 滑出式面板（上/下/左/右） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrawerExample.xaml) |
| [wd:Mask](window-navigation/mask.md) | NuGet | 遮罩层（窗体级/控件级） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MaskExample.xaml) |
| [wd:AcrylicBlur](window-navigation/acrylic-blur.md) | NuGet | 亚克力模糊窗体 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/AcrylicBlurExample.xaml) |
| [TaskbarItemInfo](window-navigation/taskbar-item-info.md) | 示例专属 | 任务栏角徽 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/TaskbarItemInfoExample.xaml) |

### 基础控件与样式 { #basic-controls }

| 控件说明 | 来源 | 功能与示例源码 |
|---|---|---|
| [wd:WDBorder](basic-controls/wd-border.md) | NuGet | 继承 Border，提供 ContentClip 圆角裁剪几何供子元素使用 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Net40/Styles/Styles.TreeView.xaml) |
| [wd:SplitButton](basic-controls/split-button.md) | NuGet | 分割按钮（主按钮+下拉菜单，支持文本/图标选项） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SplitButtonExample.xaml) |
| [wd:Segmented](basic-controls/segmented.md) | NuGet | 分段控制器（多选项切换） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SegmentedExample.xaml) |

### 输入控件 { #input-controls }

| 控件说明 | 来源 | 功能与示例源码 |
|---|---|---|
| [wd:ColorPicker](input-controls/color-picker.md) | NuGet | 颜色选择器 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ColorPickerExample.xaml) |
| [wd:TimePicker](input-controls/time-picker.md) | NuGet | 时间选择器 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/TimePickerExample.xaml) |
| [wd:DateRangePicker](input-controls/date-range-picker.md) | NuGet | 日期范围选择器 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DateRangePickerExample.xaml) |
| [wd:NumericBox](input-controls/numeric-box.md) | NuGet | 数字输入框 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/NumericBoxExample.xaml) |
| [wd:IPEditBox](input-controls/ip-edit-box.md) | NuGet | IP 地址输入框 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/IPEditBoxExample.xaml) |
| [wd:MultiSelectComboBox](input-controls/multi-select-combo-box.md) | NuGet | 下拉多选框 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MultiSelectComboBoxExample.xaml) |
| [MultiSelectSearchComboBox](input-controls/multi-select-search-combo-box.md) | 示例专属 | 下拉搜索多选框 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MultiSelectSearchComboBoxExample.xaml) |
| [Password](input-controls/password.md) | 示例专属 | 密码显示/隐藏组合示例 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Passwrod/PasswordExample.xaml) |
| [wd:VerifyCode](input-controls/verify-code.md) | NuGet | 验证码绘制 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/VerifyCodeExample.xaml) |
| [RoundPicker](input-controls/round-picker.md) | 示例专属 | 圆形颜色选择器 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/RoundPickerExample.xaml) |
| [wd:RulerControl](input-controls/ruler-control.md) | NuGet | 刻度尺控件 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/RulerExample.xaml) |
| [wd:ScrollIntoView](input-controls/scroll-into-view.md) | NuGet | 列表定位辅助功能 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SelectorExample.xaml) |
| [wd:Dial](input-controls/dial.md) | NuGet | 拨号盘控件 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DialExample.xaml) |
| [wd:GestureUnlock](input-controls/gesture-unlock.md) | NuGet | 手势解锁 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/GestureUnlockExample.xaml) |
| [wd:SvgViewer](input-controls/svg-viewer.md) | NuGet | SVG 查看器 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SvgViewerExample.xaml) |
| [wd:OtpBox](input-controls/otp-box.md) | NuGet | OTP 验证码输入框（自动跳转/退格/粘贴） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/OtpBoxExample.xaml) |

### 加载动画 { #loading-animations }

| 控件说明 | 来源 | 功能与示例源码 |
|---|---|---|
| [wd:RingLoading](loading-animations/ring-loading.md) | NuGet | 环形加载动画 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/RingLoadingExample.xaml) |
| [wd:BallLoading](loading-animations/ball-loading.md) | NuGet | 弹球加载动画 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/BallLoadingExample.xaml) |
| [StreamerLoading](loading-animations/streamer-loading.md) | 示例专属 | 流光加载动画 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/StreamerLoadingExample.xaml) |
| [wd:WaitLoading](loading-animations/wait-loading.md) | NuGet | 等待加载动画 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/WaitLoadingExample.xaml) |
| [wd:CycleLoading](loading-animations/cycle-loading.md) | NuGet | 循环加载动画 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/CycleLoadingExample.xaml) |
| [wd:RollLoading](loading-animations/roll-loading.md) | NuGet | 滚动加载动画 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading/RollLoadingExample.xaml) |
| [wd:Loading](loading-animations/loading.md) | NuGet | 按钮/控件加载状态 · [示例目录](https://github.com/WPFDevelopersOrg/WPFDevelopers/tree/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Loading) |

### 图表与数据可视化 { #charts-data-visualization }

| 控件说明 | 来源 | 功能与示例源码 |
|---|---|---|
| [wd:ChartRadar](charts-data-visualization/chart-radar.md) | NuGet | 雷达图 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ChartRadarExample.xaml) |
| [wd:ChartBar](charts-data-visualization/chart-bar.md) | NuGet | 柱状图 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ChartBarExample.xaml) |
| [wd:ChartLine](charts-data-visualization/chart-line.md) | NuGet | 折线图 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ChartLineExample.xaml) |
| [wd:ChartPie](charts-data-visualization/chart-pie.md) | NuGet | 饼图 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ChartPieExample.xaml) |
| [Dashboard](charts-data-visualization/dashboard.md) | 示例专属 | 仪表盘（刻度跟随进度） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DashboardExample.xaml) |
| [PieSegment](charts-data-visualization/pie-segment.md) | 示例专属 | 分段饼图示例（区别于 wd:ChartPie） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/PieSegmentExample.xaml) |
| [wd:Gauge](charts-data-visualization/gauge.md) | NuGet | 仪表盘 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/GaugeExample.xaml) |
| [LineChart](charts-data-visualization/line-chart.md) | 示例专属 | 折线图（另一实现） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/LineChartExample.xaml) |
| [wd:CircleProgressBar](charts-data-visualization/circle-progress-bar.md) | NuGet | 环形进度条 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CircleProgressBarExample.xaml) |
| [wd:Step](charts-data-visualization/step.md) | NuGet | 步骤条向导 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/StepExample.xaml) |
| [wd:BreadCrumbBar](charts-data-visualization/bread-crumb-bar.md) | NuGet | 面包屑导航栏 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BreadCrumbBarExample.xaml) |
| [wd:Pagination](charts-data-visualization/pagination.md) | NuGet | 分页控件 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/PaginationExample.xaml) |
| [ActivityFeed](charts-data-visualization/activity-feed.md) | 示例专属 | 活动记录（仿 Gitee） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ActivityFeedExample.xaml) |
| [wd:Thermometer](charts-data-visualization/thermometer.md) | NuGet | 温度计 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ThermometerExample.xaml) |
| [wd:DataGridFilter](charts-data-visualization/data-grid-filter.md) | NuGet | DataGrid 列过滤引擎 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DataGridFilterExample.xaml) |

### 布局与面板 { #layouts-panels }

| 控件说明 | 来源 | 功能与示例源码 |
|---|---|---|
| [wd:Carousel](layouts-panels/carousel.md) | NuGet | 轮播图（自动播放/圆点指示器/箭头/点击跳转） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CarouselExample.xaml) |
| [wd:FocusCarousel](layouts-panels/focus-carousel.md) | NuGet | 强调型轮播图（3D 翻转 + 缩放效果） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/FocusCarouselExample.xaml) |
| [wd:CardCarousel](layouts-panels/card-carousel.md) | NuGet | 高级轮播图（多层叠加动画） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CardCarouselExample.xaml) |
| [wd:CircleMenu](layouts-panels/circle-menu.md) | NuGet | 圆形菜单 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CircleMenuExample.xaml) |
| [wd:SixGridView](layouts-panels/six-grid-view.md) | NuGet | 六宫格布局 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SixGirdViewExample.xaml) |
| [wd:WaterfallPanel](layouts-panels/waterfall-panel.md) | NuGet | 瀑布流面板 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/WaterfallPanelExample.xaml) |
| [wd:WaterfallItemsControl](layouts-panels/waterfall-items-control.md) | NuGet | 内置滚动模板的瀑布流集合控件 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/WaterfallPanelExample.xaml) |
| [wd:VirtualizingWrapPanel](layouts-panels/virtualizing-wrap-panel.md) | NuGet | 虚拟化 WrapPanel · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/VirtualizingWrapPanelExample.xaml) |
| [TransformLayout](layouts-panels/transform-layout.md) | 示例专属 | 可拖拽旋转缩放控件 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/TransformLayoutExample.xaml) |
| [wd:DrapView](layouts-panels/drap-view.md) | NuGet | 拖拽视图控件 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrapView/DrapViewExample.xaml) |
| [wd:Spacing](layouts-panels/spacing.md) | NuGet | 面板子元素间距 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SpacingExample.xaml) |
| [wd:PanningItems](layouts-panels/panning-items.md) | NuGet | 平移控件 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/PanningItemsExample.xaml) |
| [wd:WDScrollViewer](layouts-panels/wd-scroll-viewer.md) | NuGet | 带平滑滚动动画的 ScrollViewer 控件。 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ScrollViewerAnimationExample.xaml) |
| [wd:ScrollViewerBehavior](layouts-panels/scroll-viewer-behavior.md) | NuGet | ScrollViewer 的动画与辅助行为，包括回到顶部功能。 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ScrollViewerAnimationExample.xaml) |
| [wd:IconicThumbnail](layouts-panels/iconic-thumbnail.md) | NuGet | 图标缩略图 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/IconicThumbnailExample.xaml) |
| [GridSplitter](layouts-panels/grid-splitter.md) | WPF 标准控件（WD 主题样式） | 使用 WD 主题样式的标准 WPF 网格分隔条，支持拖动调整行列尺寸。 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Basics/GridSplitterExample.xaml) |

### 动画与特效 { #animation-effects }

| 控件说明 | 来源 | 功能与示例源码 |
|---|---|---|
| [wd:BreatheLight](animation-effects/breathe-light.md) | NuGet | 呼吸灯动画 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BreatheLightExample.xaml) |
| [wd:SpotLight](animation-effects/spot-light.md) | NuGet | 聚光灯效果 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SpotLightExample.xaml) |
| [wd:EdgeLight](animation-effects/edge-light.md) | NuGet | 边框跑马灯 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/EdgeLightExample.xaml) |
| [RainbowButtons](animation-effects/rainbow-buttons.md) | 示例专属 | 彩虹按钮 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/RainbowButtonsExample.xaml) |
| [wd:Shake](animation-effects/shake.md) | NuGet | 窗体抖动 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ShakeExample.xaml) |
| [BubbleSelection](animation-effects/bubble-selection.md) | 示例专属 | 气泡选择示例 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BubbleSelectionExample.xaml) |
| [wd:StarrySky](animation-effects/starry-sky.md) | NuGet | 星空动画 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/StarrySkyExample.xaml) |
| [SnowCanvas](animation-effects/snow-canvas.md) | 示例专属 | 圣诞树雪花 Canvas · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SnowCanvasExample.xaml) |
| [SpeedRockets](animation-effects/speed-rockets.md) | 示例专属 | 加速火箭动画 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SpeedRockets/SpeedRocketsExample.xaml) |
| [CountdownTimer](animation-effects/countdown-timer.md) | 示例专属 | 倒计时动画 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CountdownTimerExample.xaml) |
| [NumberCard](animation-effects/number-card.md) | 示例专属 | 3D 翻转倒计时卡片 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/NumberCard/NumberCardExample.xaml) |
| [wd:AnimationGrid](animation-effects/animation-grid.md) | NuGet | 动画网格 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/AnimationGridExample.xaml) |
| [LogoAnimation](animation-effects/logo-animation.md) | 示例专属 | 登录 Logo 动画 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/LogoAnimationExample.xaml) |
| [SongWords](animation-effects/song-words.md) | 示例专属 | 歌词滚动动画 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/SongWordsExample.xaml) |
| [wd:AnimationAudio](animation-effects/animation-audio.md) | NuGet | 音频波形可视化 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/AnimationAudioExample.xaml) |
| [Barrage](animation-effects/barrage.md) | 示例专属 | 弹幕控件 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BarrageExample.xaml) |
| [CanvasHandWriting](animation-effects/canvas-hand-writing.md) | 示例专属 | Canvas 平滑笔迹绘制 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CanvasHandWriting/CanvasHandWritingExample.xaml) |
| [Drawing](animation-effects/drawing.md) | 示例专属 | 自由手绘 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrawingExample.xaml) |
| [DrawPrize](animation-effects/draw-prize.md) | 示例专属 | 大转盘抽奖 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/DrawPrizeExample.xaml) |

### 特效与滤镜 { #effects-filters }

| 控件说明 | 来源 | 功能与示例源码 |
|---|---|---|
| [wd:GrayscaleEffect](effects-filters/grayscale-effect.md) | NuGet | 灰度滤镜效果（可控制灰度强度） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/GrayscaleEffectExample.xaml) |

### 媒体与图像处理 { #media-image-processing }

| 控件说明 | 来源 | 功能与示例源码 |
|---|---|---|
| [wd:ScreenCut](media-image-processing/screen-cut.md) | NuGet | 截屏工具（画笔/箭头标注） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ScreenCutExample.xaml) |
| [wd:CropImage](media-image-processing/crop-image.md) | NuGet | 图片裁剪 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CropImageExample.xaml) |
| [wd:CropAvatar](media-image-processing/crop-avatar.md) | NuGet | 头像裁剪选择器 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CropAvatar/CropAvatarExample.xaml) |
| [ImageCrop](media-image-processing/image-crop.md) | 示例专属 | 图片九宫格分块效果示例 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ImageCropExample.xaml) |
| [CutImage](media-image-processing/cut-image.md) | 示例专属 | 用户头像裁剪方案 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/CutImageExample.xaml) |
| [wd:Magnifier](media-image-processing/magnifier.md) | NuGet | 放大镜 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MagnifierExample.xaml) |

### 提示与消息 { #feedback-messages }

| 控件说明 | 来源 | 功能与示例源码 |
|---|---|---|
| [wd:Badge](feedback-messages/badge.md) | NuGet | 角标通知 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/BadgeExample.xaml) |
| [wd:Toast](feedback-messages/toast.md) | NuGet | 轻提示/Toast 消息 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ToastExample.xaml) |
| [wd:Tag](feedback-messages/tag.md) | NuGet | 标签控件 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/TagExample.xaml) |
| [wd:PathIcon](feedback-messages/path-icon.md) | NuGet | 矢量路径图标 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/PathIconExample.xaml) |
| [wd:AllPathIcon](feedback-messages/all-path-icon.md) | NuGet | 全部内置图标浏览 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/AllPathIconExample.xaml) |
| [MessageBox](feedback-messages/message-box.md) | NuGet | 消息对话框 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MessageBoxExample.xaml) |
| [Dialog](feedback-messages/dialog.md) | NuGet | 用于显示自定义内容的通用弹窗，支持输入校验和 MVVM 用法。 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Dialog/DialogExample.xaml) |

### 其他控件 { #other-controls }

| 控件说明 | 来源 | 功能与示例源码 |
|---|---|---|
| [ZooSemy](other-controls/zoo-semy.md) | 示例专属 | 拟物旋转按钮（音量旋钮） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ZooSemy/ZooSemyExample.xaml) |
| [Flame](other-controls/flame.md) | 示例专属 | 火苗动画 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/FlameExample.xaml) |
| [Desktop](other-controls/desktop.md) | 示例专属 | 动态桌面壁纸（视频播放） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Desktop/DesktopBackground.xaml) |
| [AMap](other-controls/a-map.md) | 示例专属 | 地图集成（Bing/高德） · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/Map/BingAMapExample.xaml) |
| [LoginWindow](other-controls/login-window.md) | 示例专属 | 登录窗口模板 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/LoginWindow/LoginExample.xaml) |
| [ChatEmoji](other-controls/chat-emoji.md) | 示例专属 | Emoji + 文本聊天控件 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/ChatEmojiExample.xaml) |
| [wd:MarkdownViewer](other-controls/markdown-viewer.md) | NuGet | 将 Markdown 文本渲染为 WPF 富文本，适合展示说明文档、README、日志等内容。 · [示例源码](https://github.com/WPFDevelopersOrg/WPFDevelopers/blob/master/src/WPFDevelopers.Samples.Shared/ExampleViews/MarkdownViewerExample.xaml) |
