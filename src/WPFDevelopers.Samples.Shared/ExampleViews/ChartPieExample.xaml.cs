using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// ChartPieExample.xaml 的交互逻辑
    /// </summary>
    public partial class ChartPieExample : UserControl
    {
        public IEnumerable<KeyValuePair<string, double>> Datas
        {
            get { return (IEnumerable<KeyValuePair<string, double>>)GetValue(DatasProperty); }
            set { SetValue(DatasProperty, value); }
        }

        public static readonly DependencyProperty DatasProperty =
            DependencyProperty.Register("Datas", typeof(IEnumerable<KeyValuePair<string, double>>), typeof(ChartPieExample), new PropertyMetadata(null));

        private Dictionary<string, IEnumerable<KeyValuePair<string, double>>> keyValues = new Dictionary<string, IEnumerable<KeyValuePair<string, double>>>();
        private int _index = 0;
        public ChartPieExample()
        {
            InitializeComponent();
            var models1 = new[]
            {
                new KeyValuePair<string, double>("Mon", 120),
                new KeyValuePair<string, double>("Tue", 530),
                new KeyValuePair<string, double>("Wed", 1060),
                new KeyValuePair<string, double>("Thu", 140),
                new KeyValuePair<string, double>("Fri", 8000.123456) ,
                new KeyValuePair<string, double>("Sat", 200) ,
                new KeyValuePair<string, double>("Sun", 300) ,
            };
            var models2 = new[]
            {
                new KeyValuePair<string, double>("Bing", 120),
                new KeyValuePair<string, double>("Google", 170),
                new KeyValuePair<string, double>("Baidu", 30),
                new KeyValuePair<string, double>("Github", 200),
                new KeyValuePair<string, double>("Stack Overflow", 100) ,
                new KeyValuePair<string, double>("Runoob", 180) ,
                new KeyValuePair<string, double>("Open AI", 90) ,
                new KeyValuePair<string, double>("Open AI2", 93) ,
                new KeyValuePair<string, double>("Open AI3", 94) ,
                new KeyValuePair<string, double>("Open AI4", 95) ,
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
