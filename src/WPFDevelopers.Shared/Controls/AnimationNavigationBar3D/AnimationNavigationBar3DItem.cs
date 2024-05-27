using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class AnimationNavigationBar3DItem : Control
    {
        public static readonly DependencyProperty FileBackgroundProperty =
            DependencyProperty.Register("FileBackground", typeof(Brush), typeof(AnimationNavigationBar3DItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty BackFileBackgroundProperty =
            DependencyProperty.Register("BackFileBackground", typeof(Brush), typeof(AnimationNavigationBar3DItem),
                new PropertyMetadata(null));

        public static readonly DependencyProperty PathDateProperty =
            DependencyProperty.Register("PathDate", typeof(Geometry), typeof(AnimationNavigationBar3DItem),
                new PropertyMetadata());

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AnimationNavigationBar3DItem),
                new PropertyMetadata());

        static AnimationNavigationBar3DItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimationNavigationBar3DItem),
                new FrameworkPropertyMetadata(typeof(AnimationNavigationBar3DItem)));
        }

        /// <summary>
        ///     默认颜色
        /// </summary>
        public Brush FileBackground
        {
            get => (Brush)GetValue(FileBackgroundProperty);
            set => SetValue(FileBackgroundProperty, value);
        }

        /// <summary>
        ///     背面颜色
        /// </summary>
        public Brush BackFileBackground
        {
            get => (Brush)GetValue(BackFileBackgroundProperty);
            set => SetValue(BackFileBackgroundProperty, value);
        }


        /// <summary>
        ///     设置PathData
        /// </summary>
        public Geometry PathDate
        {
            get => (Geometry)GetValue(PathDateProperty);
            set => SetValue(PathDateProperty, value);
        }


        /// <summary>
        ///     文本显示内容
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}