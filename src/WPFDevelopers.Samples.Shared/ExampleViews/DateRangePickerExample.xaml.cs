using System.Windows;
using System;
using System.Windows.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// DateRangePickerExample.xaml 的交互逻辑
    /// </summary>
    public partial class DateRangePickerExample : UserControl
    {
        public DateTime? StartDate
        {
            get { return (DateTime)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }

        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime?), typeof(DateRangePickerExample), new PropertyMetadata(null));
        public DateTime? EndDate
        {
            get { return (DateTime)GetValue(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }

        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register("EndDate", typeof(DateTime?), typeof(DateRangePickerExample), new PropertyMetadata(null));
        public DateRangePickerExample()
        {
            InitializeComponent();
            StartDate = DateTime.Now.AddDays(1);
            EndDate = StartDate.Value.AddDays(10);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WPFDevelopers.Controls.MessageBox.Show($"开始时间：{MyDateRangePicker.StartDate} \r结束时间：{MyDateRangePicker.EndDate}", "获取日期");
        }
    }
}
