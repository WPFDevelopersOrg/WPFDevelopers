using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfCircularMenu
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public IEnumerable MenuArray
        {
            get { return (IEnumerable)GetValue(MenuArrayProperty); }
            set { SetValue(MenuArrayProperty, value); }
        }

        public static readonly DependencyProperty MenuArrayProperty =
            DependencyProperty.Register("MenuArray", typeof(IEnumerable), typeof(MainWindow), new PropertyMetadata(null));
        public MainWindow()
        {
            InitializeComponent();
            var menuItemModels = new List<MenuItemModel>();
            var angle = 0;
            for (int i = 1; i <= 8; i++)
            {
                var brushConverter = new BrushConverter();
                var brush = (Brush)brushConverter.ConvertFromString("#BAE766");
                if (IsOdd(i))
                    brush = (Brush)brushConverter.ConvertFromString("#B0D440");

                menuItemModels.Add(new MenuItemModel { Angle = angle, Title = $"菜单{i}", FillColor = brush, IconImage = new BitmapImage(new Uri($"pack://application:,,,/Images/{i}.png")) });
                angle += 45;
            }

            MenuArray = menuItemModels;
        }
        bool IsOdd(int num)
        {
            return (num % 2) == 1;
        }
    }
    public class MenuItemModel
    {
        public double Angle { get; set; }
        public string Title { get; set; }
        public Brush FillColor { get; set; }
        public ImageSource IconImage { get; set; }
    }
}
