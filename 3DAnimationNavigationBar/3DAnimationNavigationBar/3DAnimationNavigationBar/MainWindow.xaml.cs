using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace _3DAnimationNavigationBar
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        DoubleAnimation animation = null;
        private void viewport3DeEmoji_MouseEnter(object sender, MouseEventArgs e)
        {
            if (animation != null) return;

            animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(1.0)),
                From = 0,
                To = 90,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
            };
            animation.Completed += (s, e1) =>
            {
                animation = null;
            };
            axis3dEmoji.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
        }
        private void viewport3DBus_MouseEnter(object sender, MouseEventArgs e)
        {
            if (animation != null) return;

            animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(1.5)),
                From = 0,
                To = 90,
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseInOut },
            };
            animation.Completed += (s, e1) =>
            {
                animation = null;
            };
            axis3dBus.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
        }
    }
}
