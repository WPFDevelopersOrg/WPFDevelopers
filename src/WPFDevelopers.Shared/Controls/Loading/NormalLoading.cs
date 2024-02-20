using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Expression.Drawing.Media;
using Microsoft.Expression.Drawing.Shapes;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = IndicatorTemplateName, Type = typeof(Arc))]
    public class NormalLoading : RangeBase
    {
        private const string IndicatorTemplateName = "PART_Indicator";

        public static readonly DependencyProperty ArcThicknessProperty = 
            DependencyProperty.Register("ArcThickness", typeof(double), typeof(NormalLoading),
                new PropertyMetadata(0d));

        public static readonly DependencyProperty IsIndeterminateProperty =
           ProgressBar.IsIndeterminateProperty.AddOwner(typeof(NormalLoading),
               new FrameworkPropertyMetadata(true));

        private Arc _indicator;

        public double ArcThickness
        {
            get => (double)GetValue(ArcThicknessProperty);
            set => SetValue(ArcThicknessProperty, value);
        }
        public bool IsIndeterminate
        {
            get => (bool)GetValue(IsIndeterminateProperty);
            set => SetValue(IsIndeterminateProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _indicator = GetTemplateChild(IndicatorTemplateName) as Arc;
            if (_indicator != null)
            {
                _indicator.StartAngle = 0;
                _indicator.EndAngle = 0;
            }
            SetProgressBarIndicatorAngle();
        }

        private void SetProgressBarIndicatorAngle()
        {
            if (_indicator == null) return;
            var minimum = Minimum;
            var maximum = Maximum;
            var num = Value;
            _indicator.EndAngle = (maximum <= minimum ? 0 : (num - minimum) / (maximum - minimum)) * 360;
        }
    }

}