using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFDevelopers.Controls
{
    public abstract class ItemsControlBase : ItemsControl
    {
        private static readonly Type _typeofSelf = typeof(ItemsControlBase);

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), _typeofSelf,
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedIndexChanged));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), _typeofSelf,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemChanged));

        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register(nameof(ItemClickCommand), typeof(ICommand), _typeofSelf,
                new PropertyMetadata(null));

        public static readonly RoutedEvent ItemClickEvent =
            EventManager.RegisterRoutedEvent(nameof(ItemClick), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), _typeofSelf);

        protected bool _isSyncing;

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

        public event RoutedEventHandler ItemClick
        {
            add => AddHandler(ItemClickEvent, value);
            remove => RemoveHandler(ItemClickEvent, value);
        }

        protected virtual void OnSelectedIndexChanged(int oldIndex, int newIndex)
        {
        }

        protected virtual void OnSelectedItemChanged(object oldItem, object newItem)
        {
        }

        protected void RaiseItemClick(object clickedItem)
        {
            RaiseEvent(new RoutedEventArgs(ItemClickEvent, clickedItem));
            if (ItemClickCommand is ICommand cmd && cmd.CanExecute(clickedItem))
                cmd.Execute(clickedItem);
        }

        protected void SyncSelectedIndex(int index)
        {
            if (_isSyncing) return;
            if (index < 0 || index >= Items.Count) return;

            _isSyncing = true;
            try
            {
                SelectedIndex = index;
                if (index >= 0 && index < Items.Count)
                    SelectedItem = Items[index];
            }
            finally
            {
                _isSyncing = false;
            }
        }

        protected void SyncSelectedItem(object item)
        {
            if (_isSyncing) return;

            var items = new List<object>();
            foreach (var i in Items)
                items.Add(i);
            var idx = items.IndexOf(item);
            if (idx >= 0)
            {
                _isSyncing = true;
                try
                {
                    SelectedIndex = idx;
                }
                finally
                {
                    _isSyncing = false;
                }
            }
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (ItemsControlBase)d;
            if (ctrl._isSyncing) return;

            var oldIndex = (int)e.OldValue;
            var newIndex = (int)e.NewValue;

            ctrl._isSyncing = true;
            try
            {
                if (newIndex >= 0 && newIndex < ctrl.Items.Count)
                    ctrl.SelectedItem = ctrl.Items[newIndex];
                else
                    ctrl.SelectedItem = null;
            }
            finally
            {
                ctrl._isSyncing = false;
            }

            ctrl.OnSelectedIndexChanged(oldIndex, newIndex);
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (ItemsControlBase)d;
            if (ctrl._isSyncing) return;

            var oldItem = e.OldValue;
            var newItem = e.NewValue;

            var items = new List<object>();
            foreach (var item in ctrl.Items)
                items.Add(item);
            var idx = items.IndexOf(newItem);

            ctrl._isSyncing = true;
            try
            {
                ctrl.SelectedIndex = idx;
            }
            finally
            {
                ctrl._isSyncing = false;
            }

            ctrl.OnSelectedItemChanged(oldItem, newItem);
        }
    }
}
