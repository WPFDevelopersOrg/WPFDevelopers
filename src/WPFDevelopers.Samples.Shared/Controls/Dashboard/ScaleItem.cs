using System.Windows.Media;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Samples.Controls
{
    public class ScaleItem : ViewModelBase
    {
        private Brush _background;
        public int Index { get; set; }

        public Brush Background
        {
            get => _background;
            set
            {
                _background = value;
                NotifyPropertyChange("Background");
            }
        }
    }
}