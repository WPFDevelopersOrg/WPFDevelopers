using System;
using System.Collections;
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

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// SongWordsExample.xaml 的交互逻辑
    /// </summary>
    public partial class SongWordsExample : UserControl
    {
        public IEnumerable MusicWordArray
        {
            get { return (IEnumerable)GetValue(MusicWordArrayProperty); }
            set { SetValue(MusicWordArrayProperty, value); }
        }

        public static readonly DependencyProperty MusicWordArrayProperty =
            DependencyProperty.Register("MusicWordArray", typeof(IEnumerable), typeof(SongWordsExample), new PropertyMetadata(null));
        public SongWordsExample()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var musicWords = new List<MusicWord>();
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(1), StarTime = TimeSpan.FromSeconds(0), SongWords = "作词 : 田汉" });
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(1), StarTime = TimeSpan.FromSeconds(1), SongWords = "作曲 : 聂耳" });
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(0.5), StarTime = TimeSpan.FromSeconds(2), SongWords = "起来！" });
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(2), StarTime = TimeSpan.FromSeconds(2.5), SongWords = "不愿做奴隶的人们！！" });
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(4), StarTime = TimeSpan.FromSeconds(4.5), SongWords = "把我们的血肉，筑成我们新的长城！" });
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(3), StarTime = TimeSpan.FromSeconds(8.5), SongWords = "中华民族到了最危险的时候，" });
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(3.5), StarTime = TimeSpan.FromSeconds(11.5), SongWords = "每个人被迫着发出最后的吼声。" });
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(0.5), StarTime = TimeSpan.FromSeconds(15), SongWords = "起来！" });
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(0.5), StarTime = TimeSpan.FromSeconds(15.5), SongWords = "起来！" });
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(0.5), StarTime = TimeSpan.FromSeconds(16), SongWords = "起来！" });
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(4.5), StarTime = TimeSpan.FromSeconds(16.5), SongWords = "我们万众一心，冒着敌人的炮火前进！" });
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(2), StarTime = TimeSpan.FromSeconds(21), SongWords = "冒着敌人的炮火前进！" });
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(0.5), StarTime = TimeSpan.FromSeconds(23), SongWords = "前进！" });
            musicWords.Add(new MusicWord { RunTime = TimeSpan.FromSeconds(0.5), StarTime = TimeSpan.FromSeconds(23.5), SongWords = "前进！进！" });
            MusicWordArray = musicWords;
        }
    }
    public class MusicWord
    {
        public TimeSpan RunTime { get; set; }
        public TimeSpan StarTime { get; set; }
        public string SongWords { get; set; }

    }
}
