using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// BubbleSelectionExample.xaml 的交互逻辑
    /// </summary>
    public partial class BubbleSelectionExample : UserControl
    {
        public BubbleSelectionExample()
        {
            InitializeComponent();
        }
        public ICommand ClickCommand => new RelayCommand(delegate
        {
           WPFDevelopers.Controls.MessageBox.Show("点击完成。");
        });

        private void BubbleSelection_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBox.Show($"点击了“ {MyBubbleSelection.SelectedText}开发者 ”.", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
