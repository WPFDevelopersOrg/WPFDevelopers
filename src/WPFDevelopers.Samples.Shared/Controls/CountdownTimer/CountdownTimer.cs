using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace WPFDevelopers.Samples.Controls
{
    public enum CountdownTimerEffect
    {
        Default,
        MultiColor
    }

    public class CountdownTimer : ContentControl
    {
        private const double seconds = 1000;

        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register("Number", typeof(int), typeof(CountdownTimer), new PropertyMetadata(3));

        public static readonly DependencyProperty IsFinishStartProperty =
            DependencyProperty.Register("IsFinishStart", typeof(bool), typeof(CountdownTimer),
                new PropertyMetadata(false));

        public static readonly DependencyProperty CountdownTimerEffectProperty =
            DependencyProperty.Register("ExhibitionEnum", typeof(CountdownTimerEffect), typeof(CountdownTimer),
                new PropertyMetadata(CountdownTimerEffect.Default));

        private double currentSeconds = seconds;
        private Grid myGrid;
        private Storyboard storyboard;

        public int Number
        {
            get => (int)GetValue(NumberProperty);
            set => SetValue(NumberProperty, value);
        }


        /// <summary>
        ///     完成后回到开始
        /// </summary>
        public bool IsFinishStart
        {
            get => (bool)GetValue(IsFinishStartProperty);
            set => SetValue(IsFinishStartProperty, value);
        }


        public CountdownTimerEffect CountdownTimerEffect
        {
            get => (CountdownTimerEffect)GetValue(CountdownTimerEffectProperty);
            set => SetValue(CountdownTimerEffectProperty, value);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            NameScope.SetNameScope(this, new NameScope());
            if (FontSize == SystemFonts.CaptionFontSize)
                FontSize = 200;
            FontFamily = DrawingContextHelper.FontFamily;
            storyboard = new Storyboard();
            myGrid = new Grid();
            myGrid.Name = "myGrid";
            myGrid.ToolTip = "MouseDown";
            myGrid.Background = new SolidColorBrush(Colors.White);
            var linearGradient = new LinearGradientBrush
            {
                GradientStops = new GradientStopCollection
                {
                    new GradientStop { Color = Colors.Red, Offset = 1 },
                    new GradientStop { Color = Colors.White, Offset = 1 },
                    new GradientStop { Color = Colors.White, Offset = .5 },
                    new GradientStop { Color = Colors.Red, Offset = .5 },
                    new GradientStop { Color = Colors.Red, Offset = 0 },
                    new GradientStop { Color = Colors.White, Offset = 0 }
                },
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(10, 10),
                SpreadMethod = GradientSpreadMethod.Reflect,
                MappingMode = BrushMappingMode.Absolute
            };
            SolidColorBrush solidColor;
            RegisterName(myGrid.Name, myGrid);
            var num = 0;
            //switch (CountdownTimerEffect)
            //{
            //    case CountdownTimerEffect.MultiColor:
            //        num = 1;
            //        break;
            //}
            for (var i = Number; i >= num; i--)
            {
                var textBlock = new TextBlock();
                switch (CountdownTimerEffect)
                {
                    case CountdownTimerEffect.Default:
                        if (i % 2 == 0)
                            solidColor = Brushes.White;
                        else
                            solidColor = Brushes.Black;
                        textBlock.Foreground = solidColor;
                        break;
                    case CountdownTimerEffect.MultiColor:
                        textBlock.Foreground = linearGradient;
                        break;
                }

                textBlock.Text = i.ToString();
                textBlock.Name = $"textBlock{i}";
                textBlock.FontSize = FontSize;
                textBlock.FontWeight = FontWeights.ExtraBold;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.RenderTransformOrigin = new Point(.5, .5);
                textBlock.Effect = new DropShadowEffect
                {
                    ShadowDepth = 2,
                    RenderingBias = RenderingBias.Performance,
                    Color = Colors.Red
                };
                if (!i.Equals(Number))
                    textBlock.Opacity = 0;
                textBlock.RenderTransform = new ScaleTransform
                {
                    ScaleX = 2,
                    ScaleY = 2
                };
                RegisterName(textBlock.Name, textBlock);

                var beginTime = TimeSpan.Zero;
                if (storyboard.Children.Count > 0)
                {
                    beginTime = TimeSpan.FromMilliseconds(currentSeconds);
                    currentSeconds += seconds;
                }

                var cubicEase = new CubicEase
                {
                    EasingMode = EasingMode.EaseIn
                };
                var doubleAnimationScaleX = new DoubleAnimation();
                doubleAnimationScaleX.From = 2;
                doubleAnimationScaleX.To = 0;
                doubleAnimationScaleX.EasingFunction = cubicEase;


                Storyboard.SetTargetName(doubleAnimationScaleX, textBlock.Name);
                Storyboard.SetTargetProperty(doubleAnimationScaleX,
                    new PropertyPath("(TextBlock.RenderTransform).(ScaleTransform.ScaleX)"));

                var doubleAnimationScaleY = new DoubleAnimation
                {
                    From = 2,
                    To = 0,
                    EasingFunction = cubicEase
                };
                Storyboard.SetTargetName(doubleAnimationScaleY, textBlock.Name);
                Storyboard.SetTargetProperty(doubleAnimationScaleY,
                    new PropertyPath("(TextBlock.RenderTransform).(ScaleTransform.ScaleY)"));


                doubleAnimationScaleX.BeginTime = beginTime;
                doubleAnimationScaleY.BeginTime = beginTime;
                doubleAnimationScaleX.Duration = TimeSpan.FromMilliseconds(seconds);
                doubleAnimationScaleY.Duration = TimeSpan.FromMilliseconds(seconds);
                if (!i.Equals(Number))
                {
                    var doubleAnimationOpacity = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromMilliseconds(0),
                        BeginTime = beginTime,
                        From = 0,
                        To = 1
                    };
                    Storyboard.SetTargetName(doubleAnimationOpacity, textBlock.Name);
                    Storyboard.SetTargetProperty(doubleAnimationOpacity, new PropertyPath(OpacityProperty));
                    storyboard.Children.Add(doubleAnimationOpacity);
                }


                if (i % 2 == 0)
                {
                    var colorAnimation = new ColorAnimation
                    {
                        Duration = TimeSpan.FromMilliseconds(0),
                        From = Colors.White,
                        BeginTime = beginTime,
                        To = Colors.Black
                    };
                    Storyboard.SetTargetName(colorAnimation, myGrid.Name);
                    Storyboard.SetTargetProperty(colorAnimation,
                        new PropertyPath("(Panel.Background).(SolidColorBrush.Color)"));
                    storyboard.Children.Add(colorAnimation);
                }
                else
                {
                    if (!i.Equals(Number))
                    {
                        var colorAnimation = new ColorAnimation
                        {
                            Duration = TimeSpan.FromMilliseconds(0),
                            BeginTime = beginTime,
                            From = Colors.Black,
                            To = Colors.White
                        };
                        Storyboard.SetTargetName(colorAnimation, myGrid.Name);
                        Storyboard.SetTargetProperty(colorAnimation,
                            new PropertyPath("(Panel.Background).(SolidColorBrush.Color)"));
                        storyboard.Children.Add(colorAnimation);
                    }
                }


                storyboard.Children.Add(doubleAnimationScaleX);
                storyboard.Children.Add(doubleAnimationScaleY);


                myGrid.Children.Add(textBlock);
            }

            Content = myGrid;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (storyboard != null && storyboard.Children.Count > 0)
            {
                storyboard.Completed += (s, y) =>
                {
                    myGrid.Background = new SolidColorBrush(Colors.White);
                    if (IsFinishStart)
                    {
                        var scaleTransform = new ScaleTransform
                        {
                            ScaleX = 2,
                            ScaleY = 2
                        };
                        var tb = myGrid.Children.Cast<TextBlock>().First();
                        tb.RenderTransform = scaleTransform;
                    }
                };
                storyboard.Begin(this);
            }
        }
    }
}