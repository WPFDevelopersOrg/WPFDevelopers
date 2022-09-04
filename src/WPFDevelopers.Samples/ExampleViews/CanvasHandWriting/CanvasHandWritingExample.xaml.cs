using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFDevelopers.Samples.ExampleViews.CanvasHandWriting
{
    /// <summary>
    ///     CanvasHandWritingExample.xaml 的交互逻辑
    /// </summary>
    public partial class CanvasHandWritingExample : UserControl
    {
        public static readonly DependencyProperty TensionProperty =
            DependencyProperty.Register("Tension", typeof(double), typeof(CanvasHandWritingExample),
                new PropertyMetadata(0.618));

        public static readonly DependencyProperty SmoothSamplingProperty =
            DependencyProperty.Register("SmoothSampling", typeof(double), typeof(CanvasHandWritingExample),
                new UIPropertyMetadata(OnSmoothSamplingChanged));

        public static readonly DependencyProperty IsEraserProperty =
            DependencyProperty.Register("IsEraser", typeof(bool), typeof(CanvasHandWritingExample),
                new PropertyMetadata(false));

        private readonly Dictionary<Path, List<Vector2D>> _PathVector2DDictionary ;

        volatile bool _IsStart = false;
        Path _DrawingPath = default;

        private static void OnSmoothSamplingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mWindow = (CanvasHandWritingExample)d;
            foreach (var item in mWindow._PathVector2DDictionary.Keys)
            {
                mWindow.DrawLine(item);
            }
        }
        public CanvasHandWritingExample()
        {
            InitializeComponent();
            _PathVector2DDictionary = new Dictionary<Path, List<Vector2D>>();
            SmoothSampling = 0.8;
        }

        private void DrawingCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _IsStart = true;
            _DrawingPath = new Path()
            {
                StrokeDashCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
            };

            if (IsEraser)
            {
                _DrawingPath.Stroke = new SolidColorBrush(Colors.Black);
                _DrawingPath.StrokeThickness = 40;
            }
            else
            {
                var random = new Random();
                var strokeBrush = new SolidColorBrush(Color.FromRgb((byte)random.Next(200, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)));
                _DrawingPath.Stroke = strokeBrush;
                _DrawingPath.StrokeThickness = 10;
            }
            _PathVector2DDictionary.Add(_DrawingPath, new List<Vector2D>());
            drawingCanvas.Children.Add(_DrawingPath);
        }

        private void DrawingCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _IsStart = false;
            _DrawingPath = default;
        }

        private void DrawingCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_IsStart)
                return;

            if (_DrawingPath is null)
                return;

            Vector2D currenPoint = e.GetPosition(drawingCanvas);
            if (currenPoint.X < 0 || currenPoint.Y < 0)
                return;

            if (currenPoint.X > drawingCanvas.ActualWidth || currenPoint.Y > drawingCanvas.ActualHeight)
                return;

            if (_PathVector2DDictionary[_DrawingPath].Count > 0)
            {
                if (Vector2D.CalculateVectorDistance(currenPoint, _PathVector2DDictionary[_DrawingPath][_PathVector2DDictionary[_DrawingPath].Count - 1]) > 1)
                    _PathVector2DDictionary[_DrawingPath].Add(e.GetPosition(drawingCanvas));
            }
            else
                _PathVector2DDictionary[_DrawingPath].Add(e.GetPosition(drawingCanvas));

            DrawLine(_DrawingPath);
        }


        public double Tension
        {
            get => (double)GetValue(TensionProperty);
            set => SetValue(TensionProperty, value);
        }

        public double SmoothSampling
        {
            get => (double)GetValue(SmoothSamplingProperty);
            set => SetValue(SmoothSamplingProperty, value);
        }

        public bool IsEraser
        {
            get => (bool)GetValue(IsEraserProperty);
            set => SetValue(IsEraserProperty, value);
        }

       

        private void DrawLine(Path path)
        {
            if (_PathVector2DDictionary[path].Count > 2)
            {
                var pathVector2Ds = _PathVector2DDictionary[path];
                var smoothNum = (int)(_PathVector2DDictionary[path].Count * SmoothSampling);
                if (smoothNum > 1)
                    pathVector2Ds = ComputingHelper.AverageSampling(_PathVector2DDictionary[path], smoothNum);
                var lineB = new LineB(pathVector2Ds, false);
                lineB.Tension = Tension;

                path.Data = lineB;
            }
        }

        private void btnClertCanvas_Click(object sender, RoutedEventArgs e)
        {
            drawingCanvas.Children.Clear();
            _PathVector2DDictionary.Clear();
        }
    }
}