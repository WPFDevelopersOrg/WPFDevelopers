using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class FireControl : Control
    {
        static FireControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FireControl), new FrameworkPropertyMetadata(typeof(FireControl)));
        }

        public bool IsStart
        {
            get { return (bool)GetValue(IsStartProperty); }
            set { SetValue(IsStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStartProperty =
            DependencyProperty.Register("IsStart", typeof(bool), typeof(FireControl), new PropertyMetadata(default(bool)));
    }
}
