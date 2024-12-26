using System.Windows;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    ///     NoChromeWindow.xaml 的交互逻辑
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