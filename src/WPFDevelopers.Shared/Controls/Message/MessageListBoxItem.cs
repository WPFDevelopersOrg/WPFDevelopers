using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class MessageListBoxItem : ListBoxItem
    {
        public MessageBoxImage MessageType
        {
            get { return (MessageBoxImage)GetValue(MessageTypeProperty); }
            set { SetValue(MessageTypeProperty, value); }
        }
        public static readonly DependencyProperty MessageTypeProperty =
            DependencyProperty.Register("MessageType", typeof(MessageBoxImage), typeof(MessageListBoxItem), new PropertyMetadata(MessageBoxImage.Information));

        public bool IsCenter
        {
            get { return (bool)GetValue(IsCenterProperty); }
            set { SetValue(IsCenterProperty, value); }
        }

        public static readonly DependencyProperty IsCenterProperty =
            DependencyProperty.Register("IsCenter", typeof(bool), typeof(MessageListBoxItem), new PropertyMetadata(false));

    }
}
