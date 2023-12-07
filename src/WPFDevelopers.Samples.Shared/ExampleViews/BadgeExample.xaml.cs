using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// BarrageExample.xaml 的交互逻辑
    /// </summary>
    public partial class BadgeExample : UserControl
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BadgeExample), new PropertyMetadata("3"));

        public BadgeExample()
        {
            InitializeComponent();
        }

        private void myButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Text = "10";
        }
    }
}
