using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// ChartLineExample.xaml 的交互逻辑
    /// </summary>
    public partial class ChartLineExample : UserControl
    {
        public IEnumerable<KeyValuePair<string, double>> Datas
        {
            get { return (IEnumerable<KeyValuePair<string, double>>)GetValue(DatasProperty); }
            set { SetValue(DatasProperty, value); }
        }

        public static readonly DependencyProperty DatasProperty =
            DependencyProperty.Register("Datas", typeof(IEnumerable<KeyValuePair<string, double>>), typeof(ChartLineExample), new PropertyMetadata(null));

        private Dictionary<string, IEnumerable<KeyValuePair<string, double>>> keyValues = new Dictionary<string, IEnumerable<KeyValuePair<string, double>>>();
        private int _index = 0;
        public ChartLineExample()
        {
            InitializeComponent();
            var models1 = new[]
            {
                new KeyValuePair<string, double>("Mon", 120),
                new KeyValuePair<string, double>("Tue", 530),
                new KeyValuePair<string, double>("Wed", 1060),
                new KeyValuePair<string, double>("Thu", 140),
                new KeyValuePair<string, double>("Fri", 8000) ,
                new KeyValuePair<string, double>("Sat", 200) ,
                new KeyValuePair<string, double>("Sun", 300) ,
            };
            var models2 = new[]
            {
                new KeyValuePair<string, double>("（1）月", 120),
                new KeyValuePair<string, double>("（2）月", 170),
                new KeyValuePair<string, double>("（3）月", 30),
                new KeyValuePair<string, double>("（4）月", 200),
                new KeyValuePair<string, double>("（5）月", 100) ,
                new KeyValuePair<string, double>("（6）月", 180) ,
                new KeyValuePair<string, double>("（7）月", 90) ,
            };
            keyValues.Add("1", models1);
            keyValues.Add("2", models2);
            Datas = models1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _index++;
            if (_index >= keyValues.Count)
            {
                _index = 0;
            }
            Datas = keyValues.ToList()[_index].Value;
        }
    }
}
