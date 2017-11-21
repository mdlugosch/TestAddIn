using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
    class EnumMatchToBooleanConverter : IValueConverter
    {
        // Value enthält den Inhalt der vom Benutzerelement gesendet wird
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string checkValue = value.ToString();
            string targetValue = parameter.ToString();

            if (value == null || parameter == null)
                return false;

            return checkValue.Equals(targetValue,
                     StringComparison.InvariantCultureIgnoreCase);

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value) ? parameter : Binding.DoNothing;
        }
    }
}
