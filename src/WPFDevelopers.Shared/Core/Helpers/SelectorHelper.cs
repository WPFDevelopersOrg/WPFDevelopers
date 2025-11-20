using System.Windows.Controls;

namespace WPFDevelopers.Helpers
{
    public static class SelectorHelper
    {
        public static object GetItemDisplayValue(this ListView listView,object item)
        {
            if (item == null) return null;
            if (string.IsNullOrWhiteSpace(listView.DisplayMemberPath) && string.IsNullOrWhiteSpace(listView.SelectedValuePath))
            {
                var property = item.GetType().GetProperty("Content");
                if (property != null)
                    return property.GetValue(item, null);
            }
            var nameParts = listView.DisplayMemberPath.Split('.');
            if (nameParts.Length == 1)
            {
                var property = item.GetType().GetProperty(listView.SelectedValuePath);
                property = property == null ? item.GetType().GetProperty(listView.DisplayMemberPath) : property;
                if (property != null)
                    return property.GetValue(item, null);
            }
            return item;
        }
    }
}
