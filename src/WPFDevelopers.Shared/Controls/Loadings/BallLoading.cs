using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = Part_BackGridTemplateName, Type = typeof(Grid))]
    [TemplatePart(Name = Part_EillipseTemplateName, Type = typeof(Ellipse))]
    [TemplatePart(Name = Part_EillpseDock1TemplateName, Type = typeof(Grid))]
    [TemplatePart(Name = Part_Eillipse1TemplateName, Type = typeof(Ellipse))]
    [TemplatePart(Name = Part_EillpseDock2TemplateName, Type = typeof(Grid))]
    [TemplatePart(Name = Part_Eillipse2TemplateName, Type = typeof(Ellipse))]
    [TemplatePart(Name = Part_EillpseDock3TemplateName, Type = typeof(Grid))]
    [TemplatePart(Name = Part_Eillipse3TemplateName, Type = typeof(Ellipse))]
    [TemplatePart(Name = Part_EillpseDock4TemplateName, Type = typeof(Grid))]
    [TemplatePart(Name = Part_Eillipse4TemplateName, Type = typeof(Ellipse))]
    public class BallLoading : Control
    {
        private const string Part_BackGridTemplateName = "Part_BackGrid";
        private const string Part_EillipseTemplateName = "Part_Eillipse";
        private const string Part_EillpseDock1TemplateName = "Part_EillpseDock1";
        private const string Part_Eillipse1TemplateName = "Part_Eillipse1";
        private const string Part_EillpseDock2TemplateName = "Part_EillpseDock2";
        private const string Part_Eillipse2TemplateName = "Part_Eillipse2";
        private const string Part_EillpseDock3TemplateName = "Part_EillpseDock3";
        private const string Part_Eillipse3TemplateName = "Part_Eillipse3";
        private const string Part_EillpseDock4TemplateName = "Part_EillpseDock4";
        private const string Part_Eillipse4TemplateName = "Part_Eillipse4";

        private const int _nBallCount = 5;

        // Using a DependencyProperty as the backing store for IsStartAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStartAnimationProperty =
            DependencyProperty.Register("IsStartAnimation", typeof(bool), typeof(BallLoading),
                new PropertyMetadata(true, OnPropertyChangedCallback));

        private readonly int _AnimationSlowTime = 500;

        private double _BallFrom;

        private readonly double _BallSize = 40;
        private double _BallTo;

        private bool _bLoadedAnimation;

        private bool _IsStart;

        private readonly Dictionary<int, Tuple<Color, Color>> _mapBallColors =
            new Dictionary<int, Tuple<Color, Color>>();

        private readonly int _OffsetTime = 1;

        private readonly int _SingleAnimationTime = 4;

        //
        private Storyboard _Storyboard;

        private Storyboard _Storyboard1;

        private Grid Part_BackGrid;
        private Ellipse Part_Eillipse;
        private Ellipse Part_Eillipse1;
        private Ellipse Part_Eillipse2;
        private Ellipse Part_Eillipse3;
        private Ellipse Part_Eillipse4;
        private Grid Part_EillpseDock1;
        private Grid Part_EillpseDock2;
        private Grid Part_EillpseDock3;
        private Grid Part_EillpseDock4;

        static BallLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BallLoading),
                new FrameworkPropertyMetadata(typeof(BallLoading)));
        }

        public BallLoading()
        {
            Loaded += BallLoading_Loaded;
            Unloaded += BallLoading_Unloaded;
        }


        public bool IsStartAnimation
        {
            get => (bool)GetValue(IsStartAnimationProperty);
            set => SetValue(IsStartAnimationProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Part_BackGrid = GetTemplateChild(Part_BackGridTemplateName) as Grid;
            Part_Eillipse = GetTemplateChild(Part_EillipseTemplateName) as Ellipse;
            Part_EillpseDock1 = GetTemplateChild(Part_EillpseDock1TemplateName) as Grid;
            Part_Eillipse1 = GetTemplateChild(Part_Eillipse1TemplateName) as Ellipse;
            Part_EillpseDock2 = GetTemplateChild(Part_EillpseDock2TemplateName) as Grid;
            Part_Eillipse2 = GetTemplateChild(Part_Eillipse2TemplateName) as Ellipse;
            Part_EillpseDock3 = GetTemplateChild(Part_EillpseDock3TemplateName) as Grid;
            Part_Eillipse3 = GetTemplateChild(Part_Eillipse3TemplateName) as Ellipse;
            Part_EillpseDock4 = GetTemplateChild(Part_EillpseDock4TemplateName) as Grid;
            Part_Eillipse4 = GetTemplateChild(Part_Eillipse4TemplateName) as Ellipse;
        }

        private void BallLoading_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingProcedure();
        }


        private void BallLoading_Unloaded(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;

            if (d is BallLoading control)
                if (e.Property == IsStartAnimationProperty)
                    if (bool.TryParse(e.NewValue.ToString(), out var bResult))
                    {
                        if (bResult)
                            control.Start();
                        else
                            control.Stop();
                    }
        }

        private bool LoadingProcedure()
        {
            if (_bLoadedAnimation)
                return true;

            CalculateContainer();
            LoadResource();
            LoadAnimation();

            _bLoadedAnimation = true;
            return true;
        }

        private bool CalculateContainer()
        {
            var vActualWidth = Part_BackGrid.ActualWidth;
            var vSingleWidth = vActualWidth / _nBallCount;

            var vMargin = (vSingleWidth - _BallSize) / 2;

            _BallFrom = vMargin;
            _BallTo = vActualWidth - vMargin - _BallSize;

            return true;
        }


        private bool LoadResource()
        {
            _mapBallColors.Clear();

            var tuple0 = new Tuple<Color, Color>(Color.FromRgb(0xff, 0x93, 0x03), Color.FromRgb(0xff, 0x8c, 0x09));
            _mapBallColors[0] = tuple0;

            var tuple1 = new Tuple<Color, Color>(Color.FromRgb(0xff, 0x78, 0x1a), Color.FromRgb(0xff, 0x6a, 0x27));
            _mapBallColors[1] = tuple1;

            var tuple2 = new Tuple<Color, Color>(Color.FromRgb(0xff, 0x55, 0x39), Color.FromRgb(0xff, 0x48, 0x45));
            _mapBallColors[2] = tuple2;

            var tuple3 = new Tuple<Color, Color>(Color.FromRgb(0xff, 0x33, 0x57), Color.FromRgb(0xff, 0x24, 0x65));
            _mapBallColors[3] = tuple3;

            var tuple4 = new Tuple<Color, Color>(Color.FromRgb(0xff, 0x09, 0x7c), Color.FromRgb(0xff, 0x03, 0x82));
            _mapBallColors[4] = tuple4;


            var rotate1 = new RotateTransform();
            Part_EillpseDock1.RenderTransform = rotate1;

            var rotate2 = new RotateTransform();
            Part_EillpseDock2.RenderTransform = rotate2;

            var rotate3 = new RotateTransform();
            Part_EillpseDock3.RenderTransform = rotate3;

            var rotate4 = new RotateTransform();
            Part_EillpseDock4.RenderTransform = rotate4;

            return true;
        }


        private bool LoadAnimation()
        {
            _Storyboard = new Storyboard();
            _Storyboard.Children.Clear();
            _Storyboard.SpeedRatio = 2;
            _Storyboard.Completed += _Storyboard_Completed;

            LoadAnimationFromLeftToRight(_Storyboard);
            LoadAnimationBallLToRTurnColors(_Storyboard);

            LoadAnimationBall1Rotate(_Storyboard);


            _Storyboard1 = new Storyboard();
            _Storyboard1.Children.Clear();
            _Storyboard1.SpeedRatio = 2;
            _Storyboard1.Completed += _Storyboard1_Completed;

            LoadAnimationFromRightToLeft(_Storyboard1);
            LoadAnimationBallRToLTurnColors(_Storyboard1);

            LoadAnimationBall1RotateBack(_Storyboard1);


            if (IsStartAnimation)
                Start();

            return true;
        }


        //第一个小球 从左向右移动 从0号位移动到4号位  单程4s 动画缓动 500ms
        private bool LoadAnimationFromLeftToRight(Storyboard storyboard)
        {
            var animation = new DoubleAnimation
            {
                From = _BallFrom,
                To = _BallTo,
                BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime),
                Duration = new Duration(new TimeSpan(0, 0, _SingleAnimationTime))
            };

            Storyboard.SetTarget(animation, Part_Eillipse);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(animation);

            return true;
        }

        //第一个小球 从右向左移动 从4号位移动到0号位  单程4s 动画缓动 500ms
        private bool LoadAnimationFromRightToLeft(Storyboard storyboard)
        {
            var animation = new DoubleAnimation
            {
                From = _BallTo,
                To = _BallFrom,
                BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime),
                Duration = new Duration(new TimeSpan(0, 0, _SingleAnimationTime))
            };

            Storyboard.SetTarget(animation, Part_Eillipse);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(animation);

            return true;
        }

        //第一个小球 从左向右 过程中颜色变换 
        private bool LoadAnimationBallLToRTurnColors(Storyboard storyboard)
        {
            {
                //第一个小球移动过程中变色 移动到下一个位置前变色

                var animation1 = BallMoveColor(Part_Eillipse, 0, 0, 1);
                storyboard.Children.Add(animation1);

                var animation2 = BallMoveColor(Part_Eillipse, 1, 0, 1);
                storyboard.Children.Add(animation2);
            }

            {
                //第一个小球移动过程中变色 移动到下一个位置前变色

                var animation1 = BallMoveColor(Part_Eillipse, 0, 1, 2);
                storyboard.Children.Add(animation1);

                var animation2 = BallMoveColor(Part_Eillipse, 1, 1, 2);
                storyboard.Children.Add(animation2);
            }

            {
                //第一个小球移动过程中变色 移动到下一个位置前变色

                var animation1 = BallMoveColor(Part_Eillipse, 0, 2, 3);
                storyboard.Children.Add(animation1);

                var animation2 = BallMoveColor(Part_Eillipse, 1, 2, 3);
                storyboard.Children.Add(animation2);
            }

            {
                //第一个小球移动过程中变色 移动到下一个位置前变色

                var animation1 = BallMoveColor(Part_Eillipse, 0, 3, 4);
                storyboard.Children.Add(animation1);

                var animation2 = BallMoveColor(Part_Eillipse, 1, 3, 4);
                storyboard.Children.Add(animation2);
            }

            return true;
        }

        //第一个小球 从右向左 过程中颜色变换 
        private bool LoadAnimationBallRToLTurnColors(Storyboard storyboard)
        {
            //从右向左
            {
                //第一个小球移动过程中变色 移动到下一个位置前变色

                var animation1 = BallMoveColor(Part_Eillipse, 0, 4, 3);
                storyboard.Children.Add(animation1);

                var animation2 = BallMoveColor(Part_Eillipse, 1, 4, 3);
                storyboard.Children.Add(animation2);
            }


            {
                //第一个小球移动过程中变色 移动到下一个位置前变色

                var animation1 = BallMoveColor(Part_Eillipse, 0, 3, 2);
                storyboard.Children.Add(animation1);

                var animation2 = BallMoveColor(Part_Eillipse, 1, 3, 2);
                storyboard.Children.Add(animation2);
            }

            {
                //第一个小球移动过程中变色 移动到下一个位置前变色

                var animation1 = BallMoveColor(Part_Eillipse, 0, 2, 1);
                storyboard.Children.Add(animation1);

                var animation2 = BallMoveColor(Part_Eillipse, 1, 2, 1);
                storyboard.Children.Add(animation2);
            }

            {
                //第一个小球移动过程中变色 移动到下一个位置前变色

                var animation1 = BallMoveColor(Part_Eillipse, 0, 1, 0);
                storyboard.Children.Add(animation1);

                var animation2 = BallMoveColor(Part_Eillipse, 1, 1, 0);
                storyboard.Children.Add(animation2);
            }

            return true;
        }

        //替补小球换位过程
        private bool LoadAnimationBall1Rotate(Storyboard storyboard)
        {
            var nIndex = 0;

            {
                //替补小球1 从0-180度翻转 
                var animation = new DoubleAnimation
                {
                    From = 0,
                    To = 180,
                    BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime + nIndex * 1000),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation, Part_EillpseDock1);
                Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.Angle"));
                storyboard.Children.Add(animation);

                //替补小球1 从1号位移动到0号位颜色变化
                var color1 = new Color();
                var color2 = new Color();
                var vResult = _mapBallColors[0];
                if (vResult != null)
                {
                    color1 = vResult.Item1;
                    color2 = vResult.Item2;
                }

                var animation1 = new ColorAnimation
                {
                    To = color1,
                    BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime + nIndex * 1000),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation1, Part_Eillipse1);
                Storyboard.SetTargetProperty(animation1,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)"));
                storyboard.Children.Add(animation1);

                var animation2 = new ColorAnimation
                {
                    To = color2,
                    BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime + nIndex * 1000),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };
                Storyboard.SetTarget(animation2, Part_Eillipse1);
                Storyboard.SetTargetProperty(animation2,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)"));
                storyboard.Children.Add(animation2);

                ++nIndex;
            }


            {
                //替补小球2 从0-180度翻转 
                var animation = new DoubleAnimation
                {
                    From = 0,
                    To = 180,
                    BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime + nIndex * 1000),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation, Part_EillpseDock2);
                Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.Angle"));
                storyboard.Children.Add(animation);

                //替补小球2 从2号位移动到1号位颜色变化
                var color1 = new Color();
                var color2 = new Color();
                var vResult = _mapBallColors[1];
                if (vResult != null)
                {
                    color1 = vResult.Item1;
                    color2 = vResult.Item2;
                }

                var animation1 = new ColorAnimation
                {
                    To = color1,
                    BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime + nIndex * 1000),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation1, Part_Eillipse2);
                Storyboard.SetTargetProperty(animation1,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)"));
                storyboard.Children.Add(animation1);

                var animation2 = new ColorAnimation
                {
                    To = color2,
                    BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime + nIndex * 1000),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };
                Storyboard.SetTarget(animation2, Part_Eillipse2);
                Storyboard.SetTargetProperty(animation2,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)"));
                storyboard.Children.Add(animation2);

                ++nIndex;
            }

            {
                //替补小球3 从0-180度翻转 
                var animation = new DoubleAnimation
                {
                    From = 0,
                    To = 180,
                    BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime + nIndex * 1000),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation, Part_EillpseDock3);
                Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.Angle"));
                storyboard.Children.Add(animation);

                //替补小球3 从3号位移动到2号位颜色变化
                var color1 = new Color();
                var color2 = new Color();
                var vResult = _mapBallColors[2];
                if (vResult != null)
                {
                    color1 = vResult.Item1;
                    color2 = vResult.Item2;
                }

                var animation1 = new ColorAnimation
                {
                    To = color1,
                    BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime + nIndex * 1000),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation1, Part_Eillipse3);
                Storyboard.SetTargetProperty(animation1,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)"));
                storyboard.Children.Add(animation1);

                var animation2 = new ColorAnimation
                {
                    To = color2,
                    BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime + nIndex * 1000),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };
                Storyboard.SetTarget(animation2, Part_Eillipse3);
                Storyboard.SetTargetProperty(animation2,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)"));
                storyboard.Children.Add(animation2);

                ++nIndex;
            }

            {
                //替补小球4 从0-180度翻转 
                var animation = new DoubleAnimation
                {
                    From = 0,
                    To = 180,
                    BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime + nIndex * 1000),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation, Part_EillpseDock4);
                Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.Angle"));
                storyboard.Children.Add(animation);

                //替补小球4 从4号位移动到3号位颜色变化
                var color1 = new Color();
                var color2 = new Color();
                var vResult = _mapBallColors[3];
                if (vResult != null)
                {
                    color1 = vResult.Item1;
                    color2 = vResult.Item2;
                }

                var animation1 = new ColorAnimation
                {
                    To = color1,
                    BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime + nIndex * 1000),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation1, Part_Eillipse4);
                Storyboard.SetTargetProperty(animation1,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)"));
                storyboard.Children.Add(animation1);

                var animation2 = new ColorAnimation
                {
                    To = color2,
                    BeginTime = TimeSpan.FromMilliseconds(_AnimationSlowTime + nIndex * 1000),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };
                Storyboard.SetTarget(animation2, Part_Eillipse4);
                Storyboard.SetTargetProperty(animation2,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)"));
                storyboard.Children.Add(animation2);

                ++nIndex;
            }

            return true;
        }

        //替补小球回位过程
        private bool LoadAnimationBall1RotateBack(Storyboard storyboard)
        {
            var nIndex = 0;

            {
                //替补小球4 从180 -360翻转
                var animation = new DoubleAnimation
                {
                    From = 180,
                    To = 360,
                    BeginTime = TimeSpan.FromMilliseconds(nIndex * 1000 + _AnimationSlowTime),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation, Part_EillpseDock4);
                Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.Angle"));
                storyboard.Children.Add(animation);

                //替补小球4 从3号位移动到4号位颜色变化
                var color1 = new Color();
                var color2 = new Color();
                var vResult = _mapBallColors[4];
                if (vResult != null)
                {
                    color1 = vResult.Item1;
                    color2 = vResult.Item2;
                }

                var animation1 = new ColorAnimation
                {
                    To = color1,
                    BeginTime = TimeSpan.FromMilliseconds(nIndex * 1000 + _AnimationSlowTime),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation1, Part_Eillipse4);
                Storyboard.SetTargetProperty(animation1,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)"));
                storyboard.Children.Add(animation1);

                var animation2 = new ColorAnimation
                {
                    To = color2,
                    BeginTime = TimeSpan.FromMilliseconds(nIndex * 1000 + _AnimationSlowTime),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };
                Storyboard.SetTarget(animation2, Part_Eillipse4);
                Storyboard.SetTargetProperty(animation2,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)"));
                storyboard.Children.Add(animation2);


                ++nIndex;
            }

            {
                //替补小球3 从180 -360翻转
                var animation = new DoubleAnimation
                {
                    From = 180,
                    To = 360,
                    BeginTime = TimeSpan.FromMilliseconds(nIndex * 1000 + _AnimationSlowTime),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation, Part_EillpseDock3);
                Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.Angle"));
                storyboard.Children.Add(animation);

                //替补小球3 从2号位移动到3号位颜色变化
                var color1 = new Color();
                var color2 = new Color();
                var vResult = _mapBallColors[3];
                if (vResult != null)
                {
                    color1 = vResult.Item1;
                    color2 = vResult.Item2;
                }

                var animation1 = new ColorAnimation
                {
                    To = color1,
                    BeginTime = TimeSpan.FromMilliseconds(nIndex * 1000 + _AnimationSlowTime),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation1, Part_Eillipse3);
                Storyboard.SetTargetProperty(animation1,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)"));
                storyboard.Children.Add(animation1);

                var animation2 = new ColorAnimation
                {
                    To = color2,
                    BeginTime = TimeSpan.FromMilliseconds(nIndex * 1000 + _AnimationSlowTime),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };
                Storyboard.SetTarget(animation2, Part_Eillipse3);
                Storyboard.SetTargetProperty(animation2,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)"));
                storyboard.Children.Add(animation2);


                ++nIndex;
            }

            {
                //替补小球2 从180 -360翻转
                var animation = new DoubleAnimation
                {
                    From = 180,
                    To = 360,
                    BeginTime = TimeSpan.FromMilliseconds(nIndex * 1000 + _AnimationSlowTime),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation, Part_EillpseDock2);
                Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.Angle"));
                storyboard.Children.Add(animation);

                //替补小球2 从1号位移动到2号位颜色变化
                var color1 = new Color();
                var color2 = new Color();
                var vResult = _mapBallColors[2];
                if (vResult != null)
                {
                    color1 = vResult.Item1;
                    color2 = vResult.Item2;
                }

                var animation1 = new ColorAnimation
                {
                    To = color1,
                    BeginTime = TimeSpan.FromMilliseconds(nIndex * 1000 + _AnimationSlowTime),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation1, Part_Eillipse2);
                Storyboard.SetTargetProperty(animation1,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)"));
                storyboard.Children.Add(animation1);

                var animation2 = new ColorAnimation
                {
                    To = color2,
                    BeginTime = TimeSpan.FromMilliseconds(nIndex * 1000 + _AnimationSlowTime),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };
                Storyboard.SetTarget(animation2, Part_Eillipse2);
                Storyboard.SetTargetProperty(animation2,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)"));
                storyboard.Children.Add(animation2);


                ++nIndex;
            }

            {
                //替补小球1 从180 -360翻转
                var animation = new DoubleAnimation
                {
                    From = 180,
                    To = 360,
                    BeginTime = TimeSpan.FromMilliseconds(nIndex * 1000 + _AnimationSlowTime),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation, Part_EillpseDock1);
                Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.Angle"));
                storyboard.Children.Add(animation);

                //替补小球1 从0号位移动到1号位颜色变化
                var color1 = new Color();
                var color2 = new Color();
                var vResult = _mapBallColors[1];
                if (vResult != null)
                {
                    color1 = vResult.Item1;
                    color2 = vResult.Item2;
                }

                var animation1 = new ColorAnimation
                {
                    To = color1,
                    BeginTime = TimeSpan.FromMilliseconds(nIndex * 1000 + _AnimationSlowTime),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };

                Storyboard.SetTarget(animation1, Part_Eillipse1);
                Storyboard.SetTargetProperty(animation1,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)"));
                storyboard.Children.Add(animation1);

                var animation2 = new ColorAnimation
                {
                    To = color2,
                    BeginTime = TimeSpan.FromMilliseconds(nIndex * 1000 + _AnimationSlowTime),
                    Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
                };
                Storyboard.SetTarget(animation2, Part_Eillipse1);
                Storyboard.SetTargetProperty(animation2,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)"));
                storyboard.Children.Add(animation2);
            }


            return true;
        }


        private ColorAnimation BallMoveColor(DependencyObject dependencyObject, int nIndex, int from, int to)
        {
            var color = new Color();
            var beginTime = 0;

            if (from <= to)
                beginTime = from * 1000 + _AnimationSlowTime;
            else
                beginTime = (_SingleAnimationTime - from) * 1000 + _AnimationSlowTime;

            switch (to)
            {
                case 1:
                {
                    var vResult = _mapBallColors[1];
                    if (vResult != null)
                    {
                        if (nIndex == 0)
                            color = vResult.Item1;
                        else
                            color = vResult.Item2;
                    }
                }
                    break;
                case 2:
                {
                    var vResult = _mapBallColors[2];
                    if (vResult != null)
                    {
                        if (nIndex == 0)
                            color = vResult.Item1;
                        else
                            color = vResult.Item2;
                    }
                }
                    break;
                case 3:
                {
                    var vResult = _mapBallColors[3];
                    if (vResult != null)
                    {
                        if (nIndex == 0)
                            color = vResult.Item1;
                        else
                            color = vResult.Item2;
                    }
                }
                    break;
                case 4:
                {
                    var vResult = _mapBallColors[4];
                    if (vResult != null)
                    {
                        if (nIndex == 0)
                            color = vResult.Item1;
                        else
                            color = vResult.Item2;
                    }
                }
                    break;
                default:
                {
                    var vResult = _mapBallColors[0];
                    if (vResult != null)
                    {
                        if (nIndex == 0)
                            color = vResult.Item1;
                        else
                            color = vResult.Item2;
                    }
                }
                    break;
            }

            var animation = new ColorAnimation
            {
                To = color,
                BeginTime = TimeSpan.FromMilliseconds(beginTime),
                Duration = new Duration(new TimeSpan(0, 0, _OffsetTime))
            };

            Storyboard.SetTarget(animation, dependencyObject);

            if (nIndex == 0)
                Storyboard.SetTargetProperty(animation,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[0].(GradientStop.Color)"));
            else
                Storyboard.SetTargetProperty(animation,
                    new PropertyPath("(Ellipse.Fill).(LinearGradientBrush.GradientStops)[1].(GradientStop.Color)"));

            return animation;
        }

        private void _Storyboard_Completed(object sender, EventArgs e)
        {
            if (Part_EillpseDock1.RenderTransform is RotateTransform rotate1)
                rotate1.Angle = 180;

            if (Part_EillpseDock2.RenderTransform is RotateTransform rotate2)
                rotate2.Angle = 180;

            if (Part_EillpseDock3.RenderTransform is RotateTransform rotate3)
                rotate3.Angle = 180;

            if (Part_EillpseDock4.RenderTransform is RotateTransform rotate4)
                rotate4.Angle = 180;

            Canvas.SetLeft(Part_Eillipse, _BallTo);

            if (IsStartAnimation)
                _Storyboard1.Begin();
        }

        private void _Storyboard1_Completed(object sender, EventArgs e)
        {
            if (Part_EillpseDock1.RenderTransform is RotateTransform rotate1)
                rotate1.Angle = 0;

            if (Part_EillpseDock2.RenderTransform is RotateTransform rotate2)
                rotate2.Angle = 0;

            if (Part_EillpseDock3.RenderTransform is RotateTransform rotate3)
                rotate3.Angle = 0;

            if (Part_EillpseDock4.RenderTransform is RotateTransform rotate4)
                rotate4.Angle = 0;

            Canvas.SetLeft(Part_Eillipse, _BallFrom);

            if (IsStartAnimation)
                _Storyboard.Begin();
        }


        private bool Start()
        {
            if (_Storyboard == null || _Storyboard1 == null)
                return false;

            if (_IsStart)
                return true;

            _IsStart = true;
            _Storyboard.Begin();

            return true;
        }

        private bool Stop()
        {
            _Storyboard?.Stop();
            _Storyboard1?.Stop();
            _IsStart = false;
            return true;
        }

        private bool Close()
        {
            Stop();

            _Storyboard = null;
            _Storyboard1 = null;

            return true;
        }
    }
}