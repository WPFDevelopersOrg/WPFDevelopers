using System;

namespace Microsoft.Expression.Controls
{
    public interface IPathLayoutItem
    {
        event EventHandler<PathLayoutUpdatedEventArgs> PathLayoutUpdated;

        int LayoutPathIndex { get; }

        int GlobalIndex { get; }

        int LocalIndex { get; }

        double GlobalOffset { get; }

        double LocalOffset { get; }

        double NormalAngle { get; }

        double OrientationAngle { get; }

        bool IsArranged { get; }

        void Update(PathLayoutData data);
    }
}
