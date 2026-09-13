using System.Collections.ObjectModel;
using WPFDevelopers.Core;

namespace WPFDevelopers.Controls
{
    public class MapPushpinLayer : ObservableObject
    {
        private string _name;
        private bool _isVisible;
        private bool _enableClustering;
        private ObservableCollection<Pushpin> _pushpins;

        public MapPushpinLayer()
        {
            _pushpins = new ObservableCollection<Pushpin>();
            _isVisible = true;
            _enableClustering = true;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value, nameof(Name));
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                SetProperty(ref _isVisible, value, nameof(IsVisible));
            }
        }

        public bool EnableClustering
        {
            get { return _enableClustering; }
            set
            {
                SetProperty(ref _enableClustering, value, nameof(EnableClustering));
            }
        }

        public ObservableCollection<Pushpin> Pushpins
        {
            get { return _pushpins; }
            set
            {
                var next = value ?? new ObservableCollection<Pushpin>();
                SetProperty(ref _pushpins, next, nameof(Pushpins));
            }
        }
    }
}
