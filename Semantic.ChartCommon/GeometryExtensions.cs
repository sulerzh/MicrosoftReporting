using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public static class GeometryExtensions
    {
        public static IEnumerable<Point> GetPoints(this Geometry geometry)
        {
            if (geometry == null)
                return Enumerable.Empty<Point>();
            RectangleGeometry rectGeometry = geometry as RectangleGeometry;
            if (rectGeometry != null)
                return rectGeometry.GetRectPoints();
            EllipseGeometry ellipseGeometry = geometry as EllipseGeometry;
            if (ellipseGeometry != null)
                return ellipseGeometry.GetEllipsePoints();
            LineGeometry lineGeometry = geometry as LineGeometry;
            if (lineGeometry != null)
                return lineGeometry.GetLinePoints();
            PathGeometry pathGeometry = geometry as PathGeometry;
            if (pathGeometry != null)
                return pathGeometry.GetPathPoints();
            GeometryGroup geometryGroup = geometry as GeometryGroup;
            if (geometryGroup != null)
                return geometryGroup.GetGroupPoints();
            throw new NotSupportedException("This type of geometry is not supported");
        }

        public static IEnumerable<Point> GetRectPoints(this RectangleGeometry rectGeometry)
        {
            if (rectGeometry != null)
            {
                Rect rect = rectGeometry.Rect;
                yield return new Point(rect.Left, rect.Top);
                yield return new Point(rect.Right, rect.Top);
                yield return new Point(rect.Right, rect.Bottom);
                yield return new Point(rect.Left, rect.Bottom);
            }
        }

        public static IEnumerable<Point> GetEllipsePoints(this EllipseGeometry ellipseGeometry)
        {
            if (ellipseGeometry != null)
            {
                yield return new Point(ellipseGeometry.Center.X, ellipseGeometry.Center.Y - ellipseGeometry.RadiusY);
                yield return new Point(ellipseGeometry.Center.X - ellipseGeometry.RadiusX, ellipseGeometry.Center.Y);
                yield return new Point(ellipseGeometry.Center.X, ellipseGeometry.Center.Y + ellipseGeometry.RadiusY);
                yield return new Point(ellipseGeometry.Center.X + ellipseGeometry.RadiusX, ellipseGeometry.Center.Y);
            }
        }

        public static IEnumerable<Point> GetLinePoints(this LineGeometry lineGeometry)
        {
            yield return lineGeometry.StartPoint;
            yield return lineGeometry.EndPoint;
        }

        public static IEnumerable<Point> GetPathPoints(this PathGeometry pathGeometry)
        {
            if (pathGeometry != null)
            {
                foreach (PathFigure figure in pathGeometry.Figures)
                {
                    yield return figure.StartPoint;
                    foreach (PathSegment segment in figure.Segments)
                    {
                        ArcSegment arcSegment = segment as ArcSegment;
                        if (arcSegment != null)
                        {
                            yield return arcSegment.Point;
                        }
                        else
                        {
                            LineSegment lineSergment = segment as LineSegment;
                            if (lineSergment != null)
                            {
                                yield return lineSergment.Point;
                            }
                            else
                            {
                                PolyLineSegment polyLineSegment = segment as PolyLineSegment;
                                if (polyLineSegment != null)
                                {
                                    foreach (Point point in polyLineSegment.Points)
                                        yield return point;
                                }
                                else
                                {
                                    BezierSegment bezierSegment = segment as BezierSegment;
                                    if (bezierSegment != null)
                                    {
                                        yield return bezierSegment.Point3;
                                    }
                                    else
                                    {
                                        PolyBezierSegment polyBezierSegment = segment as PolyBezierSegment;
                                        if (polyBezierSegment != null)
                                        {
                                            foreach (Point point in polyBezierSegment.Points)
                                                yield return point;
                                        }
                                        else
                                        {
                                            QuadraticBezierSegment quadraticBezierSegment = segment as QuadraticBezierSegment;
                                            if (quadraticBezierSegment != null)
                                            {
                                                yield return quadraticBezierSegment.Point2;
                                            }
                                            else
                                            {
                                                PolyQuadraticBezierSegment polyQuadraticBezierSegment = segment as PolyQuadraticBezierSegment;
                                                if (polyQuadraticBezierSegment != null)
                                                {
                                                    foreach (Point point in polyQuadraticBezierSegment.Points)
                                                        yield return point;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<Point> GetGroupPoints(this GeometryGroup geometryGroup)
        {
            if (geometryGroup != null)
            {
                foreach (Geometry child in geometryGroup.Children)
                {
                    foreach (Point point in child.GetPoints())
                        yield return point;
                }
            }
        }

        public static Geometry Clone(this Geometry source)
        {
            if (source == null)
                return null;
            RectangleGeometry source1 = source as RectangleGeometry;
            if (source1 != null)
                return source1.CloneRect();
            EllipseGeometry source2 = source as EllipseGeometry;
            if (source2 != null)
                return source2.CloneEllipse();
            LineGeometry source3 = source as LineGeometry;
            if (source3 != null)
                return source3.CloneLine();
            PathGeometry source4 = source as PathGeometry;
            if (source4 != null)
                return source4.ClonePath();
            GeometryGroup source5 = source as GeometryGroup;
            if (source5 != null)
                return source5.CloneGroup();
            throw new NotSupportedException("This type of geometry is not supported");
        }

        public static RectangleGeometry CloneRect(this RectangleGeometry source)
        {
            if (source == null)
                return null;
            RectangleGeometry rectangleGeometry = new RectangleGeometry();
            rectangleGeometry.Rect = source.Rect;
            rectangleGeometry.RadiusX = source.RadiusX;
            rectangleGeometry.RadiusY = source.RadiusY;
            rectangleGeometry.Transform = source.Transform;
            return rectangleGeometry;
        }

        public static EllipseGeometry CloneEllipse(this EllipseGeometry source)
        {
            if (source == null)
                return null;
            EllipseGeometry ellipseGeometry = new EllipseGeometry();
            ellipseGeometry.Center = source.Center;
            ellipseGeometry.RadiusX = source.RadiusX;
            ellipseGeometry.RadiusY = source.RadiusY;
            ellipseGeometry.Transform = source.Transform;
            return ellipseGeometry;
        }

        public static LineGeometry CloneLine(this LineGeometry source)
        {
            if (source == null)
                return null;
            LineGeometry lineGeometry = new LineGeometry();
            lineGeometry.StartPoint = source.StartPoint;
            lineGeometry.EndPoint = source.EndPoint;
            lineGeometry.Transform = source.Transform;
            return lineGeometry;
        }

        public static PathGeometry ClonePath(this PathGeometry source)
        {
            if (source == null)
                return null;
            PathGeometry pathGeometry = new PathGeometry();
            foreach (PathFigure figure in source.Figures)
            {
                PathFigure pathFigure = GeometryExtensions.CloneFigure(figure);
                pathGeometry.Figures.Add(pathFigure);
            }
            pathGeometry.FillRule = source.FillRule;
            pathGeometry.Transform = source.Transform;
            return pathGeometry;
        }

        public static PathFigure CloneFigure(PathFigure source)
        {
            if (source == null)
                return null;
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = source.StartPoint;
            pathFigure.IsClosed = source.IsClosed;
            pathFigure.IsFilled = source.IsFilled;
            foreach (PathSegment segment in source.Segments)
                pathFigure.Segments.Add(segment.Clone());
            return pathFigure;
        }

        public static PathSegment Clone(this PathSegment source)
        {
            if (source == null)
                return null;
            ArcSegment source1 = source as ArcSegment;
            if (source1 != null)
                return source1.CloneArcSegment();
            LineSegment source2 = source as LineSegment;
            if (source2 != null)
                return source2.CloneLineSegment();
            PolyLineSegment source3 = source as PolyLineSegment;
            if (source3 != null)
                return source3.ClonePolyLineSegment();
            BezierSegment source4 = source as BezierSegment;
            if (source4 != null)
                return source4.CloneBezierSegment();
            PolyBezierSegment source5 = source as PolyBezierSegment;
            if (source5 != null)
                return source5.ClonePolyBezierSegment();
            QuadraticBezierSegment source6 = source as QuadraticBezierSegment;
            if (source6 != null)
                return source6.CloneQuadraticBezierSegment();
            PolyQuadraticBezierSegment source7 = source as PolyQuadraticBezierSegment;
            if (source7 != null)
                return source7.ClonePolyQuadraticBezierSegment();
            throw new NotSupportedException("This type of segment is not supported by Clone extension method");
        }

        public static ArcSegment CloneArcSegment(this ArcSegment source)
        {
            if (source == null)
                return null;
            return new ArcSegment() { IsLargeArc = source.IsLargeArc, Point = source.Point, RotationAngle = source.RotationAngle, Size = source.Size, SweepDirection = source.SweepDirection };
        }

        public static LineSegment CloneLineSegment(this LineSegment source)
        {
            if (source == null)
                return null;
            return new LineSegment() { Point = source.Point };
        }

        public static PolyLineSegment ClonePolyLineSegment(this PolyLineSegment source)
        {
            if (source == null)
                return null;
            PolyLineSegment polyLineSegment = new PolyLineSegment();
            foreach (Point point in source.Points)
                polyLineSegment.Points.Add(point);
            return polyLineSegment;
        }

        public static BezierSegment CloneBezierSegment(this BezierSegment source)
        {
            if (source == null)
                return null;
            return new BezierSegment() { Point1 = source.Point1, Point2 = source.Point2, Point3 = source.Point3 };
        }

        public static PolyBezierSegment ClonePolyBezierSegment(this PolyBezierSegment source)
        {
            if (source == null)
                return null;
            PolyBezierSegment polyBezierSegment = new PolyBezierSegment();
            foreach (Point point in source.Points)
                polyBezierSegment.Points.Add(point);
            return polyBezierSegment;
        }

        public static QuadraticBezierSegment CloneQuadraticBezierSegment(this QuadraticBezierSegment source)
        {
            if (source == null)
                return null;
            return new QuadraticBezierSegment() { Point1 = source.Point1, Point2 = source.Point2 };
        }

        public static PolyQuadraticBezierSegment ClonePolyQuadraticBezierSegment(this PolyQuadraticBezierSegment source)
        {
            if (source == null)
                return null;
            PolyQuadraticBezierSegment quadraticBezierSegment = new PolyQuadraticBezierSegment();
            foreach (Point point in source.Points)
                quadraticBezierSegment.Points.Add(point);
            return quadraticBezierSegment;
        }

        public static GeometryGroup CloneGroup(this GeometryGroup source)
        {
            if (source == null)
                return null;
            GeometryGroup geometryGroup = new GeometryGroup();
            foreach (Geometry child in source.Children)
                geometryGroup.Children.Add(GeometryExtensions.Clone(child));
            geometryGroup.FillRule = source.FillRule;
            geometryGroup.Transform = source.Transform;
            return geometryGroup;
        }
    }
}
