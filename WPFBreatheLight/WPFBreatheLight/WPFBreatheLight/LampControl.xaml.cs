using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFBreatheLight
{
    public enum LampEffect
    {
        OuterGlow,
        Eclipse,
        Ripple
    }

    /// <summary>
    /// Interaction logic for LampControl.xaml
    /// </summary>
    [DefaultProperty("Geometry")]
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    [ContentProperty("Geometry")]
    public partial class LampControl : UserControl
    {
        public LampControl()
        {
            InitializeComponent();
        }

        private new object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        private static new readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(LampControl), new PropertyMetadata(default(object)));


        private new DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentTemplate.  This enables animation, styling, binding, etc...
        private static new readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(LampControl), new PropertyMetadata(default(DataTemplate)));

        // Using a DependencyProperty as the backing store for FillBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillBackgroundProperty =
            DependencyProperty.Register("FillBackground", typeof(Brush), typeof(LampControl), new PropertyMetadata(Brushes.LightGray));

        // Using a DependencyProperty as the backing store for FillForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(LampControl), new PropertyMetadata(Brushes.White));

        // Using a DependencyProperty as the backing store for Geometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry), typeof(LampControl), new PropertyMetadata(default(Geometry)));

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(LampControl), new PropertyMetadata(Brushes.White));

        // Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LampControl), new PropertyMetadata(0d));

        // Using a DependencyProperty as the backing store for GeometryPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GeometryPaddingProperty =
            DependencyProperty.Register("GeometryPadding", typeof(Thickness), typeof(LampControl), new PropertyMetadata(new Thickness(15d)));

        // Using a DependencyProperty as the backing store for IsLampLight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLampLightProperty =
            DependencyProperty.Register("IsLampLight", typeof(bool), typeof(LampControl), new PropertyMetadata(default(bool), OnPropertyChangedCallBack));

        // Using a DependencyProperty as the backing store for LampEffect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LampEffectProperty =
            DependencyProperty.Register("LampEffect", typeof(LampEffect), typeof(LampControl), new PropertyMetadata(default(LampEffect), OnLampEffectPropertyChangedCallBack));

        // Using a DependencyProperty as the backing store for BlurRadius.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register("BlurRadius", typeof(double), typeof(LampControl), new PropertyMetadata(0d));

        // Using a DependencyProperty as the backing store for LampLightColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LampLightColorProperty =
            DependencyProperty.Register("LampLightColor", typeof(Color), typeof(LampControl), new PropertyMetadata(Colors.Red, OnLampLightColorPropertyChangedCallBack));

        // Using a DependencyProperty as the backing store for LampLightBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LampLightBrushProperty =
            DependencyProperty.Register("LampLightBrush", typeof(Brush), typeof(LampControl), new PropertyMetadata(Brushes.Red));

        // Using a DependencyProperty as the backing store for AssistLampLightColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AssistLampLightColorProperty =
            DependencyProperty.Register("AssistLampLightColor", typeof(Color), typeof(LampControl), new PropertyMetadata(Color.FromArgb(255, 0XFC, 0x73, 0x73)));

        // Using a DependencyProperty as the backing store for LampBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LampBorderThicknessProperty =
            DependencyProperty.Register("LampBorderThickness", typeof(Thickness), typeof(LampControl), new PropertyMetadata(new Thickness(0d)));

        // Using a DependencyProperty as the backing store for LampBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LampBorderBrushProperty =
            DependencyProperty.Register("LampBorderBrush", typeof(Brush), typeof(LampControl), new PropertyMetadata(default(Brush)));

        // Using a DependencyProperty as the backing store for LampCornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LampCornerRadiusProperty =
            DependencyProperty.Register("LampCornerRadius", typeof(CornerRadius), typeof(LampControl), new PropertyMetadata(new CornerRadius(100d)));


        public Thickness LampBorderThickness
        {
            get { return (Thickness)GetValue(LampBorderThicknessProperty); }
            set { SetValue(LampBorderThicknessProperty, value); }
        }

        public Brush LampBorderBrush
        {
            get { return (Brush)GetValue(LampBorderBrushProperty); }
            set { SetValue(LampBorderBrushProperty, value); }
        }

        public CornerRadius LampCornerRadius
        {
            get { return (CornerRadius)GetValue(LampCornerRadiusProperty); }
            set { SetValue(LampCornerRadiusProperty, value); }
        }

        public Color LampLightColor
        {
            get { return (Color)GetValue(LampLightColorProperty); }
            set { SetValue(LampLightColorProperty, value); }
        }

        public Color AssistLampLightColor
        {
            get { return (Color)GetValue(AssistLampLightColorProperty); }
            set { SetValue(AssistLampLightColorProperty, value); }
        }
        public Brush LampLightBrush
        {
            get { return (Brush)GetValue(LampLightBrushProperty); }
            set { SetValue(LampLightBrushProperty, value); }
        }

        public LampEffect LampEffect
        {
            get { return (LampEffect)GetValue(LampEffectProperty); }
            set { SetValue(LampEffectProperty, value); }
        }

        public Brush FillBackground
        {
            get { return (Brush)GetValue(FillBackgroundProperty); }
            set { SetValue(FillBackgroundProperty, value); }
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public Geometry Geometry
        {
            get { return (Geometry)GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public Thickness GeometryPadding
        {
            get { return (Thickness)GetValue(GeometryPaddingProperty); }
            set { SetValue(GeometryPaddingProperty, value); }
        }

        public bool IsLampLight
        {
            get { return (bool)GetValue(IsLampLightProperty); }
            set { SetValue(IsLampLightProperty, value); }
        }

        internal double BlurRadius
        {
            get { return (double)GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }

        private static void OnPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LampControl lampControl)
            {
                if (e.Property == IsLampLightProperty)
                {
                    if (bool.TryParse(e.NewValue?.ToString(), out bool bResult))
                    {
                        if (bResult)
                            lampControl.Start();
                        else
                            lampControl.Stop();
                    }
                }
            }

        }

        private static void OnLampEffectPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not LampControl lampControl)
                return;

            if (e.Property == LampEffectProperty)
            {
                if (lampControl.IsLampLight)
                {
                    lampControl.Stop();
                    lampControl.Start();
                }
            }
        }

        private static void OnLampLightColorPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not LampControl lampControl)
                return;

            if (e.Property == LampLightColorProperty)
            {
                if (e.NewValue is Color color)
                {
                    byte R = color.R;
                    byte G = color.G;
                    byte B = color.B;

                    if (R + 10 > 255)
                        R = (byte)(R - 10);
                    else
                        R = (byte)(R + 10);

                    if (G + 10 > 255)
                        G = (byte)(G - 10);
                    else
                        G = (byte)(G + 10);

                    if (B + 10 > 255)
                        B = (byte)(B - 10);
                    else
                        B = (byte)(B + 10);

                    lampControl.AssistLampLightColor = Color.FromRgb(R, G, B);
                }
            }
        }


        #region Storyboard

        private Storyboard _StoryBoard = new();
        private double _Duration = 600;

        private bool Start()
        {
            _StoryBoard.Children.Clear();

            switch (LampEffect)
            {
                case LampEffect.OuterGlow:
                    {
                        DoubleAnimation animation1 = new()
                        {
                            RepeatBehavior = RepeatBehavior.Forever,
                            AutoReverse = true,
                            From = 0d,
                            To = 40d,
                            Duration = new Duration(TimeSpan.FromMilliseconds(_Duration)),
                        };

                        Storyboard.SetTarget(animation1, this);
                        Storyboard.SetTargetProperty(animation1, new PropertyPath("BlurRadius"));
                        _StoryBoard.Children.Add(animation1);

                        Part_LampOuter.Visibility = Visibility.Visible;
                    }
                    break;
                case LampEffect.Eclipse:
                    {
                        DoubleAnimation animation1 = new()
                        {
                            RepeatBehavior = RepeatBehavior.Forever,
                            AutoReverse = true,
                            From = 0.85,
                            To = 1,
                            Duration = new Duration(TimeSpan.FromMilliseconds(_Duration)),
                        };

                        Storyboard.SetTarget(animation1, Part_Lamp);
                        Storyboard.SetTargetProperty(animation1, new PropertyPath("(UIElement.RenderTransform).ScaleX"));
                        _StoryBoard.Children.Add(animation1);

                        DoubleAnimation animation2 = new()
                        {
                            RepeatBehavior = RepeatBehavior.Forever,
                            AutoReverse = true,
                            From = 0.85,
                            To = 1,
                            Duration = new Duration(TimeSpan.FromMilliseconds(_Duration)),
                        };

                        Storyboard.SetTarget(animation2, Part_Lamp);
                        Storyboard.SetTargetProperty(animation2, new PropertyPath("(UIElement.RenderTransform).ScaleY"));
                        _StoryBoard.Children.Add(animation2);

                        Part_Lamp.Visibility = Visibility.Visible;
                    }
                    break;
                case LampEffect.Ripple:
                    {
                        DoubleAnimation animation1 = new()
                        {
                            From = 0.7,
                            To = 1.1,
                            Duration = new Duration(TimeSpan.FromMilliseconds(_Duration * 2)),
                        };

                        Storyboard.SetTarget(animation1, Part_Ripple_1);
                        Storyboard.SetTargetProperty(animation1, new PropertyPath("(UIElement.RenderTransform).ScaleX"));
                        _StoryBoard.Children.Add(animation1);

                        DoubleAnimation animation2 = new()
                        {
                            From = 0.7,
                            To = 1.1,
                            Duration = new Duration(TimeSpan.FromMilliseconds(_Duration * 2)),
                        };

                        Storyboard.SetTarget(animation2, Part_Ripple_1);
                        Storyboard.SetTargetProperty(animation2, new PropertyPath("(UIElement.RenderTransform).ScaleY"));
                        _StoryBoard.Children.Add(animation2);

                        DoubleAnimation animation5 = new()
                        {
                            From = 1,
                            To = 0,
                            Duration = new Duration(TimeSpan.FromMilliseconds(_Duration * 4)),
                        };
                        Storyboard.SetTarget(animation5, Part_Ripple_1);
                        Storyboard.SetTargetProperty(animation5, new PropertyPath("Opacity"));
                        _StoryBoard.Children.Add(animation5);

                        DoubleAnimation animation3 = new()
                        {
                            BeginTime = TimeSpan.FromMilliseconds(_Duration),
                            From = 0.7,
                            To = 1.1,
                            Duration = new Duration(TimeSpan.FromMilliseconds(_Duration * 2)),
                        };

                        Storyboard.SetTarget(animation3, Part_Ripple_2);
                        Storyboard.SetTargetProperty(animation3, new PropertyPath("(UIElement.RenderTransform).ScaleX"));
                        _StoryBoard.Children.Add(animation3);

                        DoubleAnimation animation4 = new()
                        {
                            BeginTime = TimeSpan.FromMilliseconds(_Duration),
                            From = 0.7,
                            To = 1.1,
                            Duration = new Duration(TimeSpan.FromMilliseconds(_Duration * 2)),
                        };

                        Storyboard.SetTarget(animation4, Part_Ripple_2);
                        Storyboard.SetTargetProperty(animation4, new PropertyPath("(UIElement.RenderTransform).ScaleY"));
                        _StoryBoard.Children.Add(animation4);

                        DoubleAnimation animation6 = new()
                        {
                            BeginTime = TimeSpan.FromMilliseconds(_Duration),
                            From = 1,
                            To = 0,
                            Duration = new Duration(TimeSpan.FromMilliseconds(_Duration * 4)),
                        };
                        Storyboard.SetTarget(animation6, Part_Ripple_2);
                        Storyboard.SetTargetProperty(animation6, new PropertyPath("Opacity"));
                        _StoryBoard.Children.Add(animation6);

                        _StoryBoard.AutoReverse = false;
                        _StoryBoard.RepeatBehavior = RepeatBehavior.Forever;

                        Part_Ripple.Visibility = Visibility.Visible;
                    }
                    break;
                default:
                    return true;
            }

            _StoryBoard.Begin();

            return true;
        }

        private bool Stop()
        {
            _StoryBoard.Stop();
            _StoryBoard.Children.Clear();

            Part_Lamp.Visibility = Visibility.Collapsed;
            Part_LampOuter.Visibility = Visibility.Collapsed;
            Part_Ripple.Visibility = Visibility.Collapsed;

            if (Part_Lamp.RenderTransform is ScaleTransform vObject)
            {
                vObject.ScaleX = 0.85d;
                vObject.ScaleY = 0.85d;
            }

            BlurRadius = 0d;

            return true;
        }

    }
        #endregion
}

