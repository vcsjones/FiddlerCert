using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Expression = System.Linq.Expressions.Expression;

namespace VCSJones.FiddlerCert
{
    public class KeyStengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var publicKey = value as PublicKeyModel;
            var red = new SolidColorBrush(Colors.Red);
            var black = new SolidColorBrush(Colors.Black);
            if (publicKey == null)
            {
                return red;
            }
            switch (publicKey.Algorithm)
            {
                case PublicKeyAlgorithm.ECDSA:
                    return Colors.Green;
                case PublicKeyAlgorithm.RSA:
                    return publicKey.KeySizeBits == null ? red : publicKey.KeySizeBits.Value >= 2048 ? black : red;
                default:
                    return red;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ExpiryStrengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var red = new SolidColorBrush(Colors.Red);
            var black = new SolidColorBrush(Colors.Black);
            var yellow = new SolidColorBrush(Colors.Goldenrod);
            if (value is DateTime)
            {
                var dateTime = (DateTime)value;
                if (dateTime < DateTime.Now)
                {
                    return red;
                }
                if (dateTime < DateTime.Now.AddMonths(2))
                {
                    return yellow;
                }
                return black;

            }
            if (value is DateTimeOffset)
            {
                var dateTime = (DateTimeOffset)value;
                if (dateTime < DateTimeOffset.Now)
                {
                    return red;
                }
                if (dateTime < DateTimeOffset.Now.AddMonths(2))
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

    public class OidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var oid = value as Oid;
            return oid?.FriendlyName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var friendlyName = value as string;
            if (friendlyName == null)
            {
                return null;
            }
            return new Oid(CryptoConfig.MapNameToOID(friendlyName));
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
                    return red;
                case KnownOids.SignatureAlgorithms.sha1ECDSA:
                case KnownOids.SignatureAlgorithms.sha1RSA:
                    return yellow;
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

    public class ErrorsToShieldConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is CertificateErrors))
            {
                return null;
            }
            var errors = (CertificateErrors)value;
            var blank = new BitmapImage();
            blank.BeginInit();
            switch (errors)
            {
                default:
                    blank.UriSource = new Uri("/VCSJones.FiddlerCert;component/Assets/security_Shields_Blank_16xLG.png", UriKind.Relative);
                    break;
                case CertificateErrors.UnknownRevocation:
                    blank.UriSource = new Uri("/VCSJones.FiddlerCert;component/Assets/Security_Shields_Alert_16xLG_color.png", UriKind.Relative);
                    break;
                case CertificateErrors.Critical:
                    blank.UriSource = new Uri("/VCSJones.FiddlerCert;component/Assets/Security_Shields_Critical_16xLG_color.png", UriKind.Relative);
                    break;
                case CertificateErrors.None:
                    blank.UriSource = new Uri("/VCSJones.FiddlerCert;component/Assets/Security_Shields_Complete_and_ok_16xLG_color.png", UriKind.Relative);
                    break;

            }
            blank.EndInit();
            return blank;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class CommonErrorToShieldConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is CommonError))
            {
                return null;
            }
            var errors = (CommonError) value;
            var blank = new BitmapImage();
            blank.BeginInit();
            switch (errors)
            {
                default:
                    blank.UriSource = new Uri("/VCSJones.FiddlerCert;component/Assets/security_Shields_Blank_16xLG.png", UriKind.Relative);
                    break;
                case CommonError.Warning:
                    blank.UriSource = new Uri("/VCSJones.FiddlerCert;component/Assets/Security_Shields_Alert_16xLG_color.png", UriKind.Relative);
                    break;
                case CommonError.Fail:
                    blank.UriSource = new Uri("/VCSJones.FiddlerCert;component/Assets/Security_Shields_Critical_16xLG_color.png", UriKind.Relative);
                    break;
                case CommonError.OK:
                    blank.UriSource = new Uri("/VCSJones.FiddlerCert;component/Assets/Security_Shields_Complete_and_ok_16xLG_color.png", UriKind.Relative);
                    break;

            }
            blank.EndInit();
            return blank;
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

    public class EnumFlagsVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return null;
            }
            if (!value.GetType().IsEnum || !parameter.GetType().IsEnum)
            {
                return null;
            }
            var flagsParam = Expression.Parameter(typeof(object), "flags");
            var flagParam = Expression.Parameter(typeof(object), "flag");
            var hasFlag = Expression.Equal(Expression.And(Expression.Convert(flagsParam, value.GetType()), Expression.Convert(flagParam, parameter.GetType())), flagParam);
            var expression = Expression.Lambda<Func<object, object, bool>>(hasFlag, flagsParam, flagParam);
            return expression.Compile()(value, parameter) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class AndAllVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var alltrue = values.Select(m => (bool) m).All(x => x);
            return alltrue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
