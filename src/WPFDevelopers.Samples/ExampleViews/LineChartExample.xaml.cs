using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// LineChartExample.xaml 的交互逻辑
    /// </summary>
    public partial class LineChartExample : UserControl
    {
        double maxYMarginTop = 20D;//Y轴上方的空白间距
        double yMarginLeft = 30D;//Y轴左侧，用来显示刻度文字的空白间距

        double yNum = 10D;//Y轴的刻度个数
        int yNumInt = 10;//Y轴的刻度个数

        double minY = 10D;//Y轴最小值
        double maxY = 110D;//Y轴最大值
        double yInterval = 10D;//Y轴刻度

        double xMarginBottom = 20D;//X轴下侧，用来显示刻度文字的空白间距

        double xNum = 25D;//x轴的刻度个数
        int xNumInt = 25;//x轴的刻度个数
        double xIntervalLineHeight = 10D;//X轴刻度线的高度
        DateTime minX = Convert.ToDateTime("2021-07-14 00:00:00");//X轴最小值
        DateTime maxX = Convert.ToDateTime("2021-07-15 00:00:00");//X轴最大值
        public LineChartExample()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            gArea.Children.Clear();

            //X轴刻度值，因为要留出左右空白，所以画的时候多画一格，实际计算减1
            double timeInterval = (maxX.ToOADate() - minX.ToOADate()) / (xNum - 1);

            //折线数据
            Dictionary<DateTime, int> csource = new Dictionary<DateTime, int>();
            csource.Add(Convert.ToDateTime("2021-07-14 09:00:00"), 23);
            csource.Add(Convert.ToDateTime("2021-07-14 10:00:00"), 46);
            csource.Add(Convert.ToDateTime("2021-07-14 11:00:00"), 18);
            csource.Add(Convert.ToDateTime("2021-07-14 12:00:00"), 88);
            csource.Add(Convert.ToDateTime("2021-07-14 13:00:00"), 34);
            csource.Add(Convert.ToDateTime("2021-07-14 14:00:00"), 45);
            csource.Add(Convert.ToDateTime("2021-07-14 15:00:00"), 61);
            csource.Add(Convert.ToDateTime("2021-07-14 16:00:00"), 33);
            csource.Add(Convert.ToDateTime("2021-07-14 17:00:00"), 78);
            csource.Add(Convert.ToDateTime("2021-07-14 18:00:00"), 90);
            csource.Add(Convert.ToDateTime("2021-07-14 19:00:00"), 18);
            csource.Add(Convert.ToDateTime("2021-07-14 20:00:00"), 50);

            //折线数据
            Dictionary<DateTime, int> bsource = new Dictionary<DateTime, int>();
            bsource.Add(Convert.ToDateTime("2021-07-14 09:00:00"), 66);
            bsource.Add(Convert.ToDateTime("2021-07-14 10:00:00"), 56);
            bsource.Add(Convert.ToDateTime("2021-07-14 11:00:00"), 87);
            bsource.Add(Convert.ToDateTime("2021-07-14 12:00:00"), 33);
            bsource.Add(Convert.ToDateTime("2021-07-14 13:00:00"), 54);
            bsource.Add(Convert.ToDateTime("2021-07-14 14:00:00"), 27);
            bsource.Add(Convert.ToDateTime("2021-07-14 15:00:00"), 80);
            bsource.Add(Convert.ToDateTime("2021-07-14 16:00:00"), 23);
            bsource.Add(Convert.ToDateTime("2021-07-14 17:00:00"), 43);
            bsource.Add(Convert.ToDateTime("2021-07-14 18:00:00"), 77);
            bsource.Add(Convert.ToDateTime("2021-07-14 19:00:00"), 56);
            bsource.Add(Convert.ToDateTime("2021-07-14 20:00:00"), 43);


            //画X轴
            Rectangle rx = new Rectangle();
            rx.Height = 1;
            rx.Stroke = new SolidColorBrush(Color.FromRgb(128, 128, 142));
            rx.HorizontalAlignment = HorizontalAlignment.Stretch;
            rx.VerticalAlignment = VerticalAlignment.Bottom;
            gArea.Children.Add(rx);

            //画Y轴
            Rectangle ry = new Rectangle();
            ry.Width = 1;
            ry.Stroke = new SolidColorBrush(Color.FromRgb(128, 128, 142));
            ry.HorizontalAlignment = HorizontalAlignment.Left;
            ry.VerticalAlignment = VerticalAlignment.Stretch;
            gArea.Children.Add(ry);

            //画Y轴横虚线
            for (int i = 0; i < yNumInt; i++)
            {
                Line ly = new Line();
                ly.X1 = 0;
                ly.Y1 = ((gArea.ActualHeight - maxYMarginTop) / yNum * i) + maxYMarginTop;
                ly.X2 = gArea.ActualWidth;
                ly.Y2 = ((gArea.ActualHeight - maxYMarginTop) / yNum * i) + maxYMarginTop;
                ly.Stroke = new SolidColorBrush(Color.FromRgb(73, 73, 91));
                ly.StrokeThickness = 1;
                ly.StrokeDashArray = new DoubleCollection() { 10, 15 };//实线的长度和间距

                TextBlock tb = new TextBlock();
                tb.FontSize = 12;
                tb.Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 142));
                tb.Text = (maxY - (i * yInterval)).ToString();
                tb.VerticalAlignment = VerticalAlignment.Top;
                tb.HorizontalAlignment = HorizontalAlignment.Left;
                tb.Margin = new Thickness(-yMarginLeft, (((gArea.ActualHeight - maxYMarginTop) / yNum * i) + maxYMarginTop) - (GetFontSizeByTextBlock(tb).Height / 2D), 0, 0);
                gArea.Children.Add(tb);
                gArea.Children.Add(ly);
            }

            //画X轴刻度线
            for (int i = 0; i < xNumInt; i++)
            {
                Line ly = new Line();
                ly.X1 = (gArea.ActualWidth / xNum * i) + ((gArea.ActualWidth / xNum) / 2D);
                ly.Y1 = gArea.ActualHeight;
                ly.X2 = (gArea.ActualWidth / xNum * i) + ((gArea.ActualWidth / xNum) / 2D);
                ly.Y2 = gArea.ActualHeight - xIntervalLineHeight;
                ly.Stroke = new SolidColorBrush(Color.FromRgb(128, 128, 142));
                ly.StrokeThickness = 1;

                if (i % 2 == 0)//两个刻度显示一次文字
                {
                    TextBlock tb = new TextBlock();
                    tb.FontSize = 12;
                    tb.Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 142));
                    if (i < 10)//个位数补零
                    {
                        tb.Text = "0" + i.ToString() + ":00";
                    }
                    else
                    {
                        tb.Text = i.ToString() + ":00";
                    }
                    tb.VerticalAlignment = VerticalAlignment.Bottom;
                    tb.HorizontalAlignment = HorizontalAlignment.Left;
                    tb.Margin = new Thickness((gArea.ActualWidth / xNum * i) + ((gArea.ActualWidth / xNum) / 2D) - (GetFontSizeByTextBlock(tb).Width / 2D), 0, 0, -xMarginBottom);
                    gArea.Children.Add(tb);
                }

                gArea.Children.Add(ly);
            }

            Polyline plView = new Polyline();
            plView.Stroke = new SolidColorBrush(Color.FromRgb(66, 120, 255));
            plView.StrokeThickness = 1;
            plView.StrokeLineJoin = PenLineJoin.Round;
            plView.IsHitTestVisible = false;
            foreach (DateTime k in csource.Keys)
            {
                Point p = new Point();
                p.X = (gArea.ActualWidth / xNum * ((k.ToOADate() - minX.ToOADate()) / timeInterval)) + ((gArea.ActualWidth / xNum) / 2D);
                p.Y = ((gArea.ActualHeight - maxYMarginTop) / (maxY - minY) * (maxY - csource[k])) + maxYMarginTop;
                plView.Points.Add(p);

                Ellipse ep = new Ellipse();
                ep.Width = 10;
                ep.Height = 10;
                ep.Stroke = new SolidColorBrush(Color.FromRgb(230, 230, 236));
                ep.StrokeThickness = 0;
                ep.Fill = new SolidColorBrush(Color.FromRgb(66, 120, 255));
                ep.HorizontalAlignment = HorizontalAlignment.Left;
                ep.VerticalAlignment = VerticalAlignment.Top;
                ep.Margin = new Thickness(p.X - 5, p.Y - 5, 0, -5);//底部设置-5(圆球宽高的一半)，当Y等于0的时候，可以让圆球全部显示
                ep.MouseEnter += Ep_MouseEnter;
                ep.MouseLeave += Ep_MouseLeave;
                ep.MouseLeftButtonDown += Ep_MouseLeftButtonDown;
                ep.Tag = k + "|" + csource[k];
                gArea.Children.Add(ep);
            }
            gArea.Children.Add(plView);


            Polyline plPeople = new Polyline();
            plPeople.Stroke = new SolidColorBrush(Color.FromRgb(27, 221, 58));
            plPeople.StrokeThickness = 1;
            plPeople.StrokeLineJoin = PenLineJoin.Round;
            plPeople.IsHitTestVisible = false;
            foreach (DateTime k in bsource.Keys)
            {
                Point p = new Point();
                p.X = (gArea.ActualWidth / xNum * ((k.ToOADate() - minX.ToOADate()) / timeInterval)) + ((gArea.ActualWidth / xNum) / 2D);
                p.Y = ((gArea.ActualHeight - maxYMarginTop) / (maxY - minY) * (maxY - bsource[k])) + maxYMarginTop;
                plPeople.Points.Add(p);

                Ellipse ep = new Ellipse();
                ep.Width = 10;
                ep.Height = 10;
                ep.Stroke = new SolidColorBrush(Color.FromRgb(230, 230, 236));
                ep.StrokeThickness = 0;
                ep.Fill = new SolidColorBrush(Color.FromRgb(27, 221, 58));
                ep.HorizontalAlignment = HorizontalAlignment.Left;
                ep.VerticalAlignment = VerticalAlignment.Top;
                ep.Margin = new Thickness(p.X - 5, p.Y - 5, 0, -5);//底部设置-5(圆球宽高的一半)，当Y等于0的时候，可以让圆球全部显示
                ep.MouseEnter += Ep_MouseEnter;
                ep.MouseLeave += Ep_MouseLeave;
                ep.MouseLeftButtonDown += Ep_MouseLeftButtonDown;
                ep.Tag = k + "|" + bsource[k];
                gArea.Children.Add(ep);
            }
            gArea.Children.Add(plPeople);
        }
        private void Ep_MouseLeave(object sender, MouseEventArgs e)
        {
            Ellipse ep = sender as Ellipse;
            ep.StrokeThickness = 0;
        }

        private void Ep_MouseEnter(object sender, MouseEventArgs e)
        {
            Ellipse ep = sender as Ellipse;
            ep.StrokeThickness = 2;
        }
        private void Ep_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse ep = sender as Ellipse;
            string[] ts = ep.Tag.ToString().Split('|');

            pop1.PlacementTarget = ep;
            pop1.VerticalOffset = -46;
            pop1.HorizontalOffset = -26;
            tbT.Text = "时间：" + ts[0];
            tbV.Text = "数量：" + ts[1];
            pop1.IsOpen = true;
        }

        //测量文本框的尺寸
        public static Size GetFontSizeByTextBlock(TextBlock tb)
        {
            var formattedText = new FormattedText(
               tb.Text,
               CultureInfo.CurrentUICulture,
               FlowDirection.LeftToRight,
               new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
               tb.FontSize,
               tb.Foreground
               );
            Size size = new Size(formattedText.Width, formattedText.Height);
            return size;
        }
        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (pop1.IsOpen)
            {
                pop1.IsOpen = false;
            }
        }

    }
}
