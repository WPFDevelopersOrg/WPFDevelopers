using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// MessageExample.xaml 的交互逻辑
    /// </summary>
    public partial class MessageExample : UserControl
    {
        public MessageExample()
        {
            InitializeComponent();
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            switch (btn.Tag)
            {
                case "Info":
                    Message.Push(App.Current.MainWindow, "This is a info message", MessageBoxImage.Information);
                    break;
                case "Error":
                    Message.Push("This is a error message", MessageBoxImage.Error, true);
                    break;
                case "Warning":
                    Message.Push("This is a warning message", MessageBoxImage.Warning, true);
                    break;
                case "Question":
                    Message.Push("This is a question message", MessageBoxImage.Question);
                    break;
                default:
                    Message.Push("这是一条很长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长消息", MessageBoxImage.Information);
                    break;
            }

        }
    }
}
