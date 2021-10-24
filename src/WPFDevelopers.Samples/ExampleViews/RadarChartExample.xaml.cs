using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// RadarChartExample.xaml 的交互逻辑
    /// </summary>
    public partial class RadarChartExample : UserControl
    {
        public ObservableCollection<RadarModel> RadarModels
        {
            get { return (ObservableCollection<RadarModel>)GetValue(RadarModelsProperty); }
            set { SetValue(RadarModelsProperty, value); }
        }

        public static readonly DependencyProperty RadarModelsProperty =
            DependencyProperty.Register("RadarModels", typeof(ObservableCollection<RadarModel>), typeof(RadarChartExample), new PropertyMetadata(null));
        List<ObservableCollection<RadarModel>> collectionList = new List<ObservableCollection<RadarModel>>();
        public RadarChartExample()
        {
            InitializeComponent();
            RadarModels = new ObservableCollection<RadarModel>();
            var collection1 = new ObservableCollection<RadarModel>();
            collection1.Add(new RadarModel { Text = "击杀", ValueMax = 95});
            collection1.Add(new RadarModel { Text = "生存", ValueMax = 80 });
            collection1.Add(new RadarModel { Text = "助攻", ValueMax = 70 });
            collection1.Add(new RadarModel { Text = "物理", ValueMax = 80 });
            collection1.Add(new RadarModel { Text = "魔法", ValueMax = 90 });
            collection1.Add(new RadarModel { Text = "防御", ValueMax = 87 });
            collection1.Add(new RadarModel { Text = "金钱", ValueMax = 59 });

            var collection2 = new ObservableCollection<RadarModel>();
            collection2.Add(new RadarModel { Text = "击杀", ValueMax = 59 });
            collection2.Add(new RadarModel { Text = "生存", ValueMax = 80 });
            collection2.Add(new RadarModel { Text = "助攻", ValueMax = 90 });
            collection2.Add(new RadarModel { Text = "物理", ValueMax = 70 });
            collection2.Add(new RadarModel { Text = "魔法", ValueMax = 80 });
            collection2.Add(new RadarModel { Text = "防御", ValueMax = 90 });
            collection2.Add(new RadarModel { Text = "金钱", ValueMax = 66 });
            collectionList.AddRange(new[] { collection1, collection2 });
            RadarModels = collectionList[0];
        }
        bool isRefresh = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!isRefresh)
                RadarModels = collectionList[1];
            else
                RadarModels = collectionList[0];
            isRefresh = !isRefresh;
        }
    }
}
