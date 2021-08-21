namespace Binateq.GpsTrackFilter.Viewer
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using MapControl;

    public class MapLayers : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Dictionary<string, UIElement> mapLayers = new Dictionary<string, UIElement>
        {
            {
                "OpenStreetMap",
                MapTileLayer.OpenStreetMapTileLayer
            }
        };

        private string currentMapLayerName = "OpenStreetMap";

        public string CurrentMapLayerName
        {
            get => currentMapLayerName;
            set
            {
                currentMapLayerName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMapLayerName)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMapLayer)));
            }
        }

        public UIElement CurrentMapLayer => mapLayers[currentMapLayerName];

        public UIElement SeamarksLayer => mapLayers["Seamarks"];

        public List<string> MapLayerNames { get; } = new List<string>
        {
            "OpenStreetMap",
        };

        public MapLayers()
        {

        }
    }
}
