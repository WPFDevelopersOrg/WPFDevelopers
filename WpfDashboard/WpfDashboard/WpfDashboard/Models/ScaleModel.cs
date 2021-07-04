using System.Windows.Media;

namespace WpfDashboard.Models
{
    public class ScaleModel : ViewModelBase
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
