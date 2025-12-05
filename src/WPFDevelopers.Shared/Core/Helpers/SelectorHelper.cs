using System.Windows.Controls;

namespace WPFDevelopers.Helpers
{
    public static class SelectorHelper
    {
        public static object GetDisplayAndSelectedValue(this ListView listView, object item)
        {
            if (item == null) return null;
            if (!string.IsNullOrWhiteSpace(listView.DisplayMemberPath))
            {
                return GetPropertyValueByPath(item, listView.DisplayMemberPath);
            }
            else if (!string.IsNullOrWhiteSpace(listView.SelectedValuePath))
            {
                return GetPropertyValueByPath(item, listView.SelectedValuePath);
            }
            var property = item.GetType().GetProperty("Content");
            if (property != null)
                return property.GetValue(item, null);
            return item; 
        }

      
        private static object GetPropertyValueByPath(object item, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return item;

            var currentObject = item;
            var nameParts = path.Split('.');

            foreach (var part in nameParts)
            {
                if (currentObject == null) return null;

                var property = currentObject.GetType().GetProperty(part);
                if (property == null) return null;

                currentObject = property.GetValue(currentObject, null);
            }

            return currentObject;
        }
    }
}
