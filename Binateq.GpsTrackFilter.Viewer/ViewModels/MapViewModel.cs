namespace Binateq.GpsTrackFilter.Viewer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Helpers;
    using Models;
    using Newtonsoft.Json;
    using MapControlLocation = MapControl.Location;

    public class MapViewModel : BaseViewModel
    {
        private readonly TrackService trackService;
        
        public MapViewModel(TrackService trackService)
        {
            this.trackService = trackService;
            
            ZoomLevel = 15;
            MapCenter = new MapControlLocation(53.5, 8.2);

            ResetToDefaultCommand = CommandFactory.Create(WrapCommandFunc(ResetDefaultFilterOptionsAsync), () => IsNotBusy);
            LoadCellIdCommand = CommandFactory.Create<Track>(LoadCellIdAsync);
            ApplyParametersCommand = CommandFactory.Create(WrapCommandFunc(ApplyParametersAsync), () => IsNotBusy);

            ResetDefaultFilterOptionsAsync();
            LoadCachedTracks();
        }

        private async void LoadCachedTracks()
        {
            try
            {
                var data = await trackService.ReadAllCacheAsync();
                foreach (var item in data)
                    CachedTracks.Add(item);

            }
            catch (Exception exception)
            {
                // Ignore
                Debug.WriteLine(exception);
            }
        }
        
        public ObservableCollection<TrackView> RawPolyline { get; } = new();
        public ObservableCollection<TrackView> FilteredPolyline { get; } = new();
        public ObservableCollection<Track> CachedTracks { get; } = new();

        public MapControlLocation MapCenter { get; set; }
        public MapControlLocation Start { get; set; }
        public MapControlLocation End { get; set; }

        public double ZoomLevel { get; set; }
        public string TrackIdTextField { get; set; }
        public double ModelPrecisionTextField { get; set; }
        public double OutlineSpeedTextField { get; set; }
        public double SensorPrecisionTextField { get; set; }
        public double ZeroSpeedDriftTextField { get; set; }
        public TimeSpan OriginDuration { get; set; }
        public double RawMeters { get; set; }
        public TimeSpan RawDuration { get; set; }
        public double FilteredMeters { get; set; }
        public TimeSpan FilteredDuration { get; set; }
        public long? CurrentTrackId { get; set; }
        public MapLayers MapLayers { get; } = new();
        
        public ICommand ResetToDefaultCommand { get; }
        private Task ResetDefaultFilterOptionsAsync()
        {
            var filterOptions = trackService.DefaultFilterOptions;

            ModelPrecisionTextField = filterOptions.ModelPrecision;
            OutlineSpeedTextField = filterOptions.OutlineSpeed;
            SensorPrecisionTextField = filterOptions.SensorPrecision;
            ZeroSpeedDriftTextField = filterOptions.ZeroSpeedDrift;

            return Task.CompletedTask;
        }
        
        private static MapControlLocation[] GetMapControlLocation(string route)
        {
            var filteredRoute = JsonConvert.DeserializeObject<RootObject>(route);
            if (filteredRoute == null)
                return Array.Empty<MapControlLocation>();

            var filteredFeatures = filteredRoute.Features[0].Geometry;
            var filteredMapControlLocations = filteredFeatures.Coordinates.Select(x => new MapControlLocation(x[1], x[0])).ToList();
            return filteredMapControlLocations.ToArray();
        }

        public ICommand LoadCellIdCommand { get; }
        private Task LoadCellIdAsync(Track track)
        {
            var tracks = trackService.FromRaw(track.Locations, BuildFilterOptions());
            var rawMapControlLocations = GetMapControlLocation(tracks.Raw);
            var filteredMapControlLocations = GetMapControlLocation(tracks.Filtered);

            var originDuration = track.EndAt - track.StartAt;
            var rawMeters = tracks.RawSummary.Meters;
            var rawDuration = tracks.RawSummary.Duration;
            var filteredMeters = tracks.FilteredSummary.Meters;
            var filteredDuration = tracks.FilteredSummary.Duration;

            SetView(originDuration, 
                rawMapControlLocations.ToTrackView(), rawMeters, rawDuration, 
                filteredMapControlLocations.ToTrackView(), filteredMeters, filteredDuration);

            CurrentTrackId = track.Id;

            return Task.CompletedTask;
        }

        private void SetView(TimeSpan originDuration, TrackView raw, double rawMeters, TimeSpan rawDuration,
            TrackView filtered, double filteredMeters, TimeSpan filteredDuration,
            bool changeZoom = true)
        {
            RawPolyline.Clear();
            FilteredPolyline.Clear();
            RawPolyline.Add(raw);
            FilteredPolyline.Add(filtered);

            if (!changeZoom)
                return;

            var start = raw.Locations.First();
            var end = raw.Locations.Last();

            var (newCenter, newZoom) = CalculateCenter(start, end, 1.08);

            Start = start;
            End = end;

            MapCenter = newCenter;
            ZoomLevel = newZoom;

            OriginDuration = originDuration;

            RawMeters = rawMeters;
            RawDuration = rawDuration;

            FilteredMeters = filteredMeters;
            FilteredDuration = filteredDuration;
        }
        
        private static (MapControlLocation center, double zoom) CalculateCenter(MapControlLocation start, MapControlLocation end, double zoomFactor = 1)
        {
            var distance = start.DistanceBetween(end);
            var log = Math.Log(distance);
            
            var zoom = 20 - log > 1 ? 20 - log : 1;
            zoom = zoomFactor * zoom;

            var center = start.Center(end);

            return (center, zoom);
        }

        public ICommand ApplyParametersCommand { get; }
        private Task ApplyParametersAsync()
        {
            if (CurrentTrackId == null)
                return Task.CompletedTask;

            var track = CachedTracks.First(x => x.Id == CurrentTrackId);
            var tracks = trackService.FromRaw(track.Locations, BuildFilterOptions());

            var rawMapControlLocations = GetMapControlLocation(tracks.Raw);
            var filteredMapControlLocations = GetMapControlLocation(tracks.Filtered);

            var originDuration = track.EndAt - track.StartAt;
            var rawMeters = tracks.RawSummary.Meters;
            var rawDuration = tracks.RawSummary.Duration;
            var filteredMeters = tracks.FilteredSummary.Meters;
            var filteredDuration = tracks.FilteredSummary.Duration;

            SetView(originDuration: originDuration,
                    raw: rawMapControlLocations.ToTrackView(), 
                    rawMeters: rawMeters, 
                    rawDuration: rawDuration,
                    filtered: filteredMapControlLocations.ToTrackView(), 
                    filteredMeters: filteredMeters, 
                    filteredDuration: filteredDuration, 
                    changeZoom: false);

            return Task.CompletedTask;
        }
        
        private GpsTrackFilterOptions BuildFilterOptions() =>
            new()
            {
                ModelPrecision = ModelPrecisionTextField,
                OutlineSpeed = OutlineSpeedTextField,
                SensorPrecision = SensorPrecisionTextField,
                ZeroSpeedDrift = ZeroSpeedDriftTextField
            };
    }
}


