using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// DrawerExample.xaml 的交互逻辑
    /// </summary>
    public partial class DrawerExample : UserControl
    {
        public DrawerExample()
        {
            InitializeComponent();
        }

        private void ButtonTop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MyDrawerTop.IsOpen = true;
        }

        private void ButtonBottom_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MyDrawerBottom.IsOpen = true;
        }
        private void ButtonLeft_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MyDrawerLeft.IsOpen = true;
        }
        private void ButtonRight_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MyDrawerRight.IsOpen = true;
        }
    }
}
