using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = TextBlockBottomTemplateName, Type = typeof(TextBlock))]
    [TemplatePart(Name = TextBlockTopTemplateName, Type = typeof(TextBlock))]
    [TemplatePart(Name = EllipseGeometryTemplateName, Type = typeof(EllipseGeometry))]
    public class SpotLight : Control
    {
        private const string TextBlockBottomTemplateName = "PART_TextBlockBottom";
        private const string TextBlockTopTemplateName = "PART_TextBlockTop";
        private const string EllipseGeometryTemplateName = "PART_EllipseGeometry";

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SpotLight),
                new PropertyMetadata("WPFDevelopers"));

        public static readonly DependencyProperty DefaultForegroundProperty =
            DependencyProperty.Register("DefaultForeground", typeof(Brush), typeof(SpotLight),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323232"))));


        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(SpotLight),
                new PropertyMetadata(TimeSpan.FromSeconds(3)));


        private EllipseGeometry _ellipseGeometry;
        private TextBlock _textBlockBottom, _textBlockTop;

        static SpotLight()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpotLight),
                new FrameworkPropertyMetadata(typeof(SpotLight)));
        }

       
        public TimeSpan Duration
        {
            get => (TimeSpan)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }
        public Brush DefaultForeground
        {
            get => (Brush)GetValue(DefaultForegroundProperty);
            set => SetValue(DefaultForegroundProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _textBlockBottom = GetTemplateChild(TextBlockBottomTemplateName) as TextBlock;
            _textBlockTop = GetTemplateChild(TextBlockTopTemplateName) as TextBlock;

            _ellipseGeometry = GetTemplateChild(EllipseGeometryTemplateName) as EllipseGeometry;
            var center = new Point(FontSize / 2, FontSize / 2);
            _ellipseGeometry.RadiusX = FontSize;
            _ellipseGeometry.RadiusY = FontSize;
            _ellipseGeometry.Center = center;
            if (_textBlockBottom != null && _textBlockTop != null && _ellipseGeometry != null)
                _textBlockTop.Loaded += _textBlockTop_Loaded;
        }


        private void _textBlockTop_Loaded(object sender, RoutedEventArgs e)
        {
            var doubleAnimation = new DoubleAnimation
            {
                From = 0,
                To = _textBlockTop.ActualWidth,
                Duration = Duration
            };

            Storyboard.SetTarget(doubleAnimation, _textBlockTop);
            Storyboard.SetTargetProperty(doubleAnimation,
                new PropertyPath("(UIElement.Clip).(EllipseGeometry.Transform).(TranslateTransform.X)"));
            var storyboard = new Storyboard
            {
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true
            };
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }
    }
}