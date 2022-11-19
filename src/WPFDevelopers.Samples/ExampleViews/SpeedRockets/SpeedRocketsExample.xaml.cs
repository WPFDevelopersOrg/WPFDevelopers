using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// SpeedRocketsExample.xaml 的交互逻辑
    /// </summary>
    public partial class SpeedRocketsExample : UserControl
    {
        public SpeedRocketsExample()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new SpeedRocketsMini().Show();
            //AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(App.CurrentMainWindow);
            //var speedRocketsAdorner = new SpeedRocketsAdorner(adornerLayer)
            //{
            //    Child = new Border()
            //    {
            //        Background =Brushes.Transparent,
            //        Child = new UserControl1()
            //    }
            //};
            //adornerLayer.Add(speedRocketsAdorner);
        }

    }
}
