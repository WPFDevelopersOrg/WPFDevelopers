using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace WPFDevelopers.Samples.Controls
{
    [TemplatePart(Name = TabControlTemplateName, Type = typeof(TabControl))]
    public class CodeViewer : HeaderedContentControl
    {
        private static readonly Type _typeofSelf = typeof(CodeViewer);
        public ObservableCollection<SourceCodeModel> SourceCodes { get; } = new ObservableCollection<SourceCodeModel>();
        private const string TabControlTemplateName = "PART_TabControl";
        private TabControl _tabControl = null;
        private static readonly Lazy<HighlightingManager> DarkHighlighting = new Lazy<HighlightingManager>(CreateDarkHighlighting);
        private static readonly Brush DarkLinkBrush = new SolidColorBrush(Color.FromRgb(0x9C, 0xDC, 0xFE));

        static CodeViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(_typeofSelf,
                new FrameworkPropertyMetadata(_typeofSelf));
        }

        public CodeViewer()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _tabControl = GetTemplateChild(TabControlTemplateName) as TabControl;
            foreach (var item in SourceCodes)
            {
                var tabItem = CreateTabItem(item);
                tabItem.Height = 38;
                tabItem.Padding = new Thickness(20,0,20,0);
                _tabControl.Items.Add(tabItem);
            }
            UpdateHighlighting();
        }
        TabItem CreateTabItem(SourceCodeModel codeModel)
        {
            if(codeModel== null)return null;
            var partTextEditor = new TextEditor();
            partTextEditor.Options = new TextEditorOptions { ConvertTabsToSpaces = true };
            partTextEditor.TextArea.SelectionCornerRadius = 0;
            partTextEditor.SetResourceReference(TextArea.SelectionBrushProperty, "WD.WindowBorderBrush");
            partTextEditor.TextArea.SelectionBorder = null;
            partTextEditor.TextArea.SelectionForeground = null;
            partTextEditor.IsReadOnly = true;
            partTextEditor.ShowLineNumbers = true;
            partTextEditor.FontFamily = DrawingContextHelper.FontFamily;
            partTextEditor.Text = GetCodeText(codeModel.CodeSource);
            var tabItem = new TabItem
            {
                Content = partTextEditor,
                Tag = codeModel.CodeType
            };
            switch (codeModel.CodeType)
            {
                case CodeType.Xaml:
                    tabItem.Header = codeModel.Haader == null ? "Xaml" : codeModel.Haader;
                    break;
                case CodeType.CSharp:
                    tabItem.Header = codeModel.Haader == null ? "CSharp" : codeModel.Haader;
                    break;
            }
            
            return tabItem;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            WPFDevelopers.Resources.ThemeChanged -= OnThemeChanged;
            WPFDevelopers.Resources.ThemeChanged += OnThemeChanged;
            UpdateHighlighting();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            WPFDevelopers.Resources.ThemeChanged -= OnThemeChanged;
        }

        private void OnThemeChanged(ThemeType theme)
        {
            UpdateHighlighting();
        }

        private void UpdateHighlighting()
        {
            if (_tabControl == null)
                return;

            var resources = ThemeManager.Instance.Resources;
            var isDark = resources != null && resources.IsDarkTheme;
            var manager = isDark ? DarkHighlighting.Value : HighlightingManager.Instance;
            foreach (TabItem tab in _tabControl.Items)
            {
                var editor = tab.Content as TextEditor;
                if (editor == null || !(tab.Tag is CodeType))
                    continue;

                if (isDark)
                    editor.TextArea.TextView.LinkTextForegroundBrush = DarkLinkBrush;
                else
                    editor.TextArea.TextView.ClearValue(TextView.LinkTextForegroundBrushProperty);

                switch ((CodeType)tab.Tag)
                {
                    case CodeType.Xaml:
                        editor.SyntaxHighlighting = manager.GetDefinitionByExtension(".xml");
                        break;
                    case CodeType.CSharp:
                        editor.SyntaxHighlighting = manager.GetDefinitionByExtension(".cs");
                        break;
                }
            }
        }

        private static HighlightingManager CreateDarkHighlighting()
        {
            var manager = new HighlightingManager();
            manager.RegisterHighlighting("XmlDoc", null, LoadDarkHighlighting("XmlDoc", manager));
            manager.RegisterHighlighting("XML", new[] { ".xml" }, LoadDarkHighlighting("XML-Mode", manager));
            manager.RegisterHighlighting("C#", new[] { ".cs" }, LoadDarkHighlighting("CSharp-Mode", manager));
            return manager;
        }

        private static IHighlightingDefinition LoadDarkHighlighting(string name, HighlightingManager manager)
        {
            var colors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Green", "#8FCB81" },
                { "Blue", "#9CDCFE" },
                { "DarkMagenta", "#D7A0D4" },
                { "Red", "#FFAA91" },
                { "Teal", "#80CBC4" },
                { "Olive", "#DCDCAA" },
                { "Black", "#E6E6E6" },
                { "Magenta", "#D7A0D4" },
                { "MidnightBlue", "#DCDCAA" },
                { "DarkBlue", "#B5CEA8" },
                { "Navy", "#C6A8E0" },
                { "DarkGray", "#BDBDBD" },
                { "Pink", "#E6B8C6" },
                { "DeepPink", "#E6A6CD" },
                { "Brown", "#C6A8E0" },
                { "SaddleBrown", "#DCDCAA" },
                { "DarkCyan", "#80CBC4" },
                { "#E0E000", "#E0E080" },
                { "Silver", "#C0C0C0" },
                { "Gray", "#ADADAD" }
            };
            var assembly = typeof(HighlightingManager).Assembly;
            var resourceName = "ICSharpCode.AvalonEdit.Highlighting.Resources." + name + ".xshd";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new InvalidOperationException("Missing AvalonEdit highlighting resource: " + resourceName);

                var document = new XmlDocument();
                document.Load(stream);
                foreach (XmlElement element in document.SelectNodes("//*[@foreground]"))
                {
                    var foreground = element.GetAttribute("foreground");
                    string darkColor;
                    if (!colors.TryGetValue(foreground, out darkColor))
                        throw new InvalidOperationException("Unknown AvalonEdit highlighting color: " + foreground);
                    element.SetAttribute("foreground", darkColor);
                }
                using (var reader = new XmlNodeReader(document))
                    return HighlightingLoader.Load(reader, manager);
            }
        }

        string GetCodeText(string codeSource)
        {
            var code = string.Empty;
            var uri = new Uri(codeSource, UriKind.Relative);
            var resourceStream = Application.GetResourceStream(uri);
            if (resourceStream != null)
            {
                var streamReader = new StreamReader(resourceStream.Stream);
                code = streamReader.ReadToEnd();
                return code;
            }
            return code;
        }
    }
}
