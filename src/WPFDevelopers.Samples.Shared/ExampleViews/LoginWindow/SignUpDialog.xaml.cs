using System.Windows;
#if NET40
using Microsoft.Windows.Shell;
#else
using System.Windows.Shell;
# endif

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class SignUpDialog : WindowBase
    {
        public SignUpDialog()
        {
            InitializeComponent();
#if NET40
            var windowChrome = new WindowChrome
            {
                CaptionHeight = 100,
                GlassFrameThickness = new Thickness(1)
            };
#else
            var windowChrome = new WindowChrome
            {
                CaptionHeight = 100,
                GlassFrameThickness = new Thickness(1),
                UseAeroCaptionButtons = false
            };
#endif
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow();
        }

     
    }
}