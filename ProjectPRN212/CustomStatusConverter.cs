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
            if (parameter == null)
                return DependencyProperty.UnsetValue;

            string param = parameter.ToString();

            if (value is bool boolValue)
            {
                if (targetType == typeof(string))
                {
                    return param switch
                    {
                        "Payment" => boolValue ? "✅ Paid" : "❌ Unpaid",
                        "Notification" => boolValue ? "📨 Sent" : "❌ Not Sent",
                        _ => DependencyProperty.UnsetValue
                    };
                }
                else if (targetType == typeof(Brush))
                {
                    return boolValue ? Brushes.Green : Brushes.Red;
                }
            }
            else if (value is string status)
            {
                if (parameter?.ToString() == "Visibility")
                {
                    return status == "Approved" ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (parameter?.ToString() == "RejectionReason")
                {
                    return string.IsNullOrEmpty(status) ? Visibility.Collapsed : Visibility.Visible;
                }
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
