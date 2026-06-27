using System;
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
    public class CircleMenu : ListBox
    {
        private const string ItemsControlTemplateName = "PART_ItemsControl";
        private const string EllipseGeometryTemplateName = "PART_EllipseGeometry";
        private const string ToggleButtonTemplateName = "PART_ToggleButton";
        private EllipseGeometry _ellipseGeometry;

        private ItemsControl _itemsControl;
        private ToggleButton _toggleButton;
        private Storyboard _openStoryboard;
        private Storyboard _closeStoryboard;

        static CircleMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircleMenu),
                new FrameworkPropertyMetadata(typeof(CircleMenu)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AlternationCount = 8;
            _itemsControl = GetTemplateChild(ItemsControlTemplateName) as ItemsControl;
            if (_itemsControl != null)
                _itemsControl.MouseLeftButtonDown += OnItemsControl_MouseLeftButtonDown;
            _ellipseGeometry = GetTemplateChild(EllipseGeometryTemplateName) as EllipseGeometry;
            _toggleButton = GetTemplateChild(ToggleButtonTemplateName) as ToggleButton;
            if (_toggleButton != null)
            {
                _toggleButton.Checked -= OnToggleButton_Checked;
                _toggleButton.Checked += OnToggleButton_Checked;
                _toggleButton.Unchecked -= OnToggleButton_Unchecked;
                _toggleButton.Unchecked += OnToggleButton_Unchecked;
            }
            UpdateCenter();
            BuildStoryboards();
        }

        private void UpdateCenter()
        {
            if (_ellipseGeometry != null)
                _ellipseGeometry.Center = new Point(Width / 2, Height / 2);
        }

        private void BuildStoryboards()
        {
            if (_itemsControl == null) return;

            var openRx = new DoubleAnimation
            {
                To = Width,
                Duration = new Duration(TimeSpan.FromSeconds(0.4)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(openRx, _itemsControl);
            Storyboard.SetTargetProperty(openRx, new PropertyPath("Clip.RadiusX"));

            var openRy = new DoubleAnimation
            {
                To = Height,
                Duration = new Duration(TimeSpan.FromSeconds(0.4)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(openRy, _itemsControl);
            Storyboard.SetTargetProperty(openRy, new PropertyPath("Clip.RadiusY"));

            _openStoryboard = new Storyboard();
            _openStoryboard.Children.Add(openRx);
            _openStoryboard.Children.Add(openRy);

            var closeRx = new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(closeRx, _itemsControl);
            Storyboard.SetTargetProperty(closeRx, new PropertyPath("Clip.RadiusX"));

            var closeRy = new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(closeRy, _itemsControl);
            Storyboard.SetTargetProperty(closeRy, new PropertyPath("Clip.RadiusY"));

            _closeStoryboard = new Storyboard();
            _closeStoryboard.Children.Add(closeRx);
            _closeStoryboard.Children.Add(closeRy);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateCenter();
        }

        private void OnItemsControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = (e.OriginalSource as FrameworkElement).DataContext;
            SelectedItem = item;
        }

        private void OnToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (_openStoryboard == null) return;
            if (_ellipseGeometry != null)
            {
                _ellipseGeometry.RadiusX = Width;
                _ellipseGeometry.RadiusY = Height;
            }
            _openStoryboard.Begin();
        }

        private void OnToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_closeStoryboard == null) return;
            if (_ellipseGeometry != null)
            {
                _ellipseGeometry.RadiusX = 0;
                _ellipseGeometry.RadiusY = 0;
            }
            _closeStoryboard.Begin();
        }
    }
}