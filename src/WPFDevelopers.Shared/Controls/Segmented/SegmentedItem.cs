using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class SegmentedItem : RadioButton
    {
        static SegmentedItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SegmentedItem),
                new FrameworkPropertyMetadata(typeof(SegmentedItem)));
        }
    }
}
