using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = ContainerPartName, Type = typeof(Border))]
    public class SidePanel : ContentControl
    {
        private const string ContainerPartName = "PART_Container";
        private Border _container;

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SidePanel),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsExpandedChanged));

        public static readonly DependencyProperty ExpandedWidthProperty =
            DependencyProperty.Register("ExpandedWidth", typeof(double), typeof(SidePanel),
                new PropertyMetadata(220.0, OnExpandedSizeChanged), IsValidExpandedSize);

        public static readonly DependencyProperty ExpandedHeightProperty =
            DependencyProperty.Register("ExpandedHeight", typeof(double), typeof(SidePanel),
                new PropertyMetadata(220.0, OnExpandedSizeChanged), IsValidExpandedSize);

        public static readonly DependencyProperty DockProperty =
            DependencyProperty.Register("Dock", typeof(Dock), typeof(SidePanel),
                new PropertyMetadata(Dock.Left, OnDockChanged), IsValidDock);

        static SidePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SidePanel), new FrameworkPropertyMetadata(typeof(SidePanel)));
        }

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public double ExpandedWidth
        {
            get => (double)GetValue(ExpandedWidthProperty);
            set => SetValue(ExpandedWidthProperty, value);
        }

        public double ExpandedHeight
        {
            get => (double)GetValue(ExpandedHeightProperty);
            set => SetValue(ExpandedHeightProperty, value);
        }

        public Dock Dock
        {
            get => (Dock)GetValue(DockProperty);
            set => SetValue(DockProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _container = GetTemplateChild(ContainerPartName) as Border;
            if (_container != null && !IsExpanded)
                _container.BeginAnimation(SizeProperty, new DoubleAnimation(0, TimeSpan.Zero));
        }

        private bool IsHorizontal => Dock == Dock.Left || Dock == Dock.Right;

        private DependencyProperty SizeProperty => IsHorizontal ? WidthProperty : HeightProperty;

        private static bool IsValidExpandedSize(object value)
        {
            var size = (double)value;
            return size >= 0 && !double.IsInfinity(size);
        }

        private static bool IsValidDock(object value)
        {
            var dock = (Dock)value;
            return dock == Dock.Left || dock == Dock.Right || dock == Dock.Top || dock == Dock.Bottom;
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SidePanel)d).AnimateSize();
        }

        private static void OnExpandedSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (SidePanel)d;
            if (panel.IsExpanded && (e.Property == ExpandedWidthProperty) == panel.IsHorizontal)
                panel.AnimateSize();
        }

        private static void OnDockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (SidePanel)d;
            if (panel._container == null)
                return;

            panel._container.BeginAnimation(WidthProperty, null);
            panel._container.BeginAnimation(HeightProperty, null);
            if (!panel.IsExpanded)
                panel._container.BeginAnimation(panel.SizeProperty, new DoubleAnimation(0, TimeSpan.Zero));
        }

        private void AnimateSize()
        {
            if (_container == null)
                return;

            _container.BeginAnimation(SizeProperty, new DoubleAnimation
            {
                From = (double)_container.GetValue(SizeProperty),
                To = IsExpanded ? (IsHorizontal ? ExpandedWidth : ExpandedHeight) : 0,
                Duration = TimeSpan.FromSeconds(0.2)
            });
        }
    }
}
