using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFDevelopers.Controls;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Samples.ExampleViews
{
    /// <summary>
    /// UsageColor.xaml 的交互逻辑
    /// </summary>
    public partial class UsageColor : UserControl
    {
        public ObservableCollection<ColorItem> Colors
        {
            get { return (ObservableCollection<ColorItem>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }

        public static readonly DependencyProperty ColorsProperty =
            DependencyProperty.Register(nameof(Colors), typeof(ObservableCollection<ColorItem>), typeof(UsageColor), new PropertyMetadata(new ObservableCollection<ColorItem>()));
        public UsageColor()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += UsageColor_Loaded;
        }

        private void UsageColor_Loaded(object sender, RoutedEventArgs e)
        {
            Colors.Clear();
            Colors.Add(new ColorItem { Name = "Primary", BrushKey = "WD.PrimaryBrush", Brush = (SolidColorBrush)ThemeManager.Instance.PrimaryBrush, MouseOverKey = "WD.PrimaryMouseOverBrush", MouseOverBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.PrimaryMouseOverBrush") });
            Colors.Add(new ColorItem { Name = "Success", BrushKey = "WD.SuccessBrush", Brush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.SuccessBrush"), MouseOverKey = "WD.SuccessMouseOverBrush", MouseOverBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.SuccessMouseOverBrush") });
            Colors.Add(new ColorItem { Name = "Warning", BrushKey = "WD.WarningBrush", Brush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.WarningBrush"), MouseOverKey = "WD.WarningMouseOverBrush", MouseOverBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.WarningMouseOverBrush") });
            Colors.Add(new ColorItem { Name = "Danger", BrushKey = "WD.DangerBrush", Brush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.DangerBrush"), MouseOverKey = "WD.DangerMouseOverBrush", MouseOverBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.DangerMouseOverBrush") });
            var primaryTextBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.PrimaryTextBrush");
            var regularTextBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.RegularTextBrush");
            var placeholderTextBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.PlaceholderTextBrush");
            var baseBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.BaseBrush");
            var lighterBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.LighterBrush");

            Colors.Add(new ColorItem { Name = "Primary Text", BrushKey = "WD.PrimaryTextBrush", IsMouseOver = true, Brush = primaryTextBrush, MouseOverBrush = CreateTransparentMouseOverBrush(primaryTextBrush) });
            Colors.Add(new ColorItem { Name = "Regular Text", BrushKey = "WD.RegularTextBrush", IsMouseOver = true, Brush = regularTextBrush, MouseOverBrush = CreateTransparentMouseOverBrush(regularTextBrush) });
            Colors.Add(new ColorItem { Name = "Placeholder Text", BrushKey = "WD.PlaceholderTextBrush", IsMouseOver = true, Brush = placeholderTextBrush, MouseOverBrush = CreateTransparentMouseOverBrush(placeholderTextBrush) });
            Colors.Add(new ColorItem { Name = "Border Base", BrushKey = "WD.BaseBrush", IsMouseOver = true, Brush = baseBrush, MouseOverBrush = CreateTransparentMouseOverBrush(baseBrush) });
            Colors.Add(new ColorItem { Name = "Lighter", BrushKey = "WD.LighterBrush", IsMouseOver = true, Brush = lighterBrush, MouseOverBrush = CreateTransparentMouseOverBrush(lighterBrush) });
        }

        private static SolidColorBrush CreateTransparentMouseOverBrush(SolidColorBrush brush)
        {
            if (brush == null)
            {
                return null;
            }

            var color = brush.Color;
            var mouseOverBrush = new SolidColorBrush(Color.FromArgb(0x19, color.R, color.G, color.B));
            mouseOverBrush.Freeze();
            return mouseOverBrush;
        }

        public void BtnPrimary_Click(object sender, RoutedEventArgs e)
        {
            CopyDynamicResource(sender as Button);
        }

        public void BtnMouseOver_Click(object sender, RoutedEventArgs e)
        {
            CopyDynamicResource(sender as Button);
        }

        private void CopyDynamicResource(Button button)
        {
            var key = button?.Tag?.ToString();
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            if (ClipboardHelper.TrySetText($"{{DynamicResource {key}}}"))
            {
                Toast.Push($"{key} 已复制到剪贴板");
                return;
            }

            Toast.Push("复制失败，请稍后重试", ToastImage.Error, true);
        }

    }
    public class ColorItem
    {
        private static readonly SolidColorBrush LightTextBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private static readonly SolidColorBrush DarkTextBrush = new SolidColorBrush(Color.FromRgb(31, 41, 55));
        private static readonly SolidColorBrush LightPanelBorderBrush = new SolidColorBrush(Color.FromArgb(0x33, 0, 0, 0));
        private static readonly Thickness NoBorderThickness = new Thickness(0);
        private static readonly Thickness ThinBorderThickness = new Thickness(1);

        static ColorItem()
        {
            LightTextBrush.Freeze();
            DarkTextBrush.Freeze();
            LightPanelBorderBrush.Freeze();
        }

        public string Name { get; set; }
        public SolidColorBrush Brush { get; set; }
        public string BrushKey { get; set; }
        public string ColorCode => Brush?.Color.ToString();
        public SolidColorBrush ForegroundBrush => GetReadableForegroundBrush(Brush);
        public SolidColorBrush MouseOverBrush { get; set; }
        public bool IsMouseOver { get; set; } = true;
        public bool ShowMouseOverSection => IsMouseOver && MouseOverBrush != null;
        public bool HasMouseOverKey => ShowMouseOverSection && !string.IsNullOrWhiteSpace(MouseOverKey);
        public SolidColorBrush MouseOverBorderBrush => NeedsMouseOverBorder(MouseOverBrush) ? LightPanelBorderBrush : null;
        public Thickness MouseOverBorderThickness => NeedsMouseOverBorder(MouseOverBrush) ? ThinBorderThickness : NoBorderThickness;
        public string MouseOver => IsMouseOver == true ? "MouseOver" : string.Empty;
        public string MouseOverKey { get; set; }
        public string MouseOverColorCode => IsMouseOver == true ? MouseOverBrush?.Color.ToString() : string.Empty;
        public SolidColorBrush MouseOverForegroundBrush => GetReadableForegroundBrush(MouseOverBrush);

        private static SolidColorBrush GetReadableForegroundBrush(SolidColorBrush backgroundBrush)
        {
            if (backgroundBrush == null)
            {
                return DarkTextBrush;
            }

            var color = backgroundBrush.Color;
            var alpha = color.A / 255.0;
            var blendedR = (byte)Math.Round((color.R * alpha) + (255 * (1 - alpha)));
            var blendedG = (byte)Math.Round((color.G * alpha) + (255 * (1 - alpha)));
            var blendedB = (byte)Math.Round((color.B * alpha) + (255 * (1 - alpha)));
            var luminance = (0.299 * blendedR) + (0.587 * blendedG) + (0.114 * blendedB);
            return luminance >= 160 ? DarkTextBrush : LightTextBrush;
        }

        private static bool NeedsMouseOverBorder(SolidColorBrush backgroundBrush)
        {
            if (backgroundBrush == null)
            {
                return false;
            }

            var color = backgroundBrush.Color;
            var alpha = color.A / 255.0;
            var blendedR = (byte)Math.Round((color.R * alpha) + (255 * (1 - alpha)));
            var blendedG = (byte)Math.Round((color.G * alpha) + (255 * (1 - alpha)));
            var blendedB = (byte)Math.Round((color.B * alpha) + (255 * (1 - alpha)));
            var luminance = (0.299 * blendedR) + (0.587 * blendedG) + (0.114 * blendedB);
            return luminance >= 230;
        }
    }
}
