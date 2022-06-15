using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    public class Snowflake : Control
    {
        static Snowflake()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Snowflake),
                new FrameworkPropertyMetadata(typeof(Snowflake)));
        }

        public Snowflake()
        {
            CacheMode = new BitmapCache();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}