using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// TimePickerExample.xaml 的交互逻辑
    /// </summary>
    public partial class TimePickerExample : UserControl
    {

        public DateTime? MyDateTime
        {
            get { return (DateTime)GetValue(MyDateTimeProperty); }
            set { SetValue(MyDateTimeProperty, value); }
        }

        public static readonly DependencyProperty MyDateTimeProperty =
            DependencyProperty.Register("MyDateTime", typeof(DateTime?), typeof(TimePickerExample), new PropertyMetadata(null));

        public TimePickerExample()
        {
            InitializeComponent();
            MyDateTime = DateTime.Now.AddHours(1);
        }
    }
}
