using System.Windows;
using System.Windows.Input;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// AcrylicBlurWindowExample.xaml 的交互逻辑
    /// </summary>
    public partial class AcrylicBlurWindowExample : Window
    {
        public AcrylicBlurWindowExample()
        {
            InitializeComponent();
        }
        public ICommand CloseCommand => new RelayCommand(obj =>
        {
           Close();
        });
    }
}
