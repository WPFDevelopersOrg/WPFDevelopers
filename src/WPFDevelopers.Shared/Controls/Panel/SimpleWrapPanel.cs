using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class SimpleWrapPanel : Panel
    {
        public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
            nameof(Spacing), typeof(double), typeof(SimpleWrapPanel),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double Spacing
        {
            get => (double)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            double panelWidth = constraint.Width == double.PositiveInfinity ? 0 : constraint.Width;
            double currentLineWidth = 0;
            double currentLineHeight = 0;
            double totalHeight = 0;
            var spacing = Spacing;
            foreach (UIElement child in InternalChildren)
            {
                if (child == null) continue;
                child.Measure(new Size(Math.Min(panelWidth, constraint.Width), double.PositiveInfinity));
                var desiredSize = child.DesiredSize;
                if (currentLineWidth + desiredSize.Width > panelWidth)
                {
                    totalHeight += currentLineHeight + spacing;
                    currentLineWidth = 0;
                    currentLineHeight = 0;
                }
                currentLineWidth += desiredSize.Width + spacing;
                currentLineHeight = Math.Max(currentLineHeight, desiredSize.Height);
            }

            totalHeight += currentLineHeight;
            return new Size(panelWidth, totalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double panelWidth = finalSize.Width;
            double currentLineWidth = 0;
            double currentLineHeight = 0;
            double currentY = 0;
            var spacing = Spacing;
            foreach (UIElement child in InternalChildren)
            {
                if (child == null) continue;
                var desiredSize = child.DesiredSize;
                if (currentLineWidth + desiredSize.Width > panelWidth)
                {
                    currentY += currentLineHeight + spacing;
                    currentLineWidth = 0;
                    currentLineHeight = 0;
                }
                child.Arrange(new Rect(new Point(currentLineWidth, currentY), desiredSize));
                currentLineWidth += desiredSize.Width + spacing;
                currentLineHeight = Math.Max(currentLineHeight, desiredSize.Height);
            }
            return finalSize;
        }
    }
}
