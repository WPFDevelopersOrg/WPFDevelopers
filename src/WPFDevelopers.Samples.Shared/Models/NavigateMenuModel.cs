namespace WPFDevelopers.Samples.Models
{
    public class NavigateMenuModel:ViewModelBase
    {
        private bool _isVisible = true;
        public string Name { get; set; }
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