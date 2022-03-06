using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = TopScaleTransformTemplateName, Type = typeof(ScaleTransform))]
    [TemplatePart(Name = RightScaleTransformTemplateName, Type = typeof(ScaleTransform))]
    [TemplatePart(Name = BottomScaleTransformTemplateName, Type = typeof(ScaleTransform))]
    [TemplatePart(Name = LeftScaleTransformTemplateName, Type = typeof(ScaleTransform))]
    public class EdgeLight:ContentControl
    {
        private const string TopScaleTransformTemplateName = "PART_Top";
        private const string RightScaleTransformTemplateName = "PART_Right";
        private const string BottomScaleTransformTemplateName = "PART_Bottom";
        private const string LeftScaleTransformTemplateName = "PART_Left";

        private ScaleTransform _TopScaleTransform;
        private ScaleTransform _RightScaleTransform;
        private ScaleTransform _BottomScaleTransform;
        private ScaleTransform _LeftScaleTransform;




        public bool IsAnimation
        {
            get { return (bool)GetValue(IsAnimationProperty); }
            set { SetValue(IsAnimationProperty, value); }
        }

        public static readonly DependencyProperty IsAnimationProperty =
            DependencyProperty.Register("IsAnimation", typeof(bool), typeof(EdgeLight), new PropertyMetadata(false));



        public double LineSize
        {
            get { return (double)GetValue(LineSizeProperty); }
            set { SetValue(LineSizeProperty, value); }
        }

        public static readonly DependencyProperty LineSizeProperty =
            DependencyProperty.Register("LineSize", typeof(double), typeof(EdgeLight), new PropertyMetadata(1.0d));



        //private Storyboard storyboard;
        //private const double seconds = 1000;

        static EdgeLight()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EdgeLight), new FrameworkPropertyMetadata(typeof(EdgeLight)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //_TopScaleTransform = GetTemplateChild(TopScaleTransformTemplateName) as ScaleTransform;
            //_RightScaleTransform = GetTemplateChild(RightScaleTransformTemplateName) as ScaleTransform;
            //_BottomScaleTransform = GetTemplateChild(BottomScaleTransformTemplateName) as ScaleTransform;
            //_LeftScaleTransform = GetTemplateChild(LeftScaleTransformTemplateName) as ScaleTransform;
            //storyboard = new Storyboard();
            //var doubleAnimationScaleXTop = new DoubleAnimation();
            //doubleAnimationScaleXTop.To = 1;
            //doubleAnimationScaleXTop.Duration = TimeSpan.FromMilliseconds(seconds);
            //Storyboard.SetTargetName(doubleAnimationScaleXTop, TopScaleTransformTemplateName);
            //Storyboard.SetTargetProperty(doubleAnimationScaleXTop, new PropertyPath(ScaleTransform.ScaleXProperty));
            //storyboard.Children.Add(doubleAnimationScaleXTop);
            //TimeSpan beginTime = TimeSpan.Zero;
           
            //var doubleAnimationScaleYRight = new DoubleAnimation();
            //doubleAnimationScaleYRight.To = 1;
            //doubleAnimationScaleYRight.Duration = TimeSpan.FromMilliseconds(seconds);
            //doubleAnimationScaleYRight.BeginTime = doubleAnimationScaleXTop.Duration.TimeSpan;
            //Storyboard.SetTargetName(doubleAnimationScaleYRight, RightScaleTransformTemplateName);
            //Storyboard.SetTargetProperty(doubleAnimationScaleXTop, new PropertyPath(ScaleTransform.ScaleYProperty));
            //storyboard.Children.Add(doubleAnimationScaleYRight);

            //var doubleAnimationScaleXBottom = new DoubleAnimation();
            //doubleAnimationScaleXBottom.To = 1;
            //doubleAnimationScaleXBottom.Duration = TimeSpan.FromMilliseconds(seconds);
            //doubleAnimationScaleXBottom.BeginTime = doubleAnimationScaleYRight.Duration.TimeSpan;
            //Storyboard.SetTargetName(doubleAnimationScaleXBottom, BottomScaleTransformTemplateName);
            //Storyboard.SetTargetProperty(doubleAnimationScaleXBottom, new PropertyPath(ScaleTransform.ScaleXProperty));
            //storyboard.Children.Add(doubleAnimationScaleXBottom);

            //var doubleAnimationScaleYLeft = new DoubleAnimation();
            //doubleAnimationScaleYLeft.To = 1;
            //doubleAnimationScaleYLeft.Duration = TimeSpan.FromMilliseconds(seconds);
            //doubleAnimationScaleYLeft.BeginTime = doubleAnimationScaleXBottom.Duration.TimeSpan;
            //Storyboard.SetTargetName(doubleAnimationScaleYLeft, LeftScaleTransformTemplateName);
            //Storyboard.SetTargetProperty(doubleAnimationScaleYLeft, new PropertyPath(ScaleTransform.ScaleYProperty));
            //storyboard.Children.Add(doubleAnimationScaleYLeft);
        }

        //public void Start()
        //{
        //    if (storyboard == null) return;
        //    storyboard.Begin(this, true);
        //}

    }
}
