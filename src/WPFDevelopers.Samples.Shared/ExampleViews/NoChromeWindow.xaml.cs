using System.Windows;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    ///     Window1.xaml 的交互逻辑
    /// </summary>
    public partial class NoChromeWindow 
    {
        public NoChromeWindow()
        {
            InitializeComponent();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}