using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// MagnifierExample.xaml 的交互逻辑
    /// </summary>
    public partial class MagnifierExample : UserControl
    {
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(MagnifierExample), new PropertyMetadata(false));
        public MagnifierExample()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var magnifierDesktop = new MagnifierDesktop();
            if (IsChecked)
            {
                App.CurrentMainWindow.WindowState = WindowState.Minimized;
                magnifierDesktop.Show();
                magnifierDesktop.Activate();
            }
            else
                magnifierDesktop.ShowDialog();
        }
    }
}
