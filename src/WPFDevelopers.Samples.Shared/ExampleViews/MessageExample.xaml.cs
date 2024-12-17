using System;
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                if (Enum.TryParse(radioButton.Content.ToString(), out Position position))
                    Message.SetPosition(position);
            }
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
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Message.Clear();
        }
        private void BtnDesktopClear_Click(object sender, RoutedEventArgs e)
        {
            Message.ClearDesktop();
        }
        

        private void AddButtonDesktop_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            switch (btn.Tag)
            {
                case "Info":
                    Message.PushDesktop("This is a info message", MessageBoxImage.Information);
                    break;
                case "Error":
                    Message.PushDesktop("This is a error message", MessageBoxImage.Error, true);
                    break;
                case "Warning":
                    Message.PushDesktop("This is a warning message", MessageBoxImage.Warning, true);
                    break;
                case "Question":
                    Message.PushDesktop("This is a question message", MessageBoxImage.Question);
                    break;
                default:
                    Message.PushDesktop("这是一条很长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长长消息", MessageBoxImage.Information);
                    break;
            }
        }
    }
}
