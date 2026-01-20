using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace WPFDevelopers.Controls
{
    public class SvgViewer : Control
    {
        private DrawingBrush _svgBrush;
        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(string), typeof(SvgViewer),
                new PropertyMetadata(null, OnSvgSourceChanged));

        private static void OnSvgSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SvgViewer canvas)
            {
                canvas.LoadAndRenderSvg();
            }
        }

        private Brush ParseBrush(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "none")
                return null;

            try
            {
                if (value.StartsWith("#"))
                {
                    var hex = value.Substring(1);
                    if (hex.Length == 3)
                    {
                        hex = new string(hex[0], 2) + new string(hex[1], 2) + new string(hex[2], 2);
                    }

                    if (hex.Length == 6)
                    {
                        byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                        byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                        byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                        return new SolidColorBrush(Color.FromRgb(r, g, b));
                    }

                    if (hex.Length == 8)
                    {
                        byte a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                        byte r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                        byte g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                        byte b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
                        return new SolidColorBrush(Color.FromArgb(a, r, g, b));
                    }
                }
                else
                {
                    var converted = ColorConverter.ConvertFromString(value);
                    if (converted is Color color)
                        return new SolidColorBrush(color);
                }
            }
            catch { }
            return null;
        }
        private Uri GetUriFromSource(string source)
        {
            try
            {
                if (source.StartsWith("http", StringComparison.OrdinalIgnoreCase) ||
                    source.StartsWith("file:", StringComparison.OrdinalIgnoreCase) ||
                    source.StartsWith("pack:", StringComparison.OrdinalIgnoreCase))
                {
                    return new Uri(source, UriKind.Absolute);
                }

                if (Path.IsPathRooted(source) && File.Exists(source))
                {
                    return new Uri(source, UriKind.Absolute);
                }

                return new Uri($"pack://application:,,,/{source.TrimStart('/')}", UriKind.Absolute);
            }
            catch
            {
                return null;
            }
        }

        private Rect GetSvgBounds(XmlElement svgRoot)
        {
            var viewBox = svgRoot.GetAttribute("viewBox");
            if (!string.IsNullOrEmpty(viewBox))
            {
                var parts = viewBox.Split(new[] { ' ', ',', '\t', '\n', '\r' },
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 4 &&
                    double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double x) &&
                    double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double y) &&
                    double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double w) &&
                    double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double h))
                {
                    return new Rect(x, y, w, h);
                }
            }
            double width = 100;
            double height = 100;
            var widthAttr = svgRoot.GetAttribute("width");
            var heightAttr = svgRoot.GetAttribute("height");
            if (!string.IsNullOrEmpty(widthAttr))
                double.TryParse(widthAttr, NumberStyles.Float, CultureInfo.InvariantCulture, out width);

            if (!string.IsNullOrEmpty(heightAttr))
                double.TryParse(heightAttr, NumberStyles.Float, CultureInfo.InvariantCulture, out height);
            return new Rect(0, 0, width, height);
        }

        private DrawingGroup ParseSvgPaths(XmlElement svgRoot, Rect bounds)
        {
            var drawingGroup = new DrawingGroup();
            var pathNodes = svgRoot.GetElementsByTagName("path");

            foreach (XmlNode node in pathNodes)
            {
                var dAttr = node.Attributes?["d"];
                if (dAttr == null || string.IsNullOrWhiteSpace(dAttr.Value))
                    continue;

                Geometry geometry;
                try
                {
                    geometry = Geometry.Parse(dAttr.Value);
                }
                catch
                {
                    continue;
                }

                var fillAttr = node.Attributes?["fill"]?.Value;
                var strokeAttr = node.Attributes?["stroke"]?.Value;
                var strokeWidthAttr = node.Attributes?["stroke-width"]?.Value;
                var lineCapAttr = node.Attributes?["stroke-linecap"]?.Value;
                var lineJoinAttr = node.Attributes?["stroke-linejoin"]?.Value;
                Brush fillBrush = ParseBrush(fillAttr);
                Brush strokeBrush = ParseBrush(strokeAttr);
                if (fillBrush == null && strokeBrush == null && Foreground != null)
                {
                    fillBrush = Foreground;
                }

                Pen pen = null;
                if (strokeBrush != null)
                {
                    double thickness = 1.0;
                    if (!string.IsNullOrWhiteSpace(strokeWidthAttr))
                    {
                        double.TryParse(strokeWidthAttr, NumberStyles.Float,
                            CultureInfo.InvariantCulture, out thickness);
                    }

                    pen = new Pen(strokeBrush, thickness);

                    if (!string.IsNullOrEmpty(lineCapAttr))
                    {
                        if (lineCapAttr == "round")
                        {
                            pen.StartLineCap = PenLineCap.Round;
                            pen.EndLineCap = PenLineCap.Round;
                        }
                        else if (lineCapAttr == "square")
                        {
                            pen.StartLineCap = PenLineCap.Square;
                            pen.EndLineCap = PenLineCap.Square;
                        }
                        else if (lineCapAttr == "butt")
                        {
                            pen.StartLineCap = PenLineCap.Flat;
                            pen.EndLineCap = PenLineCap.Flat;
                        }
                        else
                        {
                            pen.StartLineCap = PenLineCap.Flat;
                            pen.EndLineCap = PenLineCap.Flat;
                        }
                    }

                    if (!string.IsNullOrEmpty(lineJoinAttr))
                    {
                        if (lineJoinAttr == "round")
                        {
                            pen.LineJoin = PenLineJoin.Round;
                        }
                        else if (lineJoinAttr == "bevel")
                        {
                            pen.LineJoin = PenLineJoin.Bevel;
                        }
                        else if (lineJoinAttr == "miter")
                        {
                            pen.LineJoin = PenLineJoin.Miter;
                        }
                        else
                        {
                            pen.LineJoin = PenLineJoin.Miter;
                        }
                    }

                    if (pen.CanFreeze)
                        pen.Freeze();
                }

                if (fillBrush is SolidColorBrush fill && fill.CanFreeze)
                    fill.Freeze();
                if (strokeBrush is SolidColorBrush stroke && stroke.CanFreeze)
                    stroke.Freeze();
                if (geometry.CanFreeze)
                    geometry.Freeze();

                var drawing = new GeometryDrawing(fillBrush, pen, geometry);
                drawingGroup.Children.Add(drawing);
            }
            drawingGroup.Freeze();
            return drawingGroup;
        }

        private void CreateDrawingBrush(DrawingGroup drawingGroup, Rect bounds)
        {
            _svgBrush = new DrawingBrush(drawingGroup)
            {
                Stretch = Stretch.Uniform,
                ViewboxUnits = BrushMappingMode.Absolute,
                Viewbox = bounds,
                ViewportUnits = BrushMappingMode.RelativeToBoundingBox
            };
            if (_svgBrush.CanFreeze)
                _svgBrush.Freeze();
            Background = _svgBrush;
        }

        private void LoadAndRenderSvg()
        {
            if (string.IsNullOrWhiteSpace(Source)) return;
            var extension = Path.GetExtension(Source)?.ToLower();
            if (extension != ".svg")
                return;
            try
            {
                Uri uri = GetUriFromSource(Source);
                if (uri == null)
                    return;
                Stream stream = null;
                if (uri != null)
                {
                    var resource = Application.GetResourceStream(uri);
                    if (resource != null)
                    {
                        stream = resource.Stream;
                    }
                }
                string svgContent = null;

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    svgContent = reader.ReadToEnd();
                }

                if (string.IsNullOrEmpty(svgContent))
                    return;

                svgContent = RemoveDoctype(svgContent);
                var xml = new XmlDocument();
                xml.XmlResolver = null;

                using (var stringReader = new StringReader(svgContent))
                using (var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings
                {
                    XmlResolver = null,
                    DtdProcessing = DtdProcessing.Ignore,
                    IgnoreComments = true,
                    IgnoreWhitespace = true
                }))
                {
                    xml.Load(xmlReader);
                }
                var svgRoot = xml.DocumentElement;
                if (svgRoot == null || svgRoot.Name != "svg")
                    return;

                var bounds = GetSvgBounds(svgRoot);
                var drawingGroup = ParseSvgPaths(svgRoot, bounds);

                if (drawingGroup == null || drawingGroup.Children.Count == 0)
                    return;

                CreateDrawingBrush(drawingGroup, bounds);
            }
            catch
            {
                Background = null;
                throw;
            }
        }

        private string RemoveDoctype(string xmlContent)
        {
            var doctypeStart = xmlContent.IndexOf("<!DOCTYPE", StringComparison.OrdinalIgnoreCase);
            if (doctypeStart >= 0)
            {
                var doctypeEnd = xmlContent.IndexOf(">", doctypeStart);
                if (doctypeEnd > doctypeStart)
                {
                    xmlContent = xmlContent.Remove(doctypeStart, doctypeEnd - doctypeStart + 1);
                }
            }
            return xmlContent;
        }

        static SvgViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SvgViewer), new FrameworkPropertyMetadata(typeof(SvgViewer)));
        }
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (_svgBrush == null)
                return;

            dc.DrawRectangle(
                _svgBrush,
                null,
                new Rect(0, 0, ActualWidth, ActualHeight)
            );
        }

    }
}
