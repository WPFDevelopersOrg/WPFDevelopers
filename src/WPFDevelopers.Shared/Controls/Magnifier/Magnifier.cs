using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WPFDevelopers.Utilities;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = BorderTemplateName, Type = typeof(Border))]
    [TemplatePart(Name = VisualBrushTemplateName, Type = typeof(VisualBrush))]
    public class Magnifier : Control
    {
        private const string BorderTemplateName = "PART_Border";
        private const string VisualBrushTemplateName = "PART_VisualBrush";

        public static Magnifier Default = new Magnifier();

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Magnifier), new PropertyMetadata(new CornerRadius(0)));

        public static readonly DependencyProperty ParentTargetProperty =
            DependencyProperty.Register("ParentTarget", typeof(FrameworkElement), typeof(Magnifier),
                new PropertyMetadata(default, OnParentTargetChanged));

        public static readonly DependencyProperty AddProperty =
            DependencyProperty.RegisterAttached("Add", typeof(Magnifier), typeof(Magnifier),
                new PropertyMetadata(default, OnAddChanged));

        private AdornerContainer _adornerContainer;
        private Border _border;
        private double _factor = 0.5;
        private VisualBrush _visualBrush = new VisualBrush();

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public FrameworkElement ParentTarget
        {
            get => (FrameworkElement)GetValue(ParentTargetProperty);
            set => SetValue(ParentTargetProperty, value);
        }

        private static void OnParentTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var magnifier = (Magnifier)d;
            magnifier.OnParentTargetChanged((FrameworkElement)e.NewValue);
        }

        private void OnParentTargetChanged(FrameworkElement element)
        {
            if (element == null) return;
            element.Unloaded -= Element_Unloaded;
            element.Unloaded += Element_Unloaded;
            element.MouseEnter -= Element_MouseEnter;
            element.MouseEnter += Element_MouseEnter;
            element.MouseLeave -= Element_MouseLeave;
            element.MouseLeave += Element_MouseLeave;
            element.MouseMove -= Element_MouseMove;
            element.MouseMove += Element_MouseMove;
            element.MouseWheel -= Element_MouseWheel;
            element.MouseWheel += Element_MouseWheel;
        }

        private void Element_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                _factor -= 0.2;
            else
                _factor += 0.2;
            _factor = _factor < 0.2 ? 0.2 : _factor;
            _factor = _factor > 1 * 4 ? 4 : _factor;
            MoveMagnifier();
        }

        private void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_adornerContainer == null) return;
            var layer = AdornerLayer.GetAdornerLayer(ParentTarget);
            if (layer != null) layer.Remove(_adornerContainer);
            if (_adornerContainer != null)
            {
                _adornerContainer.Child = null;
                _adornerContainer = null;
            }
        }

        private void Element_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
                element.Unloaded -= Element_Unloaded;
        }

        private void Element_MouseMove(object sender, MouseEventArgs e)
        {
            MoveMagnifier();
        }

        private void MoveMagnifier()
        {
            if (_border == null) return;
            var length = Width * _factor;
            var radius = length / 2;
            var parentTargetPoint = Mouse.GetPosition(ParentTarget);
            var parentTargetVector = VisualTreeHelper.GetOffset(ParentTarget);
            var size = new Size(length, length);
            var viewboxRect =
                new Rect(
                    new Point(parentTargetPoint.X - radius + parentTargetVector.X,
                        parentTargetPoint.Y - radius + parentTargetVector.Y), size);
            _visualBrush.Viewbox = viewboxRect;
            var adornerPoint = Mouse.GetPosition(_adornerContainer);
            _border.SetValue(Canvas.LeftProperty, adornerPoint.X - Width / 2);
            _border.SetValue(Canvas.TopProperty, adornerPoint.Y - Height / 2);
        }

        private void Element_MouseEnter(object sender, MouseEventArgs e)
        {
            ParentTarget.Cursor = Cursors.Cross;
            if (_adornerContainer == null)
            {
                var layer = AdornerLayer.GetAdornerLayer(ParentTarget);
                if (layer == null) return;
                _adornerContainer = new AdornerContainer(layer)
                {
                    Child = this
                };
                layer.Add(_adornerContainer);
            }
        }


        public static Magnifier GetAdd(DependencyObject obj)
        {
            return (Magnifier)obj.GetValue(AddProperty);
        }

        public static void SetAdd(DependencyObject obj, int value)
        {
            obj.SetValue(AddProperty, value);
        }

        private static void OnAddChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement parent)
            {
                var element = (Magnifier)e.NewValue;
                element.OnAddChanged(parent);
            }
        }

        private void OnAddChanged(FrameworkElement parent)
        {
            ParentTarget = parent;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CornerRadius = new CornerRadius(Width / 2);
            _border = GetTemplateChild(BorderTemplateName) as Border;
            _visualBrush = GetTemplateChild(VisualBrushTemplateName) as VisualBrush ?? new VisualBrush();
        }
    }
}