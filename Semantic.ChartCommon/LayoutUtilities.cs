using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public sealed class LayoutUtilities
    {
        public static readonly double DefaultTolerance = 0.5;
        public static readonly double MediumTolerance = 0.001;
        public static readonly double SmallTolerance = 1E-05;

        private LayoutUtilities()
        {
        }

        public static bool AreWithinTolerance(double value1, double value2)
        {
            return LayoutUtilities.AreWithinTolerance(value1, value2, LayoutUtilities.DefaultTolerance);
        }

        public static bool AreWithinTolerance(double value1, double value2, double tolerance)
        {
            return Math.Abs(value1 - value2) < tolerance;
        }

        public static bool AreWithinTolerance(Matrix value1, Matrix value2, double tolerance)
        {
            if (LayoutUtilities.AreWithinTolerance(value1.M11, value2.M11, tolerance) && LayoutUtilities.AreWithinTolerance(value1.M12, value2.M12, tolerance) && (LayoutUtilities.AreWithinTolerance(value1.M21, value2.M21, tolerance) && LayoutUtilities.AreWithinTolerance(value1.M22, value2.M22, tolerance)) && LayoutUtilities.AreWithinTolerance(value1.OffsetX, value2.OffsetX, tolerance))
                return LayoutUtilities.AreWithinTolerance(value1.OffsetY, value2.OffsetY, tolerance);
            return false;
        }

        public static bool AreWithinTolerance(Matrix3D value1, Matrix3D value2, double tolerance)
        {
            if (LayoutUtilities.AreWithinTolerance(value1.M11, value2.M11, tolerance) && LayoutUtilities.AreWithinTolerance(value1.M12, value2.M12, tolerance) && (LayoutUtilities.AreWithinTolerance(value1.M13, value2.M13, tolerance) && LayoutUtilities.AreWithinTolerance(value1.M14, value2.M14, tolerance)) && (LayoutUtilities.AreWithinTolerance(value1.M21, value2.M21, tolerance) && LayoutUtilities.AreWithinTolerance(value1.M22, value2.M22, tolerance) && (LayoutUtilities.AreWithinTolerance(value1.M23, value2.M23, tolerance) && LayoutUtilities.AreWithinTolerance(value1.M24, value2.M24, tolerance))) && (LayoutUtilities.AreWithinTolerance(value1.M31, value2.M31, tolerance) && LayoutUtilities.AreWithinTolerance(value1.M32, value2.M32, tolerance) && (LayoutUtilities.AreWithinTolerance(value1.M33, value2.M33, tolerance) && LayoutUtilities.AreWithinTolerance(value1.M34, value2.M34, tolerance)) && (LayoutUtilities.AreWithinTolerance(value1.M44, value2.M44, tolerance) && LayoutUtilities.AreWithinTolerance(value1.OffsetX, value2.OffsetX, tolerance) && LayoutUtilities.AreWithinTolerance(value1.OffsetY, value2.OffsetY, tolerance))))
                return LayoutUtilities.AreWithinTolerance(value1.OffsetZ, value2.OffsetZ, tolerance);
            return false;
        }

        public static bool AreWithinTolerance(Point value1, Point value2, double tolerance)
        {
            if (LayoutUtilities.AreWithinTolerance(value1.X, value2.X, tolerance))
                return LayoutUtilities.AreWithinTolerance(value1.Y, value2.Y, tolerance);
            return false;
        }

        public static bool AreWithinTolerance(Rect val1, Rect val2, double tolerance)
        {
            bool flag = false;
            if (val1.IsEmpty && val2.IsEmpty)
                flag = true;
            else if (!val1.IsEmpty && !val2.IsEmpty)
                flag = LayoutUtilities.AreWithinTolerance(val1.Top, val2.Top, tolerance) && LayoutUtilities.AreWithinTolerance(val1.Left, val2.Left, tolerance) && LayoutUtilities.AreWithinTolerance(val1.Bottom, val2.Bottom, tolerance) && LayoutUtilities.AreWithinTolerance(val1.Right, val2.Right, tolerance);
            return flag;
        }

        public static bool AreWithinTolerance(Size value1, Size value2, double tolerance)
        {
            if (LayoutUtilities.AreWithinTolerance(value1.Width, value2.Width, tolerance))
                return LayoutUtilities.AreWithinTolerance(value1.Height, value2.Height, tolerance);
            return false;
        }

        public static Rect ComputeAspectRatioRect(double requiredAspectRatio, Size currentSize)
        {
            if (currentSize.Width / currentSize.Height < requiredAspectRatio)
                return new Rect(0.0, 0.0, currentSize.Width, currentSize.Width / requiredAspectRatio);
            return new Rect(0.0, 0.0, currentSize.Height * requiredAspectRatio, currentSize.Height);
        }

        public static DependencyObject GetAncestorOfType(DependencyObject obj, Type type)
        {
            DependencyObject reference = obj;
            while (reference != null)
            {
                reference = VisualTreeHelper.GetParent(reference);
                if (reference != null && reference.GetType().Equals(type))
                    break;
            }
            return reference;
        }

        public static Rect GetControlRectInWindowSpace(UIElement element)
        {
            Visual ancestor = null;
            PresentationSource presentationSource = PresentationSource.FromVisual(element);
            if (presentationSource != null)
                ancestor = presentationSource.RootVisual;
            if (ancestor != null)
                return element.TransformToAncestor(ancestor).TransformBounds(new Rect(element.RenderSize));
            return Rect.Empty;
        }

        public static Point GetControlScaleInWindowSpace(UIElement element)
        {
            Visual ancestor = null;
            PresentationSource presentationSource = PresentationSource.FromVisual(element);
            if (presentationSource != null)
                ancestor = presentationSource.RootVisual;
            if (ancestor == null)
                return new Point(1.0, 1.0);
            Transform transform = (Transform)element.TransformToAncestor(ancestor);
            return new Point(transform.Value.M11, transform.Value.M22);
        }

        public static bool IsBoundedWithinTolerance(double value, double bounds)
        {
            return LayoutUtilities.IsBoundedWithinTolerance(value, bounds, LayoutUtilities.DefaultTolerance);
        }

        public static bool IsBoundedWithinTolerance(double value, double bounds, double tolerance)
        {
            return value - bounds < tolerance;
        }

        public static bool ValueGreaterThan(double value1, double value2, double tolerance)
        {
            if (value1 > value2)
                return !LayoutUtilities.AreWithinTolerance(value1, value2, tolerance);
            return false;
        }

        public static bool ValueGreaterThanOrEqualTo(double value1, double value2, double tolerance)
        {
            if (value1 <= value2)
                return LayoutUtilities.AreWithinTolerance(value1, value2, tolerance);
            return true;
        }

        public static bool ValueLessThan(double value1, double value2, double tolerance)
        {
            if (value1 < value2)
                return !LayoutUtilities.AreWithinTolerance(value1, value2, tolerance);
            return false;
        }

        public static bool ValueLessThanOrEqualTo(double value1, double value2, double tolerance)
        {
            if (value1 >= value2)
                return LayoutUtilities.AreWithinTolerance(value1, value2, tolerance);
            return true;
        }
    }
}
