using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Samples.Controls
{
    [TemplatePart(Name = TabControlTemplateName, Type = typeof(TabControl))]
    public class CodeViewer : ContentControl
    {
        private static readonly Type _typeofSelf = typeof(CodeViewer);
        public ObservableCollection<SourceCodeModel> SourceCodes { get; } = new ObservableCollection<SourceCodeModel>();
        private const string TabControlTemplateName = "PART_TabControl";
        private TabControl _tabControl = null;
        static CodeViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(_typeofSelf,
                new FrameworkPropertyMetadata(_typeofSelf));
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
                Content = partTextEditor
            };
            switch (codeModel.CodeType)
            {
                case CodeType.Xaml:
                    partTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(".XML");
                    tabItem.Header = codeModel.Haader == null ? "Xaml" : codeModel.Haader;
                    break;
                case CodeType.CSharp:
                    partTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(".CS");
                    tabItem.Header = codeModel.Haader == null ? "CSharp" : codeModel.Haader;
                    break;
            }
            
            return tabItem;
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
