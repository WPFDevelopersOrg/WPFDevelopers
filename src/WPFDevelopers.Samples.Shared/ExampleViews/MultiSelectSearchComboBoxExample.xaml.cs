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
    public partial class MultiSelectSearchComboBoxExample : UserControl
    {
        public ObservableCollection<UserInfo> SelectedItems
        {
            get { return (ObservableCollection<UserInfo>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<UserInfo>), typeof(MultiSelectSearchComboBoxExample), new PropertyMetadata(null));

        public ObservableCollection<UserInfo> ItemsSource
        {
            get { return (ObservableCollection<UserInfo>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<UserInfo>), typeof(MultiSelectSearchComboBoxExample), new PropertyMetadata(null));

        public MultiSelectSearchComboBoxExample()
        {
            InitializeComponent();
            Loaded += MultiSelectSearchComboBoxExample_Loaded;
        }

        private void MultiSelectSearchComboBoxExample_Loaded(object sender, RoutedEventArgs e)
        {
            //var list = new List<string>();
            //for (int i = 0; i < 10; i++)
            //    list.Add(i.ToString());
            //MyMultiSelectionSearchComboBox2.ItemsSource = list;
            //MyMultiSelectionSearchComboBox2.SelectedItems = list.Skip(3).ToList();

            var list2 = new List<UserInfo>();
            list2.Add(new UserInfo() { ID = "0", Name = "343DST.com" });
            list2.Add(new UserInfo() { ID = "1", Name = "ABCV.wang" });
            list2.Add(new UserInfo() { ID = "2", Name = "PPOI.xu" });
            list2.Add(new UserInfo() { ID = "3", Name = "josh.peng" });
            MyMultiSelectionSearchComboBox3.ItemsSource = list2;
            MyMultiSelectionSearchComboBox3.SelectedItems = list2.Where(x => x.ID == "1" || x.ID == "3").ToList();

            ItemsSource = new ObservableCollection<UserInfo>(list2);
            SelectedItems = new ObservableCollection<UserInfo>(list2.Where(x => x.ID == "1" || x.ID == "3"));
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WPFDevelopers.Controls.MessageBox.Show($"{MyMultiSelectionSearchComboBox2.Text} \r\n总共选中:{SelectedItems.Count} 条", "选中内容", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ItemsSource.Add(new UserInfo() { ID = "4", Name = "OpenAI.com" });
        }
    }
    public class UserInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
