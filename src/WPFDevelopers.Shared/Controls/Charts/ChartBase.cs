using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using WPFDevelopers.Core;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class ChartBase : Control
    {
        public static readonly DependencyProperty DatasProperty =
            DependencyProperty.Register("Datas", typeof(IEnumerable<KeyValuePair<string, double>>),
                typeof(ChartBase), new UIPropertyMetadata(DatasChanged));

        private Border _border;
        private KeyValuePair<Rect, string> _lastItem;
        private Popup _popup;
        private TextBlock _textBlock;
        private bool isPopupOpen;
        public IDictionary<Rect, string> PointCache;

        static ChartBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartBase),
                new FrameworkPropertyMetadata(typeof(ChartBase)));
        }

        protected double EllipseSize { get; } = 7;
        protected double EllipsePadding { get; } = 20;
        protected double Rows { get; } = 5;

        protected double Interval { get; } = 120;

        protected double MaxY { get; }
        protected double StartY { get; set; }
        protected double StartX { get; set; } = 40;

        protected Brush NormalBrush => ThemeManager.Instance.PrimaryBrush;


        public IEnumerable<KeyValuePair<string, double>> Datas
        {
            get => (IEnumerable<KeyValuePair<string, double>>) GetValue(DatasProperty);
            set => SetValue(DatasProperty, value);
        }

        private static void DatasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ChartBase;
            if (e.NewValue != null)
                ctrl.InvalidateVisual();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (Datas == null || Datas.Count() == 0 || isPopupOpen)
                return;
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
                        if (!_lastItem.Key.Contains(point))
                        {
                            _popup.IsOpen = false;
                            isPopupOpen = false;
                            _lastItem = new KeyValuePair<Rect, string>();
                        }
                };
                _popup.Closed += delegate { isPopupOpen = false; };
                _textBlock = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = (Brush) Application.Current?.TryFindResource("WD.WindowTextBrush")
                };
                _border = new Border
                {
                    Child = _textBlock,
                    Background = (Brush) Application.Current?.TryFindResource("WD.ChartFillBrush"),
                    Effect = Application.Current?.TryFindResource("WD.PopupShadowDepth") as DropShadowEffect,
                    Margin = new Thickness(10),
                    CornerRadius = new CornerRadius(3),
                    Padding = new Thickness(6)
                };
                _popup.Child = _border;
            }

            if (PointCache == null) return;
            var currentPoint = e.GetPosition(this);
            if (PointCache.Any(x => x.Key.Contains(currentPoint)))
            {
                isPopupOpen = true;

                var currentItem = PointCache.FirstOrDefault(x => x.Key.Contains(currentPoint));
                if (currentItem.Key == null) return;
                _textBlock.Text = currentItem.Value;
                _popup.IsOpen = true;
                _lastItem = currentItem;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (Datas == null || Datas.Count() == 0)
                return;
            SnapsToDevicePixels = true;
            UseLayoutRounding = true;
        }

        public void DrawEllipse(IEnumerable<Rect> rects, DrawingContext drawingContext)
        {
            var drawingPen = new Pen
            {
                Thickness = 2,
                Brush = NormalBrush
            };
            drawingPen.Freeze();

            var backgroupBrush = new SolidColorBrush
            {
                Color = (Color) Application.Current?.TryFindResource("WD.BackgroundColor")
            };
            backgroupBrush.Freeze();
            foreach (var item in rects)
            {
                var ellipseGeom = new EllipseGeometry(item);
                drawingContext.DrawGeometry(backgroupBrush, drawingPen, ellipseGeom);
            }
        }
    }
}