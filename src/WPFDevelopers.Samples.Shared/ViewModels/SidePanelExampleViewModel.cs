using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using WPFDevelopers;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ViewModels
{
    public class SidePanelExampleViewModel : ViewModelBase, IWeakEventListener
    {
        private bool _isExpanded = true;
        private int _taskCount = 3;

        public SidePanelExampleViewModel()
        {
            PropertyChangedEventManager.AddListener(
                LanguageManager.Instance, this, nameof(LanguageManager.CurrentCulture));
            ToggleCommand = new RelayCommand(parameter => IsExpanded = !IsExpanded);
            AddTaskCommand = new RelayCommand(parameter => Tasks.Insert(0, new SidePanelTask(++_taskCount)));
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value, nameof(IsExpanded));
        }

        public ObservableCollection<SidePanelTask> Tasks { get; } =
            new ObservableCollection<SidePanelTask> { new SidePanelTask(1), new SidePanelTask(2), new SidePanelTask(3) };

        public ICommand ToggleCommand { get; }
        public ICommand AddTaskCommand { get; }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType != typeof(PropertyChangedEventManager))
                return false;

            foreach (var task in Tasks)
                task.UpdateLanguage();
            return true;
        }
    }

    public class SidePanelTask : ViewModelBase
    {
        private readonly int _number;

        public SidePanelTask(int number)
        {
            _number = number;
        }

        public string Text => SampleLanguageManager.Instance.TranslateText($"任务 {_number}");

        internal void UpdateLanguage()
        {
            NotifyPropertyChange(nameof(Text));
        }
    }
}
