# MapView 使用指南

> 这是 WPFDevelopers 中地图控件的使用说明，重点放在「如何使用」「如何配置底图」「如何绑定业务对象」以及「常见示例」上。
> 这份文档是实际使用手册，不是背景说明文档；如果需要了解为什么不依赖 Bing Map，以及为什么要自行封装 MapView，请参考项目背景说明。

## 1. 简介

`wd:MapView` 是一个面向 WPF 桌面端的地图控件，适合在企业应用中展示：

- 车辆轨迹
- 资源分布
- 区域边界
- 站点/仓库/设备位置
- 地理监控等场景

它的设计目标不是“嵌入一个第三方地图页面”，而是让地图能力以控件的方式融入 WPF 界面：

- 底图来自瓦片服务
- 点、线、面通过图层渲染
- 支持聚合、点击、选中、动画
- 可直接使用 WPF 的模板与样式系统

这使得它更适合传统桌面应用场景，也更适合 Win7 等老版本系统环境。

---

## 2. 适用场景

MapView 适合以下场景：

- 物流调度和车辆监控
- GIS 数据可视化
- 区域分布地图
- 工地、仓库、设备地图
- 需要和 WPF 业务逻辑深度集成的场景

如果你的客户环境中还有 Win7、老版 Windows 或需要稳定桌面部署，MapView 方案通常比直接依赖 WebView 的地图方案更稳妥。

---

## 3. 基本使用方式

### 3.1 在 XAML 中使用

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

### 3.2 设置底图

MapView 的底图通过 `TileSource` 设置：

```csharp
mapView.TileSource = new TiandituTileSource("vec_w", 0, "your-tianditu-key");
mapView.AnnotationTileSource = new TiandituTileSource("cva_w", 0, "your-tianditu-key");
```

常见底图源：

- `TiandituTileSource`
- `AMapTileSource`
- `TencentTileSource`
- `OSMTileSource`
- 自定义 `TileSource`

> 如果 key 为空或 layer 为空，MapView 应该避免继续发起无效请求，这一点非常重要。

---

## 4. 地图数据结构

### 4.1 位置对象

地图上的位置通常使用经纬度：

```csharp
var location = new MapLocation
{
    Latitude = 39.9132801985722,
    Longitude = 116.392009995601
};
```

#### 4.1.1 坐标系与转换

MapView 处理的是地理经纬度数据，但真实业务中，来自不同来源的坐标并不一定是同一种坐标系。常见情况有：

- `WGS84`：GPS、手持设备原始坐标
- `GCJ02`：高德、腾讯常见国内坐标
- `BD09`：百度坐标
- `Web Mercator`：瓦片地图和底图投影坐标
- `Tile X/Y/Z`：瓦片请求坐标

如果你的业务数据来自不同厂商或设备，需要先明确来源，再做统一转换。为了避免在 MapView 层混入厂商 SDK 细节，项目中新增了统一地图转换工具类：

- [src/WPFDevelopers.Shared/Controls/MapView/MapCoordinateType.cs](../../src/WPFDevelopers.Shared/Controls/MapView/MapCoordinateType.cs)
- [src/WPFDevelopers.Shared/Controls/MapView/MapGeoPoint.cs](../../src/WPFDevelopers.Shared/Controls/MapView/MapGeoPoint.cs)
- [src/WPFDevelopers.Shared/Controls/MapView/MapCoordinateHelper.cs](../../src/WPFDevelopers.Shared/Controls/MapView/MapCoordinateHelper.cs)
- [src/WPFDevelopers.Shared/Controls/MapView/MapProjectionHelper.cs](../../src/WPFDevelopers.Shared/Controls/MapView/MapProjectionHelper.cs)

典型转换链路是：

```csharp
var gps = new MapGeoPoint(116.397428, 39.90923, MapCoordinateType.Wgs84);
var gcj = MapCoordinateHelper.Convert(gps, MapCoordinateType.Gcj02);
var tile = MapProjectionHelper.LonLatToTileXY(gcj.Longitude, gcj.Latitude, 18);
```

这套做法的意义在于：

- 手持 GPS 数据保持原始语义；
- 高德/腾讯/百度坐标可以统一转换；
- MapView 最终只关心地理坐标或投影坐标，不需要关心厂商 SDK 细节；
- 对天地图等瓦片底图场景，能够做 Web Mercator 与 tile xyz 转换。

> 如果坐标来源不明确，最好在协议或数据库中保留 `CoordinateType`，不要只靠数值猜测坐标系。

### 4.2 标注点

最简单的方式是直接使用 `Pushpin`：

```xml
<wd:Pushpin
    Title="北京"
    Location="39.9132801985722,116.392009995601"
    AnchorPoint="0.5,1" />
```

对应 C#：

```csharp
var pushpin = new Pushpin
{
    Title = "北京",
    Latitude = 39.9132801985722,
    Longitude = 116.392009995601,
    AnchorPoint = new Point(0.5, 1)
};
```

### 4.3 线和面

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

```xml
<wd:MapView.Polygons>
    <wd:MapPolygon Fill="#553B82F6" Stroke="#1D4ED8" StrokeThickness="2">
        <wd:MapPolygon.Points>
            <wd:MapLocation Latitude="34.960425" Longitude="107.923137" />
            <wd:MapLocation Latitude="34.703700" Longitude="110.245209" />
            <wd:MapLocation Latitude="31.832828" Longitude="109.496231" />
            <wd:MapLocation Latitude="32.730635" Longitude="106.387506" />
        </wd:MapPolygon.Points>
    </wd:MapPolygon>
</wd:MapView.Polygons>
```

---

## 5. 图层使用方式

MapView 支持多个图层，业务上通常分成以下几类：

- 底图层
- 标注层
- 线层
- 面层
- 聚合层
- 自定义覆盖层

### 5.1 标注图层

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

### 5.2 聚合

很多场景下，地图上有大量点，最好启用聚合：

```csharp
var layer = new MapPushpinLayer
{
    Name = "ClusterLayer",
    EnableClustering = true,
    ClusterRadius = 35,
    ClusterMinimumCount = 2
};
```

相关配置常见包括：

- `EnableClustering`
- `ClusterRadius`
- `ClusterMinimumCount`
- `ClusterTemplate`

如果没有自定义聚合模板，可以提供默认样式兜底。

---

## 6. 自定义模板

MapView 的一个突出优势，是可以直接复用 WPF 模板系统。

### 6.1 为 Pushpin 自定义模板

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

### 6.2 自定义聚合模板

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

这让你可以用业务数据直接决定聚合节点样式，而不必绑定死在默认渲染。

---

## 7. 车辆轨迹示例

MapView 的车辆路径演示是一个很典型的使用例子，尤其适合：

- 跟踪实时车位；
- 车辆沿路线移动；
- 轨迹回放；
- 车头方向跟随路线角度。

示例代码在：

- [src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml)
- [src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml.cs](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml.cs)

它的关键点包括：

- 使用自定义车图标模板；
- 把车辆放到单独的 `MapPushpinLayer`；
- 关闭聚合，以避免车辆被合并；
- 根据路线上两点夹角计算车头方向；
- 使用 `Tag` 承载角度值，结合 `RotateTransform` 完成方向显示。

### 车头方向示例

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

这样车辆可以沿路线正确朝向前进方向。

---

## 8. 天地图接入说明

如果你使用天地图瓦片服务，需要先申请 key，并将 key 放在配置中，而不是直接写死在代码中。

申请地址：

https://oauth.tianditu.gov.cn/login?service=https%3A%2F%2Fcloudcenter.tianditu.gov.cn%2Fapi%2Fsvc%2Flogin%3Furl%3Dhttps%25253A%25252F%25252Fcloudcenter.tianditu.gov.cn%25252Fcenter%25252Fdevelopment%25252FmyApp

推荐做法：

```csharp
var tiandituKey = ConfigurationManager.AppSettings["TiandituKey"] ?? string.Empty;
mapView.TileSource = new TiandituTileSource("vec_w", 0, tiandituKey);
```

### 关键建议

- `layer` 为空时不要请求；
- `token` 为空时不要请求；
- 请求失败时不应该触发全局异常；
- 允许在配置中心动态修改底图参数。

---

## 9. 常见问题

### 9.1 地图不显示

常见原因：

- 底图 key/ token 没配置；
- URL 参数不正确；
- `TileSource` 没绑定；
- 网络不可用；
- 地理坐标超出正常范围。

### 9.2 点没有出现在正确位置

常见原因：

- 纬度/经度写反；
- `AnchorPoint` 不正确；
- `Location` 绑定值错误；
- 坐标单位错误。

### 9.3 聚合后点看起来异常

常见原因：

- `EnableClustering` 没打开；
- `ClusterRadius` 设置过小或过大；
- 模板没有兜底样式；
- 车辆等不该聚合的对象放在了同一层。

### 9.4 车头方向不对

常见原因：

- 初始角度计算不对；
- 图片本身的前向方向和想象中的方向不一致；
- `AnchorPoint` 位置不在中心；
- 需要在车图标模板中通过 `RotateTransform` 做微调。

---

## 10. 实战建议

如果你在做客户项目，建议采用以下方式：

1. 底图和业务图层分离；
2. 把车辆、站点、区域分开图层；
3. 不该聚合的对象单独层处理；
4. 重要配置放到配置文件中；
5. 对空 key / 空 layer 做保护；
6. 旋转、模板和标注锚点统一测试；
7. 在地图业务中优先使用强类型数据模型，而不是直接拼接字符串。

---

## 11. 参考示例

本仓库中已经有比较完整的示例：

- [src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml)
- [src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml.cs](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewExample.xaml.cs)
- [src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewFeaturesWindow.xaml](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/MapViewFeaturesWindow.xaml)
- [src/WPFDevelopers.Samples.Shared/ExampleViews/Map/TiandituTileSource.cs](../../src/WPFDevelopers.Samples.Shared/ExampleViews/Map/TiandituTileSource.cs)

这些示例可以作为快速上手和业务对接参考。

---

## 12. 结论

`MapView` 不是一个“简单地图控件”，而是一个面向桌面端业务场景的地图可视化组件。它在 WPF 中的价值在于：

- 与 WPF UI 深度融合；
- 适合企业桌面应用；
- 更容易做业务定制；
- 在老旧系统上更容易稳定部署；
- 支持自定义模板、聚合和动画。

因此，MapView 应该被当作一个独立的使用指南来维护，而不是塞进通用的快速开始文档里。

---

如果你需要进一步扩展，可以继续补充：

- `MapView` API 详细说明
- `MapPushpinLayer` 使用详解
- 聚合模板示例
- 轨迹回放示例
- 天地图接入最佳实践
