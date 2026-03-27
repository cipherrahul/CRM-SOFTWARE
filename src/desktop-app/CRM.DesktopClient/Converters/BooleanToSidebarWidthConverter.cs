using System.Globalization;
using System.Windows.Data;

namespace CRM.DesktopClient.Converters;

public class BooleanToSidebarWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? 70.0 : 260.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
