using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = ArcSegmentTemplateName, Type = typeof(ArcSegment))]
    [TemplatePart(Name = ArcSegmentAngleTemplateName, Type = typeof(ArcSegment))]
    [TemplatePart(Name = PathFigureTemplateName, Type = typeof(PathFigure))]
    [TemplatePart(Name = PathFigureAngleTemplateName, Type = typeof(PathFigure))]
    [TemplatePart(Name = TextBlockTemplateName, Type = typeof(TextBlock))]
    public class CircularProgressBar : ProgressBar
    {
        private const string ArcSegmentTemplateName = "PART_ArcSegment";
        private const string ArcSegmentAngleTemplateName = "PART_ArcSegmentAngle";
        private const string PathFigureTemplateName = "PART_PathFigure";
        private const string PathFigureAngleTemplateName = "PART_PathFigureAngle";
        private const string TextBlockTemplateName = "PART_TextBlock";
        private ArcSegment _arcSegment, _arcSegmentAngle;
        private PathFigure _pathFigure, _pathFigureAngle;
        private TextBlock _textBlock;


        public static readonly DependencyProperty SizeProperty =
           DependencyProperty.Register("Size", typeof(Size), typeof(CircularProgressBar),
               new PropertyMetadata(new Size(50,50)));
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(CircularProgressBar),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CircularProgressBar),
                new PropertyMetadata(10.0));

        public static readonly DependencyProperty BrushStrokeThicknessProperty =
            DependencyProperty.Register("BrushStrokeThickness", typeof(double), typeof(CircularProgressBar),
                new PropertyMetadata(1.0));

        public CircularProgressBar()
        {
            ValueChanged += CircularProgressBar_ValueChanged;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
           
            if (Size.Width != Size.Height)
            {
                var max = Math.Max(Size.Width, Size.Height);
                Size = new Size(max, max);
            }
           
            _pathFigure = GetTemplateChild(PathFigureTemplateName) as PathFigure;
            _pathFigureAngle = GetTemplateChild(PathFigureAngleTemplateName) as PathFigure;
            _pathFigure.StartPoint = new Point(Size.Width, 0);
            _pathFigureAngle.StartPoint = new Point(Size.Width, 0);
            _arcSegment = GetTemplateChild(ArcSegmentTemplateName) as ArcSegment;
            _arcSegment.Size = Size;
            _arcSegment.Point = new Point(Size.Width - 0.000872664626, 7.61543361704753E-09);
            _arcSegmentAngle = GetTemplateChild(ArcSegmentAngleTemplateName) as ArcSegment;
            _arcSegmentAngle.Size = Size;
            _textBlock = GetTemplateChild(TextBlockTemplateName) as TextBlock;
            if (Size.Width < 15)
            {
                FontSize = 8;
            }
        }
        
        public Size Size
        {
            get => (Size)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public double BrushStrokeThickness
        {
            get => (double)GetValue(BrushStrokeThicknessProperty);
            set => SetValue(BrushStrokeThicknessProperty, value);
        }

        private void CircularProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var bar = sender as CircularProgressBar;
            var currentAngle = bar.Angle;
            var targetAngle = e.NewValue / bar.Maximum * 359.999;
            var anim = new DoubleAnimation(currentAngle, targetAngle, TimeSpan.FromMilliseconds(500));
            bar.BeginAnimation(AngleProperty, anim, HandoffBehavior.SnapshotAndReplace);
        }
    }
}