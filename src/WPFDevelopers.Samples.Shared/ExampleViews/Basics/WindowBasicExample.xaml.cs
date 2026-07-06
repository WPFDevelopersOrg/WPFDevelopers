using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews.Basics
{
    public partial class WindowBasicExample : UserControl
    {
        public WindowBasicExample() { InitializeComponent(); }
        private void Button_Click(object sender, RoutedEventArgs e) => new ToolWindow().Show();
        private void ButtonNone_Click(object sender, RoutedEventArgs e) => new NoneTitleBarWindow().Show();
    }
}
