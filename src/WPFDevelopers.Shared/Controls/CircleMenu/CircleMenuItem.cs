using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class CircleMenuItem : Control
    {
        private const string RotateTransformTemplateName = "PART_RotateTransform";
        private static readonly Type _typeofSelf = typeof(CircleMenuItem);

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(CircleMenuItem),
                new UIPropertyMetadata(OnAngleChanged));

        public static readonly DependencyProperty MenuTxtProperty =
            DependencyProperty.Register("MenuTxt", typeof(string), typeof(CircleMenuItem),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(CircleMenuItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IconImageProperty =
            DependencyProperty.Register("IconImage", typeof(ImageSource), typeof(CircleMenuItem),
                new PropertyMetadata(null));

        private RotateTransform _angleRotateTransform;

        static CircleMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(_typeofSelf, new FrameworkPropertyMetadata(_typeofSelf));
        }

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public string MenuTxt
        {
            get => (string)GetValue(MenuTxtProperty);
            set => SetValue(MenuTxtProperty, value);
        }

        public Brush BackgroundColor
        {
            get => (Brush)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public ImageSource IconImage
        {
            get => (ImageSource)GetValue(IconImageProperty);
            set => SetValue(IconImageProperty, value);
        }

        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CircleMenuItem)d;
            control.UpdateAngle();
        }

        private void UpdateAngle()
        {
            if (_angleRotateTransform == null) return;
            _angleRotateTransform.Angle = Angle;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _angleRotateTransform = GetTemplateChild(RotateTransformTemplateName) as RotateTransform;
            UpdateAngle();
        }
    }
}