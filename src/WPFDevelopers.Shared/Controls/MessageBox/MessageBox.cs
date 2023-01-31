using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public static class MessageBox
    {
        public static MessageBoxResult Show(string messageBoxText)
        {
            var msg = new WPFMessageBox(messageBoxText);
            return GetWindow(msg);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            var msg = new WPFMessageBox(messageBoxText, caption);
            return GetWindow(msg);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            var msg = new WPFMessageBox(messageBoxText, caption, button);
            return GetWindow(msg);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxImage icon)
        {
            var msg = new WPFMessageBox(messageBoxText, caption, icon);
            return GetWindow(msg);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button,
            MessageBoxImage icon)
        {
            var msg = new WPFMessageBox(messageBoxText, caption, button, icon);
            return GetWindow(msg);
        }

        private static MessageBoxResult GetWindow(WPFMessageBox msg)
        {
            try
            {
                msg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                Window win = null;
                if (Application.Current.Windows.Count > 0)
                    win = Application.Current.Windows.OfType<Window>().FirstOrDefault(o => o.IsActive);
                if (win != null)
                {
                    var layer = new Grid();
                    layer.Children.Add(new Rectangle { Fill = ControlsHelper.WindowForegroundBrush, Opacity = .7 });
                    var original = win.Content as UIElement;
                    win.Content = null;
                    var container = new Grid();
                    container.Children.Add(original);
                    container.Children.Add(layer);
                    win.Content = container;
                    msg.Owner = win;
                    msg.ShowDialog();
                }
                else
                {
                    msg.Show();
                }

                return msg.Result;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}