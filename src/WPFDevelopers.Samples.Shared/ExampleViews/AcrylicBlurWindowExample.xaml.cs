#if NET40
using Microsoft.Windows.Shell;
#else
using System.Windows.Shell;
# endif
using System.Windows;
using System.Windows.Controls;
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
#if NET40
            var windowChrome = new WindowChrome
            {
                GlassFrameThickness = new Thickness(0, 1, 0, 0)
            };
            WindowChrome.SetIsHitTestVisibleInChrome(myStackPanel, true);
            WindowChrome.SetIsHitTestVisibleInChrome(MyStackPanelContent, true);
#else
            var windowChrome = new WindowChrome
            {
                GlassFrameThickness = new Thickness(0,1,0,0)
            };
            WindowChrome.SetIsHitTestVisibleInChrome(myStackPanel, true);
            WindowChrome.SetIsHitTestVisibleInChrome(MyStackPanelContent, true);
#endif
        }
        public ICommand CloseCommand => new RelayCommand(obj =>
        {
            Close();
        });
    }
}
