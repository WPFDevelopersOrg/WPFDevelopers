using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.Models;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// CircularMenuExample.xaml 的交互逻辑
    /// </summary>
    public partial class CircularMenuExample : UserControl
    {
        public IEnumerable MenuArray
        {
            get { return (IEnumerable)GetValue(MenuArrayProperty); }
            set { SetValue(MenuArrayProperty, value); }
        }

        public static readonly DependencyProperty MenuArrayProperty =
            DependencyProperty.Register("MenuArray", typeof(IEnumerable), typeof(MainWindow), new PropertyMetadata(null));
        public CircularMenuExample()
        {
            InitializeComponent();
            //var menuItemModels = new List<MenuItemModel>();
            //var angle = 0;
            //for (int i = 1; i <= 8; i++)
            //{
            //    var brushConverter = new BrushConverter();
            //    var brush = (Brush)brushConverter.ConvertFromString("#BAE766");
            //    if (IsOdd(i))
            //        brush = (Brush)brushConverter.ConvertFromString("#B0D440");

            //    menuItemModels.Add(new MenuItemModel { Angle = angle, Title = $"菜单{i}", FillColor = brush, IconImage = new BitmapImage(new Uri($"pack://application:,,,/Images/CircularMenu/{i}.png")) });
            //    angle += 45;
            //}
            //var menuItemModels = new List<CircularMenuItem>();
            //for (int i = 1; i <= 8; i++)
            //{
            //    menuItemModels.Add(new CircularMenuItem { MenuTxt = $"菜单{i}", IconImage = new BitmapImage(new Uri($"pack://application:,,,/Images/CircularMenu/{i}.png")) });
            //}
            var angle = 0;
            var menuItemModels = new List<MenuItemModel>();
            for (int i = 1; i <= 8; i++)
            {
                menuItemModels.Add(new MenuItemModel { Angle = angle, Title = $"菜单{i}", IconImage = new BitmapImage(new Uri($"pack://application:,,,/Images/CircularMenu/{i}.png")) });
                angle += 45;
            }
            MenuArray = menuItemModels;
        }
        //bool IsOdd(int num)
        //{
        //    return (num % 2) == 1;
        //}
    }
}
