using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    [ContentProperty(nameof(Content))]
    public class CircleMenuItem : Control
    {
        private const string RotateTransformTemplateName = "PART_RotateTransform";
        private static readonly Type _typeofSelf = typeof(CircleMenuItem);

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(CircleMenuItem),
                new UIPropertyMetadata(OnAngleChanged));

        public static readonly DependencyProperty RotateCenterProperty =
            DependencyProperty.Register(nameof(RotateCenter), typeof(double), typeof(CircleMenuItem),
                new PropertyMetadata(200.0, OnRotateCenterChanged));
        
        public static readonly DependencyProperty AlternationIndexProperty =
            DependencyProperty.Register("AlternationIndex", typeof(int), typeof(CircleMenuItem),
                new PropertyMetadata(0));

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(CircleMenuItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SectorGeometryProperty =
            DependencyProperty.Register(nameof(SectorGeometry), typeof(Geometry), typeof(CircleMenuItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SectorBackgroundProperty =
            DependencyProperty.Register(nameof(SectorBackground), typeof(Brush), typeof(CircleMenuItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register(nameof(SelectedBackground), typeof(Brush), typeof(CircleMenuItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(CircleMenuItem),
                new PropertyMetadata(false));

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

        public double RotateCenter
        {
            get => (double)GetValue(RotateCenterProperty);
            set => SetValue(RotateCenterProperty, value);
        }

        public int AlternationIndex
        {
            get => (int)GetValue(AlternationIndexProperty);
            set => SetValue(AlternationIndexProperty, value);
        }

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public Geometry SectorGeometry
        {
            get => (Geometry)GetValue(SectorGeometryProperty);
            set => SetValue(SectorGeometryProperty, value);
        }

        public Brush SectorBackground
        {
            get => (Brush)GetValue(SectorBackgroundProperty);
            set => SetValue(SectorBackgroundProperty, value);
        }

        public Brush SelectedBackground
        {
            get => (Brush)GetValue(SelectedBackgroundProperty);
            set => SetValue(SelectedBackgroundProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        #endregion

        #region Angle → RotateTransform

        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CircleMenuItem)d;
            control.UpdateAngle();
        }

        private static void OnRotateCenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CircleMenuItem)d;
            control.UpdateRotateCenter();
        }

        private void UpdateAngle()
        {
            if (_angleRotateTransform == null) return;
            _angleRotateTransform.Angle = Angle;
        }

        private void UpdateRotateCenter()
        {
            if (_angleRotateTransform == null) return;
            _angleRotateTransform.CenterX = RotateCenter;
            _angleRotateTransform.CenterY = RotateCenter;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _angleRotateTransform = GetTemplateChild(RotateTransformTemplateName) as RotateTransform;
            UpdateAngle();
            UpdateRotateCenter();
        }

        #endregion
    }
}
