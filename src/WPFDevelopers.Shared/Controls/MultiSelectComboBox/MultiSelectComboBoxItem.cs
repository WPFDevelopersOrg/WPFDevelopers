using System.Windows.Controls;
using System.Windows.Input;

namespace WPFDevelopers.Controls
{
    public class MultiSelectComboBoxItem : ListViewItem
    {
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (!IsSelected)
            {
                IsSelected = true;
            }
        }
    }
}