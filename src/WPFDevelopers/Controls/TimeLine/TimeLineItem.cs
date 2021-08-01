using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class TimeLineItem : Control
    {
        public bool IsBottom
        {
            get { return (bool)GetValue(IsBottomProperty); }
            set { SetValue(IsBottomProperty, value); }
        }

        public static readonly DependencyProperty IsBottomProperty =
            DependencyProperty.Register("IsBottom", typeof(bool), typeof(TimeLineItem), new PropertyMetadata(false));



        public ItemTypeEnum ItemType
        {
            get { return (ItemTypeEnum)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }

        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.Register("ItemType", typeof(ItemTypeEnum), typeof(TimeLineItem));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TimeLineItem), new PropertyMetadata(string.Empty));
        public string Head
        {
            get { return (string)GetValue(HeadProperty); }
            set { SetValue(HeadProperty, value); }
        }

        public static readonly DependencyProperty HeadProperty =
            DependencyProperty.Register("Head", typeof(string), typeof(TimeLineItem), new PropertyMetadata(string.Empty));
        public DataTemplate CommonTemplate
        {
            get { return (DataTemplate)GetValue(CommonTemplateProperty); }
            set { SetValue(CommonTemplateProperty, value); }
        }
        public static readonly DependencyProperty CommonTemplateProperty =
            DependencyProperty.Register("CommonTemplate", typeof(DataTemplate), typeof(TimeLineItem));
      
        public DataTemplate TextTemplate
        {
            get { return (DataTemplate)GetValue(TextTemplateProperty); }
            set { SetValue(TextTemplateProperty, value); }
        }

        public static readonly DependencyProperty TextTemplateProperty =
            DependencyProperty.Register("TextTemplate", typeof(DataTemplate), typeof(TimeLineItem));


        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }
        public static readonly DependencyProperty BackgroundColorProperty =
           DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(TimeLineItem), new PropertyMetadata(null));
        static TimeLineItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeLineItem), new FrameworkPropertyMetadata(typeof(TimeLineItem)));
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
