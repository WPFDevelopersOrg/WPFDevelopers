using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class CircularMenuItem : Control
    {
        private static readonly Type _typeofSelf = typeof(CircularMenuItem);
        private const string RotateTransformTemplateName = "PART_RotateTransform";
        private RotateTransform _angleRotateTransform;
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(CircularMenuItem), new UIPropertyMetadata(OnAngleChanged));

        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularMenuItem control = (CircularMenuItem)d;
            control.UpdateAngle();
        }
        void UpdateAngle()
        {
            if (_angleRotateTransform == null) return;
            _angleRotateTransform.Angle = Angle;
        }
        public string MenuTxt
        {
            get { return (string)GetValue(MenuTxtProperty); }
            set { SetValue(MenuTxtProperty, value); }
        }

        public static readonly DependencyProperty MenuTxtProperty =
            DependencyProperty.Register("MenuTxt", typeof(string), typeof(CircularMenuItem), new PropertyMetadata(string.Empty));



        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }
        public static readonly DependencyProperty BackgroundColorProperty =
           DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(CircularMenuItem), new PropertyMetadata(null));

        public ImageSource IconImage
        {
            get { return (ImageSource)GetValue(IconImageProperty); }
            set { SetValue(IconImageProperty, value); }
        }
        public static readonly DependencyProperty IconImageProperty =
            DependencyProperty.Register("IconImage", typeof(ImageSource), typeof(CircularMenuItem), new PropertyMetadata(null));

        static CircularMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(_typeofSelf, new FrameworkPropertyMetadata(_typeofSelf));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _angleRotateTransform = GetTemplateChild(RotateTransformTemplateName) as RotateTransform;
            UpdateAngle();
        }
    }
}
