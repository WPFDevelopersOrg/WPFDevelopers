using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class RollLoading : ContentControl
    {
        static RollLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RollLoading), new FrameworkPropertyMetadata(typeof(RollLoading)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }





        public Color ForegroundColor
        {
            get { return (Color)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForegroundColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register("ForegroundColor", typeof(Color), typeof(RollLoading), new PropertyMetadata(Colors.Red));





        public bool IsStart
        {
            get { return (bool)GetValue(IsStartProperty); }
            set { SetValue(IsStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStartProperty =
            DependencyProperty.Register("IsStart", typeof(bool), typeof(RollLoading), new PropertyMetadata(true));


    }
}
