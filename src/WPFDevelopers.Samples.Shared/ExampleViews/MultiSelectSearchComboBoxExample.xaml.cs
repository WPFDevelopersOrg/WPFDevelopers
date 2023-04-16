using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// MultiSelectComboBoxExample.xaml 的交互逻辑
    /// </summary>
    public partial class MultiSelectSearchComboBoxExample : UserControl
    {
        public MultiSelectSearchComboBoxExample()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WPFDevelopers.Controls.MessageBox.Show($"{MyMultiSelectionSearchComboBox2.Text} \r\n总共选中:{MyMultiSelectionSearchComboBox2.SelectedItems.Count} 条","选中内容",MessageBoxButton.OK,MessageBoxImage.Information);      
        }
    }
}
