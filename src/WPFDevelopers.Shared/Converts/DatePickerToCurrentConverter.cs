using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace WPFDevelopers.Converts
{
    //public class DatePickerToCurrentConverter : IValueConverter
    //{

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value == null)
    //            return Visibility.Collapsed;
    //        if (value is CalendarButton calendar)
    //        {
    //            var current = calendar.DataContext;

    //        }

    //        var time = DateTime.Now.Date.Month;
    //        return false;
    //    }



    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    public class DatePickerToCurrentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Visibility.Collapsed;
            if (values[0] == null)
                return Visibility.Collapsed;
            if (values[1] is CalendarButton calendar)
            {
                var current = (DateTime)calendar.DataContext;
                var time = DateTime.Now.Date;
                int result;
                var isYear = int.TryParse(calendar.Content.ToString(), out result);
                if (isYear)
                {
                    if (current.Year == time.Year)
                        return Visibility.Visible;
                }
                else
                {
                    if (current.Year == time.Year
                        &&
                        current.Month == time.Month)
                        return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }
        

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
