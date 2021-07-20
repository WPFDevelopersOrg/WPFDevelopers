using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = ItemsControlTemplateName, Type = typeof(ItemsControl))]
    [TemplatePart(Name = EllipseGeometryTemplateName, Type = typeof(EllipseGeometry))]
    [TemplatePart(Name = ToggleButtonTemplateName, Type = typeof(ToggleButton))]
    public class CircularMenu : ListBox
    {
        private const string ItemsControlTemplateName = "PART_ItemsControl";
        private const string EllipseGeometryTemplateName = "PART_EllipseGeometry";
        private const string ToggleButtonTemplateName = "PART_ToggleButton";

        private ItemsControl _itemsControl;
        private EllipseGeometry _ellipseGeometry;
        private ToggleButton _toggleButton;
        static CircularMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularMenu), new FrameworkPropertyMetadata(typeof(CircularMenu)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AlternationCount = 8;
            _itemsControl = GetTemplateChild(ItemsControlTemplateName) as ItemsControl;
            if (_itemsControl != null)
            {
                _itemsControl.MouseLeftButtonDown += _itemsControl_MouseLeftButtonDown;

            }
            _ellipseGeometry = GetTemplateChild(EllipseGeometryTemplateName) as EllipseGeometry;
            if (_ellipseGeometry != null)
                _ellipseGeometry.Center = new Point(this.Width / 2, this.Height / 2);
            _toggleButton = GetTemplateChild(ToggleButtonTemplateName) as ToggleButton;
            if (_toggleButton != null)
            {
                _toggleButton.Checked -= _toggleButton_Checked;
                _toggleButton.Checked += _toggleButton_Checked;
                _toggleButton.Unchecked -= _toggleButton_Unchecked;
                _toggleButton.Unchecked += _toggleButton_Unchecked;
            }
            //AlternationCount = this.Items.Count;
        }

        private void _itemsControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object item = (e.OriginalSource as FrameworkElement).DataContext;
            SelectedItem = item;
        }

        private void _toggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var radiusXAnimation = new DoubleAnimation
            {
                To = Width,
                Duration = new Duration(TimeSpan.FromSeconds(0.4)),
            };
            Storyboard.SetTarget(radiusXAnimation, _itemsControl);
            Storyboard.SetTargetProperty(radiusXAnimation, new PropertyPath("Clip.RadiusX"));
            var radiusYAnimation = new DoubleAnimation
            {
                To = Height,
                Duration = new Duration(TimeSpan.FromSeconds(0.4)),
            };
            Storyboard.SetTarget(radiusYAnimation, _itemsControl);
            Storyboard.SetTargetProperty(radiusYAnimation, new PropertyPath("Clip.RadiusY"));
            var storyboard = new Storyboard();
            storyboard.Children.Add(radiusXAnimation);
            storyboard.Children.Add(radiusYAnimation);
            storyboard.Begin();
        }
        private void _toggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var radiusXAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
            };
            Storyboard.SetTarget(radiusXAnimation, _itemsControl);
            Storyboard.SetTargetProperty(radiusXAnimation, new PropertyPath("Clip.RadiusX"));
            var radiusYAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
            };
            Storyboard.SetTarget(radiusYAnimation, _itemsControl);
            Storyboard.SetTargetProperty(radiusYAnimation, new PropertyPath("Clip.RadiusY"));
            var storyboard = new Storyboard();
            storyboard.Children.Add(radiusXAnimation);
            storyboard.Children.Add(radiusYAnimation);
            storyboard.Begin();
        }

        
    }
}
