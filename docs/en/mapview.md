# MapView Usage Guide

> This document focuses on practical usage of `wd:MapView` in WPF projects, including configuration, layer usage, tile source setup, and demo patterns.
> This is a runtime usage guide, not the conceptual background document.

## 1. Overview

`wd:MapView` is a WPF desktop map control designed for scenarios such as:

- vehicle tracking
- resource distribution
- geographic monitoring
- area visualization
- station / device / warehouse location display

It is not a simple browser-based map embed. Instead, it is a WPF-native map control that renders data through tiles, layers, overlays, and custom templates.

This makes it more suitable for enterprise desktop apps and older Windows environments where WebView-based solutions may be unstable or unavailable.

---

## 2. Typical Use Cases

MapView is ideal for:

- logistics and dispatch systems
- GIS dashboards
- regional data visualization
- service/asset mapping
- custom desktop map experiences

---

## 3. Basic Usage

### 3.1 Use in XAML

```xml
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:wd="https://github.com/WPFDevelopersOrg/WPFDevelopers"
        Title="MapView Demo" Width="1100" Height="700">
    <Grid>
        <wd:MapView
            x:Name="mapView"
            CenterLatitude="39.9132801985722"
            CenterLongitude="116.392009995601"
            ZoomLevel="5"
            TileStyle="7" />
    </Grid>
</Window>
```

### 3.2 Set the tile source

```csharp
mapView.TileSource = new TiandituTileSource("vec_w", 0, "your-tianditu-key");
mapView.AnnotationTileSource = new TiandituTileSource("cva_w", 0, "your-tianditu-key");
```

Supported providers may include:

- `TiandituTileSource`
- `AMapTileSource`
- `TencentTileSource`
- `OSMTileSource`
- custom tile sources

> If the key or layer is empty, the map should not continue sending invalid requests.

---

## 4. Map Data Model

### 4.1 Coordinate usage

```csharp
var location = new MapLocation
{
    Latitude = 39.9132801985722,
    Longitude = 116.392009995601
};
```

#### 4.1.1 Coordinate systems and conversion

MapView works with geographic coordinates, but in real business scenarios the same location may arrive from different sources using different coordinate systems. Common examples include:

- `WGS84`: GPS / handheld device coordinates
- `GCJ02`: commonly used by AMap and Tencent in China
- `BD09`: Baidu coordinate system
- `Web Mercator`: tile-map projection coordinate system
- `Tile X/Y/Z`: tile request coordinates

If your data comes from different devices or third-party providers, it is important to identify the source first and then normalize it before display. To keep the map control independent from vendor SDK details, the project includes a common coordinate utility set:

- [src/WPFDevelopers.Shared/Controls/MapView/MapCoordinateType.cs](../../src/WPFDevelopers.Shared/Controls/MapView/MapCoordinateType.cs)
- [src/WPFDevelopers.Shared/Controls/MapView/MapGeoPoint.cs](../../src/WPFDevelopers.Shared/Controls/MapView/MapGeoPoint.cs)
- [src/WPFDevelopers.Shared/Controls/MapView/MapCoordinateHelper.cs](../../src/WPFDevelopers.Shared/Controls/MapView/MapCoordinateHelper.cs)
- [src/WPFDevelopers.Shared/Controls/MapView/MapProjectionHelper.cs](../../src/WPFDevelopers.Shared/Controls/MapView/MapProjectionHelper.cs)

A typical conversion flow looks like this:

```csharp
var gps = new MapGeoPoint(116.397428, 39.90923, MapCoordinateType.Wgs84);
var gcj = MapCoordinateHelper.Convert(gps, MapCoordinateType.Gcj02);
var tile = MapProjectionHelper.LonLatToTileXY(gcj.Longitude, gcj.Latitude, 18);
```

This pattern helps with:

- keeping GPS data in its original semantic form;
- normalizing AMap / Tencent / Baidu coordinate data;
- allowing MapView to operate on geographic or projected coordinates without vendor-specific logic;
- supporting tile-based providers such as Tianditu and Web Mercator rendering.

> If the source coordinate type is not explicit, it is safer to keep a `CoordinateType` field in the protocol or data model rather than guessing from the numeric values alone.

### 4.2 Pushpin usage

The current MapView API supports inline pushpins and layer-based pushpins. For a simple single marker, you can add it directly to the map:

```xml
<wd:MapView x:Name="mapView" CenterLatitude="39.9132801985722" CenterLongitude="116.392009995601" ZoomLevel="5">
    <wd:Pushpin
        Title="Beijing"
        Latitude="39.9132801985722"
        Longitude="116.392009995601"
        AnchorPoint="0.5,1" />
</wd:MapView>
```

```csharp
var pushpin = new Pushpin
{
    Title = "Beijing",
    Latitude = 39.9132801985722,
    Longitude = 116.392009995601,
    AnchorPoint = new Point(0.5, 1)
};

mapView.Pushpins.Add(pushpin);
```

> Use `Pushpin.Template` for a single custom marker, or set `MapView.PushpinTemplate` for a default visual shared by all pushpins in the control.

### 4.3 Polylines and polygons

```xml
<wd:MapView.Polylines>
    <wd:MapPolyline Stroke="#22C55E" StrokeThickness="3">
        <wd:MapPolyline.Points>
            <wd:MapLocation Latitude="39.785633" Longitude="116.557292" />
            <wd:MapLocation Latitude="36.6797276003243" Longitude="118.495410536117" />
            <wd:MapLocation Latitude="31.915951" Longitude="107.240750" />
        </wd:MapPolyline.Points>
    </wd:MapPolyline>
</wd:MapView.Polylines>
```

### 4.4 Circles and rectangles

```xml
<wd:MapView.Circles>
    <wd:MapCircle
        CenterLatitude="39.912"
        CenterLongitude="116.205"
        RadiusMeters="1200"
        Fill="#33EF4444"
        Stroke="#FFEF4444"
        StrokeThickness="2" />
</wd:MapView.Circles>

<wd:MapView.Rectangles>
    <wd:MapRectangle
        MinLatitude="39.902"
        MinLongitude="116.248"
        MaxLatitude="39.936"
        MaxLongitude="116.288"
        Fill="#334F46E5"
        Stroke="#FF4F46E5"
        StrokeThickness="2" />
</wd:MapView.Rectangles>
```

`MapCircle` and `MapRectangle` are rendered internally as polygon paths and participate in `MapFeatureClicked` hit detection.

### 4.5 Closed paths and XAML usage notes

`MapPolyline` supports explicit closure:

```xml
<wd:MapPolyline
    IsClosed="True"
    Stroke="#FFF97316"
    StrokeThickness="3">
    <wd:MapPolyline.Points>
        <wd:MapLocation Latitude="39.918694" Longitude="116.148180" />
        <wd:MapLocation Latitude="39.920537" Longitude="116.156420" />
        <wd:MapLocation Latitude="39.918694" Longitude="116.148180" />
    </wd:MapPolyline.Points>
</wd:MapPolyline>
```

The default is `false`, which means the control renders the path as an open polyline. Only when `IsClosed="True"` does it automatically append the closing segment back to the start point. This is useful for looped routes, enclosure boundaries, or fence-like areas.

Also, `MapView` uses `InlinePushpins` as its default content property. As a result, direct child elements placed inside `<wd:MapView>...</wd:MapView>` are treated as inline pushpins. To avoid XAML parsing ambiguity, keep `MapView.Polylines`, `MapView.Polygons`, and direct `Pushpin` children grouped rather than interleaved:

```xml
<wd:MapView ...>
    <wd:MapView.Polylines>
        ...
    </wd:MapView.Polylines>

    <wd:MapView.Polygons>
        ...
    </wd:MapView.Polygons>

    <wd:Pushpin ... />
    <wd:Pushpin ... />
</wd:MapView>
```

In short: collection property elements and direct child pushpins should be grouped, not mixed together in one sequence.

---

## 5. Map click event and command

MapView exposes a unified click API for map interaction:

- `MapClicked` routed event
- `MapClickCommand` command

```xml
<wd:MapView
    x:Name="DemoMap"
    MapClicked="DemoMap_MapClicked"
    MapClickCommand="{Binding MapClickCommand}" />
```

The event args type is `MapClickEventArgs`, which includes:

- `Latitude`
- `Longitude`
- `ScreenX`
- `ScreenY`
- `IsEmptyAreaClick`
- `ClickedPushpin`

Example:

```csharp
private void DemoMap_MapClicked(object sender, RoutedEventArgs e)
{
    var args = e as MapClickEventArgs;
    if (args == null)
    {
        return;
    }

    var latitude = args.Latitude;
    var longitude = args.Longitude;
    var isEmptyArea = args.IsEmptyAreaClick;
}
```

The command receives the same `MapClickEventArgs`, which is useful for MVVM code paths:

```csharp
public ICommand MapClickCommand { get; }

public MainViewModel()
{
    MapClickCommand = new RelayCommand<MapClickEventArgs>(args =>
    {
        if (args == null)
        {
            return;
        }

        // Handle coordinate click here
    });
}
```

> This is the preferred place to handle map-click semantics instead of manually translating mouse positions to geographic coordinates in each page.

### 5.1 Feature click event and command (Pushpin / Polyline / Polygon)

MapView also exposes feature-level click APIs:

- `MapFeatureClicked` routed event
- `MapFeatureClickCommand` command

```xml
<wd:MapView
    x:Name="DemoMap"
    MapFeatureClicked="DemoMap_MapFeatureClicked"
    MapFeatureClickCommand="{Binding MapFeatureClickCommand}" />
```

The event args type is `MapFeatureClickEventArgs`, including:

- `FeatureType` (`None` / `Pushpin` / `Polyline` / `Polygon`)
- `ClickedPushpin`
- `ClickedPolyline`
- `ClickedPolygon`
- `Latitude`
- `Longitude`
- `ScreenX`
- `ScreenY`

Example:

```csharp
private void DemoMap_MapFeatureClicked(object sender, RoutedEventArgs e)
{
    var args = e as MapFeatureClickEventArgs;
    if (args == null)
    {
        return;
    }

    if (args.FeatureType == MapFeatureType.Pushpin && args.ClickedPushpin != null)
    {
        // Pushpin click
    }
    else if (args.FeatureType == MapFeatureType.Polyline && args.ClickedPolyline != null)
    {
        // Polyline click
    }
    else if (args.FeatureType == MapFeatureType.Polygon && args.ClickedPolygon != null)
    {
        // Polygon click
    }
}
```

MVVM command example:

```csharp
public ICommand MapFeatureClickCommand { get; }

public MainViewModel()
{
    MapFeatureClickCommand = new RelayCommand<MapFeatureClickEventArgs>(args =>
    {
        if (args == null)
        {
            return;
        }

        // Open different business panels based on FeatureType
    });
}
```

Behavior notes:

- One click resolves to at most one feature (to avoid opening multiple panels for overlapping items).
- If you subscribe to both `MapFeatureClicked` and `MapClicked`, one user click can trigger both callbacks:
  - feature-level callback (`MapFeatureClicked`)
  - map-level callback (`MapClicked`)
- `MapClicked.IsEmptyAreaClick` is `false` when any feature (point/line/polygon) is hit.

---

## 6. Layers

MapView supports multiple layer types:

- base map layer
- pushpin layer
- polyline layer
- polygon layer
- clustering layer
- custom overlay layer

### 5.1 Pushpin layer

```csharp
var layer = new MapPushpinLayer
{
    Name = "VehicleLayer",
    IsVisible = true,
    EnableClustering = false
};

layer.Pushpins.Add(new Pushpin
{
    Title = "Car",
    Latitude = 39.9132801985722,
    Longitude = 116.392009995601,
    AnchorPoint = new Point(0.5, 0.5)
});

mapView.PushpinLayers.Add(layer);
```

### 5.2 Clustering

```csharp
var layer = new MapPushpinLayer
{
    Name = "ClusterLayer",
    EnableClustering = true,
    Pushpins = new ObservableCollection<Pushpin>()
};

mapView.PushpinLayers.Add(layer);
```

The current API exposes clustering behavior on the layer, while the visual template is configured on the map control itself:

```csharp
mapView.ClusterRadius = 35;
mapView.ClusterMinimumCount = 2;
mapView.ClusterTemplate = (DataTemplate)Application.Current.FindResource("ClusterTemplate");
```

Common clustering properties include:

- `MapPushpinLayer.EnableClustering`
- `MapView.ClusterRadius`
- `MapView.ClusterMinimumCount`
- `MapView.ClusterTemplate`

> `ClusterTemplate` is not a property on `MapPushpinLayer`; it is set on `MapView`.

---

## 6. Custom Templates

One of the main advantages of MapView is that it can use WPF templates naturally.

### 6.1 Custom pushpin template

```xml
<DataTemplate x:Key="CarPushpinTemplate">
    <Grid Width="40" Height="40" RenderTransformOrigin="0.5,0.5">
        <Grid.RenderTransform>
            <RotateTransform Angle="{Binding Tag}" />
        </Grid.RenderTransform>
        <wd:SvgViewer
            Width="40"
            Height="40"
            Source="pack://application:,,,/WPFDevelopers.Samples;component/Resources/Svg/Car.svg" />
    </Grid>
</DataTemplate>
```

```csharp
var car = new Pushpin
{
    Title = "Car",
    Latitude = 39.9132801985722,
    Longitude = 116.392009995601,
    AnchorPoint = new Point(0.5, 0.5),
    Template = (DataTemplate)Application.Current.FindResource("CarPushpinTemplate"),
    Tag = 90
};

mapView.Pushpins.Add(car);
```

If you want the same template to apply to all pushpins in a map, set `mapView.PushpinTemplate` instead of attaching a template to each item.

### 6.2 Custom cluster template

```xml
<DataTemplate x:Key="ClusterTemplate">
    <Border Width="30" Height="30" CornerRadius="15" Background="#F97316">
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center"
                   Foreground="White" Text="{Binding Count}" />
    </Border>
</DataTemplate>
```

```csharp
mapView.ClusterTemplate = (DataTemplate)Application.Current.FindResource("ClusterTemplate");
```

---

## 7. Vehicle Route Example

A typical route demo in MapView includes:

- using a dedicated non-clustered layer for the vehicle
- calculating heading based on route direction
- keeping the vehicle icon aligned with the movement direction
- using `Tag` to carry heading value

Sample references:

- [src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml)
- [src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml.cs](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml.cs)

### Heading calculation example

```csharp
private static double CalculateHeading(PushpinModel from, PushpinModel to)
{
    var dLongitude = to.Longitude - from.Longitude;
    var dLatitude = to.Latitude - from.Latitude;
    var radians = Math.Atan2(dLongitude, dLatitude);
    var degrees = radians * 180d / Math.PI;
    return NormalizeAngle(degrees);
}
```

```xml
<Grid.RenderTransform>
    <RotateTransform Angle="{Binding Tag}" />
</Grid.RenderTransform>
```

---

## 8. Tianditu Key Configuration

When using Tianditu, you need to obtain a valid key and avoid hardcoding it into source files.

Application URL:

https://oauth.tianditu.gov.cn/login?service=https%3A%2F%2Fcloudcenter.tianditu.gov.cn%2Fapi%2Fsvc%2Flogin%3Furl%3Dhttps%25253A%25252F%25252Fcloudcenter.tianditu.gov.cn%25252Fcenter%25252Fdevelopment%25252FmyApp

Recommended pattern:

```csharp
var tiandituKey = ConfigurationManager.AppSettings["TiandituKey"] ?? string.Empty;
mapView.TileSource = new TiandituTileSource("vec_w", 0, tiandituKey);
```

---

## 9. Common Issues

### Map not visible

Check:

- token/key is configured correctly
- tile source is assigned
- URL parameters are valid
- network access is available

### Marker position wrong

Check:

- latitude/longitude order
- anchor point
- template transform
- map center / zoom level

### Clustering not working as expected

Check:

- `EnableClustering` value
- `ClusterRadius`
- template fallback styling
- whether some objects should be in a separate non-clustered layer

---

## 10. Recommended Practice

For real projects, it is best to:

1. separate base map and business layers
2. use dedicated layers for vehicles, stations, areas, etc.
3. disable clustering for moving vehicle objects when needed
4. keep key/token in configuration instead of source code
5. protect empty key / empty layer scenarios
6. test heading and anchor alignment before shipping

---

## 11. Example References

- [src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml)
- [src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml.cs](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml.cs)
- [src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewFeaturesWindow.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewFeaturesWindow.xaml)
- [src/WPFDevelopers.Samples.Shared/ExampleViews/Map/TiandituTileSource.cs](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/TiandituTileSource.cs)

---

## 12. Conclusion

`MapView` should be treated as a dedicated WPF map component with a separate usage guide, rather than being mixed into the general quick-start documentation. It is especially valuable for enterprise desktop applications that need:

- stability
- WPF-native rendering
- custom business templates
- older system compatibility
- fine control over map behavior and visuals
