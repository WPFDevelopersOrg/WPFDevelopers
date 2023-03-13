using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
            Visual parent = null;
            if (Application.Current.Windows.Count > 0)
                parent = Application.Current.Windows.OfType<Window>().FirstOrDefault(o => o.IsActive);
            var _layer = GetAdornerLayer(parent);
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
        static AdornerLayer GetAdornerLayer(Visual visual)
        {
            var decorator = visual as AdornerDecorator;
            if (decorator != null)
                return decorator.AdornerLayer;
            var presenter = visual as ScrollContentPresenter;
            if (presenter != null)
                return presenter.AdornerLayer;
            var visualContent = (visual as Window)?.Content as Visual;
            return AdornerLayer.GetAdornerLayer(visualContent ?? visual);
        }
    }
}
