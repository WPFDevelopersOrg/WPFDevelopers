using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Samples.Models;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// DrawPrizeExample.xaml 的交互逻辑
    /// </summary>
    public partial class DrawPrizeExample : UserControl
    {
        public IEnumerable MenuArray
        {
            get { return (IEnumerable)GetValue(MenuArrayProperty); }
            set { SetValue(MenuArrayProperty, value); }
        }

        public static readonly DependencyProperty MenuArrayProperty =
            DependencyProperty.Register("MenuArray", typeof(IEnumerable), typeof(DrawPrizeExample), new PropertyMetadata(null));

      
        public List<int> ListAngle
        {
            get { return (List<int>)GetValue(ListAngleProperty); }
            set { SetValue(ListAngleProperty, value); }
        }

        public static readonly DependencyProperty ListAngleProperty =
            DependencyProperty.Register("ListAngle", typeof(List<int>), typeof(DrawPrizeExample), new PropertyMetadata());


       
        public DrawPrizeExample()
        {
            InitializeComponent();
            this.Loaded += DrawPrizeExample_Loaded;
        }

        private void DrawPrizeExample_Loaded(object sender, RoutedEventArgs e)
        {
            ListAngle = new List<int>();
            var menuItemModels = new List<MenuItemModel>();
            var angle = 0;
            var anglePrize = 2000;
            for (int i = 0; i <= 7; i++)
            {
                var prizeTitle = i == 0 ? "谢谢参与" : $"{i}等奖";
                angle += 45;
                anglePrize += 45;
                ListAngle.Add(anglePrize);
                menuItemModels.Add(new MenuItemModel { Angle = angle, Title = prizeTitle});
            }

            MenuArray = menuItemModels;
        }
       
    }
}
