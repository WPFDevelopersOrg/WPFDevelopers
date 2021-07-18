using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WPFDevelopers.Samples.ExampleViews;
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
        /// 更换左侧面板
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
            NavigateMenuModelList.Add(new NavigateMenuModel { Name = "Navigation3D" });
            NavigateMenuModelList.Add(new NavigateMenuModel { Name = "Loading" });
            NavigateMenuModelList.Add(new NavigateMenuModel { Name = "CutImage" });
            NavigateMenuModelList.Add(new NavigateMenuModel { Name = "持续更新" });
            NavigateMenuModelList.Add(new NavigateMenuModel { Name = "持续更新" });

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
                case MenuEnum.Loading:
                    ControlPanel = new LoadingExample();
                    break;
                case MenuEnum.CutImage:
                    ControlPanel = new CutImageExample();
                    break;
                default:
                    break;
            }
        }


     }
}
