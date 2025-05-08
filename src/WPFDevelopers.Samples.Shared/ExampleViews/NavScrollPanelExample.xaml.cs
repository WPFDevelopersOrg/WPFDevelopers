using System.Collections.ObjectModel;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// NavScrollPanelExample.xaml 的交互逻辑
    /// </summary>
    public partial class NavScrollPanelExample : UserControl
    {
        public ObservableCollection<NavScrollPanelItem> Items { get; set; }
        public NavScrollPanelExample()
        {
            InitializeComponent();
            Items = new ObservableCollection<NavScrollPanelItem>
            {
                new NavScrollPanelItem{ Title = "播放相关", Content = new PlaybackSettings()},
                new NavScrollPanelItem{ Title = "桌面歌词", Content = new DesktopLyrics()},
                new NavScrollPanelItem{ Title = "快捷键", Content = new ShortcutKeys()},
                new NavScrollPanelItem{ Title = "隐私设置", Content = new PrivacySettings()},
                new NavScrollPanelItem{ Title = "关于我们", Content = new About()},
            };
            DataContext = this;
        }
    }
}
