using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class AnimationNavigationBar3DItem : Control
    {
        /// <summary>
        /// 默认颜色
        /// </summary>
        public Brush FileBackground
        {
            get { return (Brush)GetValue(FileBackgroundProperty); }
            set { SetValue(FileBackgroundProperty, value); }
        }

        public static readonly DependencyProperty FileBackgroundProperty =
            DependencyProperty.Register("FileBackground", typeof(Brush), typeof(AnimationNavigationBar3DItem), new PropertyMetadata(null));

        /// <summary>
        /// 背面颜色
        /// </summary>
        public Brush BackFileBackground
        {
            get { return (Brush)GetValue(BackFileBackgroundProperty); }
            set { SetValue(BackFileBackgroundProperty, value); }
        }

        public static readonly DependencyProperty BackFileBackgroundProperty =
            DependencyProperty.Register("BackFileBackground", typeof(Brush), typeof(AnimationNavigationBar3DItem), new PropertyMetadata(null));



        /// <summary>
        /// 设置PathData
        /// </summary>
        public Geometry PathDate
        {
            get { return (Geometry)GetValue(PathDateProperty); }
            set { SetValue(PathDateProperty, value); }
        }

        public static readonly DependencyProperty PathDateProperty =
            DependencyProperty.Register("PathDate", typeof(Geometry), typeof(AnimationNavigationBar3DItem), new PropertyMetadata());




        /// <summary>
        /// 文本显示内容
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AnimationNavigationBar3DItem), new PropertyMetadata());

        static AnimationNavigationBar3DItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimationNavigationBar3DItem), new FrameworkPropertyMetadata(typeof(AnimationNavigationBar3DItem)));
        }
    }
}
