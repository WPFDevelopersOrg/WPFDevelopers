using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    public class SixGirdView : Panel
    {
        public static readonly DependencyProperty SelectBrushProperty =
            DependencyProperty.Register("SelectBrush", typeof(Brush), typeof(SixGirdView),
                new PropertyMetadata(Brushes.Red));

        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(SixGirdView),
                new PropertyMetadata(new Thickness(1)));

        private readonly Dictionary<Rect, int> _dicRect = new Dictionary<Rect, int>();

        private readonly int _columns = 3;

        private readonly int _rows = 3;

        private Border _border;

        private int _last;

        private Rect _lastRect;

        private Storyboard _storyboard;

        public Brush SelectBrush
        {
            get => (Brush) GetValue(SelectBrushProperty);
            set => SetValue(SelectBrushProperty, value);
        }

        public Thickness BorderThickness
        {
            get => (Thickness) GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        public override void EndInit()
        {
            base.EndInit();
            var children = InternalChildren;

            if (_border == null && children.Count >= 1)
            {
                _border = new Border
                {
                    BorderThickness = BorderThickness,
                    BorderBrush = SelectBrush
                };
                _border.RenderTransform = new TranslateTransform();
                children.Add(_border);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var children = InternalChildren;
            int numCol = 0, numRow = 0;
            var isRow = false;
            Point point = default;
            Size size = default;
            _dicRect.Clear();
            double _width = availableSize.Width / _columns, _height = availableSize.Height / _rows;
            for (int i = 0, count = children.Count; i < count; ++i)
            {
                if (i >= 6) continue;
                var uIElement = children[i];
                if (uIElement != null)
                {
                    uIElement.Measure(availableSize);
                    if (i == 0)
                    {
                        point = new Point(0, 0);
                        size = new Size(availableSize.Width / _columns * 2, availableSize.Height / _rows * 2);
                        numRow++;
                    }
                    else
                    {
                        var num = i - 1;
                        var x = 0d;
                        if (!isRow)
                        {
                            x = _width * 2;
                            numCol = numRow + 1;
                            if (numCol < _columns)
                            {
                                point = new Point(x, 0);
                            }
                            else
                            {
                                point = new Point(x, _height);
                                isRow = true;
                                numCol = 0;
                            }

                            numRow++;
                        }
                        else
                        {
                            x = _width * numCol;
                            numCol++;
                            x = x >= availableSize.Width ? 0 : x;

                            point = new Point(x, _height * 2);
                        }

                        size = new Size(_width, _height);
                    }

                    uIElement.Arrange(new Rect(point, size));
                    if (i >= 6 || i == 0) continue;
                    var rect = new Rect(point.X, point.Y, size.Width, size.Height);
                    _dicRect.Add(rect, i);
                }
            }


            _last = _last == 0 ? 1 : _last;
            if (_border != null)
            {
                _border.Measure(availableSize);
                point = new Point(0, 0);
                size = new Size(availableSize.Width / _columns, availableSize.Height / _columns);
                _border.Arrange(new Rect(point, size));
                var _translateTransform = (TranslateTransform) _border.RenderTransform;
                if (_last == 1)
                {
                    _translateTransform.X = availableSize.Width / _columns * 2;
                }
                else
                {
                    var uIElement = InternalChildren[_last];
                    if (uIElement != null)
                    {
                        var rect = _dicRect.FirstOrDefault(x => x.Value == _last).Key;
                        if (rect != null)
                        {
                            point = new Point(rect.X, rect.Y);
                            CreateStoryboard(point);
                        }
                    }
                }
            }


            return availableSize;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            var currentPoint = e.GetPosition(this);
            if (_lastRect.Contains(currentPoint))
                return;

            var model = _dicRect.Keys.FirstOrDefault(x => x.Contains(currentPoint));
            if (model == default) return;
            _dicRect.TryGetValue(model, out _last);
            if (_border == null) return;

            CreateStoryboard(new Point(model.X, model.Y));
            _lastRect = model;
        }

        private void CreateStoryboard(Point point)
        {
            var sineEase = new SineEase {EasingMode = EasingMode.EaseOut};
            if (_storyboard == null)
            {
                _storyboard = new Storyboard();
            }
            else
            {
                _storyboard.Stop();
                _storyboard.Children.Clear();
            }

            var animationX = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(500),
                To = point.X,
                EasingFunction = sineEase
            };
            Storyboard.SetTargetProperty(animationX,
                new PropertyPath("(Border.RenderTransform).(TranslateTransform.X)"));
            _storyboard.Children.Add(animationX);
            var animationY = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(500),
                To = point.Y,
                EasingFunction = sineEase
            };
            Storyboard.SetTargetProperty(animationY,
                new PropertyPath("(Border.RenderTransform).(TranslateTransform.Y)"));
            _storyboard.Children.Add(animationY);
            _storyboard.Begin(_border);
        }
    }
}