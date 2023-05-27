using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Samples.Controls
{
    public class TimeLineItem : Control
    {
        public static readonly DependencyProperty IsBottomProperty =
            DependencyProperty.Register("IsBottom", typeof(bool), typeof(TimeLineItem), new PropertyMetadata(false));

        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.Register("ItemType", typeof(ItemTypeEnum), typeof(TimeLineItem));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TimeLineItem),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty HeadProperty =
            DependencyProperty.Register("Head", typeof(string), typeof(TimeLineItem),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty CommonTemplateProperty =
            DependencyProperty.Register("CommonTemplate", typeof(DataTemplate), typeof(TimeLineItem));

        public static readonly DependencyProperty TextTemplateProperty =
            DependencyProperty.Register("TextTemplate", typeof(DataTemplate), typeof(TimeLineItem));

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(TimeLineItem),
                new PropertyMetadata(null));

        static TimeLineItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeLineItem),
                new FrameworkPropertyMetadata(typeof(TimeLineItem)));
        }

        public bool IsBottom
        {
            get => (bool)GetValue(IsBottomProperty);
            set => SetValue(IsBottomProperty, value);
        }


        public ItemTypeEnum ItemType
        {
            get => (ItemTypeEnum)GetValue(ItemTypeProperty);
            set => SetValue(ItemTypeProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string Head
        {
            get => (string)GetValue(HeadProperty);
            set => SetValue(HeadProperty, value);
        }

        public DataTemplate CommonTemplate
        {
            get => (DataTemplate)GetValue(CommonTemplateProperty);
            set => SetValue(CommonTemplateProperty, value);
        }

        public DataTemplate TextTemplate
        {
            get => (DataTemplate)GetValue(TextTemplateProperty);
            set => SetValue(TextTemplateProperty, value);
        }


        public Brush BackgroundColor
        {
            get => (Brush)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }
    }

    public enum ItemTypeEnum
    {
        Time,
        Name,
        Fork,
        Star
    }
}