using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WPFDevelopers.Samples
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            NotifyPropertyChange(propertyName);
            return true;
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, Action<T> onChanged, string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            onChanged?.Invoke(value);
            NotifyPropertyChange(propertyName);

            return true;
        }
    }
}
