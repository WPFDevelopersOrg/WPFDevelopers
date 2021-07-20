using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class CircularMenu : ItemsControl
    {
        static CircularMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularMenu), new FrameworkPropertyMetadata(typeof(CircularMenu)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AlternationCount = 8;
            //AlternationCount = this.Items.Count;
        }
        
    }
}
