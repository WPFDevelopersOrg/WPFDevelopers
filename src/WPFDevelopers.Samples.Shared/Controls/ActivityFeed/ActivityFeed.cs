using System.Windows;
using System.Windows.Controls.Primitives;

namespace WPFDevelopers.Samples.Controls
{
    public class ActivityFeed : Selector
    {
        static ActivityFeed()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ActivityFeed), new FrameworkPropertyMetadata(typeof(ActivityFeed)));
        }
    }
}