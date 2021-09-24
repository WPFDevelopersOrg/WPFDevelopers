using System;
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
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// ScreenCutExample.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenCutExample : UserControl
    {
        public ScreenCutExample()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var screenCut = new ScreenCut();
            screenCut.ShowDialog();
        }
    }
}
