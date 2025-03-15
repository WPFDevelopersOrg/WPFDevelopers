using System.Linq;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// TagExample.xaml 的交互逻辑
    /// </summary>
    public partial class TagExample : UserControl
    {
        public TagExample()
        {
            InitializeComponent();
            Loaded += TagExample_Loaded;
        }

        private void TagExample_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

            foreach (Tag item in MyStackPanel.Children.OfType<Tag>())
            {
                item.Close += Tag_Close;
            }
        }

        private void Tag_Close(object sender, System.Windows.RoutedEventArgs e)
        {
            if(sender is Tag tag)
                MyStackPanel.Children.Remove(tag);
        }
    }
}
