using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = PART_EillipseTemplateName, Type = typeof(Ellipse))]
    public class NormalLoading : Control
    {
        private const string PART_EillipseTemplateName = "PART_Ellipse";

        public static readonly DependencyProperty StrokeValueProperty =
            DependencyProperty.Register("StrokeValue", typeof(double), typeof(NormalLoading)
                , new PropertyMetadata(0.0, OnStrokeValueChanged));

        public static readonly DependencyProperty StrokeArrayProperty =
            DependencyProperty.Register("StrokeArray", typeof(DoubleCollection), typeof(NormalLoading)
                , new PropertyMetadata(new DoubleCollection { 10, 100 }));

        private Storyboard _storyboard;

        public static NormalLoading Default = new NormalLoading();
        private Ellipse _ellipse;
        public NormalLoading()
        {
            Loaded += LoadingNew_Loaded;
            Unloaded += LoadingNew_Unloaded;
        }

        public double StrokeValue
        {
            get => (double)GetValue(StrokeValueProperty);
            set => SetValue(StrokeValueProperty, value);
        }


        public DoubleCollection StrokeArray
        {
            get => (DoubleCollection)GetValue(StrokeArrayProperty);
            set => SetValue(StrokeArrayProperty, value);
        }

        private static void OnStrokeValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NormalLoading).StrokeArray = new DoubleCollection { (double)e.NewValue, 100 };
        }

        private void LoadingNew_Loaded(object sender, RoutedEventArgs e)
        {
            _storyboard = new Storyboard();
            _storyboard.RepeatBehavior = RepeatBehavior.Forever;
            var animation = new DoubleAnimation(0, Width + 20, new Duration(TimeSpan.FromSeconds(1.0)));
            //animation.EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut };
            //animation.AutoReverse = true;
            _storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath(StrokeValueProperty));
            _storyboard.Begin();


            //_storyboard = new Storyboard();
            //_storyboard.RepeatBehavior = RepeatBehavior.Forever;
            //var doubleAnimation1 = new DoubleAnimation
            //{
            //    Duration = TimeSpan.FromMilliseconds(1500),
            //    From = 55,
            //    //To = 24 //W,H = 25
            //    To = 0 // W,H = 40
            //};
            //Storyboard.SetTargetProperty(doubleAnimation1, new PropertyPath("StrokeDashOffset"));
            //Storyboard.SetTarget(doubleAnimation1, _ellipse);
            //_storyboard.Children.Add(doubleAnimation1);
            //var doubleAnimation2 = new DoubleAnimation
            //{
            //    Duration = TimeSpan.FromMilliseconds(1500),
            //    To = 360
            //};
            //Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath("(Ellipse.RenderTransform).(RotateTransform.Angle)"));
            //Storyboard.SetTarget(doubleAnimation2, _ellipse);
            //_storyboard.Children.Add(doubleAnimation2);
            //_storyboard.Begin();

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //_ellipse = GetTemplateChild(PART_EillipseTemplateName) as Ellipse;
        }
        private void LoadingNew_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_storyboard != null)
                _storyboard.Stop();
        }
    }
}