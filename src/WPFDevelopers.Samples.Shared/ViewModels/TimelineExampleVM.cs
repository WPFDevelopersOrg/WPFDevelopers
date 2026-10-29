using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ViewModels
{
    public class TimelineExampleVM : ViewModelBase
    {
        private Orientation _orientation = Orientation.Vertical;
        private TimelineMode _mode = TimelineMode.Alternate;
        private bool _reverse;
        private int _eventNumber;

        public ObservableCollection<TimelineEvent> Events { get; private set; }
        public Array Orientations { get; private set; }
        public Array Modes { get; private set; }
        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }
        public ICommand CompleteCommand { get; private set; }
        public ICommand NextCommand { get; private set; }

        public Orientation Orientation
        {
            get { return _orientation; }
            set { _orientation = value; NotifyPropertyChange(nameof(Orientation)); }
        }

        public TimelineMode Mode
        {
            get { return _mode; }
            set { _mode = value; NotifyPropertyChange(nameof(Mode)); }
        }

        public bool Reverse
        {
            get { return _reverse; }
            set { _reverse = value; NotifyPropertyChange(nameof(Reverse)); }
        }

        public TimelineExampleVM()
        {
            Events = new ObservableCollection<TimelineEvent>();
            Orientations = Enum.GetValues(typeof(Orientation));
            Modes = Enum.GetValues(typeof(TimelineMode));
            AddCommand = new RelayCommand(_ => AddEvent());
            RemoveCommand = new RelayCommand(_ =>
            {
                if (Events.Count > 0) Events.RemoveAt(Events.Count - 1);
            });
            ResetCommand = new RelayCommand(_ => ResetEvents());
            CompleteCommand = new RelayCommand(_ => CompleteEvents(true));
            NextCommand = new RelayCommand(_ =>
            {
                CompleteEvents(false);
                AddEvent(true);
            });
            ResetEvents();
        }

        private void CompleteEvents(bool onlyLoading)
        {
            foreach (var item in Events)
            {
                if (onlyLoading && !item.IsLoading) continue;
                item.IsLoading = false;
                item.Title = "Completed";
                item.Description = "Updated through INotifyPropertyChanged.";
                item.Brush = Brushes.SeaGreen;
            }
        }

        private void AddEvent(bool isLoading = false)
        {
            Events.Add(new TimelineEvent
            {
                Time = DateTime.Now.ToString("HH:mm:ss"),
                Title = "Event " + ++_eventNumber,
                Description = "Added by a ViewModel command.",
                Brush = Brushes.DodgerBlue,
                IsLoading = isLoading
            });
        }

        private void ResetEvents()
        {
            Events.Clear();
            _eventNumber = 3;
            Events.Add(new TimelineEvent
            {
                Time = "09:00", Title = "Created",
                Description = "Create a service site.", Brush = Brushes.DodgerBlue
            });
            Events.Add(new TimelineEvent
            {
                Time = "10:00", Title = "Completed",
                Description = "Solve initial network problems.", Brush = Brushes.SeaGreen
            });
            Events.Add(new TimelineEvent
            {
                Time = "11:00", Title = "In progress",
                Description = "Technical testing is in progress.", Brush = Brushes.DodgerBlue,
                IsLoading = true
            });
        }
    }

    public class TimelineEvent : ViewModelBase
    {
        private string _time;
        private string _title;
        private string _description;
        private Brush _brush;
        private bool _isLoading;

        public string Time
        {
            get { return _time; }
            set { _time = value; NotifyPropertyChange(nameof(Time)); }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChange(nameof(Title)); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChange(nameof(Description)); }
        }

        public Brush Brush
        {
            get { return _brush; }
            set { _brush = value; NotifyPropertyChange(nameof(Brush)); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; NotifyPropertyChange(nameof(IsLoading)); }
        }
    }
}
