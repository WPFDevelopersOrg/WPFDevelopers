using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class MessageAdorner : Adorner
    {
        private MessageListBox listBox;
        private UIElement _child;
        private FrameworkElement adornedElement;
        public MessageAdorner(UIElement adornedElement) : base(adornedElement)
        {
            this.adornedElement = adornedElement as FrameworkElement;
        }

        public void Push(string message, MessageBoxImage type = MessageBoxImage.Information, bool center = false)
        {
            if (listBox == null)
            {
                listBox = new MessageListBox();
                Child = listBox;
            }
            var mItem = new MessageListBoxItem { Content = message, MessageType = type, IsCenter = center };
            listBox.Items.Insert(0, mItem);
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
            var x = (adornedElement.ActualWidth - _child.DesiredSize.Width) / 2;
            _child.Arrange(new Rect(new Point(x, 0), _child.DesiredSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0 && _child != null) return _child;
            return base.GetVisualChild(index);
        }
    }
}
