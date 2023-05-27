using System.Windows;
using System.Windows.Controls.Primitives;

namespace WPFDevelopers.Samples.Controls
{
    public class TimeLine : Selector
    {
        static TimeLine()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeLine), new FrameworkPropertyMetadata(typeof(TimeLine)));
        }
    }
}