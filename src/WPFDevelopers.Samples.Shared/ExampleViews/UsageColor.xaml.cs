using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFDevelopers.Controls;

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
            Colors.Add(new ColorItem { Name = "Primary Text", BrushKey = "WD.PrimaryTextBrush", IsMouseOver = false, Brush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.PrimaryTextBrush"), MouseOverBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.PrimaryTextBrush") });
            Colors.Add(new ColorItem { Name = "Regular Text", BrushKey = "WD.RegularTextBrush", IsMouseOver = false, Brush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.RegularTextBrush"), MouseOverBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.RegularTextBrush") });
            Colors.Add(new ColorItem { Name = "Placeholder Text", BrushKey = "WD.PlaceholderTextBrush", IsMouseOver = false, Brush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.PlaceholderTextBrush"), MouseOverBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.PlaceholderTextBrush") });
            Colors.Add(new ColorItem { Name = "Border Base", BrushKey = "WD.BaseBrush", IsMouseOver = false, Brush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.BaseBrush"), MouseOverBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.BaseBrush") });
            Colors.Add(new ColorItem { Name = "Lighter", BrushKey = "WD.LighterBrush", IsMouseOver = false, Brush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.LighterBrush"), MouseOverBrush = ThemeManager.Instance.Resources.TryFindResource<SolidColorBrush>("WD.LighterBrush") });
        }

        public void BtnPrimary_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var key = btn.Tag.ToString();
            Message.Push($"{key} 已复制粘贴板");
            Clipboard.SetText($"{{DynamicResource {key}}}");
        }

        public void BtnMouseOver_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var key = btn.Tag.ToString();
            Message.Push($"{key} 已复制粘贴板");
            Clipboard.SetText($"{{DynamicResource {key}}}");
        }

    }
    public class ColorItem
    {
        public string Name { get; set; }
        public SolidColorBrush Brush { get; set; }
        public string BrushKey { get; set; }
        public string ColorCode => Brush?.Color.ToString();
        public SolidColorBrush MouseOverBrush { get; set; }
        public bool IsMouseOver { get; set; } = true;
        public string MouseOver => IsMouseOver == true ? "MouseOver" : string.Empty;
        public string MouseOverKey { get; set; }
        public string MouseOverColorCode => IsMouseOver == true ? MouseOverBrush?.Color.ToString() : string.Empty;
    }
}
