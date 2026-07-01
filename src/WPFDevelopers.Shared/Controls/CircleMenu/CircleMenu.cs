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

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(CircleMenu),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsExpandedChanged));

        public static readonly DependencyProperty SectorBackgroundProperty =
            DependencyProperty.Register(nameof(SectorBackground), typeof(Brush), typeof(CircleMenu),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SectorDualBackgroundProperty =
            DependencyProperty.Register(nameof(SectorDualBackground), typeof(Brush), typeof(CircleMenu),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register(nameof(SelectedBackground), typeof(Brush), typeof(CircleMenu),
                new PropertyMetadata(null));

        public static readonly DependencyProperty MinItemsProperty =
            DependencyProperty.Register(nameof(MinItems), typeof(int), typeof(CircleMenu),
                new PropertyMetadata(2));


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

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public Brush SectorBackground
        {
            get => (Brush)GetValue(SectorBackgroundProperty);
            set => SetValue(SectorBackgroundProperty, value);
        }

        public Brush SectorDualBackground
        {
            get => (Brush)GetValue(SectorDualBackgroundProperty);
            set => SetValue(SectorDualBackgroundProperty, value);
        }

        public Brush SelectedBackground
        {
            get => (Brush)GetValue(SelectedBackgroundProperty);
            set => SetValue(SelectedBackgroundProperty, value);
        }

        public int MinItems
        {
            get => (int)GetValue(MinItemsProperty);
            set => SetValue(MinItemsProperty, value);
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
                    menuItem.Content = sourceItem.Content;
                    menuItem.SectorGeometry = sourceItem.SectorGeometry;
                }
                else if (!(item is CircleMenuItem))
                {
                    var path = string.IsNullOrEmpty(DisplayMemberPath) ? "Content" : DisplayMemberPath;
                    menuItem.SetBinding(CircleMenuItem.ContentProperty, new Binding(path));
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
            if (Items.Count < MinItems)
            {
                Visibility = Visibility.Collapsed;
                return;
            }
            Visibility = Visibility.Visible;

            var radius = Math.Min(Width, Height) / 2.0;
            if (radius <= 0) radius = 200.0;
            var step = 360.0 / Items.Count;
            var startAngle = -180.0;
            var iconDistance = radius * 0.75;
            var iconHalfSize = radius * 0.1;

            var isOdd = Items.Count % 2 == 1;

            for (int i = 0; i < Items.Count; i++)
            {
                if (ItemContainerGenerator.ContainerFromIndex(i) is CircleMenuItem menuItem)
                {
                    var panelAngle = startAngle + i * step;
                    menuItem.Angle = panelAngle;
                    menuItem.AlternationIndex = i;
                    menuItem.SectorGeometry = BuildSectorGeometry(step, radius);
                    menuItem.RotateCenter = radius;

                    if (i % 2 == 1)
                    {
                        if (SectorDualBackground != null)
                            menuItem.SectorBackground = SectorDualBackground;
                        else if (isOdd && SectorBackground != null)
                            menuItem.SectorBackground = SectorBackground;
                        else
                            menuItem.SetResourceReference(CircleMenuItem.SectorBackgroundProperty, "WD.BackgroundBrush");
                    }
                    else
                    {
                        if (SectorBackground != null)
                            menuItem.SectorBackground = SectorBackground;
                        else
                            menuItem.SetResourceReference(CircleMenuItem.SectorBackgroundProperty, "WD.ChartXAxisBrush");
                    }

                    if (SelectedBackground != null)
                        menuItem.SelectedBackground = SelectedBackground;
                    else
                        menuItem.SetResourceReference(CircleMenuItem.SelectedBackgroundProperty, "WD.PrimaryBrush");
                    menuItem.IsSelected = (i == SelectedIndex);

                    Thickness padding;
                    if (Items.Count == 2)
                    {
                        var left = radius - iconHalfSize;
                        var top = radius - iconDistance - iconHalfSize;
                        padding = new Thickness(left, top, 0, 0);
                    }
                    else
                    {
                        var halfStepRad = step / 2.0 * Math.PI / 180.0;
                        var left = iconDistance * Math.Sin(halfStepRad) - iconHalfSize;
                        var top = iconDistance * Math.Cos(halfStepRad) - iconHalfSize;
                        padding = new Thickness(left, top, 0, 0);
                    }
                    menuItem.Padding = padding;
                }
            }
        }

        private Geometry BuildSectorGeometry(double stepAngle, double radius)
        {
            var rad = stepAngle * Math.PI / 180;
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
            menu.UpdateSelectedItems();
        }

        private void UpdateSelectedItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (ItemContainerGenerator.ContainerFromIndex(i) is CircleMenuItem menuItem)
                    menuItem.IsSelected = (i == SelectedIndex);
            }
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = (CircleMenu)d;
            menu.SelectedIndex = e.NewValue != null ? menu.Items.IndexOf(e.NewValue) : -1;
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = (CircleMenu)d;
            if (menu._toggleButton != null)
                menu._toggleButton.IsChecked = (bool)e.NewValue;
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
            var radius = Math.Min(Width, Height) / 2.0;
            if (_ellipseGeometry != null)
                _ellipseGeometry.Center = new Point(radius, radius);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateCenter();
            UpdateAllAngles();
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
            IsExpanded = true;
            _openStoryboard?.Begin();
        }

        private void OnToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            IsExpanded = false;
            _closeStoryboard?.Begin();
        }

    }
}
