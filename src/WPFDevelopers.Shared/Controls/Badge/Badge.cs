using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class Badge : Adorner
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Badge), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(Badge), new PropertyMetadata(10.0d));


        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.RegisterAttached("IsShow", typeof(bool), typeof(Badge),
                new PropertyMetadata(false, OnIsBadgeChanged));

        private readonly double _size;

        private readonly string _text;

        public Badge(UIElement adornedElement, string text = null, double size = 0)
            : base(adornedElement)
        {
            _text = text;
            _size = size;
            ToolTip = text;
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public double FontSize
        {
            get => (double) GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static string GetText(UIElement element)
        {
            if (element == null) throw new ArgumentNullException("Text");

            return (string) element.GetValue(TextProperty);
        }

        public static void SetText(UIElement element, string Text)
        {
            if (element == null) throw new ArgumentNullException("Text");

            element.SetValue(TextProperty, Text);
        }

        public static double GetFontSize(UIElement element)
        {
            if (element == null) throw new ArgumentNullException("FontSize");

            return (double) element.GetValue(FontSizeProperty);
        }

        public static void SetFontSize(UIElement element, string Text)
        {
            if (element == null) throw new ArgumentNullException("FontSize");

            element.SetValue(FontSizeProperty, Text);
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
            var badgeAdorner = new Badge(uIElement, value, size);
            layer.Add(badgeAdorner);
        }

        public static bool GetIsShow(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsShowProperty);
        }

        public static void SetIsShow(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShowProperty, value);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            //var margin = adornedElement.Margin;
            //var desiredWidth = adornedElement.DesiredSize.Width - margin.Left - margin.Right;
            var desiredWidth = adornedElement.ActualWidth;
            var brush = new SolidColorBrush((Color) Application.Current.TryFindResource("WD.DangerColor"));
            brush.Freeze();
            var radius = 5.0;
            var center = new Point(desiredWidth, 0);
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
                    centerRect.Y - formattedText.Height / 2);
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