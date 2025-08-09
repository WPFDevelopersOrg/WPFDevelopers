using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Samples.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// AnimationGridExample.xaml 的交互逻辑
    /// </summary>
    public partial class AnimationGridExample : UserControl
    {
        public ObservableCollection<GridItem> GridItems
        {
            get { return (ObservableCollection<GridItem>)GetValue(GridItemsProperty); }
            set { SetValue(GridItemsProperty, value); }
        }

        public static readonly DependencyProperty GridItemsProperty =
            DependencyProperty.Register("GridItems", typeof(ObservableCollection<GridItem>), typeof(AnimationGridExample), new PropertyMetadata(null));
        public AnimationGridExample()
        {
            InitializeComponent();
            Loaded += OnAnimatedGridExample_Loaded;
        }

        private void OnAnimatedGridExample_Loaded(object sender, RoutedEventArgs e)
        {
            var list = new List<GridItem>();
            list.Add(new GridItem { Content = "Single", Data = "M0.5,0.5 L60.5,0.5 L60.5,43.26 L0.5,43.26 z", IsSelected = true });
            list.Add(new GridItem { Content = "Dual", Data = "M0,0 L61,0 L61,43.760002 L0,43.760002 z M25.5,0 L35.5,0 L35.5,43.760002 L25.5,43.760002 z" });
            list.Add(new GridItem { Content = "Three", Data = "M0,0 L61,0 L61,43.760002 L0,43.760002 z M17,0.5 L22,0.5 L22,43.260002 L17,43.260002 z M39,0.5 L44,0.5 L44,43.260002 L39,43.260002 z" });
            GridItems = new ObservableCollection<GridItem>(list);
        }

        public ICommand IsSelectedCommand => new RelayCommand(param =>
        {
            if (param == null) return;
            var item = (GridItem)param;
            if (item == null) return;
            MyPanel.ShowItem(item);
        });

    }
    public class GridItem : ViewModelBase
    {
        public string Content { get; set; }
        public string Data { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; NotifyPropertyChange("IsSelected"); }
        }
    }
}
