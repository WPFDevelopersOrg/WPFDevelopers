using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = BorderTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = RotateTransformTemplateName, Type = typeof(RotateTransform))]
    public class DrawPrize : ListBox
    {
        private const string BorderTemplateName = "PART_Border";
        private const string RotateTransformTemplateName = "PART_ItemsControlAngle";

        private Border _border;
        private RotateTransform _rotateTransform;



        public List<int> ListAngle
        {
            get { return (List<int>)GetValue(ListAngleProperty); }
            set { SetValue(ListAngleProperty, value); }
        }

        public static readonly DependencyProperty ListAngleProperty =
            DependencyProperty.Register("ListAngle", typeof(List<int>), typeof(DrawPrize), new PropertyMetadata());


        private int value;

        static DrawPrize()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawPrize), new FrameworkPropertyMetadata(typeof(DrawPrize)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AlternationCount = 8;
            _border = GetTemplateChild(BorderTemplateName) as Border;
            _rotateTransform = GetTemplateChild(RotateTransformTemplateName) as RotateTransform;
            _border.MouseDown += _border_MouseDown;

        }
       
        private void _border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _border.IsEnabled = false;
            _border.Cursor = Cursors.None;
            var random = new Random();
            var to = random.Next(0, 8);
            var doubleAnimation = new DoubleAnimationUsingKeyFrames();
            value = ListAngle[to];
            
            var splineDoubleKey1 = new SplineDoubleKeyFrame
            {
                KeyTime = TimeSpan.FromSeconds(0),
                Value = value % 360,
            };
            var splineDoubleKey2 = new SplineDoubleKeyFrame
            {
                KeyTime = TimeSpan.FromMilliseconds(1000),
                Value = 360,
            };
            var splineDoubleKey3 = new SplineDoubleKeyFrame
            {
                KeyTime = TimeSpan.FromMilliseconds(2000),
                Value = 1230,
            };
            var splineDoubleKey4 = new SplineDoubleKeyFrame
            {
                KeyTime = TimeSpan.FromMilliseconds(4000),
                Value = value,
                KeySpline = new KeySpline(0, 0, 0, 1)
            };
            doubleAnimation.KeyFrames.Add(splineDoubleKey1);
            doubleAnimation.KeyFrames.Add(splineDoubleKey2);
            doubleAnimation.KeyFrames.Add(splineDoubleKey3);
            doubleAnimation.KeyFrames.Add(splineDoubleKey4);
            doubleAnimation.Completed += (s1,e1)=> { _border.IsEnabled = true; _border.Cursor = Cursors.Hand; };
            _rotateTransform.BeginAnimation(RotateTransform.AngleProperty, doubleAnimation);
        }
    }
}
