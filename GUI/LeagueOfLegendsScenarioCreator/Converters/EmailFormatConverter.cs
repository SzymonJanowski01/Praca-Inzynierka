using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueOfLegendsScenarioCreator.Converters
{
    public class EmailFormatConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string email)
            {
                if (Regex.IsMatch(email, @"^\w+@\w+\.\w+$"))
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
