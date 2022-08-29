using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class Dashboard : ProgressBar
    {
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(Dashboard), new PropertyMetadata(0.0));

        public static readonly DependencyProperty ScaleArrayProperty =
            DependencyProperty.Register("ScaleArray", typeof(IList<ScaleItem>), typeof(Dashboard),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ScaleNumProperty =
            DependencyProperty.Register("ScaleNum", typeof(int), typeof(Dashboard), new PropertyMetadata(18));

        public Dashboard()
        {
            ValueChanged += CircularProgressBar_ValueChanged;
        }

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public IList<ScaleItem> ScaleArray
        {
            get => (IList<ScaleItem>)GetValue(ScaleArrayProperty);
            private set => SetValue(ScaleArrayProperty, value);
        }

        public int ScaleNum
        {
            get => (int)GetValue(ScaleNumProperty);
            set => SetValue(ScaleNumProperty, value);
        }

        private void CircularProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var bar = sender as Dashboard;
            var currentAngle = bar.Angle;
            var targetAngle = e.NewValue / bar.Maximum * 180;
            Angle = targetAngle;
            if (ScaleArray == null)
                ArrayList();
            var count = Convert.ToInt32(Angle / (180 / ScaleNum));
            ScaleArray.ToList().ForEach(y => { y.Background = Brushes.White; });

            Brush color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF19DCF0"));
            ScaleArray.Where(x => x.Index <= count).ToList().ForEach(y => { y.Background = color; });
        }

        private void ArrayList()
        {
            var shortticks = new List<ScaleItem>();
            for (var i = 0; i < ScaleNum; i++) shortticks.Add(new ScaleItem { Index = i, Background = Brushes.White });
            ScaleArray = shortticks;
        }
    }
}