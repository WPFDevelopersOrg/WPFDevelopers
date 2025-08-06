using System;
using System.Windows;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public static class Message
    {
        private static MessageAdorner _messageAdorner;
        private static MessageExt _messageExt;
        private static Position _position = Position.Top;

        static void CreateMessageAdorner(Window owner = null, string message = null, MessageBoxImage type = MessageBoxImage.Information, bool center = false)
        {
            try
            {
                if (_messageAdorner != null)
                {
                    if (_messageAdorner.Position != _position)
                        _messageAdorner.Position = _position;
                    _messageAdorner.Push(message, type, center);
                }
                else
                {
                    if (owner == null)
                        owner = ControlsHelper.GetDefaultWindow();
                    var layer = ControlsHelper.GetAdornerLayer(owner);
                    if (layer == null)
                        throw new Exception("AdornerLayer is not empty, it is recommended to use PushDesktop");
                    _messageAdorner = new MessageAdorner(layer);
                    layer.Add(_messageAdorner);
                    if (_messageAdorner.Position != _position)
                        _messageAdorner.Position = _position;
                    if (!string.IsNullOrWhiteSpace(message))
                        _messageAdorner.Push(message, type, center);
                }
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
        public static void Push(IntPtr intPtr, string message, MessageBoxImage type = MessageBoxImage.Information, bool center = false)
        {
            PushDesktop(message, type, center, intPtr);
        }

        public static void PushDesktop(string message, MessageBoxImage type = MessageBoxImage.Information, bool center = false, IntPtr intPtr = default)
        {
            if (_messageExt == null)
            {
                _messageExt = new MessageExt();
                _messageExt.Closed += delegate { _messageExt = null; };
            }
            if (!_messageExt.IsVisible)
                _messageExt.Show();
            if (_messageExt.Position != _position)
            {
                _messageExt.IsPosition = false;
                _messageExt.Position = _position;
            }
            _messageExt.Push(message, type, center, intPtr);
        }

        public static void SetPosition(Position position = Position.Top)
        {
            if (_position != position)
                _position = position;
        }

        public static void Clear()
        {
            if(_messageAdorner != null)
                _messageAdorner.Clear();
        }
        public static void ClearDesktop()
        {
            if (_messageExt != null)
                _messageExt.Clear();
        }
    }
}
