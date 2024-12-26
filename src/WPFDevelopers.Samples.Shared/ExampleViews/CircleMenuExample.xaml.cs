using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
        public IEnumerable MenuArray
        {
            get { return (IEnumerable)GetValue(MenuArrayProperty); }
            set { SetValue(MenuArrayProperty, value); }
        }

        public static readonly DependencyProperty MenuArrayProperty =
            DependencyProperty.Register("MenuArray", typeof(IEnumerable), typeof(CircleMenuExample), new PropertyMetadata(null));
        public CircleMenuExample()
        {
            InitializeComponent();
            
            var angle = 0;
            var menuItemModels = new List<MenuItemModel>();
            for (int i = 1; i <= 8; i++)
            {
                menuItemModels.Add(new MenuItemModel { Angle = angle, Title = $"菜单{i}", IconImage = new BitmapImage(new Uri($"pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/CircleMenu/{i}.png")) });
                angle += 45;
            }
            MenuArray = menuItemModels;
        }

        private void CircleMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var circularMenu = sender as CircleMenu;
            var menuItemModel = circularMenu.SelectedItem as MenuItemModel;
            MessageBox.Show($"点击了{menuItemModel.Title}");
        }
        
    }
}
