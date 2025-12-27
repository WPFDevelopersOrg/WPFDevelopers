using System.Windows;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    ///     NoneTitleBarWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NoneTitleBarWindow
    {
        public NoneTitleBarWindow()
        {
            InitializeComponent();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}