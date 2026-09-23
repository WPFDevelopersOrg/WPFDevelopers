using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    public partial class MapViewFeaturesWindow
    {
        private const int DefaultCircleZIndex = 10;
        private const int DefaultRectangleZIndex = 20;
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
            _runningRoute.Add(new MapLocation { Latitude = 39.918694, Longitude = 116.148180 });
            _runningRoute.Add(new MapLocation { Latitude = 39.920537, Longitude = 116.156420 });
            _runningRoute.Add(new MapLocation { Latitude = 39.915797, Longitude = 116.162256 });
            _runningRoute.Add(new MapLocation { Latitude = 39.908292, Longitude = 116.167578 });
            _runningRoute.Add(new MapLocation { Latitude = 39.900391, Longitude = 116.175474 });
            _runningRoute.Add(new MapLocation { Latitude = 39.898284, Longitude = 116.166548 });
            _runningRoute.Add(new MapLocation { Latitude = 39.904342, Longitude = 116.156248 });
            _runningRoute.Add(new MapLocation { Latitude = 39.911320, Longitude = 116.150240 });
            _runningRoute.Add(new MapLocation { Latitude = 39.918694, Longitude = 116.148180 });
        }

        private void EnsureRunningPushpin()
        {
            if (_runningPushpin != null)
            {
                return;
            }

            var template = Resources["RunningPushpinTemplate"] as DataTemplate;
            var start = _runningRoute.Count > 0 ? _runningRoute[0] : new MapLocation { Latitude = 39.918694, Longitude = 116.148180 };

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

        private void BringCircleToFrontButton_Click(object sender, RoutedEventArgs e)
        {
            if (DemoMap == null || DemoCircle == null)
            {
                return;
            }

            var maxZIndex = int.MinValue;

            if (DemoMap.Polylines != null)
            {
                for (var i = 0; i < DemoMap.Polylines.Count; i++)
                {
                    var item = DemoMap.Polylines[i];
                    if (item != null && item.ShapeZIndex > maxZIndex)
                    {
                        maxZIndex = item.ShapeZIndex;
                    }
                }
            }

            if (DemoMap.Polygons != null)
            {
                for (var i = 0; i < DemoMap.Polygons.Count; i++)
                {
                    var item = DemoMap.Polygons[i];
                    if (item != null && item.ShapeZIndex > maxZIndex)
                    {
                        maxZIndex = item.ShapeZIndex;
                    }
                }
            }

            if (DemoMap.Circles != null)
            {
                for (var i = 0; i < DemoMap.Circles.Count; i++)
                {
                    var item = DemoMap.Circles[i];
                    if (item != null && item.ShapeZIndex > maxZIndex)
                    {
                        maxZIndex = item.ShapeZIndex;
                    }
                }
            }

            if (DemoMap.Rectangles != null)
            {
                for (var i = 0; i < DemoMap.Rectangles.Count; i++)
                {
                    var item = DemoMap.Rectangles[i];
                    if (item != null && item.ShapeZIndex > maxZIndex)
                    {
                        maxZIndex = item.ShapeZIndex;
                    }
                }
            }

            if (maxZIndex == int.MinValue)
            {
                maxZIndex = 0;
            }

            DemoCircle.ShapeZIndex = maxZIndex + 1;
            FeatureClickInfoTextBlock.Text = $"Circle 已置顶，当前 ShapeZIndex={DemoCircle.ShapeZIndex}";
        }

        private void ResetShapeZIndexButton_Click(object sender, RoutedEventArgs e)
        {
            if (DemoCircle != null)
            {
                DemoCircle.ShapeZIndex = DefaultCircleZIndex;
            }

            if (DemoRectangle != null)
            {
                DemoRectangle.ShapeZIndex = DefaultRectangleZIndex;
            }

            FeatureClickInfoTextBlock.Text = $"已恢复默认层级：Circle={DefaultCircleZIndex}, Rectangle={DefaultRectangleZIndex}";
        }

        private void DemoMap_MapClicked(object sender, RoutedEventArgs e)
        {
            if (DemoMap == null)
            {
                return;
            }

            var clickArgs = e as MapClickEventArgs;
            if (clickArgs == null)
            {
                return;
            }

            var text = $"点击坐标: Latitude={clickArgs.Latitude:F6}, Longitude={clickArgs.Longitude:F6}";
            var mapClickInfoTextBlock = FindName("MapClickInfoTextBlock") as TextBlock;
            if (mapClickInfoTextBlock != null)
            {
                mapClickInfoTextBlock.Text = text;
            }

            Debug.WriteLine(text);
        }

        private void DemoMap_MapFeatureClicked(object sender, RoutedEventArgs e)
        {
            var featureArgs = e as MapFeatureClickEventArgs;
            if (featureArgs == null)
            {
                return;
            }

            var featureClickInfoTextBlock = FindName("FeatureClickInfoTextBlock") as TextBlock;
            if (featureClickInfoTextBlock == null)
            {
                return;
            }

            var text = string.Empty;
            if (featureArgs.FeatureType == MapFeatureType.Pushpin && featureArgs.ClickedPushpin != null)
            {
                text = $"点击 Pushpin: {featureArgs.ClickedPushpin.Title} ({featureArgs.Latitude:F6}, {featureArgs.Longitude:F6})";
            }
            else if (featureArgs.FeatureType == MapFeatureType.Polyline && featureArgs.ClickedPolyline != null)
            {
                text = $"点击 Polyline: 点数={featureArgs.ClickedPolyline.Points.Count} ({featureArgs.Latitude:F6}, {featureArgs.Longitude:F6})";
            }
            else if (featureArgs.FeatureType == MapFeatureType.Polygon && featureArgs.ClickedPolygon != null)
            {
                text = $"点击 Polygon: 点数={featureArgs.ClickedPolygon.Points.Count} ({featureArgs.Latitude:F6}, {featureArgs.Longitude:F6})";
            }
            else if (featureArgs.FeatureType == MapFeatureType.Circle && featureArgs.ClickedCircle != null)
            {
                text = $"点击 Circle: 半径={featureArgs.ClickedCircle.RadiusMeters:F0}m ({featureArgs.Latitude:F6}, {featureArgs.Longitude:F6})";
            }
            else if (featureArgs.FeatureType == MapFeatureType.Rectangle && featureArgs.ClickedRectangle != null)
            {
                text = $"点击 Rectangle: [{featureArgs.ClickedRectangle.MinLatitude:F4},{featureArgs.ClickedRectangle.MinLongitude:F4}]~[{featureArgs.ClickedRectangle.MaxLatitude:F4},{featureArgs.ClickedRectangle.MaxLongitude:F4}]";
            }

            if (!string.IsNullOrEmpty(text))
            {
                featureClickInfoTextBlock.Text = text;
                Debug.WriteLine(text);
            }
        }

        private void ConvertCoordinateButton_Click(object sender, RoutedEventArgs e)
        {
            var sourceSegmented = FindName("CoordinateSourceSegmented") as Segmented;
            var targetSegmented = FindName("CoordinateTargetSegmented") as Segmented;
            var longitudeTextBox = FindName("CoordinateLongitudeTextBox") as TextBox;
            var latitudeTextBox = FindName("CoordinateLatitudeTextBox") as TextBox;
            var resultTextBlock = FindName("CoordinateConvertResultTextBlock") as TextBlock;

            if (sourceSegmented == null || targetSegmented == null || longitudeTextBox == null || latitudeTextBox == null || resultTextBlock == null)
            {
                return;
            }

            if (!double.TryParse(longitudeTextBox.Text, out var longitude) ||
                !double.TryParse(latitudeTextBox.Text, out var latitude))
            {
                resultTextBlock.Text = "请输入有效的经度/纬度";
                return;
            }

            var sourceType = ParseCoordinateType(sourceSegmented.SelectedIndex);
            var targetType = ParseCoordinateType(targetSegmented.SelectedIndex);
            var point = new MapGeoPoint(longitude, latitude, sourceType);
            var converted = MapCoordinateHelper.Convert(point, targetType);

            resultTextBlock.Text =
                $"{sourceType} -> {targetType}\nLongitude={converted.Longitude:F6}\nLatitude={converted.Latitude:F6}";
        }

        private static MapCoordinateType ParseCoordinateType(int index)
        {
            switch (index)
            {
                case 1:
                    return MapCoordinateType.Gcj02;
                case 2:
                    return MapCoordinateType.Bd09;
                default:
                    return MapCoordinateType.Wgs84;
            }
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
