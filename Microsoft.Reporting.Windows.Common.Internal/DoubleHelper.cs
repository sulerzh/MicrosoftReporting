using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public static class DoubleHelper
    {
        private static readonly double[] DoublePositivePowers = new double[309]
        {
      1.0,
      10.0,
      100.0,
      1000.0,
      10000.0,
      100000.0,
      1000000.0,
      10000000.0,
      100000000.0,
      1000000000.0,
      10000000000.0,
      100000000000.0,
      1000000000000.0,
      10000000000000.0,
      100000000000000.0,
      1E+15,
      1E+16,
      1E+17,
      1E+18,
      1E+19,
      1E+20,
      1E+21,
      1E+22,
      1E+23,
      1E+24,
      1E+25,
      1E+26,
      1E+27,
      1E+28,
      1E+29,
      1E+30,
      1E+31,
      1E+32,
      1E+33,
      1E+34,
      1E+35,
      1E+36,
      1E+37,
      1E+38,
      1E+39,
      1E+40,
      1E+41,
      1E+42,
      1E+43,
      1E+44,
      1E+45,
      1E+46,
      1E+47,
      1E+48,
      1E+49,
      1E+50,
      1E+51,
      1E+52,
      1E+53,
      1E+54,
      1E+55,
      1E+56,
      1E+57,
      1E+58,
      1E+59,
      1E+60,
      1E+61,
      1E+62,
      1E+63,
      1E+64,
      1E+65,
      1E+66,
      1E+67,
      1E+68,
      1E+69,
      1E+70,
      1E+71,
      1E+72,
      1E+73,
      1E+74,
      1E+75,
      1E+76,
      1E+77,
      1E+78,
      1E+79,
      1E+80,
      1E+81,
      1E+82,
      1E+83,
      1E+84,
      1E+85,
      1E+86,
      1E+87,
      1E+88,
      1E+89,
      1E+90,
      1E+91,
      1E+92,
      1E+93,
      1E+94,
      1E+95,
      1E+96,
      1E+97,
      1E+98,
      1E+99,
      1E+100,
      1E+101,
      1E+102,
      1E+103,
      1E+104,
      1E+105,
      1E+106,
      1E+107,
      1E+108,
      1E+109,
      1E+110,
      1E+111,
      1E+112,
      1E+113,
      1E+114,
      1E+115,
      1E+116,
      1E+117,
      1E+118,
      1E+119,
      1E+120,
      1E+121,
      1E+122,
      1E+123,
      1E+124,
      1E+125,
      1E+126,
      1E+127,
      1E+128,
      1E+129,
      1E+130,
      1E+131,
      1E+132,
      1E+133,
      1E+134,
      1E+135,
      1E+136,
      1E+137,
      1E+138,
      1E+139,
      1E+140,
      1E+141,
      1E+142,
      1E+143,
      1E+144,
      1E+145,
      1E+146,
      1E+147,
      1E+148,
      1E+149,
      1E+150,
      1E+151,
      1E+152,
      1E+153,
      1E+154,
      1E+155,
      1E+156,
      1E+157,
      1E+158,
      1E+159,
      1E+160,
      1E+161,
      1E+162,
      1E+163,
      1E+164,
      1E+165,
      1E+166,
      1E+167,
      1E+168,
      1E+169,
      1E+170,
      1E+171,
      1E+172,
      1E+173,
      1E+174,
      1E+175,
      1E+176,
      1E+177,
      1E+178,
      1E+179,
      1E+180,
      1E+181,
      1E+182,
      1E+183,
      1E+184,
      1E+185,
      1E+186,
      1E+187,
      1E+188,
      1E+189,
      1E+190,
      1E+191,
      1E+192,
      1E+193,
      1E+194,
      1E+195,
      1E+196,
      1E+197,
      1E+198,
      1E+199,
      1E+200,
      1E+201,
      1E+202,
      1E+203,
      1E+204,
      1E+205,
      1E+206,
      1E+207,
      1E+208,
      1E+209,
      1E+210,
      1E+211,
      1E+212,
      1E+213,
      1E+214,
      1E+215,
      1E+216,
      1E+217,
      1E+218,
      1E+219,
      1E+220,
      1E+221,
      1E+222,
      1E+223,
      1E+224,
      1E+225,
      1E+226,
      1E+227,
      1E+228,
      1E+229,
      1E+230,
      1E+231,
      1E+232,
      1E+233,
      1E+234,
      1E+235,
      1E+236,
      1E+237,
      1E+238,
      1E+239,
      1E+240,
      1E+241,
      1E+242,
      1E+243,
      1E+244,
      1E+245,
      1E+246,
      1E+247,
      1E+248,
      1E+249,
      1E+250,
      1E+251,
      1E+252,
      1E+253,
      1E+254,
      1E+255,
      1E+256,
      1E+257,
      1E+258,
      1E+259,
      1E+260,
      1E+261,
      1E+262,
      1E+263,
      1E+264,
      1E+265,
      1E+266,
      1E+267,
      1E+268,
      1E+269,
      1E+270,
      1E+271,
      1E+272,
      1E+273,
      1E+274,
      1E+275,
      1E+276,
      1E+277,
      1E+278,
      1E+279,
      1E+280,
      1E+281,
      1E+282,
      1E+283,
      1E+284,
      1E+285,
      1E+286,
      1E+287,
      1E+288,
      1E+289,
      1E+290,
      1E+291,
      1E+292,
      1E+293,
      1E+294,
      1E+295,
      1E+296,
      1E+297,
      1E+298,
      1E+299,
      1E+300,
      1E+301,
      1E+302,
      1E+303,
      1E+304,
      1E+305,
      1E+306,
      1E+307,
      1E+308
        };
        private static readonly double[] DoubleNegativePowers = new double[325]
        {
      1.0,
      0.1,
      0.01,
      0.001,
      0.0001,
      1E-05,
      1E-06,
      1E-07,
      1E-08,
      1E-09,
      1E-10,
      1E-11,
      1E-12,
      1E-13,
      1E-14,
      1E-15,
      1E-16,
      1E-17,
      1E-18,
      1E-19,
      1E-20,
      1E-21,
      1E-22,
      1E-23,
      1E-24,
      1E-25,
      1E-26,
      1E-27,
      1E-28,
      1E-29,
      1E-30,
      1E-31,
      1E-32,
      1E-33,
      1E-34,
      1E-35,
      1E-36,
      1E-37,
      1E-38,
      1E-39,
      1E-40,
      1E-41,
      1E-42,
      1E-43,
      1E-44,
      1E-45,
      1E-46,
      1E-47,
      1E-48,
      1E-49,
      1E-50,
      1E-51,
      1E-52,
      1E-53,
      1E-54,
      1E-55,
      1E-56,
      1E-57,
      1E-58,
      1E-59,
      1E-60,
      1E-61,
      1E-62,
      1E-63,
      1E-64,
      1E-65,
      1E-66,
      1E-67,
      1E-68,
      1E-69,
      1E-70,
      1E-71,
      1E-72,
      1E-73,
      1E-74,
      1E-75,
      1E-76,
      1E-77,
      1E-78,
      1E-79,
      1E-80,
      1E-81,
      1E-82,
      1E-83,
      1E-84,
      1E-85,
      1E-86,
      1E-87,
      1E-88,
      1E-89,
      1E-90,
      1E-91,
      1E-92,
      1E-93,
      1E-94,
      1E-95,
      1E-96,
      1E-97,
      1E-98,
      1E-99,
      1E-100,
      1E-101,
      1E-102,
      1E-103,
      1E-104,
      1E-105,
      1E-106,
      1E-107,
      1E-108,
      1E-109,
      1E-110,
      1E-111,
      1E-112,
      1E-113,
      1E-114,
      1E-115,
      1E-116,
      1E-117,
      1E-118,
      1E-119,
      1E-120,
      1E-121,
      1E-122,
      1E-123,
      1E-124,
      1E-125,
      1E-126,
      1E-127,
      1E-128,
      1E-129,
      1E-130,
      1E-131,
      1E-132,
      1E-133,
      1E-134,
      1E-135,
      1E-136,
      1E-137,
      1E-138,
      1E-139,
      1E-140,
      1E-141,
      1E-142,
      1E-143,
      1E-144,
      1E-145,
      1E-146,
      1E-147,
      1E-148,
      1E-149,
      1E-150,
      1E-151,
      1E-152,
      1E-153,
      1E-154,
      1E-155,
      1E-156,
      1E-157,
      1E-158,
      1E-159,
      1E-160,
      1E-161,
      1E-162,
      1E-163,
      1E-164,
      1E-165,
      1E-166,
      1E-167,
      1E-168,
      1E-169,
      1E-170,
      1E-171,
      1E-172,
      1E-173,
      1E-174,
      1E-175,
      1E-176,
      1E-177,
      1E-178,
      1E-179,
      1E-180,
      1E-181,
      1E-182,
      1E-183,
      1E-184,
      1E-185,
      1E-186,
      1E-187,
      1E-188,
      1E-189,
      1E-190,
      1E-191,
      1E-192,
      1E-193,
      1E-194,
      1E-195,
      1E-196,
      1E-197,
      1E-198,
      1E-199,
      1E-200,
      1E-201,
      1E-202,
      1E-203,
      1E-204,
      1E-205,
      1E-206,
      1E-207,
      1E-208,
      1E-209,
      1E-210,
      1E-211,
      1E-212,
      1E-213,
      1E-214,
      1E-215,
      1E-216,
      1E-217,
      1E-218,
      1E-219,
      1E-220,
      1E-221,
      1E-222,
      1E-223,
      1E-224,
      1E-225,
      1E-226,
      1E-227,
      1E-228,
      1E-229,
      1E-230,
      1E-231,
      1E-232,
      1E-233,
      1E-234,
      1E-235,
      1E-236,
      1E-237,
      1E-238,
      1E-239,
      1E-240,
      1E-241,
      1E-242,
      1E-243,
      1E-244,
      1E-245,
      1E-246,
      1E-247,
      1E-248,
      1E-249,
      1E-250,
      1E-251,
      1E-252,
      1E-253,
      1E-254,
      1E-255,
      1E-256,
      1E-257,
      1E-258,
      1E-259,
      1E-260,
      1E-261,
      1E-262,
      1E-263,
      1E-264,
      1E-265,
      1E-266,
      1E-267,
      1E-268,
      1E-269,
      1E-270,
      1E-271,
      1E-272,
      1E-273,
      1E-274,
      1E-275,
      1E-276,
      1E-277,
      1E-278,
      1E-279,
      1E-280,
      1E-281,
      1E-282,
      1E-283,
      1E-284,
      1E-285,
      1E-286,
      1E-287,
      1E-288,
      1E-289,
      1E-290,
      1E-291,
      1E-292,
      1E-293,
      1E-294,
      1E-295,
      1E-296,
      1E-297,
      1E-298,
      1E-299,
      1E-300,
      1E-301,
      1E-302,
      1E-303,
      1E-304,
      1E-305,
      1E-306,
      1E-307,
      1E-308,
      1E-309,
      9.99999999999997E-311,
      9.99999999999948E-312,
      9.99999999998465E-313,
      1.00000000001329E-313,
      9.99999999963881E-315,
      9.99999998481684E-316,
      9.99999983659714E-317,
      1.00000023069254E-317,
      9.999987484956E-319,
      9.99988867182683E-320,
      9.99988867182683E-321,
      9.98012604599318E-322,
      9.88131291682493E-323,
      9.88131291682493E-324,
      0.0
        };
        public const double DefaultPrecision = 0.0001;
        private const int DefaultPrecisionInDecimalDigits = 12;

        public static bool EqualsWithPrecision(this double value1, double value2, double precision)
        {
            if (value1 != value2)
                return Math.Abs(value1 - value2) <= Math.Abs(precision);
            return true;
        }

        public static bool LessWithPrecision(this double value1, double value2, double precision)
        {
            if (value1 < value2)
                return Math.Abs(value1 - value2) > Math.Abs(precision);
            return false;
        }

        public static bool LessOrEqualWithPrecision(this double value1, double value2, double precision)
        {
            if (value1 > value2)
                return Math.Abs(value1 - value2) <= Math.Abs(precision);
            return true;
        }

        public static bool GreaterWithPrecision(this double value1, double value2, double precision)
        {
            if (value1 > value2)
                return Math.Abs(value1 - value2) > Math.Abs(precision);
            return false;
        }

        public static bool GreaterOrEqualWithPrecision(this double value1, double value2, double precision)
        {
            if (value1 < value2)
                return Math.Abs(value1 - value2) <= Math.Abs(precision);
            return true;
        }

        public static bool EqualsWithPrecision(this double value1, double value2)
        {
            return value1.EqualsWithPrecision(value2, 0.0001);
        }

        public static bool LessWithPrecision(this double value1, double value2)
        {
            return value1.LessWithPrecision(value2, 0.0001);
        }

        public static bool LessOrEqualWithPrecision(this double value1, double value2)
        {
            return value1.LessOrEqualWithPrecision(value2, 0.0001);
        }

        public static bool GreaterWithPrecision(this double value1, double value2)
        {
            return value1.GreaterWithPrecision(value2, 0.0001);
        }

        public static bool GreaterOrEqualWithPrecision(this double value1, double value2)
        {
            return value1.GreaterOrEqualWithPrecision(value2, 0.0001);
        }

        public static IEnumerable<double> GetSteps(this double from, double to, double step)
        {
            double precision = DoubleHelper.GetPrecision(new double[2] { from, to });
            if (step.EqualsWithPrecision(0.0, precision))
                throw new ArgumentOutOfRangeException("step", string.Format("The step={0} is negligebly small for the range of {1} to {2}", step, from, to));
            double x = from;
            while (!x.EqualsWithPrecision(to, precision))
            {
                if (x >= to)
                {
                    yield break;
                }
                else
                {
                    yield return x;
                    x += step;
                }
            }
            yield return to;
        }

        public static IEnumerable<double> GetSteps(this double from, double to, int count)
        {
            if (count > 0)
            {
                double step = (to - from) / count;
                double precision = DoubleHelper.GetPrecision(new double[2] { from, to });
                double x = from;
                while (!x.EqualsWithPrecision(to, precision))
                {
                    if (x >= to)
                    {
                        yield break;
                    }
                    else
                    {
                        yield return x;
                        x += step;
                    }
                }
                yield return to;
            }
        }

        public static double RoundWithPrecision(this double value, double precision)
        {
            if (precision == 0.0 || value == 0.0)
                return value;
            int num1 = DoubleHelper.Log10(Math.Abs(value));
            int exp = DoubleHelper.Log10(Math.Abs(precision));
            int num2 = num1 - exp;
            if (num2 > 16)
                return value;
            if (num2 < 0)
                return 0.0;
            double num3 = DoubleHelper.Pow10(exp);
            if (num3 == 0.0)
                return value;
            double num4 = (long)Math.Round(value / num3) * num3;
            if (0 > exp && exp > -16)
                return Math.Round(num4, -exp);
            return num4;
        }

        public static bool IsNaN(this double value)
        {
            DoubleHelper.NanUnion nanUnion = new DoubleHelper.NanUnion();
            nanUnion.DoubleValue = value;
            ulong num1 = nanUnion.UintValue & 18442240474082181120UL;
            ulong num2 = nanUnion.UintValue & 4503599627370495UL;
            if ((long)num1 == 9218868437227405312L || (long)num1 == -4503599627370496L)
                return (long)num2 != 0L;
            return false;
        }

        public static double GetPrecision(params double[] values)
        {
            return DoubleHelper.GetPrecision(12, values);
        }

        public static double GetPrecision(int digits, params double[] values)
        {
            int num1 = int.MinValue;
            for (int index = 0; index < values.Length; ++index)
            {
                double val = Math.Abs(values[index]);
                if (val != 0.0)
                {
                    int num2 = DoubleHelper.Log10(val);
                    if (num2 > num1)
                        num1 = num2;
                }
            }
            if (num1 > int.MinValue)
                return DoubleHelper.Pow10(Math.Max(num1 - digits, -DoubleHelper.DoubleNegativePowers.Length + 1));
            return 0.0;
        }

        internal static double Pow10(int exp)
        {
            if (exp >= 0)
            {
                if (exp < DoubleHelper.DoublePositivePowers.Length)
                    return DoubleHelper.DoublePositivePowers[exp];
                return double.PositiveInfinity;
            }
            exp = -exp;
            if (exp > 0 && exp < DoubleHelper.DoubleNegativePowers.Length)
                return DoubleHelper.DoubleNegativePowers[exp];
            return 0.0;
        }

        internal static int Log10(double val)
        {
            if (val > 1.0 && val < 1E+32)
            {
                if (val < 1E+16)
                {
                    if (val < 100000000.0)
                    {
                        if (val < 10000.0)
                        {
                            if (val < 100.0)
                                return val < 10.0 ? 0 : 1;
                            return val < 1000.0 ? 2 : 3;
                        }
                        if (val < 1000000.0)
                            return val < 100000.0 ? 4 : 5;
                        return val < 10000000.0 ? 6 : 7;
                    }
                    if (val < 1000000000000.0)
                    {
                        if (val < 10000000000.0)
                            return val < 1000000000.0 ? 8 : 9;
                        return val < 100000000000.0 ? 10 : 11;
                    }
                    if (val < 100000000000000.0)
                        return val < 10000000000000.0 ? 12 : 13;
                    return val < 1E+15 ? 14 : 15;
                }
                if (val < 1E+24)
                {
                    if (val < 1E+20)
                    {
                        if (val < 1E+18)
                            return val < 1E+17 ? 16 : 17;
                        return val < 1E+19 ? 18 : 19;
                    }
                    if (val < 1E+22)
                        return val < 1E+21 ? 20 : 21;
                    return val < 1E+23 ? 22 : 23;
                }
                if (val < 1E+28)
                {
                    if (val < 1E+26)
                        return val < 1E+25 ? 24 : 25;
                    return val < 1E+27 ? 26 : 27;
                }
                if (val < 1E+30)
                    return val < 1E+29 ? 28 : 29;
                return val < 1E+31 ? 30 : 31;
            }
            if (val <= 1E-16 || val >= 1.0)
                return (int)Math.Floor(Math.Log10(val));
            if (val < 1E-08)
            {
                if (val < 1E-12)
                {
                    if (val < 1E-14)
                        return val < 1E-15 ? -16 : -15;
                    return val < 1E-13 ? -14 : -13;
                }
                if (val < 1E-10)
                    return val < 1E-11 ? -12 : -11;
                return val < 1E-09 ? -10 : -9;
            }
            if (val < 0.0001)
            {
                if (val < 1E-06)
                    return val < 1E-07 ? -8 : -7;
                return val < 1E-05 ? -6 : -5;
            }
            if (val < 0.01)
                return val < 0.001 ? -4 : -3;
            return val < 0.1 ? -2 : -1;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            [FieldOffset(0)]
            internal double DoubleValue;
            [FieldOffset(0)]
            internal ulong UintValue;
        }
    }
}
