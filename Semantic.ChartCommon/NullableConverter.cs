using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class NullableConverter<T> : TypeConverter where T : struct
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(T) || sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(T);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;
            if (value is T)
                return new T?((T)value);
            if (string.IsNullOrEmpty(str) || string.Equals(str, "Auto", StringComparison.OrdinalIgnoreCase))
                return new T?();
            if (str != null)
            {
                if (typeof(T).IsEnum)
                {
                    try
                    {
                        return new T?((T)Enum.Parse(typeof(T), str, false));
                    }
                    catch (ArgumentNullException ex)
                    {
                    }
                    catch (ArgumentException ex)
                    {
                    }
                }
            }
            if (typeof(T) == typeof(TimeSpan))
                return new T?((T)(object)TimeSpan.Parse(str, culture));
            return new T?((T)Convert.ChangeType(value, typeof(T), culture));
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return string.Empty;
            if (destinationType == typeof(string))
                return value.ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
