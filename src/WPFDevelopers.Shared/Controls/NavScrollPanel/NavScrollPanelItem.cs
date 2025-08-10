using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class NavScrollPanelItem : ContentControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(NavScrollPanelItem), new PropertyMetadata(string.Empty));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
    }
}