using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Common.PivotViewer.Internal
{
    internal static class TransformExtensions
    {
        internal static Matrix GetMatrix(this Transform thisObject)
        {
            MatrixTransform matrixTransform = thisObject as MatrixTransform;
            if (matrixTransform != null)
                return matrixTransform.Matrix;
            TransformGroup transformGroup = thisObject as TransformGroup;
            if (transformGroup == null)
            {
                transformGroup = new TransformGroup();
                transformGroup.Children.Add(thisObject);
            }
            return transformGroup.Value;
        }

        internal static ScaleTransform MakeScaleTransform(double scaleX, double scaleY)
        {
            return TransformExtensions.MakeScaleTransform(scaleX, scaleY, 0.0, 0.0);
        }

        internal static ScaleTransform MakeScaleTransform(double scaleX, double scaleY, double centerX, double centerY)
        {
            return new ScaleTransform() { ScaleX = scaleX, ScaleY = scaleY, CenterX = centerX, CenterY = centerY };
        }

        internal static TranslateTransform MakeTranslateTransform(double offsetX, double offsetY)
        {
            return new TranslateTransform() { X = offsetX, Y = offsetY };
        }

        internal static MatrixTransform MakeMatrixTransform(Matrix xf)
        {
            return new MatrixTransform() { Matrix = xf };
        }

        internal static MatrixTransform MakeScaleTranslateTransform(double scaleX, double scaleY, double offsetX, double offsetY)
        {
            return TransformExtensions.MakeMatrixTransform(new Matrix(scaleX, 0.0, 0.0, scaleY, offsetX, offsetY));
        }

        internal static Transform MakeIdentityTransform()
        {
            return TransformExtensions.MakeTranslateTransform(0.0, 0.0);
        }
    }
}
