using System;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews.Basics
{
    public partial class DateTimePickerExample : UserControl
    {
        public DateTime CurrentDateTime
        {
            get { return (DateTime)GetValue(CurrentDateTimeProperty); }
            set { SetValue(CurrentDateTimeProperty, value); }
        }

        public static readonly DependencyProperty CurrentDateTimeProperty =
            DependencyProperty.Register("CurrentDateTime", typeof(DateTime), typeof(DateTimePickerExample),
                new PropertyMetadata(DateTime.Now));

        public DateTimePickerExample() { InitializeComponent(); }

        private void BtnGetDateTime_Click(object sender, RoutedEventArgs e)
        {
            Toast.Push(CurrentDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
