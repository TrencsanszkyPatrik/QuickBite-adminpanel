using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Quickbite_AdminPanel.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive ? new SolidColorBrush(Color.FromArgb(255, 105, 117, 101)) : 
                                 new SolidColorBrush(Color.FromArgb(255, 196, 69, 54));
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
