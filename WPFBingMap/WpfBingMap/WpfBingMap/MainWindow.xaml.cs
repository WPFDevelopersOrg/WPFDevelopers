/**
 * 作者闫驚鏵
 * https://github.com/yanjinhuagood/WPFBingMap.git
 * **/

using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

/*
 * 
 * 39.0654365763652,115.513103745601,0
38.5861378332358,114.897869370601,0
38.0690298850334,114.238689683101,0
37.4436424646135,113.491619370601,0
36.8833163124675,112.832439683101,0
36.6015984304246,112.480877183101,0
36.2125510101126,112.041424058101,0

35.6074752751952,111.426189683101,0

34.9977887035825,110.591228745601,0
34.456028305434,109.932049058101,0
33.9836399832877,109.580486558101,0
33.5086116028286,108.965252183101,0
33.1046158275268,108.525799058101,0

32.6617655474571,108.042400620601,0
32.179523137361,107.515056870601,0

 * **/

namespace WpfBingMap
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private LocationCollection _polyLocations;
        private MapPolyline mapPolyline;
        private Pushpin carPushpin;
        private DispatcherTimer dispatcherTimer;
        private List<Location> locations;

        public IEnumerable PushpinArray
        {
            get { return (IEnumerable)GetValue(PushpinArrayProperty); }
            set { SetValue(PushpinArrayProperty, value); }
        }

        public static readonly DependencyProperty PushpinArrayProperty =
            DependencyProperty.Register("PushpinArray", typeof(IEnumerable), typeof(MainWindow), new PropertyMetadata(null));


        public MainWindow()
        {
            InitializeComponent();
            //this.map.Mode = new MercatorMode();
            //this.map.Children.Add(new AMapTitleLayer());
            //this.map.MouseDown += Map_MouseDown;
            var pushpins = new List<PushpinModel>();
            pushpins.Add(new PushpinModel { ID=1, Location = new Location(39.8151940395589, 116.411970893135),Title="和义东里社区" });
            pushpins.Add(new PushpinModel { ID = 2, Location = new Location(39.9094878843105, 116.33299936282) ,Title="中国水科院南小区"});
            pushpins.Add(new PushpinModel { ID = 3, Location = new Location(39.9219204792284, 116.203500574855),Title="石景山山姆会员超市" });
            pushpins.Add(new PushpinModel { ID = 4, Location = new Location(39.9081417418219, 116.331244439925), Title = "茂林居小区" });
            PushpinArray = pushpins;

             _polyLocations = new LocationCollection();
            //_polyLocations.Add(new Location(39.9082973053021, 116.63105019548));
            //_polyLocations.Add(new Location(39.9155572462212, 116.192505993178));
            //_polyLocations.Add(new Location(39.8065773542251, 116.276113341099));
            _polyLocations.Add(new Location(39.9082973053021, 116.63105019548));
            _polyLocations.Add(new Location(31.9121578992881, 107.233555852083));

            mapPolyline = new MapPolyline 
            {
                Stroke = Brushes.Green,
                StrokeThickness = 2,
                Locations = _polyLocations,
            };
            CarLayer.Children.Add(mapPolyline);

            carPushpin = new Pushpin
            {
                Template = this.Resources["CarTemplate"] as ControlTemplate,
                //Location = new Location(39.9082973053021, 116.63105019548),//_polyLocations[0],
                Location = new Location(31.9121578992881, 107.233555852083),
                PositionOrigin = PositionOrigin.Center,
            };
           
            CarLayer.Children.Add(carPushpin);

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1.5);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            
        }
        int index = 0;
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            //carPushpin.Location = carPushpin.Location
            //var currtLocation = carPushpin.Location;
            //if (currtLocation == _polyLocations[1])
            //{
            //    dispatcherTimer.Stop();
            //    return;
            //}

            //currtLocation.Longitude = currtLocation.Longitude - 0.233555852083;
            //currtLocation.Latitude = currtLocation.Latitude - 0.9121578992881;
            //map.Center = currtLocation;
            //map.ZoomLevel = 16;
            if (index < 0)
            {
                index = locations.Count - 1;
                dispatcherTimer.Stop();
                return; 
            }
            carPushpin.Location = locations[index];
            index--;
            //map.Center = carPushpin.Location;
        }

        private void BtnCar_Click(object sender, RoutedEventArgs e)
        {
            locations = new List<Location>();
            locations.Add(new Location(39.9082973053021, 116.63105019548));
            locations.Add(new Location(39.0654365763652, 115.513103745601));
            locations.Add(new Location(38.5861378332358, 114.897869370601));
            locations.Add(new Location(38.0690298850334, 114.238689683101));
            locations.Add(new Location(37.4436424646135, 113.491619370601));
            locations.Add(new Location(36.8833163124675, 112.832439683101));
            locations.Add(new Location(36.6015984304246, 112.480877183101));
            locations.Add(new Location(36.2125510101126, 112.041424058101));
            locations.Add(new Location(35.6074752751952, 111.426189683101));
            locations.Add(new Location(34.9977887035825, 110.591228745601));
            locations.Add(new Location(34.456028305434, 109.932049058101));
            locations.Add(new Location(33.9836399832877, 109.580486558101));
            locations.Add(new Location(33.5086116028286, 108.965252183101));
            locations.Add(new Location(33.1046158275268, 108.525799058101));
            locations.Add(new Location(32.6617655474571, 108.042400620601));
            locations.Add(new Location(32.179523137361, 107.515056870601));
            locations.Add(new Location(31.9121578992881, 107.233555852083));
            index = locations.Count - 1;
            dispatcherTimer.Start();
            //var tt = Enumerable.Range((int)_polyLocations[1].Latitude, (int)_polyLocations[0].Latitude).ToList();
            //for (int i = 0; i < tt.Count(); i++)
            //{
            //    Console.WriteLine(tt[i]);
            //}

        }
        private void Map_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);
            Location pinLocation = this.map.ViewportPointToLocation(mousePosition);
            
            Console.WriteLine(pinLocation);

        }

        private void Pushpin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var model = sender as Pushpin;
            map.Center = model.Location;
            map.ZoomLevel = 16;
        }

        private void PART_Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            var model = PushpinArray.OfType<PushpinModel>().FirstOrDefault(x=>x.ID.Equals(grid.Tag));
            map.Center = model.Location;
            map.ZoomLevel = 16;
        }

       
    }
    public class PushpinModel
    {
        public Location Location { get; set; }
        public int ID { get; set; }
        public string Title { get; set; }
    }
    //public class OpenstreetmapTileSource: TileSource
    //{
    //    public override Uri GetUri(int x, int y, int zoomLevel)
    //    {
    //        //var url = string.Format("https://www.openstreetmap.org/#map={z}/{x}/{y}",zoomLevel,x,y);
    //        //return new Uri(url, UriKind.Absolute);
    //        var uri= new Uri(UriFormat.
    //                   Replace("{x}", x.ToString()).
    //                   Replace("{y}", y.ToString()).
    //                   Replace("{z}", zoomLevel.ToString()));
    //        Console.WriteLine(uri);
    //        return uri;
    //    }
    //}
    //public class OpenstreetmapTileLayer : MapTileLayer 
    //{
    //    public OpenstreetmapTileLayer()
    //    {
    //        TileSource = new OpenstreetmapTileSource();
    //    }

    //    public string UriFormat
    //    {
    //        get { return TileSource.UriFormat; }
    //        set { TileSource.UriFormat = value; }
    //    }
    //}
    public class AMapTitleLayer : MapTileLayer
    {
        public AMapTitleLayer()
        {
            TileSource = new AMapTileSource();
        }

        public string UriFormat
        {
            get { return TileSource.UriFormat; }
            set { TileSource.UriFormat = value; }
        }
    }
    public class AMapTileSource: TileSource
    {
        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            //string url = "https://webrd01.is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x=" + x + "&y=" + y + "&z=" + zoomLevel;
            //https://www.amap.com/service/regeo?longitude=115.975052&latitude=39.778747
            //string url = "https://wprd01.is.autonavi.com/appmaptile?x=" + x + "&y=" + y + "&z=" + zoomLevel + "&lang=zh_cn&size=1&scale=1&style=8";
            string url = string.Format("http://wprd01.is.autonavi.com/appmaptile?x={0}&y={1}&z={2}&lang=zh_cn&size=1&scl=1&style=7",x,y,zoomLevel);
            return new Uri(url, UriKind.Absolute);
        }
    }


}
