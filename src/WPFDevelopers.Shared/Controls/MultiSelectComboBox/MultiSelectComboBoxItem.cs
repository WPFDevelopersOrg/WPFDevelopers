using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class MultiSelectComboBoxItem : ListViewItem
    {
        static MultiSelectComboBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectComboBoxItem),
                new FrameworkPropertyMetadata(typeof(MultiSelectComboBoxItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var gridViewContainer = GetTemplateChild("PART_GridViewRowPresenter") as GridViewRowPresenter;
            if (gridViewContainer != null && gridViewContainer.Visibility == Visibility.Visible)
            {
                var parent = VisualTreeHelper.GetParent(gridViewContainer) as FrameworkElement;
                if (parent != null)
                {
                    parent.MouseLeftButtonDown += OnGridViewMouseLeftButtonDown;
                    parent.MouseLeftButtonUp += OnGridViewMouseLeftButtonUp;
                }
            }
            var border = GetTemplateChild("PART_Border") as FrameworkElement;
            if (border != null)
            {
                border.PreviewMouseLeftButtonDown -= OnItemPreviewMouseLeftButtonDown;
                border.PreviewMouseLeftButtonDown += OnItemPreviewMouseLeftButtonDown;
            }
        }

        private void OnGridViewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsSelected = !IsSelected;
            e.Handled = false;
        }

        private void OnGridViewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
        }
        private void OnItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsClickInside<CheckBox>(e.OriginalSource as DependencyObject))
                return;
            var listView = ItemsControl.ItemsControlFromItemContainer(this) as ListView;
            if (listView != null && listView.SelectionMode == SelectionMode.Single)
            {
                foreach (var item in listView.Items)
                {
                    var container = listView.ItemContainerGenerator.ContainerFromItem(item) as MultiSelectComboBoxItem;
                    if (container != null && container != this)
                    {
                        container.IsSelected = false;
                    }
                }
                IsSelected = true;
            }
            else
            {
                IsSelected = !IsSelected;
            }
            //IsSelected = !IsSelected;
            e.Handled = true;
        }

        private static bool IsClickInside<T>(DependencyObject source) where T : DependencyObject
        {
            while (source != null)
            {
                if (source is T) return true;
                source = VisualTreeHelper.GetParent(source);
            }
            return false;
        }
    }
}