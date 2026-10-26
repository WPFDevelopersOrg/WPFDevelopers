namespace WPFDevelopers.Samples.Models
{
    public class NavigateMenuModel:ViewModelBase
    {
        private bool _isVisible = true;
        private string _displayName;
        private string _changeType = string.Empty;
        public bool IsDemo { get; set; }
        public string Name { get; set; }
        public string DisplayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value, nameof(DisplayName)); }
        }

        public string ChangeType
        {
            get { return _changeType; }
            set { SetProperty(ref _changeType, value, nameof(ChangeType)); }
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