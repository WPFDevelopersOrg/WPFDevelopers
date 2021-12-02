using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WPFDevelopers.Samples.ExampleViews;
using WPFDevelopers.Samples.ExampleViews.Passwrod;
using WPFDevelopers.Samples.ExampleViews.Win10Menu;
using WPFDevelopers.Samples.Helpers;
using WPFDevelopers.Samples.Models;

namespace WPFDevelopers.Samples.ViewModels
{
    public class MainVM:ViewModelBase
    {
        private ObservableCollection<NavigateMenuModel> _navigateMenuModelList;

        public ObservableCollection<NavigateMenuModel> NavigateMenuModelList
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
                this.NotifyPropertyChange("NavigateMenuItem");
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
                this.NotifyPropertyChange("ControlPanel");
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

        }
        public ICommand ViewLoaded => new RelayCommand(obj =>
        {
            ControlPanel = new AnimationNavigationBar3DExample();

        });
        public ICommand MenuSelectionChangedCommand => new RelayCommand(obj =>
        {
            if (obj == null) return;
            var model = obj as NavigateMenuModel;
            MenuItemSelection(model.Name);
        });

        public ICommand CloseCommand => new RelayCommand( obj => 
        {
            //Environment.Exit(0);
            Application.Current.MainWindow.Close();
        });


        void MenuItemSelection(string _menuName)
        {
            MenuEnum flag;
            if (!Enum.TryParse<MenuEnum>(_menuName, true, out flag))
                return;
            var menuEnum = (MenuEnum)Enum.Parse(typeof(MenuEnum), _menuName,true);
            switch (menuEnum)
            {
                case MenuEnum.Navigation3D:
                    ControlPanel = new AnimationNavigationBar3DExample();
                    break;
                case MenuEnum.BaseControl:
                    ControlPanel = new BaseControlExample();
                    break;
                case MenuEnum.Loading:
                    ControlPanel = new LoadingExample();
                    break;
                case MenuEnum.CutImage:
                    ControlPanel = new CutImageExample();
                    break;
                case MenuEnum.WeChatAudio:
                    ControlPanel = new AnimationWeChatExample();
                    break;
                case MenuEnum.AMap:
                    ControlPanel = new BingAMapExample();
                    break;
                case MenuEnum.ThumbAngle:
                    ControlPanel = new ThumbDragAndAngleExample();
                    break;
                case MenuEnum.CheckCode:
                    ControlPanel = new CheckCodeExample();
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
                case MenuEnum.RoundMenu:
                    ControlPanel = new RoundMenuExample();
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
                case MenuEnum.Win10Menu:
                    ControlPanel = new Win10MenuExample();
                    break;
                case MenuEnum.RadarChart:
                    ControlPanel = new RadarChartExample();
                    break;
                case MenuEnum.LoginWindow:
                    var loginWindow = new LoginWindowExample();
                    loginWindow.ShowDialog();
                    break;
                case MenuEnum.Pagination:
                    ControlPanel = new PaginationExample();
                    break;
                case MenuEnum.BasicBarChart:
                    ControlPanel = new BasicBarChartExample();
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
                case MenuEnum.LogoAnimation:
                    ControlPanel = new LogoAnimationExample();
                    break;
                case MenuEnum.Thermometer:
                    ControlPanel = new ThermometerExample();
                    break;
                case MenuEnum.Drawing:
                    ControlPanel = new DrawingExample();
                    break;
                //将TaskbarInfo放到最后
                case MenuEnum.TaskbarInfo:
                    var taskbar = new TaskbarItemInfoExample();
                    taskbar.ShowDialog();
                    break;
                default:
                    break;
            }
        }



     }
}
