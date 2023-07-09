using System;
using System.Linq;
using System.Windows;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public static class Message
    {
        private static MessageAdorner messageAdorner;

        static void CreateMessageAdorner(Window owner = null, string message = null, MessageBoxImage type = MessageBoxImage.Information, bool center = false)
        {
            try
            {
                if (messageAdorner != null)
                {
                    messageAdorner.Push(message, type, center);
                    return;
                }
                if (owner == null)
                    owner = ControlsHelper.GetDefaultWindow();
                var layer = ControlsHelper.GetAdornerLayer(owner);
                if (layer == null)
                    throw new Exception("not AdornerLayer is null");
                messageAdorner = new MessageAdorner(layer);
                layer.Add(messageAdorner);
                messageAdorner.Push(message, type, center);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void Push(this Window owner, string message, MessageBoxImage type = MessageBoxImage.Information, bool center = false)
        {
            CreateMessageAdorner(owner, message, type, center);
        }
        public static void Push(string message, MessageBoxImage type = MessageBoxImage.Information, bool center = false)
        {
            CreateMessageAdorner(message: message, type: type, center: center);
        }
    }
}
