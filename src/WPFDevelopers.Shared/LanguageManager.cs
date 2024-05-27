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
        private static readonly Lazy<LanguageManager> _lazy = new Lazy<LanguageManager>(() => new LanguageManager());
        public static LanguageManager Instance => _lazy.Value;
        public event PropertyChangedEventHandler PropertyChanged;

        public LanguageManager()
        {
            _resourceManager = new ResourceManager("WPFDevelopers.Languages.Language", typeof(LanguageManager).Assembly);
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
            OnPropertyChanged("Item[]");
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
