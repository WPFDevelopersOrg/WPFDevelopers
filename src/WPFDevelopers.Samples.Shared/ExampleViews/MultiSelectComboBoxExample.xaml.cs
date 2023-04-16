using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// MultiSelectComboBoxExample.xaml 的交互逻辑
    /// </summary>
    public partial class MultiSelectComboBoxExample : UserControl
    {
        public MultiSelectComboBoxExample()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WPFDevelopers.Controls.MessageBox.Show($"{MyMultiSelectComboBox.Text} \r\n总共选中:{MyMultiSelectComboBox.SelectedItems.Count} 条", "选中内容", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
