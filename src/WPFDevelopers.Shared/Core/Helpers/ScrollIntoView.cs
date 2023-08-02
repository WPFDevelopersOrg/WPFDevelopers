using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Helpers
{
    public class ScrollIntoView
    {
        public static readonly DependencyProperty IsPositionProperty = DependencyProperty.RegisterAttached(
          "IsPosition",
          typeof(object),
          typeof(ScrollIntoView),
          new PropertyMetadata(default(object), OnIsPositionChanged));

        public static object GetIsPosition(DependencyObject target)
        {
            return (object)target.GetValue(IsPositionProperty);
        }

        public static void SetIsPosition(DependencyObject target, object value)
        {
            target.SetValue(IsPositionProperty, value);
        }

        static void OnIsPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var type = sender.GetType();
            switch (type)
            {
                case Type _ when type == typeof(DataGrid):
                    var dataGrid = (DataGrid)sender;
                    if (dataGrid == null)
                        return;
                    dataGrid.SelectionChanged += delegate
                    {
                        if (dataGrid.SelectedItem == null) return;
#if NET40
                        dataGrid.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            dataGrid.UpdateLayout();
                            dataGrid.ScrollIntoView(dataGrid.SelectedItem, null);
                        }));
#else                   
                    dataGrid.Dispatcher.InvokeAsync(() =>
                    {
                        dataGrid.UpdateLayout();
                        dataGrid.ScrollIntoView(dataGrid.SelectedItem, null);
                    });
#endif
                    };
                    break;
                case Type _ when type == typeof(ListBox):
                    var listBox = (ListBox)sender;
                    if (listBox == null)
                        return;
                    listBox.SelectionChanged += delegate
                    {
                        if (listBox.SelectedItem == null) return;
#if NET40
                        listBox.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            listBox.UpdateLayout();
                            listBox.ScrollIntoView(listBox.SelectedItem);
                        }));
#else                   
                    listBox.Dispatcher.InvokeAsync(() =>
                    {
                        listBox.UpdateLayout();
                        listBox.ScrollIntoView(listBox.SelectedItem);
                    });
#endif
                    };
                    break;
                case Type _ when type == typeof(ListView):
                    var listView = (ListView)sender;
                    if (listView == null)
                        return;
                    listView.SelectionChanged += delegate
                    {
                        if (listView.SelectedItem == null) return;
#if NET40
                        listView.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            listView.UpdateLayout();
                            listView.ScrollIntoView(listView.SelectedItem);
                        }));
#else                   
                    listView.Dispatcher.InvokeAsync(() =>
                    {
                        listView.UpdateLayout();
                        listView.ScrollIntoView(listView.SelectedItem);
                    });
#endif
                    };
                    break;
                default:
                    break;
            }
        }

    }
}
