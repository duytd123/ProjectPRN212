using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ProjectPRN212
{
    public class CustomStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (targetType == typeof(string))
                {
                    return parameter?.ToString() switch
                    {
                        "Payment" => boolValue ? "✅ Paid" : "❌ Unpaid",
                        "Notification" => boolValue ? "📨 Sent" : "❌ Not Sent",
                        _ => value
                    };
                }
                else if (targetType == typeof(Brush))
                {
                    return boolValue ? Brushes.Green : Brushes.Red;
                }
            }
            else if (value is string status && parameter?.ToString() == "Visibility")
            {
                return status == "Approved" ? Visibility.Visible : Visibility.Collapsed;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(bool?) && value is string stringValue)
            {
                return stringValue.Contains("✅") || stringValue.Contains("📨");
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
