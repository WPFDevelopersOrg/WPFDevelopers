using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class SmallPanel : Panel
    {
        protected override Size MeasureOverride(Size constraint)
        {
            Size gridDesiredSize = new Size();
            foreach (UIElement child in InternalChildren)
            {
                if (child == null) continue;
                child.Measure(constraint);
                gridDesiredSize.Width = Math.Max(gridDesiredSize.Width, child.DesiredSize.Width);
                gridDesiredSize.Height = Math.Max(gridDesiredSize.Height, child.DesiredSize.Height);
            }

            double width = gridDesiredSize.Width;
            if (double.IsInfinity(width))
                width = double.IsInfinity(constraint.Width)
                    ? SystemParameters.WorkArea.Width
                    : constraint.Width;

            double height = gridDesiredSize.Height;
            if (double.IsInfinity(height))
                height = double.IsInfinity(constraint.Height)
                    ? SystemParameters.WorkArea.Height
                    : constraint.Height;

            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                child?.Arrange(new Rect(arrangeSize));
            }
            return arrangeSize;
        }
    }
}
