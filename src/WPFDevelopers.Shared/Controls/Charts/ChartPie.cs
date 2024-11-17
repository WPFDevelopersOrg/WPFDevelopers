using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using WPFDevelopers.Core;

namespace WPFDevelopers.Controls
{
    public class ChartPie : Control
    {
        public static readonly DependencyProperty DatasProperty =
            DependencyProperty.Register("Datas", typeof(IEnumerable<KeyValuePair<string, double>>),
                typeof(ChartPie), new UIPropertyMetadata(DatasChanged));

        private Border _border;
        private Ellipse _ellipse;
        private KeyValuePair<PathGeometry, string> _lastItem;
        private Popup _popup;
        private StackPanel _stackPanel;
        private TextBlock _textBlock;
        private double centerX, centerY, radius;
        private bool isPopupOpen;
        private readonly Dictionary<PathGeometry, string> pathGeometries = new Dictionary<PathGeometry, string>();

        private readonly Color[] vibrantColors;

        public ChartPie()
        {
            vibrantColors = new[]
            {
                Color.FromArgb(255, 84, 112, 198),
                Color.FromArgb(255, 145, 204, 117),
                Color.FromArgb(255, 250, 200, 88),
                Color.FromArgb(255, 238, 102, 102),
                Color.FromArgb(255, 115, 192, 222),
                Color.FromArgb(255, 59, 162, 114),
                Color.FromArgb(255, 252, 132, 82),
                Color.FromArgb(255, 154, 96, 180),
                Color.FromArgb(255, 234, 124, 204)
            };
        }

        public IEnumerable<KeyValuePair<string, double>> Datas
        {
            get => (IEnumerable<KeyValuePair<string, double>>) GetValue(DatasProperty);
            set => SetValue(DatasProperty, value);
        }

        private static void DatasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ChartPie;
            if (e.NewValue != null)
                ctrl.InvalidateVisual();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (Datas == null || Datas.Count() == 0 || isPopupOpen) return;
            if (_popup == null)
            {
                _popup = new Popup
                {
                    AllowsTransparency = true,
                    Placement = PlacementMode.MousePoint,
                    PlacementTarget = this,
                    StaysOpen = false
                };
                _popup.MouseMove += (y, j) =>
                {
                    var point = j.GetPosition(this);
                    if (isPopupOpen && _lastItem.Value != null)
                        if (!IsMouseOverGeometry(_lastItem.Key))
                        {
                            _popup.IsOpen = false;
                            isPopupOpen = false;
                            _lastItem = new KeyValuePair<PathGeometry, string>();
                        }
                };
                _popup.Closed += delegate { isPopupOpen = false; };

                _textBlock = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = (Brush) Application.Current.TryFindResource("WD.WindowForegroundColorBrush"),
                    Padding = new Thickness(4, 0, 2, 0)
                };
                _ellipse = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Stroke = Brushes.White
                };
                _stackPanel = new StackPanel {Orientation = Orientation.Horizontal};
                _stackPanel.Children.Add(_ellipse);
                _stackPanel.Children.Add(_textBlock);

                _border = new Border
                {
                    Child = _stackPanel,
                    Background = (Brush) Application.Current.TryFindResource("WD.ChartFillSolidColorBrush"),
                    Effect = Application.Current.TryFindResource("WD.PopupShadowDepth") as DropShadowEffect,
                    Margin = new Thickness(10),
                    CornerRadius = new CornerRadius(3),
                    Padding = new Thickness(6)
                };
                _popup.Child = _border;
            }

            var index = 0;
            foreach (var pathGeometry in pathGeometries)
            {
                if (IsMouseOverGeometry(pathGeometry.Key))
                {
                    isPopupOpen = true;
                    _ellipse.Fill = new SolidColorBrush
                    {
                        Color = vibrantColors[index >= vibrantColors.Length ? index % vibrantColors.Length : index]
                    };
                    _textBlock.Text = pathGeometry.Value;
                    //var bounds = pathGeometry.Key.Bounds;
                    //var center = new Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
                    //_popup.HorizontalOffset = center.X - (_border.ActualWidth / 2);
                    //_popup.VerticalOffset = center.Y - (_border.ActualHeight / 2);
                    _popup.IsOpen = true;
                    _lastItem = pathGeometry;
                    break;
                }

                index++;
            }
        }

        private bool IsMouseOverGeometry(PathGeometry pathGeometry)
        {
            var mousePosition = Mouse.GetPosition(this);
            return pathGeometry.FillContains(mousePosition);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (Datas == null || Datas.Count() == 0)
                return;
            SnapsToDevicePixels = true;
            UseLayoutRounding = true;
            pathGeometries.Clear();
            var drawingPen = CreatePen(2);
            var boldDrawingPen = CreatePen(4);
            var pieWidth = ActualWidth > ActualHeight ? ActualHeight : ActualWidth;
            var pieHeight = ActualWidth > ActualHeight ? ActualHeight : ActualWidth;
            centerX = pieWidth / 2;
            centerY = pieHeight / 2;
            radius = ActualWidth > ActualHeight ? ActualHeight / 2 : ActualWidth / 2;
            var angle = 0d;
            var prevAngle = 0d;
            var sum = Datas.Select(ser => ser.Value).Sum();
            var index = 0;
            var isFirst = false;
            foreach (var item in Datas)
            {
                var arcStartX = radius * Math.Cos(angle * Math.PI / 180) + centerX;
                var arcStartY = radius * Math.Sin(angle * Math.PI / 180) + centerY;
                angle = item.Value / sum * 360 + prevAngle;
                var arcEndX = 0d;
                var arcEndY = 0d;
                if (Datas.Count() == 1 && angle == 360)
                {
                    isFirst = true;
                    arcEndX = centerX + Math.Cos(359.99999 * Math.PI / 180) * radius;
                    arcEndY = radius * Math.Sin(359.99999 * Math.PI / 180) + centerY;
                }
                else
                {
                    arcEndX = centerX + Math.Cos(angle * Math.PI / 180) * radius;
                    arcEndY = radius * Math.Sin(angle * Math.PI / 180) + centerY;
                }
                var startPoint = new Point(arcStartX, arcStartY);
                var line1Segment = new LineSegment(startPoint, false);
                var isLargeArc = item.Value / sum > 0.5;
                var arcSegment = new ArcSegment();
                var size = new Size(radius, radius);
                var endPoint = new Point(arcEndX, arcEndY);
                arcSegment.Size = size;
                arcSegment.Point = endPoint;
                arcSegment.SweepDirection = SweepDirection.Clockwise;
                arcSegment.IsLargeArc = isLargeArc;
                var center = new Point(centerX, centerY);
                var line2Segment = new LineSegment(center, false);
                var pathGeometry = new PathGeometry(new[]
                {
                    new PathFigure(new Point(centerX, centerY), new List<PathSegment>
                    {
                        line1Segment,
                        arcSegment,
                        line2Segment
                    }, true)
                });
                pathGeometries.Add(pathGeometry,
                    $"{item.Key} : {item.Value.FormatNumber()}");
                var backgroupBrush = new SolidColorBrush
                {
                    Color = vibrantColors[
                        index >= vibrantColors.Length
                            ? index % vibrantColors.Length
                            : index]
                };
                backgroupBrush.Freeze();
                drawingContext.DrawGeometry(backgroupBrush, null, pathGeometry);
                index++;
                if (!isFirst)
                {
                    if (index == 1)
                        drawingContext.DrawLine(boldDrawingPen, center, startPoint);
                    else
                        drawingContext.DrawLine(drawingPen, center, startPoint);
                }
                prevAngle = angle;
            }
        }

        private Pen CreatePen(double thickness)
        {
            var pen = new Pen
            {
                Thickness = thickness,
                Brush = Brushes.White
            };
            pen.Freeze();
            return pen;
        }
    }
}