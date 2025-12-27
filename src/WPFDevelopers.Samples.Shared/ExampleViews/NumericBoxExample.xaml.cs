using System.Windows.Controls;
using WPFDevelopers.Core;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// MessageExample.xaml 的交互逻辑
    /// </summary>
    public partial class NumericBoxExample : UserControl
    {
        public NumericBoxExample()
        {
            InitializeComponent();
        }
        private void NumericBox_IncreaseClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (e is NumericBoxClickEventArgs args)
                args.SkipStepChange = true;
            MyNumericBox.Value = MyNumericBox.Value * 10;
        }

        private void NumericBox_DecreaseClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (e is NumericBoxClickEventArgs args)
                args.SkipStepChange = true;
            MyNumericBox.Value = MyNumericBox.Value / 10;
        }
    }
}
