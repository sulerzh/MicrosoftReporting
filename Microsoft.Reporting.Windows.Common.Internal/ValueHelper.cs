using Microsoft.Reporting.Common.Toolkit.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public static class ValueHelper
    {
        public const double Radian = 0.0174532925199433;

        public static bool CanGraph(double value)
        {
            if (!value.IsNaN())
                return !double.IsInfinity(value);
            return false;
        }

        public static bool CanGraph(float value)
        {
            if (!float.IsNaN(value))
                return !float.IsInfinity(value);
            return false;
        }

        public static bool IsNaNOrInfinity(object value)
        {
            if (value is double)
                return !ValueHelper.CanGraph((double)value);
            if (value is float)
                return !ValueHelper.CanGraph((float)value);
            return false;
        }

        public static bool IsNullNanOrInfinity(this object value)
        {
            if (value == null)
                return true;
            return ValueHelper.IsNaNOrInfinity(value);
        }

        public static bool IsNumericFloat(object value)
        {
            IConvertible convertible = value as IConvertible;
            if (convertible == null)
                return false;
            return ValueHelper.IsNumericFloat(convertible.GetTypeCode());
        }

        public static bool IsNumericInteger(object value)
        {
            IConvertible convertible = value as IConvertible;
            if (convertible == null)
                return false;
            return ValueHelper.IsNumericInteger(convertible.GetTypeCode());
        }

        public static bool IsNumeric(object value)
        {
            IConvertible convertible = value as IConvertible;
            if (convertible == null)
                return false;
            return ValueHelper.IsNumeric(convertible.GetTypeCode());
        }

        private static bool IsNumericInteger(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsNumericFloat(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsNumeric(TypeCode typeCode)
        {
            if (!ValueHelper.IsNumericInteger(typeCode))
                return ValueHelper.IsNumericFloat(typeCode);
            return true;
        }

        public static bool TryConvert(object value, out double doubleValue)
        {
            return ValueHelper.TryConvert(value, false, out doubleValue);
        }

        public static bool TryConvert(object value, bool convertFromString, out double doubleValue)
        {
            doubleValue = 0.0;
            try
            {
                IConvertible convertible = value as IConvertible;
                if (convertible != null)
                {
                    TypeCode typeCode = convertible.GetTypeCode();
                    if (ValueHelper.IsNumeric(typeCode))
                    {
                        doubleValue = convertible.ToDouble(CultureInfo.InvariantCulture);
                        return true;
                    }
                    if (convertFromString)
                    {
                        if (typeCode == TypeCode.String)
                            return double.TryParse((string)value, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleValue);
                    }
                }
            }
            catch (FormatException ex)
            {
            }
            catch (InvalidCastException ex)
            {
            }
            return false;
        }

        public static double ToDouble(object value)
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        public static double ToDoubleWithNull(IComparable value)
        {
            if (value == null)
                return 0.0;
            if (((IConvertible)value).GetTypeCode() == TypeCode.Double)
                return (double)value;
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        public static bool TryConvert(object value, out long intValue)
        {
            return ValueHelper.TryConvert(value, false, out intValue);
        }

        public static bool TryConvert(object value, bool convertFromString, out long intValue)
        {
            intValue = 0L;
            try
            {
                IConvertible convertible = value as IConvertible;
                if (convertible != null)
                {
                    TypeCode typeCode = convertible.GetTypeCode();
                    if (ValueHelper.IsNumeric(typeCode))
                    {
                        intValue = convertible.ToInt64(CultureInfo.InvariantCulture);
                        return true;
                    }
                    if (convertFromString)
                    {
                        if (typeCode == TypeCode.String)
                            return long.TryParse((string)value, NumberStyles.Any, CultureInfo.InvariantCulture, out intValue);
                    }
                }
            }
            catch (FormatException ex)
            {
            }
            catch (InvalidCastException ex)
            {
            }
            catch (OverflowException ex)
            {
            }
            return false;
        }

        public static long ToInt64(object value)
        {
            return Convert.ToInt64(value, CultureInfo.InvariantCulture);
        }

        public static bool TryConvert(object value, out DateTime dateTimeValue)
        {
            return ValueHelper.TryConvert(value, false, out dateTimeValue);
        }

        public static bool TryConvert(object value, bool convertFromString, out DateTime dateTimeValue)
        {
            dateTimeValue = new DateTime();
            try
            {
                IConvertible convertible = value as IConvertible;
                if (convertible != null)
                {
                    TypeCode typeCode = convertible.GetTypeCode();
                    if (typeCode == TypeCode.DateTime)
                    {
                        dateTimeValue = convertible.ToDateTime(CultureInfo.InvariantCulture);
                        return true;
                    }
                    if (convertFromString)
                    {
                        if (typeCode == TypeCode.String)
                            return DateTime.TryParse((string)value, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeValue);
                    }
                }
            }
            catch (FormatException ex)
            {
            }
            catch (InvalidCastException ex)
            {
            }
            catch (OverflowException ex)
            {
            }
            return false;
        }

        public static DateTime ToDateTime(object value)
        {
            return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
        }

        public static TimeSpan ToTimeSpan(object value)
        {
            if (value is TimeSpan)
                return (TimeSpan)value;
            string input = value as string;
            if (input != null)
                return TimeSpan.Parse(input, CultureInfo.InvariantCulture);
            throw new InvalidCastException();
        }

        public static bool CheckUniformDataType<T>(ObservableCollection<T> collection, DependencyProperty property)
        {
            DataValueType dataValueType1 = DataValueType.Auto;
            foreach (T obj1 in collection)
            {
                DependencyObject dependencyObject = (object)obj1 as DependencyObject;
                if (dependencyObject != null)
                {
                    object obj2 = dependencyObject.GetValue(property);
                    if (obj2 != null)
                    {
                        DataValueType dataValueType2 = ValueHelper.GetDataValueType(obj2);
                        if (dataValueType1 == DataValueType.Auto)
                            dataValueType1 = dataValueType2;
                        else if (dataValueType1 != dataValueType2)
                            return false;
                    }
                }
            }
            return true;
        }

        public static DataValueType GetDataValueType(object value)
        {
            IConvertible convertible = value as IConvertible;
            if (convertible == null)
                return DataValueType.Category;
            switch (convertible.GetTypeCode())
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.String:
                    return DataValueType.Category;
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return DataValueType.Integer;
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return DataValueType.Float;
                case TypeCode.DateTime:
                    return DataValueType.DateTime;
                default:
                    if (value is DateTimeOffset)
                        return DataValueType.DateTimeOffset;
                    return value is TimeSpan ? DataValueType.TimeSpan : DataValueType.Category;
            }
        }

        public static DataValueType GetDataValueType(this IEnumerable<object> values)
        {
            foreach (object obj in values)
            {
                if (obj != null)
                {
                    DataValueType dataValueType = ValueHelper.GetDataValueType(obj);
                    if (dataValueType != DataValueType.Auto)
                        return dataValueType;
                }
            }
            return DataValueType.Auto;
        }

        public static DataValueType GetDataValueType(Range<IComparable> range)
        {
            if (!range.HasData)
                return DataValueType.Auto;
            return ValueHelper.CombineDataValueTypes(ValueHelper.GetDataValueType(range.Minimum), ValueHelper.GetDataValueType(range.Maximum));
        }

        public static TypeCode? GetDataTypeCode(object value)
        {
            IConvertible convertible = value as IConvertible;
            if (convertible == null)
                return new TypeCode?();
            return new TypeCode?(convertible.GetTypeCode());
        }

        public static IComparable AddComparableValues(IComparable value1, IComparable value2)
        {
            switch (ValueHelper.GetDataValueType(value1))
            {
                case DataValueType.Integer:
                    return Convert.ToInt64(value1, CultureInfo.InvariantCulture) + Convert.ToInt64(value2, CultureInfo.InvariantCulture);
                case DataValueType.Float:
                    if (value1 is Decimal || value2 is Decimal)
                        return Convert.ToDecimal(value1, CultureInfo.InvariantCulture) + Convert.ToDecimal(value2, CultureInfo.InvariantCulture);
                    return Convert.ToDouble(value1, CultureInfo.InvariantCulture) + Convert.ToDouble(value2, CultureInfo.InvariantCulture);
                case DataValueType.DateTime:
                case DataValueType.Date:
                    return (((DateTime)value1) + (((DateTime)value2) - ((DateTime)value1)));
                case DataValueType.TimeSpan:
                    return (TimeSpan)value1 + (TimeSpan)value2;
                case DataValueType.DateTimeOffset:
                    return (((DateTimeOffset)value1) + (((DateTimeOffset)value2) - ((DateTimeOffset)value1)));
                default:
                    throw new ArgumentException();
            }
        }

        public static IComparable GetZeroValue(object value)
        {
            IConvertible convertible = value as IConvertible;
            if (convertible == null)
                return null;
            switch (convertible.GetTypeCode())
            {
                case TypeCode.SByte:
                    return (sbyte)0;
                case TypeCode.Byte:
                    return (byte)0;
                case TypeCode.Int16:
                    return (short)0;
                case TypeCode.UInt16:
                    return (ushort)0;
                case TypeCode.Int32:
                    return 0;
                case TypeCode.UInt32:
                    return 0U;
                case TypeCode.Int64:
                    return 0L;
                case TypeCode.UInt64:
                    return 0UL;
                case TypeCode.Single:
                    return 0.0f;
                case TypeCode.Double:
                    return 0.0;
                case TypeCode.Decimal:
                    return new Decimal(0, 0, 0, false, 1);
                default:
                    return null;
            }
        }

        public static DataValueType CombineDataValueTypes(DataValueType a, DataValueType b)
        {
            if (a == b)
                return a;
            if (a == DataValueType.Auto)
                return b;
            if (b == DataValueType.Auto)
                return a;
            if (a == DataValueType.Category || b == DataValueType.Category)
                return DataValueType.Category;
            if (a == DataValueType.Float)
                return b == DataValueType.Integer ? DataValueType.Float : DataValueType.Category;
            if (a == DataValueType.Integer)
            {
                if (b == DataValueType.Float)
                    return DataValueType.Float;
            }
            else if (a == DataValueType.Date)
            {
                if (b == DataValueType.DateTime)
                    return DataValueType.DateTime;
                if (b == DataValueType.DateTimeOffset)
                    return DataValueType.DateTimeOffset;
                if (b == DataValueType.Time)
                    return DataValueType.DateTime;
            }
            else if (a == DataValueType.DateTime)
            {
                if (b == DataValueType.Date)
                    return DataValueType.DateTime;
                if (b == DataValueType.DateTimeOffset)
                    return DataValueType.DateTimeOffset;
                if (b == DataValueType.Time)
                    return DataValueType.DateTime;
            }
            else if (a == DataValueType.DateTimeOffset)
            {
                if (b == DataValueType.Date || b == DataValueType.DateTime || b == DataValueType.Time)
                    return DataValueType.DateTimeOffset;
            }
            else if (a == DataValueType.Time)
            {
                if (b == DataValueType.Date || b == DataValueType.DateTime)
                    return DataValueType.DateTime;
                if (b == DataValueType.DateTimeOffset)
                    return DataValueType.DateTimeOffset;
            }
            return DataValueType.Category;
        }

        public static IEnumerable<DateTime> GetDateTimesBetweenInclusive(DateTime start, DateTime end, long count)
        {
            return ValueHelper.GetIntervalsInclusive(start.Ticks, end.Ticks, count).Select<long, DateTime>(value => new DateTime(value));
        }

        public static IEnumerable<TimeSpan> GetTimeSpanIntervalsInclusive(TimeSpan timeSpan, long count)
        {
            return ValueHelper.GetIntervalsInclusive(0L, timeSpan.Ticks, count).Select<long, TimeSpan>(value => new TimeSpan(value));
        }

        public static object ConvertValue(object value, DataValueType valueType)
        {
            object obj = value;
            try
            {
                if (valueType == DataValueType.Float)
                {
                    double doubleValue;
                    if (ValueHelper.TryConvert(value, true, out doubleValue))
                        obj = doubleValue;
                }
                else if (valueType == DataValueType.Integer)
                {
                    long intValue;
                    if (ValueHelper.TryConvert(value, true, out intValue))
                        obj = intValue;
                }
                else if (valueType == DataValueType.Date || valueType == DataValueType.DateTime || valueType == DataValueType.Time)
                {
                    DateTime dateTimeValue;
                    if (ValueHelper.TryConvert(value, true, out dateTimeValue))
                        obj = dateTimeValue;
                }
                else
                {
                    string input = value as string;
                    if (input != null)
                    {
                        if (valueType == DataValueType.DateTimeOffset)
                            obj = DateTimeOffset.Parse(input, CultureInfo.InvariantCulture);
                        else if (valueType == DataValueType.TimeSpan)
                            obj = TimeSpan.Parse(input, CultureInfo.InvariantCulture);
                    }
                }
            }
            catch (FormatException ex)
            {
            }
            catch (InvalidCastException ex)
            {
            }
            return obj;
        }

        public static IEnumerable<object> ConvertValues(IEnumerable<object> values, DataValueType dataType)
        {
            foreach (object obj in values)
                yield return ValueHelper.ConvertValue(obj, dataType);
        }

        public static IEnumerable<long> GetIntervalsInclusive(long start, long end, long count)
        {
            long interval = end - start;
            for (long index = 0; index < count; ++index)
            {
                double ratio = index / (double)(count - 1L);
                long value = (long)(ratio * interval + start);
                yield return value;
            }
        }

        public static int Compare(IComparable left, IComparable right)
        {
            if (left == right)
                return 0;
            if (left == null)
                return -1;
            if (right == null)
                return 1;
            if (left.GetType() == right.GetType())
                return left.CompareTo(right);
            if (ValueHelper.IsNumeric(left) && ValueHelper.IsNumeric(right))
                return ValueHelper.CompareNumericsOfDifferentTypes(left, right);
            return string.CompareOrdinal(left.ToString(), right.ToString());
        }

        private static int CompareNumericsOfDifferentTypes(IComparable left, IComparable right)
        {
            left = ValueHelper.ConvertToDecimalOrDouble(left);
            right = ValueHelper.ConvertToDecimalOrDouble(right);
            if (left.GetType() == right.GetType())
                return left.CompareTo(right);
            if (left is Decimal)
                return ValueHelper.CompareDecimalToDouble((Decimal)left, (double)right);
            return -ValueHelper.CompareDecimalToDouble((Decimal)right, (double)left);
        }

        public static IComparable ConvertToDecimalOrDouble(IComparable value)
        {
            if (ValueHelper.IsNumericInteger(value))
                return (IComparable)Convert.ChangeType(value, typeof(Decimal), CultureInfo.InvariantCulture);
            if (value is float)
                return (IComparable)Convert.ChangeType(value, typeof(double), CultureInfo.InvariantCulture);
            return value;
        }

        private static int CompareDecimalToDouble(Decimal decimalValue, double doubleValue)
        {
            if (doubleValue < -7.92281625142643E+28)
                return 1;
            if (doubleValue > 7.92281625142643E+28)
                return -1;
            return -doubleValue.CompareTo((double)Convert.ChangeType(decimalValue, typeof(double), CultureInfo.InvariantCulture));
        }

        public static bool AreEqual(object left, object right)
        {
            if (left == right)
                return true;
            if (left == null || right == null)
                return false;
            return left.Equals(right);
        }

        public static Point Translate(this Point origin, Point offset)
        {
            return new Point(origin.X + offset.X, origin.Y + offset.Y);
        }

        public static void SetStyle(this FrameworkElement element, Style style)
        {
            element.Style = style;
        }

        public static double? FindNonEmpty(params double?[] values)
        {
            for (int index = 0; index < values.Length; ++index)
            {
                double? nullable = values[index];
                if (nullable.HasValue)
                    return nullable;
            }
            return new double?();
        }

        public static Thickness Inflate(this Thickness margin, Thickness value)
        {
            margin.Left = Math.Max(0.0, margin.Left + value.Left);
            margin.Right = Math.Max(0.0, margin.Right + value.Right);
            margin.Bottom = Math.Max(0.0, margin.Bottom + value.Bottom);
            margin.Top = Math.Max(0.0, margin.Top + value.Top);
            return margin;
        }

        public static Thickness Union(this Thickness margin, Thickness value)
        {
            margin.Left = Math.Max(margin.Left, value.Left);
            margin.Right = Math.Max(margin.Right, value.Right);
            margin.Bottom = Math.Max(margin.Bottom, value.Bottom);
            margin.Top = Math.Max(margin.Top, value.Top);
            return margin;
        }

        public static double Width(this Thickness margin)
        {
            return margin.Left + margin.Right;
        }

        public static double Height(this Thickness margin)
        {
            return margin.Top + margin.Bottom;
        }

        public static string PrepareFormatString(string format)
        {
            if (string.IsNullOrEmpty(format))
                return "{0}";
            if (format.Contains("{0"))
                return format;
            return "{0:" + format + "}";
        }

        public static bool CompareBrushes(Brush brush1, Brush brush2)
        {
            if (brush1 == brush2)
                return true;
            SolidColorBrush solidColorBrush1 = brush1 as SolidColorBrush;
            SolidColorBrush solidColorBrush2 = brush2 as SolidColorBrush;
            return solidColorBrush1 != null && solidColorBrush2 != null && (solidColorBrush1.Color == solidColorBrush2.Color && solidColorBrush1.Opacity == solidColorBrush2.Opacity);
        }

        public static bool CompareEffects(Effect effect1, Effect effect2)
        {
            if (effect1 == effect2)
                return true;
            DropShadowEffect dropShadowEffect1 = effect1 as DropShadowEffect;
            DropShadowEffect dropShadowEffect2 = effect2 as DropShadowEffect;
            if (dropShadowEffect1 != null && dropShadowEffect2 != null && (dropShadowEffect1.BlurRadius == dropShadowEffect2.BlurRadius && dropShadowEffect1.Color == dropShadowEffect2.Color) && (dropShadowEffect1.Direction == dropShadowEffect2.Direction && dropShadowEffect1.Opacity == dropShadowEffect2.Opacity && dropShadowEffect1.ShadowDepth == dropShadowEffect2.ShadowDepth))
                return true;
            BlurEffect blurEffect1 = effect1 as BlurEffect;
            BlurEffect blurEffect2 = effect2 as BlurEffect;
            return blurEffect1 != null && blurEffect2 != null && blurEffect1.Radius != blurEffect2.Radius;
        }

        internal static AutoBool And(AutoBool value, AutoBool other)
        {
            if (value == AutoBool.Auto)
                return other;
            if (other == AutoBool.Auto)
                return value;
            return value != AutoBool.True || other != AutoBool.True ? AutoBool.False : AutoBool.True;
        }

        public static AutoBool Or(AutoBool value, AutoBool other)
        {
            if (value == AutoBool.Auto)
                return other;
            if (other == AutoBool.Auto)
                return value;
            return value != AutoBool.True && other != AutoBool.True ? AutoBool.False : AutoBool.True;
        }

        public static bool ToBoolean(AutoBool value, bool defaultValue)
        {
            switch (value)
            {
                case AutoBool.True:
                    return true;
                case AutoBool.False:
                    return false;
                default:
                    return defaultValue;
            }
        }

        public static Color ColorFromHexString(string colorString)
        {
            Color color = new Color();
            byte result = 0;
            if (byte.TryParse(colorString.Substring(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
                color.A = result;
            if (byte.TryParse(colorString.Substring(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
                color.R = result;
            if (byte.TryParse(colorString.Substring(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
                color.G = result;
            if (byte.TryParse(colorString.Substring(7, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
                color.B = result;
            return color;
        }

        public static Brush GetContrastBasedPaletteBrush(Color color)
        {
            if (HighContrastHelper.IsHighContrastOn())
                color = ValueHelper.InvertColor(color);
            return new SolidColorBrush(color);
        }

        public static Color InvertColor(Color color)
        {
            color.R = (byte)(byte.MaxValue - (uint)color.R);
            color.G = (byte)(byte.MaxValue - (uint)color.G);
            color.B = (byte)(byte.MaxValue - (uint)color.B);
            return color;
        }

        public static Visibility ToVisibility(this bool value)
        {
            return !value ? Visibility.Collapsed : Visibility.Visible;
        }

        public static IEnumerable<T> Top<T>(this IEnumerable<T> data, int count)
        {
            return data.Take<T>(count);
        }

        public static IEnumerable<T> Sample<T>(this IList<T> data, int count)
        {
            if (count != 0 && data.Count != 0)
            {
                if (count == 1)
                    yield return data[0];
                else if (count >= data.Count)
                {
                    foreach (T obj in data)
                        yield return obj;
                }
                else
                {
                    double step = (data.Count - 1) / (double)(count - 1);
                    for (int i = 0; i < count; ++i)
                    {
                        int index = (int)Math.Round(i * step);
                        yield return data[index];
                    }
                }
            }
        }

        public static IEnumerable<T> Limit<T>(this IList<T> data, int count, bool isScalar)
        {
            if (isScalar)
                return data.Sample<T>(count);
            return data.Top<T>(count);
        }

        public static void ApplyLimit<T>(this IList<T> list, IList<T> filteredList) where T : class
        {
            if (list.Count == filteredList.Count)
                return;
            if (filteredList.Count > 0 && list.Count - filteredList.Count < 3)
            {
                int index = filteredList.Count - 1;
                if (list[index] == filteredList[index])
                {
                    while (list.Count > filteredList.Count)
                        list.RemoveAt(index + 1);
                    return;
                }
            }
            list.Clear();
            foreach (T filtered in filteredList)
                list.Add(filtered);
        }

        public static int EnsureInRange(this int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public static string ConvertToString(this object val, string nullValue = null)
        {
            if (val == null)
                return nullValue;
            return val.ToString();
        }

        public static bool HasPositivesAndNegatives(IEnumerable<object> values)
        {
            DataValueType dataValueType = values.GetDataValueType();
            ValueAggregator positiveAggregator = ValueAggregator.GetPositiveAggregator(dataValueType);
            ValueAggregator negativeAggregator = ValueAggregator.GetNegativeAggregator(dataValueType);
            bool flag1 = false;
            bool flag2 = false;
            foreach (object obj in values)
            {
                bool flag3 = positiveAggregator.CanPlot(obj);
                bool flag4 = negativeAggregator.CanPlot(obj);
                if (flag3 != flag4)
                {
                    if (flag3)
                        flag2 = true;
                    else
                        flag1 = true;
                    if (flag2 && flag1)
                        return true;
                }
            }
            return false;
        }
    }
}
