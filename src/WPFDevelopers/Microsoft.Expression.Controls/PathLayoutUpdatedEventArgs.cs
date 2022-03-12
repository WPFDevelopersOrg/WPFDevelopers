using System;

namespace Microsoft.Expression.Controls
{
    public sealed class PathLayoutUpdatedEventArgs : EventArgs
    {
        public ChangedPathLayoutProperties ChangedProperties { get; private set; }

        public PathLayoutUpdatedEventArgs(ChangedPathLayoutProperties changedProperties)
        {
            this.ChangedProperties = changedProperties;
        }
    }
}
