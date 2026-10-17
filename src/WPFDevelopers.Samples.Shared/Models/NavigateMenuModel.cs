namespace WPFDevelopers.Samples.Models
{
    public class NavigateMenuModel:ViewModelBase
    {
        private bool _isVisible = true;
        private string _displayName;
        public string Name { get; set; }
        public string DisplayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value, nameof(DisplayName)); }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyPropertyChange("IsVisible");
            }
        }
    }
}