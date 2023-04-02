using System;
using System.IO;
using System.Windows.Controls;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// AnimationWeChatExample.xaml 的交互逻辑
    /// </summary>
    public partial class AnimationAudioExample : UserControl
    {
        public AnimationAudioExample()
        {
            InitializeComponent();
            AnimationAudioLeft.AudioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,  "Audio", "HelloWPFDevelopes_en.mp3");
            AnimationAudioRight.AudioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,  "Audio", "HelloWPFDevelopes_zh.mp3");
        }

        private void AnimationAudioLeft_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var animationAudio = sender as AnimationAudio;
            var animationAudioList = ElementVisualTreeHelper.FindVisualChild<AnimationAudio>(MyUniformGrid);
            if (animationAudioList == null) return;
            if (!animationAudio.IsPlay)
            {
                animationAudioList.ForEach(h =>
                {
                    if (h.IsPlay && h != animationAudio)
                    {
                        h.IsPlay = false;
                    }
                });
                animationAudio.IsPlay = true;
            }
            else
                animationAudio.IsPlay = false;
        }
    }
}
