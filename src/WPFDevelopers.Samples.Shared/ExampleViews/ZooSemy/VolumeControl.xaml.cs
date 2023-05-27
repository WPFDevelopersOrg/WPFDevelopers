using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFDevelopers.Samples.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// VolumeControl.xaml 的交互逻辑
    /// </summary>
    public partial class VolumeControl : UserControl
    {
        public static readonly DependencyProperty AngleProperty =
    DependencyProperty.Register("Angle", typeof(double), typeof(VolumeControl), new UIPropertyMetadata());

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
        public IList<ScaleItem> TicksArray
        {
            get { return (IList<ScaleItem>)GetValue(TicksArrayProperty); }
            private set { SetValue(TicksArrayProperty, value); }
        }
        public static readonly DependencyProperty TicksArrayProperty =
            DependencyProperty.Register("TicksArray", typeof(IList<ScaleItem>), typeof(VolumeControl));

        private Point _center;
        private Brush defaultColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#151515"));
        private Brush selectColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF81FB00"));

        public VolumeControl()
        {
            InitializeComponent();
            List<ScaleItem> shortticks = new List<ScaleItem>();
            for (int i = 0; i < 36; i++)
                shortticks.Add(new ScaleItem { Index = i, Background = defaultColor });
            shortticks[0].Background = selectColor;
            this.TicksArray = shortticks;
            _center = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
            this.MouseUp += new MouseButtonEventHandler(OnMouseUp);
            this.MouseMove += new MouseEventHandler(OnMouseMove);
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {

            if (Mouse.Captured == this)
            {
                if (Angle >= 360)
                {
                    Angle = 0;
                    TicksArray.ToList().ForEach(y =>
                    {
                        if (y.Index.Equals(0))
                            y.Background = selectColor;
                        y.Background = defaultColor;
                    });
                }
                var curPoint = e.GetPosition(this);
                var relPoint = new Point(curPoint.X - _center.X, curPoint.Y - _center.Y);
                var angle = Math.Atan2(relPoint.X, relPoint.Y);
                Angle += angle;
                var max = Angle / 10;
                TicksArray.Where(x => x.Index <= max).ToList().ForEach(y =>
                {
                    y.Background = selectColor;
                });
            }
        }
    }
}
