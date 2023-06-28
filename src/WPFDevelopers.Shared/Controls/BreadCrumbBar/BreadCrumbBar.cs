using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class BreadCrumbBar : ListBox
    {
        static BreadCrumbBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadCrumbBar),
                new FrameworkPropertyMetadata(typeof(BreadCrumbBar)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            for (var i = 0; i <= SelectedIndex; i++)
            {
                var item = (ListBoxItem) ItemContainerGenerator.ContainerFromIndex(i);
                if (item == null) continue;
                if (!item.IsEnabled)
                    item.IsEnabled = true;
            }

            for (var i = Items.Count - 1; i > SelectedIndex; i--)
            {
                var item = (ListBoxItem) ItemContainerGenerator.ContainerFromIndex(i);
                if (item == null) continue;
                if (item.IsEnabled)
                    item.IsEnabled = false;
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is BreadCrumbBarItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BreadCrumbBarItem();
        }
    }
}