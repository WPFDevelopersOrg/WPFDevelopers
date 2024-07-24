using Microsoft.Expression.Drawing.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Controls;
using WPFDevelopers.Helpers;
using WPFDevelopers.Sample.ExampleViews;
using WPFDevelopers.Samples.ExampleViews;
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
        private IList<NavigateMenuModel> _navigateMenuModelList;

        public IList<NavigateMenuModel> NavigateMenuModelList
        {
            get { return _navigateMenuModelList; }
            set { _navigateMenuModelList = value; }
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
            NavigateMenuModelList = new ObservableCollection<NavigateMenuModel>();
            foreach (MenuEnum menuEnum in Enum.GetValues(typeof(MenuEnum)))
            {
                NavigateMenuModelList.Add(new NavigateMenuModel { Name = menuEnum.ToString() });
            }
            NavigateMenuModelList.Add(new NavigateMenuModel { Name = "持续更新中" });
            ControlPanel = new AnimationNavigationBar3DExample();
        }

        public ICommand ViewLoaded => new RelayCommand(obj =>
        {


        });

        public ICommand MenuSearchTextChanged => new RelayCommand(obj =>
        {
            var search = obj.ToString();
            if (string.IsNullOrEmpty(search))
            {
                NavigateMenuModelList.ForEach(y => y.IsVisible = true);
            }
            else
            {
                var key = search.ToLower();
                foreach (var item in NavigateMenuModelList)
                {
                    if (item.Name.ToLower().Contains(key))
                        item.IsVisible = true;
                    else
                        item.IsVisible = false;
                }
            }
        });

        public ICommand MenuSelectionChangedCommand => new RelayCommand(obj =>
        {
            if (obj == null) return;
            var model = obj as NavigateMenuModel;
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
                case MenuEnum.Navigation3D:
                    ControlPanel = new AnimationNavigationBar3DExample();
                    break;
                case MenuEnum.BasicControls:
                    ControlPanel = new BasicControlsExample();
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
                case MenuEnum.CutImage:
                    ControlPanel = new CutImageExample();
                    break;
                case MenuEnum.CropAvatar:
                    ControlPanel = new CropAvatarExample();
                    break;
                case MenuEnum.AnimationAudio:
                    ControlPanel = new AnimationAudioExample();
                    break;
                case MenuEnum.AMap:
                    ControlPanel = new BingAMapExample();
                    break;
                case MenuEnum.TransformLayout:
                    ControlPanel = new TransformLayoutExample();
                    break;
                case MenuEnum.VerifyCode:
                    ControlPanel = new VerifyCodeExample();
                    break;
                case MenuEnum.CircularMenu:
                    ControlPanel = new CircularMenuExample();
                    break;
                case MenuEnum.BreatheLight:
                    ControlPanel = new BreatheLightExample();
                    break;

                case MenuEnum.ChatEmoji:
                    ControlPanel = new ChatEmojiExample();
                    break;
                case MenuEnum.ProgressBar:
                    ControlPanel = new CircularProgressBarExample();
                    break;
                case MenuEnum.Dashboard:
                    ControlPanel = new DashboardExample();
                    break;
                case MenuEnum.PieControl:
                    ControlPanel = new PieControlExample();
                    break;
                case MenuEnum.Password:
                    ControlPanel = new PasswordExample();
                    break;
                case MenuEnum.SongWords:
                    ControlPanel = new SongWordsExample();
                    break;
                case MenuEnum.TimeLine:
                    ControlPanel = new TimeLineExample();
                    break;
                case MenuEnum.ScrollViewer:
                    ControlPanel = new ScrollViewerAnimationExample();
                    break;
                case MenuEnum.Carousel:
                    ControlPanel = new CarouselExample();
                    break;
                case MenuEnum.CarouselEx:
                    ControlPanel = new CarouselExampleEx();
                    break;
                case MenuEnum.OtherControls:
                    ControlPanel = new OtherControlExample();
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
                case MenuEnum.RulerControl:
                    ControlPanel = new RulerControlExample();
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
                case MenuEnum.CropControl:
                    ControlPanel = new CropControlExample();
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
                case MenuEnum.BubblleControl:
                    ControlPanel = new BubblleControlExample();
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
                case MenuEnum.MultiSelectSearchComboBox:
                    ControlPanel = new MultiSelectSearchComboBoxExample();
                    break;
                case MenuEnum.BreadCrumbBar:
                    ControlPanel = new BreadCrumbBarExample();
                    break;
                case MenuEnum.CropImage:
                    ControlPanel = new CropImageExample();
                    break;
                case MenuEnum.Badge:
                    ControlPanel = new BadgeExample();
                    break;
                case MenuEnum.Message:
                    ControlPanel = new MessageExample();
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
                case MenuEnum.VirtualizingWrapPanel:
                    ControlPanel = new VirtualizingWrapPanel();
                    new VirtualizingWrapPanelExample().MaskShowDialog();
                    break;
                case MenuEnum.AcrylicBlur:
                    ControlPanel = new AcrylicBlurExample();
                    new AcrylicBlurWindowExample().MaskShowDialog();
                    break;
                //将TaskbarInfo放到最后
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
