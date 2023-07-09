using System.Windows;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    ///     Window1.xaml 的交互逻辑
    /// </summary>
    public partial class NoneNoChromeWindow
    {
        public NoneNoChromeWindow()
        {
            InitializeComponent();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}