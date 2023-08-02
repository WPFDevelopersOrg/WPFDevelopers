using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFDevelopers.Helpers
{
    public static class ListBoxItemExtensions
    {
        public static readonly DependencyProperty AutoRemoveOnOpacityZeroProperty =
        DependencyProperty.RegisterAttached("AutoRemoveOnOpacityZero", typeof(bool), typeof(ListBoxItemExtensions), new PropertyMetadata(false, OnAutoRemoveOnOpacityZeroChanged));

        public static bool GetAutoRemoveOnOpacityZero(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoRemoveOnOpacityZeroProperty);
        }

        public static void SetAutoRemoveOnOpacityZero(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoRemoveOnOpacityZeroProperty, value);
        }

        private static void OnAutoRemoveOnOpacityZeroChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ListBoxItem item = obj as ListBoxItem;
            if (item != null)
            {
                if ((bool)e.NewValue)
                    item.Loaded += Item_Loaded;
                else
                    item.Loaded -= Item_Loaded;
            }
        }

        private static void Item_Loaded(object sender, RoutedEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item != null)
            {
                var binding = new Binding("Opacity");
                binding.Source = item;
                binding.Mode = BindingMode.OneWay;

                var dpd = DependencyPropertyDescriptor.FromProperty(UIElement.OpacityProperty, typeof(UIElement));
                dpd.AddValueChanged(item, (s, args) =>
                {
                    if (item.Opacity < 0.1)
                    {
                        var parent = ItemsControl.ItemsControlFromItemContainer(item);
                        if (parent != null)
                        {
                            object selectedItem = parent.ItemContainerGenerator.ItemFromContainer(item);
                            parent.Items.Remove(selectedItem);
                            parent.Items.Refresh();
                        }
                    }
                });
            }
        }
    }
}
