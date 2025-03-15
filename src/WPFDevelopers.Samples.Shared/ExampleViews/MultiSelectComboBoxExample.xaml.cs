using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// MultiSelectComboBoxExample.xaml 的交互逻辑
    /// </summary>
    public partial class MultiSelectComboBoxExample : UserControl
    {
        public ObservableCollection<UserInfo> SelectedItems
        {
            get { return (ObservableCollection<UserInfo>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<UserInfo>), typeof(MultiSelectComboBoxExample), new PropertyMetadata(null));

        public ObservableCollection<UserInfo> ItemsSource
        {
            get { return (ObservableCollection<UserInfo>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<UserInfo>), typeof(MultiSelectComboBoxExample), new PropertyMetadata(null));
        public MultiSelectComboBoxExample()
        {
            InitializeComponent();
            Loaded += MultiSelectComboBoxExample_Loaded;
        }

        private void MultiSelectComboBoxExample_Loaded(object sender, RoutedEventArgs e)
        {
            var list2 = new List<UserInfo>();
            list2.Add(new UserInfo() { ID = "0", Name = "343DST.com" });
            list2.Add(new UserInfo() { ID = "1", Name = "ABCV.wang" });
            list2.Add(new UserInfo() { ID = "2", Name = "PPOI.xu" });
            list2.Add(new UserInfo() { ID = "3", Name = "josh.peng" });

            ItemsSource = new ObservableCollection<UserInfo>(list2);
            SelectedItems = new ObservableCollection<UserInfo>(list2.Where(x => x.ID == "1" || x.ID == "3"));
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string message = MyMultiSelectComboBox.Text.Trim().Length > 0
    ? $"{MyMultiSelectComboBox.Text} \r\n总共选中:{MyMultiSelectComboBox.SelectedItems.Count} 条"
    : $"总共选中:{MyMultiSelectComboBox.SelectedItems.Count} 条";
            WPFDevelopers.Controls.MessageBox.Show(message, "选中内容", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
