using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class MultiSelectListBox : ListBox
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MultiSelectComboBoxItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultiSelectComboBoxItem();
        }

    }
}
