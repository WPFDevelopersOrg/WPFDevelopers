using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WPFDevelopers.Controls;

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


        public DrawerMenuExample()
        {
            InitializeComponent();
            var items = new List<DrawerMenuItem>();
            items.Add(new DrawerMenuItem { Text = "Menu01", Icon = new Image() { Source = new BitmapImage(new Uri($"pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/CircleMenu/1.png")) } });
            items.Add(new DrawerMenuItem { Text = "Menu02", Icon = new Image() { Source = new BitmapImage(new Uri($"pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/CircleMenu/2.png")) } });
            items.Add(new DrawerMenuItem { Text = "Menu03", Icon = new Image() { Source = new BitmapImage(new Uri($"pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/CircleMenu/3.png")) } });
            DrawerMenuItems = items;
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
}
