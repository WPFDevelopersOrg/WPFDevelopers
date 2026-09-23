using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFDevelopers.Controls
{
    public class MapLayer : Panel
    {
        private Size _viewportSize;

        protected static readonly DependencyProperty ViewportPositionProperty =
            DependencyProperty.RegisterAttached(
                "ViewportPosition",
                typeof(Point),
                typeof(MapLayer),
                new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.AffectsParentArrange));

        protected static readonly DependencyProperty PositionOriginProperty =
            DependencyProperty.RegisterAttached(
                "PositionOrigin",
                typeof(Point),
                typeof(MapLayer),
                new FrameworkPropertyMetadata(new Point(0.5, 0.5), FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public MapLayer()
        {
            IsHitTestVisible = false;
            ClipToBounds = false;
        }

        protected static void SetViewportPosition(DependencyObject element, Point position)
        {
            element.SetValue(ViewportPositionProperty, position);
        }

        protected static void SetPositionOrigin(DependencyObject element, Point origin)
        {
            element.SetValue(PositionOriginProperty, origin);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _viewportSize = new Size(
                double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width,
                double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height);

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(_viewportSize);
            }

            return _viewportSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _viewportSize = finalSize;
            foreach (UIElement child in InternalChildren)
            {
                var position = (Point)child.GetValue(ViewportPositionProperty);
                var origin = (Point)child.GetValue(PositionOriginProperty);
                var desiredSize = child.DesiredSize;
                child.Arrange(new Rect(
                    position.X - origin.X * desiredSize.Width,
                    position.Y - origin.Y * desiredSize.Height,
                    desiredSize.Width,
                    desiredSize.Height));
            }

            return finalSize;
        }
    }

    public class MapTileLayer : MapLayer
    {
        private readonly Dictionary<string, Image> _tileImages = new Dictionary<string, Image>();

        public MapTileSource TileSource { get; set; }

        public void UpdateViewport(int zoom, Point viewTopLeft, Size viewportSize, Dictionary<string, BitmapSource> tileCache, string cacheKeyPrefix = "")
        {
            if (viewportSize.Width <= 0 || viewportSize.Height <= 0)
            {
                Children.Clear();
                _tileImages.Clear();
                return;
            }

            var firstTileX = (int)Math.Floor(viewTopLeft.X / 256.0);
            var firstTileY = (int)Math.Floor(viewTopLeft.Y / 256.0);
            var tileCountX = (int)Math.Ceiling(viewportSize.Width / 256.0) + 2;
            var tileCountY = (int)Math.Ceiling(viewportSize.Height / 256.0) + 2;
            var maxTile = (1 << zoom) - 1;
            var visibleKeys = new HashSet<string>();

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

                    var wrappedX = tileX % (1 << zoom);
                    if (wrappedX < 0)
                    {
                        wrappedX += (1 << zoom);
                    }

                    var cacheKey = cacheKeyPrefix + zoom + ":" + wrappedX + ":" + tileY;
                    visibleKeys.Add(cacheKey);

                    Image image;
                    if (!_tileImages.TryGetValue(cacheKey, out image))
                    {
                        image = new Image
                        {
                            IsHitTestVisible = false,
                            Stretch = Stretch.Fill,
                            SnapsToDevicePixels = true,
                            UseLayoutRounding = true,
                            Width = 256,
                            Height = 256,
                            Opacity = 1,
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left
                        };
                        _tileImages[cacheKey] = image;
                        Children.Add(image);
                    }

                    var pixelX = tileX * 256.0 - viewTopLeft.X;
                    var pixelY = tileY * 256.0 - viewTopLeft.Y;
                    if (TryGetBestAvailableTile(tileCache, cacheKeyPrefix, zoom, wrappedX, tileY, out var tileBitmap, out var isFallback))
                    {
                        image.Source = tileBitmap;
                        image.Opacity = isFallback ? 0.9 : 1.0;
                    }
                    else
                    {
                        image.Source = null;
                        image.Opacity = 1.0;
                    }

                    SetViewportPosition(image, new Point(pixelX, pixelY));
                    SetPositionOrigin(image, new Point(0, 0));
                }
            }

            foreach (var key in new List<string>(_tileImages.Keys))
            {
                if (!visibleKeys.Contains(key))
                {
                    var staleImage = _tileImages[key];
                    _tileImages.Remove(key);
                    Children.Remove(staleImage);
                }
            }

            InvalidateMeasure();
        }

        private static bool TryGetBestAvailableTile(
            Dictionary<string, BitmapSource> tileCache,
            string cacheKeyPrefix,
            int zoom,
            int tileX,
            int tileY,
            out BitmapSource bitmap,
            out bool isFallback)
        {
            bitmap = null;
            isFallback = false;
            if (tileCache == null)
            {
                return false;
            }

            var key = cacheKeyPrefix + zoom + ":" + tileX + ":" + tileY;
            if (tileCache.TryGetValue(key, out bitmap) && bitmap != null)
            {
                return true;
            }

            for (var parentZoom = zoom - 1; parentZoom >= 0; parentZoom--)
            {
                var levelDelta = zoom - parentZoom;
                var split = 1 << levelDelta;
                if (split <= 0)
                {
                    continue;
                }

                var parentX = tileX / split;
                var parentY = tileY / split;
                var parentKey = cacheKeyPrefix + parentZoom + ":" + parentX + ":" + parentY;
                if (!tileCache.TryGetValue(parentKey, out var parentBitmap) || parentBitmap == null)
                {
                    continue;
                }

                var sourceWidth = Math.Max(1, parentBitmap.PixelWidth);
                var sourceHeight = Math.Max(1, parentBitmap.PixelHeight);
                var childX = tileX % split;
                var childY = tileY % split;

                var cropX = sourceWidth * childX / split;
                var cropY = sourceHeight * childY / split;
                var cropWidth = Math.Max(1, sourceWidth / split);
                var cropHeight = Math.Max(1, sourceHeight / split);

                if (cropX >= sourceWidth || cropY >= sourceHeight)
                {
                    continue;
                }

                if (cropX + cropWidth > sourceWidth)
                {
                    cropWidth = sourceWidth - cropX;
                }

                if (cropY + cropHeight > sourceHeight)
                {
                    cropHeight = sourceHeight - cropY;
                }

                if (cropWidth <= 0 || cropHeight <= 0)
                {
                    continue;
                }

                var crop = new CroppedBitmap(parentBitmap, new Int32Rect(cropX, cropY, cropWidth, cropHeight));
                if (crop.CanFreeze)
                {
                    crop.Freeze();
                }

                bitmap = crop;
                isFallback = true;
                return true;
            }

            return false;
        }
    }

    internal sealed class MapShapeLayer : MapLayer
    {
        public void ClearShapes()
        {
            if (Children.Count == 0)
            {
                return;
            }

            Children.Clear();
        }

        public void UpdateShapes(IList<MapPolylineLayoutInfo> polylines, IList<MapPolygonLayoutInfo> polygons)
        {
            Children.Clear();

            var drawItems = new List<Tuple<int, int, bool, MapPolygonLayoutInfo, MapPolylineLayoutInfo>>();
            var sequence = 0;

            if (polygons != null)
            {
                for (var i = 0; i < polygons.Count; i++)
                {
                    var polygonLayout = polygons[i];
                    if (polygonLayout?.Polygon == null || polygonLayout.Points == null || polygonLayout.Points.Count < 3)
                    {
                        continue;
                    }

                    drawItems.Add(Tuple.Create(polygonLayout.ShapeZIndex, sequence++, true, polygonLayout, (MapPolylineLayoutInfo)null));
                }
            }

            if (polylines != null)
            {
                for (var i = 0; i < polylines.Count; i++)
                {
                    var lineLayout = polylines[i];
                    if (lineLayout?.Polyline == null || lineLayout.Points == null || lineLayout.Points.Count < 2)
                    {
                        continue;
                    }

                    drawItems.Add(Tuple.Create(lineLayout.ShapeZIndex, sequence++, false, (MapPolygonLayoutInfo)null, lineLayout));
                }
            }

            drawItems.Sort((a, b) =>
            {
                var zCompare = a.Item1.CompareTo(b.Item1);
                return zCompare != 0 ? zCompare : a.Item2.CompareTo(b.Item2);
            });

            for (var i = 0; i < drawItems.Count; i++)
            {
                var drawItem = drawItems[i];
                if (drawItem.Item3)
                {
                    var polygonLayout = drawItem.Item4;
                    var polygon = new Polygon
                    {
                        IsHitTestVisible = false,
                        Stroke = polygonLayout.Polygon.Stroke,
                        Fill = polygonLayout.Polygon.Fill,
                        StrokeThickness = polygonLayout.Polygon.StrokeThickness,
                        Opacity = polygonLayout.Polygon.Opacity,
                        Points = new PointCollection(polygonLayout.Points)
                    };

                    SetViewportPosition(polygon, new Point(0, 0));
                    SetPositionOrigin(polygon, new Point(0, 0));
                    Children.Add(polygon);
                }
                else
                {
                    var lineLayout = drawItem.Item5;
                    var polyline = new Polyline
                    {
                        IsHitTestVisible = false,
                        Stroke = lineLayout.Polyline.Stroke,
                        StrokeThickness = lineLayout.Polyline.StrokeThickness,
                        Opacity = lineLayout.Polyline.Opacity,
                        StrokeLineJoin = PenLineJoin.Round,
                        StrokeStartLineCap = PenLineCap.Round,
                        StrokeEndLineCap = PenLineCap.Round,
                        Points = new PointCollection(lineLayout.Points)
                    };

                    SetViewportPosition(polyline, new Point(0, 0));
                    SetPositionOrigin(polyline, new Point(0, 0));
                    Children.Add(polyline);
                }
            }

            InvalidateMeasure();
        }
    }

    internal sealed class MapOverlayLayer : FrameworkElement
    {
        public System.Action<DrawingContext> RenderContent { get; set; }

        public MapOverlayLayer()
        {
            IsHitTestVisible = false;
            SnapsToDevicePixels = true;
            UseLayoutRounding = true;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            RenderContent?.Invoke(drawingContext);
        }
    }

    internal sealed class MapItemsLayer : MapLayer
    {
        private readonly List<UIElement> _items = new List<UIElement>();
        private static readonly DependencyProperty RenderStateKeyProperty =
            DependencyProperty.RegisterAttached(
                "RenderStateKey",
                typeof(string),
                typeof(MapItemsLayer),
                new FrameworkPropertyMetadata(string.Empty));

        public void ClearItems()
        {
            if (_items.Count == 0)
            {
                return;
            }

            Children.Clear();
            _items.Clear();
        }

        public void UpdateItems(IList<MapItemLayoutInfo> layouts, DataTemplate itemTemplate, DataTemplate clusterTemplate, double defaultSize)
        {
            if (layouts == null || layouts.Count == 0)
            {
                ClearItems();
                return;
            }

            SyncItems(layouts.Count);
            for (var i = 0; i < layouts.Count; i++)
            {
                var layout = layouts[i];
                var pushpin = layout.Item as Pushpin;
                var cluster = layout.Item as PushpinCluster;
                var effectiveTemplate = cluster != null ? clusterTemplate : (pushpin?.Template ?? itemTemplate);
                var presenter = _items[i] as ContentPresenter;
                if (presenter == null)
                {
                    presenter = new ContentPresenter { IsHitTestVisible = false };
                    _items[i] = presenter;
                    Children[i] = presenter;
                }

                presenter.ContentTemplate = effectiveTemplate;
                presenter.Content = layout.Item;
                presenter.MinWidth = defaultSize;
                presenter.MinHeight = defaultSize;
                SetViewportPosition(presenter, new Point(layout.X, layout.Y));
                SetPositionOrigin(presenter, cluster != null ? new Point(0.5, 0.5) : (pushpin?.AnchorPoint ?? new Point(0.5, 0.5)));
            }

            InvalidateMeasure();
        }

        public void UpdateDefaultItems(
            IList<MapItemLayoutInfo> layouts,
            Brush fill,
            Brush stroke,
            double size,
            PushpinVisualMode mode,
            ImageSource image,
            Brush clusterFill,
            Brush clusterText,
            double clusterRadius,
            int clusterMinimumCount)
        {
            if (layouts == null || layouts.Count == 0)
            {
                ClearItems();
                return;
            }

            SyncItems(layouts.Count);
            for (var i = 0; i < layouts.Count; i++)
            {
                var layout = layouts[i];
                var item = layout.Item as PushpinCluster;
                var pushpin = layout.Item as Pushpin;
                var isCluster = item != null && item.Count >= Math.Max(2, clusterMinimumCount);
                var renderStateKey = isCluster
                    ? "cluster"
                    : string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}:{1}:{2}",
                        pushpin != null && pushpin.IsSelected,
                        pushpin != null && pushpin.IsActivated,
                        mode);

                var existing = _items[i];
                var shouldReplace = !(existing is FrameworkElement existingElement
                    && existingElement.Tag is MapItemLayoutInfo existingLayout
                    && ReferenceEquals(existingLayout.Item, layout.Item)
                    && string.Equals(existingElement.GetValue(RenderStateKeyProperty) as string, renderStateKey, StringComparison.Ordinal));
                if (shouldReplace)
                {
                    var replacement = CreateDefaultItem(isCluster, pushpin, fill, stroke, size, mode, image, clusterFill, clusterText, clusterRadius, item);
                    if (Children.Count > i)
                    {
                        Children.RemoveAt(i);
                    }

                    Children.Insert(i, replacement);
                    _items[i] = replacement;
                }

                var element = _items[i];
                var usesDefaultPin = !isCluster && !(mode == PushpinVisualMode.Icon && image != null);
                var half = isCluster ? Math.Max(size, Math.Min(clusterRadius, size * 0.9 + (item.Count * 0.45))) : size / 2.0;
                var clusterOuterDiameter = half * 2.0 + 10.0;
                var width = usesDefaultPin ? Math.Max(16.0, size * 2.5) : (isCluster ? clusterOuterDiameter : half * 2.0);
                var height = usesDefaultPin ? Math.Max(24.0, size * 3.4) : (isCluster ? clusterOuterDiameter : half * 2.0);
                if (element is FrameworkElement frameworkElement)
                {
                    frameworkElement.Tag = layout;
                    frameworkElement.SetValue(RenderStateKeyProperty, renderStateKey);
                    frameworkElement.Width = width;
                    frameworkElement.Height = height;
                    SetViewportPosition(frameworkElement, new Point(layout.X, layout.Y));
                    SetPositionOrigin(frameworkElement, usesDefaultPin ? new Point(0.5, 1.0) : new Point(0.5, 0.5));
                }
            }

            InvalidateMeasure();
        }

        private UIElement CreateDefaultItem(
            bool isCluster,
            Pushpin pushpin,
            Brush fill,
            Brush stroke,
            double size,
            PushpinVisualMode mode,
            ImageSource image,
            Brush clusterFill,
            Brush clusterText,
            double clusterRadius,
            PushpinCluster cluster)
        {
            if (isCluster)
            {
                var clusterCoreDiameter = Math.Max(size, Math.Min(clusterRadius, size * 0.9 + (cluster.Count * 0.45))) * 2.0;
                var clusterOuterDiameter = clusterCoreDiameter + 10.0;
                return new MapCluster
                {
                    CountText = cluster.CountText,
                    ClusterFill = clusterFill ?? new SolidColorBrush(Color.FromRgb(0xE7, 0x4A, 0x4A)),
                    ClusterTextBrush = clusterText ?? Brushes.White,
                    Width = clusterOuterDiameter,
                    Height = clusterOuterDiameter
                };
            }

            if (mode == PushpinVisualMode.Icon && image != null)
            {
                var imageControl = new Image
                {
                    Source = image,
                    Width = size,
                    Height = size,
                    Stretch = Stretch.Fill,
                    IsHitTestVisible = false
                };
                return imageControl;
            }

            return CreateDefaultMapPin(pushpin);
        }

        private static UIElement CreateDefaultMapPin(Pushpin pushpin)
        {
            return new MapPin
            {
                IsSelected = pushpin != null && pushpin.IsSelected,
                IsActivated = pushpin != null && pushpin.IsActivated,
                AnchorPoint = new Point(0.5, 1.0)
            };
        }

        private void SyncItems(int neededCount)
        {
            while (_items.Count > neededCount)
            {
                var lastIndex = _items.Count - 1;
                Children.RemoveAt(lastIndex);
                _items.RemoveAt(lastIndex);
            }

            while (_items.Count < neededCount)
            {
                var presenter = new ContentPresenter
                {
                    IsHitTestVisible = false
                };
                Children.Add(presenter);
                _items.Add(presenter);
            }
        }
    }

}
