using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = CanvasTemplateName, Type = typeof(Canvas))]
    public class GestureUnlock : Control
    {
        private const string CanvasTemplateName = "PART_GestureUnlockCanvas";

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(GestureUnlock),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(GestureState), typeof(GestureUnlock),
                new PropertyMetadata(GestureState.None, OnIsErrorChanged));

        public static readonly DependencyProperty GestureCompletedCommandProperty =
            DependencyProperty.Register("GestureCompletedCommand", typeof(ICommand), typeof(GestureUnlock),
                new PropertyMetadata(null));


        public static readonly RoutedEvent GestureCompletedEvent =
            EventManager.RegisterRoutedEvent("GestureCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(GestureUnlock));

        private Canvas _canvas;

        private readonly List<GestureItem> _gestureItems = new List<GestureItem>();
        private readonly List<int> _gestures = new List<int>();
        private Polyline _line;
        private Line _moveLine;
        private double _thickness = 1;
        private DispatcherTimer _timer;
        private bool _tracking;
        private bool _isEventHandled = false;

        static GestureUnlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GestureUnlock),
                new FrameworkPropertyMetadata(typeof(GestureUnlock)));
        }

        public GestureUnlock()
        {
            SizeChanged -= GestureUnlock_SizeChanged;
            SizeChanged += GestureUnlock_SizeChanged;
        }

        public string Password
        {
            get => (string) GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public GestureState State
        {
            get => (GestureState) GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public ICommand GestureCompletedCommand
        {
            get => (ICommand) GetValue(GestureCompletedCommandProperty);
            set => SetValue(GestureCompletedCommandProperty, value);
        }

        private static void OnIsErrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as GestureUnlock;
            if (ctrl != null)
            {
                ctrl.UpdateLineStroke();
                switch (ctrl.State)
                {
                    case GestureState.Success:
                    case GestureState.Error:
                        if (ctrl._timer == null)
                        {
                            ctrl._timer = new DispatcherTimer
                            {
                                Interval = TimeSpan.FromSeconds(1.5)
                            };
                            ctrl._timer.Tick += (sender, args) =>
                            {
                                ctrl._timer.Stop();
                                ctrl.CleraGestureAndLine();
                            };
                        }

                        ctrl._timer.Start();
                        break;
                }
            }
        }


        public event RoutedEventHandler GestureCompleted
        {
            add => AddHandler(GestureCompletedEvent, value);
            remove => RemoveHandler(GestureCompletedEvent, value);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = GetTemplateChild(CanvasTemplateName) as Canvas;
            if (_canvas != null)
            {
                _canvas.MouseDown -= OnCanvas_MouseDown;
                _canvas.MouseDown += OnCanvas_MouseDown;
                _canvas.MouseMove -= OnCanvas_MouseMove;
                _canvas.MouseMove += OnCanvas_MouseMove;
                _canvas.MouseUp -= OnCanvas_MouseUp;
                _canvas.MouseUp += OnCanvas_MouseUp;
                Loaded += (s, e) => CreateGestureItems();
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (_tracking && !_isEventHandled)
            {
                _isEventHandled = true; 
                CompleteGesture();
            }
            _tracking = false;
            _isEventHandled = false;
        }

        private void CleraGestureAndLine()
        {
            State = GestureState.None;
            _gestureItems.ForEach(x => { x.IsSelected = false; });
            _gestures.Clear();
            _line.Points.Clear();
            _isEventHandled = false;
        }

        private void ResetMoveLine()
        {
            if (_moveLine != null)
            {
                _canvas.Children.Remove(_moveLine);
                _moveLine = null;
            }
            _moveLine = new Line
            {
                Stroke = _gestureItems.Count > 0 ? _gestureItems[0].BorderBrush : ThemeManager.Instance.PrimaryBrush,
                StrokeThickness = _thickness > 1 ? _thickness - 1 : _thickness
            };
        }

        private void GestureUnlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CreateGestureItems();
        }

        private void CreateGestureItems()
        {
            _gestureItems.Clear();
            _canvas.Children.Clear();
            double radius = 20d;
            int cols = 3; 
            int rows = 3; 
            double gap = (300 - (3 * radius * 2)) / 4; 
            gap = Math.Max(gap, radius);
            double totalWidth = (3 * radius * 2) + (2 * gap);
            double totalHeight = (3 * radius * 2) + (2 * gap);
            double startX = (ActualWidth - totalWidth) / 2 + radius;
            double startY = (ActualHeight - totalHeight) / 2 + radius;
            for (int i = 0; i < 9; i++)
            {
                int col = i % cols;
                int row = i / rows;
                double x = startX + col * (gap + radius * 2);
                double y = startY + row * (gap + radius * 2);
                var gestureItem = new GestureItem();
                gestureItem.Number = i;
                gestureItem.Loaded += (s, e) =>
                {
                    double actualRadius = Math.Max(gestureItem.ActualWidth, gestureItem.ActualHeight) / 2;
                    Canvas.SetLeft(gestureItem, x - actualRadius);
                    Canvas.SetTop(gestureItem, y - actualRadius);
                    ElementHelper.SetCornerRadius(gestureItem, new CornerRadius(actualRadius));
                };
                _canvas.Children.Add(gestureItem);
                _gestureItems.Add(gestureItem);
            }
            _line = new Polyline
            {
                Stroke = _gestureItems.Count > 0 ? _gestureItems[0].BorderBrush : ThemeManager.Instance.PrimaryBrush,
                StrokeThickness = _thickness
            };
            _canvas.Children.Add(_line);
            Panel.SetZIndex(_line, -98);
            ResetMoveLine();
        }

        private void OnCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isEventHandled)
            {
                _tracking = false;
                _isEventHandled = true; 
                CompleteGesture();
            }
        }

        void CompleteGesture()
        {
            if (_gestures.Count == 0) return;
            Password = string.Join("", _gestures);
            GestureCompletedCommand?.Execute(Password);
            RaiseEvent(new RoutedEventArgs(GestureCompletedEvent, Password));
            ResetMoveLine();
        }

        private void OnCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_tracking && e.LeftButton == MouseButtonState.Pressed)
            {
                var point = e.GetPosition(_canvas);
                if (_moveLine.X2 == 0 && _moveLine.Y1 == 0)
                {
                    _moveLine.X1 = point.X;
                    _moveLine.Y1 = point.Y;
                }
                if (_moveLine.X2 != point.X)
                    _moveLine.X2 = point.X;
                if (_moveLine.Y2 != point.Y)
                    _moveLine.Y2 = point.Y;
                TryAddPoint(point);
            }
        }

        private void OnCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CleraGestureAndLine();
            if (_timer != null)
                _timer.Stop();
            ResetMoveLine();
            _tracking = true;
            _isEventHandled = false;
            var point = e.GetPosition(_canvas);
            TryAddPoint(point);
        }

        private void TryAddPoint(Point position)
        {
            for (var i = 0; i < _gestureItems.Count; i++)
            {
                var gestureItem = _gestureItems[i];
                var left = Canvas.GetLeft(gestureItem);
                var top = Canvas.GetTop(gestureItem);
                var centerX = left + gestureItem.Width / 2;
                var centerY = top + gestureItem.Height / 2;
                var dist = (position - new Point(centerX, centerY)).Length;
                if (dist <= gestureItem.Width / 2 && !_gestures.Contains(i))
                {
                    if(_gestures.Count == 1)
                    {
                        var brush = _gestureItems.Count > 0 ? _gestureItems[0].BorderBrush : ThemeManager.Instance.PrimaryBrush;
                        _line.Stroke = brush;
                        _moveLine.Stroke = brush;
                    }
                    _gestures.Add(i);
                    _line.Points.Add(new Point(centerX, centerY));
                    gestureItem.IsSelected = true;
                    if (_moveLine.X1 == 0 
                        && 
                        _moveLine.Y1 == 0
                        &&
                        _moveLine.X2 == 0 
                        && 
                        _moveLine.Y2 == 0)
                    {
                        _moveLine.X1 = centerX;
                        _moveLine.Y1 = centerY;
                        _canvas.Children.Add(_moveLine);
                    }
                    else
                    {
                        var endPoint = _line.Points.LastOrDefault();
                        _moveLine.X1 = endPoint.X;
                        _moveLine.Y1 = endPoint.Y;
                    }

                    Panel.SetZIndex(_moveLine, -99);
                    _moveLine.X2 = centerX;
                    _moveLine.Y2 = centerY;
                    break;
                }
            }
        }

        private void UpdateLineStroke()
        {
            if (_line == null)
                return;

            switch (State)
            {
                case GestureState.Success:
                    _line.Stroke = ThemeManager.Instance.Resources.TryFindResource<Brush>("WD.SuccessBrush");
                    break;
                case GestureState.Error:
                    _line.Stroke = ThemeManager.Instance.Resources.TryFindResource<Brush>("WD.DangerBrush");
                    break;
                default:
                    _line.Stroke = _gestureItems.Count > 0 ? _gestureItems[0].BorderBrush : ThemeManager.Instance.PrimaryBrush;
                    break;
            }
        }
    }
}