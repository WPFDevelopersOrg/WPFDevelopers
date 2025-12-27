using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFDevelopers.Controls
{
    public class DrawerMenu : ItemsControl
    {
        public static readonly RoutedEvent IsOpenChangedEvent = EventManager.RegisterRoutedEvent("IsOpenChanged", RoutingStrategy.Bubble,
         typeof(RoutedEventHandler), typeof(DrawerMenu));

        public event RoutedEventHandler IsOpenChanged
        {
            add => AddHandler(IsOpenChangedEvent, value);
            remove => RemoveHandler(IsOpenChangedEvent, value);
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
           DependencyProperty.Register("IsOpen", typeof(bool), typeof(DrawerMenu), new PropertyMetadata(false, OnIsOpenChanged));

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrawerMenu ctrl)
            {
                ctrl.RaiseEvent(new RoutedEventArgs(IsOpenChangedEvent));
                if (ctrl.IsOpenCommand?.CanExecute(e.NewValue) == true)
                {
                    ctrl.IsOpenCommand?.Execute(e.NewValue);
                }
            }
        }

        public ICommand IsOpenCommand
        {
            get => (ICommand)GetValue(IsOpenCommandProperty);
            set => SetValue(IsOpenCommandProperty, value);
        }

        public static readonly DependencyProperty IsOpenCommandProperty =
            DependencyProperty.Register("IsOpenCommand", typeof(ICommand), typeof(DrawerMenu),
                new PropertyMetadata(null));
        public object OpenIcon
        {
            get { return (object)GetValue(OpenIconProperty); }
            set { SetValue(OpenIconProperty, value); }
        }

        public static readonly DependencyProperty OpenIconProperty =
            DependencyProperty.Register("OpenIcon", typeof(object), typeof(DrawerMenu), new PropertyMetadata(null));



        public static readonly DependencyProperty ClosedIconProperty =
            DependencyProperty.Register("ClosedIcon", typeof(object), typeof(DrawerMenu),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(DrawerMenu),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged;

        public ICommand SelectionCommand
        {
            get => (ICommand)GetValue(SelectionCommandProperty);
            set => SetValue(SelectionCommandProperty, value);
        }

        public static readonly DependencyProperty SelectionCommandProperty =
            DependencyProperty.Register("SelectionCommand", typeof(ICommand), typeof(DrawerMenu),
                new PropertyMetadata(null));

        public object ClosedIcon
        {
            get => GetValue(ClosedIconProperty);
            set => SetValue(ClosedIconProperty, value);
        }
        static DrawerMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawerMenu),
                new FrameworkPropertyMetadata(typeof(DrawerMenu)));
        }
        protected override bool IsItemItsOwnContainerOverride(object item) => item is DrawerMenuItem;
        protected override DependencyObject GetContainerForItemOverride() => new DrawerMenuItem();

        internal void OnItemSelected(object item)
        {
            var old = SelectedItem;
            SelectedItem = item;
            SelectedItemChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<object>(old, item));
            if (SelectionCommand?.CanExecute(item) == true)
            {
                SelectionCommand.Execute(item);
            }
        }
    }
}