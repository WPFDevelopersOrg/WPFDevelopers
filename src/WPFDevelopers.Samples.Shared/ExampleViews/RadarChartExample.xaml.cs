using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
        List<Player> Players = new List<Player>();
        private int NowPlayerIndex = 0;
        public string NowPlayerName
        {
            get { return (string)GetValue(NowPlayerNameProperty); }
            set { SetValue(NowPlayerNameProperty, value); }
        }
        public static readonly DependencyProperty NowPlayerNameProperty =
     DependencyProperty.Register("NowPlayerName", typeof(string), typeof(RadarChartExample), new PropertyMetadata(null));

        List<ObservableCollection<RadarModel>> collectionList = new List<ObservableCollection<RadarModel>>();
        public RadarChartExample()
        {
            InitializeComponent();
            RadarModels = new ObservableCollection<RadarModel>();
            Player theShy = new Player()
            {
                姓名 = "The Shy",
                击杀 = 80,
                助攻 = 50,
                物理 = 90,
                生存 = 10,
                金钱 = 60,
                防御 = 30,
                魔法 = 30
            };
            Player xiaoHu = new Player()
            {
                姓名 = "销户",
                击杀 = 50,
                助攻 = 50,
                物理 = 50,
                生存 = 50,
                金钱 = 50,
                防御 = 50,
                魔法 = 50
            };
            Player yinHang = new Player()
            {
                姓名 = "狼行",
                击杀 = 40,
                助攻 = 60,
                物理 = 60,
                生存 = 90,
                金钱 = 40,
                防御 = 80,
                魔法 = 60
            };
            Player flandre = new Player()
            {
                姓名 = "圣枪哥",
                击杀 = 60,
                助攻 = 70,
                物理 = 80,
                生存 = 70,
                金钱 = 80,
                防御 = 80,
                魔法 = 30
            };
            Players.AddRange(new[] { theShy, xiaoHu, yinHang, flandre });

            Type t = theShy.GetType();
            PropertyInfo[] pArray = t.GetProperties();
            pArray = pArray.Where(it => it.PropertyType == typeof(int)).ToArray();

            foreach (var player in Players)
            {
                var collectionpPayer = new ObservableCollection<RadarModel>();
                Array.ForEach<PropertyInfo>(pArray, p =>
                {
                    collectionpPayer.Add(new RadarModel { Text = $"{p.Name}（{(int)p.GetValue(player, null)}分）", ValueMax = (int)p.GetValue(player, null) });
                });
                collectionList.Add(collectionpPayer);
            }
            RadarModels = collectionList[0];
            NowPlayerName = Players[0].姓名;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NowPlayerIndex++;
            if (NowPlayerIndex >= collectionList.Count)
            {
                NowPlayerIndex = 0;
            }
            RadarModels = collectionList[NowPlayerIndex];
            NowPlayerName = Players[NowPlayerIndex].姓名;
        }
    }

    public class Player
    {
        public string 姓名 { get; set; }
        public int 击杀 { get; set; }
        public int 生存 { get; set; }
        public int 助攻 { get; set; }
        public int 物理 { get; set; }
        public int 魔法 { get; set; }
        public int 防御 { get; set; }
        public int 金钱 { get; set; }
    }
}
