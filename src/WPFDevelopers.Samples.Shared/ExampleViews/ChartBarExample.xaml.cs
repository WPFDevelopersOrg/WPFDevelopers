using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// ChartBarExample.xaml 的交互逻辑
    /// </summary>
    public partial class ChartBarExample : UserControl
    {
        public IEnumerable<KeyValuePair<string, double>> SeriesModels
        {
            get { return (IEnumerable<KeyValuePair<string, double>>)GetValue(SeriesModelsProperty); }
            set { SetValue(SeriesModelsProperty, value); }
        }

        public static readonly DependencyProperty SeriesModelsProperty =
            DependencyProperty.Register("SeriesModels", typeof(IEnumerable<KeyValuePair<string, double>>), typeof(ChartBarExample), new PropertyMetadata(null));


        Dictionary<string, IEnumerable<KeyValuePair<string, double>>> keyValues = new Dictionary<string, IEnumerable<KeyValuePair<string, double>>>();
        public string KeyBarChart
        {
            get { return (string)GetValue(KeyBarChartProperty); }
            set { SetValue(KeyBarChartProperty, value); }
        }
        public static readonly DependencyProperty KeyBarChartProperty =
     DependencyProperty.Register("KeyBarChart", typeof(string), typeof(ChartBarExample), new PropertyMetadata(null));
        private int _index = 0;
        public ChartBarExample()
        {
            InitializeComponent();
            var Models1 = new[]
            {
                new KeyValuePair<string, double>("Mon", 120),
                new KeyValuePair<string, double>("Tue", 130),
                new KeyValuePair<string, double>("Wed", 160),
                new KeyValuePair<string, double>("Thu", 140),
                new KeyValuePair<string, double>("Fri", 200) ,
                new KeyValuePair<string, double>("Sat", 80) ,
                new KeyValuePair<string, double>("Sun", 90) ,
            };
            keyValues.Add("到访数", Models1);
            var Models2 = new[]
            {
                new KeyValuePair<string, double>("蛐蛐", 120),
                new KeyValuePair<string, double>("常威", 170),
                new KeyValuePair<string, double>("来福", 30),
                new KeyValuePair<string, double>("包龙星", 200),
                new KeyValuePair<string, double>("包有为", 100) ,
                new KeyValuePair<string, double>("雷豹", 180) ,
                new KeyValuePair<string, double>("方唐镜", 90) ,
            };
            keyValues.Add("能力值", Models2);

            SeriesModels = keyValues.ToList()[0].Value;
            KeyBarChart = keyValues.ToList()[0].Key;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _index++;
            if (_index >= keyValues.Count)
            {
                _index = 0;
            }
            SeriesModels = keyValues.ToList()[_index].Value;
            KeyBarChart = keyValues.ToList()[_index].Key;
        }
    }
}
