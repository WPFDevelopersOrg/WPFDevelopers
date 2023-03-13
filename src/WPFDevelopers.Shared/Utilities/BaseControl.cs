using System.Windows;
using System;
using System.Windows.Controls;

namespace WPFDevelopers.Utilities
{
    public class BaseControl : Control
    {
        public static object GetChild(UIElement element)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            return (object)element.GetValue(ChildProperty);
        }

        public static void SetChild(UIElement element, object child)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            element.SetValue(ChildProperty, child);
        }
        public object Child
        {
            get { return (object)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(object), typeof(BaseControl), new PropertyMetadata(null));
    }
}
