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
    [TemplatePart(Name = PathTemplateName, Type = typeof(Path))]
    internal sealed class WPFMessageBox : Window
    {
        private const string TitleTemplateName = "PART_Title";
        private const string CloseButtonTemplateName = "PART_CloseButton";
        private const string MessageTemplateName = "PART_Message";
        private const string ButtonCancelTemplateName = "PART_ButtonCancel";
        private const string ButtonOKTemplateName = "PART_ButtonOK";
        private const string PathTemplateName = "PART_Path";

        private readonly string _messageString;
        private readonly string _titleString;
        private Button _buttonCancel;
        private Button _buttonOK;
        private Visibility _cancelVisibility = Visibility.Collapsed;
        private Button _closeButton;
        private Geometry _geometry;
        private TextBox _message;
        private Visibility _okVisibility;
        private Path _path;
        private SolidColorBrush _solidColorBrush;

        private TextBlock _title;


        static WPFMessageBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPFMessageBox),
                new FrameworkPropertyMetadata(typeof(WPFMessageBox)));
        }

        public WPFMessageBox(string message)
        {
            _messageString = message;
        }

        public WPFMessageBox(string message, string caption)
        {
            _titleString = caption;
            _messageString = message;
        }

        public WPFMessageBox(string message, string caption, MessageBoxButton button)
        {
            _titleString = caption;
            _messageString = message;
            ;
        }

        public WPFMessageBox(string message, string caption, MessageBoxImage image)
        {
            _titleString = caption;
            _messageString = message;
            DisplayImage(image);
        }

        public WPFMessageBox(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            _titleString = caption;
            _messageString = message;
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
                _closeButton.Click += _closeButton_Click;
            _buttonCancel = GetTemplateChild(ButtonCancelTemplateName) as Button;
            if (_buttonCancel != null)
            {
                _buttonCancel.Visibility = _cancelVisibility;
                _buttonCancel.Click += _buttonCancel_Click;
            }

            _buttonOK = GetTemplateChild(ButtonOKTemplateName) as Button;
            if (_buttonOK != null)
            {
                _buttonOK.Visibility = _okVisibility;
                _buttonOK.Click += _buttonOK_Click;
            }

            if (Owner == null)
            {
                BorderThickness = new Thickness(1);
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        private void _buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void _buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        private void _closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DisplayButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OKCancel:
                case MessageBoxButton.YesNo:
                    _cancelVisibility = Visibility.Visible;
                    _okVisibility = Visibility.Visible;
                    break;
                //case MessageBoxButton.YesNoCancel:
                //    break;
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
                    _geometry = (Geometry) Application.Current.TryFindResource("PathWarning");
                    _solidColorBrush = (SolidColorBrush) Application.Current.TryFindResource("WarningSolidColorBrush");
                    break;
                case MessageBoxImage.Error:
                    _geometry = (Geometry) Application.Current.TryFindResource("PathError");
                    _solidColorBrush = (SolidColorBrush) Application.Current.TryFindResource("DangerSolidColorBrush");
                    break;
                case MessageBoxImage.Information:
                    _geometry = (Geometry) Application.Current.TryFindResource("PathWarning");
                    _solidColorBrush = (SolidColorBrush) Application.Current.TryFindResource("SuccessSolidColorBrush");
                    break;
                case MessageBoxImage.Question:
                    _geometry = (Geometry) Application.Current.TryFindResource("PathQuestion");
                    _solidColorBrush = (SolidColorBrush) Application.Current.TryFindResource("NormalSolidColorBrush");
                    break;
            }
        }
    }
}