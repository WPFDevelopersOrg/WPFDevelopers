using System.Windows.Media;
using WPFDevelopers.Helpers;

namespace WPFDevelopers.Controls
{
    public class ScaleItem : ViewModelBase
    {
        public int Index { get; set; }

        private Brush _background;

        public Brush Background
        {
            get { return _background; }
            set
            {
                _background = value;
                this.NotifyPropertyChange("Background");
            }
        }
    }
}
