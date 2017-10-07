using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace VCSJones.FiddlerCert
{
    public class NullVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ExpiryStrengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const int MONTHS_BEFORE_WARNING = 1;
            var red = new SolidColorBrush(Colors.Red);
            var black = new SolidColorBrush(Colors.Black);
            var yellow = new SolidColorBrush(Colors.Goldenrod);
            if (value is DateTime dateTime)
            {
                if (dateTime < DateTime.Now)
                {
                    return red;
                }
                if (dateTime < DateTime.Now.AddMonths(MONTHS_BEFORE_WARNING))
                {
                    return yellow;
                }
                return black;

            }
            if (value is DateTimeOffset dateTimeOffset)
            {
                if (dateTimeOffset < DateTimeOffset.Now)
                {
                    return red;
                }
                if (dateTimeOffset < DateTimeOffset.Now.AddMonths(MONTHS_BEFORE_WARNING))
                {
                    return yellow;
                }
                return black;

            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan))
            {
                return null;
            }
            var timeSpan = (TimeSpan)value;
            if (timeSpan < TimeSpan.Zero)
            {
                return "Expired";
            }
            var builder = new StringBuilder("Expires in ");
            builder.AppendFormat((int)timeSpan.TotalDays == 1 ? "{0:N0} day, " : "{0:N0} days, ", (int)timeSpan.TotalDays);
            builder.AppendFormat(timeSpan.Hours == 1 ? "{0:N0} hour, and " : "{0:N0} hours, and ", timeSpan.Hours);
            builder.AppendFormat(timeSpan.Minutes == 1 ? "{0:N0} minute. " : "{0:N0} minutes.", timeSpan.Minutes);
            return builder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class SignatureAlgorithmStrengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var red = new SolidColorBrush(Colors.Red);
            var black = new SolidColorBrush(Colors.Black);
            var yellow = new SolidColorBrush(Colors.Goldenrod);
            var algorithmModel = value as SignatureAlgorithmModel;
            if (algorithmModel?.IsTrustedRoot == true)
            {
                return black;
            }
            switch (algorithmModel?.SignatureAlgorithm?.Value)
            {
                case KnownOids.SignatureAlgorithms.sha1DSA:
                case KnownOids.SignatureAlgorithms.md5RSA:
                case KnownOids.SignatureAlgorithms.sha1ECDSA:
                case KnownOids.SignatureAlgorithms.sha1RSA:
                    return red;
                case KnownOids.SignatureAlgorithms.sha256ECDSA:
                case KnownOids.SignatureAlgorithms.sha256RSA:
                case KnownOids.SignatureAlgorithms.sha384ECDSA:
                case KnownOids.SignatureAlgorithms.sha384RSA:
                case KnownOids.SignatureAlgorithms.sha512ECDSA:
                case KnownOids.SignatureAlgorithms.sha512RSA:
                    return black;
                default:
                    return red;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ContainedValueVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return null;
            }
            var objType = value.GetType();
            var paramType = parameter.GetType();
            if (!paramType.IsArray)
            {
                return null;
            }
            if (paramType.GetElementType() != objType)
            {
                return null;
            }
            var arr = (Array)parameter;
            return Array.BinarySearch(arr, value) >= 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
