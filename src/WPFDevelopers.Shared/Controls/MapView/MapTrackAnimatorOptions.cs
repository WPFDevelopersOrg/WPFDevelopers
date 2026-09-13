using System;

namespace WPFDevelopers.Controls
{
    public sealed class MapTrackAnimatorOptions
    {
        public static MapTrackAnimatorOptions Default
        {
            get
            {
                return new MapTrackAnimatorOptions
                {
                    SegmentSteps = 20,
                    FrameInterval = TimeSpan.FromMilliseconds(500),
                    FollowMapCenter = true,
                    FollowZoomLevel = null,
                    SelectTargetOnTick = false,
                    HeadingOffsetDegrees = 0d,
                    Loop = false
                };
            }
        }

        public int SegmentSteps { get; set; }

        public TimeSpan FrameInterval { get; set; }

        public bool FollowMapCenter { get; set; }

        public int? FollowZoomLevel { get; set; }

        public bool SelectTargetOnTick { get; set; }

        public double HeadingOffsetDegrees { get; set; }

        public bool Loop { get; set; }
    }
}
