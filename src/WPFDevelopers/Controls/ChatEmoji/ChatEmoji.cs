using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = TextBlockTemplateName, Type = typeof(TextBlock))]
    [TemplatePart(Name = WrapPanelLeftTemplateName, Type = typeof(WrapPanel))]
    [TemplatePart(Name = BorderTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = WrapPanelRightTemplateName, Type = typeof(WrapPanel))]
    [TemplatePart(Name = ImageBrushLeftTemplateName, Type = typeof(ImageBrush))]
    [TemplatePart(Name = ImageBrushRightTemplateName, Type = typeof(ImageBrush))]
    public class ChatEmoji : Control
    {
        private const string TextBlockTemplateName = "PART_TextBlock";
        private const string WrapPanelLeftTemplateName = "PART_Left";
        private const string BorderTemplateName = "PART_Border";
        private const string WrapPanelRightTemplateName = "PART_Right";
        private const string ImageBrushLeftTemplateName = "LeftUser";
        private const string ImageBrushRightTemplateName = "RightUser";
        private TextBlock m_textBlock;
        private WrapPanel leftWrapPanel;
        private Border border;
        private WrapPanel rightWrapPanel;
        private ImageBrush leftImageBrush;
        private ImageBrush rightImageBrush;
        private bool m_IgnoreChanges = false;
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        static ChatEmoji()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatEmoji), new FrameworkPropertyMetadata(typeof(ChatEmoji)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_textBlock = GetTemplateChild(TextBlockTemplateName) as TextBlock;
            leftWrapPanel = GetTemplateChild(WrapPanelLeftTemplateName) as WrapPanel;
            border = GetTemplateChild(BorderTemplateName) as Border;
            rightWrapPanel = GetTemplateChild(WrapPanelRightTemplateName) as WrapPanel;
            leftImageBrush = GetTemplateChild(ImageBrushLeftTemplateName) as ImageBrush;
            rightImageBrush = GetTemplateChild(ImageBrushRightTemplateName) as ImageBrush;
            UpdateEmoji();
            UpdateIsRight();
            UpdateRightImageSource();
            UpdateLeftImageSource();
        }

        public static readonly DependencyProperty TextProperty =
          DependencyProperty.Register("Text", typeof(string), typeof(ChatEmoji),
            new UIPropertyMetadata(string.Empty, OnTextChanged));

        static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChatEmoji emoji = (ChatEmoji)d;
            emoji.UpdateEmoji();
        }

        void UpdateEmoji()
        {
            if (m_textBlock == null) return;
            if (!m_IgnoreChanges)
            {
                m_IgnoreChanges = true;
                EmojiHelper.Instance.ParseText(m_textBlock);
                m_IgnoreChanges = false;
            }
        }


        public bool IsRight
        {
            get { return (bool)GetValue(IsRightProperty); }
            set { SetValue(IsRightProperty, value); }
        }

        public static readonly DependencyProperty IsRightProperty =
            DependencyProperty.Register("IsRight", typeof(bool), typeof(ChatEmoji), new UIPropertyMetadata(OnIsRightChanged));

        static void OnIsRightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChatEmoji control = (ChatEmoji)d;
            control.UpdateIsRight();
        }

        void UpdateIsRight()
        {
            if (leftWrapPanel == null
                ||
                border == null
                ||
                rightWrapPanel == null) return;
            if (!IsRight)
            {
                leftWrapPanel.Visibility = Visibility.Visible;
                border.Background = Brushes.White;
                rightWrapPanel.Visibility = Visibility.Collapsed;
            }
        }

        public ImageSource LeftImageSource
        {
            get { return (ImageSource)GetValue(LeftImageSourceProperty); }
            set { SetValue(LeftImageSourceProperty, value); }
        }

        public static readonly DependencyProperty LeftImageSourceProperty =
            DependencyProperty.Register("LeftImageSource", typeof(ImageSource), typeof(ChatEmoji), new UIPropertyMetadata(OnLeftImageSourceChanged));
        private static void OnLeftImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChatEmoji control = (ChatEmoji)d;
            control.UpdateLeftImageSource();
        }

        void UpdateLeftImageSource()
        {
            if (leftImageBrush == null) return;
            leftImageBrush.ImageSource = LeftImageSource;
        }
        public ImageSource RightImageSource
        {
            get { return (ImageSource)GetValue(RightImageSourceProperty); }
            set { SetValue(RightImageSourceProperty, value); }
        }

        public static readonly DependencyProperty RightImageSourceProperty =
            DependencyProperty.Register("RightImageSource", typeof(ImageSource), typeof(ChatEmoji), new UIPropertyMetadata(OnRightImageSourceChanged));

        private static void OnRightImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChatEmoji control = (ChatEmoji)d;
            control.UpdateRightImageSource();
        }

        void UpdateRightImageSource()
        {
            if (rightImageBrush == null) return;
            rightImageBrush.ImageSource = RightImageSource;
        }

    }
}
