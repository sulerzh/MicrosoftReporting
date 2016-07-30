using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    [DebuggerDisplay("{M} E{E}")]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DoubleR10 : IEquatable<DoubleR10>
    {
        private static readonly long[] Powers = new long[19]
        {
            1L,
            10L,
            100L,
            1000L,
            10000L,
            100000L,
            1000000L,
            10000000L,
            100000000L,
            1000000000L,
            10000000000L,
            100000000000L,
            1000000000000L,
            10000000000000L,
            100000000000000L,
            1000000000000000L,
            10000000000000000L,
            100000000000000000L,
            1000000000000000000L
        };
        public static DoubleR10 MaxValue = DoubleR10.NewUncheckedDouble10(999999999999999999L, 1000000000);
        public static DoubleR10 MinValue = DoubleR10.NewUncheckedDouble10(-999999999999999999L, 1000000000);
        public static DoubleR10 Zero = DoubleR10.NewUncheckedDouble10(0L, 0);
        public static DoubleR10 NaN = DoubleR10.NewUncheckedDouble10(long.MaxValue, 2000000000);
        public static DoubleR10 PositiveInfinity = DoubleR10.NewUncheckedDouble10(9223372036854775806L, 2000000000);
        public static DoubleR10 NegativeInfinity = DoubleR10.NewUncheckedDouble10(-9223372036854775807L, 2000000000);
        private const int DecimalDigits = 18;
        private const int DecimalDigitsMinus = -18;
        private const int DecimalDigitsPlusOne = 19;
        private const int DecimalDigitsX2 = 36;
        private const int DecimalDigitsX2Minus = -36;
        private const int DecimalDigitsHalf = 9;
        private const int DoubleCastingPrecisionDigits = 14;
        private const long MaxM = 999999999999999999;
        private const long MinM = -999999999999999999;
        public const int MaxE = 1000000000;
        public const int MinE = -1000000000;
        private const int SpecialE = 2000000000;
        private const long MaxPower = 1000000000000000000;
        private const long MaxPowerMinus = -1000000000000000000;
        private const long MaxPowerHalf = 1000000000;
        private const long MaxPowerHalfMinus = -1000000000;
        private const long NaNM = 9223372036854775807;
        private const long PositiveInfinityM = 9223372036854775806;
        private const long NegativeInfinityM = -9223372036854775807;
        public long M;
        public int E;

        public DoubleR10(long m, int e)
        {
            this.M = m;
            this.E = e;
            if (this.M >= -999999999999999999L && this.M <= 999999999999999999L)
                return;
            this.M = this.M / 10L;
            ++this.E;
            if (this.E <= 1000000000)
                return;
            if (this.M > 0L)
                this = DoubleR10.PositiveInfinity;
            else
                this = DoubleR10.NegativeInfinity;
        }

        public DoubleR10(long value)
        {
            this.M = value;
            this.E = 0;
            if (this.M >= -999999999999999999L && this.M <= 999999999999999999L)
                return;
            this.M = this.M / 10L;
            this.E = 1;
        }

        public DoubleR10(int value)
        {
            this.M = value;
            this.E = 0;
        }

        public DoubleR10(double value)
        {
            if (value > 0.0)
            {
                long num1 = (long)value;
                if (value < 1E+18 && num1 == value)
                {
                    this.M = num1;
                    this.E = 0;
                }
                else
                {
                    if (value < 100000000000000.0)
                    {
                        double num2 = value * 10000.0;
                        long num3 = (long)num2;
                        if (num3 == num2)
                        {
                            this.M = num3;
                            this.E = -4;
                            return;
                        }
                    }
                    if (value <= double.MaxValue)
                    {
                        int exp = DoubleHelper.Log10(value) - 14;
                        double num2 = DoubleHelper.Pow10(exp);
                        this.M = (long)Math.Round(value / num2);
                        this.E = exp;
                    }
                    else
                        this = DoubleR10.PositiveInfinity;
                }
            }
            else if (value < 0.0)
            {
                long num1 = (long)value;
                if (value > -1E+18 && num1 == value)
                {
                    this.M = num1;
                    this.E = 0;
                }
                else
                {
                    if (value > -100000000000000.0)
                    {
                        double num2 = value * 10000.0;
                        long num3 = (long)num2;
                        if (num3 == num2)
                        {
                            this.M = num3;
                            this.E = -4;
                            return;
                        }
                    }
                    if (value >= double.MinValue)
                    {
                        int exp = DoubleHelper.Log10(-value) - 14;
                        double num2 = DoubleHelper.Pow10(exp);
                        this.M = (long)Math.Round(value / num2);
                        this.E = exp;
                    }
                    else
                        this = DoubleR10.NegativeInfinity;
                }
            }
            else if (value == 0.0)
                this = DoubleR10.Zero;
            else
                this = DoubleR10.NaN;
        }

        public static implicit operator DoubleR10(double x)
        {
            return new DoubleR10(x);
        }

        public static implicit operator DoubleR10(Decimal x)
        {
            return new DoubleR10((double)x);
        }

        public static implicit operator DoubleR10(long x)
        {
            return new DoubleR10(x);
        }

        public static implicit operator DoubleR10(int x)
        {
            return new DoubleR10(x);
        }

        public static explicit operator double(DoubleR10 x)
        {
            if (x.E == 0)
                return x.M;
            if (x.E == -4)
                return Math.Round(x.M * 0.0001, 4);
            if (x.E == 2000000000)
            {
                if (x.M == long.MaxValue)
                    return double.NaN;
                return x.M == 9223372036854775806L ? double.PositiveInfinity : double.NegativeInfinity;
            }
            if (x.E > 290)
            {
                if (x.E == 294)
                {
                    if (x.M == 179769313486232L)
                        return double.MaxValue;
                    if (x.M == -179769313486232L)
                        return double.MinValue;
                }
                if (x.E == 293)
                {
                    if (x.M == 1797693134862316L)
                        return double.MaxValue;
                    if (x.M == -1797693134862316L)
                        return double.MinValue;
                }
                if (x.E == 292)
                {
                    if (x.M == 17976931348623160L)
                        return double.MaxValue;
                    if (x.M == -17976931348623160L)
                        return double.MinValue;
                }
                if (x.E == 291)
                {
                    if (x.M == 179769313486231600L)
                        return double.MaxValue;
                    if (x.M == -179769313486231600L)
                        return double.MinValue;
                }
            }
            x.Normalize();
            double num = x.M * DoubleHelper.Pow10(x.E);
            if (0 > x.E && x.E > -16)
                return Math.Round(num, -x.E);
            return num;
        }

        public static explicit operator long(DoubleR10 x)
        {
            if (x.E == 0 || x.M == 0L)
                return x.M;
            if (x.E > 0)
            {
                if (x.E < 19)
                    return x.M * DoubleR10.Powers[x.E];
                return x.M > 0L ? long.MaxValue : long.MinValue;
            }
            int index = -x.E;
            if (index > 18)
                return 0;
            return x.M / DoubleR10.Powers[index];
        }

        public static explicit operator int(DoubleR10 x)
        {
            return (int)(long)x;
        }

        public static explicit operator Decimal(DoubleR10 x)
        {
            return (Decimal)(double)x;
        }

        public static DoubleR10 operator +(DoubleR10 x1, DoubleR10 x2)
        {
            int shift1 = x1.E - x2.E;
            if (shift1 == 0 && x1.E != 2000000000)
                return new DoubleR10(x1.M + x2.M, x1.E);
            if (x1.M == 0L)
                return x2;
            if (x2.M == 0L)
                return x1;
            if (x1.E == 2000000000 || x2.E == 2000000000)
                return DoubleR10.SpecialAdditionCases(x1, x2);
            if (shift1 >= 36)
                return x1;
            if (shift1 <= -36)
                return x2;
            if (shift1 > 0)
            {
                int shift2 = 17 - x1.MLog10();
                if (shift2 < shift1)
                {
                    x1.ShiftL(shift2);
                    int shift3 = shift1 - shift2;
                    if (shift3 != 0)
                    {
                        if (shift3 >= 18)
                            return x1;
                        x2.ShiftR(shift3);
                    }
                }
                else
                    x1.ShiftL(shift1);
                return new DoubleR10(x1.M + x2.M, x1.E);
            }
            if (shift1 >= 0)
                return DoubleR10.NaN;
            int shift4 = -shift1;
            int shift5 = 17 - x2.MLog10();
            if (shift5 < shift4)
            {
                x2.ShiftL(shift5);
                int shift2 = shift4 - shift5;
                if (shift2 != 0)
                {
                    if (shift2 >= 18)
                        return x2;
                    x1.ShiftR(shift2);
                }
            }
            else
                x2.ShiftL(shift4);
            return new DoubleR10(x1.M + x2.M, x2.E);
        }

        public static DoubleR10 operator -(DoubleR10 x1, DoubleR10 x2)
        {
            int shift1 = x1.E - x2.E;
            if (shift1 == 0 && x1.E != 2000000000)
                return new DoubleR10(x1.M - x2.M, x1.E);
            if (x1.M == 0L)
                return -x2;
            if (x2.M == 0L)
                return x1;
            if (x1.E == 2000000000 || x2.E == 2000000000)
                return DoubleR10.SpecialSubtractionCases(x1, x2);
            if (shift1 >= 36)
                return x1;
            if (shift1 <= -36)
                return -x2;
            if (shift1 > 0)
            {
                int shift2 = 17 - x1.MLog10();
                if (shift2 < shift1)
                {
                    x1.ShiftL(shift2);
                    int shift3 = shift1 - shift2;
                    if (shift3 != 0)
                    {
                        if (shift3 >= 18)
                            return x1;
                        x2.ShiftR(shift3);
                    }
                }
                else
                    x1.ShiftL(shift1);
                return new DoubleR10(x1.M - x2.M, x1.E);
            }
            if (shift1 >= 0)
                return DoubleR10.NaN;
            int shift4 = -shift1;
            int shift5 = 17 - x2.MLog10();
            if (shift5 < shift4)
            {
                x2.ShiftL(shift5);
                int shift2 = shift4 - shift5;
                if (shift2 != 0)
                {
                    if (shift2 >= 18)
                        return -x2;
                    x1.ShiftR(shift2);
                }
            }
            else
                x2.ShiftL(shift4);
            return new DoubleR10(x1.M - x2.M, x2.E);
        }

        public static DoubleR10 operator -(DoubleR10 x)
        {
            if (x.E != 2000000000)
                return DoubleR10.NewUncheckedDouble10(-x.M, x.E);
            if (x.M == long.MaxValue)
                return DoubleR10.NaN;
            if (x.M == 9223372036854775806L)
                return DoubleR10.NegativeInfinity;
            return DoubleR10.PositiveInfinity;
        }

        public static DoubleR10 operator *(DoubleR10 x1, DoubleR10 x2)
        {
            if (x1.M == 0L || x2.M == 0L || (x1.E == 2000000000 || x2.E == 2000000000))
                return DoubleR10.SpecialMultiplicationCases(x1, x2);
            if (x1.M > 1000000000L || x1.M < -1000000000L || (x2.M > 1000000000L || x2.M < -1000000000L))
            {
                int num1 = x1.MLog10() + 1;
                int num2 = x2.MLog10() + 1;
                int shift1 = num1 + num2 - 18;
                if (shift1 > 0)
                {
                    int num3 = num1 - num2;
                    if (num3 == 0)
                    {
                        int num4 = shift1 / 2;
                        int shift2 = num4;
                        int shift3 = num4;
                        if (shift1 % 2 == 1)
                            ++shift2;
                        x1.ShiftR_WithRounding(shift2);
                        x2.ShiftR_WithRounding(shift3);
                    }
                    if (num3 > 0)
                    {
                        if (num3 > shift1)
                        {
                            x1.ShiftR(shift1);
                        }
                        else
                        {
                            shift1 -= num3;
                            int num4 = shift1 / 2;
                            int shift2 = num3 + num4;
                            int shift3 = num4;
                            if (shift1 % 2 == 1)
                                ++shift2;
                            x1.ShiftR_WithRounding(shift2);
                            x2.ShiftR_WithRounding(shift3);
                        }
                    }
                    if (num3 < 0)
                    {
                        int num4 = -num3;
                        if (num4 > shift1)
                        {
                            x2.ShiftR(shift1);
                        }
                        else
                        {
                            int num5 = shift1 - num4;
                            int num6 = num5 / 2;
                            int shift2 = num6;
                            int shift3 = num4 + num6;
                            if (num5 % 2 == 1)
                                ++shift3;
                            x1.ShiftR_WithRounding(shift2);
                            x2.ShiftR_WithRounding(shift3);
                        }
                    }
                }
            }
            long m = x1.M * x2.M;
            int e = x1.E + x2.E;
            if (e > 1000000000)
            {
                if (m < 0L)
                    return DoubleR10.NegativeInfinity;
                if (m > 0L)
                    return DoubleR10.PositiveInfinity;
            }
            else if (e < -1000000000)
                return DoubleR10.Zero;
            return new DoubleR10(m, e);
        }

        public static DoubleR10 operator /(DoubleR10 x1, DoubleR10 x2)
        {
            if (x1.M == 0L || x2.M == 0L || (x1.E == 2000000000 || x2.E == 2000000000))
                return DoubleR10.SpecialDivisionCases(x1, x2);
            if (x1.M % x2.M != 0L)
            {
                int shift1 = 18 - x1.MLog10() - 1;
                if (shift1 > 0)
                    x1.ShiftL(shift1);
                if (x2.M < -1000000000L || x2.M > 1000000000L)
                {
                    int shift2 = x2.MLog10() - 9 + 1;
                    if (shift2 > 0)
                        x2.ShiftR_WithRounding(shift2);
                }
            }
            long m = x1.M / x2.M;
            int e = x1.E - x2.E;
            if (e > 1000000000)
            {
                if (m < 0L)
                    return DoubleR10.NegativeInfinity;
                if (m > 0L)
                    return DoubleR10.PositiveInfinity;
            }
            else if (e < -1000000000)
                return DoubleR10.Zero;
            return new DoubleR10(m, e);
        }

        public static DoubleR10 operator %(DoubleR10 x1, DoubleR10 x2)
        {
            if (x1.E == 2000000000 || x2.E == 2000000000)
            {
                if (x1.M == 9223372036854775806L || x1.M == -9223372036854775807L)
                    return DoubleR10.NaN;
                if (x2.M == 9223372036854775806L || x2.M == -9223372036854775807L)
                    return x1;
            }
            return x1 - x2 * DoubleR10.Trunc(x1 / x2);
        }

        public static bool operator ==(DoubleR10 x1, DoubleR10 x2)
        {
            if (x1.E == x2.E)
                return x1.M == x2.M;
            if (x1.M == 0L && x2.M == 0L)
                return true;
            int num1 = x1.E - x2.E;
            if (num1 >= 36 || num1 <= -36)
                return false;
            int num2 = x1.MLog10();
            int num3 = x2.MLog10();
            if (num2 + x1.E != num3 + x2.E)
                return false;
            int shift = num2 - num3;
            if (shift > 0)
                x2.ShiftL(shift);
            else
                x1.ShiftL(-shift);
            return x1.M == x2.M;
        }

        public static bool operator !=(DoubleR10 x1, DoubleR10 x2)
        {
            return !(x1 == x2);
        }

        public static bool operator <(DoubleR10 x1, DoubleR10 x2)
        {
            if (x1.E == 2000000000 || x2.E == 2000000000)
                return DoubleR10.SpecialLessCases(x1, x2);
            if (x1.M == 0L)
                return x2.M > 0L;
            if (x2.M == 0L)
                return x1.M < 0L;
            if (x1.E == x2.E)
                return x1.M < x2.M;
            if (x1.M < 0L && x2.M > 0L)
                return true;
            if (x1.M > 0L && x2.M < 0L)
                return false;
            int num1 = x1.E - x2.E;
            if (num1 >= 36)
                return x1.M < 0L;
            if (num1 <= -36)
                return x1.M > 0L;
            int num2 = x1.MLog10();
            int num3 = x2.MLog10();
            int num4 = num2 + x1.E;
            int num5 = num3 + x2.E;
            if (num4 != num5)
            {
                if (x1.M > 0L)
                    return num4 < num5;
                return num4 > num5;
            }
            int shift = num2 - num3;
            if (shift > 0)
                x2.ShiftL(shift);
            else
                x1.ShiftL(-shift);
            return x1.M < x2.M;
        }

        public static bool operator <=(DoubleR10 x1, DoubleR10 x2)
        {
            if (!(x1 == x2))
                return x1 < x2;
            return true;
        }

        public static bool operator >(DoubleR10 x1, DoubleR10 x2)
        {
            return !(x1 <= x2);
        }

        public static bool operator >=(DoubleR10 x1, DoubleR10 x2)
        {
            if (x1 == DoubleR10.NaN || x2 == DoubleR10.NaN)
                return x1 == x2;
            return !(x1 < x2);
        }

        public static bool operator <(DoubleR10 x1, int x2)
        {
            if (x2 == 0 || x1.E == 0)
                return x1.M < x2;
            return x1 < (DoubleR10)x2;
        }

        public static bool operator <=(DoubleR10 x1, int x2)
        {
            if (x2 == 0 || x1.E == 0)
                return x1.M <= x2;
            return x1 <= (DoubleR10)x2;
        }

        public static bool operator >(DoubleR10 x1, int x2)
        {
            if (x2 == 0 || x1.E == 0)
                return x1.M > x2;
            return x1 > (DoubleR10)x2;
        }

        public static bool operator >=(DoubleR10 x1, int x2)
        {
            if (x2 == 0 || x1.E == 0)
                return x1.M >= x2;
            return x1 >= (DoubleR10)x2;
        }

        public static bool operator ==(DoubleR10 x1, int x2)
        {
            if (x2 == 0 || x1.E == 0)
                return x1.M == x2;
            return x1 == (DoubleR10)x2;
        }

        public static bool operator !=(DoubleR10 x1, int x2)
        {
            if (x2 == 0 || x1.E == 0)
                return x1.M != x2;
            return !(x1 == (DoubleR10)x2);
        }

        private static DoubleR10 NewUncheckedDouble10(long m, int e)
        {
            return new DoubleR10() { M = m, E = e };
        }

        internal int MLog10()
        {
            if (this.M == 0L)
                return 0;
            long num = Math.Abs(this.M);
            if (num < 10000000000L)
            {
                if (num < 100000L)
                {
                    if (num < 1000L)
                    {
                        if (num >= 100L)
                            return 2;
                        return num < 10L ? 0 : 1;
                    }
                    return num < 10000L ? 3 : 4;
                }
                if (num < 100000000L)
                {
                    if (num >= 10000000L)
                        return 7;
                    return num < 1000000L ? 5 : 6;
                }
                return num < 1000000000L ? 8 : 9;
            }
            if (num < 1000000000000000L)
            {
                if (num < 10000000000000L)
                {
                    if (num >= 1000000000000L)
                        return 12;
                    return num < 100000000000L ? 10 : 11;
                }
                return num < 100000000000000L ? 13 : 14;
            }
            if (num >= 1000000000000000000L)
                return 18;
            if (num >= 100000000000000000L)
                return 17;
            return num < 10000000000000000L ? 15 : 16;
        }

        public int Log10()
        {
            if (this.E == 2000000000)
                return this.E;
            return this.E + this.MLog10();
        }

        public DoubleR10 Power10()
        {
            if (this.M == 0L)
                return 0;
            if (this.E != 2000000000)
                return DoubleR10.NewUncheckedDouble10(DoubleR10.Powers[this.MLog10()], this.E);
            if (this.M == long.MaxValue)
                return DoubleR10.NaN;
            return DoubleR10.PositiveInfinity;
        }

        public bool IsSpecial()
        {
            return this.E == 2000000000;
        }

        internal void ShiftL(int shift)
        {
            this.M = this.M * DoubleR10.Powers[shift];
            this.E -= shift;
        }

        internal void ShiftR(int shift)
        {
            this.M = this.M / DoubleR10.Powers[shift];
            this.E += shift;
        }

        internal void ShiftR_WithRounding(int shift)
        {
            if (shift <= 18)
            {
                long num1 = DoubleR10.Powers[shift];
                long num2 = num1 / 2L;
                long num3 = this.M % num1;
                this.M = this.M / num1;
                if (num3 > num2)
                    ++this.M;
                else if (num3 < -num2)
                    --this.M;
            }
            else if (shift > 19)
                this.M = 0L;
            else if (shift == 19)
                this.M = this.M <= 5000000000000000000L ? 0L : 1L;
            this.E += shift;
        }

        public override string ToString()
        {
            return this.ToString(CultureInfo.CurrentCulture);
        }

        public string ToString(IFormatProvider provider)
        {
            if (this.E == 0)
                return this.M.ToString(provider);
            if (this.E == -4)
                return (this.M / 10000.0).ToString(provider);
            return this.ToString(provider);
        }

        public bool IsDoubleInfinity()
        {
            if (this.M == 0L || this.E < 280)
                return false;
            if (this.E > 309 || this == DoubleR10.PositiveInfinity || this == DoubleR10.NegativeInfinity)
                return true;
            return double.IsInfinity((double)this);
        }

        public static DoubleR10 Round(DoubleR10 value)
        {
            if (value.E >= 0)
                return value;
            if (value.E < -18)
                return DoubleR10.Zero;
            value.ShiftR_WithRounding(-value.E);
            return value;
        }

        public static DoubleR10 Round(DoubleR10 value, int exp)
        {
            int num = value.E - exp;
            if (num >= 0)
                return value;
            if (num < -18)
                return DoubleR10.Zero;
            value.ShiftR_WithRounding(-num);
            return value;
        }

        public static DoubleR10 Trunc(DoubleR10 value)
        {
            if (value.E >= 0)
                return value;
            if (value.E < -18)
                return DoubleR10.Zero;
            long num = DoubleR10.Powers[-value.E];
            value.M = value.M / num;
            value.E = 0;
            return value;
        }

        public static DoubleR10 Floor(DoubleR10 value)
        {
            if (value.E >= 0)
                return value;
            if (value.E < -18)
                return DoubleR10.Zero;
            long num = DoubleR10.Powers[-value.E];
            if (value.M % num != 0L && value.M < 0L)
            {
                value.M = value.M / num;
                --value.M;
            }
            else
                value.M = value.M / num;
            value.E = 0;
            return value;
        }

        public static DoubleR10 Ceiling(DoubleR10 value)
        {
            if (value.E >= 0)
                return value;
            if (value.E < -18)
                return DoubleR10.Zero;
            long num1 = DoubleR10.Powers[-value.E];
            long num2 = value.M % num1;
            value.E = 0;
            value.M = value.M / num1;
            if (num2 > 0L)
                ++value.M;
            return value;
        }

        public static DoubleR10 Floor(DoubleR10 value, int exp)
        {
            if (value.E == 2000000000 || value.M == 0L)
                return value;
            int index = exp - value.E;
            if (index < 0)
                return value;
            if (index > 18)
            {
                if (value.M > 0L)
                    return DoubleR10.Zero;
                return -DoubleR10.Pow10(exp);
            }
            long num = DoubleR10.Powers[index];
            if (value.M % num != 0L && value.M < 0L)
            {
                if (value.M > -1000000000000000000L)
                {
                    value.M = (value.M / num - 1L) * num;
                }
                else
                {
                    value.M = value.M / num - 1L;
                    value.E += index;
                }
            }
            else
                value.M = value.M / num * num;
            return value;
        }

        public static DoubleR10 Ceiling(DoubleR10 value, int exp)
        {
            if (value.E == 2000000000 || value.M == 0L)
                return value;
            int index = exp - value.E;
            if (index < 0)
                return value;
            if (index > 18)
            {
                if (value.M > 0L)
                    return DoubleR10.Pow10(exp);
                return DoubleR10.Zero;
            }
            long num = DoubleR10.Powers[index];
            if (value.M % num != 0L && value.M > 0L)
            {
                if (value.M < 1000000000000000000L)
                {
                    value.M = (value.M / num + 1L) * num;
                }
                else
                {
                    value.M = value.M / num + 1L;
                    value.E += index;
                }
            }
            else
                value.M = value.M / num * num;
            return value;
        }

        public static long Divide(DoubleR10 x1, DoubleR10 x2)
        {
            if (x1.E == x2.E)
                return x1.M / x2.M;
            return (long)(x1 / x2);
        }

        public static DoubleR10 Pow10(int exp)
        {
            return new DoubleR10(1L, exp);
        }

        private static DoubleR10 SpecialAdditionCases(DoubleR10 x1, DoubleR10 x2)
        {
            if (x1.M == long.MaxValue || x2.M == long.MaxValue)
                return DoubleR10.NaN;
            if (x1.M == 9223372036854775806L)
            {
                if (x2.M == -9223372036854775807L)
                    return DoubleR10.NaN;
                return DoubleR10.PositiveInfinity;
            }
            if (x1.M == -9223372036854775807L)
            {
                if (x2.M == 9223372036854775806L)
                    return DoubleR10.NaN;
                return DoubleR10.NegativeInfinity;
            }
            if (x2.M == 9223372036854775806L)
                return DoubleR10.PositiveInfinity;
            if (x2.M == -9223372036854775807L)
                return DoubleR10.NegativeInfinity;
            return DoubleR10.NaN;
        }

        private static DoubleR10 SpecialSubtractionCases(DoubleR10 x1, DoubleR10 x2)
        {
            if (x1.M == long.MaxValue || x2.M == long.MaxValue)
                return DoubleR10.NaN;
            if (x1.M == 9223372036854775806L)
            {
                if (x2.M == -9223372036854775807L)
                    return DoubleR10.NaN;
                return DoubleR10.PositiveInfinity;
            }
            if (x1.M == -9223372036854775807L)
            {
                if (x2.M == 9223372036854775806L)
                    return DoubleR10.NaN;
                return DoubleR10.NegativeInfinity;
            }
            if (x2.M == 9223372036854775806L)
                return DoubleR10.NegativeInfinity;
            if (x2.M == -9223372036854775807L)
                return DoubleR10.PositiveInfinity;
            return DoubleR10.NaN;
        }

        private static DoubleR10 SpecialMultiplicationCases(DoubleR10 x1, DoubleR10 x2)
        {
            if (x1.M == long.MaxValue || x2.M == long.MaxValue)
                return DoubleR10.NaN;
            if (x1.M == 9223372036854775806L)
            {
                if (x2.M == 0L)
                    return DoubleR10.NaN;
                if (x2.M < 0L)
                    return DoubleR10.NegativeInfinity;
                if (x2.M > 0L)
                    return DoubleR10.PositiveInfinity;
            }
            if (x1.M == -9223372036854775807L)
            {
                if (x2.M == 0L)
                    return DoubleR10.NaN;
                if (x2.M < 0L)
                    return DoubleR10.PositiveInfinity;
                if (x2.M > 0L)
                    return DoubleR10.NegativeInfinity;
            }
            if (x2.M == 9223372036854775806L)
            {
                if (x1.M == 0L)
                    return DoubleR10.NaN;
                if (x1.M < 0L)
                    return DoubleR10.NegativeInfinity;
                if (x1.M > 0L)
                    return DoubleR10.PositiveInfinity;
            }
            if (x2.M == -9223372036854775807L)
            {
                if (x1.M == 0L)
                    return DoubleR10.NaN;
                if (x1.M < 0L)
                    return DoubleR10.PositiveInfinity;
                if (x1.M > 0L)
                    return DoubleR10.NegativeInfinity;
            }
            if (x1.M == 0L || x2.M == 0L)
                return DoubleR10.Zero;
            return DoubleR10.NaN;
        }

        private static DoubleR10 SpecialDivisionCases(DoubleR10 x1, DoubleR10 x2)
        {
            if (x1.M == long.MaxValue || x2.M == long.MaxValue)
                return DoubleR10.NaN;
            if (x1.M == 9223372036854775806L)
            {
                if (x2.M == 9223372036854775806L || x2.M == -9223372036854775807L)
                    return DoubleR10.NaN;
                if (x2.M < 0L)
                    return DoubleR10.NegativeInfinity;
                return DoubleR10.PositiveInfinity;
            }
            if (x1.M == -9223372036854775807L)
            {
                if (x2.M == 9223372036854775806L || x2.M == -9223372036854775807L)
                    return DoubleR10.NaN;
                if (x2.M < 0L)
                    return DoubleR10.PositiveInfinity;
                return DoubleR10.NegativeInfinity;
            }
            if (x2.M == 9223372036854775806L || x2.M == -9223372036854775807L)
                return DoubleR10.Zero;
            if (x2.M == 0L)
            {
                if (x1.M < 0L)
                    return DoubleR10.NegativeInfinity;
                if (x1.M == 0L)
                    return DoubleR10.NaN;
                return DoubleR10.PositiveInfinity;
            }
            if (x1.M == 0L)
                return DoubleR10.Zero;
            return DoubleR10.NaN;
        }

        private static bool SpecialLessCases(DoubleR10 x1, DoubleR10 x2)
        {
            if (x1.M == long.MaxValue || x2.M == long.MaxValue || x1.M == 9223372036854775806L && x2.M != 9223372036854775806L)
                return false;
            if (x2.M == 9223372036854775806L && x1.M != 9223372036854775806L || x1.M == -9223372036854775807L && x2.M != -9223372036854775807L)
                return true;
            return x2.M == -9223372036854775807L && x1.M != -9223372036854775807L ? false : false;
        }

        public DoubleR10 Add(ref DoubleR10 x2)
        {
            int shift1 = this.E - x2.E;
            if (shift1 == 0 && this.E < 2000000000)
                return new DoubleR10(this.M + x2.M, this.E);
            if (this.M == 0L)
                return x2;
            if (x2.M == 0L)
                return this;
            if (this.E == 2000000000 || x2.E == 2000000000)
                return DoubleR10.SpecialAdditionCases(this, x2);
            if (shift1 >= 36)
                return this;
            if (shift1 <= -36)
                return x2;
            if (shift1 > 0)
            {
                int shift2 = 17 - this.MLog10();
                if (shift2 < shift1)
                {
                    this.ShiftL(shift2);
                    int shift3 = shift1 - shift2;
                    if (shift3 != 0)
                    {
                        if (shift3 >= 18)
                            return this;
                        x2.ShiftR(shift3);
                    }
                }
                else
                    this.ShiftL(shift1);
                return new DoubleR10(this.M + x2.M, this.E);
            }
            if (shift1 >= 0)
                return DoubleR10.NaN;
            int shift4 = -shift1;
            int shift5 = 17 - x2.MLog10();
            if (shift5 < shift4)
            {
                x2.ShiftL(shift5);
                int shift2 = shift4 - shift5;
                if (shift2 != 0)
                {
                    if (shift2 >= 18)
                        return x2;
                    this.ShiftR(shift2);
                }
            }
            else
                x2.ShiftL(shift4);
            return new DoubleR10(this.M + x2.M, x2.E);
        }

        public DoubleR10 IncrementByStep(DoubleR10 toX, DoubleR10 step)
        {
            DoubleR10 doubleR10 = this;
            DoubleR10 size = toX - doubleR10;
            if (size.M > 0L)
            {
                long num = DoubleR10.CountSteps(size, step);
                if (num > 5L)
                    doubleR10 += step * num;
                else if (num > 0L)
                {
                    step.M = step.M * num;
                    doubleR10 += step;
                }
            }
            return doubleR10;
        }

        public DoubleR10 DecrementByStep(DoubleR10 toX, DoubleR10 step)
        {
            DoubleR10 doubleR10 = this;
            DoubleR10 size = toX - doubleR10;
            if (size.M < 0L)
            {
                long num = DoubleR10.CountSteps(size, step);
                if (num > 5L)
                    doubleR10 -= step * num;
                else if (num > 0L)
                {
                    step.M = -step.M * num;
                    doubleR10 += step;
                }
            }
            return doubleR10;
        }

        public static long CountSteps(DoubleR10 size, DoubleR10 step)
        {
            size.M = Math.Abs(size.M);
            step.M = Math.Abs(step.M);
            int val2 = size.E - step.E;
            if (val2 == 0)
                return size.M / step.M;
            if (val2 > 0)
            {
                if (val2 >= 9)
                    return (long)(size / step);
                if (size.M % step.M == 0L)
                    return size.M / step.M * DoubleR10.Powers[val2];
                int shift1 = Math.Min(18 - size.MLog10() - 1, val2);
                size.ShiftL(shift1);
                int shift2 = val2 - shift1;
                step.ShiftR(shift2);
                if (step.M != 0L)
                    return size.M / step.M;
                return long.MaxValue;
            }
            int shift = -val2;
            if (shift >= 18)
                return 0;
            size.ShiftR(shift);
            return size.M / step.M;
        }

        public void Normalize()
        {
            for (; this.E < 0 && this.M % 10L == 0L; ++this.E)
                this.M = this.M / 10L;
        }

        public override bool Equals(object other)
        {
            return this == (DoubleR10)other;
        }

        public bool Equals(DoubleR10 other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return this.E.GetHashCode() ^ this.M.GetHashCode();
        }

        internal static class Counts
        {
            internal static long InternalLog10 { get; set; }

            internal static long ShiftR { get; set; }

            internal static long ShiftL { get; set; }

            internal static long Equal { get; set; }

            internal static long Less { get; set; }

            internal static long Plus { get; set; }

            internal static long Multiply { get; set; }

            internal static long Divide { get; set; }

            internal static long Round { get; set; }

            internal static long Ceiling { get; set; }

            internal static long Floor { get; set; }

            internal static long Trunc { get; set; }

            internal static long CastToDouble { get; set; }

            internal static long CastToInt { get; set; }

            internal static long CastFromDouble { get; set; }

            internal static long CastFromInt { get; set; }

            internal static void Reset()
            {
                DoubleR10.Counts.InternalLog10 = 0L;
                DoubleR10.Counts.ShiftR = 0L;
                DoubleR10.Counts.ShiftL = 0L;
                DoubleR10.Counts.Equal = 0L;
                DoubleR10.Counts.Less = 0L;
                DoubleR10.Counts.Plus = 0L;
                DoubleR10.Counts.Multiply = 0L;
                DoubleR10.Counts.Divide = 0L;
                DoubleR10.Counts.Round = 0L;
                DoubleR10.Counts.Ceiling = 0L;
                DoubleR10.Counts.Floor = 0L;
                DoubleR10.Counts.Trunc = 0L;
                DoubleR10.Counts.CastToDouble = 0L;
                DoubleR10.Counts.CastToInt = 0L;
                DoubleR10.Counts.CastFromDouble = 0L;
                DoubleR10.Counts.CastFromInt = 0L;
            }
        }
    }
}
