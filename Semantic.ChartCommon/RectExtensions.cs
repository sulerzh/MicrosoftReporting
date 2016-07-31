using Microsoft.Reporting.Common.Toolkit.Internal;
using Microsoft.Reporting.Windows.Common.PivotViewer.Internal;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public static class RectExtensions
    {
        public static Point GetLocation(this Rect rect)
        {
            return new Point(rect.X, rect.Y);
        }

        public static Size GetSize(this Rect rect)
        {
            return new Size(rect.Width, rect.Height);
        }

        public static bool Contains(this Rect value, Rect rect)
        {
            if (value.X <= rect.X && rect.X + rect.Width <= value.X + value.Width && value.Y <= rect.Y)
                return rect.Y + rect.Height <= value.Y + value.Height;
            return false;
        }

        public static bool Contains(this Rect value, double x, double y)
        {
            if (value.X <= x && x < value.X + value.Width && value.Y <= y)
                return y < value.Y + value.Height;
            return false;
        }

        public static bool IntersectsWith(this Rect value, Rect rect)
        {
            if (rect.X < value.X + value.Width && value.X < rect.X + rect.Width && rect.Y < value.Y + value.Height)
                return value.Y < rect.Y + rect.Height;
            return false;
        }

        public static bool IsEmptyOrHasNoSize(this Rect value)
        {
            if (!value.IsEmpty && value.Width != 0.0)
                return value.Height == 0.0;
            return true;
        }

        public static Rect Transform(this Rect rectangle, Transform transform)
        {
            return Rect.Transform(rectangle, transform.GetMatrix());
        }

        public static Rect Translate(this Rect rectangle, double x, double y)
        {
            return new Rect(rectangle.X + x, rectangle.Y + y, rectangle.Width, rectangle.Height);
        }

        public static Rect Translate(this Rect rectangle, Point point)
        {
            return rectangle.Translate(point.X, point.Y);
        }

        public static Rect Expand(this Rect rectangle, double width, double height)
        {
            return new Rect(rectangle.X - width, rectangle.Y - height, rectangle.Width + width * 2.0, rectangle.Height + height * 2.0);
        }

        public static Rect Expand(this Rect rectangle, Size size)
        {
            return rectangle.Expand(size.Width, size.Height);
        }

        public static double LeftOrDefault(this Rect rectangle, double value)
        {
            if (!rectangle.IsEmpty)
                return rectangle.Left;
            return value;
        }

        public static double RightOrDefault(this Rect rectangle, double value)
        {
            if (!rectangle.IsEmpty)
                return rectangle.Right;
            return value;
        }

        public static double WidthOrDefault(this Rect rectangle, double value)
        {
            if (!rectangle.IsEmpty)
                return rectangle.Width;
            return value;
        }

        public static double HeightOrDefault(this Rect rectangle, double value)
        {
            if (!rectangle.IsEmpty)
                return rectangle.Height;
            return value;
        }

        public static double BottomOrDefault(this Rect rectangle, double value)
        {
            if (!rectangle.IsEmpty)
                return rectangle.Bottom;
            return value;
        }

        public static double TopOrDefault(this Rect rectangle, double value)
        {
            if (!rectangle.IsEmpty)
                return rectangle.Top;
            return value;
        }

        public static Rect TranslateToParent(this Rect rect, FrameworkElement child, FrameworkElement parent)
        {
            foreach (FrameworkElement element in child.GetVisualAncestors().OfType<FrameworkElement>())
            {
                if (element != parent)
                {
                    Rect layoutSlot = LayoutInformation.GetLayoutSlot(element);
                    rect = rect.Translate(layoutSlot.X, layoutSlot.Y);
                }
                else
                    break;
            }
            return rect;
        }

        public static Rect Round(Rect value)
        {
            return new Rect((int)Math.Round(value.X), (int)Math.Round(value.Y), (int)Math.Round(value.Width), (int)Math.Round(value.Height));
        }

        public static Rect Intersect(Rect a, Rect b)
        {
            double x = Math.Max(a.X, b.X);
            double num1 = Math.Min(a.X + a.Width, b.X + b.Width);
            double y = Math.Max(a.Y, b.Y);
            double num2 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            if (num1 >= x && num2 >= y)
                return new Rect(x, y, num1 - x, num2 - y);
            return Rect.Empty;
        }

        public static Rect Union(Rect a, Rect b)
        {
            double x = Math.Min(a.X, b.X);
            double num1 = Math.Max(a.X + a.Width, b.X + b.Width);
            double y = Math.Min(a.Y, b.Y);
            double num2 = Math.Max(a.Y + a.Height, b.Y + b.Height);
            return new Rect(x, y, num1 - x, num2 - y);
        }

        public static Rect InflateRect(Rect rect, double x, double y)
        {
            rect.Expand(x, y);
            return rect;
        }
    }
}
