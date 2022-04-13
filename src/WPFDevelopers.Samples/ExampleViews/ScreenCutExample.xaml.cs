using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// ScreenCutExample.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenCutExample : UserControl
    {
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ScreenCutExample), new PropertyMetadata(false));


        public ScreenCutExample()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var screenCut = new ScreenCut();
            if (IsChecked)
            {
                App.CurrentMainWindow.WindowState = WindowState.Minimized;
                screenCut.Show();
                screenCut.Activate();
            }
            else
                screenCut.ShowDialog();
        }
    }
}
