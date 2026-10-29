using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFDevelopers.Samples.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// ActivityFeedExample.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityFeedExample : UserControl
    {
        public ActivityFeedExample()
        {
            InitializeComponent();
        }

        int num = 0;
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            num++;
            switch (num)
            {
                case 1:
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = DateTime.Now.ToString("yyyy-MM-dd"), ItemType = ItemTypeEnum.Time });
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = "我是骗人布010", Head = "我", ItemType = ItemTypeEnum.Name, BackgroundColor = new SolidColorBrush(GetRandomColor()) });
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = "闫驚鏵/WPFDevelopers", ItemType = ItemTypeEnum.Star });
                    break;
                case 2:
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ItemType = ItemTypeEnum.Time });
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = "风云大陆", Head = "风", ItemType = ItemTypeEnum.Name, BackgroundColor = new SolidColorBrush(GetRandomColor()) });
                    break;
                case 3:
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd"), ItemType = ItemTypeEnum.Time });
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = "王羲之", Head = "王", ItemType = ItemTypeEnum.Name, BackgroundColor = new SolidColorBrush(GetRandomColor()) });
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = "闫驚鏵/WPFDevelopers", ItemType = ItemTypeEnum.Star });
                    break;
                case 4:
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd"), ItemType = ItemTypeEnum.Time });
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = "花雨", Head = "花", ItemType = ItemTypeEnum.Name, BackgroundColor = new SolidColorBrush(GetRandomColor()) });
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = "闫驚鏵/WPFDevelopers", ItemType = ItemTypeEnum.Star });
                    break;
                case 5:
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd"), ItemType = ItemTypeEnum.Time });
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = "纪春庆", Head = "纪", ItemType = ItemTypeEnum.Name, BackgroundColor = new SolidColorBrush(GetRandomColor()) });
                    this.Feed.Items.Add(new ActivityFeedItem() { Text = "闫驚鏵/WPFDevelopers", ItemType = ItemTypeEnum.Star });
                    break;
                default:
                    break;
            }
        }
        Color GetRandomColor()
        {
            var random = new Random();
            return Color.FromRgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
        }
    }
}
