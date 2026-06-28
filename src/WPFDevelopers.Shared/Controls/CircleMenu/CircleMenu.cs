using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace WPFDevelopers.Controls
{
    [ContentProperty(nameof(Items))]
    [TemplatePart(Name = EllipseGeometryTemplateName, Type = typeof(EllipseGeometry))]
    [TemplatePart(Name = ItemsPresenterTemplateName, Type = typeof(ItemsPresenter))]
    [TemplatePart(Name = ToggleButtonTemplateName, Type = typeof(ToggleButton))]
    public class CircleMenu : ItemsControl
    {
        private const string EllipseGeometryTemplateName = "PART_EllipseGeometry";
        private const string ItemsPresenterTemplateName = "PART_ItemsPresenter";
        private const string ToggleButtonTemplateName = "PART_ToggleButton";

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(CircleMenu),
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedIndexChanged, CoerceSelectedIndex));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(CircleMenu),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemChanged));

        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register(nameof(ItemClickCommand), typeof(ICommand), typeof(CircleMenu),
                new PropertyMetadata(null));


        public static readonly RoutedEvent ItemClickEvent =
            EventManager.RegisterRoutedEvent(nameof(ItemClick), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(CircleMenu));

        public event RoutedEventHandler ItemClick
        {
            add => AddHandler(ItemClickEvent, value);
            remove => RemoveHandler(ItemClickEvent, value);
        }

        private EllipseGeometry _ellipseGeometry;
        private ItemsPresenter _itemsPresenter;
        private ToggleButton _toggleButton;
        private Storyboard _openStoryboard;
        private Storyboard _closeStoryboard;


        static CircleMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircleMenu),
                new FrameworkPropertyMetadata(typeof(CircleMenu)));
        }

        #region CLR Properties

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public ICommand ItemClickCommand
        {
            get => (ICommand)GetValue(ItemClickCommandProperty);
            set => SetValue(ItemClickCommandProperty, value);
        }

        #endregion

        #region Template

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _ellipseGeometry = GetTemplateChild(EllipseGeometryTemplateName) as EllipseGeometry;
            _itemsPresenter = GetTemplateChild(ItemsPresenterTemplateName) as ItemsPresenter;
            _toggleButton = GetTemplateChild(ToggleButtonTemplateName) as ToggleButton;

            if (_toggleButton != null)
            {
                _toggleButton.Checked -= OnToggleButton_Checked;
                _toggleButton.Checked += OnToggleButton_Checked;
                _toggleButton.Unchecked -= OnToggleButton_Unchecked;
                _toggleButton.Unchecked += OnToggleButton_Unchecked;
            }

            MouseLeftButtonDown += OnCircleMenu_MouseLeftButtonDown;
            UpdateCenter();
            BuildStoryboards();
            UpdateAllAngles();
        }

        #endregion

        protected override DependencyObject GetContainerForItemOverride()
            => new CircleMenuItem();

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (element is CircleMenuItem menuItem)
            {
                if (item is CircleMenuItem sourceItem && sourceItem != menuItem)
                {
                    menuItem.Text = sourceItem.Text;
                    menuItem.Icon = sourceItem.Icon;
                    menuItem.SectorGeometry = sourceItem.SectorGeometry;
                }
                else if (!(item is CircleMenuItem))
                {
                    menuItem.SetBinding(CircleMenuItem.TextProperty, new Binding("Text"));
                    menuItem.SetBinding(CircleMenuItem.IconProperty, new Binding("Icon"));
                }

                menuItem.AlternationIndex = Items.IndexOf(item);
            }
        }


        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            UpdateAllAngles();
            CoerceValue(SelectedIndexProperty);
        }

        private void UpdateAllAngles()
        {
            if (Items.Count == 0) return;

            if (!IsLoaded)
            {
                Dispatcher.BeginInvoke(new Action(UpdateAllAnglesCore), DispatcherPriority.Loaded);
                return;
            }
            UpdateAllAnglesCore();
        }

        private void UpdateAllAnglesCore()
        {
            if (Items.Count == 0) return;

            var step = 360.0 / Items.Count;
            var startAngle = -180;
            var halfStepRad = step / 2 * Math.PI / 180;
            var left = 200 * Math.Sin(halfStepRad) - 20;
            for (int i = 0; i < Items.Count; i++)
            {
                if (ItemContainerGenerator.ContainerFromIndex(i) is CircleMenuItem menuItem)
                {
                    menuItem.Angle = startAngle + i * step;
                    menuItem.AlternationIndex = i;
                    menuItem.SectorGeometry = BuildSectorGeometry(step);
                    menuItem.Padding = new Thickness(left, 60, 0, 0);
                }
            }
        }

        private Geometry BuildSectorGeometry(double stepAngle)
        {
            var rad = stepAngle * Math.PI / 180;
            var radius = 200.0;
            var cx = radius;
            var cy = radius;
            var ex = cx - radius * Math.Cos(rad);
            var ey = cy - radius * Math.Sin(rad);
            return Geometry.Parse($"M {cx},{cy} 0,{cy} A {radius},{radius} 0 0 1 {ex:F1},{ey:F1}z");
        }

        private static object CoerceSelectedIndex(DependencyObject d, object baseValue)
        {
            var menu = (CircleMenu)d;
            var index = (int)baseValue;
            if (index < -1 || index >= menu.Items.Count)
                return -1;
            return index;
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = (CircleMenu)d;
            var index = (int)e.NewValue;
            menu.SelectedItem = index >= 0 && index < menu.Items.Count
                ? menu.Items[index]
                : null;
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = (CircleMenu)d;
            menu.SelectedIndex = e.NewValue != null ? menu.Items.IndexOf(e.NewValue) : -1;
        }



        private void OnCircleMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var source = e.OriginalSource as DependencyObject;
            while (source != null && !(source is CircleMenuItem))
                source = VisualTreeHelper.GetParent(source);

            if (source is CircleMenuItem container)
            {
                var clickedItem = ItemContainerGenerator.ItemFromContainer(container);
                SelectedItem = clickedItem;
                RaiseItemClick(clickedItem);
            }
        }

        private void RaiseItemClick(object clickedItem)
        {
            RaiseEvent(new RoutedEventArgs(ItemClickEvent, clickedItem));
            if (ItemClickCommand is ICommand cmd && cmd.CanExecute(clickedItem))
                cmd.Execute(clickedItem);
        }

        private void UpdateCenter()
        {
            if (_ellipseGeometry != null)
                _ellipseGeometry.Center = new Point(Width / 2, Height / 2);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateCenter();
        }

        private void BuildStoryboards()
        {
            if (_itemsPresenter == null || _ellipseGeometry == null) return;
            var openRx = new DoubleAnimation
            {
                To = Width,
                Duration = new Duration(TimeSpan.FromSeconds(0.4)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(openRx, _itemsPresenter);
            Storyboard.SetTargetProperty(openRx, new PropertyPath("Clip.RadiusX"));

            var openRy = new DoubleAnimation
            {
                To = Height,
                Duration = new Duration(TimeSpan.FromSeconds(0.4)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(openRy, _itemsPresenter);
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
            Storyboard.SetTarget(closeRx, _itemsPresenter);
            Storyboard.SetTargetProperty(closeRx, new PropertyPath("Clip.RadiusX"));

            var closeRy = new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(closeRy, _itemsPresenter);
            Storyboard.SetTargetProperty(closeRy, new PropertyPath("Clip.RadiusY"));

            _closeStoryboard = new Storyboard();
            _closeStoryboard.Children.Add(closeRx);
            _closeStoryboard.Children.Add(closeRy);
        }

        private void OnToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            _openStoryboard?.Begin();
        }

        private void OnToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _closeStoryboard?.Begin();
        }

    }
}
