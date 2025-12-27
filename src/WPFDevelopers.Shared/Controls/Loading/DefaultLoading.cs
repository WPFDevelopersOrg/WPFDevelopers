using Microsoft.Expression.Drawing.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = IndicatorTemplateName, Type = typeof(Arc))]
    public class DefaultLoading : Control
    {
        private const string IndicatorTemplateName = "PART_Indicator";

        public static readonly DependencyProperty ArcThicknessProperty = 
            DependencyProperty.Register("ArcThickness", typeof(double),typeof(DefaultLoading), new PropertyMetadata(0d));

        private Arc _indicator;

        public static DefaultLoading Default = new DefaultLoading();

        public double ArcThickness
        {
            get => (double)GetValue(ArcThicknessProperty);
            set => SetValue(ArcThicknessProperty, value);
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
        }
    }
}