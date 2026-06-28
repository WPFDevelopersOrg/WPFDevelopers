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

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CircleMenuItem),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty AlternationIndexProperty =
            DependencyProperty.Register("AlternationIndex", typeof(int), typeof(CircleMenuItem),
                new PropertyMetadata(0));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(CircleMenuItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SectorGeometryProperty =
            DependencyProperty.Register(nameof(SectorGeometry), typeof(Geometry), typeof(CircleMenuItem),
                new PropertyMetadata(null));

        private RotateTransform _angleRotateTransform;

        static CircleMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(_typeofSelf, new FrameworkPropertyMetadata(_typeofSelf));
        }

        #region Properties

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public int AlternationIndex
        {
            get => (int)GetValue(AlternationIndexProperty);
            set => SetValue(AlternationIndexProperty, value);
        }

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public Geometry SectorGeometry
        {
            get => (Geometry)GetValue(SectorGeometryProperty);
            set => SetValue(SectorGeometryProperty, value);
        }

        #endregion

        #region Angle → RotateTransform

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

        #endregion
    }
}
