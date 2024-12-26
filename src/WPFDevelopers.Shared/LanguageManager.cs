using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace WPFDevelopers
{
    public class LanguageManager : INotifyPropertyChanged
    {
        private readonly ResourceManager _resourceManager;
#pragma warning disable CA1416
        private static readonly Lazy<LanguageManager> _lazy = new Lazy<LanguageManager>(() => new LanguageManager());
#pragma warning restore CA1416
        private CultureInfo _currentCulture;
        public static LanguageManager Instance => _lazy.Value;
        public event PropertyChangedEventHandler PropertyChanged;
        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            private set
            {
                if (_currentCulture != value)
                {
                    _currentCulture = value;
                    OnPropertyChanged(nameof(CurrentCulture));
                }
            }
        }

        public LanguageManager()
        {
            _resourceManager = new ResourceManager("WPFDevelopers.Languages.Language", typeof(LanguageManager).Assembly);
            _currentCulture = Thread.CurrentThread.CurrentUICulture ?? CultureInfo.InvariantCulture;
        }

        public string this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }
                return _resourceManager.GetString(name);
            }
        }

        public void ChangeLanguage(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            CurrentCulture = cultureInfo;
            OnPropertyChanged("Item[]");
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
