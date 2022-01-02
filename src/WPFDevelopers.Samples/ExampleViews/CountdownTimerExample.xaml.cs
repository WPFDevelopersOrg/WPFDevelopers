using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// CountdownTimerExample.xaml 的交互逻辑
    /// </summary>
    public partial class CountdownTimerExample : UserControl
    {
        public CountdownTimerExample()
        {
            InitializeComponent();
        }

        private void NavigateMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems[0] as ListBoxItem;
            if (item == null) return;
            switch (item.Content.ToString())
            {
                case "Default":
                    if(CountdownTimer1.Visibility != Visibility.Visible)
                    {
                        CountdownTimer1.Visibility = Visibility.Visible;
                        CountdownTimerGroup.Visibility = Visibility.Collapsed;
                    }
                    break;
                case "MultiColor":
                    if (CountdownTimerGroup.Visibility != Visibility.Visible)
                    {
                        CountdownTimerGroup.Visibility = Visibility.Visible;
                        CountdownTimer1.Visibility = Visibility.Collapsed;
                    }
                    break;
            }
        }
    }
}
