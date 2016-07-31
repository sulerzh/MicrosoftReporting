using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public static class VisualUtilities
    {
        public static Geometry GetMarkerGeometry(MarkerType markerType, Size markerSize)
        {
            return VisualUtilities.GetMarkerGeometry(markerType, new Point(0.0, 0.0), markerSize, 0.0, 0.0);
        }

        public static Geometry GetMarkerGeometry(MarkerType markerType, Point markerOrigin, Size markerSize, double startAngle = 0.0, double sweepAngle = 0.0)
        {
            switch (markerType)
            {
                case MarkerType.Circle:
                    return new EllipseGeometry() { Center = new Point(markerSize.Width / 2.0 - markerOrigin.X, markerSize.Height / 2.0 - markerOrigin.Y), RadiusX = (markerSize.Width / 2.0), RadiusY = (markerSize.Height / 2.0) };
                case MarkerType.Star4:
                case MarkerType.Star5:
                case MarkerType.Star10:
                    PointCollection points1 = new PointCollection();
                    RotateTransform rotateTransform1 = new RotateTransform();
                    rotateTransform1.CenterX = markerSize.Width / 2.0;
                    rotateTransform1.CenterY = markerSize.Height / 2.0;
                    int num1 = 4;
                    if (markerType == MarkerType.Star5)
                        num1 = 5;
                    else if (markerType == MarkerType.Star10)
                        num1 = 10;
                    for (int index = 0; index < num1 * 2; ++index)
                    {
                        Point point = new Point(markerSize.Width / 2.0, index % 2 == 0 ? 0.0 : markerSize.Height / 3.0);
                        rotateTransform1.Angle = index * 360.0 / (num1 * 2.0);
                        point = rotateTransform1.Transform(point);
                        point.X -= markerOrigin.X;
                        point.Y -= markerOrigin.Y;
                        points1.Add(point);
                    }
                    return VisualUtilities.CreatePolyLineGeometry(points1);
                case MarkerType.Triangle:
                    return VisualUtilities.CreatePolyLineGeometry(new PointCollection() { new Point(markerSize.Width / 2.0 - markerOrigin.X, 0.0 - markerOrigin.Y), new Point(markerSize.Width - markerOrigin.X, markerSize.Height - markerOrigin.Y), new Point(0.0 - markerOrigin.X, markerSize.Height - markerOrigin.Y) });
                case MarkerType.Square:
                case MarkerType.Rectangle:
                    return new RectangleGeometry() { Rect = new Rect(0.0 - markerOrigin.X, 0.0 - markerOrigin.Y, markerSize.Width, markerSize.Height) };
                case MarkerType.Diamond:
                    return VisualUtilities.CreatePolyLineGeometry(new PointCollection() { new Point(markerSize.Width / 2.0 - markerOrigin.X, 0.0 - markerOrigin.Y), new Point(markerSize.Width - markerOrigin.X, markerSize.Height / 2.0 - markerOrigin.Y), new Point(markerSize.Width / 2.0 - markerOrigin.X, markerSize.Height - markerOrigin.Y), new Point(0.0 - markerOrigin.X, markerSize.Height / 2.0 - markerOrigin.Y) });
                case MarkerType.Cross:
                case MarkerType.CrossRotated:
                    Size size1 = new Size(markerSize.Width / 4.0, markerSize.Height / 4.0);
                    Point[] pointArray = new Point[12] { new Point(markerSize.Width / 2.0 - size1.Width / 2.0 - markerOrigin.X, 0.0 - markerOrigin.Y), new Point(markerSize.Width / 2.0 + size1.Width / 2.0 - markerOrigin.X, 0.0 - markerOrigin.Y), new Point(markerSize.Width / 2.0 + size1.Width / 2.0 - markerOrigin.X, markerSize.Height / 2.0 - size1.Height / 2.0 - markerOrigin.Y), new Point(markerSize.Width - markerOrigin.X, markerSize.Height / 2.0 - size1.Height / 2.0 - markerOrigin.Y), new Point(markerSize.Width - markerOrigin.X, markerSize.Height / 2.0 + size1.Height / 2.0 - markerOrigin.Y), new Point(markerSize.Width / 2.0 + size1.Width / 2.0 - markerOrigin.X, markerSize.Height / 2.0 + size1.Height / 2.0 - markerOrigin.Y), new Point(markerSize.Width / 2.0 + size1.Width / 2.0 - markerOrigin.X, markerSize.Height - markerOrigin.Y), new Point(markerSize.Width / 2.0 - size1.Width / 2.0 - markerOrigin.X, markerSize.Height - markerOrigin.Y), new Point(markerSize.Width / 2.0 - size1.Width / 2.0 - markerOrigin.X, markerSize.Height / 2.0 + size1.Height / 2.0 - markerOrigin.Y), new Point(0.0 - markerOrigin.X, markerSize.Height / 2.0 + size1.Height / 2.0 - markerOrigin.Y), new Point(0.0 - markerOrigin.X, markerSize.Height / 2.0 - size1.Height / 2.0 - markerOrigin.Y), new Point(markerSize.Width / 2.0 - size1.Width / 2.0 - markerOrigin.X, markerSize.Height / 2.0 - size1.Height / 2.0 - markerOrigin.Y) };
                    PointCollection points2 = new PointCollection();
                    if (markerType == MarkerType.CrossRotated)
                    {
                        RotateTransform rotateTransform2 = new RotateTransform();
                        rotateTransform2.Angle = 45.0;
                        rotateTransform2.CenterX = markerSize.Width / 2.0 - markerOrigin.X;
                        rotateTransform2.CenterY = markerSize.Height / 2.0 - markerOrigin.Y;
                        for (int index = 0; index < pointArray.Length; ++index)
                            points2.Add(rotateTransform2.Transform(pointArray[index]));
                    }
                    else
                    {
                        for (int index = 0; index < pointArray.Length; ++index)
                            points2.Add(pointArray[index]);
                    }
                    return VisualUtilities.CreatePolyLineGeometry(points2);
                case MarkerType.Trapezoid:
                    Size size2 = new Size(markerSize.Width / 3.0, markerSize.Height / 3.0);
                    return VisualUtilities.CreatePolyLineGeometry(new PointCollection() { new Point(size2.Width - markerOrigin.X, 0.0 - markerOrigin.Y), new Point(markerSize.Width - size2.Width - markerOrigin.X, 0.0 - markerOrigin.Y), new Point(markerSize.Width - markerOrigin.X, markerSize.Height - markerOrigin.Y), new Point(0.0 - markerOrigin.X, markerSize.Height - markerOrigin.Y) });
                case MarkerType.Pentagon:
                    PointCollection points3 = new PointCollection();
                    RotateTransform rotateTransform3 = new RotateTransform();
                    rotateTransform3.CenterX = markerSize.Width / 2.0 - markerOrigin.X;
                    rotateTransform3.CenterY = markerSize.Height / 2.0 - markerOrigin.Y;
                    int num2 = 5;
                    for (int index = 0; index < num2; ++index)
                    {
                        Point point = new Point(markerSize.Width / 2.0 - markerOrigin.X, 0.0 - markerOrigin.Y);
                        rotateTransform3.Angle = index * 360.0 / num2;
                        point = rotateTransform3.Transform(point);
                        points3.Add(point);
                    }
                    return VisualUtilities.CreatePolyLineGeometry(points3);
                case MarkerType.Wedge:
                    Size size3 = new Size(markerSize.Width / 3.0, markerSize.Height / 3.0);
                    return VisualUtilities.CreatePolyLineGeometry(new PointCollection() { new Point(markerSize.Width / 2.0 - markerOrigin.X, 0.0 - markerOrigin.Y), new Point(markerSize.Width - markerOrigin.X, size3.Height - markerOrigin.Y), new Point(markerSize.Width - markerOrigin.X, markerSize.Height - markerOrigin.Y), new Point(0.0 - markerOrigin.X, markerSize.Height - markerOrigin.Y), new Point(0.0 - markerOrigin.X, size3.Height - markerOrigin.Y) });
                case MarkerType.PieSlice:
                    if (sweepAngle >= 360.0)
                        return new EllipseGeometry() { Center = new Point(markerSize.Width / 2.0 - markerOrigin.X, markerSize.Height / 2.0 - markerOrigin.Y), RadiusX = (markerSize.Width / 2.0), RadiusY = (markerSize.Height / 2.0) };
                    Point point1 = new Point(markerSize.Width / 2.0 - markerOrigin.X, markerSize.Height / 2.0 - markerOrigin.Y);
                    Point point2 = new Point(point1.X, point1.Y - markerSize.Height / 2.0);
                    Point point3 = VisualUtilities.RotatePointBy(point2, startAngle);
                    Point point4 = VisualUtilities.RotatePointBy(point2, startAngle + sweepAngle);
                    PathFigure pathFigure = new PathFigure() { StartPoint = VisualUtilities.ShiftPoint(point1) };
                    pathFigure.Segments.Add(new LineSegment()
                    {
                        Point = VisualUtilities.ShiftPoint(point3)
                    });
                    pathFigure.Segments.Add(new ArcSegment()
                    {
                        IsLargeArc = (sweepAngle >= 180.0),
                        SweepDirection = SweepDirection.Clockwise,
                        Size = new Size(markerSize.Width / 2.0, markerSize.Height / 2.0),
                        Point = VisualUtilities.ShiftPoint(point4)
                    });
                    pathFigure.Segments.Add(new LineSegment()
                    {
                        Point = VisualUtilities.ShiftPoint(point1)
                    });
                    return new PathGeometry() { Figures = { pathFigure } };
                default:
                    return null;
            }
        }

        private static Point ShiftPoint(Point point)
        {
            return new Point(point.X + 1.0, point.Y + 1.0);
        }

        private static Point RotatePointBy(Point point, double angle)
        {
            double num1 = angle * (Math.PI / 180.0);
            double num2 = Math.Sin(num1);
            double num3 = Math.Cos(num1);
            return new Point(point.X * num3 - point.Y * num2, point.X * num2 + point.Y * num3);
        }

        private static Geometry CreatePolyLineGeometry(PointCollection points)
        {
            Point point = points[0];
            points.RemoveAt(0);
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            PolyLineSegment polyLineSegment = new PolyLineSegment();
            pathFigure.IsClosed = true;
            pathFigure.StartPoint = point;
            polyLineSegment.Points = points;
            pathFigure.Segments.Add(polyLineSegment);
            pathGeometry.Figures.Add(pathFigure);
            return pathGeometry;
        }

        public static DoubleCollection GetStrokeDashArray(StrokeDashType dashType)
        {
            DoubleCollection doubleCollection = new DoubleCollection();
            switch (dashType)
            {
                case StrokeDashType.None:
                    doubleCollection.Add(0.0);
                    doubleCollection.Add(0.0);
                    break;
                case StrokeDashType.Solid:
                    doubleCollection.Add(1.0);
                    doubleCollection.Add(0.0);
                    break;
                case StrokeDashType.Dash:
                    doubleCollection.Add(5.0);
                    doubleCollection.Add(1.0);
                    break;
                case StrokeDashType.Dot:
                    doubleCollection.Add(2.0);
                    doubleCollection.Add(1.0);
                    break;
                case StrokeDashType.DashDot:
                    doubleCollection.Add(5.0);
                    doubleCollection.Add(1.0);
                    doubleCollection.Add(2.0);
                    doubleCollection.Add(1.0);
                    break;
                case StrokeDashType.DashDotDot:
                    doubleCollection.Add(5.0);
                    doubleCollection.Add(1.0);
                    doubleCollection.Add(2.0);
                    doubleCollection.Add(1.0);
                    doubleCollection.Add(2.0);
                    doubleCollection.Add(1.0);
                    break;
                case StrokeDashType.LongDash:
                    doubleCollection.Add(10.0);
                    doubleCollection.Add(2.0);
                    break;
                case StrokeDashType.LongDashDot:
                    doubleCollection.Add(10.0);
                    doubleCollection.Add(2.0);
                    doubleCollection.Add(4.0);
                    doubleCollection.Add(2.0);
                    break;
                case StrokeDashType.LongDashDotDot:
                    doubleCollection.Add(10.0);
                    doubleCollection.Add(2.0);
                    doubleCollection.Add(4.0);
                    doubleCollection.Add(2.0);
                    doubleCollection.Add(4.0);
                    doubleCollection.Add(2.0);
                    break;
            }
            return doubleCollection;
        }
    }
}
