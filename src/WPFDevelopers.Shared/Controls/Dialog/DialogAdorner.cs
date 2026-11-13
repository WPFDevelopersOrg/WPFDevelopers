using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    internal sealed class DialogAdorner : Adorner
    {
        private readonly Dialog _host;
        private readonly MaskControl _background;

        internal DialogAdorner(UIElement element, Dialog host) : base(element)
        {
            _host = host;
            _background = new MaskControl(element) { IsHitTestVisible = false };
            AddVisualChild(_background);
            AddVisualChild(host);
            ClipToBounds = true;
            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
            KeyboardNavigation.SetControlTabNavigation(this, KeyboardNavigationMode.Cycle);
        }

        protected override int VisualChildrenCount => 2;

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index > 1)
                throw new ArgumentOutOfRangeException(nameof(index));
            return index == 0 ? (Visual)_background : _host;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = AdornedElement.RenderSize;
            _background.Measure(size);
            var width = double.IsNaN(_host.DialogWidth) ? size.Width : Math.Min(size.Width, _host.DialogWidth);
            var height = double.IsNaN(_host.DialogHeight) ? size.Height : Math.Min(size.Height, _host.DialogHeight);
            _host.Measure(new Size(width, height));
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _background.Arrange(new Rect(finalSize));
            var width = double.IsNaN(_host.DialogWidth) ? Math.Max(240, _host.DesiredSize.Width) : _host.DialogWidth;
            var height = double.IsNaN(_host.DialogHeight) ? Math.Max(120, _host.DesiredSize.Height) : _host.DialogHeight;
            width = Math.Min(width, finalSize.Width);
            height = Math.Min(height, finalSize.Height);
            _host.Arrange(new Rect((finalSize.Width - width) / 2, (finalSize.Height - height) / 2, width, height));
            return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
        }

        internal void Detach()
        {
            RemoveVisualChild(_background);
            RemoveVisualChild(_host);
        }
    }
}
