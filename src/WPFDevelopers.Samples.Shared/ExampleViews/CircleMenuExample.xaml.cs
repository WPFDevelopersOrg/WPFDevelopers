using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.Models;
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

            for (int i = 1; i <= 8; i++)
            {
                MenuItems.Add(new MenuItemModel
                {
                    Text = $"菜单{i}",
                    Icon = new Image
                    {
                        Source = new BitmapImage(new Uri($"pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/CircleMenu/{i}.png")),
                        Stretch = System.Windows.Media.Stretch.Uniform,
                        Width = 30,
                        Height = 30
                    }
                });
            }

            DataContext = this;
        }

        private void CircleMenu_ItemClick(object sender, RoutedEventArgs e)
        {
            var circularMenu = sender as CircleMenu;
            var menuItemModel = circularMenu.SelectedItem as MenuItemModel;
            if (menuItemModel != null)
                MessageBox.Show($"点击了{menuItemModel.Text}", "info", MessageBoxImage.Information);
        }
    }
}
