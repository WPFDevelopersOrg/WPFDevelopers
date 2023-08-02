using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Converts
{

    [ValueConversion(typeof(Int32), typeof(StepItem))]
    public class StepIndexConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
        {
            var item = (StepItem)value;
            var step = ItemsControl.ItemsControlFromItemContainer(item) as Step;
            var index = step.ItemContainerGenerator.IndexFromContainer(item) + 1;
            item.Index = index;
            return index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
