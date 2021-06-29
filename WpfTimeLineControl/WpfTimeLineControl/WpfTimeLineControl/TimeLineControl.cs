using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WpfTimeLineControl
{
    public class TimeLineControl:Selector
    {
        static TimeLineControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeLineControl), new FrameworkPropertyMetadata(typeof(TimeLineControl)));
        }
    }
}
