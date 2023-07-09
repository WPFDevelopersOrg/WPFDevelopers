using System.Linq;
using System.Windows;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public static class MessageBox
    {
        public static MessageBoxResult Show(string messageBoxText,Window owner = null)
        {
            var msg = new WPFMessageBox(messageBoxText);
            return GetWindow(msg, owner);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, Window owner = null)
        {
            var msg = new WPFMessageBox(messageBoxText, caption);
            return GetWindow(msg, owner);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, Window owner = null)
        {
            var msg = new WPFMessageBox(messageBoxText, caption, button);
            return GetWindow(msg, owner);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxImage icon, Window owner = null)
        {
            var msg = new WPFMessageBox(messageBoxText, caption, icon);
            return GetWindow(msg, owner);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button,
            MessageBoxImage icon, Window owner = null)
        {
            var msg = new WPFMessageBox(messageBoxText, caption, button, icon);
            return GetWindow(msg, owner);
        }

        private static MessageBoxResult GetWindow(WPFMessageBox msg, Window owner = null)
        {
            msg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (owner != null)
            {
                msg.CreateMask();
                msg.Owner = owner;
                msg.ShowDialog();
            }
            else
            {
                var win = ControlsHelper.GetDefaultWindow();
                if (win != null)
                {
                    if (win.WindowState == WindowState.Minimized)
                        msg.BorderThickness = new Thickness(1);
                    msg.CreateMask();
                    msg.Owner = win;
                    msg.ShowDialog();
                }
                else
                {
                    msg.BorderThickness = new Thickness(1);
                    msg.Show();
                }
            }
            return msg.Result;
        }
    }
}