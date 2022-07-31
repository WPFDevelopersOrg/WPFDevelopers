using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// BubbleControlExample.xaml 的交互逻辑
    /// </summary>
    public partial class BubblleControlExample : UserControl
    {
        public BubblleControlExample()
        {
            InitializeComponent();
        }
        public ICommand ClickCommand => new RelayCommand(delegate
        {
           WPFDevelopers.Minimal.Controls.MessageBox.Show("点击完成。");
        });

        private void BubblleControl_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBox.Show($"点击了“ {MyBubblleControl.SelectedText}开发者 ”.", "提示",MessageBoxButton.OK,MessageBoxImage.Information);
        }
    }
}
