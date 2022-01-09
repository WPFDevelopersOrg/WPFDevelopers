using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Samples.ExampleViews.NumberCard
{
    /// <summary>
    /// NumberCardExample.xaml 的交互逻辑
    /// </summary>
    public partial class NumberCardExample : UserControl
    {
        private Storyboard storyboard;
        private const double seconds = 1000;
        private double currentSeconds = seconds;
        private int number = 10;
        public NumberCardExample()
        {
            InitializeComponent();
            this.Loaded += NumberCardExample_Loaded;
        }

        private void NumberCardExample_Loaded(object sender, RoutedEventArgs e)
        {
            storyboard = new Storyboard();
            storyboard.FillBehavior = FillBehavior.Stop;
            var num = 1;
            
            for (int i = num; i <= number; i++)
            {
                currentSeconds = seconds * (number - i);
                var numberCard = new NumberCardControl();
                
                numberCard.Number = i.ToString();
                numberCard.Name = $"numberCard{i}";
                var next = number;
                if (!i.Equals(num))
                    next = i - 1;
                else
                    next = 0;
                numberCard.NextNumber = next.ToString();
                this.RegisterName(numberCard.Name, numberCard);
                numberCard.PART_BorderDefault.Name = $"PART_BorderDefault{i}";
                this.RegisterName(numberCard.PART_BorderDefault.Name, numberCard.PART_BorderDefault);

                TimeSpan beginTime = TimeSpan.FromMilliseconds(currentSeconds);

                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.From = 0;
                doubleAnimation.To = 180;
                doubleAnimation.BeginTime = beginTime;
                doubleAnimation.Duration = TimeSpan.FromMilliseconds(seconds);
                numberCard.PART_Viewport3D.Name = $"Viewport3D{i}";
                this.RegisterName(numberCard.PART_Viewport3D.Name, numberCard.PART_Viewport3D);
                Storyboard.SetTargetName(doubleAnimation, numberCard.PART_Viewport3D.Name);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Viewport3D.Children)[1].(ContainerUIElement3D.Transform).(RotateTransform3D.Rotation).(AxisAngleRotation3D.Angle)"));

                storyboard.Children.Add(doubleAnimation);
                
                var animationOpacity = new DoubleAnimation
                {
                    Duration = TimeSpan.FromMilliseconds(0),
                    BeginTime = doubleAnimation.Duration.TimeSpan + beginTime,
                    From = 1.0,
                    To = 0,
                };
                Storyboard.SetTargetName(animationOpacity, numberCard.Name);
                Storyboard.SetTargetProperty(animationOpacity, new PropertyPath(UserControl.OpacityProperty));
                storyboard.Children.Add(animationOpacity);
                MainGrid.Children.Add(numberCard);
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (storyboard != null && storyboard.Children.Count > 0)
            {
                storyboard.Begin(this);
            }
        }
    }
}
