using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// BarrageExample.xaml 的交互逻辑
    /// </summary>
    public partial class BarrageExample : UserControl
    {
        private Dictionary<TimeSpan, List<Border>> _dicBorder;
        private long _num, _index;
        private double _right, _top;
        private Random _random = new Random();
        public BarrageExample()
        {
            InitializeComponent();
            _dicBorder = new Dictionary<TimeSpan, List<Border>>();
            Loaded += delegate
            {
                _num = (int)(ActualHeight - MyGrid.ActualHeight) / 40;
                var list = new List<string>();
                list.Add("2333");
                list.Add("测试弹幕");
                list.Add("很难开心");
                list.Add("map");
                list.Add("map加载");
                list.Add("bing");
                list.Add("地图");
                foreach (var item in list)
                {
                    SolidColorBrush brush = new SolidColorBrush(Color.FromRgb((byte)_random.Next(1, 255),
                        (byte)_random.Next(1, 255), (byte)_random.Next(1, 233)));

                    AddBarrage(brush.Color, item);

                }

            };
        }

        void AddBarrage(Color color, string text)
        {
            _index++;
            TimeSpan time = default;

            var linearGradientBrush = new LinearGradientBrush()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
                MappingMode = BrushMappingMode.RelativeToBoundingBox,
                GradientStops = new GradientStopCollection
                {
                    new GradientStop { Color = Colors.Transparent, Offset = 2},
                    new GradientStop { Color = color },
                },

            };
            var border = new Border()
            {
                Background = linearGradientBrush,
                Height = 40,
                CornerRadius = new CornerRadius(20),
                Padding = new Thickness(40, 0, 40, 0)

            };

            var textBlock = new TextBlock()
            {
                Text = text,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
            };
            border.Child = textBlock;
            MyCanvas.Children.Add(border);
            border.Loaded += delegate
            {

                time = TimeSpan.FromMilliseconds(border.ActualWidth * 60);
                _right = _right == 0 ? ActualWidth + border.ActualWidth : _right;
                var y = ActualHeight - MyGrid.ActualHeight - border.ActualHeight;
                _top = _top + 40 >= y ? border.ActualHeight : _top;
                Canvas.SetLeft(border, _right);
                Canvas.SetTop(border, _top);
                var doubleAnimation = new DoubleAnimation
                {
                    From = _right,
                    To = -(ActualWidth + border.ActualWidth),
                    Duration = time
                };
                doubleAnimation.Completed += (s, e) =>
                {
                    var animationClock = s as AnimationClock;
                    if (animationClock == null) return;
                    var duration = animationClock.Timeline.Duration;
                    var bordersList = new List<Border>();
                    _dicBorder.TryGetValue(duration.TimeSpan, out bordersList);
                    if (bordersList != null && bordersList.Count > 0)
                    {
                        foreach (var item in bordersList)
                        {
                            MyCanvas.Children.Remove(item);
                        }
                        _dicBorder.Remove(duration.TimeSpan);
                    }
                };
                border.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
                _top += border.ActualHeight + 20;
                if (!_dicBorder.ContainsKey(time))
                    _dicBorder.Add(time, new List<Border> { border });
                else
                {
                    var bordersList = new List<Border>();
                    _dicBorder.TryGetValue(time, out bordersList);
                    bordersList.Add(border);
                }
            };

            if (_index > _num)
            {
                _index = 0;
            }

        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb((byte)_random.Next(1, 255),
                (byte)_random.Next(1, 255), (byte)_random.Next(1, 233)));

            AddBarrage(brush.Color, tbBarrage.Text);
        }
    }


}
