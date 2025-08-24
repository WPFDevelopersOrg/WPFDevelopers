using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = ToggleButtonTemplateName, Type = typeof(ToggleButton))]
    public class DrawerMenuItem : HeaderedItemsControl
    {
        private const string ToggleButtonTemplateName = "PART_ToggleButton";
        private bool _isOpen;
        private ToggleButton _toggleButton;

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DrawerMenuItem),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(DrawerMenuItem),
                new PropertyMetadata(null));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(DrawerMenuItem),
                new PropertyMetadata(40d));


        static DrawerMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawerMenuItem),
                new FrameworkPropertyMetadata(typeof(DrawerMenuItem)));
        }

        public object Icon
        {
            get => (object)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }


        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(DrawerMenuItem), new PropertyMetadata(false, OnIsExpandedChanged));

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (DrawerMenuItem)d;
            if (!(bool)e.NewValue) return;
            var parentMenu = FindParent<DrawerMenu>(item);
            if (parentMenu == null) return;
            if (IsTopLevelItem(item, parentMenu))
            {
                foreach (var obj in parentMenu.Items)
                {
                    var container = obj as DrawerMenuItem
                                    ?? parentMenu.ItemContainerGenerator.ContainerFromItem(obj) as DrawerMenuItem;

                    if (container != null && container != item)
                    {
                        container.SetCurrentValue(IsExpandedProperty, false);
                    }
                }
            }
        }

        private static bool IsTopLevelItem(DrawerMenuItem item, DrawerMenu parentMenu)
        {
            var parentItem = ItemsControl.ItemsControlFromItemContainer(item);
            return parentItem == parentMenu;
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(DrawerMenuItem),
                new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (DrawerMenuItem)d;
            if (!(bool)e.NewValue) return;
            var parentMenu = FindParent<DrawerMenu>(item);
            if (parentMenu == null) return;
            UnselectOtherItems(parentMenu, item);
            parentMenu.OnItemSelected(item);
        }


        private static void UnselectOtherItems(DependencyObject parent, DrawerMenuItem currentItem)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is DrawerMenuItem navItem && navItem != currentItem)
                {
                    navItem.SetCurrentValue(IsSelectedProperty, false);
                }
                UnselectOtherItems(child, currentItem);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _toggleButton = GetTemplateChild(ToggleButtonTemplateName) as ToggleButton;
            if (_toggleButton != null)
            {
                _toggleButton.Click -= OnToggleButton_Click;
                _toggleButton.Click += OnToggleButton_Click;
            }

            var navMenu = FindParent<DrawerMenu>(this);
            if (navMenu != null)
            {
                _isOpen = navMenu.IsOpen;
                navMenu.AddHandler(DrawerMenu.IsOpenChangedEvent, new RoutedEventHandler(OnDrawerMenuIsOpenChanged));
            }
        }

        private void OnToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!HasItems && !IsSelected)
            {
                SetCurrentValue(IsSelectedProperty, !IsSelected);
            }
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is T))
                parent = VisualTreeHelper.GetParent(parent);
            return parent as T;
        }

        protected override DependencyObject GetContainerForItemOverride() => new DrawerMenuItem();

        protected override bool IsItemItsOwnContainerOverride(object item) => item is DrawerMenuItem;

        private void OnDrawerMenuIsOpenChanged(object sender, RoutedEventArgs e)
        {
            var DrawerMenu = sender as DrawerMenu;
            _isOpen = DrawerMenu.IsOpen;
        }

    }
}