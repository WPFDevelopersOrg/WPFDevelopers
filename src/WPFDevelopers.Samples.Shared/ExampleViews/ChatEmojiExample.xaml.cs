using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFDevelopers.Samples.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// ChatEmojiExample.xaml 的交互逻辑
    /// </summary>
    public partial class ChatEmojiExample : UserControl
    {
        public IEnumerable EmojiArray
        {
            get { return (IEnumerable)GetValue(EmojiArrayProperty); }
            set { SetValue(EmojiArrayProperty, value); }
        }

        public static readonly DependencyProperty EmojiArrayProperty =
            DependencyProperty.Register("EmojiArray", typeof(IEnumerable), typeof(ChatEmojiExample), new PropertyMetadata(null));



        public ChatEmojiExample()
        {
            InitializeComponent();
            Loaded += delegate
            {
                var emojiModels = new List<EmojiModel>();

                EmojiHelper.Instance._emojiHeight = 30;
                EmojiHelper.Instance._emojiWidth = 30;

                var m_Emojis = new Dictionary<string, string>();
                var emojiPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emoji");
                var directory = new DirectoryInfo(emojiPath);
                foreach (var item in directory.GetFiles())
                {
                    var _key = $"[{Path.GetFileNameWithoutExtension(item.Name)}]";
                    m_Emojis.Add(_key, item.FullName);
                    emojiModels.Add(new EmojiModel { Name = Path.GetFileNameWithoutExtension(item.Name), Key = _key, Value = item.FullName });
                }
                EmojiHelper.Instance.m_Emojis = m_Emojis;

                EmojiArray = emojiModels;
            };
        }

        private void PART_Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LeftButtonEmoji.IsChecked = false;
            var send = sender as Border;
            LeftInput.Text += send.Tag.ToString();
            LeftInput.Focus();
            LeftInput.SelectionStart = LeftInput.Text.Length;
        }

        private void LeftSend_Click(object sender, RoutedEventArgs e)
        {
            LeftChat();


        }
        void LeftChat()
        {
            var leftText = new ChatEmoji();
            leftText.IsRight = true;
            leftText.RightImageSource = new BitmapImage(new Uri("pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Chat/UserImages/jingtao.png"));
            leftText.Text = LeftInput.Text;
            var leftPara = new Paragraph();
            leftPara.TextAlignment = TextAlignment.Right;
            leftPara.Inlines.Add(leftText);
            _LeftChat.Document.Blocks.Add(leftPara);


            var rightText = new ChatEmoji();
            rightText.IsRight = false;
            rightText.LeftImageSource = new BitmapImage(new Uri("pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Chat/UserImages/jingtao.png"));
            rightText.Text = LeftInput.Text;
            var rightPara = new Paragraph();
            rightPara.TextAlignment = TextAlignment.Left;
            rightPara.Inlines.Add(rightText);
            _RightChat.Document.Blocks.Add(rightPara);

            LeftInput.Text = string.Empty;
            LeftInput.Focus();
        }
        void RightChat()
        {
            var leftText = new ChatEmoji();
            leftText.IsRight = true;
            leftText.RightImageSource = new BitmapImage(new Uri("pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Chat/UserImages/yanjinhua.png"));
            leftText.Text = RightInput.Text;
            var leftPara = new Paragraph();
            leftPara.TextAlignment = TextAlignment.Right;
            leftPara.Inlines.Add(leftText);
            _RightChat.Document.Blocks.Add(leftPara);


            var rightText = new ChatEmoji();
            rightText.IsRight = false;
            rightText.LeftImageSource = new BitmapImage(new Uri("pack://application:,,,/WPFDevelopers.Samples;component/Resources/Images/Chat/UserImages/yanjinhua.png"));
            rightText.Text = RightInput.Text;
            var rightPara = new Paragraph();
            rightPara.TextAlignment = TextAlignment.Left;
            rightPara.Inlines.Add(rightText);
            _LeftChat.Document.Blocks.Add(rightPara);

            RightInput.Text = string.Empty;
            RightInput.Focus();
        }

        private void RightSend_Click(object sender, RoutedEventArgs e)
        {
            RightChat();
        }

        private void PART_Border_RightPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RightButtonEmoji.IsChecked = false;
            var send = sender as Border;
            RightInput.Text += send.Tag.ToString();
            RightInput.Focus();
            RightInput.SelectionStart = RightInput.Text.Length;
        }
    }
    public class EmojiModel
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
