using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class Dashboard : ProgressBar
    {
        public Dashboard()
        {
            this.ValueChanged += CircularProgressBar_ValueChanged;
        }

        void CircularProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dashboard bar = sender as Dashboard;
            double currentAngle = bar.Angle;
            double targetAngle = e.NewValue / bar.Maximum * 180;
            Angle = targetAngle;
            if (ScaleArray == null)
                ArrayList();
            var count = Convert.ToInt32(Angle / (180 / ScaleNum));
            ScaleArray.ToList().ForEach(y =>
            {
                y.Background = Brushes.White;
            });

            Brush color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF19DCF0"));
            ScaleArray.Where(x => x.Index <= count).ToList().ForEach(y =>
            {

                y.Background = color;
            });
        }

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(Dashboard), new PropertyMetadata(0.0));

        public IList<ScaleItem> ScaleArray
        {
            get { return (IList<ScaleItem>)GetValue(ScaleArrayProperty); }
            private set { SetValue(ScaleArrayProperty, value); }
        }

        public static readonly DependencyProperty ScaleArrayProperty =
            DependencyProperty.Register("ScaleArray", typeof(IList<ScaleItem>), typeof(Dashboard), new PropertyMetadata(null));

        public int ScaleNum
        {
            get { return (int)GetValue(ScaleNumProperty); }
            set { SetValue(ScaleNumProperty, value); }
        }

        public static readonly DependencyProperty ScaleNumProperty =
            DependencyProperty.Register("ScaleNum", typeof(int), typeof(Dashboard), new PropertyMetadata(18));
        void ArrayList()
        {
            List<ScaleItem> shortticks = new List<ScaleItem>();
            for (int i = 0; i < ScaleNum; i++)
            {
                shortticks.Add(new ScaleItem { Index = i, Background = Brushes.White });
            }
            this.ScaleArray = shortticks;
        }
    }
}
