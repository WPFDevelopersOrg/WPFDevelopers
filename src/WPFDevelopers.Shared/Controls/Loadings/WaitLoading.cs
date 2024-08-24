using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class WaitLoading : LoadingBase
    {
        static WaitLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaitLoading),
                new FrameworkPropertyMetadata(typeof(WaitLoading)));
        }
    }
}