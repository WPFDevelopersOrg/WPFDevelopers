using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Controls;
using WPFDevelopers.Helpers;
using WPFDevelopers.Samples.Helpers;
using MenuItemModel = WPFDevelopers.Samples.Models.MenuItemModel;
using MessageBox = WPFDevelopers.Controls.MessageBox;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// CircleMenuExample.xaml 的交互逻辑
    /// </summary>
    public partial class CircleMenuExample : UserControl
    {
        public ObservableCollection<MenuItemModel> MenuItems { get; } = new ObservableCollection<MenuItemModel>();

        public CircleMenuExample()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += OnCircleMenuExample_Loaded;
        }

        private void OnCircleMenuExample_Loaded(object sender, RoutedEventArgs e)
        {
            var resourceUris = ResourceHelper.GetResourceUris("Resources/Images/CircleMenu/");
            foreach (var uri in resourceUris)
            {
                MenuItems.Add(new MenuItemModel
                {
                    Text = $"{Path.GetFileNameWithoutExtension(uri)}",
                    Icon = new SvgViewer { Source = uri}
                });
            }
        }

        public ICommand ItemClickCommand => new RelayCommand(param =>
        {
            var menuItemModel = param as MenuItemModel;
            if (menuItemModel != null)
                Toast.Push($"点击了{menuItemModel.Text}",ToastImage.Info);
        });

        private void CircleMenu_ItemClick(object sender, RoutedEventArgs e)
        {
            var menuItem = (CircleMenuItem)e.OriginalSource;
            Toast.Push($"点击了：{menuItem.Tag}", ToastImage.Info);
        }
    }
}
