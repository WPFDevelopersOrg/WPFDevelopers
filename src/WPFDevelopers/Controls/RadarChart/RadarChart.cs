using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFDevelopers.Controls
{
    public class RadarChart:Control
    {
        public ObservableCollection<RadarModel> RadarArray
        {
            get { return (ObservableCollection<RadarModel>)GetValue(RadarArrayProperty); }
            set { SetValue(RadarArrayProperty, value); }
        }

        public static readonly DependencyProperty RadarArrayProperty =
            DependencyProperty.Register("RadarArray", typeof(ObservableCollection<RadarModel>), typeof(RadarChart), new UIPropertyMetadata(OnRadarArrayChanged));

        private static void OnRadarArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadarChart radarChart = d as RadarChart;
            if (e.NewValue != null)
                radarChart.InvalidateVisual();
        }

        static RadarChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadarChart), new FrameworkPropertyMetadata(typeof(RadarChart)));
        }
       
        protected override void OnRender(DrawingContext drawingContext)
        {
            //防止延迟加载时，RadarArray为null时报错，防止RadarArray.Count==0时，后续做 被除数（double perangle = 360 / polygonBound） 时溢出错误
            if (RadarArray == null || RadarArray.Count == 0)
                return;

            //base.OnRender(drawingContext);
            DrawPoints(150, drawingContext, true);
            DrawPoints(100, drawingContext);
            DrawPoints(50, drawingContext);

            var myPen = new Pen
            {
                Thickness = 4,
                Brush = Brushes.DodgerBlue
            };
            myPen.Freeze();
            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext geometryContext = streamGeometry.Open())
            {
                var h = this.ActualHeight / 2;
                var w = this.ActualWidth / 2;
                PointCollection points = new PointCollection();
                foreach (var item in RadarArray)
                {
                    var ss = new Point((item.PointValue.X - w) / 100 * item.ValueMax + w, (item.PointValue.Y - h) / 100 * item.ValueMax + h);
                    points.Add(ss);
                }
                geometryContext.BeginFigure(points[points.Count - 1], true, true);
                geometryContext.PolyLineTo(points, true, true);
            }
            streamGeometry.Freeze();
            SolidColorBrush rectBrush = new SolidColorBrush(Colors.LightSkyBlue);
            rectBrush.Opacity = 0.5;
            drawingContext.DrawGeometry(rectBrush, myPen, streamGeometry);
        }
        void DrawPoints(int circleRadius, DrawingContext drawingContext,bool isDrawText = false)
        {
            var myPen = new Pen
            {
                Thickness = 2,
                Brush = Brushes.Gainsboro
            };
            myPen.Freeze();
            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext geometryContext = streamGeometry.Open())
            {
                var h = this.ActualHeight / 2;
                var w = this.ActualWidth / 2;
                PointCollection points = null;
                if (isDrawText)
                    points = GetPolygonPoint(new Point(w, h), circleRadius, RadarArray.Count, drawingContext);
                else
                    points = GetPolygonPoint(new Point(w, h), circleRadius, RadarArray.Count);
                geometryContext.BeginFigure(points[points.Count - 1], true, true);
                geometryContext.PolyLineTo(points, true, true);
            }
            streamGeometry.Freeze();
            drawingContext.DrawGeometry(null, myPen, streamGeometry);
        }
        private PointCollection GetPolygonPoint(Point center, double r, int polygonBound, DrawingContext drawingContext = null)
        {
            double g = 18;
            double perangle = 360 / polygonBound;
            double pi = Math.PI;
            List<Point> values = new List<Point>();
            for (int i = 0; i < polygonBound; i++)
            {
                Point p2 = new Point(r * Math.Cos(g * pi / 180) + center.X, r * Math.Sin(g * pi / 180) + center.Y);
                if(drawingContext != null)
                {
                    var formattedText = DrawingContextHelper.GetFormattedText(RadarArray[i].Text,flowDirection:FlowDirection.LeftToRight,textSize:20.001D);
                     RadarArray[i].PointValue = p2;
                    if (p2.Y > center.Y && p2.X < center.X)
                        drawingContext.DrawText(formattedText, new Point(p2.X - formattedText.Width - 5, p2.Y - formattedText.Height / 2));
                    else if (p2.Y < center.Y && p2.X > center.X)
                        drawingContext.DrawText(formattedText, new Point(p2.X, p2.Y - formattedText.Height));
                    else if (p2.Y < center.Y && p2.X < center.X)
                        drawingContext.DrawText(formattedText, new Point(p2.X - formattedText.Width - 5, p2.Y - formattedText.Height));
                    else if (p2.Y < center.Y && p2.X == center.X)
                        drawingContext.DrawText(formattedText, new Point(p2.X - formattedText.Width, p2.Y - formattedText.Height));
                    else
                        drawingContext.DrawText(formattedText, new Point(p2.X, p2.Y));
                }
                values.Add(p2);
                g += perangle;
            }
            PointCollection pcollect = new PointCollection(values);
            return pcollect;
        }
    }
}
