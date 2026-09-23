using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class MapCluster : ContentControl
    {
        public static readonly DependencyProperty CountTextProperty =
            DependencyProperty.Register(nameof(CountText), typeof(string), typeof(MapCluster),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ClusterFillProperty =
            DependencyProperty.Register(nameof(ClusterFill), typeof(Brush), typeof(MapCluster),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ClusterTextBrushProperty =
            DependencyProperty.Register(nameof(ClusterTextBrush), typeof(Brush), typeof(MapCluster),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        static MapCluster()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MapCluster), new FrameworkPropertyMetadata(typeof(MapCluster)));
        }

        public MapCluster()
        {
            IsHitTestVisible = false;
            Focusable = false;
        }

        public string CountText
        {
            get { return (string)GetValue(CountTextProperty); }
            set { SetValue(CountTextProperty, value); }
        }

        public Brush ClusterFill
        {
            get { return (Brush)GetValue(ClusterFillProperty); }
            set { SetValue(ClusterFillProperty, value); }
        }

        public Brush ClusterTextBrush
        {
            get { return (Brush)GetValue(ClusterTextBrushProperty); }
            set { SetValue(ClusterTextBrushProperty, value); }
        }
    }
}
