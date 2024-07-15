using System.Windows;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public static class MessageBox
    {
        public static MessageBoxResult Show(string messageBoxText, Window owner = null, double buttonRadius = 0d)
        {
            var msg = new WDMessageBox(messageBoxText, buttonRadius);
            return GetWindow(msg, owner);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, Window owner = null, double buttonRadius = 0d)
        {
            var msg = new WDMessageBox(messageBoxText, caption, buttonRadius);
            return GetWindow(msg, owner);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, Window owner = null, double buttonRadius = 0d)
        {
            var msg = new WDMessageBox(messageBoxText, caption, button, buttonRadius);
            return GetWindow(msg, owner);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxImage icon, Window owner = null, double buttonRadius = 0d)
        {
            var msg = new WDMessageBox(messageBoxText, caption, icon, buttonRadius);
            return GetWindow(msg, owner);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button,
            MessageBoxImage icon, Window owner = null, double buttonRadius = 0d)
        {
            var msg = new WDMessageBox(messageBoxText, caption, button, icon, buttonRadius);
            return GetWindow(msg, owner);
        }

        private static MessageBoxResult GetWindow(WDMessageBox msg, Window owner = null)
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
                if (win != null && win != msg)
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