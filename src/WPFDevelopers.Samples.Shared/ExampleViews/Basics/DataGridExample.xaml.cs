using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Sample.Models;

namespace WPFDevelopers.Samples.ExampleViews.Basics
{
    public partial class DataGridExample : UserControl
    {
        public ObservableCollection<UserModel> UserCollection
        {
            get => (ObservableCollection<UserModel>)GetValue(UserCollectionProperty);
            set => SetValue(UserCollectionProperty, value);
        }

        public static readonly DependencyProperty UserCollectionProperty =
            DependencyProperty.Register("UserCollection", typeof(ObservableCollection<UserModel>), typeof(DataGridExample),
                new PropertyMetadata(null));

        public bool AllSelected
        {
            get => (bool)GetValue(AllSelectedProperty);
            set => SetValue(AllSelectedProperty, value);
        }

        public static readonly DependencyProperty AllSelectedProperty =
            DependencyProperty.Register("AllSelected", typeof(bool), typeof(DataGridExample),
                new PropertyMetadata(AllSelectedChangedCallback));

        public DataGridExample()
        {
            InitializeComponent();
            UserCollection = BasicDataHelper.CreateUserCollection();
        }

        private static void AllSelectedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as DataGridExample;
            var isChecked = (bool)e.NewValue;
            BasicDataHelper.ApplyAllSelected(view.UserCollection, isChecked);
        }
    }
}
