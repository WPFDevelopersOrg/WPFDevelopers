using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// WaterfallPanelExample.xaml 的交互逻辑
    /// </summary>
    public partial class WaterfallPanelExample : UserControl
    {
        private bool _imageFailureReported;

        public WaterfallPanelExample()
        {
            InitializeComponent();
        }

        private void Image_Failed(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine("[Waterfall] Image failed: " + e.ErrorException);
            if (_imageFailureReported)
                return;

            _imageFailureReported = true;
            WPFDevelopers.Controls.MessageBox.Show(e.ErrorException.ToString(),
                "Waterfall image loading failed");
        }
    }
}
