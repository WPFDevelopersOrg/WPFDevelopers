using ICSharpCode.AvalonEdit.Document;
using System.Collections.Generic;
using System.Linq;
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
            Loaded += MultiSelectSearchComboBoxExample_Loaded;
        }

        private void MultiSelectSearchComboBoxExample_Loaded(object sender, RoutedEventArgs e)
        {
            var list = new List<string>();
            for (int i = 0; i < 10; i++)
                list.Add(i.ToString());
            MyMultiSelectionSearchComboBox2.ItemsSource = list;
            MyMultiSelectionSearchComboBox2.SelectedItems = list.Skip(3).ToList();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WPFDevelopers.Controls.MessageBox.Show($"{MyMultiSelectionSearchComboBox2.Text} \r\n总共选中:{MyMultiSelectionSearchComboBox2.SelectedItems.Count} 条","选中内容",MessageBoxButton.OK,MessageBoxImage.Information);      
        }
    }
}
