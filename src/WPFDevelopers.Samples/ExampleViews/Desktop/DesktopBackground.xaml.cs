using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Samples.ExampleViews.Desktop;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// WorkerWBackground.xaml 的交互逻辑
    /// </summary>
    public partial class DesktopBackground : UserControl
    {
        
        public DesktopBackground()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new DesktopPlayVideo().Show();
        } 
    }
}
