using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = RunTemplateName, Type = typeof(Run))]
    public partial class AnimationAudio : Control
    {
        const string RunTemplateName = "PART_RunTimeLength";

        private Run _run;
        private TimeSpan _timeSpan;
        private IntPtr _handle;
        private AudioWindow _win = null;

        static string[] mediaExtensions = { ".MP3", ".WAV" };
        /// <summary>
        /// 音频路径
        /// </summary>
        public string AudioPath
        {
            get { return (string)GetValue(AudioPathProperty); }
            set { SetValue(AudioPathProperty, value); }
        }
        public static readonly DependencyProperty AudioPathProperty =
            DependencyProperty.Register("AudioPath", typeof(string), typeof(AnimationAudio), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 是否右侧
        /// </summary>
        public bool IsRight
        {
            get { return (bool)GetValue(IsRightProperty); }
            set { SetValue(IsRightProperty, value); }
        }
        public static readonly DependencyProperty IsRightProperty =
            DependencyProperty.Register("IsRight", typeof(bool), typeof(AnimationAudio), new PropertyMetadata(false));

        public bool IsPlay
        {
            get { return (bool)GetValue(IsPlayProperty); }
            set { SetValue(IsPlayProperty, value); }
        }

        public static readonly DependencyProperty IsPlayProperty =
            DependencyProperty.Register("IsPlay", typeof(bool), typeof(AnimationAudio), new PropertyMetadata(false, new PropertyChangedCallback(OnIsPlayChanged)));

        private static void OnIsPlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            var animationAudio = d as AnimationAudio;
            if(newValue != (bool)e.OldValue)
            {
                if (newValue)
                {
                    animationAudio.Play();
                }
                else
                {
                    AudioPlayer.Stop();
                }
            }
        }

        static AnimationAudio()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimationAudio), new FrameworkPropertyMetadata(typeof(AnimationAudio)));
        }

       
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _run = GetTemplateChild(RunTemplateName) as Run;
            if (string.IsNullOrWhiteSpace(AudioPath)) return;
            if (!File.Exists(AudioPath)) return;
            if (!mediaExtensions.Contains(Path.GetExtension(AudioPath), StringComparer.OrdinalIgnoreCase)) return;
            _timeSpan = AudioPlayer.GetSoundLength(AudioPath);
            if (_timeSpan == TimeSpan.Zero) return;
            _run.Text = $"{_timeSpan.Seconds.ToString()}\"";
            Width = 80;
            if (_timeSpan.Seconds > 5)
            {
                Width += _timeSpan.Seconds;
            }
        }

        private void Play()
        {
            if(_win != null)
            {
                _win.Close();
                _win = null;
            }
            _win = new AudioWindow
            {
                Width = 0,
                Height = 0,
                Left = Int32.MinValue,
                Top = Int32.MinValue,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                ShowActivated = false,
            };
            _win.Show();
            _win.StopDelegateEvent += _win_StopDelegateEvent;
            _handle = new WindowInteropHelper(_win).Handle;
            AudioPlayer.PlaySong(AudioPath, _handle);
        }

       
        private void _win_StopDelegateEvent()
        {
            IsPlay = false;
            _win.Close();
            _win = null;
        }
    }
}
