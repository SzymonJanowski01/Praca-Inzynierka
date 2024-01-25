using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.Converters
{
    /// <summary>
    /// Class responsible for ensuring that email is in correct format.
    /// </summary>
    public class EmailFormatConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string email)
            {
                bool negate = parameter != null && bool.TryParse(parameter.ToString(), out _);

                if (Regex.IsMatch(email, @"^\w+@\w+\.\w+$"))
                {
                    return negate ? false : true;
                }
                return negate ? true : false;
            }
            return true;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
