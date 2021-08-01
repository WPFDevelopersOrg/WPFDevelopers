using System.Windows;
using System.Windows.Controls.Primitives;

namespace WPFDevelopers.Controls
{
    public class TimeLine:Selector
    {
        static TimeLine()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeLine), new FrameworkPropertyMetadata(typeof(TimeLine)));
        }
    }
}
