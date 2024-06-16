#if NET40
using Microsoft.Windows.Shell;
#else
using System.Windows.Shell;
# endif
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = TitleTemplateName, Type = typeof(TextBlock))]
    [TemplatePart(Name = CloseButtonTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = MessageTemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = ButtonCancelTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = ButtonCancelTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = ButtonYesTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = ButtonNoTemplateName, Type = typeof(Button))]
    [TemplatePart(Name = PathTemplateName, Type = typeof(Path))]
    internal sealed class WDMessageBox : Window
    {
        private const string TitleTemplateName = "PART_Title";
        private const string CloseButtonTemplateName = "PART_CloseButton";
        private const string MessageTemplateName = "PART_Message";
        private const string ButtonCancelTemplateName = "PART_ButtonCancel";
        private const string ButtonOKTemplateName = "PART_ButtonOK";
        private const string ButtonYesTemplateName = "PART_ButtonYes";
        private const string ButtonNoTemplateName = "PART_ButtonNo";
        private const string PathTemplateName = "PART_Path";

        private readonly string _messageString;
        private readonly string _titleString;
        private Button _buttonCancel;
        private Button _buttonOK;
        private Button _buttonYes;
        private Button _buttonNo;
        private Visibility _cancelVisibility = Visibility.Collapsed;
        private Visibility _yesVisibility = Visibility.Collapsed;
        private Visibility _noVisibility = Visibility.Collapsed;
        private Button _closeButton;
        private Geometry _geometry;
        private TextBox _message;
        private Visibility _okVisibility;
        private Path _path;
        private SolidColorBrush _solidColorBrush;

        private TextBlock _title;


        public CornerRadius ButtonCornerRadius
        {
            get { return (CornerRadius)GetValue(ButtonRadiusProperty); }
            set { SetValue(ButtonRadiusProperty, value); }
        }

        public static readonly DependencyProperty ButtonRadiusProperty =
            DependencyProperty.Register("ButtonCornerRadius", typeof(CornerRadius), typeof(WDMessageBox), new PropertyMetadata(null));


        static WDMessageBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WDMessageBox),
                new FrameworkPropertyMetadata(typeof(WDMessageBox)));
        }

        public WDMessageBox(string message, double buttonRadius = 0d)
        {
            _messageString = message;
            ButtonCornerRadius = new CornerRadius(buttonRadius);
        }

        public WDMessageBox(string message, string caption, double buttonRadius = 0d)
        {
            _titleString = caption;
            _messageString = message;
            ButtonCornerRadius = new CornerRadius(buttonRadius);
        }

        public WDMessageBox(string message, string caption, MessageBoxButton button, double buttonRadius = 0d)
        {
            _titleString = caption;
            _messageString = message;
            ButtonCornerRadius = new CornerRadius(buttonRadius);
        }

        public WDMessageBox(string message, string caption, MessageBoxImage image, double buttonRadius = 0d)
        {
            _titleString = caption;
            _messageString = message;
            ButtonCornerRadius = new CornerRadius(buttonRadius);
            DisplayImage(image);
        }

        public WDMessageBox(string message, string caption, MessageBoxButton button, MessageBoxImage image, double buttonRadius = 0d)
        {
            _titleString = caption;
            _messageString = message;
            ButtonCornerRadius = new CornerRadius(buttonRadius);
            DisplayImage(image);
            DisplayButtons(button);
        }

        public MessageBoxResult Result { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _title = GetTemplateChild(TitleTemplateName) as TextBlock;
            _message = GetTemplateChild(MessageTemplateName) as TextBox;

            if (_title == null || _message == null)
                throw new InvalidOperationException("the title or message control is null!");

            _title.Text = _titleString;
            _message.Text = _messageString;
            _path = GetTemplateChild(PathTemplateName) as Path;
            if (_path != null)
            {
                _path.Data = _geometry;
                _path.Fill = _solidColorBrush;
            }

            _closeButton = GetTemplateChild(CloseButtonTemplateName) as Button;
            if (_closeButton != null)
                _closeButton.Click += CloseButton_Click;
            _buttonCancel = GetTemplateChild(ButtonCancelTemplateName) as Button;
            if (_buttonCancel != null)
            {
                _buttonCancel.Visibility = _cancelVisibility;
                _buttonCancel.Click += ButtonCancel_Click;
            }

            _buttonOK = GetTemplateChild(ButtonOKTemplateName) as Button;
            if (_buttonOK != null)
            {
                _buttonOK.Visibility = _okVisibility;
                _buttonOK.Click += ButtonOK_Click;
            }

            _buttonYes = GetTemplateChild(ButtonYesTemplateName) as Button;
            if (_buttonYes != null)
            {
                _buttonYes.Visibility = _yesVisibility;
                _buttonYes.Click += ButtonYes_Click;
            }

            _buttonNo = GetTemplateChild(ButtonNoTemplateName) as Button;
            if (_buttonNo != null)
            {
                _buttonNo.Visibility = _noVisibility;
                _buttonNo.Click += ButtonNo_Click;
            }
            if (Owner == null)
            {
                BorderThickness = new Thickness(1);
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

#if NET40
            var chrome = new WindowChrome
            {
                CaptionHeight = 40,
                GlassFrameThickness = new Thickness(1),
            };
            WindowChrome.SetIsHitTestVisibleInChrome(_closeButton, true);
            WindowChrome.SetWindowChrome(this, chrome);
#else
            var chrome = new WindowChrome
            {
                CaptionHeight = 40,
                GlassFrameThickness = new Thickness(1),
                UseAeroCaptionButtons = false
            };
             WindowChrome.SetIsHitTestVisibleInChrome(_closeButton, true);
             WindowChrome.SetWindowChrome(this, chrome);
#endif
        }

        private void ButtonNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }

        private void ButtonYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DisplayButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OKCancel:
                    _cancelVisibility = Visibility.Visible;
                    _okVisibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    _yesVisibility = Visibility.Visible;
                    _noVisibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    _yesVisibility = Visibility.Visible;
                    _noVisibility = Visibility.Visible;
                    _cancelVisibility = Visibility.Visible;
                    break;
                default:
                    _okVisibility = Visibility.Visible;
                    break;
            }
        }

        private void DisplayImage(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Warning:
                    _geometry = (Geometry) Application.Current.TryFindResource("WD.WarningGeometry");
                    _solidColorBrush =
                        (SolidColorBrush) Application.Current.TryFindResource("WD.WarningSolidColorBrush");
                    break;
                case MessageBoxImage.Error:
                    _geometry = (Geometry) Application.Current.TryFindResource("WD.ErrorGeometry");
                    _solidColorBrush =
                        (SolidColorBrush) Application.Current.TryFindResource("WD.DangerSolidColorBrush");
                    break;
                case MessageBoxImage.Information:
                    _geometry = (Geometry) Application.Current.TryFindResource("WD.WarningGeometry");
                    _solidColorBrush =
                        (SolidColorBrush) Application.Current.TryFindResource("WD.SuccessSolidColorBrush");
                    break;
                case MessageBoxImage.Question:
                    _geometry = (Geometry) Application.Current.TryFindResource("WD.QuestionGeometry");
                    _solidColorBrush =
                        (SolidColorBrush) Application.Current.TryFindResource("WD.NormalSolidColorBrush");
                    break;
            }
        }
    }
}