using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class BreadCrumbBarItem : ListBoxItem
    {
        private static readonly Type _typeofSelf = typeof(BreadCrumbBarItem);
        static BreadCrumbBarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(_typeofSelf,
                new FrameworkPropertyMetadata(_typeofSelf));
        }
    }
}
