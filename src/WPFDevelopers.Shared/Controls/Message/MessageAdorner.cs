using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class MessageAdorner : Adorner
    {
        private MessageListBox _listBox;
        private UIElement _child;
        private FrameworkElement _adornedElement;
        internal Position Position = Position.Top;
        public MessageAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _adornedElement = adornedElement as FrameworkElement;
        }
        internal void Push(string message, MessageBoxImage type = MessageBoxImage.Information, bool center = false)
        {
            if (_listBox == null)
            {
                _listBox = new MessageListBox();
                Child = _listBox;
            }
            var mItem = new MessageListBoxItem { Content = message, MessageType = type, IsCenter = center };
            _listBox.Items.Insert(0, mItem);
        }
        internal void Clear()
        {
            if(_listBox != null)
                _listBox.Items.Clear();
        }
        public UIElement Child
        {
            get => _child;
            set
            {
                if (value == null)
                {
                    RemoveVisualChild(_child);
                    _child = value;
                    return;
                }
                AddVisualChild(value);
                _child = value;
            }
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return _child != null ? 1 : 0;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_child == null)
                return finalSize;
            double x = 0;
            double y = 0;
            switch (Position)
            {
                case Position.Top:
                    x = (_adornedElement.ActualWidth - _child.DesiredSize.Width) / 2;
                    break;
                case Position.Right:
                    x = _adornedElement.ActualWidth - _child.DesiredSize.Width;
                    break;
                case Position.Bottom:
                    x = (_adornedElement.ActualWidth - _child.DesiredSize.Width) / 2;
                    y = _adornedElement.ActualHeight - _child.DesiredSize.Height;
                    break;
            }
            _child.Arrange(new Rect(new Point(x, y), _child.DesiredSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0 && _child != null) return _child;
            return base.GetVisualChild(index);
        }
    }
}
