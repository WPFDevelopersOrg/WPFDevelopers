using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class MultiSelector : ListView
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MultiSelectComboBoxItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultiSelectComboBoxItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element is MultiSelectComboBoxItem comboBoxItem)
            {
                if (View == null)
                    comboBoxItem.Content = this.GetDisplayAndSelectedValue(item);
            }
        }
    }
}
