using Microsoft.Expression.Drawing.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPFDevelopers;
using WPFDevelopers.Helpers;
using WPFDevelopers.Sample.ExampleViews;
using WPFDevelopers.Samples.ExampleViews;
using WPFDevelopers.Samples.ExampleViews.Basics;
using WPFDevelopers.Samples.ExampleViews.CanvasHandWriting;
using WPFDevelopers.Samples.ExampleViews.DrawerMenu;
using WPFDevelopers.Samples.ExampleViews.LoginWindow;
using WPFDevelopers.Samples.ExampleViews.NumberCard;
using WPFDevelopers.Samples.ExampleViews.Passwrod;
using WPFDevelopers.Samples.Helpers;
using WPFDevelopers.Samples.Models;
using VirtualizingWrapPanel = WPFDevelopers.Samples.ExampleViews.VirtualizingWrapPanel;

namespace WPFDevelopers.Samples.ViewModels
{
    public class MainVM : ViewModelBase
    {
        private static readonly HashSet<string> BasicEnumSet = new HashSet<string>
        {
            nameof(MenuEnum.Theme),
            nameof(MenuEnum.Menu),
            nameof(MenuEnum.Window),
            nameof(MenuEnum.BasicLoading),
            nameof(MenuEnum.Button),
            nameof(MenuEnum.RadioButton),
            nameof(MenuEnum.Checkbox),
            nameof(MenuEnum.TextBox),
            nameof(MenuEnum.PasswordBox),
            nameof(MenuEnum.ComboBox),
            nameof(MenuEnum.ToggleButton),
            nameof(MenuEnum.DatePicker),
            nameof(MenuEnum.DateTimePicker),
            nameof(MenuEnum.Calendar),
            nameof(MenuEnum.Slider),
            nameof(MenuEnum.GridSplitter),
            nameof(MenuEnum.ProgressBar),
            nameof(MenuEnum.DataGrid),
            nameof(MenuEnum.ListBox),
            nameof(MenuEnum.ListView),
            nameof(MenuEnum.TreeView),
            nameof(MenuEnum.Expander),
            nameof(MenuEnum.GroupBox),
            nameof(MenuEnum.TabControl),
            nameof(MenuEnum.TaskbarInfo),
        };

        private static readonly HashSet<MenuEnum> DemoMenuItems = new HashSet<MenuEnum>
        {
            MenuEnum.StreamerLoading,
            MenuEnum.TransformLayout,
            MenuEnum.ChatEmoji,
            MenuEnum.Dashboard,
            MenuEnum.PieSegment,
            MenuEnum.Password,
            MenuEnum.SongWords,
            MenuEnum.ActivityFeed,
            MenuEnum.Flame,
            MenuEnum.TransitionPanel,
            MenuEnum.ZooSemy,
            MenuEnum.RainbowBtn,
            MenuEnum.RoundPicker,
            MenuEnum.LineChart,
            MenuEnum.SnowCanvas,
            MenuEnum.LoginWindow,
            MenuEnum.Drawing,
            MenuEnum.SpeedRockets,
            MenuEnum.CountdownTimer,
            MenuEnum.NumberCard,
            MenuEnum.ImageCrop,
            MenuEnum.Desktop,
            MenuEnum.DrawPrize,
            MenuEnum.BubbleSelection,
            MenuEnum.CanvasHandWriting,
            MenuEnum.Barrage,
            MenuEnum.SixGridView,
            MenuEnum.Drap,
            MenuEnum.TaskbarInfo,
        };

        private static readonly HashSet<MenuEnum> NewMenuItems = new HashSet<MenuEnum>
        {
            MenuEnum.ScrollNavigation,
            MenuEnum.Timeline,
        };

        private static readonly HashSet<MenuEnum> UpdatedMenuItems = new HashSet<MenuEnum>
        {
            MenuEnum.ComboBox,
            MenuEnum.DataGrid,
            MenuEnum.ListBox,
            MenuEnum.ListView,
            MenuEnum.ScrollViewer,
            MenuEnum.ScreenCut,
            MenuEnum.ColorPicker,
            MenuEnum.TimePicker,
            MenuEnum.VirtualizingWrapPanel,
            MenuEnum.WaterfallPanel,
        };

        private IList<NavigateMenuModel> _navigateMenuModelList;

        public IList<NavigateMenuModel> NavigateMenuModelList
        {
            get { return _navigateMenuModelList; }
            set
            {
                _navigateMenuModelList = value;
                NotifyPropertyChange("NavigateMenuModelList");
            }
        }

        private NavigateMenuModel _navigateMenuItem;
        /// <summary>
        /// 当前选中
        /// </summary>
        public NavigateMenuModel NavigateMenuItem
        {
            get { return _navigateMenuItem; }
            set
            {
                _navigateMenuItem = value;
                NotifyPropertyChange("NavigateMenuItem");
            }
        }
        private object _controlPanel;
        private ObservableCollection<NavigateMenuModel> _customMenuModelList;
        private ObservableCollection<NavigateMenuModel> _basicMenuModelList;

        private int _segmentedSelectedIndex;
        private string _searchText = string.Empty;
        private string _searchWatermark;
        private string _customMenuLabel;
        private string _basicMenuLabel;
        private string _demoBadgeToolTip;

        public bool IsChineseLanguage
        {
            get { return LanguageManager.Instance.CurrentCulture.TwoLetterISOLanguageName == "zh"; }
            set
            {
                var cultureName = value ? "zh-CN" : "en-US";
                if (LanguageManager.Instance.CurrentCulture.Name != cultureName)
                    LanguageManager.Instance.ChangeLanguage(new CultureInfo(cultureName));
            }
        }

        public string SearchWatermark
        {
            get { return _searchWatermark; }
            private set
            {
                _searchWatermark = value;
                NotifyPropertyChange(nameof(SearchWatermark));
            }
        }

        public string CustomMenuLabel
        {
            get { return _customMenuLabel; }
            private set
            {
                _customMenuLabel = value;
                NotifyPropertyChange(nameof(CustomMenuLabel));
            }
        }

        public string BasicMenuLabel
        {
            get { return _basicMenuLabel; }
            private set
            {
                _basicMenuLabel = value;
                NotifyPropertyChange(nameof(BasicMenuLabel));
            }
        }

        public string DemoBadgeToolTip
        {
            get { return _demoBadgeToolTip; }
            private set
            {
                _demoBadgeToolTip = value;
                NotifyPropertyChange(nameof(DemoBadgeToolTip));
            }
        }

        public int SegmentedSelectedIndex
        {
            get { return _segmentedSelectedIndex; }
            set
            {
                _segmentedSelectedIndex = value;
                NotifyPropertyChange("SegmentedSelectedIndex");
                NavigateMenuModelList = value == 1 ? _basicMenuModelList : _customMenuModelList;
                ApplySearchFilter();
                if (NavigateMenuModelList.Count > 0)
                    NavigateMenuItem = NavigateMenuModelList.FirstOrDefault(y => y.IsVisible);
            }
        }

        /// <summary>
        /// 更换右侧面板
        /// </summary>
        public object ControlPanel
        {
            get { return _controlPanel; }
            set
            {
                _controlPanel = value;
                NotifyPropertyChange("ControlPanel");
            }
        }
        public MainVM()
        {
            _customMenuModelList = new ObservableCollection<NavigateMenuModel>();
            _basicMenuModelList = new ObservableCollection<NavigateMenuModel>();
            foreach (MenuEnum menuEnum in Enum.GetValues(typeof(MenuEnum)))
            {
                var name = menuEnum.ToString();
                var menuModel = new NavigateMenuModel
                {
                    Name = name,
                    IsDemo = DemoMenuItems.Contains(menuEnum)
                };
                if (NewMenuItems.Contains(menuEnum))
                    menuModel.ChangeType = "New";
                else if (UpdatedMenuItems.Contains(menuEnum))
                    menuModel.ChangeType = "Updated";

                if (BasicEnumSet.Contains(name))
                    _basicMenuModelList.Add(menuModel);
                else
                    _customMenuModelList.Add(menuModel);
            }
            NavigateMenuModelList = _customMenuModelList;
            _navigateMenuItem = NavigateMenuModelList.First();
            _customMenuModelList.Add(new NavigateMenuModel
            {
                Name = "持续更新中",
                IconSource = GetMenuIconSource("Navigation")
            });
            LanguageManager.Instance.PropertyChanged += LanguageManager_PropertyChanged;
            UpdateMenuDisplayNames();
            ControlPanel = new NavMenu3DExample();
        }

        private static string GetMenuIconSource(MenuEnum menuEnum)
        {
            string iconName;
            switch (menuEnum)
            {
                case MenuEnum.NavMenu3D:
                    iconName = "NavMenu3D";
                    break;
                case MenuEnum.MapView:
                    iconName = "Map";
                    break;
                case MenuEnum.UsageGuide:
                case MenuEnum.MarkdownViewer:
                    iconName = "Book";
                    break;
                case MenuEnum.UsageColor:
                case MenuEnum.ColorPicker:
                case MenuEnum.Theme:
                    iconName = "Palette";
                    break;
                case MenuEnum.MessageBox:
                case MenuEnum.Toast:
                case MenuEnum.ChatEmoji:
                case MenuEnum.Barrage:
                    iconName = "Chat";
                    break;
                case MenuEnum.NotifyIcon:
                case MenuEnum.Badge:
                    iconName = "Activity";
                    break;
                case MenuEnum.PanningItems:
                case MenuEnum.Drap:
                    iconName = "Hand";
                    break;
                case MenuEnum.BreatheLight:
                case MenuEnum.SpotLight:
                case MenuEnum.EdgeLight:
                case MenuEnum.AcrylicBlur:
                    iconName = "Sparkle";
                    break;
                case MenuEnum.RingLoading:
                case MenuEnum.Loading:
                case MenuEnum.BasicLoading:
                case MenuEnum.CircleProgressBar:
                    iconName = "Spinner";
                    break;
                case MenuEnum.ProgressBar:
                    iconName = "Progress";
                    break;
                case MenuEnum.BallLoading:
                case MenuEnum.RoundPicker:
                case MenuEnum.Gauge:
                case MenuEnum.Dial:
                    iconName = "Circle";
                    break;
                case MenuEnum.StreamerLoading:
                case MenuEnum.WaitLoading:
                case MenuEnum.CycleLoading:
                case MenuEnum.RollLoading:
                    iconName = "Hourglass";
                    break;
                case MenuEnum.CropAvatar:
                case MenuEnum.CropImage:
                case MenuEnum.ImageCrop:
                case MenuEnum.ScreenCut:
                    iconName = "Crop";
                    break;
                case MenuEnum.AnimationAudio:
                case MenuEnum.SongWords:
                    iconName = "Music";
                    break;
                case MenuEnum.TransformLayout:
                case MenuEnum.WaterfallPanel:
                case MenuEnum.VirtualizingWrapPanel:
                case MenuEnum.SixGridView:
                    iconName = "Grid";
                    break;
                case MenuEnum.GridSplitter:
                    iconName = "GridSplitter";
                    break;
                case MenuEnum.VerifyCode:
                case MenuEnum.OtpBox:
                case MenuEnum.GestureUnlock:
                    iconName = "Shield";
                    break;
                case MenuEnum.CircleMenu:
                case MenuEnum.DrawerMenu:
                case MenuEnum.Drawer:
                case MenuEnum.Menu:
                case MenuEnum.NavScrollPanel:
                case MenuEnum.ScrollNavigation:
                    iconName = "Navigation";
                    break;
                case MenuEnum.Dashboard:
                case MenuEnum.NumberCard:
                case MenuEnum.DataGridFilter:
                    iconName = "Dashboard";
                    break;
                case MenuEnum.DataGrid:
                    iconName = "DataGrid";
                    break;
                case MenuEnum.PieSegment:
                case MenuEnum.ChartPie:
                case MenuEnum.ChartRadar:
                case MenuEnum.ChartBar:
                case MenuEnum.ChartLine:
                case MenuEnum.LineChart:
                    iconName = "Chart";
                    break;
                case MenuEnum.Password:
                case MenuEnum.PasswordBox:
                    iconName = "Lock";
                    break;
                case MenuEnum.ActivityFeed:
                case MenuEnum.Timeline:
                    iconName = "Activity";
                    break;
                case MenuEnum.CardCarousel:
                case MenuEnum.FocusCarousel:
                case MenuEnum.Carousel:
                case MenuEnum.Pagination:
                    iconName = "Cards";
                    break;
                case MenuEnum.ScrollViewer:
                    iconName = "Scroll";
                    break;
                case MenuEnum.Flame:
                    iconName = "Flame";
                    break;
                case MenuEnum.ZooSemy:
                    iconName = "Animal";
                    break;
                case MenuEnum.Ruler:
                case MenuEnum.Spacing:
                    iconName = "Ruler";
                    break;
                case MenuEnum.RainbowBtn:
                    iconName = "Rainbow";
                    break;
                case MenuEnum.Thermometer:
                    iconName = "Thermometer";
                    break;
                case MenuEnum.SnowCanvas:
                    iconName = "Snow";
                    break;
                case MenuEnum.LoginWindow:
                case MenuEnum.Desktop:
                case MenuEnum.Window:
                    iconName = "Window";
                    break;
                case MenuEnum.Drawing:
                case MenuEnum.CanvasHandWriting:
                    iconName = "Pen";
                    break;
                case MenuEnum.SpeedRockets:
                    iconName = "Rocket";
                    break;
                case MenuEnum.CountdownTimer:
                case MenuEnum.DateRangePicker:
                case MenuEnum.Calendar:
                    iconName = "Calendar";
                    break;
                case MenuEnum.TimePicker:
                case MenuEnum.DateTimePicker:
                    iconName = "DateTimePicker";
                    break;
                case MenuEnum.DatePicker:
                    iconName = "DatePicker";
                    break;
                case MenuEnum.DrawPrize:
                    iconName = "Gift";
                    break;
                case MenuEnum.StarrySky:
                    iconName = "Stars";
                    break;
                case MenuEnum.Shake:
                case MenuEnum.TransitionPanel:
                case MenuEnum.AnimationGrid:
                    iconName = "Activity";
                    break;
                case MenuEnum.BubbleSelection:
                    iconName = "Circle";
                    break;
                case MenuEnum.Magnifier:
                    iconName = "Search";
                    break;
                case MenuEnum.Step:
                    iconName = "Steps";
                    break;
                case MenuEnum.MultiSelectComboBox:
                case MenuEnum.Selector:
                case MenuEnum.Segmented:
                case MenuEnum.SplitButton:
                    iconName = "Selection";
                    break;
                case MenuEnum.ToggleButton:
                    iconName = "ToggleButton";
                    break;
                case MenuEnum.RadioButton:
                    iconName = "RadioButton";
                    break;
                case MenuEnum.Checkbox:
                    iconName = "Checkbox";
                    break;
                case MenuEnum.Mask:
                case MenuEnum.GrayscaleEffect:
                    iconName = "Image";
                    break;
                case MenuEnum.BreadCrumbBar:
                    iconName = "Navigation";
                    break;
                case MenuEnum.PathIcon:
                case MenuEnum.AllPathIcon:
                case MenuEnum.SvgViewer:
                    iconName = "Vector";
                    break;
                case MenuEnum.NumericBox:
                case MenuEnum.TextBox:
                    iconName = "Text";
                    break;
                case MenuEnum.IPEditBox:
                    iconName = "Network";
                    break;
                case MenuEnum.Tag:
                    iconName = "Tag";
                    break;
                case MenuEnum.IconicThumbnail:
                    iconName = "Image";
                    break;
                case MenuEnum.Slider:
                    iconName = "Slider";
                    break;
                case MenuEnum.Button:
                    iconName = "Button";
                    break;
                case MenuEnum.ComboBox:
                    iconName = "ComboBox";
                    break;
                case MenuEnum.ListBox:
                    iconName = "ListBox";
                    break;
                case MenuEnum.ListView:
                    iconName = "ListView";
                    break;
                case MenuEnum.TreeView:
                    iconName = "TreeView";
                    break;
                case MenuEnum.GroupBox:
                    iconName = "GroupBox";
                    break;
                case MenuEnum.TabControl:
                    iconName = "TabControl";
                    break;
                case MenuEnum.Expander:
                    iconName = "Expander";
                    break;
                case MenuEnum.TaskbarInfo:
                    iconName = "TaskbarInfo";
                    break;
                default:
                    iconName = "Navigation";
                    break;
            }

            return GetMenuIconSource(iconName);
        }

        private static string GetMenuIconSource(string iconName)
        {
            return "pack://application:,,,/WPFDevelopers.Samples;component/Resources/MenuIcons/"
                + iconName + ".svg";
        }

        private void LanguageManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LanguageManager.CurrentCulture))
                UpdateMenuDisplayNames();
        }

        private void UpdateMenuDisplayNames()
        {
            var culture = LanguageManager.Instance.CurrentCulture;
            var isChinese = culture.TwoLetterISOLanguageName == "zh";
            NotifyPropertyChange(nameof(IsChineseLanguage));
            SearchWatermark = isChinese ? "搜索控件" : "Search Control";
            CustomMenuLabel = isChinese ? "自定义" : "Custom";
            BasicMenuLabel = isChinese ? "基础控件" : "Basic";
            DemoBadgeToolTip = isChinese
                ? "示例专属实现或组合效果，非 WD 内置控件；请参考示例源码。"
                : "Sample-only implementation or composition, not a built-in WD control. See the sample source.";

            foreach (var item in _customMenuModelList.Concat(_basicMenuModelList))
                item.DisplayName = MenuNameLocalizer.GetDisplayName(item.Name, culture);

            ApplySearchFilter();
        }

        public ICommand ViewLoaded => new RelayCommand(obj =>
        {


        });

        public ICommand MenuSearchTextChanged => new RelayCommand(obj =>
        {
            _searchText = obj.ToString();
            ApplySearchFilter();
        });

        private void ApplySearchFilter()
        {
            if (string.IsNullOrEmpty(_searchText))
            {
                NavigateMenuModelList.ForEach(y => y.IsVisible = true);
            }
            else
            {
                var key = _searchText.ToLower();
                foreach (var item in NavigateMenuModelList)
                {
                    item.IsVisible = item.Name.ToLower().Contains(key) ||
                                     item.DisplayName.ToLower().Contains(key);
                }
            }
        }

        public ICommand MenuSelectionChangedCommand => new RelayCommand(obj =>
        {
            if (obj == null) return;
            var model = obj as NavigateMenuModel;
            NavigateMenuItem = model;
            MenuItemSelection(model.Name);
        });

        public ICommand CloseCommand => new RelayCommand(obj =>
        {
            Application.Current.MainWindow.Close();
        });


        void MenuItemSelection(string _menuName)
        {
            MenuEnum flag;
            if (!Enum.TryParse<MenuEnum>(_menuName, true, out flag))
                return;
            var menuEnum = (MenuEnum)Enum.Parse(typeof(MenuEnum), _menuName, true);
            switch (menuEnum)
            {
                case MenuEnum.NavMenu3D:
                    ControlPanel = new NavMenu3DExample();
                    break;
                case MenuEnum.UsageGuide:
                    ControlPanel = new UsageGuide();
                    break;
                case MenuEnum.UsageColor:
                    ControlPanel = new UsageColor();
                    break;
                case MenuEnum.Theme:
                    ControlPanel = new ThemeExample();
                    break;
                case MenuEnum.Menu:
                    ControlPanel = new MenuExample();
                    break;
                case MenuEnum.Window:
                    ControlPanel = new WindowExample();
                    break;
                case MenuEnum.BasicLoading:
                    ControlPanel = new LoadingBasicExample();
                    break;
                case MenuEnum.Button:
                    ControlPanel = new ButtonExample();
                    break;
                case MenuEnum.RadioButton:
                    ControlPanel = new RadioButtonExample();
                    break;
                case MenuEnum.Checkbox:
                    ControlPanel = new CheckboxExample();
                    break;
                case MenuEnum.TextBox:
                    ControlPanel = new TextBoxExample();
                    break;
                case MenuEnum.PasswordBox:
                    ControlPanel = new PasswordBoxExample();
                    break;
                case MenuEnum.ComboBox:
                    ControlPanel = new ComboBoxExample();
                    break;
                case MenuEnum.ToggleButton:
                    ControlPanel = new ToggleButtonExample();
                    break;
                case MenuEnum.DatePicker:
                    ControlPanel = new DatePickerExample();
                    break;
                case MenuEnum.DateTimePicker:
                    ControlPanel = new DateTimePickerExample();
                    break;
                case MenuEnum.Calendar:
                    ControlPanel = new CalendarExample();
                    break;
                case MenuEnum.Slider:
                    ControlPanel = new SliderExample();
                    break;
                case MenuEnum.GridSplitter:
                    ControlPanel = new GridSplitterExample();
                    break;
                case MenuEnum.ProgressBar:
                    ControlPanel = new ProgressBarExample();
                    break;
                case MenuEnum.DataGrid:
                    ControlPanel = new DataGridExample();
                    break;
                case MenuEnum.ListBox:
                    ControlPanel = new ListBoxExample();
                    break;
                case MenuEnum.ListView:
                    ControlPanel = new ListViewExample();
                    break;
                case MenuEnum.TreeView:
                    ControlPanel = new TreeViewExample();
                    break;
                case MenuEnum.Expander:
                    ControlPanel = new ExpanderExample();
                    break;
                case MenuEnum.GroupBox:
                    ControlPanel = new GroupBoxExample();
                    break;
                case MenuEnum.TabControl:
                    ControlPanel = new TabControlExample();
                    break;
                case MenuEnum.MessageBox:
                    ControlPanel = new MessageBoxExample();
                    break;
                case MenuEnum.NotifyIcon:
                    ControlPanel = new NotifyIconExample();
                    break;
                case MenuEnum.RingLoading:
                    ControlPanel = new RingLoadingExample();
                    break;
                case MenuEnum.BallLoading:
                    ControlPanel = new BallLoadingExample();
                    break;
                case MenuEnum.StreamerLoading:
                    ControlPanel = new StreamerLoadingExample();
                    break;
                case MenuEnum.WaitLoading:
                    ControlPanel = new WaitLoadingExample();
                    break;
                case MenuEnum.CycleLoading:
                    ControlPanel = new CycleLoadingExample();
                    break;
                case MenuEnum.RollLoading:
                    ControlPanel = new RollLoadingExample();
                    break;
                //case MenuEnum.CutImage:
                //    ControlPanel = new CutImageExample();
                //    break;
                case MenuEnum.CropAvatar:
                    ControlPanel = new CropAvatarExample();
                    break;
                case MenuEnum.AnimationAudio:
                    ControlPanel = new AnimationAudioExample();
                    break;

                case MenuEnum.TransformLayout:
                    ControlPanel = new TransformLayoutExample();
                    break;
                case MenuEnum.VerifyCode:
                    ControlPanel = new VerifyCodeExample();
                    break;
                case MenuEnum.CircleMenu:
                    ControlPanel = new CircleMenuExample();
                    break;
                case MenuEnum.BreatheLight:
                    ControlPanel = new BreatheLightExample();
                    break;

                case MenuEnum.ChatEmoji:
                    ControlPanel = new ChatEmojiExample();
                    break;
                case MenuEnum.CircleProgressBar:
                    ControlPanel = new CircleProgressBarExample();
                    break;
                case MenuEnum.Dashboard:
                    ControlPanel = new DashboardExample();
                    break;
                case MenuEnum.PieSegment:
                    ControlPanel = new PieSegmentExample();
                    break;
                case MenuEnum.Password:
                    ControlPanel = new PasswordExample();
                    break;
                case MenuEnum.SongWords:
                    ControlPanel = new SongWordsExample();
                    break;
                case MenuEnum.ActivityFeed:
                    ControlPanel = new ActivityFeedExample();
                    break;
                case MenuEnum.ScrollViewer:
                    ControlPanel = new ScrollViewerAnimationExample();
                    break;
                case MenuEnum.BackToTop:
                    ControlPanel = new BackToTopExample();
                    break;
                case MenuEnum.CardCarousel:
                    ControlPanel = new CardCarouselExample();
                    break;
                case MenuEnum.FocusCarousel:
                    ControlPanel = new FocusCarouselExample();
                    break;
                case MenuEnum.Flame:
                    ControlPanel = new FlameExample();
                    break;
                case MenuEnum.ScreenCut:
                    ControlPanel = new ScreenCutExample();
                    break;
                case MenuEnum.TransitionPanel:
                    ControlPanel = new TransitionPanelExample();
                    break;
                case MenuEnum.SpotLight:
                    ControlPanel = new SpotLightExample();
                    break;
                case MenuEnum.DrawerMenu:
                    ControlPanel = new DrawerMenuExample();
                    break;
                case MenuEnum.ChartRadar:
                    ControlPanel = new ChartRadarExample();
                    break;
                case MenuEnum.LoginWindow:
                    ControlPanel = new LoginExample();
                    var loginWindow = new LoginWindowExample();
                    loginWindow.MaskShowDialog();
                    break;
                case MenuEnum.Pagination:
                    ControlPanel = new PaginationExample();
                    break;
                case MenuEnum.ChartBar:
                    ControlPanel = new ChartBarExample();
                    break;
                case MenuEnum.ZooSemy:
                    ControlPanel = new ZooSemyExample();
                    break;
                case MenuEnum.Ruler:
                    ControlPanel = new RulerExample();
                    break;
                case MenuEnum.RainbowBtn:
                    ControlPanel = new RainbowButtonsExample();
                    break;
                case MenuEnum.RoundPicker:
                    ControlPanel = new RoundPickerExample();
                    break;
                case MenuEnum.LineChart:
                    ControlPanel = new LineChartExample();
                    break;
                //case MenuEnum.LogoAnimation:
                //    ControlPanel = new LogoAnimationExample();
                //    break;
                case MenuEnum.Thermometer:
                    ControlPanel = new ThermometerExample();
                    break;
                case MenuEnum.SnowCanvas:
                    ControlPanel = new SnowCanvasExample();
                    break;
                case MenuEnum.Drawing:
                    ControlPanel = new DrawingExample();
                    break;
                case MenuEnum.SpeedRockets:
                    ControlPanel = new SpeedRocketsExample();
                    break;
                case MenuEnum.CountdownTimer:
                    ControlPanel = new CountdownTimerExample();
                    break;
                case MenuEnum.NumberCard:
                    ControlPanel = new NumberCardExample();
                    break;
                case MenuEnum.ImageCrop:
                    ControlPanel = new ImageCropExample();
                    break;
                case MenuEnum.Desktop:
                    ControlPanel = new DesktopBackground();
                    break;
                case MenuEnum.DrawPrize:
                    ControlPanel = new DrawPrizeExample();
                    break;
                case MenuEnum.EdgeLight:
                    ControlPanel = new EdgeLightExample();
                    break;
                case MenuEnum.StarrySky:
                    ControlPanel = new StarrySkyExample();
                    break;
                case MenuEnum.Shake:
                    ControlPanel = new ShakeExample();
                    break;
                case MenuEnum.PanningItems:
                    ControlPanel = new PanningItemsExample();
                    break;
                case MenuEnum.BubbleSelection:
                    ControlPanel = new BubbleSelectionExample();
                    break;
                case MenuEnum.CanvasHandWriting:
                    ControlPanel = new CanvasHandWritingExample();
                    break;
                case MenuEnum.Barrage:
                    ControlPanel = new BarrageExample();
                    break;
                case MenuEnum.SixGridView:
                    ControlPanel = new SixGirdViewExample();
                    break;
                case MenuEnum.Magnifier:
                    ControlPanel = new MagnifierExample();
                    break;
                case MenuEnum.Step:
                    ControlPanel = new StepExample();
                    break;
                case MenuEnum.MultiSelectComboBox:
                    ControlPanel = new MultiSelectComboBoxExample();
                    break;
                case MenuEnum.Mask:
                    ControlPanel = new MaskExample();
                    break;
                case MenuEnum.Loading:
                    ControlPanel = new LoadingExample();
                    break;
                case MenuEnum.Selector:
                    ControlPanel = new SelectorExample();
                    break;
                //case MenuEnum.MultiSelectSearchComboBox:
                //    ControlPanel = new MultiSelectSearchComboBoxExample();
                //    break;
                case MenuEnum.BreadCrumbBar:
                    ControlPanel = new BreadCrumbBarExample();
                    break;
                case MenuEnum.CropImage:
                    ControlPanel = new CropImageExample();
                    break;
                case MenuEnum.Badge:
                    ControlPanel = new BadgeExample();
                    break;
                case MenuEnum.Toast:
                    ControlPanel = new ToastExample();
                    break;
                case MenuEnum.PathIcon:
                    ControlPanel = new PathIconExample();
                    break;
                case MenuEnum.NumericBox:
                    ControlPanel = new NumericBoxExample();
                    break;
                case MenuEnum.ColorPicker:
                    ControlPanel = new ColorPickerExample();
                    break;
                case MenuEnum.IPEditBox:
                    ControlPanel = new IPEditBoxExample();
                    break;
                case MenuEnum.TimePicker:
                    ControlPanel = new TimePickerExample();
                    break;
                case MenuEnum.WaterfallPanel:
                    ControlPanel = new WaterfallPanelExample();
                    break;
                case MenuEnum.ChartLine:
                    ControlPanel = new ChartLineExample();
                    break;
                case MenuEnum.Drap:
                    ControlPanel = new DrapViewExample();
                    break;
                case MenuEnum.ChartPie:
                    ControlPanel = new ChartPieExample();
                    break;
                case MenuEnum.Spacing:
                    ControlPanel = new SpacingExample();
                    break;
                case MenuEnum.Drawer:
                    ControlPanel = new DrawerExample();
                    break;
                case MenuEnum.DateRangePicker:
                    ControlPanel = new DateRangePickerExample();
                    break;
                case MenuEnum.Gauge:
                    ControlPanel = new GaugeExample();
                    break;
                case MenuEnum.Tag:
                    ControlPanel = new TagExample();
                    break;
                case MenuEnum.NavScrollPanel:
                    ControlPanel = new NavScrollPanelExample();
                    break;
                case MenuEnum.IconicThumbnail:
                    ControlPanel = new IconicThumbnailExample();
                    new IconicThumbnailWindowExample().MaskShowDialog();
                    break;
                case MenuEnum.GestureUnlock:
                    ControlPanel = new GestureUnlockExample();
                    break;
                case MenuEnum.AnimationGrid:
                    ControlPanel = new AnimationGridExample();
                    break;
                case MenuEnum.AllPathIcon:
                    ControlPanel = new AllPathIconExample();
                    break;
                case MenuEnum.Dial:
                    ControlPanel = new DialExample();
                    break;
                case MenuEnum.SvgViewer:
                    ControlPanel = new SvgViewerExample();
                    break;
                case MenuEnum.DataGridFilter:
                    ControlPanel = new DataGridFilterExample();
                    break;
                case MenuEnum.GrayscaleEffect:
                    ControlPanel = new GrayscaleEffectExample();
                    break;
                case MenuEnum.OtpBox:
                    ControlPanel = new OtpBoxExample();
                    break;
                case MenuEnum.Carousel:
                    ControlPanel = new CarouselExample();
                    break;
                case MenuEnum.SplitButton:
                    ControlPanel = new SplitButtonExample();
                    break;
                case MenuEnum.Segmented:
                    ControlPanel = new SegmentedExample();
                    break;
                case MenuEnum.MarkdownViewer:
                    ControlPanel = new MarkdownViewerExample();
                    break;
                case MenuEnum.MapView:
                    ControlPanel = new MapViewExample();
                    break;
                case MenuEnum.Timeline:
                    ControlPanel = new TimelineExample();
                    break;
                //新的添加到上方
                case MenuEnum.VirtualizingWrapPanel:
                    ControlPanel = new VirtualizingWrapPanel();
                    new VirtualizingWrapPanelExample().MaskShowDialog();
                    break;
                case MenuEnum.AcrylicBlur:
                    ControlPanel = new AcrylicBlurExample();
                    new AcrylicBlurWindowExample().MaskShowDialog();
                    break;
                case MenuEnum.TaskbarInfo:
                    ControlPanel = new TaskbarItemInfo();
                    var taskbar = new TaskbarItemInfoExample();
                    taskbar.MaskShowDialog();
                    break;
                default:
                    break;
            }
        }



    }
}
