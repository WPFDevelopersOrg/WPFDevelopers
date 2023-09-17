using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class TimeSelectorListBox : ListBox
    {
        private bool isFirst = true;
        private double lastIndex = 4;
        private ScrollViewer scrollViewer;

        public TimeSelectorListBox()
        {
            Loaded += TimeSelectorListBox_Loaded;
            PreviewMouseWheel += ScrollListBox_PreviewMouseWheel;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TimeSelectorItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TimeSelectorItem();
        }

        private void ScrollListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Items != null && Items.Count > 0)
            {
                var delta = e.Delta;
                var itemCount = Items.Count;
                var scrollCount = delta > 0 ? -1 : 1;
                var newIndex = SelectedIndex + scrollCount;
                if (newIndex < 4)
                    newIndex = 5;
                else if (newIndex >= itemCount - 4)
                    newIndex = itemCount;
                SelectedIndex = newIndex;
                e.Handled = true;
            }
        }

        private void TimeSelectorListBox_Loaded(object sender, RoutedEventArgs e)
        {
            scrollViewer = ControlsHelper.FindVisualChild<ScrollViewer>(this);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var offset = e.VerticalOffset;
            if (isFirst == false)
            {
                lastIndex = offset + 4;
            }
            else
            {
                lastIndex = offset == 0 ? 4 : offset + 4;
                isFirst = false;
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (SelectedIndex != -1 && lastIndex != -1)
            {
                if (SelectedIndex <= 0) return;
                var index = SelectedIndex - lastIndex;
                var offset = scrollViewer.VerticalOffset + index;
                scrollViewer.ScrollToVerticalOffset(offset);
            }

            base.OnSelectionChanged(e);
        }
    }
}