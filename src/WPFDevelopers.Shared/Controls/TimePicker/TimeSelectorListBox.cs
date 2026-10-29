using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class TimeSelectorListBox : ListBox
    {
        private const int CenterIndex = 4;
        private ScrollViewer _scrollViewer;
        private bool _isInternalScrolling;

        public TimeSelectorListBox()
        {
            Loaded -= TimeSelectorListBox_Loaded;
            Loaded += TimeSelectorListBox_Loaded;

            PreviewMouseWheel -= ScrollListBox_PreviewMouseWheel;
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

        private void TimeSelectorListBox_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = ControlsHelper.FindVisualChild<ScrollViewer>(this);
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
                _scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }
            if (Items.Count > CenterIndex && SelectedIndex < 0)
            {
                SelectedIndex = CenterIndex;
            }
            ScrollSelectedItemToCenter();
        }

        private void ScrollListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Items == null || Items.Count == 0)
                return;
            if (Items.Count <= CenterIndex)
            {
                e.Handled = true;
                return;
            }
            int step = e.Delta > 0 ? -1 : 1;
            MoveSelection(step);
            e.Handled = true;
        }

        private void MoveSelection(int step)
        {
            if (Items == null || Items.Count == 0)
                return;
            int newIndex = SelectedIndex;
            if (newIndex < 0)
                newIndex = CenterIndex;
            newIndex += step;
            if (newIndex < 0)
                newIndex = 0;
            if (newIndex >= Items.Count)
                newIndex = Items.Count - 1;
            if (newIndex != SelectedIndex)
            {
                SelectedIndex = newIndex;
            }
        }

        private void ScrollSelectedItemToCenter()
        {
            if (_scrollViewer == null || SelectedIndex < 0)
                return;
            var container = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as FrameworkElement;
            if (container == null)
            {
                UpdateLayout();
                container = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as FrameworkElement;
                if (container == null)
                    return;
            }
            double itemHeight = container.ActualHeight;
            if (itemHeight <= 0)
                return;
            _isInternalScrolling = true;
            double targetOffset = SelectedIndex - CenterIndex;
            if (targetOffset < 0)
                targetOffset = 0;
            _scrollViewer.ScrollToVerticalOffset(targetOffset);
            _isInternalScrolling = false;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_isInternalScrolling)
                return;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (SelectedIndex < 0)
                return;

            ScrollSelectedItemToCenter();
        }
    }
}
