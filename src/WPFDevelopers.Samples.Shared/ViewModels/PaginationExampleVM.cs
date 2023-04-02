using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WPFDevelopers.Samples.ViewModels
{
    public class PaginationExampleVM : ViewModelBase
    {
        private List<int> _sourceList = new List<int>();

        public PaginationExampleVM()
        {
            _sourceList.AddRange(Enumerable.Range(1, 300));
            Count = 300;

            CurrentPageChanged();
        }

        public ObservableCollection<int> PaginationCollection { get; set; } = new ObservableCollection<int>();

        private int _count;
        public int Count
        {
            get { return _count; }
            set { _count = value;  this.NotifyPropertyChange("Count"); CurrentPageChanged(); }
        }

        private int _countPerPage = 10;
        public int CountPerPage
        {
            get { return _countPerPage; }
            set { _countPerPage = value; this.NotifyPropertyChange("CountPerPage"); CurrentPageChanged(); }
        }

        private int _current = 1;
        public int Current
        {
            get { return _current; }
            set { _current = value; this.NotifyPropertyChange("Current"); CurrentPageChanged(); }
        }

        private void CurrentPageChanged()
        {
            PaginationCollection.Clear();

            foreach (var i in _sourceList.Skip((Current - 1) * CountPerPage).Take(CountPerPage))
            {
                PaginationCollection.Add(i);
            }
        }
    }
}
