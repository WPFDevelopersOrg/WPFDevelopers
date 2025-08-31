using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ExampleViews.DrawerMenu
{
    /// <summary>
    /// Win10MenuExample.xaml 的交互逻辑
    /// </summary>
    public partial class DrawerMenuExample : UserControl
    {
        private List<Uri> _uriList = new List<Uri>()
        {
            new Uri("pack://application:,,,/WPFDevelopers.Samples;component/ExampleViews/DrawerMenu/HomePage.xaml",UriKind.Absolute),
            new Uri("pack://application:,,,/WPFDevelopers.Samples;component/ExampleViews/DrawerMenu/EdgePage.xaml",UriKind.Absolute),
        };


        public IList<DrawerMenuItem> DrawerMenuItems
        {
            get { return (IList<DrawerMenuItem>)GetValue(DrawerMenuItemsProperty); }
            set { SetValue(DrawerMenuItemsProperty, value); }
        }

        public static readonly DependencyProperty DrawerMenuItemsProperty =
            DependencyProperty.Register("DrawerMenuItems", typeof(IList<DrawerMenuItem>), typeof(DrawerMenuExample), new PropertyMetadata(null));


        public ObservableCollection<MenuItemModel> MenuItems
        {
            get { return (ObservableCollection<MenuItemModel>)GetValue(MenuItemsProperty); }
            set { SetValue(MenuItemsProperty, value); }
        }

        public static readonly DependencyProperty MenuItemsProperty =
            DependencyProperty.Register("MenuItems", typeof(ObservableCollection<MenuItemModel>), typeof(DrawerMenuExample), new PropertyMetadata(null));


        public DrawerMenuExample()
        {
            InitializeComponent();
            var items = new List<DrawerMenuItem>();
            items.Add(new DrawerMenuItem { Text = "Menu01", Icon = new Image() { Source = new BitmapImage(new Uri($"pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/CircleMenu/1.png")) } });
            items.Add(new DrawerMenuItem { Text = "Menu02", Icon = new Image() { Source = new BitmapImage(new Uri($"pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/CircleMenu/2.png")) } });
            items.Add(new DrawerMenuItem { Text = "Menu03", Icon = new Image() { Source = new BitmapImage(new Uri($"pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/CircleMenu/3.png")) } });
            DrawerMenuItems = items;

            var list = new List<MenuItemModel>();
            var model = new MenuItemModel { Text = "Item 1" };
            for (int i = 0; i < 3; i++)
            {
                model.Children.Add(new MenuItemModel { Text = $"Item {i}" });
            }
            list.Add(model);
            var model2 = new MenuItemModel { Text = "Item 2" };
            for (int i = 0; i < 3; i++)
            {
                model2.Children.Add(new MenuItemModel { Text = $"Item {i}" });
            }
            list.Add(model2);
            var model3 = new MenuItemModel { Text = "Item 3" };
            list.Add(model3);
            MenuItems = new ObservableCollection<MenuItemModel>(list);
        }

        //public ICommand HomeCommand => new RelayCommand(obj =>
        //{
        //    myFrame.Navigate(_uriList[0]);
        //});
        //public ICommand EdgeCommand => new RelayCommand(obj =>
        //{
        //    myFrame.Navigate(_uriList[1]);
        //});
        //public ICommand CloudCommand => new RelayCommand(obj =>
        //{
        //    WPFDevelopers.Controls.MessageBox.Show("点击了云盘","提示");
        //});
        //public ICommand MailCommand => new RelayCommand(obj =>
        //{
        //    WPFDevelopers.Controls.MessageBox.Show("点击了邮件","提示");
        //});
        //public ICommand VideoCommand => new RelayCommand(obj =>
        //{
        //    WPFDevelopers.Controls.MessageBox.Show("点击了视频","提示");
        //});

        public ICommand SelectionCommand => new RelayCommand(obj => 
        {
            var selection = obj as DrawerMenuItem;
            if (selection != null) 
            {
                Message.Push($"当前选中{selection.Text}", MessageBoxImage.Information);
            }
        });


        public ICommand IsOpenCommand => new RelayCommand(obj =>
        {
            if(obj is bool isOpen)
            {
                Message.Push(isOpen ? "打开": "关闭",MessageBoxImage.Information);
            }
        });

        private void DrawerMenu_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (myFrame == null) return;
            var menu = (WPFDevelopers.Controls.DrawerMenu)sender;
            if (menu == null) return;
            var item = (WPFDevelopers.Controls.DrawerMenuItem)menu.SelectedItem;
            if (item == null) return;
            switch (item.Text)
            {
                case "主页":
                    myFrame.Navigate(_uriList[0]);
                    break;
                case "Edge":
                    myFrame.Navigate(_uriList[1]);
                    break;
                case "云盘":
                case "邮件":
                case "视频":
                case "Bus":
                    WPFDevelopers.Controls.MessageBox.Show($"点击了{item.Text}", "提示");
                    break;
            }
        }
    }
    public class MenuItemModel
    {
        public Path Icon { get; set; } = new Path { Data = Geometry.Parse("M804.571 566.857v274.286q0 14.857-10.857 25.714t-25.714 10.857h-219.429v-219.429h-146.286v219.429h-219.429q-14.857 0-25.714-10.857t-10.857-25.714v-274.286q0-0.571 0.286-1.714t0.286-1.714l328.571-270.857 328.571 270.857q0.571 1.143 0.571 3.429zM932 527.429l-35.429 42.286q-4.571 5.143-12 6.286h-1.714q-7.429 0-12-4l-395.429-329.714-395.429 329.714q-6.857 4.571-13.714 4-7.429-1.143-12-6.286l-35.429-42.286q-4.571-5.714-4-13.429t6.286-12.286l410.857-342.286q18.286-14.857 43.429-14.857t43.429 14.857l139.429 116.571v-111.429q0-8 5.143-13.143t13.143-5.143h109.714q8 0 13.143 5.143t5.143 13.143v233.143l125.143 104q5.714 4.571 6.286 12.286t-4 13.429z"), Fill = ThemeManager.Instance.PrimaryBrush, Stretch = Stretch.Uniform, Width = 15, Height = 16 };
        public string Text { get; set; }
        public ObservableCollection<MenuItemModel> Children { get; } = new ObservableCollection<MenuItemModel>();
    }
}
