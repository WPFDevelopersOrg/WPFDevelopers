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

```xml
<wd:Pushpin
    Title="Beijing"
    Location="39.9132801985722,116.392009995601"
    AnchorPoint="0.5,1" />
```

```csharp
var pushpin = new Pushpin
{
    Title = "Beijing",
    Latitude = 39.9132801985722,
    Longitude = 116.392009995601,
    AnchorPoint = new Point(0.5, 1)
};
```

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

---

## 5. Layers

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
    ClusterRadius = 35,
    ClusterMinimumCount = 2
};
```

Common clustering properties include:

- `EnableClustering`
- `ClusterRadius`
- `ClusterMinimumCount`
- `ClusterTemplate`

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
```

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
layer.ClusterTemplate = (DataTemplate)Application.Current.FindResource("ClusterTemplate");
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
