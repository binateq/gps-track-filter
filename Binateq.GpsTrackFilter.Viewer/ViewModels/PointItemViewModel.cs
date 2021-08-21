namespace Binateq.GpsTrackFilter.Viewer.ViewModels
{
    using System.ComponentModel;
    using MapControl;

    public class PointItemViewModel : INotifyPropertyChanged
    {
        private string name;
        private Location location;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => name;
            set { name = value; PropertyChanged?.Invoke(this, default); }
        }
        
        public Location Location
        {
            get => location;
            set { location = value; PropertyChanged?.Invoke(this, default); }
        }
    }
}
