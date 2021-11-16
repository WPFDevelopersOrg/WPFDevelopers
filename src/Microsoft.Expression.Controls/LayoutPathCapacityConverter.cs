using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.Controls
{
    public sealed class LayoutPathCapacityConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(double) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string text = value as string;
            if (text != null)
            {
                string a = text.ToUpperInvariant();
                if (a == "AUTO")
                {
                    return double.NaN;
                }
                return double.Parse(text, culture);
            }
            else
            {
                if (value is double)
                {
                    return value;
                }
                return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(destinationType == typeof(string)) || !(value is double))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
            double d = (double)value;
            if (double.IsNaN(d))
            {
                return "Auto";
            }
            return d.ToString(culture);
        }
    }
}
