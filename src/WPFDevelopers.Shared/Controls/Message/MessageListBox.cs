using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class MessageListBox : ListBox
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MessageListBoxItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MessageListBoxItem();
        }

    }
}
