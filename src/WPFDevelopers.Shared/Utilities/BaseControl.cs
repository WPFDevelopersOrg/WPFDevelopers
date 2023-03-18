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
        public static object GetIsAddChild(UIElement element)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            return (object)element.GetValue(IsAddChildProperty);
        }

        public static void SetIsAddChild(UIElement element, object child)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            element.SetValue(IsAddChildProperty, child);
        }

        public bool IsAddChild
        {
            get { return (bool)GetValue(IsAddChildProperty); }
            set { SetValue(IsAddChildProperty, value); }
        }

        public static readonly DependencyProperty IsAddChildProperty =
            DependencyProperty.Register("IsAddChild", typeof(bool), typeof(BaseControl), new PropertyMetadata(false));

        //public static object GetOldChild(UIElement element)
        //{
        //    if (element == null) { throw new ArgumentNullException("element"); }

        //    return (object)element.GetValue(OldChildProperty);
        //}

        //public static void SetOldChild(UIElement element, object child)
        //{
        //    if (element == null) { throw new ArgumentNullException("element"); }

        //    element.SetValue(OldChildProperty, child);
        //}

        //public object OldChild
        //{
        //    get { return (object)GetValue(OldChildProperty); }
        //    set { SetValue(OldChildProperty, value); }
        //}

        //public static readonly DependencyProperty OldChildProperty =
        //    DependencyProperty.Register("OldChild", typeof(object), typeof(BaseControl), new PropertyMetadata(null));
    }
}
