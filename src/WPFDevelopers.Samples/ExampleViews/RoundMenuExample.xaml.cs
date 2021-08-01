using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// RoundMenuSample.xaml 的交互逻辑
    /// </summary>
    public partial class RoundMenuExample : UserControl
    {
        public RoundMenuExample()
        {
            InitializeComponent();
        }
        Storyboard storyboard;
        ColorAnimation SetAnimButton(Color color, string objName)
        {
            ColorAnimation anim = new ColorAnimation();
            anim.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            anim.To = color;
            Storyboard.SetTargetName(anim, objName);
            Storyboard.SetTargetProperty(anim, new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));
            return anim;
        }


        ColorAnimation SetAnimcirkie(Color color)
        {
            ColorAnimation anim = new ColorAnimation();
            anim.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            anim.To = color;
            Storyboard.SetTargetName(anim, "ColoCirkle");
            Storyboard.SetTargetProperty(anim, new PropertyPath(GradientStop.ColorProperty));
            return anim;

        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            Color background = ((SolidColorBrush)btn.BorderBrush).Color;
            storyboard = new Storyboard();
            storyboard.Children.Add(SetAnimButton(background, btn.Name));
            storyboard.Children.Add(SetAnimcirkie(background));
            storyboard.Begin(this);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            storyboard = new Storyboard();
            storyboard.Children.Add(SetAnimButton(Color.FromRgb(113, 110, 110), btn.Name));
            storyboard.Children.Add(SetAnimcirkie(Color.FromArgb(150, 67, 67, 67)));
            storyboard.Begin(this);

        }
    }
}
