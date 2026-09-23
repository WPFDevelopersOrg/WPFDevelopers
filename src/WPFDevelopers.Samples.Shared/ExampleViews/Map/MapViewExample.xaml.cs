using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// MapViewExample.xaml 的交互逻辑
    /// </summary>
    public partial class MapViewExample : UserControl
    {
        private readonly ObservableCollection<Pushpin> _pushpins = new ObservableCollection<Pushpin>();
        private readonly MapTrackAnimator _carTrackAnimator = new MapTrackAnimator();
        private readonly int _defaultMapStyle = 7;
        private string _tencentKey = string.Empty;
        private string _tiandituToken = "key";
        private string _tiandituLayer = "vec_w";
        private string _mapToken = string.Empty;
        private readonly List<MapLocation> _routeWaypoints = new List<MapLocation>();
        private readonly ObservableCollection<MapPushpinLayer> _pushpinLayers = new ObservableCollection<MapPushpinLayer>();
        private Pushpin _carPushpin;
        private int _currentMapStyle = 7;

        private MapView MapViewControl
        {
            get { return FindName("mapView") as MapView; }
        }

        public IEnumerable PushpinArray
        {
            get { return (IEnumerable)GetValue(PushpinArrayProperty); }
            set { SetValue(PushpinArrayProperty, value); }
        }

        public static readonly DependencyProperty PushpinArrayProperty =
            DependencyProperty.Register("PushpinArray", typeof(IEnumerable), typeof(MapViewExample), new PropertyMetadata(null));

        public MapViewExample()
        {
            InitializeComponent();

            var pushpins = new List<PushpinModel>();//116.31035086800418,39.97933035921754
            pushpins.Add(new PushpinModel { ID = 1, Latitude = 39.97933035921754, Longitude = 116.31035086800418, Title = "Microsoft" });
            pushpins.Add(new PushpinModel { ID = 2, Latitude = 40.02306458088111, Longitude = 116.48969274078887, Title = "Alibaba" });//116.48969274078887,40.02306458088111
            pushpins.Add(new PushpinModel { ID = 3, Latitude = 40.04593821261656, Longitude = 116.31875661362514, Title = "Xiaomi" });//116.31875661362514,40.04593821261656
            pushpins.Add(new PushpinModel { ID = 4, Latitude = 39.91, Longitude = 116.44, Title = "Apple" });
            PushpinArray = pushpins;

            foreach (var item in pushpins)
            {
                _pushpins.Add(new Pushpin
                {
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    Title = item.Title,
                    Tag = item.ID
                });
            }

            _routeWaypoints.Add(new MapLocation { Latitude = 31.915951, Longitude = 107.240750 });
            _routeWaypoints.Add(new MapLocation { Latitude = 36.6797276003243, Longitude = 118.495410536117 });
            _routeWaypoints.Add(new MapLocation { Latitude = 39.785633, Longitude = 116.557292 });

            if (MapViewControl != null)
            {
                MapViewControl.Pushpins = _pushpins;
                MapViewControl.PushpinLayers = _pushpinLayers;
                _tiandituLayer = _currentMapStyle == 6 ? "img_w" : "vec_w";
                MapViewControl.TileSource = new TiandituTileSource(_tiandituLayer, 0, _tiandituToken);
                MapViewControl.AnnotationTileSource = new TiandituTileSource(_tiandituLayer == "img_w" ? "cia_w" : "cva_w", 0, _tiandituToken);
                //MapViewControl.TileSource = new AMapTileSource(_currentMapStyle);
                //MapViewControl.AnnotationTileSource = null;
            }

            //if (MapSourceSegmented != null)
            //{
            //    MapSourceSegmented.SelectedIndex = 1;
            //}

            //UpdateTokenInputVisibility();

            EnsureCarLayer();
            _carPushpin = FindCarPushpin();
            if (_carPushpin != null)
            {
                var start = _routeWaypoints[0];
                _carPushpin.Latitude = start.Latitude + 0.0008;
                _carPushpin.Longitude = start.Longitude + 0.0013;
                _carPushpin.Tag = GetInitialCarHeading();
            }
        }

        private void BtnCar_Click(object sender, RoutedEventArgs e)
        {
            if (_routeWaypoints.Count < 2 || MapViewControl == null)
            {
                return;
            }

            if (_carPushpin == null)
            {
                _carPushpin = FindCarPushpin();
            }

            if (_carPushpin != null)
            {
                MapViewControl.SelectPushpin(_carPushpin, false);
                _carTrackAnimator.Start(
                    MapViewControl,
                    _carPushpin,
                    _routeWaypoints,
                    new MapTrackAnimatorOptions
                    {
                        SegmentSteps = 24,
                        FrameInterval = TimeSpan.FromSeconds(1.2),
                        FollowMapCenter = true,
                        FollowZoomLevel = 7,
                        SelectTargetOnTick = false,
                        HeadingOffsetDegrees = 0d,
                        Loop = false
                    });
            }
        }

        private Pushpin FindCarPushpin()
        {
            if (MapViewControl == null)
            {
                return null;
            }

            if (MapViewControl.PushpinLayers != null)
            {
                foreach (var layer in MapViewControl.PushpinLayers)
                {
                    if (layer?.Pushpins == null)
                    {
                        continue;
                    }

                    var car = layer.Pushpins.FirstOrDefault(x => string.Equals(x?.Title, "Car", StringComparison.OrdinalIgnoreCase));
                    if (car != null)
                    {
                        return car;
                    }
                }
            }

            if (MapViewControl.InlinePushpins != null)
            {
                return MapViewControl.InlinePushpins.FirstOrDefault(x => string.Equals(x?.Title, "Car", StringComparison.OrdinalIgnoreCase));
            }

            return null;
        }

        private void EnsureCarLayer()
        {
            if (MapViewControl == null)
            {
                return;
            }

            var existingCar = FindCarPushpin();
            if (existingCar != null)
            {
                _carPushpin = existingCar;
                NormalizeCarPushpin(_carPushpin);
                return;
            }

            var carTemplate = ResolveCarPushpinTemplate();
            var carLayer = new MapPushpinLayer
            {
                Name = "CarLayer",
                IsVisible = true,
                EnableClustering = false
            };

            var start = _routeWaypoints.Count > 0 ? _routeWaypoints[0] : new MapLocation { Latitude = 31.915951, Longitude = 107.240750 };
            _carPushpin = new Pushpin
            {
                Title = "Car",
                AnchorPoint = new Point(0.5, 0.5),
                Latitude = start.Latitude + 0.0008,
                Longitude = start.Longitude + 0.0013,
                Template = carTemplate,
                Tag = GetInitialCarHeading()
            };

            NormalizeCarPushpin(_carPushpin);
            carLayer.Pushpins.Add(_carPushpin);
            _pushpinLayers.Add(carLayer);
        }

        private void NormalizeCarPushpin(Pushpin carPushpin)
        {
            if (carPushpin == null)
            {
                return;
            }

            carPushpin.AnchorPoint = new Point(0.5, 0.5);
            carPushpin.Template = ResolveCarPushpinTemplate();
        }

        private DataTemplate ResolveCarPushpinTemplate()
        {
            if (MapViewControl != null)
            {
                var fromMap = MapViewControl.TryFindResource("CarPushpinTemplate") as DataTemplate;
                if (fromMap != null)
                {
                    return fromMap;
                }
            }

            var fromControl = TryFindResource("CarPushpinTemplate") as DataTemplate;
            if (fromControl != null)
            {
                return fromControl;
            }

            return Application.Current?.TryFindResource("CarPushpinTemplate") as DataTemplate;
        }

        private double GetInitialCarHeading()
        {
            if (_routeWaypoints.Count < 2)
            {
                return 0d;
            }

            return MapTrackAnimator.CalculateHeading(_routeWaypoints[0], _routeWaypoints[1], 0d);
        }

        private void PART_Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var itemElement = sender as FrameworkElement;
            var source = PushpinArray?.OfType<PushpinModel>();
            if (itemElement == null || source == null)
            {
                return;
            }

            var model = source.FirstOrDefault(x => Equals(x.ID, itemElement.Tag));
            if (model == null)
                return;

            if (MapViewControl == null)
            {
                return;
            }

            var pushpin = _pushpins.FirstOrDefault(x => Equals(x.Tag, model.ID));
            if (pushpin != null)
            {
                MapViewControl.SelectPushpin(pushpin, true);
            }

            MapViewControl.CenterLatitude = model.Latitude;
            MapViewControl.CenterLongitude = model.Longitude;
            MapViewControl.ZoomLevel = 16;
        }

        private void StyleSegmented_ItemClick(object sender, RoutedEventArgs e)
        {
            var segmented = sender as Segmented;
            if (segmented == null)
            {
                return;
            }

            switch (segmented.SelectedIndex)
            {
                case 0:
                    _currentMapStyle = 6;
                    break;
                case 1:
                    _currentMapStyle = 7;
                    break;
                case 2:
                    _currentMapStyle = 8;
                    break;
                default:
                    _currentMapStyle = 7;
                    break;
            }

            if (MapViewControl == null)
            {
                return;
            }

            //if (MapSourceSegmented != null && MapSourceSegmented.SelectedIndex == 1)
            //{
            //    _tiandituLayer = _currentMapStyle == 6 ? "img_w" : "vec_w";
            //    MapViewControl.TileSource = new TiandituTileSource(_tiandituLayer, 0, _tiandituToken);
            //    MapViewControl.AnnotationTileSource = new TiandituTileSource(_tiandituLayer == "img_w" ? "cia_w" : "cva_w", 0, _tiandituToken);
            //    return;
            //}

            MapViewControl.TileSource = new AMapTileSource(_currentMapStyle);
            MapViewControl.AnnotationTileSource = null;
        }

        private void MapTokenTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            //if (textBox == null || MapViewControl == null || MapSourceSegmented == null)
            if (textBox == null || MapViewControl == null)
            {
                return;
            }

            _mapToken = textBox.Text ?? string.Empty;
            _tiandituToken = _mapToken;
            MapViewControl.TileSource = new TiandituTileSource(_tiandituLayer, 0, _tiandituToken);
            MapViewControl.AnnotationTileSource = new TiandituTileSource(_tiandituLayer == "img_w" ? "cia_w" : "cva_w", 0, _tiandituToken);
            //if (MapSourceSegmented.SelectedIndex == 1)
            //{
            //    _tiandituToken = _mapToken;
            //    MapViewControl.TileSource = new TiandituTileSource(_tiandituLayer, 0, _tiandituToken);
            //    MapViewControl.AnnotationTileSource = new TiandituTileSource(_tiandituLayer == "img_w" ? "cia_w" : "cva_w", 0, _tiandituToken);
            //}
        }

        //private void MapSourceSegmented_ItemClick(object sender, RoutedEventArgs e)
        //{
        //    if (MapViewControl == null)
        //    {
        //        return;
        //    }

        //    UpdateTokenInputVisibility();

        //    switch (MapSourceSegmented.SelectedIndex)
        //    {
        //        case 1:
        //            _mapToken = _tiandituToken;
        //            MapTokenTextBox.Text = _mapToken;
        //            _tiandituLayer = _currentMapStyle == 6 ? "img_w" : "vec_w";
        //            MapViewControl.TileSource = new TiandituTileSource(_tiandituLayer, 0, _tiandituToken);
        //            MapViewControl.AnnotationTileSource = new TiandituTileSource(_tiandituLayer == "img_w" ? "cia_w" : "cva_w", 0, _tiandituToken);
        //            break;
        //        default:
        //            _mapToken = string.Empty;
        //            MapTokenTextBox.Text = string.Empty;
        //            MapViewControl.TileSource = new AMapTileSource(_currentMapStyle);
        //            MapViewControl.AnnotationTileSource = null;
        //            break;
        //    }
        //}

        //private void UpdateTokenInputVisibility()
        //{
        //    if (MapTokenBorder == null || MapTokenTextBox == null || MapTokenTitle == null || MapSourceSegmented == null)
        //    {
        //        return;
        //    }

        //    var isVisible = MapSourceSegmented.SelectedIndex == 1;
        //    MapTokenBorder.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;

        //    if (MapSourceSegmented.SelectedIndex == 1)
        //    {
        //        MapTokenTitle.Text = "天地图 Token";
        //        MapTokenTextBox.Text = _tiandituToken;
        //        //WPFDevelopers.Helpers.ElementHelper.SetWatermark(MapTokenTextBox, "请输入天地图 token");
        //    }
        //    else
        //    {
        //        MapTokenTextBox.Text = string.Empty;
        //    }
        //}

        private void OpenMapViewDemo_Click(object sender, RoutedEventArgs e)
        {
            var window = new MapViewFeaturesWindow
            {
                Owner = Window.GetWindow(this)
            };
            window.Show();
        }

        private void MapView_MapFeatureClicked(object sender, RoutedEventArgs e)
        {
            var featureArgs = e as MapFeatureClickEventArgs;
            if (featureArgs == null)
            {
                return;
            }

            if (featureArgs.FeatureType == MapFeatureType.Circle && featureArgs.ClickedCircle != null)
            {
                Toast.Push("此为空域管制区",ToastImage.Warning,true);
            }
        }

    }
}
