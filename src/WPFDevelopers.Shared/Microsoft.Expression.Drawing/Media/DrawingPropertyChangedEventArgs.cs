using System;

namespace Microsoft.Expression.Drawing.Media
{
    internal class DrawingPropertyChangedEventArgs : EventArgs
    {
        public DrawingPropertyMetadata Metadata { get; set; }

        public bool IsAnimated { get; set; }
    }
}
