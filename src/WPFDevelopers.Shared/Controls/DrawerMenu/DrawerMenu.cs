using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace WPFDevelopers.Controls
{
    public class DrawerMenu : ItemsControl
    {
        public static readonly RoutedEvent IsExpandedChangedEvent = EventManager.RegisterRoutedEvent("IsExpandedChanged", RoutingStrategy.Bubble,
         typeof(RoutedEventHandler), typeof(DrawerMenu));

        public event RoutedEventHandler IsExpandedChanged
        {
            add => AddHandler(IsExpandedChangedEvent, value);
            remove => RemoveHandler(IsExpandedChangedEvent, value);
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }
        public static readonly DependencyProperty IsExpandedProperty =
           DependencyProperty.Register("IsExpanded", typeof(bool), typeof(DrawerMenu), new PropertyMetadata(false, OnIsExpandedChanged));

        public bool ShowExpandButton
        {
            get { return (bool)GetValue(ShowExpandButtonProperty); }
            set { SetValue(ShowExpandButtonProperty, value); }
        }

        public static readonly DependencyProperty ShowExpandButtonProperty =
            DependencyProperty.Register("ShowExpandButton", typeof(bool), typeof(DrawerMenu), new PropertyMetadata(true, OnShowExpandButtonChanged));

        private static void OnShowExpandButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (DrawerMenu)d;
            if (!(bool)e.NewValue)
                ctrl.AnimateHiddenMenuWidth();
            else
                ctrl.BeginAnimation(WidthProperty, null);
        }

        private void AnimateHiddenMenuWidth()
        {
            var currentWidth = Width;
            if (double.IsNaN(currentWidth))
                currentWidth = ActualWidth > 0 ? ActualWidth : 50;

            BeginAnimation(WidthProperty, new DoubleAnimation
            {
                From = currentWidth,
                To = IsExpanded ? 180 : 0,
                Duration = TimeSpan.FromSeconds(0.2)
            });
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrawerMenu ctrl)
            {
                if (!ctrl.ShowExpandButton)
                    ctrl.AnimateHiddenMenuWidth();
                ctrl.RaiseEvent(new RoutedEventArgs(IsExpandedChangedEvent));
                if (ctrl.IsExpandedCommand?.CanExecute(e.NewValue) == true)
                {
                    ctrl.IsExpandedCommand?.Execute(e.NewValue);
                }
            }
        }

        public ICommand IsExpandedCommand
        {
            get => (ICommand)GetValue(IsExpandedCommandProperty);
            set => SetValue(IsExpandedCommandProperty, value);
        }

        public static readonly DependencyProperty IsExpandedCommandProperty =
            DependencyProperty.Register("IsExpandedCommand", typeof(ICommand), typeof(DrawerMenu),
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