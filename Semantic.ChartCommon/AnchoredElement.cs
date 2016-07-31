using System.Windows;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    internal class AnchoredElement
    {
        public UIElement UIElement { get; private set; }

        public Size ArrangeSize { get; private set; }

        public OutsidePlacement OutsidePlacement { get; private set; }

        public Point AnchorPoint { get; private set; }

        public Rect AnchorRect { get; private set; }

        public RectOrientation AnchorRectOrientation { get; private set; }

        public double AnchorMargin { get; private set; }

        public ContentPositions ValidContentPositions { get; private set; }

        public double MinimumMovingDistance { get; private set; }

        public double MaximumMovingDistance { get; private set; }

        public ContentPositions ContentPosition { get; set; }

        public double Offset { get; set; }

        public AnchoredElement(UIElement element, Size arrangeSize)
        {
            this.UIElement = element;
            this.ArrangeSize = arrangeSize;
            this.AnchorPoint = AnchorPanel.GetAnchorPoint(element);
            this.AnchorMargin = AnchorPanel.GetAnchorMargin(element);
            this.AnchorRect = AnchorPanel.GetAnchorRect(element);
            this.AnchorRectOrientation = AnchorPanel.GetAnchorRectOrientation(element);
            this.ContentPosition = AnchorPanel.GetContentPosition(element);
            this.ValidContentPositions = AnchorPanel.GetValidContentPositions(element);
            this.MinimumMovingDistance = AnchorPanel.GetMinimumMovingDistance(element);
            this.MaximumMovingDistance = AnchorPanel.GetMaximumMovingDistance(element);
            this.MaximumMovingDistance += this.AnchorMargin;
            this.OutsidePlacement = AnchorPanel.GetOutsidePlacement(element);
        }
    }
}
