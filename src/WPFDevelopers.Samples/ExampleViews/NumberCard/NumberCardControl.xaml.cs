using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews.NumberCard
{
    /// <summary>
    /// NumberCardControl.xaml 的交互逻辑
    /// </summary>
    public partial class NumberCardControl : UserControl
    {
        public string Number
        {
            get { return (string)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register("Number", typeof(string), typeof(NumberCardControl), new PropertyMetadata("10"));

        public string NextNumber
        {
            get { return (string)GetValue(NextProperty); }
            set { SetValue(NextProperty, value); }
        }

        public static readonly DependencyProperty NextProperty =
            DependencyProperty.Register("NextNumber", typeof(string), typeof(NumberCardControl), new PropertyMetadata("0"));
        public NumberCardControl()
        {
            InitializeComponent();
        }
    }
}
