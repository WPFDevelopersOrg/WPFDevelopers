using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class WaitLoading : Control
    {
        static WaitLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaitLoading),
                new FrameworkPropertyMetadata(typeof(WaitLoading)));
        }
    }
}