using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public static class Message
    {
        private static readonly ConcurrentDictionary<Window, MessageAdorner> _windowAdorners = new ConcurrentDictionary<Window, MessageAdorner>();
        private static MessageExt _messageExt;
        private static Position _position = Position.Top;

        static void CreateMessageAdorner(Window owner = null, string message = null, MessageBoxImage type = MessageBoxImage.Information, bool center = false)
        {
            try
            {
                if (owner == null)
                    owner = ControlsHelper.GetDefaultWindow();
                MessageAdorner messageAdorner = null;
                if (!_windowAdorners.TryGetValue(owner, out messageAdorner))
                {
                    var layer = ControlsHelper.GetAdornerLayer(owner);
                    if (layer == null)
                        throw new Exception("AdornerLayer is not found, it is recommended to use PushDesktop");
                    messageAdorner = new MessageAdorner(layer);
                    layer.Add(messageAdorner);
                    owner.Closed -= OnOwner_Closed;
                    owner.Closed += OnOwner_Closed;
                    _windowAdorners[owner] = messageAdorner;
                }
                if (messageAdorner.Position != _position)
                    messageAdorner.Position = _position;

                if (!string.IsNullOrWhiteSpace(message))
                    messageAdorner.Push(message, type, center);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void OnOwner_Closed(object sender, EventArgs e)
        {
            var owner = sender as Window;
            if (owner != null)
            {
                MessageAdorner adorner;
                if (_windowAdorners.TryRemove(owner, out adorner))
                {
                    adorner.Clear();
                }
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

        public static void Clear(Window owner = null)
        {
            if (owner == null)
                owner = ControlsHelper.GetDefaultWindow();

            if (owner != null && _windowAdorners.ContainsKey(owner))
                _windowAdorners[owner].Clear();
        }
        public static void ClearDesktop()
        {
            if (_messageExt != null)
                _messageExt.Clear();
        }
    }
}
