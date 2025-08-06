using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class WDUserControl : ContentControl
    {
        static WDUserControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WDUserControl),
                new FrameworkPropertyMetadata(typeof(WDUserControl)));
        }
    }
}
