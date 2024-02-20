using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WPFDevelopers.Samples.Controls
{
    public class TransformLayout : ContentControl
    {
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(TransformLayout), new PropertyMetadata(0d, OnAngleChanged));

        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           
        }
        static TransformLayout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransformLayout), new FrameworkPropertyMetadata(typeof(TransformLayout)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            IsVisibleChanged += TransformContent_IsVisibleChanged;
            CreateAdorner();
        }
        void CreateAdorner()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            if (adornerLayer != null)
            {
                var adorners = adornerLayer.GetAdorners(this);
                if (adorners != null)
                    foreach (var item in adorners)
                        if (item is ElementAdorner container)
                            adornerLayer.Remove(container);
                var adorner = new ElementAdorner(this);
                adorner.AngleChanged -= Adorner_AngleChanged;
                adorner.AngleChanged += Adorner_AngleChanged;
                adornerLayer.Add(adorner);
            }
        }

        private void Adorner_AngleChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Angle = e.NewValue;
        }

        private void TransformContent_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isVisible)
            {
                if(isVisible)
                {
                    CreateAdorner();
                }
            }
        }
    }
}
