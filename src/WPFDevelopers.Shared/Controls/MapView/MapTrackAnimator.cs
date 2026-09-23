using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace WPFDevelopers.Controls
{
    public sealed class MapTrackAnimator
    {
        private readonly DispatcherTimer _timer;
        private readonly List<RouteFrame> _frames = new List<RouteFrame>();

        private MapView _mapView;
        private Pushpin _targetPushpin;
        private MapTrackAnimatorOptions _options = MapTrackAnimatorOptions.Default;
        private int _frameIndex;

        public MapTrackAnimator()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = MapTrackAnimatorOptions.Default.FrameInterval;
            _timer.Tick += Timer_Tick;
        }

        public bool IsRunning
        {
            get { return _timer.IsEnabled; }
        }

        public int FrameCount
        {
            get { return _frames.Count; }
        }

        public int CurrentFrameIndex
        {
            get { return _frameIndex; }
        }

        public void Start(MapView mapView, Pushpin targetPushpin, IEnumerable<MapLocation> waypoints, MapTrackAnimatorOptions options = null)
        {
            if (mapView == null || targetPushpin == null || waypoints == null)
            {
                Stop();
                return;
            }

            _mapView = mapView;
            _targetPushpin = targetPushpin;
            _options = options ?? MapTrackAnimatorOptions.Default;

            _frames.Clear();
            _frameIndex = 0;

            var points = waypoints.Where(x => x != null).ToList();
            if (points.Count < 2)
            {
                Stop();
                return;
            }

            BuildFrames(points, _options, _frames);
            if (_frames.Count == 0)
            {
                Stop();
                return;
            }

            _timer.Interval = _options.FrameInterval;
            ApplyFrame(_frames[0]);
            _frameIndex = 1;
            _timer.Start();
        }

        public void Pause()
        {
            _timer.Stop();
        }

        public void Resume()
        {
            if (_frames.Count == 0 || _mapView == null || _targetPushpin == null)
            {
                return;
            }

            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _frameIndex = 0;
            _frames.Clear();
            _mapView = null;
            _targetPushpin = null;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_mapView == null || _targetPushpin == null || _frames.Count == 0)
            {
                Stop();
                return;
            }

            if (_frameIndex >= _frames.Count)
            {
                if (_options.Loop)
                {
                    _frameIndex = 0;
                }
                else
                {
                    _timer.Stop();
                    return;
                }
            }

            ApplyFrame(_frames[_frameIndex]);
            _frameIndex++;
        }

        private void ApplyFrame(RouteFrame frame)
        {
            _targetPushpin.Latitude = frame.Latitude;
            _targetPushpin.Longitude = frame.Longitude;
            _targetPushpin.Tag = frame.Heading;

            if (_options.FollowMapCenter)
            {
                _mapView.CenterLatitude = frame.Latitude;
                _mapView.CenterLongitude = frame.Longitude;

                if (_options.FollowZoomLevel.HasValue)
                {
                    _mapView.ZoomLevel = _options.FollowZoomLevel.Value;
                }
            }

            if (_options.SelectTargetOnTick)
            {
                _mapView.SelectPushpin(_targetPushpin, false);
            }
        }

        private static void BuildFrames(IList<MapLocation> points, MapTrackAnimatorOptions options, IList<RouteFrame> destination)
        {
            if (points == null || points.Count < 2 || destination == null)
            {
                return;
            }

            var steps = Math.Max(1, options.SegmentSteps);
            for (var index = 0; index < points.Count - 1; index++)
            {
                var from = points[index];
                var to = points[index + 1];
                var heading = CalculateHeading(from, to, options.HeadingOffsetDegrees);

                for (var step = 0; step <= steps; step++)
                {
                    var t = step / (double)steps;
                    destination.Add(new RouteFrame
                    {
                        Latitude = Lerp(from.Latitude, to.Latitude, t),
                        Longitude = Lerp(from.Longitude, to.Longitude, t),
                        Heading = heading
                    });
                }
            }
        }

        private static double Lerp(double from, double to, double t)
        {
            return from + ((to - from) * t);
        }

        public static double CalculateHeading(MapLocation from, MapLocation to, double headingOffsetDegrees = 0d)
        {
            if (from == null || to == null)
            {
                return 0d;
            }

            var dLongitude = to.Longitude - from.Longitude;
            var dLatitude = to.Latitude - from.Latitude;
            var radians = Math.Atan2(dLongitude, dLatitude);
            var degrees = radians * 180d / Math.PI;
            return NormalizeAngle(degrees + headingOffsetDegrees);
        }

        public static double NormalizeAngle(double angle)
        {
            var normalized = angle % 360d;
            if (normalized <= -180d)
            {
                normalized += 360d;
            }

            if (normalized > 180d)
            {
                normalized -= 360d;
            }

            return normalized;
        }

        private sealed class RouteFrame
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Heading { get; set; }
        }
    }
}
