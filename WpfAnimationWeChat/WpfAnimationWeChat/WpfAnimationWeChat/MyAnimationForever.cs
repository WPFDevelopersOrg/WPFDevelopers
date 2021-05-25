using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
namespace WpfAnimationWeChat
{
    public class MyAnimationForever : Control
    {
        private static Storyboard MyStory;
        private ObjectAnimationUsingKeyFrames MyAnimation;
        private UIElement animation;

        public static readonly DependencyProperty DurationProperty =
             DependencyProperty.Register("Duration", typeof(TimeSpan),
             typeof(MyAnimationForever), new PropertyMetadata(null));

        /// <summary>
        /// 动画时间
        /// </summary>
        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty IsLitProperty =
             DependencyProperty.Register("IsLit", typeof(bool),
             typeof(MyAnimationForever), new PropertyMetadata(false, new PropertyChangedCallback(OnIsLitChanged)));

        /// <summary>
        /// 是否开始播放
        /// </summary>
        public bool IsLit
        {
            get { return (bool)GetValue(IsLitProperty); }
            set { SetValue(IsLitProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            animation = Template.FindName("animation", this) as UIElement;
            if (animation != null && IsLit)
                Animate(animation);

        }

        private static void OnIsLitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            if (newValue)
            {
                MyAnimationForever c = d as MyAnimationForever;
                if (c != null && c.animation != null)
                {
                    c.Animate(c.animation);
                }
            }
            else
            {
                MyStory.Stop();
            }
        }

        private void Animate(UIElement animation)
        {
            Storyboard.SetTarget(MyAnimation, animation);
            Storyboard.SetTargetProperty(MyAnimation, new PropertyPath(Image.SourceProperty));
            MyStory.Children.Add(MyAnimation);//将动画添加到动画板中
            Console.WriteLine($"一共添加：{MyAnimation.KeyFrames.Count} 个 DiscreteObjectKeyFrame。");
            MyStory.Begin();
        }
        public MyAnimationForever()
        {
            MyStory = new Storyboard();
            MyAnimation = new ObjectAnimationUsingKeyFrames();
            MyAnimation.FillBehavior = FillBehavior.Stop;
            MyAnimation.RepeatBehavior = RepeatBehavior.Forever;

            MyStory.CurrentTimeInvalidated += (s, e) => 
            {
                Clock storyboardClock = (Clock)s;
                Console.WriteLine(storyboardClock.CurrentTime.ToString());
                if (storyboardClock.CurrentTime >= Duration)
                {
                    IsLit = false;
                }
            };
           
            MyAnimation.KeyFrames.Add(
                new DiscreteObjectKeyFrame() 
                {
                    Value = new BitmapImage(new Uri("pack://application:,,,/Images/0.png")), 
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.33))
                 });
            MyAnimation.KeyFrames.Add(
               new DiscreteObjectKeyFrame()
               {
                   Value = new BitmapImage(new Uri("pack://application:,,,/Images/1.png")),
                   KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.66))
               });
            MyAnimation.KeyFrames.Add(
               new DiscreteObjectKeyFrame()
               {
                   Value = new BitmapImage(new Uri("pack://application:,,,/Images/2.png")),
                   KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.99))
               });
           


        }
       
    }
}
