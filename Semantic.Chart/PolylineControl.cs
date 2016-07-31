using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class PolylineControl : Canvas
    {
        private PanelElementPool<Polyline, PolylineSegmentDefinition> _polylinePool;

        internal List<PolylineSegmentDefinition> Segments { get; private set; }

        public PointCollection Points { get; set; }

        internal Collection<IAppearanceProvider> Appearances { get; set; }

        public PolylineControl()
        {
            this._polylinePool = new PanelElementPool<Polyline, PolylineSegmentDefinition>(this, () => this.CreatePolylineSegment(), (polyline, segment) => this.InitilizePolylineSegment(polyline, segment), null);
        }

        public void Update()
        {
            this._polylinePool.ReleaseAll();
            this.Segments = this.GetSegmentDefinitions();
            this.Segments.ForEach(segment => this._polylinePool.Get(segment));
            this._polylinePool.AdjustPoolSize();
        }

        private List<PolylineSegmentDefinition> GetSegmentDefinitions()
        {
            List<PolylineSegmentDefinition> segmentDefinitionList = new List<PolylineSegmentDefinition>();
            PolylineSegmentDefinition segmentDefinition = null;
            for (int index = 0; index < this.Points.Count; ++index)
            {
                if (ValueHelper.CanGraph(this.Points[index].X) && ValueHelper.CanGraph(this.Points[index].Y))
                {
                    if (segmentDefinition != null && this.IsSameAppearance(segmentDefinition.Appearance, this.Appearances[index]))
                    {
                        segmentDefinition.Points.Add(this.Points[index]);
                    }
                    else
                    {
                        segmentDefinition = new PolylineSegmentDefinition();
                        segmentDefinition.Appearance = this.Appearances[index];
                        segmentDefinition.Points.Add(this.Points[index]);
                        segmentDefinitionList.Add(segmentDefinition);
                    }
                }
            }
            return segmentDefinitionList;
        }

        private bool IsSameAppearance(IAppearanceProvider a1, IAppearanceProvider a2)
        {
            return a1 == a2 || a1 != null && a2 != null && (a1.StrokeThickness == a2.StrokeThickness && a1.StrokeDashType == a2.StrokeDashType) && (a1.Opacity == a2.Opacity && ValueHelper.CompareBrushes(a1.Stroke, a2.Stroke) && ValueHelper.CompareEffects(a1.Effect, a2.Effect));
        }

        private Polyline CreatePolylineSegment()
        {
            Polyline polyline = new Polyline();
            polyline.StrokeLineJoin = PenLineJoin.Round;
            return polyline;
        }

        private void InitilizePolylineSegment(Polyline polyline, PolylineSegmentDefinition segment)
        {
            if (segment.Points.Count == 1)
            {
                polyline.Points = new PointCollection()
        {
          segment.Points[0],
          segment.Points[0]
        };
                polyline.StrokeStartLineCap = PenLineCap.Round;
                polyline.StrokeEndLineCap = PenLineCap.Round;
                polyline.StrokeThickness = segment.Appearance.StrokeThickness * 2.0;
            }
            else
            {
                polyline.Points = segment.Points;
                polyline.StrokeStartLineCap = PenLineCap.Flat;
                polyline.StrokeEndLineCap = PenLineCap.Flat;
                polyline.StrokeThickness = segment.Appearance.StrokeThickness;
            }
            polyline.Stroke = segment.Appearance.Stroke;
            polyline.StrokeDashArray = VisualUtilities.GetStrokeDashArray(segment.Appearance.StrokeDashType);
            polyline.Effect = segment.Appearance.Effect;
            polyline.Opacity = segment.Appearance.Opacity;
        }
    }
}
