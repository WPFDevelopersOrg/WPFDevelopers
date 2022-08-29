using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = RotateTransformTemplateName, Type = typeof(RotateTransform))]
    public class PrizeItemControl : Control
    {
        private const string RotateTransformTemplateName = "PART_RotateTransform";
        private static readonly Type _typeofSelf = typeof(PrizeItemControl);

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(PrizeItemControl),
                new UIPropertyMetadata(OnAngleChanged));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(PrizeItemControl),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(PrizeItemControl),
                new PropertyMetadata(null));

        private RotateTransform _angleRotateTransform;


        static PrizeItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(_typeofSelf, new FrameworkPropertyMetadata(_typeofSelf));
        }

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public Brush BackgroundColor
        {
            get => (Brush)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PrizeItemControl)d;
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