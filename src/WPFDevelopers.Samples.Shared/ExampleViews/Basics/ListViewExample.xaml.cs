using System.Collections.ObjectModel;
using System.Windows.Controls;
using WPFDevelopers.Sample.Models;

namespace WPFDevelopers.Samples.ExampleViews.Basics
{
    public partial class ListViewExample : UserControl
    {
        public ObservableCollection<UserModel> UserCollection
        {
            get => (ObservableCollection<UserModel>)GetValue(UserCollectionProperty);
            set => SetValue(UserCollectionProperty, value);
        }

        public static readonly System.Windows.DependencyProperty UserCollectionProperty =
            System.Windows.DependencyProperty.Register("UserCollection", typeof(ObservableCollection<UserModel>), typeof(ListViewExample),
                new System.Windows.PropertyMetadata(null));

        public ListViewExample()
        {
            InitializeComponent();
            UserCollection = BasicDataHelper.CreateUserCollection();
        }
    }
}
