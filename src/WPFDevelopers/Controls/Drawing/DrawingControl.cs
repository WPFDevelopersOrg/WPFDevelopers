using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = _PartInnerName, Type = typeof(Border))]
    //[TemplatePart(Name = _PartDrawingPanelName, Type=typeof(DrawingPanel))]
    //[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(DrawingElement))]
    public class DrawingControl : ListBox
    {
        static DrawingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawingControl), new FrameworkPropertyMetadata(typeof(DrawingControl)));
        }

        private const string _PartInnerName = "PART_InnerBorder";
        //private const string _PartDrawingPanelName = "PART_DrawingPanel";

        public DrawingControl()
        {
            Loaded += DrawingControl_Loaded;
            SizeChanged += DrawingControl_SizeChanged;
        }

        protected DrawingPanel _DrawingPanel = default;
        protected Border _Border = default;


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

        public double ItemOffset
        {
            get { return (double)GetValue(ItemOffsetProperty); }
            set { SetValue(ItemOffsetProperty, value); }
        }

        public static readonly DependencyProperty ItemOffsetProperty =
            DependencyProperty.Register("ItemOffset", typeof(double), typeof(DrawingControl), new PropertyMetadata(10d));

        public Brush PanelBackground
        {
            get { return (Brush)GetValue(PanelBackgroundProperty); }
            set { SetValue(PanelBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PanelBackgroundProperty =
            DependencyProperty.Register("PanelBackground", typeof(Brush), typeof(DrawingControl), new PropertyMetadata(default));

        public Brush PanelBorderBrush
        {
            get { return (Brush)GetValue(PanelBorderBrushProperty); }
            set { SetValue(PanelBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty PanelBorderBrushProperty =
            DependencyProperty.Register("PanelBorderBrush", typeof(Brush), typeof(DrawingControl), new PropertyMetadata(Brushes.Gray));

        public Thickness PanelBorderThinckness
        {
            get { return (Thickness)GetValue(PanelBorderThincknessProperty); }
            set { SetValue(PanelBorderThincknessProperty, value); }
        }

        public static readonly DependencyProperty PanelBorderThincknessProperty =
            DependencyProperty.Register("PanelBorderThinckness", typeof(Thickness), typeof(DrawingControl), new PropertyMetadata(new Thickness(1d)));

        public CornerRadius PanelCornerRadius
        {
            get { return (CornerRadius)GetValue(PanelCornerRadiusProperty); }
            set { SetValue(PanelCornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty PanelCornerRadiusProperty =
            DependencyProperty.Register("PanelCornerRadius", typeof(CornerRadius), typeof(DrawingControl), new PropertyMetadata(new CornerRadius(1d)));

        public DrawingMode PanelDrawingMode
        {
            get { return (DrawingMode)GetValue(PanelDrawingModeProperty); }
            set { SetValue(PanelDrawingModeProperty, value); }
        }

        public static readonly DependencyProperty PanelDrawingModeProperty =
            DependencyProperty.Register("PanelDrawingMode", typeof(DrawingMode), typeof(DrawingControl), new PropertyMetadata(default(DrawingMode)));

        public double PanelStartAngleOffset
        {
            get { return (double)GetValue(PanelStartAngleOffsetProperty); }
            set { SetValue(PanelStartAngleOffsetProperty, value); }
        }

        public static readonly DependencyProperty PanelStartAngleOffsetProperty =
            DependencyProperty.Register("PanelStartAngleOffset", typeof(double), typeof(DrawingControl), new PropertyMetadata(0d));



        public string PanelMultipleDescription
        {
            get { return (string)GetValue(PanelMultipleDescriptionProperty); }
            set { SetValue(PanelMultipleDescriptionProperty, value); }
        }

        public static readonly DependencyProperty PanelMultipleDescriptionProperty =
            DependencyProperty.Register("PanelMultipleDescription", typeof(string), typeof(DrawingControl), new PropertyMetadata(default));



        public double PanelMultipleSpace
        {
            get { return (double)GetValue(PanelMultipleSpaceProperty); }
            set { SetValue(PanelMultipleSpaceProperty, value); }
        }

        public static readonly DependencyProperty PanelMultipleSpaceProperty =
            DependencyProperty.Register("PanelMultipleSpace", typeof(double), typeof(DrawingControl), new PropertyMetadata(0d));




        public Thickness InnerMargin
        {
            get { return (Thickness)GetValue(InnerMarginProperty); }
            set { SetValue(InnerMarginProperty, value); }
        }

        public static readonly DependencyProperty InnerMarginProperty =
            DependencyProperty.Register("InnerMargin", typeof(Thickness), typeof(DrawingControl), new PropertyMetadata(new Thickness(50d)));

        public Brush InnerBackground
        {
            get { return (Brush)GetValue(InnerBackgroundProperty); }
            set { SetValue(InnerBackgroundProperty, value); }
        }

        public static readonly DependencyProperty InnerBackgroundProperty =
            DependencyProperty.Register("InnerBackground", typeof(Brush), typeof(DrawingControl), new PropertyMetadata(Brushes.Transparent));

        public CornerRadius InnerCornerRadius
        {
            get { return (CornerRadius)GetValue(InnerCornerRadiusProperty); }
            set { SetValue(InnerCornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty InnerCornerRadiusProperty =
            DependencyProperty.Register("InnerCornerRadius", typeof(CornerRadius), typeof(DrawingControl), new PropertyMetadata(default));

        public Thickness InnerBorderThickness
        {
            get { return (Thickness)GetValue(InnerBorderThicknessProperty); }
            set { SetValue(InnerBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty InnerBorderThicknessProperty =
            DependencyProperty.Register("InnerBorderThickness", typeof(Thickness), typeof(DrawingControl), new PropertyMetadata(new Thickness(1d)));

        public Brush InnerBorderBrush
        {
            get { return (Brush)GetValue(InnerBorderBrushProperty); }
            set { SetValue(InnerBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty InnerBorderBrushProperty =
            DependencyProperty.Register("InnerBorderBrush", typeof(Brush), typeof(DrawingControl), new PropertyMetadata(Brushes.Gray));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(DrawingControl), new PropertyMetadata(0.4d, OnScalePropertyChangedCallBack));



        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(DrawingControl), new PropertyMetadata(default));



        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(DrawingControl), new PropertyMetadata(default));



        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
            set { SetValue(ContentTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(DrawingControl), new PropertyMetadata(default));



        public string ContentStringFormat
        {
            get { return (string)GetValue(ContentStringFormatProperty); }
            set { SetValue(ContentStringFormatProperty, value); }
        }

        public static readonly DependencyProperty ContentStringFormatProperty =
            DependencyProperty.Register("ContentStringFormat", typeof(string), typeof(DrawingControl), new PropertyMetadata(default));




        public string ContentSource
        {
            get { return (string)GetValue(ContentSourceProperty); }
            set { SetValue(ContentSourceProperty, value); }
        }

        public static readonly DependencyProperty ContentSourceProperty =
            DependencyProperty.Register("ContentSource", typeof(string), typeof(DrawingControl), new PropertyMetadata("Content"));

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
