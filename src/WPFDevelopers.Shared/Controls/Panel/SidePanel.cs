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
                new PropertyMetadata(220.0, OnExpandedWidthChanged), IsValidExpandedWidth);

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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _container = GetTemplateChild(ContainerPartName) as Border;
            if (_container != null && !IsExpanded)
                _container.BeginAnimation(WidthProperty, new DoubleAnimation(0, TimeSpan.Zero));
        }

        private static bool IsValidExpandedWidth(object value)
        {
            var width = (double)value;
            return width >= 0 && !double.IsInfinity(width);
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SidePanel)d).AnimateWidth();
        }

        private static void OnExpandedWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (SidePanel)d;
            if (panel.IsExpanded)
                panel.AnimateWidth();
        }

        private void AnimateWidth()
        {
            if (_container == null)
                return;

            _container.BeginAnimation(WidthProperty, new DoubleAnimation
            {
                From = _container.Width,
                To = IsExpanded ? ExpandedWidth : 0,
                Duration = TimeSpan.FromSeconds(0.2)
            });
        }
    }
}
