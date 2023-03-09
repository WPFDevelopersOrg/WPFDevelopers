using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using WPFDevelopers.Helpers;
using WPFDevelopers.Utilities;

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
                    msg.CreateMask();
                    msg.Owner = win;
                    msg.ShowDialog();
                }
                else
                    msg.Show();
                return msg.Result;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}