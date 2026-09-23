using WPFDevelopers.Core;

namespace WPFDevelopers.Controls
{
    public class FilterItem : ObservableObject
    {
        private bool _isChecked = true;
        public object Value { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                SetProperty(ref _isChecked, value, nameof(IsChecked));
            }
        }
    }
}
