using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = BorderTemplateName, Type = typeof(Border))]
    public class MagnifierDesktop : Window, IDisposable
    {
        private const string BorderTemplateName = "PART_Border";
        private Border _border;


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _border = GetTemplateChild(BorderTemplateName) as Border;
            _border.Background = new ImageBrush(ControlsHelper.Capture());
        }
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            Close();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
