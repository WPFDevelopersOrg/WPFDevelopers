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
    public class NineGridView : Grid
    {
        private int _rows = 3;

        private int _columns = 3;

        private Dictionary<Rect, int> _dicRect = new Dictionary<Rect, int>();

        private Border _border;

        private Storyboard _storyboard;

        private Rect _lastRect;

        private int _last;

        public static readonly DependencyProperty SelectBrushProperty =
          DependencyProperty.Register("SelectBrush", typeof(Brush), typeof(NineGridView),
              new PropertyMetadata(Brushes.Red));
        
        public static readonly DependencyProperty BorderThicknessProperty =
       DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(NineGridView),
           new PropertyMetadata(new Thickness(1)));

        public NineGridView()
        {
            Loaded += NineGridView_Loaded;
            SizeChanged += NineGridView_SizeChanged;
        }

        public Brush SelectBrush
        {
            get => (Brush)GetValue(SelectBrushProperty);
            set => SetValue(SelectBrushProperty, value);
        }
      
        public Thickness BorderThickness
        {
            get => (Thickness)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        private void NineGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_border == null || _last == 0) return;
            var frameworkElement = InternalChildren[_last] as FrameworkElement;
            if (frameworkElement == null) return;
            _border.Width = frameworkElement.ActualWidth;
            _border.Height = frameworkElement.ActualHeight;
            var point = frameworkElement.TranslatePoint(new Point(0, 0), this);
            CreateStoryboard(point);
        }
        private void NineGridView_Loaded(object sender, RoutedEventArgs e)
        {
            RowDefinitions.Clear();
            for (int i = 0; i < _rows; i++)
            {
                var row1 = new RowDefinition();
                RowDefinitions.Add(row1);
            }
            ColumnDefinitions.Clear();
            for (int i = 0; i < _columns; i++)
            {
                var col1 = new ColumnDefinition();
                ColumnDefinitions.Add(col1);
            }
            UIElementCollection children = InternalChildren;

            int numCol = 0, numRow = 0;
            for (int i = 0, count = children.Count; i < count; ++i)
            {
                if (i > 6) return;
                UIElement child = children[i];
                if (child != null)
                {
                    if (i == 0)
                    {
                        SetRowSpan(child, 2);
                        SetColumnSpan(child, 2);
                    }
                    else
                    {
                        var num = i - 1;
                        var col = GetColumnSpan(children[num]);
                        col = col == 1 ? GetColumn(children[num]) : col;
                        if (i + 1 <= _columns)
                        {
                            SetColumn(child, col);
                            SetRow(child, numRow);
                            numRow++;
                        }
                        else
                        {
                            var row = GetRowSpan(children[0]);
                            SetColumn(child, numCol);
                            SetRow(child, row);
                            numCol++;
                        }
                    }
                }
            }
            if(_border != null)
                Children.Remove(_border);
            _border = new Border
            {
                BorderThickness = BorderThickness,
                BorderBrush = SelectBrush,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            _border.Name = "PART_Adorner";
            _border.RenderTransform = new TranslateTransform();
            SetRowSpan(_border, _rows);
            SetColumnSpan(_border, _columns);
            _border.Width = ActualWidth / _columns - 2;
            _border.Height = ActualHeight / _rows - 2;
            var _translateTransform = (TranslateTransform)_border.RenderTransform;
            _translateTransform.X = _border.Width * 2 + 4;
            Children.Add(_border);
            _last = 1;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            var currentPoint = e.GetPosition(this);
            if (_lastRect.Contains(currentPoint))
                return;
            _dicRect.Clear();
            UIElementCollection children = InternalChildren;
            for (int i = 0, count = children.Count; i < count; ++i)
            {
                if (i >= 6 || i == 0) continue;
                var child = children[i] as FrameworkElement;
                if (child != null)
                {
                    var point = child.TranslatePoint(new Point(0, 0), this);
                    var rect = new Rect(point.X, point.Y, child.ActualWidth, child.ActualHeight);
                    _dicRect.Add(rect, i);
                }
            }


            var model = _dicRect.Keys.FirstOrDefault(x => x.Contains(currentPoint));
            if (model == default) return;
            _dicRect.TryGetValue(model, out _last);
            if (_border == null) return;

            CreateStoryboard(new Point(model.X, model.Y));
            _border.Width = model.Width;
            _border.Height = model.Height;
            _lastRect = model;
        }
        void CreateStoryboard(Point point)
        {
            var sineEase = new SineEase() { EasingMode = EasingMode.EaseOut };
            if (_storyboard == null)
                _storyboard = new Storyboard();
            else
                _storyboard.Children.Clear();
            var animationX = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(500),
                To = point.X,
                EasingFunction = sineEase
            };
            Storyboard.SetTargetProperty(animationX, new PropertyPath("(Border.RenderTransform).(TranslateTransform.X)"));
            _storyboard.Children.Add(animationX);
            var animationY = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(500),
                To = point.Y,
                EasingFunction = sineEase
            };
            Storyboard.SetTargetProperty(animationY, new PropertyPath("(Border.RenderTransform).(TranslateTransform.Y)"));
            _storyboard.Children.Add(animationY);
            _storyboard.Begin(_border);

        }
    }
}
