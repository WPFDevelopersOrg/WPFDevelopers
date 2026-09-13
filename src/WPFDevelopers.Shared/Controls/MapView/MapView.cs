using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WPFDevelopers.Controls
{
    [ContentProperty(nameof(InlinePushpins))]
    public class MapView : FrameworkElement
    {
        private const int TileSize = 256;
        private const int AbsoluteMinZoom = 1;
        private const int AbsoluteMaxZoom = 22;
        private const int DefaultMinZoom = 4;
        private const int DefaultMaxZoom = 19;
        private const string BaseTileCachePrefix = "B:";
        private const string AnnotationTileCachePrefix = "A:";
        private const double EarthRadiusMeters = 6378137d;
        private const double FeetPerMeter = 3.2808399;
        private const double FeetPerMile = 5280d;

        private readonly Dictionary<string, BitmapSource> _tileCache = new Dictionary<string, BitmapSource>();
        private readonly Dictionary<string, DateTime> _failedTiles = new Dictionary<string, DateTime>();
        private readonly Dictionary<string, int> _failedTileAttempts = new Dictionary<string, int>();
        private readonly HashSet<string> _loadingTiles = new HashSet<string>();
        private readonly HashSet<string> _queuedTiles = new HashSet<string>();
        private readonly Queue<TileRequest> _pendingTiles = new Queue<TileRequest>();
        private readonly MapTileLayer _baseLayer;
        private readonly MapTileLayer _annotationLayer;
        private readonly MapShapeLayer _shapeLayer;
        private readonly MapItemsLayer _itemsLayer;
        private readonly MapItemsLayer _templatedItemsLayer;
        private readonly MapOverlayLayer _overlayLayer;
        private readonly object _cacheLock = new object();
        private MapTileSource _tileSource;
        private bool _usingTemplateCompatibilitySource;
        private readonly DispatcherTimer _renderThrottleTimer;

        private int _activeTileDownloads;
        private int _tileSourceVersion;
        private bool _renderInvalidated;
        private string _lastTileError;
        private string _lastViewportKey;

        private bool _isDragging;
        private bool _hasDragged;
        private Point _dragStartPoint;
        private Point _dragStartCenterPixel;
        private Pushpin _hoveredPushpin;
        private Pushpin _selectedPushpin;
        private Pushpin _activatedPushpin;
        private readonly ToolTip _hoverToolTip;
        private readonly DispatcherTimer _pushpinActivationTimer;

        private static readonly TimeSpan FailedRetryDelay = TimeSpan.FromSeconds(6);

        static MapView()
        {
            EnsureNetworkSecuritySettings();
            SnapsToDevicePixelsProperty.OverrideMetadata(typeof(MapView), new FrameworkPropertyMetadata(true));
            FocusableProperty.OverrideMetadata(typeof(MapView), new FrameworkPropertyMetadata(true));
            ClipToBoundsProperty.OverrideMetadata(typeof(MapView), new FrameworkPropertyMetadata(true));
            ServicePointManager.DefaultConnectionLimit = Math.Max(ServicePointManager.DefaultConnectionLimit, 8);
        }

        private static void EnsureNetworkSecuritySettings()
        {
            try
            {
                const SecurityProtocolType tls12 = (SecurityProtocolType)3072;
                const SecurityProtocolType tls13 = (SecurityProtocolType)12288;
                ServicePointManager.SecurityProtocol = tls12 | tls13;
                ServicePointManager.Expect100Continue = false;
            }
            catch
            {
                try
                {
                    const SecurityProtocolType tls12 = (SecurityProtocolType)3072;
                    ServicePointManager.SecurityProtocol = tls12;
                    ServicePointManager.Expect100Continue = false;
                }
                catch
                {
                }
            }
        }

        public MapView()
        {
            Pushpins = new ObservableCollection<Pushpin>();
            PushpinLayers = new ObservableCollection<MapPushpinLayer>();
            Polylines = new ObservableCollection<MapPolyline>();
            Polygons = new ObservableCollection<MapPolygon>();
            InlinePushpins = new ObservableCollection<Pushpin>();
            _tileSource = CreateCompatibilityTileSource();
            _usingTemplateCompatibilitySource = true;
            _baseLayer = new MapTileLayer { TileSource = _tileSource };
            _annotationLayer = new MapTileLayer();
            _shapeLayer = new MapShapeLayer();
            _itemsLayer = new MapItemsLayer();
            _templatedItemsLayer = new MapItemsLayer();
            _overlayLayer = new MapOverlayLayer { RenderContent = DrawOverlayContent };
            AddVisualChild(_baseLayer);
            AddVisualChild(_annotationLayer);
            AddVisualChild(_shapeLayer);
            AddVisualChild(_itemsLayer);
            AddVisualChild(_templatedItemsLayer);
            AddVisualChild(_overlayLayer);
            AddLogicalChild(_baseLayer);
            AddLogicalChild(_annotationLayer);
            AddLogicalChild(_shapeLayer);
            AddLogicalChild(_itemsLayer);
            AddLogicalChild(_templatedItemsLayer);
            AddLogicalChild(_overlayLayer);

            _renderThrottleTimer = new DispatcherTimer();
            _renderThrottleTimer.Interval = TimeSpan.FromMilliseconds(80);
            _renderThrottleTimer.Tick += RenderThrottleTimer_Tick;

            _pushpinActivationTimer = new DispatcherTimer();
            _pushpinActivationTimer.Interval = TimeSpan.FromMilliseconds(720);
            _pushpinActivationTimer.Tick += PushpinActivationTimer_Tick;

            _hoverToolTip = new ToolTip
            {
                PlacementTarget = this,
                Placement = PlacementMode.MousePoint,
                StaysOpen = true
            };

            InlinePushpins.CollectionChanged += InlinePushpins_CollectionChanged;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public ObservableCollection<Pushpin> Pushpins
        {
            get { return (ObservableCollection<Pushpin>)GetValue(PushpinsProperty); }
            set { SetValue(PushpinsProperty, value); }
        }

        public static readonly DependencyProperty PushpinsProperty =
            DependencyProperty.Register(nameof(Pushpins), typeof(ObservableCollection<Pushpin>), typeof(MapView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnPushpinsChanged));

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<Pushpin> InlinePushpins { get; private set; }

        public ObservableCollection<MapPushpinLayer> PushpinLayers
        {
            get { return (ObservableCollection<MapPushpinLayer>)GetValue(PushpinLayersProperty); }
            set { SetValue(PushpinLayersProperty, value); }
        }

        public static readonly DependencyProperty PushpinLayersProperty =
            DependencyProperty.Register(nameof(PushpinLayers), typeof(ObservableCollection<MapPushpinLayer>), typeof(MapView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnPushpinLayersChanged));

        public ObservableCollection<MapPolyline> Polylines
        {
            get { return (ObservableCollection<MapPolyline>)GetValue(PolylinesProperty); }
            set { SetValue(PolylinesProperty, value); }
        }

        public static readonly DependencyProperty PolylinesProperty =
            DependencyProperty.Register(nameof(Polylines), typeof(ObservableCollection<MapPolyline>), typeof(MapView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnPolylinesChanged));

        public ObservableCollection<MapPolygon> Polygons
        {
            get { return (ObservableCollection<MapPolygon>)GetValue(PolygonsProperty); }
            set { SetValue(PolygonsProperty, value); }
        }

        public static readonly DependencyProperty PolygonsProperty =
            DependencyProperty.Register(nameof(Polygons), typeof(ObservableCollection<MapPolygon>), typeof(MapView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnPolygonsChanged));

        public double CenterLatitude
        {
            get { return (double)GetValue(CenterLatitudeProperty); }
            set { SetValue(CenterLatitudeProperty, value); }
        }

        public static readonly DependencyProperty CenterLatitudeProperty =
            DependencyProperty.Register(nameof(CenterLatitude), typeof(double), typeof(MapView),
                new FrameworkPropertyMetadata(39.9042, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, OnViewportPropertyChanged, CoerceLatitude));

        public double CenterLongitude
        {
            get { return (double)GetValue(CenterLongitudeProperty); }
            set { SetValue(CenterLongitudeProperty, value); }
        }

        public static readonly DependencyProperty CenterLongitudeProperty =
            DependencyProperty.Register(nameof(CenterLongitude), typeof(double), typeof(MapView),
                new FrameworkPropertyMetadata(116.4074, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, OnViewportPropertyChanged, CoerceLongitude));

        public int ZoomLevel
        {
            get { return (int)GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }

        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register(nameof(ZoomLevel), typeof(int), typeof(MapView),
                new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, OnViewportPropertyChanged, CoerceZoom));

        public int MinZoomLevel
        {
            get { return (int)GetValue(MinZoomLevelProperty); }
            set { SetValue(MinZoomLevelProperty, value); }
        }

        public static readonly DependencyProperty MinZoomLevelProperty =
            DependencyProperty.Register(nameof(MinZoomLevel), typeof(int), typeof(MapView),
                new FrameworkPropertyMetadata(DefaultMinZoom, FrameworkPropertyMetadataOptions.AffectsRender, OnZoomRangePropertyChanged, CoerceMinZoomLevel));

        public int MaxZoomLevel
        {
            get { return (int)GetValue(MaxZoomLevelProperty); }
            set { SetValue(MaxZoomLevelProperty, value); }
        }

        public static readonly DependencyProperty MaxZoomLevelProperty =
            DependencyProperty.Register(nameof(MaxZoomLevel), typeof(int), typeof(MapView),
                new FrameworkPropertyMetadata(DefaultMaxZoom, FrameworkPropertyMetadataOptions.AffectsRender, OnZoomRangePropertyChanged, CoerceMaxZoomLevel));

        public string TileUrlTemplate
        {
            get { return (string)GetValue(TileUrlTemplateProperty); }
            set { SetValue(TileUrlTemplateProperty, value); }
        }

        public static readonly DependencyProperty TileUrlTemplateProperty =
            DependencyProperty.Register(nameof(TileUrlTemplate), typeof(string), typeof(MapView),
                new FrameworkPropertyMetadata(
                    "http://wprd01.is.autonavi.com/appmaptile?x={x}&y={y}&z={z}&lang=zh_cn&size=1&scl=1&style={style}",
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnTileTemplateChanged));

        public string TileSubdomains
        {
            get { return (string)GetValue(TileSubdomainsProperty); }
            set { SetValue(TileSubdomainsProperty, value); }
        }

        public static readonly DependencyProperty TileSubdomainsProperty =
            DependencyProperty.Register(nameof(TileSubdomains), typeof(string), typeof(MapView),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender, OnTileTemplateChanged));

        public bool UseTmsY
        {
            get { return (bool)GetValue(UseTmsYProperty); }
            set { SetValue(UseTmsYProperty, value); }
        }

        public static readonly DependencyProperty UseTmsYProperty =
            DependencyProperty.Register(nameof(UseTmsY), typeof(bool), typeof(MapView),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnTileTemplateChanged));

        public int TileStyle
        {
            get { return (int)GetValue(TileStyleProperty); }
            set { SetValue(TileStyleProperty, value); }
        }

        public static readonly DependencyProperty TileStyleProperty =
            DependencyProperty.Register(nameof(TileStyle), typeof(int), typeof(MapView),
                new FrameworkPropertyMetadata(7, FrameworkPropertyMetadataOptions.AffectsRender, OnTileTemplateChanged));

        public int MaxConcurrentTileDownloads
        {
            get { return (int)GetValue(MaxConcurrentTileDownloadsProperty); }
            set { SetValue(MaxConcurrentTileDownloadsProperty, value); }
        }

        public static readonly DependencyProperty MaxConcurrentTileDownloadsProperty =
            DependencyProperty.Register(nameof(MaxConcurrentTileDownloads), typeof(int), typeof(MapView),
                new FrameworkPropertyMetadata(6, OnTileTemplateChanged, CoerceMaxConcurrentTileDownloads));

        public MapTileSource AnnotationTileSource
        {
            get { return (MapTileSource)GetValue(AnnotationTileSourceProperty); }
            set { SetValue(AnnotationTileSourceProperty, value); }
        }

        public static readonly DependencyProperty AnnotationTileSourceProperty =
            DependencyProperty.Register(nameof(AnnotationTileSource), typeof(MapTileSource), typeof(MapView),
                new FrameworkPropertyMetadata(null, OnAnnotationTileSourceChanged));

        public MapTileSource TileSource
        {
            get { return _tileSource; }
            set
            {
                _tileSource = value ?? CreateCompatibilityTileSource();
                _usingTemplateCompatibilitySource = _tileSource is TemplateMapTileSource;
                if (_baseLayer != null)
                {
                    _baseLayer.TileSource = _tileSource;
                }

                lock (_cacheLock)
                {
                    _tileSourceVersion++;
                }

                ClearTileCache();
                CoerceValue(ZoomLevelProperty);
                RefreshOverlay();
            }
        }

        private static void OnAnnotationTileSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MapView;
            if (control == null)
            {
                return;
            }

            if (control._annotationLayer != null)
            {
                control._annotationLayer.TileSource = e.NewValue as MapTileSource;
            }

            lock (control._cacheLock)
            {
                control._tileSourceVersion++;
            }

            control.ClearTileCache();
            control.CoerceValue(ZoomLevelProperty);
            control.RefreshOverlay();
        }

        public void SetTileSource(MapTileSource source)
        {
            TileSource = source;
        }

        public void SetTileSource(string template, string subdomains = "", bool useTmsY = false, int style = 7)
        {
            TileSource = MapTileSourceFactory.Create(template, subdomains, useTmsY, style);
        }

        public void SelectPushpin(Pushpin pushpin, bool activate = true)
        {
            if (!ReferenceEquals(_selectedPushpin, pushpin))
            {
                if (_selectedPushpin != null)
                {
                    _selectedPushpin.IsSelected = false;
                }

                _selectedPushpin = pushpin;

                if (_selectedPushpin != null)
                {
                    _selectedPushpin.IsSelected = true;
                }
            }

            if (activate)
            {
                ActivatePushpin(pushpin);
            }
            else if (pushpin == null)
            {
                ClearActivatedPushpin();
            }

            RefreshOverlay();
        }

        public void ActivatePushpin(Pushpin pushpin)
        {
            if (pushpin == null)
            {
                ClearActivatedPushpin();
                return;
            }

            if (_activatedPushpin != null && !ReferenceEquals(_activatedPushpin, pushpin))
            {
                _activatedPushpin.IsActivated = false;
            }

            _activatedPushpin = pushpin;
            _activatedPushpin.IsActivated = true;
            _pushpinActivationTimer.Stop();
            _pushpinActivationTimer.Start();
            RefreshOverlay();
        }

        public Brush PushpinBrush
        {
            get { return (Brush)GetValue(PushpinBrushProperty); }
            set { SetValue(PushpinBrushProperty, value); }
        }

        public static readonly DependencyProperty PushpinBrushProperty =
            DependencyProperty.Register(nameof(PushpinBrush), typeof(Brush), typeof(MapView),
                new FrameworkPropertyMetadata(CreateDefaultPushpinBrush(), FrameworkPropertyMetadataOptions.AffectsRender));

        private static Brush CreateDefaultPushpinBrush()
        {
            var brush = new SolidColorBrush(Color.FromRgb(0xE7, 0x4A, 0x4A));
            if (brush.CanFreeze)
                brush.Freeze();
            return brush;
        }

        public double PushpinSize
        {
            get { return (double)GetValue(PushpinSizeProperty); }
            set { SetValue(PushpinSizeProperty, value); }
        }

        public static readonly DependencyProperty PushpinSizeProperty =
            DependencyProperty.Register(nameof(PushpinSize), typeof(double), typeof(MapView),
                new FrameworkPropertyMetadata(12d, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool ZoomOnPushpinClick
        {
            get { return (bool)GetValue(ZoomOnPushpinClickProperty); }
            set { SetValue(ZoomOnPushpinClickProperty, value); }
        }

        public static readonly DependencyProperty ZoomOnPushpinClickProperty =
            DependencyProperty.Register(nameof(ZoomOnPushpinClick), typeof(bool), typeof(MapView),
                new FrameworkPropertyMetadata(true));

        public int PushpinClickZoomLevel
        {
            get { return (int)GetValue(PushpinClickZoomLevelProperty); }
            set { SetValue(PushpinClickZoomLevelProperty, value); }
        }

        public static readonly DependencyProperty PushpinClickZoomLevelProperty =
            DependencyProperty.Register(nameof(PushpinClickZoomLevel), typeof(int), typeof(MapView),
                new FrameworkPropertyMetadata(16, null, CoerceZoom));

        public bool EnableClustering
        {
            get { return (bool)GetValue(EnableClusteringProperty); }
            set { SetValue(EnableClusteringProperty, value); }
        }

        public static readonly DependencyProperty EnableClusteringProperty =
            DependencyProperty.Register(nameof(EnableClustering), typeof(bool), typeof(MapView),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, OnClusterLayoutPropertyChanged));

        public double ClusterRadius
        {
            get { return (double)GetValue(ClusterRadiusProperty); }
            set { SetValue(ClusterRadiusProperty, value); }
        }

        public static readonly DependencyProperty ClusterRadiusProperty =
            DependencyProperty.Register(nameof(ClusterRadius), typeof(double), typeof(MapView),
                new FrameworkPropertyMetadata(28d, FrameworkPropertyMetadataOptions.AffectsRender, OnClusterLayoutPropertyChanged));

        public int ClusterMinimumCount
        {
            get { return (int)GetValue(ClusterMinimumCountProperty); }
            set { SetValue(ClusterMinimumCountProperty, value); }
        }

        public static readonly DependencyProperty ClusterMinimumCountProperty =
            DependencyProperty.Register(nameof(ClusterMinimumCount), typeof(int), typeof(MapView),
                new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.AffectsRender, OnClusterLayoutPropertyChanged));

        public Brush ClusterBrush
        {
            get { return (Brush)GetValue(ClusterBrushProperty); }
            set { SetValue(ClusterBrushProperty, value); }
        }

        public static readonly DependencyProperty ClusterBrushProperty =
            DependencyProperty.Register(nameof(ClusterBrush), typeof(Brush), typeof(MapView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush ClusterTextBrush
        {
            get { return (Brush)GetValue(ClusterTextBrushProperty); }
            set { SetValue(ClusterTextBrushProperty, value); }
        }

        public static readonly DependencyProperty ClusterTextBrushProperty =
            DependencyProperty.Register(nameof(ClusterTextBrush), typeof(Brush), typeof(MapView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool ShowScaleBar
        {
            get { return (bool)GetValue(ShowScaleBarProperty); }
            set { SetValue(ShowScaleBarProperty, value); }
        }

        public static readonly DependencyProperty ShowScaleBarProperty =
            DependencyProperty.Register(nameof(ShowScaleBar), typeof(bool), typeof(MapView),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool ShowZoomControls
        {
            get { return (bool)GetValue(ShowZoomControlsProperty); }
            set { SetValue(ShowZoomControlsProperty, value); }
        }

        public static readonly DependencyProperty ShowZoomControlsProperty =
            DependencyProperty.Register(nameof(ShowZoomControls), typeof(bool), typeof(MapView),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public MapZoomControlPosition ZoomControlPosition
        {
            get { return (MapZoomControlPosition)GetValue(ZoomControlPositionProperty); }
            set { SetValue(ZoomControlPositionProperty, value); }
        }

        public static readonly DependencyProperty ZoomControlPositionProperty =
            DependencyProperty.Register(nameof(ZoomControlPosition), typeof(MapZoomControlPosition), typeof(MapView),
                new FrameworkPropertyMetadata(MapZoomControlPosition.TopRight, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool ShowDiagnostics
        {
            get { return (bool)GetValue(ShowDiagnosticsProperty); }
            set { SetValue(ShowDiagnosticsProperty, value); }
        }

        public static readonly DependencyProperty ShowDiagnosticsProperty =
            DependencyProperty.Register(nameof(ShowDiagnostics), typeof(bool), typeof(MapView),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public MapDistanceUnit ScaleBarUnit
        {
            get { return (MapDistanceUnit)GetValue(ScaleBarUnitProperty); }
            set { SetValue(ScaleBarUnitProperty, value); }
        }

        public static readonly DependencyProperty ScaleBarUnitProperty =
            DependencyProperty.Register(nameof(ScaleBarUnit), typeof(MapDistanceUnit), typeof(MapView),
                new FrameworkPropertyMetadata(MapDistanceUnit.Auto, FrameworkPropertyMetadataOptions.AffectsRender));

        public double ScaleBarMaxWidth
        {
            get { return (double)GetValue(ScaleBarMaxWidthProperty); }
            set { SetValue(ScaleBarMaxWidthProperty, value); }
        }

        public static readonly DependencyProperty ScaleBarMaxWidthProperty =
            DependencyProperty.Register(nameof(ScaleBarMaxWidth), typeof(double), typeof(MapView),
                new FrameworkPropertyMetadata(140d, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush ScaleBarStroke
        {
            get { return (Brush)GetValue(ScaleBarStrokeProperty); }
            set { SetValue(ScaleBarStrokeProperty, value); }
        }

        public static readonly DependencyProperty ScaleBarStrokeProperty =
            DependencyProperty.Register(nameof(ScaleBarStroke), typeof(Brush), typeof(MapView),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x2B, 0x2B, 0x2B)), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush ScaleBarTextBrush
        {
            get { return (Brush)GetValue(ScaleBarTextBrushProperty); }
            set { SetValue(ScaleBarTextBrushProperty, value); }
        }

        public static readonly DependencyProperty ScaleBarTextBrushProperty =
            DependencyProperty.Register(nameof(ScaleBarTextBrush), typeof(Brush), typeof(MapView),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x47, 0x55, 0x69)), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush ScaleBarBackground
        {
            get { return (Brush)GetValue(ScaleBarBackgroundProperty); }
            set { SetValue(ScaleBarBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ScaleBarBackgroundProperty =
            DependencyProperty.Register(nameof(ScaleBarBackground), typeof(Brush), typeof(MapView),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(0xEC, 0xFF, 0xFF, 0xFF)), FrameworkPropertyMetadataOptions.AffectsRender));

        public Thickness ScaleBarMargin
        {
            get { return (Thickness)GetValue(ScaleBarMarginProperty); }
            set { SetValue(ScaleBarMarginProperty, value); }
        }

        public static readonly DependencyProperty ScaleBarMarginProperty =
            DependencyProperty.Register(nameof(ScaleBarMargin), typeof(Thickness), typeof(MapView),
                new FrameworkPropertyMetadata(new Thickness(12), FrameworkPropertyMetadataOptions.AffectsRender));

        public MapScaleBarPosition ScaleBarPosition
        {
            get { return (MapScaleBarPosition)GetValue(ScaleBarPositionProperty); }
            set { SetValue(ScaleBarPositionProperty, value); }
        }

        public static readonly DependencyProperty ScaleBarPositionProperty =
            DependencyProperty.Register(nameof(ScaleBarPosition), typeof(MapScaleBarPosition), typeof(MapView),
                new FrameworkPropertyMetadata(MapScaleBarPosition.BottomLeft, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush PushpinStroke
        {
            get { return (Brush)GetValue(PushpinStrokeProperty); }
            set { SetValue(PushpinStrokeProperty, value); }
        }

        public static readonly DependencyProperty PushpinStrokeProperty =
            DependencyProperty.Register(nameof(PushpinStroke), typeof(Brush), typeof(MapView),
                new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

        public double PushpinStrokeThickness
        {
            get { return (double)GetValue(PushpinStrokeThicknessProperty); }
            set { SetValue(PushpinStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty PushpinStrokeThicknessProperty =
            DependencyProperty.Register(nameof(PushpinStrokeThickness), typeof(double), typeof(MapView),
                new FrameworkPropertyMetadata(1.5d, FrameworkPropertyMetadataOptions.AffectsRender));

        public ImageSource PushpinImageSource
        {
            get { return (ImageSource)GetValue(PushpinImageSourceProperty); }
            set { SetValue(PushpinImageSourceProperty, value); }
        }

        public static readonly DependencyProperty PushpinImageSourceProperty =
            DependencyProperty.Register(nameof(PushpinImageSource), typeof(ImageSource), typeof(MapView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public PushpinVisualMode PushpinRenderMode
        {
            get { return (PushpinVisualMode)GetValue(PushpinRenderModeProperty); }
            set { SetValue(PushpinRenderModeProperty, value); }
        }

        public static readonly DependencyProperty PushpinRenderModeProperty =
            DependencyProperty.Register(nameof(PushpinRenderMode), typeof(PushpinVisualMode), typeof(MapView),
                new FrameworkPropertyMetadata(PushpinVisualMode.Dot, FrameworkPropertyMetadataOptions.AffectsRender));

        public DataTemplate PushpinTemplate
        {
            get { return (DataTemplate)GetValue(PushpinTemplateProperty); }
            set { SetValue(PushpinTemplateProperty, value); }
        }

        public static readonly DependencyProperty PushpinTemplateProperty =
            DependencyProperty.Register(nameof(PushpinTemplate), typeof(DataTemplate), typeof(MapView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnPushpinTemplateChanged));

        public DataTemplate ClusterTemplate
        {
            get { return (DataTemplate)GetValue(ClusterTemplateProperty); }
            set { SetValue(ClusterTemplateProperty, value); }
        }

        public static readonly DependencyProperty ClusterTemplateProperty =
            DependencyProperty.Register(nameof(ClusterTemplate), typeof(DataTemplate), typeof(MapView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnClusterTemplateChanged));

        private static void OnPushpinTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MapView;
            if (control == null)
            {
                return;
            }

            control.RebuildPushpinPresenters();
            control.RefreshOverlay();
        }

        private static void OnClusterTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MapView;
            if (control == null)
            {
                return;
            }

            control.RebuildPushpinPresenters();
            control.RefreshOverlay();
        }

        private static void OnClusterLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MapView;
            if (control == null)
            {
                return;
            }

            control.RefreshOverlay();
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                return new UIElement[] { _baseLayer, _annotationLayer, _shapeLayer, _itemsLayer, _templatedItemsLayer, _overlayLayer }.GetEnumerator();
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 6;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            switch (index)
            {
                case 0:
                    return _baseLayer;
                case 1:
                    return _annotationLayer;
                case 2:
                    return _shapeLayer;
                case 3:
                    return _itemsLayer;
                case 4:
                    return _templatedItemsLayer;
                case 5:
                    return _overlayLayer;
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_baseLayer == null || _annotationLayer == null || _shapeLayer == null || _itemsLayer == null || _templatedItemsLayer == null || _overlayLayer == null)
            {
                return new Size(0, 0);
            }

            var viewportSize = new Size(
                double.IsInfinity(availableSize.Width) ? ActualWidth : availableSize.Width,
                double.IsInfinity(availableSize.Height) ? ActualHeight : availableSize.Height);

            var zoom = (int)CoerceZoom(this, ZoomLevel);
            var centerPixel = ToPixel(CenterLatitude, CenterLongitude, zoom);
            var viewTopLeft = new Point(centerPixel.X - viewportSize.Width / 2.0, centerPixel.Y - viewportSize.Height / 2.0);
            var viewportKey = string.Format(
                CultureInfo.InvariantCulture,
                "{0}:{1}:{2}:{3}:{4}",
                zoom,
                Math.Round(centerPixel.X, 3),
                Math.Round(centerPixel.Y, 3),
                Math.Round(viewportSize.Width, 3),
                Math.Round(viewportSize.Height, 3));

            if (!string.Equals(_lastViewportKey, viewportKey, StringComparison.Ordinal))
            {
                _lastViewportKey = viewportKey;
                QueueViewportTiles(zoom, viewTopLeft, viewportSize, _tileSource, BaseTileCachePrefix);
                if (AnnotationTileSource != null)
                {
                    QueueViewportTiles(zoom, viewTopLeft, viewportSize, AnnotationTileSource, AnnotationTileCachePrefix);
                }
            }

            _baseLayer.UpdateViewport(zoom, viewTopLeft, viewportSize, _tileCache, BaseTileCachePrefix);
            if (AnnotationTileSource != null)
            {
                _annotationLayer.UpdateViewport(zoom, viewTopLeft, viewportSize, _tileCache, AnnotationTileCachePrefix);
            }
            else
            {
                _annotationLayer.UpdateViewport(zoom, viewTopLeft, new Size(0, 0), _tileCache, AnnotationTileCachePrefix);
            }
            UpdateMapShapesLayer(zoom, viewTopLeft, viewportSize);
            UpdateMapItemsLayer(viewportSize);
            _baseLayer.Measure(availableSize);
            _annotationLayer.Measure(availableSize);
            _shapeLayer.Measure(availableSize);
            _itemsLayer.Measure(availableSize);
            _templatedItemsLayer.Measure(availableSize);
            _overlayLayer.Measure(availableSize);
            var width = double.IsInfinity(availableSize.Width) ? 0d : availableSize.Width;
            var height = double.IsInfinity(availableSize.Height) ? 0d : availableSize.Height;
            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _baseLayer.Width = finalSize.Width;
            _baseLayer.Height = finalSize.Height;
            _baseLayer.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));

            _annotationLayer.Width = finalSize.Width;
            _annotationLayer.Height = finalSize.Height;
            _annotationLayer.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));

            _shapeLayer.Width = finalSize.Width;
            _shapeLayer.Height = finalSize.Height;
            _shapeLayer.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));

            _itemsLayer.Width = finalSize.Width;
            _itemsLayer.Height = finalSize.Height;
            _itemsLayer.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));

            _templatedItemsLayer.Width = finalSize.Width;
            _templatedItemsLayer.Height = finalSize.Height;
            _templatedItemsLayer.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));

            _overlayLayer.Width = finalSize.Width;
            _overlayLayer.Height = finalSize.Height;
            _overlayLayer.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return finalSize;
        }

        private void RefreshOverlay()
        {
            if (_shapeLayer == null || _itemsLayer == null || _templatedItemsLayer == null || _overlayLayer == null)
            {
                return;
            }

            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();
            _shapeLayer.InvalidateMeasure();
            _shapeLayer.InvalidateArrange();
            _shapeLayer.InvalidateVisual();
            _itemsLayer.InvalidateMeasure();
            _itemsLayer.InvalidateArrange();
            _itemsLayer.InvalidateVisual();
            _templatedItemsLayer.InvalidateMeasure();
            _templatedItemsLayer.InvalidateArrange();
            _templatedItemsLayer.InvalidateVisual();
            _overlayLayer.InvalidateMeasure();
            _overlayLayer.InvalidateArrange();
            _overlayLayer.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(0xF6, 0xF8, 0xFB)), null, new Rect(0, 0, ActualWidth, ActualHeight));
            if (ActualWidth <= 0 || ActualHeight <= 0)
            {
                return;
            }

            drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));
            drawingContext.Pop();

        }

        private void DrawOverlayContent(DrawingContext drawingContext)
        {
            DrawZoomControls(drawingContext);
            DrawScaleBar(drawingContext);
            DrawDiagnostics(drawingContext);
        }

        private void DrawZoomControls(DrawingContext drawingContext)
        {
            if (!ShowZoomControls || ActualWidth <= 0 || ActualHeight <= 0)
            {
                return;
            }

            const double controlWidth = 30d;
            const double buttonHeight = 28d;
            const double gap = 0.5d;
            var margin = new Thickness(12d, 12d, 12d, 12d);
            var left = CalculateZoomControlLeft(controlWidth, margin);
            var top = CalculateZoomControlTop(buttonHeight, gap, margin);
            var panelBrush = new SolidColorBrush(Color.FromArgb(240, 255, 255, 255));
            var panelPen = new Pen(new SolidColorBrush(Color.FromArgb(255, 214, 219, 225)), 1d);
            var separatorBrush = new SolidColorBrush(Color.FromArgb(255, 229, 233, 238));
            var buttonTextBrush = new SolidColorBrush(Color.FromArgb(255, 35, 43, 53));

            var plusRect = new Rect(left, top, controlWidth, buttonHeight);
            var minusRect = new Rect(left, top + buttonHeight + gap, controlWidth, buttonHeight);

            var plusGeometry = CreateZoomButtonGeometry(plusRect, true);
            var minusGeometry = CreateZoomButtonGeometry(minusRect, false);

            drawingContext.DrawGeometry(panelBrush, panelPen, plusGeometry);
            drawingContext.DrawGeometry(panelBrush, panelPen, minusGeometry);
            drawingContext.DrawLine(new Pen(separatorBrush, 0.4d), new Point(left + 3d, top + buttonHeight), new Point(left + controlWidth - 3d, top + buttonHeight));

            var centerX = left + controlWidth / 2d;
            var plusY = top + buttonHeight / 2d;
            var minusY = top + buttonHeight + gap + buttonHeight / 2d;
            var linePen = new Pen(buttonTextBrush, 2d);

            drawingContext.DrawLine(linePen, new Point(centerX - 6.5d, plusY), new Point(centerX + 6.5d, plusY));
            drawingContext.DrawLine(linePen, new Point(centerX, plusY - 6.5d), new Point(centerX, plusY + 6.5d));
            drawingContext.DrawLine(linePen, new Point(centerX - 6.5d, minusY), new Point(centerX + 6.5d, minusY));
        }

        private static Geometry CreateZoomButtonGeometry(Rect rect, bool isTopButton)
        {
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                var left = rect.Left;
                var top = rect.Top;
                var right = rect.Right;
                var bottom = rect.Bottom;
                var radius = 8d;

                context.BeginFigure(new Point(left + radius, top), true, true);
                if (isTopButton)
                {
                    context.LineTo(new Point(right - radius, top), true, true);
                    context.ArcTo(new Point(right, top + radius), new Size(radius, radius), 0, false, SweepDirection.Clockwise, true, true);
                    context.LineTo(new Point(right, bottom), true, true);
                    context.LineTo(new Point(left, bottom), true, true);
                    context.LineTo(new Point(left, top + radius), true, true);
                    context.ArcTo(new Point(left + radius, top), new Size(radius, radius), 0, false, SweepDirection.Clockwise, true, true);
                }
                else
                {
                    context.LineTo(new Point(right, top), true, true);
                    context.LineTo(new Point(right, bottom - radius), true, true);
                    context.ArcTo(new Point(right - radius, bottom), new Size(radius, radius), 0, false, SweepDirection.Clockwise, true, true);
                    context.LineTo(new Point(left + radius, bottom), true, true);
                    context.ArcTo(new Point(left, bottom - radius), new Size(radius, radius), 0, false, SweepDirection.Clockwise, true, true);
                    context.LineTo(new Point(left, top), true, true);
                    context.LineTo(new Point(left + radius, top), true, true);
                }
                context.Close();
            }

            return geometry;
        }

        private bool TryHandleZoomControlClick(Point position)
        {
            if (!ShowZoomControls || ActualWidth <= 0 || ActualHeight <= 0)
            {
                return false;
            }

            const double controlWidth = 30d;
            const double buttonHeight = 28d;
            const double gap = 0.5d;
            var margin = new Thickness(12d, 12d, 12d, 12d);
            var left = CalculateZoomControlLeft(controlWidth, margin);
            var top = CalculateZoomControlTop(buttonHeight, gap, margin);
            var plusRect = new Rect(left, top, controlWidth, buttonHeight);
            var minusRect = new Rect(left, top + buttonHeight + gap, controlWidth, buttonHeight);

            if (plusRect.Contains(position))
            {
                ApplyZoomDelta(1);
                return true;
            }

            if (minusRect.Contains(position))
            {
                ApplyZoomDelta(-1);
                return true;
            }

            return false;
        }

        private double CalculateZoomControlLeft(double controlWidth, Thickness margin)
        {
            switch (ZoomControlPosition)
            {
                case MapZoomControlPosition.TopLeft:
                case MapZoomControlPosition.BottomLeft:
                    return margin.Left;
                case MapZoomControlPosition.TopRight:
                case MapZoomControlPosition.BottomRight:
                default:
                    return ActualWidth - controlWidth - margin.Right;
            }
        }

        private double CalculateZoomControlTop(double buttonHeight, double gap, Thickness margin)
        {
            switch (ZoomControlPosition)
            {
                case MapZoomControlPosition.TopLeft:
                case MapZoomControlPosition.TopRight:
                    return margin.Top;
                case MapZoomControlPosition.BottomLeft:
                case MapZoomControlPosition.BottomRight:
                default:
                    return ActualHeight - (buttonHeight * 2d) - gap - margin.Bottom;
            }
        }

        private void ApplyZoomDelta(int delta)
        {
            var oldZoom = (int)CoerceZoom(this, ZoomLevel);
            var minZoom = GetEffectiveMinZoom();
            var maxZoom = GetEffectiveMaxZoom();
            var newZoom = Math.Max(minZoom, Math.Min(maxZoom, oldZoom + delta));
            if (newZoom == oldZoom)
            {
                return;
            }

            ZoomLevel = newZoom;
            RefreshOverlay();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            Focus();
            var zoomDelta = e.Delta > 0 ? 1 : (e.Delta < 0 ? -1 : 0);
            if (zoomDelta == 0)
            {
                return;
            }

            var oldZoom = (int)CoerceZoom(this, ZoomLevel);
            var minZoom = GetEffectiveMinZoom();
            var maxZoom = GetEffectiveMaxZoom();
            var newZoom = Math.Max(minZoom, Math.Min(maxZoom, oldZoom + zoomDelta));
            if (newZoom == oldZoom || ActualWidth <= 0 || ActualHeight <= 0)
            {
                return;
            }

            var mousePosition = e.GetPosition(this);
            var oldCenterPixel = ToPixel(CenterLatitude, CenterLongitude, oldZoom);

            var worldPixelX = oldCenterPixel.X - (ActualWidth / 2.0) + mousePosition.X;
            var worldPixelY = oldCenterPixel.Y - (ActualHeight / 2.0) + mousePosition.Y;
            var anchorLatLon = ToLatLon(worldPixelX, worldPixelY, oldZoom);

            var anchorPixelAfterZoom = ToPixel(anchorLatLon.X, anchorLatLon.Y, newZoom);
            var newCenterPixelX = anchorPixelAfterZoom.X - mousePosition.X + (ActualWidth / 2.0);
            var newCenterPixelY = anchorPixelAfterZoom.Y - mousePosition.Y + (ActualHeight / 2.0);
            var newCenterLatLon = ToLatLon(newCenterPixelX, newCenterPixelY, newZoom);

            ZoomLevel = newZoom;
            CenterLatitude = newCenterLatLon.X;
            CenterLongitude = newCenterLatLon.Y;
            RefreshOverlay();

            e.Handled = true;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (TryHandleZoomControlClick(e.GetPosition(this)))
            {
                e.Handled = true;
                return;
            }

            Focus();
            CaptureMouse();
            _isDragging = true;
            _hasDragged = false;
            _dragStartPoint = e.GetPosition(this);
            _dragStartCenterPixel = ToPixel(CenterLatitude, CenterLongitude, ZoomLevel);
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var position = e.GetPosition(this);
            UpdateHoveredPushpin(position);

            if (!_isDragging)
            {
                return;
            }

            var deltaX = position.X - _dragStartPoint.X;
            var deltaY = position.Y - _dragStartPoint.Y;
            if (!_hasDragged && (Math.Abs(deltaX) > 2 || Math.Abs(deltaY) > 2))
            {
                _hasDragged = true;
            }

            var newCenterPixel = new Point(_dragStartCenterPixel.X - deltaX, _dragStartCenterPixel.Y - deltaY);
            var latLon = ToLatLon(newCenterPixel.X, newCenterPixel.Y, ZoomLevel);

            CenterLatitude = latLon.X;
            CenterLongitude = latLon.Y;
            RefreshOverlay();
            e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (!_isDragging)
            {
                return;
            }

            _isDragging = false;
            ReleaseMouseCapture();

            if (!_hasDragged)
            {
                var clickedPushpin = GetPushpinAt(e.GetPosition(this));
                if (clickedPushpin != null)
                {
                    SelectPushpin(clickedPushpin, true);
                    CenterLatitude = clickedPushpin.Latitude;
                    CenterLongitude = clickedPushpin.Longitude;
                    if (ZoomOnPushpinClick)
                    {
                        ZoomLevel = Math.Max(ZoomLevel, PushpinClickZoomLevel);
                    }
                }
            }

            e.Handled = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            UpdateHoveredPushpin(new Point(double.NaN, double.NaN));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            var step = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? 220d : 80d;
            var handled = false;

            switch (e.Key)
            {
                case Key.Left:
                    PanByPixels(-step, 0d);
                    handled = true;
                    break;
                case Key.Right:
                    PanByPixels(step, 0d);
                    handled = true;
                    break;
                case Key.Up:
                    PanByPixels(0d, -step);
                    handled = true;
                    break;
                case Key.Down:
                    PanByPixels(0d, step);
                    handled = true;
                    break;
            }

            if (handled)
            {
                e.Handled = true;
            }
        }

        private void PanByPixels(double deltaX, double deltaY)
        {
            if (ActualWidth <= 0 || ActualHeight <= 0)
            {
                return;
            }

            var centerPixel = ToPixel(CenterLatitude, CenterLongitude, ZoomLevel);
            var newCenterPixel = new Point(centerPixel.X + deltaX, centerPixel.Y + deltaY);
            var latLon = ToLatLon(newCenterPixel.X, newCenterPixel.Y, ZoomLevel);

            CenterLatitude = latLon.X;
            CenterLongitude = latLon.Y;
            RefreshOverlay();
        }

        private static object CoerceLatitude(DependencyObject d, object baseValue)
        {
            var value = (double)baseValue;
            return Math.Max(-85.05112878, Math.Min(85.05112878, value));
        }

        private static object CoerceLongitude(DependencyObject d, object baseValue)
        {
            var value = (double)baseValue;
            return NormalizeLongitude(value);
        }

        private static object CoerceZoom(DependencyObject d, object baseValue)
        {
            var value = (int)baseValue;
            var control = d as MapView;
            var minZoom = control == null ? DefaultMinZoom : control.GetEffectiveMinZoom();
            var maxZoom = control == null ? DefaultMaxZoom : control.GetEffectiveMaxZoom();
            return Math.Max(minZoom, Math.Min(maxZoom, value));
        }

        private static object CoerceMinZoomLevel(DependencyObject d, object baseValue)
        {
            var value = (int)baseValue;
            var min = Math.Max(AbsoluteMinZoom, value);
            var control = d as MapView;
            var max = control == null ? DefaultMaxZoom : control.MaxZoomLevel;
            min = Math.Min(min, Math.Max(AbsoluteMinZoom, max));
            return Math.Min(min, AbsoluteMaxZoom);
        }

        private static object CoerceMaxZoomLevel(DependencyObject d, object baseValue)
        {
            var value = (int)baseValue;
            var max = Math.Min(AbsoluteMaxZoom, value);
            var control = d as MapView;
            var min = control == null ? DefaultMinZoom : control.MinZoomLevel;
            max = Math.Max(max, Math.Min(AbsoluteMaxZoom, min));
            return Math.Max(max, AbsoluteMinZoom);
        }

        private static void OnZoomRangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MapView;
            if (control == null)
            {
                return;
            }

            if (e.Property == MinZoomLevelProperty)
            {
                control.CoerceValue(MaxZoomLevelProperty);
            }
            else if (e.Property == MaxZoomLevelProperty)
            {
                control.CoerceValue(MinZoomLevelProperty);
            }

            control.CoerceValue(ZoomLevelProperty);
            control.RefreshOverlay();
        }

        private int GetEffectiveMinZoom()
        {
            var configuredMinZoom = Math.Max(AbsoluteMinZoom, MinZoomLevel);
            var sourceMinZoom = _tileSource == null ? configuredMinZoom : _tileSource.MinZoomLevel;
            var minZoom = Math.Max(configuredMinZoom, sourceMinZoom);
            return Math.Min(minZoom, GetEffectiveMaxZoom());
        }

        private int GetEffectiveMaxZoom()
        {
            var configuredMaxZoom = Math.Min(AbsoluteMaxZoom, MaxZoomLevel);
            var sourceMaxZoom = _tileSource == null ? configuredMaxZoom : _tileSource.MaxZoomLevel;
            var maxZoom = Math.Min(configuredMaxZoom, sourceMaxZoom);
            return Math.Max(maxZoom, Math.Max(AbsoluteMinZoom, MinZoomLevel));
        }

        private static object CoerceMaxConcurrentTileDownloads(DependencyObject d, object baseValue)
        {
            var value = (int)baseValue;
            return Math.Max(1, Math.Min(16, value));
        }

        private static void OnViewportPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MapView;
            if (control == null)
            {
                return;
            }

            control.RefreshOverlay();
        }

        private static void OnTileTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MapView;
            if (control == null)
            {
                return;
            }

            control.UpdateTileSource();
            lock (control._cacheLock)
            {
                control._tileSourceVersion++;
            }
            control.ClearTileCache();
            control.CoerceValue(ZoomLevelProperty);
            control.InvalidateArrange();
            control.InvalidateVisual();
        }

        private MapTileSource CreateCompatibilityTileSource()
        {
            return new TemplateMapTileSource(TileUrlTemplate, TileSubdomains, UseTmsY, TileStyle);
        }

        private void UpdateTileSource()
        {
            if (_tileSource == null || _usingTemplateCompatibilitySource)
            {
                _tileSource = CreateCompatibilityTileSource();
                _usingTemplateCompatibilitySource = true;
            }

            if (_baseLayer != null)
            {
                _baseLayer.TileSource = _tileSource;
            }
        }

        private static void OnPushpinsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MapView;
            if (control == null)
            {
                return;
            }

            if (e.OldValue is ObservableCollection<Pushpin> oldCollection)
            {
                oldCollection.CollectionChanged -= control.Pushpins_CollectionChanged;
            }

            if (e.NewValue is ObservableCollection<Pushpin> newCollection)
            {
                newCollection.CollectionChanged += control.Pushpins_CollectionChanged;
            }

            control.RefreshOverlay();
        }

        private static void OnPushpinLayersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MapView;
            if (control == null)
            {
                return;
            }

            if (e.OldValue is ObservableCollection<MapPushpinLayer> oldLayers)
            {
                oldLayers.CollectionChanged -= control.PushpinLayers_CollectionChanged;
                control.UnsubscribeLayerCollections(oldLayers);
            }

            if (e.NewValue is ObservableCollection<MapPushpinLayer> newLayers)
            {
                newLayers.CollectionChanged += control.PushpinLayers_CollectionChanged;
                control.SubscribeLayerCollections(newLayers);
            }

            control.RebuildPushpinPresenters();
            control.RefreshOverlay();
        }

        private static void OnPolylinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MapView;
            if (control == null)
            {
                return;
            }

            if (e.OldValue is ObservableCollection<MapPolyline> oldCollection)
            {
                oldCollection.CollectionChanged -= control.Polylines_CollectionChanged;
                control.UnsubscribePolylines(oldCollection);
            }

            if (e.NewValue is ObservableCollection<MapPolyline> newCollection)
            {
                newCollection.CollectionChanged += control.Polylines_CollectionChanged;
                control.SubscribePolylines(newCollection);
            }

            control.RefreshOverlay();
        }

        private static void OnPolygonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MapView;
            if (control == null)
            {
                return;
            }

            if (e.OldValue is ObservableCollection<MapPolygon> oldCollection)
            {
                oldCollection.CollectionChanged -= control.Polygons_CollectionChanged;
                control.UnsubscribePolygons(oldCollection);
            }

            if (e.NewValue is ObservableCollection<MapPolygon> newCollection)
            {
                newCollection.CollectionChanged += control.Polygons_CollectionChanged;
                control.SubscribePolygons(newCollection);
            }

            control.RefreshOverlay();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Pushpins != null)
            {
                Pushpins.CollectionChanged -= Pushpins_CollectionChanged;
                Pushpins.CollectionChanged += Pushpins_CollectionChanged;
            }

            if (PushpinLayers != null)
            {
                PushpinLayers.CollectionChanged -= PushpinLayers_CollectionChanged;
                PushpinLayers.CollectionChanged += PushpinLayers_CollectionChanged;
                SubscribeLayerCollections(PushpinLayers);
            }

            if (Polylines != null)
            {
                Polylines.CollectionChanged -= Polylines_CollectionChanged;
                Polylines.CollectionChanged += Polylines_CollectionChanged;
                SubscribePolylines(Polylines);
            }

            if (Polygons != null)
            {
                Polygons.CollectionChanged -= Polygons_CollectionChanged;
                Polygons.CollectionChanged += Polygons_CollectionChanged;
                SubscribePolygons(Polygons);
            }

            RefreshOverlay();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (Pushpins != null)
            {
                Pushpins.CollectionChanged -= Pushpins_CollectionChanged;
            }

            if (PushpinLayers != null)
            {
                PushpinLayers.CollectionChanged -= PushpinLayers_CollectionChanged;
                UnsubscribeLayerCollections(PushpinLayers);
            }

            if (Polylines != null)
            {
                Polylines.CollectionChanged -= Polylines_CollectionChanged;
                UnsubscribePolylines(Polylines);
            }

            if (Polygons != null)
            {
                Polygons.CollectionChanged -= Polygons_CollectionChanged;
                UnsubscribePolygons(Polygons);
            }

            _renderThrottleTimer.Stop();
            _pushpinActivationTimer.Stop();
            ClearActivatedPushpin();
            _hoverToolTip.IsOpen = false;
            ClearPushpinPresenters();
            _shapeLayer.ClearShapes();
        }

        private void RenderThrottleTimer_Tick(object sender, EventArgs e)
        {
            if (!_renderInvalidated)
            {
                _renderThrottleTimer.Stop();
                return;
            }

            _renderInvalidated = false;
            InvalidateVisual();
        }

        private void PushpinActivationTimer_Tick(object sender, EventArgs e)
        {
            _pushpinActivationTimer.Stop();
            ClearActivatedPushpin();
        }

        private void ClearActivatedPushpin()
        {
            if (_activatedPushpin == null)
            {
                return;
            }

            _activatedPushpin.IsActivated = false;
            _activatedPushpin = null;
            RefreshOverlay();
        }

        private void Pushpins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                for (var i = 0; i < e.OldItems.Count; i++)
                {
                    var pushpin = e.OldItems[i] as Pushpin;
                    if (pushpin == null)
                    {
                        continue;
                    }

                    pushpin.PropertyChanged -= Pushpin_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                for (var i = 0; i < e.NewItems.Count; i++)
                {
                    var pushpin = e.NewItems[i] as Pushpin;
                    if (pushpin == null)
                    {
                        continue;
                    }

                    pushpin.PropertyChanged -= Pushpin_PropertyChanged;
                    pushpin.PropertyChanged += Pushpin_PropertyChanged;
                }
            }

            RebuildPushpinPresenters();
            RefreshOverlay();
        }

        private void InlinePushpins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                for (var i = 0; i < e.OldItems.Count; i++)
                {
                    var pushpin = e.OldItems[i] as Pushpin;
                    if (pushpin == null)
                    {
                        continue;
                    }

                    pushpin.PropertyChanged -= Pushpin_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                for (var i = 0; i < e.NewItems.Count; i++)
                {
                    var pushpin = e.NewItems[i] as Pushpin;
                    if (pushpin == null)
                    {
                        continue;
                    }

                    pushpin.PropertyChanged -= Pushpin_PropertyChanged;
                    pushpin.PropertyChanged += Pushpin_PropertyChanged;
                }
            }

            RebuildPushpinPresenters();
            RefreshOverlay();
        }

        private void PushpinLayers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var oldLayer = oldItem as MapPushpinLayer;
                    if (oldLayer != null)
                    {
                        oldLayer.PropertyChanged -= Layer_PropertyChanged;
                        if (oldLayer.Pushpins != null)
                        {
                            oldLayer.Pushpins.CollectionChanged -= LayerPushpins_CollectionChanged;
                            UnsubscribePushpinItems(oldLayer.Pushpins);
                        }
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    var newLayer = newItem as MapPushpinLayer;
                    if (newLayer != null)
                    {
                        newLayer.PropertyChanged -= Layer_PropertyChanged;
                        newLayer.PropertyChanged += Layer_PropertyChanged;
                        if (newLayer.Pushpins != null)
                        {
                            newLayer.Pushpins.CollectionChanged -= LayerPushpins_CollectionChanged;
                            newLayer.Pushpins.CollectionChanged += LayerPushpins_CollectionChanged;
                            SubscribePushpinItems(newLayer.Pushpins);
                        }
                    }
                }
            }

            RebuildPushpinPresenters();
            RefreshOverlay();
        }

        private void LayerPushpins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var pushpin = oldItem as Pushpin;
                    if (pushpin == null)
                    {
                        continue;
                    }

                    pushpin.PropertyChanged -= Pushpin_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    var pushpin = newItem as Pushpin;
                    if (pushpin == null)
                    {
                        continue;
                    }

                    pushpin.PropertyChanged -= Pushpin_PropertyChanged;
                    pushpin.PropertyChanged += Pushpin_PropertyChanged;
                }
            }

            RebuildPushpinPresenters();
            RefreshOverlay();
        }

        private void Polylines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                for (var i = 0; i < e.OldItems.Count; i++)
                {
                    var line = e.OldItems[i] as MapPolyline;
                    if (line == null)
                    {
                        continue;
                    }

                    line.PropertyChanged -= Polyline_PropertyChanged;
                    if (line.Points != null)
                    {
                        line.Points.CollectionChanged -= PolylinePoints_CollectionChanged;
                        UnsubscribePolylineLocations(line.Points);
                    }
                }
            }

            if (e.NewItems != null)
            {
                for (var i = 0; i < e.NewItems.Count; i++)
                {
                    var line = e.NewItems[i] as MapPolyline;
                    if (line == null)
                    {
                        continue;
                    }

                    line.PropertyChanged -= Polyline_PropertyChanged;
                    line.PropertyChanged += Polyline_PropertyChanged;
                    if (line.Points != null)
                    {
                        line.Points.CollectionChanged -= PolylinePoints_CollectionChanged;
                        line.Points.CollectionChanged += PolylinePoints_CollectionChanged;
                        SubscribePolylineLocations(line.Points);
                    }
                }
            }

            RefreshOverlay();
        }

        private void Polygons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                for (var i = 0; i < e.OldItems.Count; i++)
                {
                    var polygon = e.OldItems[i] as MapPolygon;
                    if (polygon == null)
                    {
                        continue;
                    }

                    polygon.PropertyChanged -= Polygon_PropertyChanged;
                    if (polygon.Points != null)
                    {
                        polygon.Points.CollectionChanged -= PolygonPoints_CollectionChanged;
                        UnsubscribePolygonLocations(polygon.Points);
                    }
                }
            }

            if (e.NewItems != null)
            {
                for (var i = 0; i < e.NewItems.Count; i++)
                {
                    var polygon = e.NewItems[i] as MapPolygon;
                    if (polygon == null)
                    {
                        continue;
                    }

                    polygon.PropertyChanged -= Polygon_PropertyChanged;
                    polygon.PropertyChanged += Polygon_PropertyChanged;
                    if (polygon.Points != null)
                    {
                        polygon.Points.CollectionChanged -= PolygonPoints_CollectionChanged;
                        polygon.Points.CollectionChanged += PolygonPoints_CollectionChanged;
                        SubscribePolygonLocations(polygon.Points);
                    }
                }
            }

            RefreshOverlay();
        }

        private void Polyline_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(MapPolyline.Points), StringComparison.Ordinal))
            {
                if (Polylines != null)
                {
                    UnsubscribePolylines(Polylines);
                    SubscribePolylines(Polylines);
                }
            }

            RefreshOverlay();
        }

        private void Polygon_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(MapPolygon.Points), StringComparison.Ordinal))
            {
                if (Polygons != null)
                {
                    UnsubscribePolygons(Polygons);
                    SubscribePolygons(Polygons);
                }
            }

            RefreshOverlay();
        }

        private void PolylinePoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var location = oldItem as MapLocation;
                    if (location == null)
                    {
                        continue;
                    }

                    location.PropertyChanged -= PolylineLocation_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    var location = newItem as MapLocation;
                    if (location == null)
                    {
                        continue;
                    }

                    location.PropertyChanged -= PolylineLocation_PropertyChanged;
                    location.PropertyChanged += PolylineLocation_PropertyChanged;
                }
            }

            RefreshOverlay();
        }

        private void PolygonPoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var location = oldItem as MapLocation;
                    if (location == null)
                    {
                        continue;
                    }

                    location.PropertyChanged -= PolygonLocation_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    var location = newItem as MapLocation;
                    if (location == null)
                    {
                        continue;
                    }

                    location.PropertyChanged -= PolygonLocation_PropertyChanged;
                    location.PropertyChanged += PolygonLocation_PropertyChanged;
                }
            }

            RefreshOverlay();
        }

        private void PolylineLocation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshOverlay();
        }

        private void PolygonLocation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshOverlay();
        }

        private void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var layer = sender as MapPushpinLayer;
            if (layer == null)
            {
                return;
            }

            if (string.Equals(e.PropertyName, nameof(MapPushpinLayer.Pushpins), StringComparison.Ordinal))
            {
                if (PushpinLayers != null)
                {
                    UnsubscribeLayerCollections(PushpinLayers);
                    SubscribeLayerCollections(PushpinLayers);
                }
            }

            RebuildPushpinPresenters();
            RefreshOverlay();
        }

        private void Pushpin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            RebuildPushpinPresenters();
            RefreshOverlay();
        }

        private void SubscribePushpinItems(IEnumerable<Pushpin> pushpins)
        {
            if (pushpins == null)
            {
                return;
            }

            foreach (var pushpin in pushpins)
            {
                if (pushpin == null)
                {
                    continue;
                }

                pushpin.PropertyChanged -= Pushpin_PropertyChanged;
                pushpin.PropertyChanged += Pushpin_PropertyChanged;
            }
        }

        private void UnsubscribePushpinItems(IEnumerable<Pushpin> pushpins)
        {
            if (pushpins == null)
            {
                return;
            }

            foreach (var pushpin in pushpins)
            {
                if (pushpin == null)
                {
                    continue;
                }

                pushpin.PropertyChanged -= Pushpin_PropertyChanged;
            }
        }

        private void SubscribeLayerCollections(IEnumerable<MapPushpinLayer> layers)
        {
            if (layers == null)
            {
                return;
            }

            foreach (var layer in layers)
            {
                if (layer == null)
                {
                    continue;
                }

                layer.PropertyChanged -= Layer_PropertyChanged;
                layer.PropertyChanged += Layer_PropertyChanged;

                if (layer.Pushpins == null)
                {
                    continue;
                }

                layer.Pushpins.CollectionChanged -= LayerPushpins_CollectionChanged;
                layer.Pushpins.CollectionChanged += LayerPushpins_CollectionChanged;
                SubscribePushpinItems(layer.Pushpins);
            }
        }

        private void UnsubscribeLayerCollections(IEnumerable<MapPushpinLayer> layers)
        {
            if (layers == null)
            {
                return;
            }

            foreach (var layer in layers)
            {
                if (layer == null)
                {
                    continue;
                }

                layer.PropertyChanged -= Layer_PropertyChanged;

                if (layer.Pushpins == null)
                {
                    continue;
                }

                layer.Pushpins.CollectionChanged -= LayerPushpins_CollectionChanged;
                UnsubscribePushpinItems(layer.Pushpins);
            }
        }

        private void SubscribePolylines(IEnumerable<MapPolyline> polylines)
        {
            if (polylines == null)
            {
                return;
            }

            foreach (var polyline in polylines)
            {
                if (polyline == null)
                {
                    continue;
                }

                polyline.PropertyChanged -= Polyline_PropertyChanged;
                polyline.PropertyChanged += Polyline_PropertyChanged;
                if (polyline.Points == null)
                {
                    continue;
                }

                polyline.Points.CollectionChanged -= PolylinePoints_CollectionChanged;
                polyline.Points.CollectionChanged += PolylinePoints_CollectionChanged;
                SubscribePolylineLocations(polyline.Points);
            }
        }

        private void UnsubscribePolylines(IEnumerable<MapPolyline> polylines)
        {
            if (polylines == null)
            {
                return;
            }

            foreach (var polyline in polylines)
            {
                if (polyline == null)
                {
                    continue;
                }

                polyline.PropertyChanged -= Polyline_PropertyChanged;
                if (polyline.Points == null)
                {
                    continue;
                }

                polyline.Points.CollectionChanged -= PolylinePoints_CollectionChanged;
                UnsubscribePolylineLocations(polyline.Points);
            }
        }

        private void SubscribePolygons(IEnumerable<MapPolygon> polygons)
        {
            if (polygons == null)
            {
                return;
            }

            foreach (var polygon in polygons)
            {
                if (polygon == null)
                {
                    continue;
                }

                polygon.PropertyChanged -= Polygon_PropertyChanged;
                polygon.PropertyChanged += Polygon_PropertyChanged;
                if (polygon.Points == null)
                {
                    continue;
                }

                polygon.Points.CollectionChanged -= PolygonPoints_CollectionChanged;
                polygon.Points.CollectionChanged += PolygonPoints_CollectionChanged;
                SubscribePolygonLocations(polygon.Points);
            }
        }

        private void UnsubscribePolygons(IEnumerable<MapPolygon> polygons)
        {
            if (polygons == null)
            {
                return;
            }

            foreach (var polygon in polygons)
            {
                if (polygon == null)
                {
                    continue;
                }

                polygon.PropertyChanged -= Polygon_PropertyChanged;
                if (polygon.Points == null)
                {
                    continue;
                }

                polygon.Points.CollectionChanged -= PolygonPoints_CollectionChanged;
                UnsubscribePolygonLocations(polygon.Points);
            }
        }

        private void SubscribePolylineLocations(IEnumerable<MapLocation> points)
        {
            if (points == null)
            {
                return;
            }

            foreach (var point in points)
            {
                if (point == null)
                {
                    continue;
                }

                point.PropertyChanged -= PolylineLocation_PropertyChanged;
                point.PropertyChanged += PolylineLocation_PropertyChanged;
            }
        }

        private void UnsubscribePolylineLocations(IEnumerable<MapLocation> points)
        {
            if (points == null)
            {
                return;
            }

            foreach (var point in points)
            {
                if (point == null)
                {
                    continue;
                }

                point.PropertyChanged -= PolylineLocation_PropertyChanged;
            }
        }

        private void SubscribePolygonLocations(IEnumerable<MapLocation> points)
        {
            if (points == null)
            {
                return;
            }

            foreach (var point in points)
            {
                if (point == null)
                {
                    continue;
                }

                point.PropertyChanged -= PolygonLocation_PropertyChanged;
                point.PropertyChanged += PolygonLocation_PropertyChanged;
            }
        }

        private void UnsubscribePolygonLocations(IEnumerable<MapLocation> points)
        {
            if (points == null)
            {
                return;
            }

            foreach (var point in points)
            {
                if (point == null)
                {
                    continue;
                }

                point.PropertyChanged -= PolygonLocation_PropertyChanged;
            }
        }

        private List<MapPolyline> GetActivePolylinesSnapshot()
        {
            var result = new List<MapPolyline>();
            if (Polylines == null)
            {
                return result;
            }

            for (var i = 0; i < Polylines.Count; i++)
            {
                var polyline = Polylines[i];
                if (polyline == null || !polyline.IsVisible || polyline.Points == null || polyline.Points.Count < 2)
                {
                    continue;
                }

                result.Add(polyline);
            }

            return result;
        }

        private List<MapPolygon> GetActivePolygonsSnapshot()
        {
            var result = new List<MapPolygon>();
            if (Polygons == null)
            {
                return result;
            }

            for (var i = 0; i < Polygons.Count; i++)
            {
                var polygon = Polygons[i];
                if (polygon == null || !polygon.IsVisible || polygon.Points == null || polygon.Points.Count < 3)
                {
                    continue;
                }

                result.Add(polygon);
            }

            return result;
        }

        private void UpdateMapShapesLayer(int zoom, Point viewTopLeft, Size viewportSize)
        {
            if (_shapeLayer == null)
            {
                return;
            }

            var polylines = GetActivePolylinesSnapshot();
            var polygons = GetActivePolygonsSnapshot();
            if (polylines.Count == 0 && polygons.Count == 0)
            {
                _shapeLayer.ClearShapes();
                return;
            }

            var polylineLayouts = new List<MapPolylineLayoutInfo>();
            for (var i = 0; i < polylines.Count; i++)
            {
                var polyline = polylines[i];
                var layout = new MapPolylineLayoutInfo(polyline);
                for (var j = 0; j < polyline.Points.Count; j++)
                {
                    var location = polyline.Points[j];
                    if (location == null)
                    {
                        continue;
                    }

                    var pixel = ToPixel(location.Latitude, location.Longitude, zoom);
                    layout.Points.Add(new Point(pixel.X - viewTopLeft.X, pixel.Y - viewTopLeft.Y));
                }

                if (layout.Points.Count >= 2)
                {
                    polylineLayouts.Add(layout);
                }
            }

            var polygonLayouts = new List<MapPolygonLayoutInfo>();
            for (var i = 0; i < polygons.Count; i++)
            {
                var polygon = polygons[i];
                var layout = new MapPolygonLayoutInfo(polygon);
                for (var j = 0; j < polygon.Points.Count; j++)
                {
                    var location = polygon.Points[j];
                    if (location == null)
                    {
                        continue;
                    }

                    var pixel = ToPixel(location.Latitude, location.Longitude, zoom);
                    layout.Points.Add(new Point(pixel.X - viewTopLeft.X, pixel.Y - viewTopLeft.Y));
                }

                if (layout.Points.Count >= 3)
                {
                    polygonLayouts.Add(layout);
                }
            }

            if (polylineLayouts.Count == 0 && polygonLayouts.Count == 0)
            {
                _shapeLayer.ClearShapes();
                return;
            }

            _shapeLayer.UpdateShapes(polylineLayouts, polygonLayouts);
        }

        private List<Pushpin> GetActivePushpinsSnapshot()
        {
            return GetActivePushpinEntriesSnapshot().Select(x => x.Pushpin).ToList();
        }

        private List<Pushpin> GetClusterablePushpinsSnapshot()
        {
            if (!EnableClustering)
            {
                return new List<Pushpin>();
            }

            return GetActivePushpinEntriesSnapshot()
                .Where(x => x.ClusterEnabled)
                .Select(x => x.Pushpin)
                .ToList();
        }

        private List<ActivePushpinEntry> GetActivePushpinEntriesSnapshot()
        {
            var entries = new List<ActivePushpinEntry>();

            if (Pushpins != null)
            {
                for (var i = 0; i < Pushpins.Count; i++)
                {
                    var pushpin = Pushpins[i];
                    if (pushpin != null)
                    {
                        entries.Add(new ActivePushpinEntry(pushpin, true));
                    }
                }
            }

            if (InlinePushpins != null)
            {
                for (var i = 0; i < InlinePushpins.Count; i++)
                {
                    var pushpin = InlinePushpins[i];
                    if (pushpin != null)
                    {
                        entries.Add(new ActivePushpinEntry(pushpin, true));
                    }
                }
            }

            if (PushpinLayers != null)
            {
                for (var i = 0; i < PushpinLayers.Count; i++)
                {
                    var layer = PushpinLayers[i];
                    if (layer == null || !layer.IsVisible || layer.Pushpins == null)
                    {
                        continue;
                    }

                    for (var j = 0; j < layer.Pushpins.Count; j++)
                    {
                        var pushpin = layer.Pushpins[j];
                        if (pushpin != null)
                        {
                            entries.Add(new ActivePushpinEntry(pushpin, layer.EnableClustering));
                        }
                    }
                }
            }

            return entries;
        }

        private void UpdateHoveredPushpin(Point mousePosition)
        {
            var isClusterHit = IsClusterHit(mousePosition);
            var pushpin = GetPushpinAt(mousePosition);
            if (isClusterHit)
            {
                pushpin = null;
            }

            if (ReferenceEquals(_hoveredPushpin, pushpin))
            {
                if (isClusterHit)
                {
                    _hoverToolTip.IsOpen = false;
                    ToolTip = null;
                }
                return;
            }

            _hoveredPushpin = pushpin;
            Cursor = pushpin == null ? null : Cursors.Hand;

            var title = pushpin == null ? null : pushpin.Title;
            if (string.IsNullOrWhiteSpace(title))
            {
                _hoverToolTip.IsOpen = false;
                ToolTip = null;
                return;
            }

            ApplyHoverToolTipStyle();

            _hoverToolTip.Content = title;
            if (_hoverToolTip.IsOpen)
            {
                _hoverToolTip.IsOpen = false;
            }

            _hoverToolTip.IsOpen = true;
            ToolTip = title;
        }

        private void ApplyHoverToolTipStyle()
        {
            var style = TryFindResource(typeof(ToolTip)) as Style;
            if (style != null && !ReferenceEquals(_hoverToolTip.Style, style))
            {
                _hoverToolTip.Style = style;
            }
        }

        private bool IsClusterHit(Point mousePosition)
        {
            if (!EnableClustering || double.IsNaN(mousePosition.X) || double.IsNaN(mousePosition.Y) || ActualWidth <= 0 || ActualHeight <= 0)
            {
                return false;
            }

            var activePushpins = GetClusterablePushpinsSnapshot();
            if (activePushpins.Count == 0)
            {
                return false;
            }

            var zoom = (int)CoerceZoom(this, ZoomLevel);
            var centerPixel = ToPixel(CenterLatitude, CenterLongitude, zoom);
            var viewTopLeft = new Point(centerPixel.X - ActualWidth / 2.0, centerPixel.Y - ActualHeight / 2.0);
            var clusterRadius = Math.Max(8, ClusterRadius);
            var minimumCount = Math.Max(2, ClusterMinimumCount);
            var candidates = CollectVisiblePushpins(activePushpins, zoom, viewTopLeft, new Size(ActualWidth, ActualHeight), clusterRadius);
            var clusters = BuildClusters(candidates, clusterRadius);

            for (var i = 0; i < clusters.Count; i++)
            {
                var cluster = clusters[i];
                if (cluster.Count < minimumCount)
                {
                    continue;
                }

                var radius = Math.Max(PushpinSize, Math.Min(clusterRadius, PushpinSize * 0.9 + (cluster.Count * 0.45)));
                var dx = mousePosition.X - cluster.X;
                var dy = mousePosition.Y - cluster.Y;
                if ((dx * dx) + (dy * dy) <= radius * radius)
                {
                    return true;
                }
            }

            return false;
        }

        private Pushpin GetPushpinAt(Point mousePosition)
        {
            if (double.IsNaN(mousePosition.X) || double.IsNaN(mousePosition.Y) || ActualWidth <= 0 || ActualHeight <= 0)
            {
                return null;
            }

            var activePushpins = GetActivePushpinsSnapshot();
            if (activePushpins.Count == 0)
            {
                return null;
            }

            var zoom = (int)CoerceZoom(this, ZoomLevel);
            var centerPixel = ToPixel(CenterLatitude, CenterLongitude, zoom);
            var viewTopLeft = new Point(centerPixel.X - ActualWidth / 2.0, centerPixel.Y - ActualHeight / 2.0);
            var pinSize = Math.Max(1d, PushpinSize);
            var hitRadius = Math.Max(8d, pinSize * 0.75);
            var hitRadiusSquared = hitRadius * hitRadius;
            var usesDefaultPinVisual = PushpinTemplate == null && !(PushpinRenderMode == PushpinVisualMode.Icon && PushpinImageSource != null);
            var defaultPinWidth = Math.Max(16.0, pinSize * 2.5);
            var defaultPinHeight = Math.Max(24.0, pinSize * 3.4);

            Pushpin nearest = null;
            var nearestDistance = double.MaxValue;
            for (var i = 0; i < activePushpins.Count; i++)
            {
                var pushpin = activePushpins[i];

                var pixel = ToPixel(pushpin.Latitude, pushpin.Longitude, zoom);
                var x = pixel.X - viewTopLeft.X;
                var y = pixel.Y - viewTopLeft.Y;

                if (usesDefaultPinVisual)
                {
                    // Default pin is anchored at bottom-center, so hit-test the full marker body.
                    var hitRect = new Rect(
                        x - (defaultPinWidth / 2.0) - 2.0,
                        y - defaultPinHeight - 2.0,
                        defaultPinWidth + 4.0,
                        defaultPinHeight + 4.0);
                    if (!hitRect.Contains(mousePosition))
                    {
                        continue;
                    }

                    var centerX = x;
                    var centerY = y - defaultPinHeight * 0.5;
                    var rectDx = mousePosition.X - centerX;
                    var rectDy = mousePosition.Y - centerY;
                    var rectDistance = rectDx * rectDx + rectDy * rectDy;
                    if (rectDistance >= nearestDistance)
                    {
                        continue;
                    }

                    nearest = pushpin;
                    nearestDistance = rectDistance;
                    continue;
                }

                var dx = mousePosition.X - x;
                var dy = mousePosition.Y - y;
                var distance = dx * dx + dy * dy;
                if (distance > hitRadiusSquared || distance >= nearestDistance)
                {
                    continue;
                }

                nearest = pushpin;
                nearestDistance = distance;
            }

            return nearest;
        }

        private void QueueViewportTiles(int zoom, Point viewTopLeft, Size viewportSize, MapTileSource source, string cacheKeyPrefix)
        {
            if (viewportSize.Width <= 0 || viewportSize.Height <= 0 || source == null)
            {
                return;
            }

            var firstTileX = (int)Math.Floor(viewTopLeft.X / (double)TileSize) - 1;
            var firstTileY = (int)Math.Floor(viewTopLeft.Y / (double)TileSize) - 1;
            var tileCountX = (int)Math.Ceiling((viewportSize.Width + TileSize * 2) / (double)TileSize) + 2;
            var tileCountY = (int)Math.Ceiling((viewportSize.Height + TileSize * 2) / (double)TileSize) + 2;
            var maxTile = (1 << zoom) - 1;
            var viewportCenter = new Point(viewTopLeft.X + viewportSize.Width / 2.0, viewTopLeft.Y + viewportSize.Height / 2.0);
            var tilesToQueue = new List<Tuple<int, int, string>>();

            for (var x = 0; x < tileCountX; x++)
            {
                for (var y = 0; y < tileCountY; y++)
                {
                    var tileX = firstTileX + x;
                    var tileY = firstTileY + y;
                    if (tileY < 0 || tileY > maxTile)
                    {
                        continue;
                    }

                    var wrappedTileX = WrapTileX(tileX, zoom);
                    var cacheKey = cacheKeyPrefix + zoom.ToString(CultureInfo.InvariantCulture) + ":" + wrappedTileX + ":" + tileY;

                    lock (_cacheLock)
                    {
                        if (_tileCache.ContainsKey(cacheKey) || _loadingTiles.Contains(cacheKey) || _queuedTiles.Contains(cacheKey))
                        {
                            continue;
                        }
                    }

                    var tileCenterX = tileX * TileSize + TileSize / 2.0;
                    var tileCenterY = tileY * TileSize + TileSize / 2.0;
                    var distance = (tileCenterX - viewportCenter.X) * (tileCenterX - viewportCenter.X) +
                                   (tileCenterY - viewportCenter.Y) * (tileCenterY - viewportCenter.Y);
                    tilesToQueue.Add(Tuple.Create((int)Math.Round(distance), wrappedTileX, cacheKey));
                }
            }

            foreach (var item in tilesToQueue.OrderBy(x => x.Item1).ThenBy(x => x.Item2))
            {
                var tileX = item.Item2;
                var tileY = ExtractTileYFromCacheKey(item.Item3, zoom);
                if (tileY >= 0)
                {
                    QueueTileLoad(zoom, tileX, tileY, item.Item3, source);
                }
            }
        }

        private static int ExtractTileYFromCacheKey(string cacheKey, int zoom)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return -1;
            }

            var parts = cacheKey.Split(':');
            if (parts.Length < 3)
            {
                return -1;
            }

            if (!int.TryParse(parts[parts.Length - 3], out var keyZoom) || keyZoom != zoom)
            {
                return -1;
            }

            if (!int.TryParse(parts[parts.Length - 1], out var tileY))
            {
                return -1;
            }

            return tileY;
        }

        private void UpdateMapItemsLayer(Size finalSize)
        {
            var activeEntries = GetActivePushpinEntriesSnapshot();
            if (activeEntries.Count == 0 || finalSize.Width <= 0 || finalSize.Height <= 0)
            {
                ClearPushpinPresenters();
                return;
            }

            var clusterEnabledPushpins = EnableClustering
                ? activeEntries.Where(x => x.ClusterEnabled).Select(x => x.Pushpin).ToList()
                : new List<Pushpin>();
            var forceSinglePushpins = EnableClustering
                ? activeEntries.Where(x => !x.ClusterEnabled).Select(x => x.Pushpin).ToList()
                : activeEntries.Select(x => x.Pushpin).ToList();

            var usesTemplateGlobally = PushpinTemplate != null;
            var templateClusterPushpins = usesTemplateGlobally
                ? clusterEnabledPushpins
                : clusterEnabledPushpins.Where(x => x != null && x.Template != null).ToList();
            var templateSinglePushpins = usesTemplateGlobally
                ? forceSinglePushpins
                : forceSinglePushpins.Where(x => x != null && x.Template != null).ToList();
            var defaultClusterPushpins = usesTemplateGlobally
                ? new List<Pushpin>()
                : clusterEnabledPushpins.Where(x => x != null && x.Template == null).ToList();
            var defaultSinglePushpins = usesTemplateGlobally
                ? new List<Pushpin>()
                : forceSinglePushpins.Where(x => x != null && x.Template == null).ToList();

            var zoom = (int)CoerceZoom(this, ZoomLevel);
            var centerPixel = ToPixel(CenterLatitude, CenterLongitude, zoom);
            var viewTopLeft = new Point(centerPixel.X - finalSize.Width / 2.0, centerPixel.Y - finalSize.Height / 2.0);
            var pinSize = Math.Max(1, PushpinSize);
            var templatedLayouts = new List<MapItemLayoutInfo>();
            var defaultLayouts = new List<MapItemLayoutInfo>();

            if (templateClusterPushpins.Count > 0)
            {
                var clusterRadius = Math.Max(8, ClusterRadius);
                var minimumCount = Math.Max(2, ClusterMinimumCount);
                var candidates = CollectVisiblePushpins(templateClusterPushpins, zoom, viewTopLeft, finalSize, clusterRadius);
                var clusters = BuildClusters(candidates, clusterRadius);
                for (var i = 0; i < clusters.Count; i++)
                {
                    var cluster = clusters[i];
                    if (cluster.Count >= minimumCount)
                    {
                        if (ClusterTemplate != null)
                        {
                            templatedLayouts.Add(new MapItemLayoutInfo(cluster, cluster.X, cluster.Y));
                        }
                        else
                        {
                            defaultLayouts.Add(new MapItemLayoutInfo(cluster, cluster.X, cluster.Y));
                        }
                    }
                    else
                    {
                        templatedLayouts.Add(new MapItemLayoutInfo(cluster.Representative, cluster.X, cluster.Y));
                    }
                }
            }

            if (templateSinglePushpins.Count > 0)
            {
                templatedLayouts.AddRange(CollectVisibleTemplateItems(templateSinglePushpins, zoom, viewTopLeft, finalSize, pinSize));
            }

            if (defaultClusterPushpins.Count > 0)
            {
                var clusterRadius = Math.Max(8, ClusterRadius);
                var minimumCount = Math.Max(2, ClusterMinimumCount);
                var candidates = CollectVisiblePushpins(defaultClusterPushpins, zoom, viewTopLeft, finalSize, clusterRadius);
                var clusters = BuildClusters(candidates, clusterRadius);
                for (var i = 0; i < clusters.Count; i++)
                {
                    var cluster = clusters[i];
                    if (cluster.Count >= minimumCount)
                    {
                        if (ClusterTemplate != null)
                        {
                            templatedLayouts.Add(new MapItemLayoutInfo(cluster, cluster.X, cluster.Y));
                        }
                        else
                        {
                            defaultLayouts.Add(new MapItemLayoutInfo(cluster, cluster.X, cluster.Y));
                        }
                    }
                    else
                    {
                        defaultLayouts.Add(new MapItemLayoutInfo(cluster.Representative, cluster.X, cluster.Y));
                    }
                }
            }

            if (defaultSinglePushpins.Count > 0)
            {
                defaultLayouts.AddRange(CollectVisibleDefaultItems(defaultSinglePushpins, zoom, viewTopLeft, finalSize, pinSize));
            }

            if (templatedLayouts.Count > 0)
            {
                _templatedItemsLayer.UpdateItems(templatedLayouts, PushpinTemplate, ClusterTemplate, pinSize);
            }
            else
            {
                _templatedItemsLayer.ClearItems();
            }

            if (defaultLayouts.Count == 0)
            {
                _itemsLayer.ClearItems();
                return;
            }

            _itemsLayer.UpdateDefaultItems(
                defaultLayouts,
                PushpinBrush,
                PushpinStroke ?? Brushes.White,
                pinSize,
                PushpinRenderMode,
                PushpinImageSource,
                ClusterBrush ?? PushpinBrush,
                ClusterTextBrush ?? (PushpinStroke ?? Brushes.White),
                ClusterRadius,
                ClusterMinimumCount);
        }

        private List<MapItemLayoutInfo> CollectVisibleTemplateItems(IList<Pushpin> sourcePushpins, int zoom, Point viewTopLeft, Size finalSize, double margin)
        {
            var visiblePushpins = new List<MapItemLayoutInfo>();
            foreach (var pushpin in sourcePushpins)
            {
                var pixel = ToPixel(pushpin.Latitude, pushpin.Longitude, zoom);
                var x = pixel.X - viewTopLeft.X;
                var y = pixel.Y - viewTopLeft.Y;
                if (x < -margin || y < -margin || x > finalSize.Width + margin || y > finalSize.Height + margin)
                {
                    continue;
                }

                visiblePushpins.Add(new MapItemLayoutInfo(pushpin, x, y));
            }

            return visiblePushpins;
        }

        private List<MapItemLayoutInfo> CollectVisibleDefaultItems(IList<Pushpin> sourcePushpins, int zoom, Point viewTopLeft, Size finalSize, double margin)
        {
            var visiblePushpins = new List<MapItemLayoutInfo>();
            foreach (var pushpin in sourcePushpins)
            {
                var pixel = ToPixel(pushpin.Latitude, pushpin.Longitude, zoom);
                var x = pixel.X - viewTopLeft.X;
                var y = pixel.Y - viewTopLeft.Y;
                if (x < -margin || y < -margin || x > finalSize.Width + margin || y > finalSize.Height + margin)
                {
                    continue;
                }

                visiblePushpins.Add(new MapItemLayoutInfo(pushpin, x, y));
            }

            return visiblePushpins;
        }

        private List<PushpinLayout> CollectVisiblePushpins(IList<Pushpin> sourcePushpins, int zoom, Point viewTopLeft, Size finalSize, double margin)
        {
            var visiblePushpins = new List<PushpinLayout>();
            foreach (var pushpin in sourcePushpins)
            {
                var pixel = ToPixel(pushpin.Latitude, pushpin.Longitude, zoom);
                var x = pixel.X - viewTopLeft.X;
                var y = pixel.Y - viewTopLeft.Y;
                if (x < -margin || y < -margin || x > finalSize.Width + margin || y > finalSize.Height + margin)
                {
                    continue;
                }

                visiblePushpins.Add(new PushpinLayout(pushpin, x, y));
            }

            return visiblePushpins;
        }

        private static List<PushpinCluster> BuildClusters(List<PushpinLayout> points, double radius)
        {
            var clusters = new List<PushpinCluster>();
            if (points == null || points.Count == 0)
            {
                return clusters;
            }

            var count = points.Count;
            var parents = new int[count];
            var ranks = new int[count];
            for (var i = 0; i < count; i++)
            {
                parents[i] = i;
                ranks[i] = 0;
            }

            var radiusSquared = radius * radius;
            for (var i = 0; i < count; i++)
            {
                for (var j = i + 1; j < count; j++)
                {
                    var dx = points[i].X - points[j].X;
                    var dy = points[i].Y - points[j].Y;
                    if (dx * dx + dy * dy <= radiusSquared)
                    {
                        Union(parents, ranks, i, j);
                    }
                }
            }

            var rootOrder = new List<int>();
            var aggregateMap = new Dictionary<int, ClusterAggregate>();
            for (var i = 0; i < count; i++)
            {
                var root = FindRoot(parents, i);
                ClusterAggregate aggregate;
                if (!aggregateMap.TryGetValue(root, out aggregate))
                {
                    aggregate = new ClusterAggregate(points[i].Pushpin);
                    aggregateMap.Add(root, aggregate);
                    rootOrder.Add(root);
                }

                aggregate.SumX += points[i].X;
                aggregate.SumY += points[i].Y;
                aggregate.Count++;
                aggregate.Items.Add(points[i].Pushpin);
            }

            for (var i = 0; i < rootOrder.Count; i++)
            {
                var aggregate = aggregateMap[rootOrder[i]];
                var cluster = new PushpinCluster(
                    aggregate.SumX / aggregate.Count,
                    aggregate.SumY / aggregate.Count,
                    aggregate.Representative)
                {
                    Count = aggregate.Count
                };

                cluster.Items.Clear();
                foreach (var item in aggregate.Items)
                {
                    cluster.Items.Add(item);
                }

                clusters.Add(cluster);
            }

            return clusters;
        }

        private static int FindRoot(int[] parents, int node)
        {
            while (parents[node] != node)
            {
                parents[node] = parents[parents[node]];
                node = parents[node];
            }

            return node;
        }

        private static void Union(int[] parents, int[] ranks, int a, int b)
        {
            var rootA = FindRoot(parents, a);
            var rootB = FindRoot(parents, b);
            if (rootA == rootB)
            {
                return;
            }

            if (ranks[rootA] < ranks[rootB])
            {
                parents[rootA] = rootB;
                return;
            }

            if (ranks[rootA] > ranks[rootB])
            {
                parents[rootB] = rootA;
                return;
            }

            parents[rootB] = rootA;
            ranks[rootA]++;
        }

        private sealed class ActivePushpinEntry
        {
            public ActivePushpinEntry(Pushpin pushpin, bool clusterEnabled)
            {
                Pushpin = pushpin;
                ClusterEnabled = clusterEnabled;
            }

            public Pushpin Pushpin { get; private set; }

            public bool ClusterEnabled { get; private set; }
        }

        private void RebuildPushpinPresenters()
        {
            if (PushpinTemplate == null && (InlinePushpins == null || InlinePushpins.All(x => x == null || x.Template == null)))
            {
                if (_templatedItemsLayer != null)
                {
                    _templatedItemsLayer.ClearItems();
                }
            }
        }

        private void ClearPushpinPresenters()
        {
            if (_itemsLayer == null || _templatedItemsLayer == null)
            {
                return;
            }

            _itemsLayer.ClearItems();
            _templatedItemsLayer.ClearItems();
        }

        private static int WrapTileX(int tileX, int zoom)
        {
            var maxTile = 1 << zoom;
            var wrapped = tileX % maxTile;
            return wrapped < 0 ? wrapped + maxTile : wrapped;
        }

        private void QueueTileLoad(int zoom, int tileX, int tileY, string cacheKey, MapTileSource source)
        {
            TileConfigSnapshot snapshot;

            lock (_cacheLock)
            {
                snapshot = new TileConfigSnapshot(
                    source ?? _tileSource ?? new TemplateMapTileSource(TileUrlTemplate, TileSubdomains, UseTmsY, TileStyle),
                    MaxConcurrentTileDownloads,
                    _tileSourceVersion);

                DateTime lastFailedAt;
                if (_failedTiles.TryGetValue(cacheKey, out lastFailedAt) && DateTime.UtcNow - lastFailedAt < FailedRetryDelay)
                {
                    return;
                }

                if (_failedTileAttempts.TryGetValue(cacheKey, out var attempts) && attempts >= 3)
                {
                    return;
                }

                if (_loadingTiles.Contains(cacheKey) || _tileCache.ContainsKey(cacheKey) || _queuedTiles.Contains(cacheKey))
                {
                    return;
                }

                _loadingTiles.Add(cacheKey);
                _queuedTiles.Add(cacheKey);
                _pendingTiles.Enqueue(new TileRequest(zoom, tileX, tileY, cacheKey, snapshot));
            }

            TryStartTileDownloads();
        }

        private void TryStartTileDownloads()
        {
            while (true)
            {
                TileRequest request;
                lock (_cacheLock)
                {
                    int maxConcurrent = _pendingTiles.Count > 0
                        ? _pendingTiles.Peek().Config.MaxConcurrentTileDownloads
                        : MaxConcurrentTileDownloads;

                    if (_activeTileDownloads >= maxConcurrent || _pendingTiles.Count == 0)
                    {
                        return;
                    }

                    request = _pendingTiles.Dequeue();
                    _activeTileDownloads++;
                }

                DownloadTileImageAsync(request, (image, errorMessage) =>
                {
                    var shouldRender = false;
                    lock (_cacheLock)
                    {
                        var isStaleRequest = request.Config.TileSourceVersion != _tileSourceVersion;
                        if (!isStaleRequest)
                        {
                            if (image != null)
                            {
                                _tileCache[request.CacheKey] = image;
                                _failedTiles.Remove(request.CacheKey);
                                _failedTileAttempts.Remove(request.CacheKey);
                                shouldRender = true;
                            }
                            else
                            {
                                var attempts = _failedTileAttempts.TryGetValue(request.CacheKey, out var currentAttempts)
                                    ? currentAttempts + 1
                                    : 1;
                                _failedTileAttempts[request.CacheKey] = attempts;
                                _failedTiles[request.CacheKey] = DateTime.UtcNow;
                                if (!string.IsNullOrWhiteSpace(errorMessage))
                                {
                                    _lastTileError = errorMessage;
                                }
                            }
                        }

                        _queuedTiles.Remove(request.CacheKey);
                        _loadingTiles.Remove(request.CacheKey);
                        _activeTileDownloads--;
                    }

                    if (shouldRender)
                    {
                        RequestRender();
                    }

                    Dispatcher.BeginInvoke((Action)(TryStartTileDownloads));
                });
            }
        }

        private void RequestRender()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                _renderInvalidated = true;
                InvalidateMeasure();
                InvalidateArrange();
                if (!_renderThrottleTimer.IsEnabled)
                {
                    _renderThrottleTimer.Start();
                }
            }));
        }

        private void DrawDiagnostics(DrawingContext drawingContext)
        {
            if (!ShowDiagnostics)
            {
                return;
            }

            string errorText;
            int cacheCount;
            int loadingCount;
            int pendingCount;
            int activeCount;
            int maxConcurrent;
            lock (_cacheLock)
            {
                cacheCount = _tileCache.Count;
                loadingCount = _loadingTiles.Count;
                pendingCount = _pendingTiles.Count;
                activeCount = _activeTileDownloads;
                maxConcurrent = MaxConcurrentTileDownloads;
                errorText = _lastTileError;
            }

            if (cacheCount > 0 && string.IsNullOrWhiteSpace(errorText))
            {
                return;
            }

            var status = string.Format(
                CultureInfo.InvariantCulture,
                "Cache={0} Loading={1} Pending={2} Active={3}/{4}",
                cacheCount,
                loadingCount,
                pendingCount,
                activeCount,
                maxConcurrent);
            var display = string.IsNullOrWhiteSpace(errorText) ? status : status + "  Error=" + errorText;
            var formatted = CreateFormattedText(
                display,
                new Typeface("Segoe UI"),
                12,
                Brushes.Firebrick);

            var rect = new Rect(8, 8, Math.Max(300, formatted.Width + 20), formatted.Height + 12);
            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(220, 255, 248, 248)), new Pen(Brushes.IndianRed, 1), rect);
            drawingContext.DrawText(formatted, new Point(16, 14));
        }

        private void DrawScaleBar(DrawingContext drawingContext)
        {
            if (!ShowScaleBar || ActualWidth <= 0 || ActualHeight <= 0)
            {
                return;
            }

            var metersPerPixel = GetMetersPerPixel(CenterLatitude, ZoomLevel);
            if (metersPerPixel <= 0)
            {
                return;
            }

            var availableWidth = Math.Min(Math.Max(40d, ScaleBarMaxWidth), Math.Max(40d, ActualWidth - 28d));
            if (availableWidth <= 0)
            {
                return;
            }

            var maxDistanceMeters = metersPerPixel * availableWidth;
            if (maxDistanceMeters <= 0)
            {
                return;
            }

            var unit = ResolveScaleUnit();
            double targetDistanceMeters;
            string label;
            if (unit == MapDistanceUnit.MilesFeet)
            {
                var maxFeet = maxDistanceMeters * FeetPerMeter;
                if (maxFeet >= FeetPerMile)
                {
                    var miles = LargestNiceNumber(maxFeet / FeetPerMile);
                    targetDistanceMeters = (miles * FeetPerMile) / FeetPerMeter;
                    label = FormatDistanceLabel(miles, "mi");
                }
                else
                {
                    var feet = LargestNiceNumber(maxFeet);
                    targetDistanceMeters = feet / FeetPerMeter;
                    label = FormatDistanceLabel(feet, "ft");
                }
            }
            else
            {
                if (maxDistanceMeters >= 1000)
                {
                    var kilometers = LargestNiceNumber(maxDistanceMeters / 1000d);
                    targetDistanceMeters = kilometers * 1000d;
                    label = FormatDistanceLabel(kilometers, "km");
                }
                else
                {
                    var meters = LargestNiceNumber(maxDistanceMeters);
                    targetDistanceMeters = meters;
                    label = FormatDistanceLabel(meters, "m");
                }
            }

            if (targetDistanceMeters <= 0)
            {
                return;
            }

            var barWidth = targetDistanceMeters / metersPerPixel;
            if (barWidth <= 0 || double.IsNaN(barWidth) || double.IsInfinity(barWidth))
            {
                return;
            }

            var textBrush = ScaleBarTextBrush ?? new SolidColorBrush(Color.FromRgb(0x74, 0x74, 0x74));
            var text = CreateFormattedText(label, new Typeface("Segoe UI"), 12, textBrush);

            var margin = ScaleBarMargin;
            var lineWidth = Math.Max(24d, barWidth);
            var panelWidth = Math.Max(lineWidth, text.Width);
            var left = ScaleBarPosition == MapScaleBarPosition.BottomRight
                ? Math.Max(margin.Left, ActualWidth - panelWidth - margin.Right)
                : margin.Left;
            var panelBottom = ActualHeight - margin.Bottom;

            var lineX = left + (panelWidth - lineWidth) / 2d;
            var lineY = panelBottom;
            var tickHeight = 4d;
            var textTop = lineY - tickHeight - 2d - text.Height;

            var lineBrush = ScaleBarStroke ?? new SolidColorBrush(Color.FromRgb(0x74, 0x74, 0x74));
            var linePen = new Pen(lineBrush, 1);

            drawingContext.DrawText(text, new Point(left + (panelWidth - text.Width) / 2d, textTop));
            drawingContext.DrawLine(linePen, new Point(lineX, lineY), new Point(lineX + lineWidth, lineY));
            drawingContext.DrawLine(linePen, new Point(lineX, lineY - tickHeight), new Point(lineX, lineY + tickHeight));
            drawingContext.DrawLine(linePen, new Point(lineX + lineWidth, lineY - tickHeight), new Point(lineX + lineWidth, lineY + tickHeight));
        }

        private MapDistanceUnit ResolveScaleUnit()
        {
            if (ScaleBarUnit != MapDistanceUnit.Auto)
            {
                return ScaleBarUnit;
            }

            try
            {
                var region = RegionInfo.CurrentRegion;
                return region != null && region.IsMetric
                    ? MapDistanceUnit.KilometersMeters
                    : MapDistanceUnit.MilesFeet;
            }
            catch
            {
                return MapDistanceUnit.KilometersMeters;
            }
        }

        private static double GetMetersPerPixel(double latitude, int zoom)
        {
            var clampedLatitude = Math.Max(-85.05112878, Math.Min(85.05112878, latitude));
            var radians = clampedLatitude * Math.PI / 180d;
            var scale = TileSize * Math.Pow(2, zoom);
            if (scale <= 0)
            {
                return 0;
            }

            return Math.Cos(radians) * 2d * Math.PI * EarthRadiusMeters / scale;
        }

        private static double LargestNiceNumber(double value)
        {
            if (value <= 0 || double.IsNaN(value) || double.IsInfinity(value))
            {
                return 0;
            }

            var exponent = Math.Pow(10d, Math.Floor(Math.Log10(value)));
            var normalized = value / exponent;
            double step;
            if (normalized >= 5d)
            {
                step = 5d;
            }
            else if (normalized >= 2d)
            {
                step = 2d;
            }
            else
            {
                step = 1d;
            }

            return step * exponent;
        }

        private static string FormatDistanceLabel(double value, string unit)
        {
            var culture = CultureInfo.CurrentUICulture;
            var numberFormat = value >= 100 ? "0" : (value >= 10 ? "0" : "0.#");
            if (culture != null && culture.Name.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(unit, "km", StringComparison.OrdinalIgnoreCase))
                {
                    unit = "公里";
                }
                else if (string.Equals(unit, "m", StringComparison.OrdinalIgnoreCase))
                {
                    unit = "米";
                }
                else if (string.Equals(unit, "mi", StringComparison.OrdinalIgnoreCase))
                {
                    unit = "英里";
                }
                else if (string.Equals(unit, "ft", StringComparison.OrdinalIgnoreCase))
                {
                    unit = "英尺";
                }
            }

            return value.ToString(numberFormat, culture) + " " + unit;
        }

        private static FormattedText CreateFormattedText(string text, Typeface typeface, double emSize, Brush foreground)
        {
#if NETFRAMEWORK || NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471 || NET472
#pragma warning disable 0618
            return new FormattedText(
                text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                typeface,
                emSize,
                foreground);
#pragma warning restore 0618
#else
            return new FormattedText(
                text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                typeface,
                emSize,
                foreground,
                1.0);
#endif
        }

        private static string BuildTileUrl(TileRequest request)
        {
            var source = request.Config.TileSource;
            if (source == null)
            {
                return string.Empty;
            }

            var uri = source.GetTileUri(request.TileX, request.TileY, request.Zoom);
            return uri == null ? string.Empty : uri.ToString();
        }

        private static void DownloadTileImageAsync(TileRequest request, Action<BitmapSource, string> callback)
        {
            EnsureNetworkSecuritySettings();

            if (request == null)
            {
                callback(null, "empty request");
                return;
            }

            var url = BuildTileUrl(request);
            if (string.IsNullOrWhiteSpace(url))
            {
                callback(null, "empty url");
                return;
            }

            try
            {
                var client = new WebClient();
                client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";
                client.Headers[HttpRequestHeader.Accept] = "image/png,image/jpeg,*/*;q=0.8";
                client.Proxy = WebRequest.DefaultWebProxy;
                if (client.Proxy != null)
                {
                    client.Proxy.Credentials = CredentialCache.DefaultCredentials;
                }

                client.DownloadDataCompleted += (sender, e) =>
                {
                    try
                    {
                        if (e.Error != null)
                        {
                            callback(null, ExtractWebExceptionMessage(e.Error));
                            return;
                        }

                        if (e.Cancelled)
                        {
                            callback(null, "cancelled");
                            return;
                        }

                        var data = e.Result;
                        if (data == null || data.Length == 0)
                        {
                            callback(null, "empty response body");
                            return;
                        }

                        try
                        {
                            var bitmap = DecodeBitmap(data);
                            callback(bitmap, null);
                        }
                        catch (Exception)
                        {
                            callback(null, "decode/unknown");
                        }
                    }
                    finally
                    {
                        if (sender is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                };

                client.DownloadDataAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                callback(null, ex.Message);
            }
        }

        private static BitmapSource DecodeBitmap(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            using (var memory = new MemoryStream(data))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = memory;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }

        private static string ExtractWebExceptionMessage(Exception exception)
        {
            var webException = exception as WebException;
            if (webException == null)
            {
                return exception == null ? "unknown" : exception.Message;
            }

            var response = webException.Response as HttpWebResponse;
            if (response != null)
            {
                return ((int)response.StatusCode).ToString(CultureInfo.InvariantCulture) + " " + response.StatusCode;
            }

            return webException.Status.ToString();
        }

        private void ClearTileCache()
        {
            lock (_cacheLock)
            {
                _tileCache.Clear();
                _failedTiles.Clear();
                _failedTileAttempts.Clear();
                _loadingTiles.Clear();
                _queuedTiles.Clear();
                _pendingTiles.Clear();
                _lastViewportKey = null;
            }
        }

        private static Point ToPixel(double latitude, double longitude, int zoom)
        {
            var sinLatitude = Math.Sin(latitude * Math.PI / 180.0);
            var scale = TileSize * Math.Pow(2, zoom);

            var x = (NormalizeLongitude(longitude) + 180.0) / 360.0 * scale;
            var y = (0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI)) * scale;
            return new Point(x, y);
        }

        private static Point ToLatLon(double pixelX, double pixelY, int zoom)
        {
            var scale = TileSize * Math.Pow(2, zoom);
            var lon = NormalizeLongitude((pixelX / scale) * 360.0 - 180.0);
            var n = Math.PI - (2.0 * Math.PI * pixelY) / scale;
            var lat = 180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));
            return new Point(lat, lon);
        }

        private static double NormalizeLongitude(double longitude)
        {
            if (double.IsNaN(longitude) || double.IsInfinity(longitude))
            {
                return 0d;
            }

            var normalized = longitude % 360d;
            if (normalized < -180d)
            {
                normalized += 360d;
            }
            else if (normalized >= 180d)
            {
                normalized -= 360d;
            }

            return normalized;
        }

    }
}
