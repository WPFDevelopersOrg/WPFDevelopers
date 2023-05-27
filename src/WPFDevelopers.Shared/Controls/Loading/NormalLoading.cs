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
                , new PropertyMetadata(new DoubleCollection {10, 100}));

        public static NormalLoading Default = new NormalLoading();
        private Ellipse _ellipse;

        private Storyboard _storyboard;

        public NormalLoading()
        {
            Loaded += LoadingNew_Loaded;
            Unloaded += LoadingNew_Unloaded;
        }

        public double StrokeValue
        {
            get => (double) GetValue(StrokeValueProperty);
            set => SetValue(StrokeValueProperty, value);
        }


        public DoubleCollection StrokeArray
        {
            get => (DoubleCollection) GetValue(StrokeArrayProperty);
            set => SetValue(StrokeArrayProperty, value);
        }

        private static void OnStrokeValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NormalLoading).StrokeArray = new DoubleCollection {(double) e.NewValue, 100};
        }

        private void LoadingNew_Loaded(object sender, RoutedEventArgs e)
        {
            _storyboard = new Storyboard();
            _storyboard.RepeatBehavior = RepeatBehavior.Forever;
            var animation = new DoubleAnimation(0, Width + 20, new Duration(TimeSpan.FromSeconds(1.0)));
            _storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath(StrokeValueProperty));
            _storyboard.Begin();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void LoadingNew_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_storyboard != null)
                _storyboard.Stop();
        }
    }
}