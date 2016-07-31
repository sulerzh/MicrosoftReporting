using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public abstract class ValueAggregator
    {
        public abstract bool CanPlot(object value);

        public abstract IComparable GetValue(object value);

        public abstract Range<IComparable> GetRange(IEnumerable<object> values);

        public abstract Range<IComparable> GetSumRange(IEnumerable<object> values);

        public static ValueAggregator GetAggregator(DataValueType valueType)
        {
            switch (valueType)
            {
                case DataValueType.Auto:
                    return new DefaultValueAggregator();
                case DataValueType.Category:
                    return new CategoryValueAggregator();
                case DataValueType.Integer:
                    return new Int64ValueAggregator();
                case DataValueType.Float:
                    return new DoubleValueAggregator();
                case DataValueType.DateTime:
                case DataValueType.Date:
                    return new DateTimeValueAggregator();
                case DataValueType.Time:
                case DataValueType.TimeSpan:
                    return new TimeValueAggregator();
                case DataValueType.DateTimeOffset:
                    return new DateTimeOffsetValueAggregator();
                default:
                    throw new NotSupportedException();
            }
        }

        public static ValueAggregator GetPositiveAggregator(DataValueType valueType)
        {
            switch (valueType)
            {
                case DataValueType.Integer:
                    return new PositiveInt64ValueAggregator();
                case DataValueType.Float:
                    return new PositiveDoubleValueAggregator();
                case DataValueType.Time:
                case DataValueType.TimeSpan:
                    return new PositiveTimeValueAggregator();
                default:
                    return ValueAggregator.GetAggregator(valueType);
            }
        }

        public static ValueAggregator GetNegativeAggregator(DataValueType valueType)
        {
            switch (valueType)
            {
                case DataValueType.Integer:
                    return new NegativeInt64ValueAggregator();
                case DataValueType.Float:
                    return new NegativeDoubleValueAggregator();
                case DataValueType.Time:
                case DataValueType.TimeSpan:
                    return new NegativeTimeValueAggregator();
                default:
                    return ValueAggregator.GetAggregator(valueType);
            }
        }

        public static ValueAggregator GetAbsAggregator(DataValueType valueType)
        {
            switch (valueType)
            {
                case DataValueType.Integer:
                    return new AbsInt64ValueAggregator();
                case DataValueType.Float:
                    return new AbsDoubleValueAggregator();
                case DataValueType.Time:
                case DataValueType.TimeSpan:
                    return new AbsTimeValueAggregator();
                default:
                    return ValueAggregator.GetAggregator(valueType);
            }
        }
    }
}
