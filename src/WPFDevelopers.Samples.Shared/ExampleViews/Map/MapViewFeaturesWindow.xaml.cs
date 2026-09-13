using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class MapViewFeaturesWindow
    {
        private readonly ObservableCollection<Pushpin> _pushpins = new ObservableCollection<Pushpin>();
        private readonly MapPushpinLayer _alertLayer = new MapPushpinLayer { Name = "告警图层" };
        private readonly MapPushpinLayer _runningLayer = new MapPushpinLayer { Name = "跑步图层", EnableClustering = false };
        private readonly MapTrackAnimator _runningAnimator = new MapTrackAnimator();
        private readonly List<MapLocation> _runningRoute = new List<MapLocation>();
        private readonly DataTemplate _clusterTemplate;
        private bool _isFirstPushpinMoved;
        private Pushpin _runningPushpin;

        public MapViewFeaturesWindow()
        {
            InitializeComponent();

            _clusterTemplate = Resources["CustomClusterTemplate"] as DataTemplate;
            CreatePushpins();
            CreateRunningRoute();
            DemoMap.Pushpins = _pushpins;
            DemoMap.PushpinLayers.Add(_alertLayer);
            DemoMap.PushpinLayers.Add(_runningLayer);
            DemoMap.TileSource = new AMapTileSource(7);
            EnsureRunningPushpin();

            Closed += MapViewFeaturesWindow_Closed;
        }

        private void MapViewFeaturesWindow_Closed(object sender, EventArgs e)
        {
            _runningAnimator.Stop();
        }

        private void CreatePushpins()
        {
            const double demoCenterLatitude = 39.976;
            const double demoCenterLongitude = 116.305;

            for (var i = 0; i < 120; i++)
            {
                var ring = 0.006 + i % 8 * 0.004;
                var angle = i * Math.PI * 0.37;
                _pushpins.Add(new Pushpin
                {
                    Latitude = demoCenterLatitude + Math.Sin(angle) * ring,
                    Longitude = demoCenterLongitude + Math.Cos(angle) * ring * 1.25,
                    Title = "动态点位 " + (i + 1),
                    Tag = i + 1
                });
            }

            _alertLayer.Pushpins.Add(new Pushpin
            {
                Latitude = 39.968,
                Longitude = 116.286,
                Title = "西侧业务点"
            });
            _alertLayer.Pushpins.Add(new Pushpin
            {
                Latitude = 39.986,
                Longitude = 116.332,
                Title = "东侧业务点"
            });
        }

        private void CreateRunningRoute()
        {
            _runningRoute.Clear();
            _runningRoute.Add(new MapLocation { Latitude = 39.9172, Longitude = 116.1688 });
            _runningRoute.Add(new MapLocation { Latitude = 39.9208, Longitude = 116.1795 });
            _runningRoute.Add(new MapLocation { Latitude = 39.9194, Longitude = 116.1926 });
            _runningRoute.Add(new MapLocation { Latitude = 39.9116, Longitude = 116.1984 });
            _runningRoute.Add(new MapLocation { Latitude = 39.9052, Longitude = 116.1910 });
            _runningRoute.Add(new MapLocation { Latitude = 39.9043, Longitude = 116.1776 });
            _runningRoute.Add(new MapLocation { Latitude = 39.9101, Longitude = 116.1681 });
            _runningRoute.Add(new MapLocation { Latitude = 39.9172, Longitude = 116.1688 });
        }

        private void EnsureRunningPushpin()
        {
            if (_runningPushpin != null)
            {
                return;
            }

            var template = Resources["RunningPushpinTemplate"] as DataTemplate;
            var start = _runningRoute.Count > 0 ? _runningRoute[0] : new MapLocation { Latitude = 39.9172, Longitude = 116.1688 };

            _runningPushpin = new Pushpin
            {
                Title = "Runner",
                Latitude = start.Latitude,
                Longitude = start.Longitude,
                AnchorPoint = new Point(0.5, 1),
                Template = template,
                Tag = 0d
            };

            _runningLayer.Pushpins.Clear();
            _runningLayer.Pushpins.Add(_runningPushpin);
        }

        private void StartRunningAnimation()
        {
            if (DemoMap == null || _runningPushpin == null || _runningRoute.Count < 2)
            {
                return;
            }

            _runningAnimator.Start(
                DemoMap,
                _runningPushpin,
                _runningRoute,
                new MapTrackAnimatorOptions
                {
                    SegmentSteps = 16,
                    FrameInterval = TimeSpan.FromMilliseconds(420),
                    FollowMapCenter = false,
                    FollowZoomLevel = null,
                    SelectTargetOnTick = false,
                    HeadingOffsetDegrees = -90d,
                    Loop = true
                });
        }

        private void MapStyleSegmented_ItemClick(object sender, RoutedEventArgs e)
        {
            var style = 7;
            if (MapStyleSegmented.SelectedIndex == 0)
            {
                style = 6;
            }
            else if (MapStyleSegmented.SelectedIndex == 2)
            {
                style = 8;
            }

            DemoMap.TileSource = new AMapTileSource(style);
            DemoMap.AnnotationTileSource = null;
        }

        private void ClusteringCheckBox_Click(object sender, RoutedEventArgs e)
        {
            DemoMap.EnableClustering = ClusteringCheckBox.IsChecked == true;
        }

        private void ClusterTemplateCheckBox_Click(object sender, RoutedEventArgs e)
        {
            DemoMap.ClusterTemplate = ClusterTemplateCheckBox.IsChecked == true ? _clusterTemplate : null;
        }

        private void ScaleBarCheckBox_Click(object sender, RoutedEventArgs e)
        {
            DemoMap.ShowScaleBar = ScaleBarCheckBox.IsChecked == true;
        }

        private void ZoomControlCheckBox_Click(object sender, RoutedEventArgs e)
        {
            DemoMap.ShowZoomControls = ZoomControlCheckBox.IsChecked == true;
        }

        private void DiagnosticsCheckBox_Click(object sender, RoutedEventArgs e)
        {
            DemoMap.ShowDiagnostics = DiagnosticsCheckBox.IsChecked == true;
        }

        private void ClickZoomCheckBox_Click(object sender, RoutedEventArgs e)
        {
            DemoMap.ZoomOnPushpinClick = ClickZoomCheckBox.IsChecked == true;
        }

        private void AlertLayerCheckBox_Click(object sender, RoutedEventArgs e)
        {
            _alertLayer.IsVisible = AlertLayerCheckBox.IsChecked == true;
        }

        private void ClusterRadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DemoMap != null)
            {
                DemoMap.ClusterRadius = e.NewValue;
            }
        }

        private void ClusterMinimumSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DemoMap != null)
            {
                DemoMap.ClusterMinimumCount = (int)e.NewValue;
            }
        }

        private void ToggleFeatureDrawerButton_Click(object sender, RoutedEventArgs e)
        {
            FeatureDrawer.IsOpen = !FeatureDrawer.IsOpen;
        }

        private void ResetViewButton_Click(object sender, RoutedEventArgs e)
        {
            DemoMap.CenterLatitude = 39.976;
            DemoMap.CenterLongitude = 116.305;
            DemoMap.ZoomLevel = 11;
        }

        private void MovePushpinButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pushpins.Count == 0)
            {
                return;
            }

            _isFirstPushpinMoved = !_isFirstPushpinMoved;
            _pushpins[0].Latitude = _isFirstPushpinMoved ? 39.983 : 39.976;
            _pushpins[0].Longitude = _isFirstPushpinMoved ? 116.335 : 116.305;

            DemoMap.SelectPushpin(_pushpins[0], true);
            DemoMap.CenterLatitude = _pushpins[0].Latitude;
            DemoMap.CenterLongitude = _pushpins[0].Longitude;
            DemoMap.ZoomLevel = Math.Max(DemoMap.ZoomLevel, 12);
        }

        private void StartRunningButton_Click(object sender, RoutedEventArgs e)
        {
            EnsureRunningPushpin();
            StartRunningAnimation();
        }

        private void PauseRunningButton_Click(object sender, RoutedEventArgs e)
        {
            _runningAnimator.Pause();
        }

        private void ResumeRunningButton_Click(object sender, RoutedEventArgs e)
        {
            _runningAnimator.Resume();
        }

        private void FocusRunnerButton_Click(object sender, RoutedEventArgs e)
        {
            if (DemoMap == null || _runningPushpin == null)
            {
                return;
            }

            DemoMap.CenterLatitude = _runningPushpin.Latitude;
            DemoMap.CenterLongitude = _runningPushpin.Longitude;
            DemoMap.ZoomLevel = Math.Max(DemoMap.ZoomLevel, 13);
            DemoMap.SelectPushpin(_runningPushpin, false);
        }
    }
}
