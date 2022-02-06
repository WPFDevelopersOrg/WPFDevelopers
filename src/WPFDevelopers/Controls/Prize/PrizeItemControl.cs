using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = RotateTransformTemplateName, Type = typeof(RotateTransform))]
    public class PrizeItemControl : Control
    {
        private static readonly Type _typeofSelf = typeof(PrizeItemControl);
        private const string RotateTransformTemplateName = "PART_RotateTransform";
        private RotateTransform _angleRotateTransform;
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(PrizeItemControl), new UIPropertyMetadata(OnAngleChanged));

        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PrizeItemControl control = (PrizeItemControl)d;
            control.UpdateAngle();
        }
        void UpdateAngle()
        {
            if (_angleRotateTransform == null) return;
            _angleRotateTransform.Angle = Angle;
        }
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(PrizeItemControl), new PropertyMetadata(string.Empty));


        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }
        public static readonly DependencyProperty BackgroundColorProperty =
           DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(PrizeItemControl), new PropertyMetadata(null));


        static PrizeItemControl()
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
