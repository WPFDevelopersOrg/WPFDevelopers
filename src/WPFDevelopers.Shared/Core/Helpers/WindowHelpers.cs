using System.Windows;
using System.Windows.Media;
using WPFDevelopers.Controls;
using WPFDevelopers.Utilities;

namespace WPFDevelopers.Helpers
{
    public static class WindowHelpers
    {
        public static void MaskShow(this Window outputWindow)
        {
            CreateMask(outputWindow);
            outputWindow.Show();
        }
        public static bool? MaskShowDialog(this Window outputWindow)
        {
            CreateMask(outputWindow);
            return outputWindow.ShowDialog();
        }
        public static void CreateMask(this Window outputWindow)
        {
            Visual parent = ControlsHelper.GetDefaultWindow();
            var _layer = ControlsHelper.GetAdornerLayer(parent);
            if (_layer == null) return;
            var _adornerContainer = new AdornerContainer(_layer)
            {
                Child = new MaskControl(parent)
            };
            _layer.Add(_adornerContainer);
            if (outputWindow != null)
            {
                outputWindow.Closed += delegate
                {
                    if (parent != null)
                        _layer.Remove(_adornerContainer);
                };
            }
        }
    }
}
