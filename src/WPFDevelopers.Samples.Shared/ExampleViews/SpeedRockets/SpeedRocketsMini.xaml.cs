using System;
using System.Windows;
using System.Windows.Data;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// SpeedRocketsMini.xaml 的交互逻辑
    /// </summary>
    public partial class SpeedRocketsMini : Window
    {
        public SpeedRocketsMini()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    public class ActualHeightConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 0 - (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
