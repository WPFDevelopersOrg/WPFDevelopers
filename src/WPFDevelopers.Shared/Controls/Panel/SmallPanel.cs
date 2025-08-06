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
            UIElementCollection children = InternalChildren;

            for (int i = 0, count = children.Count; i < count; ++i)
            {
                UIElement child = children[i];
                if (child != null)
                {
                    child.Measure(constraint);
                    gridDesiredSize.Width = Math.Max(gridDesiredSize.Width, child.DesiredSize.Width);
                    gridDesiredSize.Height = Math.Max(gridDesiredSize.Height, child.DesiredSize.Height);
                }
            }
            return (gridDesiredSize);
        }
        
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElementCollection children = InternalChildren;
            for (int i = 0, count = children.Count; i < count; ++i)
            {
                UIElement child = children[i];
                if (child != null)
                {
                    child.Arrange(new Rect(arrangeSize));
                }
            }
            return (arrangeSize);
        }
    }
}
