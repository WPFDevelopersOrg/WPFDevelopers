using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using WPFDevelopers.Helpers;

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

        private readonly double _size;

        private readonly string _text;

        private readonly double _verticalOffset;
        private readonly double _horizontalOffset;

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
            if (element == null) throw new ArgumentNullException("Text");

            return (string)element.GetValue(TextProperty);
        }

        public static void SetText(UIElement element, string Text)
        {
            if (element == null) throw new ArgumentNullException("Text");

            element.SetValue(TextProperty, Text);
        }

        public static double GetFontSize(UIElement element)
        {
            if (element == null) throw new ArgumentNullException("FontSize");

            return (double)element.GetValue(FontSizeProperty);
        }

        public static void SetFontSize(UIElement element, string Text)
        {
            if (element == null) throw new ArgumentNullException("FontSize");

            element.SetValue(FontSizeProperty, Text);
        }

        public static double GetVerticalOffset(UIElement element)
        {
            if (element == null) throw new ArgumentNullException("VerticalOffset");

            return (double)element.GetValue(VerticalOffsetProperty);
        }

        public static void SetVerticalOffset(UIElement element, string verticalOffset)
        {
            if (element == null) throw new ArgumentNullException("VerticalOffset");

            element.SetValue(VerticalOffsetProperty, verticalOffset);
        }

        public static double GetHorizontalOffset(UIElement element)
        {
            if (element == null) throw new ArgumentNullException("HorizontalOffset");

            return (double)element.GetValue(HorizontalOffsetProperty);
        }

        public static void SetHorizontalOffset(UIElement element, string horizontalOffset)
        {
            if (element == null) throw new ArgumentNullException("HorizontalOffset");

            element.SetValue(HorizontalOffsetProperty, horizontalOffset);
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

        private static void CreateBadge(UIElement uIElement, bool isRemove = false)
        {
            var layer = AdornerLayer.GetAdornerLayer(uIElement);
            if (layer == null) return;
            if (isRemove && uIElement != null)
            {
                var adorners = layer.GetAdorners(uIElement);
                if (adorners != null)
                    foreach (var item in adorners)
                        if (item is Badge container)
                            layer.Remove(container);
                return;
            }

            var value = GetText(uIElement);
            var size = GetFontSize(uIElement);
            var horizontalOffset = GetHorizontalOffset(uIElement);
            var verticalOffset = GetVerticalOffset(uIElement);
            var badgeAdorner = new Badge(uIElement, value, size, horizontalOffset, verticalOffset);
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
            var desiredWidth = adornedElement.ActualWidth;
            var brush = new SolidColorBrush((Color)Application.Current?.TryFindResource("WD.DangerColor"));
            brush.Freeze();
            var radius = 5.0;
            var center = new Point(desiredWidth + _horizontalOffset, _verticalOffset);
            FormattedText formattedText = null;
            if (!string.IsNullOrEmpty(_text))
                formattedText = DrawingContextHelper.GetFormattedText(
                    _text,
                    Brushes.White,
                    FlowDirection.LeftToRight,
                    _size);
            var pen = new Pen(Brushes.White, .3);
            pen.Freeze();
            drawingContext.PushTransform(new MatrixTransform(Matrix.Identity));
            if (formattedText != null)
            {
                var height = formattedText.Height;
                var width = formattedText.Width > 20 ? 20 : formattedText.Width;
                var isSingle = false;
                if (_text.Length == 1)
                {
                    var max = formattedText.Width > formattedText.Height ? formattedText.Width : formattedText.Height;
                    height = max;
                    width = max;
                    isSingle = true;
                }

                var startPoint = new Point(0, 0);
                var endPoint = new Point(0, 0);
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
                drawingContext.DrawRoundedRectangle(brush, pen, rect, 8, 8);
                formattedText.MaxTextWidth = width + 10;
                var centerRect = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                var textPosition = new Point(centerRect.X - formattedText.Width / 2,
                    centerRect.Y - formattedText.Height / 2 + _verticalOffset);
                drawingContext.DrawText(formattedText, textPosition);
            }
            else
            {
                drawingContext.DrawEllipse(brush, pen, center, radius, radius);
            }

            drawingContext.Pop();
            RenderOptions.SetEdgeMode(this, EdgeMode.Unspecified);
        }
    }
}