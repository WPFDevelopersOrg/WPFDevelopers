using System;
using System.Windows;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public sealed class MessageExt : Window
    {
        private MessageListBox _listBox;
        internal bool IsPosition;
        internal Position Position;

        public MessageExt()
        {
            Resources = ThemeManager.Instance.Resources;
            SizeToContent = SizeToContent.Width;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            ShowActivated = false;
            Background = Brushes.Transparent;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Topmost = true;
            if (_listBox == null)
            {
                _listBox = new MessageListBox();
                _listBox.SizeChanged -= ListBox_SizeChanged;
                _listBox.SizeChanged += ListBox_SizeChanged;
                Content = _listBox;
            }
           
        }
        
        internal void Push(string message, MessageBoxImage type = MessageBoxImage.Information, bool center = false)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            var item = new MessageListBoxItem { Content = message, MessageType = type, IsCenter = center };
            _listBox.Items.Insert(0, item);
            if (!IsPosition || Position == Position.Bottom)
            {
                double x = 0;
                double y = 0;
                switch (Position)
                {
                    case Position.Top:
                        x = (desktopWorkingArea.Right - item.Width) / 2;
                        break;
                    case Position.Right:
                        x = desktopWorkingArea.Right - (item.Width + item.Margin.Right + item.Padding.Right);
                        break;
                    case Position.Bottom:
                        x = (desktopWorkingArea.Right - item.Width) / 2;
                        _listBox.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        var controlHeight = _listBox.DesiredSize.Height;
                        y = desktopWorkingArea.Height - controlHeight;
                        break;
                }
                Height = desktopWorkingArea.Height;
                Left = x;
                Top = y;
                IsPosition = true;
            }
        }
        internal void Clear() 
        {
            if(_listBox!=null)
                _listBox.Items.Clear();
            Close();
        }
        private void ListBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(_listBox.ActualHeight <= 10)
                Close();
        }
    }
}
