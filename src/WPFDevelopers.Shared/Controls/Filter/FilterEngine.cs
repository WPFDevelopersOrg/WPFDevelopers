using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace WPFDevelopers.Controls
{
    public interface IFilterEngine
    {
        IEnumerable Source { get; }
        Type ItemType { get; }
        void ApplyFilter(string propertyName, IEnumerable<object> values);
        void ClearFilter(string propertyName);
        void Refresh();
        HashSet<object> GetFilterValues(string propertyName);
        ObservableCollection<object> View { get; }
    }

    public struct FilterInfo
    {
        internal FilterInfo(string propertyName, IEnumerable values)
        {
            PropertyName = propertyName;
            var objectValues = values.Cast<object>();
            var hashSet = new HashSet<object>(objectValues);

#if NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471
            Values = hashSet;
#else
            Values = hashSet;
#endif
        }

        public string PropertyName { get; internal set; }

#if NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471
        public ICollection<object> Values { get; internal set; }
#else
        public IReadOnlyCollection<object> Values { get; internal set; }
#endif
    }

    public class FilterRelatedEventArgs : EventArgs
    {
        public FilterRelatedEventArgs(FilterInfo filter)
        {
            Filter = filter;
        }
        public FilterInfo Filter { get; set; }
    }

    public class FilterAppliedEventArgs : FilterRelatedEventArgs
    {
        public FilterAppliedEventArgs(FilterInfo filter) : base(filter)
        {

        }
    }

    public class FilterEngine<T> : IFilterEngine, IDisposable
    {
        public IList<T> Source { get; set; }

        IEnumerable IFilterEngine.Source => Source;
        Type IFilterEngine.ItemType => typeof(T);

        public ObservableCollection<T> TypedView { get; } = new ObservableCollection<T>();
        public EventHandler<FilterAppliedEventArgs> OnFilterApplied;

        private ObservableCollection<object> _objectView;
        private readonly object _syncLock = new object();

        ObservableCollection<object> IFilterEngine.View
        {
            get
            {
                if (_objectView == null)
                {
                    lock (_syncLock)
                    {
                        if (_objectView == null)
                        {
                            _objectView = new ObservableCollection<object>();

                            var weakHandler = new WeakEventListener<FilterEngine<T>, object, NotifyCollectionChangedEventArgs>(
                                this,
                                (engine, sender, args) => engine.OnTypedViewChanged(sender, args));

                            TypedView.CollectionChanged += weakHandler.OnEvent;
                            foreach (var item in TypedView)
                                _objectView.Add(item);
                        }
                    }
                }
                return _objectView;
            }
        }

        private void OnTypedViewChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Application.Current?.Dispatcher != null &&
                !Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    UpdateObjectView(e)));
            }
            else
            {
                UpdateObjectView(e);
            }
        }

        private void UpdateObjectView(NotifyCollectionChangedEventArgs e)
        {
            if (_objectView == null) return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (T item in e.NewItems)
                            _objectView.Add(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (T item in e.OldItems)
                            _objectView.Remove(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null && e.OldItems != null)
                    {
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            var index = e.OldStartingIndex + i;
                            if (index < _objectView.Count)
                            {
                                _objectView[index] = e.NewItems[i];
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    _objectView.Clear();
                    foreach (var item in TypedView)
                        _objectView.Add(item);
                    break;
            }
        }

        private readonly Dictionary<string, FilterCondition> _filters
            = new Dictionary<string, FilterCondition>();

        public void ApplyFilter(string propertyName, IEnumerable<object> values)
        {
            HashSet<object> persist = new HashSet<object>(values);
            _filters[propertyName] = new FilterCondition
            {
                PropertyName = propertyName,
                Values = persist
            };

            Refresh();
            OnFilterApplied?.Invoke(this, new FilterAppliedEventArgs(new FilterInfo(propertyName, persist)));
        }

        public void ClearFilters(IEnumerable<string> propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                _filters.Remove(propertyName);
            }

            Refresh();
        }

        /// <summary>
        /// Remove the filter associated with the specified property name.
        /// Use <seealso cref="ClearFilters"/> to remove multiple filters at once.
        /// </summary>
        /// <param name="propertyName"></param>
        public void ClearFilter(string propertyName) => ClearFilters(new[] { propertyName });

        public void Refresh()
        {
            TypedView.Clear();

            if (Source == null)
                return;

            foreach (var item in Source)
            {
                if (Match(item))
                    TypedView.Add(item);
            }
        }

        private bool Match(T item)
        {
            foreach (var filter in _filters.Values)
            {
                var getter = PropertyAccessor<T>.Get(filter.PropertyName);
                var value = getter(item);

                if (!filter.Values.Contains(value))
                    return false;
            }

            return true;
        }

        public HashSet<object> GetFilterValues(string propertyName)
        {
            if (_filters.TryGetValue(propertyName, out var condition))
                return condition.Values;

            return null;
        }

        public void Dispose()
        {
            if (_objectView != null)
            {
                TypedView.CollectionChanged -= OnTypedViewChanged;
            }
        }
    }

    public class WeakEventListener<TInstance, TSource, TEventArgs> where TInstance : class
    {
        private readonly WeakReference _weakInstance;
        private readonly Action<TInstance, TSource, TEventArgs> _onEventAction;

        public WeakEventListener(TInstance instance, Action<TInstance, TSource, TEventArgs> onEventAction)
        {
            _weakInstance = new WeakReference(instance);
            _onEventAction = onEventAction;
        }

        public void OnEvent(TSource source, TEventArgs e)
        {
            if (_weakInstance.Target is TInstance instance)
            {
                _onEventAction(instance, source, e);
            }
        }
    }
}
