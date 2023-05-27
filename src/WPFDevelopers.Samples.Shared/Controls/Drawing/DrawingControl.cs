using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Samples.Controls
{
    [TemplatePart(Name = _PartInnerName, Type = typeof(Border))]
    public class DrawingControl : ListBox
    {
        private const string _PartInnerName = "PART_InnerBorder";

        public static readonly DependencyProperty ItemOffsetProperty =
            DependencyProperty.Register("ItemOffset", typeof(double), typeof(DrawingControl),
                new PropertyMetadata(10d));

        public static readonly DependencyProperty PanelBackgroundProperty =
            DependencyProperty.Register("PanelBackground", typeof(Brush), typeof(DrawingControl),
                new PropertyMetadata(default));

        public static readonly DependencyProperty PanelBorderBrushProperty =
            DependencyProperty.Register("PanelBorderBrush", typeof(Brush), typeof(DrawingControl),
                new PropertyMetadata(Brushes.Gray));

        public static readonly DependencyProperty PanelBorderThincknessProperty =
            DependencyProperty.Register("PanelBorderThinckness", typeof(Thickness), typeof(DrawingControl),
                new PropertyMetadata(new Thickness(1d)));

        public static readonly DependencyProperty PanelCornerRadiusProperty =
            DependencyProperty.Register("PanelCornerRadius", typeof(CornerRadius), typeof(DrawingControl),
                new PropertyMetadata(new CornerRadius(1d)));

        public static readonly DependencyProperty PanelDrawingModeProperty =
            DependencyProperty.Register("PanelDrawingMode", typeof(DrawingMode), typeof(DrawingControl),
                new PropertyMetadata(default(DrawingMode)));

        public static readonly DependencyProperty PanelStartAngleOffsetProperty =
            DependencyProperty.Register("PanelStartAngleOffset", typeof(double), typeof(DrawingControl),
                new PropertyMetadata(0d));

        public static readonly DependencyProperty PanelMultipleDescriptionProperty =
            DependencyProperty.Register("PanelMultipleDescription", typeof(string), typeof(DrawingControl),
                new PropertyMetadata(default));

        public static readonly DependencyProperty PanelMultipleSpaceProperty =
            DependencyProperty.Register("PanelMultipleSpace", typeof(double), typeof(DrawingControl),
                new PropertyMetadata(0d));

        public static readonly DependencyProperty InnerMarginProperty =
            DependencyProperty.Register("InnerMargin", typeof(Thickness), typeof(DrawingControl),
                new PropertyMetadata(new Thickness(50d)));

        public static readonly DependencyProperty InnerBackgroundProperty =
            DependencyProperty.Register("InnerBackground", typeof(Brush), typeof(DrawingControl),
                new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty InnerCornerRadiusProperty =
            DependencyProperty.Register("InnerCornerRadius", typeof(CornerRadius), typeof(DrawingControl),
                new PropertyMetadata(default));

        public static readonly DependencyProperty InnerBorderThicknessProperty =
            DependencyProperty.Register("InnerBorderThickness", typeof(Thickness), typeof(DrawingControl),
                new PropertyMetadata(new Thickness(1d)));

        public static readonly DependencyProperty InnerBorderBrushProperty =
            DependencyProperty.Register("InnerBorderBrush", typeof(Brush), typeof(DrawingControl),
                new PropertyMetadata(Brushes.Gray));

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(DrawingControl),
                new PropertyMetadata(0.4d, OnScalePropertyChangedCallBack));

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(DrawingControl),
                new PropertyMetadata(default));

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(DrawingControl),
                new PropertyMetadata(default));

        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(DrawingControl),
                new PropertyMetadata(default));

        public static readonly DependencyProperty ContentStringFormatProperty =
            DependencyProperty.Register("ContentStringFormat", typeof(string), typeof(DrawingControl),
                new PropertyMetadata(default));

        public static readonly DependencyProperty ContentSourceProperty =
            DependencyProperty.Register("ContentSource", typeof(string), typeof(DrawingControl),
                new PropertyMetadata("Content"));

        protected Border _Border;

        protected DrawingPanel _DrawingPanel = default;

        static DrawingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawingControl),
                new FrameworkPropertyMetadata(typeof(DrawingControl)));
        }
        //private const string _PartDrawingPanelName = "PART_DrawingPanel";

        public DrawingControl()
        {
            Loaded += DrawingControl_Loaded;
            SizeChanged += DrawingControl_SizeChanged;
        }

        public double ItemOffset
        {
            get => (double)GetValue(ItemOffsetProperty);
            set => SetValue(ItemOffsetProperty, value);
        }

        public Brush PanelBackground
        {
            get => (Brush)GetValue(PanelBackgroundProperty);
            set => SetValue(PanelBackgroundProperty, value);
        }

        public Brush PanelBorderBrush
        {
            get => (Brush)GetValue(PanelBorderBrushProperty);
            set => SetValue(PanelBorderBrushProperty, value);
        }

        public Thickness PanelBorderThinckness
        {
            get => (Thickness)GetValue(PanelBorderThincknessProperty);
            set => SetValue(PanelBorderThincknessProperty, value);
        }

        public CornerRadius PanelCornerRadius
        {
            get => (CornerRadius)GetValue(PanelCornerRadiusProperty);
            set => SetValue(PanelCornerRadiusProperty, value);
        }

        public DrawingMode PanelDrawingMode
        {
            get => (DrawingMode)GetValue(PanelDrawingModeProperty);
            set => SetValue(PanelDrawingModeProperty, value);
        }

        public double PanelStartAngleOffset
        {
            get => (double)GetValue(PanelStartAngleOffsetProperty);
            set => SetValue(PanelStartAngleOffsetProperty, value);
        }


        public string PanelMultipleDescription
        {
            get => (string)GetValue(PanelMultipleDescriptionProperty);
            set => SetValue(PanelMultipleDescriptionProperty, value);
        }


        public double PanelMultipleSpace
        {
            get => (double)GetValue(PanelMultipleSpaceProperty);
            set => SetValue(PanelMultipleSpaceProperty, value);
        }


        public Thickness InnerMargin
        {
            get => (Thickness)GetValue(InnerMarginProperty);
            set => SetValue(InnerMarginProperty, value);
        }

        public Brush InnerBackground
        {
            get => (Brush)GetValue(InnerBackgroundProperty);
            set => SetValue(InnerBackgroundProperty, value);
        }

        public CornerRadius InnerCornerRadius
        {
            get => (CornerRadius)GetValue(InnerCornerRadiusProperty);
            set => SetValue(InnerCornerRadiusProperty, value);
        }

        public Thickness InnerBorderThickness
        {
            get => (Thickness)GetValue(InnerBorderThicknessProperty);
            set => SetValue(InnerBorderThicknessProperty, value);
        }

        public Brush InnerBorderBrush
        {
            get => (Brush)GetValue(InnerBorderBrushProperty);
            set => SetValue(InnerBorderBrushProperty, value);
        }

        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }


        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }


        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }


        public DataTemplateSelector ContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty);
            set => SetValue(ContentTemplateSelectorProperty, value);
        }


        public string ContentStringFormat
        {
            get => (string)GetValue(ContentStringFormatProperty);
            set => SetValue(ContentStringFormatProperty, value);
        }


        public string ContentSource
        {
            get => (string)GetValue(ContentSourceProperty);
            set => SetValue(ContentSourceProperty, value);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Border = GetTemplateChild(_PartInnerName) as Border;
            // _DrawingPanel = GetTemplateChild(_PartDrawingPanelName) as DrawingPanel;

            if (_Border == null)
                throw new Exception($"The {_PartInnerName} is not exist!");
        }

        private void DrawingControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Width = Math.Min(RenderSize.Width, RenderSize.Height);
            //Height = Width;
        }

        private void DrawingControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var vMin = Math.Min(e.NewSize.Width, e.NewSize.Height);
            PanelCornerRadius = new CornerRadius(vMin);
            InnerCornerRadius = new CornerRadius(vMin);
            //InnerMargin = new Thickness(vMin * Scale);
            if (_Border != null)
            {
                _Border.Width = vMin * Scale;
                _Border.Height = vMin * Scale;
            }
        }

        private static void OnScalePropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DrawingControl drawing))
                return;

            //drawing.InnerMargin = new Thickness(vMin * drawing.Scale);
            if (drawing._Border != null)
            {
                var vMin = Math.Min(drawing.RenderSize.Width, drawing.RenderSize.Height);
                drawing._Border.Width = vMin * drawing.Scale;
                drawing._Border.Height = vMin * drawing.Scale;
            }
        }
    }
}