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
                new NavScrollPanelItem{ Header = "播放相关", Content = new PlaybackSettings()},
                new NavScrollPanelItem{ Header = "桌面歌词", Content = new DesktopLyrics()},
                new NavScrollPanelItem{ Header = "快捷键", Content = new ShortcutKeys()},
                new NavScrollPanelItem{ Header = "隐私设置", Content = new PrivacySettings()},
                new NavScrollPanelItem{ Header = "关于我们", Content = new About()},
            };
            DataContext = this;
        }
    }
}
