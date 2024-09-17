using System;

namespace WPFDevelopers.Core
{
    public static class FormatExtensions
    {
        public static string FormatNumber(this object value)
        {
            if (value == null)
                return string.Empty;
            return Convert.ToDecimal(value).ToString("#,##0.#########################");
        }
    }

}
