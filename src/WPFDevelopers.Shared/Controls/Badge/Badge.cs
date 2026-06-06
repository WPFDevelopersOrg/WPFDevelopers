using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class Badge : Adorner
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(Badge), new PropertyMetadata(string.Empty, OnTextChanged));

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(Badge), new PropertyMetadata(10.0d));

        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.RegisterAttached("IsShow", typeof(bool), typeof(Badge),
                new PropertyMetadata(false, OnIsBadgeChanged));

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(Badge),
                new PropertyMetadata(0.0d, OnOffsetChanged));

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(Badge),
                new PropertyMetadata(0.0d, OnOffsetChanged));

        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Badge badge)
                badge.InvalidateVisual();
        }

        private readonly string _text;
        private readonly double _size;
        private readonly double _horizontalOffset;
        private readonly double _verticalOffset;

        public Badge(UIElement adornedElement, string text = null, double size = 0, double horizontalOffset = 0, double verticalOffset = 0)
            : base(adornedElement)
        {
            _text = text;
            _size = size;
            ToolTip = text;
            _horizontalOffset = horizontalOffset;
            _verticalOffset = verticalOffset;
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static string GetText(UIElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            return (string)element.GetValue(TextProperty);
        }

        public static void SetText(UIElement element, string value)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            element.SetValue(TextProperty, value);
        }

        public static double GetFontSize(UIElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            return (double)element.GetValue(FontSizeProperty);
        }

        public static void SetFontSize(UIElement element, double value)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            element.SetValue(FontSizeProperty, value);
        }

        public static double GetVerticalOffset(UIElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            return (double)element.GetValue(VerticalOffsetProperty);
        }

        public static void SetVerticalOffset(UIElement element, double value)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            element.SetValue(VerticalOffsetProperty, value);
        }

        public static double GetHorizontalOffset(UIElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            return (double)element.GetValue(HorizontalOffsetProperty);
        }

        public static void SetHorizontalOffset(UIElement element, double value)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            element.SetValue(HorizontalOffsetProperty, value);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement parent)
            {
                var isShow = GetIsShow(parent);
                if (!isShow) return;
                var newEventArgs = new DependencyPropertyChangedEventArgs(
                    e.Property,
                    false,
                    isShow);
                OnIsBadgeChanged(d, newEventArgs);
            }
        }

        private static void OnIsBadgeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isShow && d is FrameworkElement parent)
            {
                if (isShow)
                {
                    parent.IsVisibleChanged += Parent_IsVisibleChanged;
                    if (!parent.IsLoaded)
                        parent.Loaded += Parent_Loaded;
                    else
                        CreateBadge(parent);
                }
                else
                {
                    parent.Loaded -= Parent_Loaded;
                    parent.IsVisibleChanged -= Parent_IsVisibleChanged;
                    CreateBadge(parent, true);
                }
            }
        }

        private static void Parent_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isVisible && sender is FrameworkElement parent)
            {
                var isShow = GetIsShow(parent);
                if (isVisible && isShow && !parent.IsLoaded)
                    CreateBadge(parent);
            }
        }

        private static void Parent_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is UIElement element)
                CreateBadge(element);
        }

        private static void CreateBadge(UIElement uiElement, bool isRemove = false)
        {
            if (uiElement == null) return;
            var layer = AdornerLayer.GetAdornerLayer(uiElement);
            if (layer == null) return;
            var adorners = layer.GetAdorners(uiElement);
            if (adorners != null)
            {
                foreach (var item in adorners)
                {
                    if (item is Badge container)
                        layer.Remove(container);
                }
            }
            if (isRemove)
                return;
            var value = GetText(uiElement);
            var size = GetFontSize(uiElement);
            var horizontalOffset = GetHorizontalOffset(uiElement);
            var verticalOffset = GetVerticalOffset(uiElement);
            var badgeAdorner = new Badge(uiElement, value, size, horizontalOffset, verticalOffset);
            layer.Add(badgeAdorner);
        }

        public static bool GetIsShow(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsShowProperty);
        }

        public static void SetIsShow(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShowProperty, value);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            if (adornedElement == null)
                return;

            if (!adornedElement.IsVisible)
                return;

            var text = string.IsNullOrEmpty(_text) ? Text : _text;
            var fontSize = _size > 0 ? _size : FontSize;
            var hOffset = _horizontalOffset != 0 ? _horizontalOffset : GetHorizontalOffset(AdornedElement);
            var vOffset = _verticalOffset != 0 ? _verticalOffset : GetVerticalOffset(AdornedElement);

            if (string.IsNullOrEmpty(text) && !GetIsShow(AdornedElement))
                return;

            var actualWidth = adornedElement.ActualWidth;
            var brush = ThemeManager.Instance.Resources
                .TryFindResource<SolidColorBrush>("WD.DangerBrush") ?? Brushes.Red;

            var center = new Point(actualWidth + hOffset, vOffset);

            if (string.IsNullOrEmpty(text))
            {
                drawingContext.DrawEllipse(brush, null, center, 5, 5);
                return;
            }

            var formattedText = DrawingContextHelper.GetFormattedText(
                text, Brushes.White, FlowDirection.LeftToRight, fontSize);

            var height = formattedText.Height;
            var width = formattedText.Width > 20 ? 20 : formattedText.Width;

            var isSingle = text.Length == 1;
            if (isSingle)
            {
                var max = formattedText.Width > formattedText.Height
                    ? formattedText.Width
                    : formattedText.Height;
                height = max;
                width = max;
            }

            Point startPoint, endPoint;
            if (!isSingle)
            {
                startPoint = new Point(center.X - width / 1.4, center.Y - height / 1.8);
                endPoint = new Point(center.X + width / 1.4 + 6, center.Y + height / 1.8);
            }
            else
            {
                startPoint = new Point(center.X - width / 2, center.Y - height / 2);
                endPoint = new Point(center.X + width / 2, center.Y + height / 2);
            }

            var rect = new Rect(startPoint, endPoint);
            drawingContext.DrawRoundedRectangle(brush, null, rect, 8, 8);

            formattedText.MaxTextWidth = width + 10;
            var centerRect = new Point(
                rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            var textPosition = new Point(
                centerRect.X - formattedText.Width / 2,
                centerRect.Y - formattedText.Height / 2 + vOffset);

            drawingContext.DrawText(formattedText, textPosition);
        }
    }
}
