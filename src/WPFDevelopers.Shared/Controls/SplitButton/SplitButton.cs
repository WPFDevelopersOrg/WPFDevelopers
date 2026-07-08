using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace WPFDevelopers.Controls
{
    [ContentProperty(nameof(Children))]
    [TemplatePart(Name = PART_ToggleButtonName, Type = typeof(ToggleButton))]
    public class SplitButton : Button
    {
        private const string PART_ToggleButtonName = "PART_ToggleButton";

        public static readonly DependencyProperty ContextMenuStyleProperty =
            DependencyProperty.Register(nameof(ContextMenuStyle), typeof(Style), typeof(SplitButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(SplitButton),
                new PropertyMetadata(false, OnIsDropDownOpenChanged));

        private ToggleButton _toggleButton;
        private ContextMenu _contextMenu;

        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
        }

        public Style ContextMenuStyle
        {
            get => (Style)GetValue(ContextMenuStyleProperty);
            set => SetValue(ContextMenuStyleProperty, value);
        }

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (SplitButton)d;
            bool isOpen = (bool)e.NewValue;
            if (ctrl._contextMenu != null && ctrl._contextMenu.IsOpen != isOpen)
                ctrl._contextMenu.IsOpen = isOpen;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_toggleButton != null)
            {
                _toggleButton.Checked -= ToggleButton_Checked;
                _toggleButton.Unchecked -= ToggleButton_Unchecked;
            }

            _toggleButton = GetTemplateChild(PART_ToggleButtonName) as ToggleButton;

            if (_toggleButton != null)
            {
                _toggleButton.Checked += ToggleButton_Checked;
                _toggleButton.Unchecked += ToggleButton_Unchecked;
                _toggleButton.Click += ToggleButton_Click;
            }

            BuildContextMenu();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (_contextMenu != null && _contextMenu.HasItems)
                _contextMenu.IsOpen = true;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_contextMenu != null)
                _contextMenu.IsOpen = false;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void BuildContextMenu()
        {
            if (_contextMenu != null)
                return;

            var menu = new ContextMenu();
            menu.PlacementTarget = this;
            menu.Placement = PlacementMode.Bottom;
            menu.HorizontalOffset = 0;
            menu.Closed += (s, e) =>
            {
                IsDropDownOpen = false;
                if (_toggleButton != null)
                    _toggleButton.IsChecked = false;
            };
            if (ContextMenuStyle != null)
                menu.Style = ContextMenuStyle;

            if (ItemsSource != null)
            {
                foreach (var item in (IEnumerable)ItemsSource)
                {
                    var menuItem = new MenuItem { Header = item };
                    menuItem.Click += ContextMenuItem_Click;
                    menu.Items.Add(menuItem);
                }
            }
            else if (_children != null && _children.Count > 0)
            {
                foreach (var child in _children)
                {
                    if (child is MenuItem existingMenuItem)
                    {
                        existingMenuItem.Click += ContextMenuItem_Click;
                        menu.Items.Add(existingMenuItem);
                    }
                    else
                    {
                        string header = child?.ToString();
                        if (child is TextBlock tb)
                            header = tb.Text;
                        else if (child is ContentControl cc && cc.Content is TextBlock innerTb)
                            header = innerTb.Text;
                        else if (child is Decorator decorator && decorator.Child is TextBlock decTb)
                            header = decTb.Text;
                        var menuItem = new MenuItem { Header = header };
                        menuItem.Click += ContextMenuItem_Click;
                        menu.Items.Add(menuItem);
                    }
                }
            }

            _contextMenu = menu;
            ContextMenu = menu;
        }

        private void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                var oldValue = Content;
                Content = menuItem.Header;
                OnSelectionChanged(oldValue, menuItem.Header);
            }
        }

        protected virtual void OnSelectionChanged(object oldValue, object newValue)
        {
            var args = new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, SelectionChangedEvent);
            RaiseEvent(args);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(SplitButton),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(SplitButton),
                new PropertyMetadata(null));

        public static readonly RoutedEvent SelectionChangedEvent =
            EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(SplitButton));

        private ObservableCollection<object> _children;

        public object ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public ObservableCollection<object> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new ObservableCollection<object>();
                }
                return _children;
            }
        }

        public event RoutedPropertyChangedEventHandler<object> SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (SplitButton)d;
            ctrl.BuildContextMenu();
        }
    }
}
