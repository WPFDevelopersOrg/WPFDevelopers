using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFDevelopers.Controls;
using WPFDevelopers.Samples.ViewModels;

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class TimelineExample : UserControl
    {
        private int _eventNumber;

        public ObservableCollection<TimelineEvent> CodeEvents { get; private set; }
        public Array Orientations { get; private set; }
        public Array Modes { get; private set; }

        public TimelineExample()
        {
            CodeEvents = new ObservableCollection<TimelineEvent>();
            Orientations = Enum.GetValues(typeof(Orientation));
            Modes = Enum.GetValues(typeof(TimelineMode));
            ResetEvents();
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddEvent();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            CompleteEvents(false);
            AddEvent(true);
        }

        private void AddEvent(bool isLoading = false)
        {
            CodeEvents.Add(new TimelineEvent
            {
                Time = DateTime.Now.ToString("HH:mm:ss"),
                Title = "Event " + ++_eventNumber,
                Description = "Added by a C# click event.",
                Brush = Brushes.DodgerBlue,
                IsLoading = isLoading
            });
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (CodeEvents.Count > 0) CodeEvents.RemoveAt(CodeEvents.Count - 1);
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            CompleteEvents(true);
        }

        private void CompleteEvents(bool onlyLoading)
        {
            foreach (var item in CodeEvents)
            {
                if (onlyLoading && !item.IsLoading) continue;
                item.IsLoading = false;
                item.Title = "Completed";
                item.Description = "Updated by a C# click event.";
                item.Brush = Brushes.SeaGreen;
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetEvents();
        }

        private void ResetEvents()
        {
            CodeEvents.Clear();
            _eventNumber = 3;
            CodeEvents.Add(new TimelineEvent
            {
                Time = "09:00", Title = "Created",
                Description = "Create a service site.", Brush = Brushes.DodgerBlue
            });
            CodeEvents.Add(new TimelineEvent
            {
                Time = "10:00", Title = "Completed",
                Description = "Solve initial network problems.", Brush = Brushes.SeaGreen
            });
            CodeEvents.Add(new TimelineEvent
            {
                Time = "11:00", Title = "In progress",
                Description = "Technical testing is in progress.", Brush = Brushes.DodgerBlue,
                IsLoading = true
            });
        }
    }
}
