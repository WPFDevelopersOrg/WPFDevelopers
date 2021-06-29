using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfTimeLineControl
{
    public class TimeLineItemControl : Control
    {
        public bool IsBottom
        {
            get { return (bool)GetValue(IsBottomProperty); }
            set { SetValue(IsBottomProperty, value); }
        }

        public static readonly DependencyProperty IsBottomProperty =
            DependencyProperty.Register("IsBottom", typeof(bool), typeof(TimeLineItemControl), new PropertyMetadata(false));



        public ItemTypeEnum ItemType
        {
            get { return (ItemTypeEnum)GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }

        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.Register("ItemType", typeof(ItemTypeEnum), typeof(TimeLineItemControl));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TimeLineItemControl), new PropertyMetadata(string.Empty));
        public string Head
        {
            get { return (string)GetValue(HeadProperty); }
            set { SetValue(HeadProperty, value); }
        }

        public static readonly DependencyProperty HeadProperty =
            DependencyProperty.Register("Head", typeof(string), typeof(TimeLineItemControl), new PropertyMetadata(string.Empty));
        public DataTemplate CommonTemplate
        {
            get { return (DataTemplate)GetValue(CommonTemplateProperty); }
            set { SetValue(CommonTemplateProperty, value); }
        }
        public static readonly DependencyProperty CommonTemplateProperty =
            DependencyProperty.Register("CommonTemplate", typeof(DataTemplate), typeof(TimeLineItemControl));
      
        public DataTemplate TextTemplate
        {
            get { return (DataTemplate)GetValue(TextTemplateProperty); }
            set { SetValue(TextTemplateProperty, value); }
        }

        public static readonly DependencyProperty TextTemplateProperty =
            DependencyProperty.Register("TextTemplate", typeof(DataTemplate), typeof(TimeLineItemControl));


        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }
        public static readonly DependencyProperty BackgroundColorProperty =
           DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(TimeLineItemControl), new PropertyMetadata(null));
        static TimeLineItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeLineItemControl), new FrameworkPropertyMetadata(typeof(TimeLineItemControl)));
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
