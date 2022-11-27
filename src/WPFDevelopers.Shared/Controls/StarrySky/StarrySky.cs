using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPFDevelopers.Controls
{

    [TemplatePart(Name = GridTemplateName, Type = typeof(SmallPanel))]
    [TemplatePart(Name = CanvasTemplateName, Type = typeof(Canvas))]
    public class StarrySky : Control
    {
        private const string GridTemplateName = "PART_GridLineContainer";

        private const string CanvasTemplateName = "PART_CanvasStarContainer";

        private SmallPanel _grid;
        private Canvas _canvas;

        public static readonly DependencyProperty StarCountProperty =
            DependencyProperty.Register("StarCount", typeof(int), typeof(StarrySky), new UIPropertyMetadata(10));

        public static readonly DependencyProperty StarSizeMinProperty =
            DependencyProperty.Register("StarSizeMin", typeof(int), typeof(StarrySky), new UIPropertyMetadata(5));

        public static readonly DependencyProperty StarSizeMaxProperty =
            DependencyProperty.Register("StarSizeMax", typeof(int), typeof(StarrySky), new UIPropertyMetadata(20));

        public static readonly DependencyProperty StarVMinProperty =
            DependencyProperty.Register("StarVMin", typeof(int), typeof(StarrySky), new UIPropertyMetadata(10));

        public static readonly DependencyProperty StarVMaxProperty =
            DependencyProperty.Register("StarVMax", typeof(int), typeof(StarrySky), new UIPropertyMetadata(20));

        public static readonly DependencyProperty StarRVMinProperty =
            DependencyProperty.Register("StarRVMin", typeof(int), typeof(StarrySky), new UIPropertyMetadata(90));

        public static readonly DependencyProperty StarRVMaxProperty =
            DependencyProperty.Register("StarRVMax", typeof(int), typeof(StarrySky), new UIPropertyMetadata(360));

        public static readonly DependencyProperty LineRateProperty =
            DependencyProperty.Register("LineRate", typeof(int), typeof(StarrySky), new UIPropertyMetadata(3));

        private readonly Random _random = new Random();
        private StarInfo[] _stars;


        static StarrySky()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StarrySky),
                new FrameworkPropertyMetadata(typeof(StarrySky)));
        }

        public StarrySky()
        {
            Loaded += delegate
            {
                CompositionTarget.Rendering += delegate
                {
                    StarRoamAnimation();
                    AddOrRemoveStarLine();
                    MoveStarLine();
                };
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _grid = GetTemplateChild(GridTemplateName) as SmallPanel;
            _canvas = GetTemplateChild(CanvasTemplateName) as Canvas;
        }

        public int StarCount
        {
            get => (int)GetValue(StarCountProperty);
            set => SetValue(StarCountProperty, value);
        }

        public int StarSizeMin
        {
            get => (int)GetValue(StarSizeMinProperty);
            set => SetValue(StarSizeMinProperty, value);
        }

        public int StarSizeMax
        {
            get => (int)GetValue(StarSizeMaxProperty);
            set => SetValue(StarSizeMaxProperty, value);
        }

        public int StarVMin
        {
            get => (int)GetValue(StarVMinProperty);
            set => SetValue(StarVMinProperty, value);
        }

        public int StarVMax
        {
            get => (int)GetValue(StarVMaxProperty);
            set => SetValue(StarVMaxProperty, value);
        }


        public int StarRVMin
        {
            get => (int)GetValue(StarRVMinProperty);
            set => SetValue(StarRVMinProperty, value);
        }

        public int StarRVMax
        {
            get => (int)GetValue(StarRVMaxProperty);
            set => SetValue(StarRVMaxProperty, value);
        }

        public int LineRate
        {
            get => (int)GetValue(LineRateProperty);
            set => SetValue(LineRateProperty, value);
        }


       
        public void InitStar()
        {
            //清空星星容器
            _stars = new StarInfo[StarCount];
            _canvas.Children.Clear();
            _grid.Children.Clear();
            //生成星星
            for (var i = 0; i < StarCount; i++)
            {
                double size = _random.Next(StarSizeMin, StarSizeMax + 1); //星星尺寸
                var starInfo = new StarInfo
                {
                    X = _random.Next(0, (int)_canvas.ActualWidth),
                    XV = (double)_random.Next(-StarVMax, StarVMax) / 60,
                    XT = _random.Next(6, 301), //帧
                    Y = _random.Next(0, (int)_canvas.ActualHeight),
                    YV = (double)_random.Next(-StarVMax, StarVMax) / 60,
                    YT = _random.Next(6, 301), //帧
                    StarLines = new Dictionary<StarInfo, Line>()
                };
                var star = new Path
                {
                    Data = Application.Current.Resources["PathStarrySky"] as Geometry,
                    Width = size,
                    Height = size,
                    Stretch = Stretch.Fill,
                    Fill = GetRandomColorBursh(),
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = new RotateTransform { Angle = 0 }
                };
                Canvas.SetLeft(star, starInfo.X);
                Canvas.SetTop(star, starInfo.Y);
                starInfo.StarRef = star;
                //设置星星旋转动画
                SetStarRotateAnimation(star);
                //添加到容器
                _stars[i] = starInfo;
                _canvas.Children.Add(star);
            }
        }
       
        private void SetStarRotateAnimation(Path star)
        {
            double v = _random.Next(StarRVMin, StarRVMax + 1); //速度
            double a = _random.Next(0, 360 * 5); //角度
            var t = a / v; //时间
            var dur = new Duration(new TimeSpan(0, 0, 0, 0, (int)(t * 1000)));

            var sb = new Storyboard
            {
                Duration = dur
            };
            //动画完成事件 再次设置此动画
            sb.Completed += (S, E) => { SetStarRotateAnimation(star); };

            var da = new DoubleAnimation
            {
                To = a,
                Duration = dur
            };
            Storyboard.SetTarget(da, star);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
            sb.Children.Add(da);
            sb.Begin(this);
        }
        private SolidColorBrush GetRandomColorBursh()
        {
            var r = (byte)_random.Next(128, 256);
            var g = (byte)_random.Next(128, 256);
            var b = (byte)_random.Next(128, 256);
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }
        /// <summary>
        ///     星星漫游动画
        /// </summary>
        private void StarRoamAnimation()
        {
            if (_stars == null)
                return;

            foreach (var starInfo in _stars)
            {
                //X轴运动
                if (starInfo.XT > 0)
                {
                    //运动时间大于0,继续运动
                    if (starInfo.X >= _canvas.ActualWidth || starInfo.X <= 0)
                        //碰到边缘,速度取反向
                        starInfo.XV = -starInfo.XV;
                    //位移加,时间减
                    starInfo.X += starInfo.XV;
                    starInfo.XT--;
                    Canvas.SetLeft(starInfo.StarRef, starInfo.X);
                }
                else
                {
                    //运动时间小于0,重新设置速度和时间
                    starInfo.XV = (double)_random.Next(-StarVMax, StarVMax) / 60;
                    starInfo.XT = _random.Next(100, 1001);
                }

                //Y轴运动
                if (starInfo.YT > 0)
                {
                    //运动时间大于0,继续运动
                    if (starInfo.Y >= _canvas.ActualHeight || starInfo.Y <= 0)
                        //碰到边缘,速度取反向
                        starInfo.YV = -starInfo.YV;
                    //位移加,时间减
                    starInfo.Y += starInfo.YV;
                    starInfo.YT--;
                    Canvas.SetTop(starInfo.StarRef, starInfo.Y);
                }
                else
                {
                    //运动时间小于0,重新设置速度和时间
                    starInfo.YV = (double)_random.Next(-StarVMax, StarVMax) / 60;
                    starInfo.YT = _random.Next(100, 1001);
                }
            }
        }
        /// <summary>
        ///     添加或者移除星星之间的连线
        /// </summary>
        private void AddOrRemoveStarLine()
        {
            //没有星星 直接返回
            if (_stars == null || StarCount != _stars.Length)
                return;

            //生成星星间的连线
            for (var i = 0; i < StarCount - 1; i++)
                for (var j = i + 1; j < StarCount; j++)
                {
                    var star1 = _stars[i];
                    var x1 = star1.X + star1.StarRef.Width / 2;
                    var y1 = star1.Y + star1.StarRef.Height / 2;
                    var star2 = _stars[j];
                    var x2 = star2.X + star2.StarRef.Width / 2;
                    var y2 = star2.Y + star2.StarRef.Height / 2;
                    var s = Math.Sqrt((y2 - y1) * (y2 - y1) + (x2 - x1) * (x2 - x1)); //两个星星间的距离
                    var threshold = star1.StarRef.Width * LineRate + star2.StarRef.Width * LineRate;
                    if (s <= threshold)
                    {
                        if (!star1.StarLines.ContainsKey(star2))
                        {
                            var line = new Line
                            {
                                X1 = x1,
                                Y1 = y1,
                                X2 = x2,
                                Y2 = y2,
                                Stroke = GetStarLineBrush(star1.StarRef, star2.StarRef)
                            };
                            star1.StarLines.Add(star2, line);
                            _grid.Children.Add(line);
                        }
                    }
                    else
                    {
                        if (star1.StarLines.ContainsKey(star2))
                        {
                            _grid.Children.Remove(star1.StarLines[star2]);
                            star1.StarLines.Remove(star2);
                        }
                    }
                }
        }

        /// <summary>
        ///     移动星星之间的连线
        /// </summary>
        private void MoveStarLine()
        {
            //没有星星 直接返回
            if (_stars == null)
                return;

            foreach (var star in _stars)
                foreach (var starLine in star.StarLines)
                {
                    var line = starLine.Value;
                    line.X1 = star.X + star.StarRef.Width / 2;
                    line.Y1 = star.Y + star.StarRef.Height / 2;
                    line.X2 = starLine.Key.X + starLine.Key.StarRef.Width / 2;
                    line.Y2 = starLine.Key.Y + starLine.Key.StarRef.Height / 2;
                }
        }

        /// <summary>
        ///     获取星星连线颜色画刷
        /// </summary>
        /// <param name="star0">起始星星</param>
        /// <param name="star1">终点星星</param>
        /// <returns>LinearGradientBrush</returns>
        private LinearGradientBrush GetStarLineBrush(Path star0, Path star1)
        {
            return new LinearGradientBrush
            {
                GradientStops = new GradientStopCollection
                {
                    new GradientStop { Offset = 0, Color = (star0.Fill as SolidColorBrush).Color },
                    new GradientStop { Offset = 1, Color = (star1.Fill as SolidColorBrush).Color }
                }
            };
        }
    }

    /// <summary>
    ///     星星
    /// </summary>
    internal class StarInfo
    {
        /// <summary>
        ///     X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        ///     X轴速度(单位距离/帧)
        /// </summary>
        public double XV { get; set; }

        /// <summary>
        ///     X坐标以X轴速度运行的时间(帧)
        /// </summary>
        public int XT { get; set; }

        /// <summary>
        ///     Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        ///     Y轴速度(单位距离/帧)
        /// </summary>
        public double YV { get; set; }

        /// <summary>
        ///     Y坐标以Y轴速度运行的时间(帧)
        /// </summary>
        public int YT { get; set; }

        /// <summary>
        ///     对星星的引用
        /// </summary>
        public Path StarRef { get; set; }

        public Dictionary<StarInfo, Line> StarLines { get; set; }
    }
}