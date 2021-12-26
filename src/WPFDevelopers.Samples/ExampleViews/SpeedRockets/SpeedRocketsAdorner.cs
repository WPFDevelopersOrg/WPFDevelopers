using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPFDevelopers.Samples.ExampleViews
{
    public class SpeedRocketsAdorner : Adorner
    {
        private UIElement child;
        public SpeedRocketsAdorner(UIElement adornedElement) : base(adornedElement)
        {

        }
        public UIElement Child
        {
            get => child;
            set
            {
                if (value == null)
                {
                    RemoveVisualChild(child);
                }
                else
                {
                    AddVisualChild(value);
                }
                child = value;
            }
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            finalSize = new Size(SystemParameters.MaximizedPrimaryScreenWidth, SystemParameters.MaximizedPrimaryScreenHeight);
            child.Arrange(new Rect(finalSize));
            return finalSize;
        }
        protected override Visual GetVisualChild(int index)
        {
            if (index == 0 && child != null) return child;
            return base.GetVisualChild(index);
        }
    }
}
