using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Samples.Controls
{
    public class PieSegment : Control
    {
        public static readonly DependencyProperty PieSegmentModelsProperty =
            DependencyProperty.Register("PieSegmentModels", typeof(ObservableCollection<PieSegmentModel>),
                typeof(PieSegment), new UIPropertyMetadata(OnPieSegmentModelChanged));

        public static readonly DependencyProperty ArcThicknessProperty =
            DependencyProperty.Register("ArcThickness", typeof(double), typeof(PieSegment), new PropertyMetadata(1.0));

        /// <summary>
        ///     colors
        /// </summary>
        private readonly string[] ColorArray = { "#FDC006", "#607E89", "#2095F2", "#F34336" };


        static PieSegment()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PieSegment),
                new FrameworkPropertyMetadata(typeof(PieSegment)));
        }

        public ObservableCollection<PieSegmentModel> PieSegmentModels
        {
            get => (ObservableCollection<PieSegmentModel>)GetValue(PieSegmentModelsProperty);
            set => SetValue(PieSegmentModelsProperty, value);
        }


        /// <summary>
        ///     0~1
        /// </summary>
        public double ArcThickness
        {
            get => (double)GetValue(ArcThicknessProperty);
            set => SetValue(ArcThicknessProperty, value);
        }

        private static void OnPieSegmentModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pieSegment = d as PieSegment;
            if (e.NewValue != null)
            {
                var array = e.NewValue as ObservableCollection<PieSegmentModel>;
                double angleNum = 0;
                foreach (var item in array)
                {
                    var color = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(pieSegment.ColorArray[array.IndexOf(item)]));
                    item.Color = color;
                    item.StartAngle = angleNum;
                    item.EndAngle = angleNum + item.Value / 100 * 360;
                    angleNum = item.EndAngle;
                }
            }
        }
    }
}