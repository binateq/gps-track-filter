namespace Binateq.GpsTrackFilter.Viewer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Binateq.GpsTrackFilter;
    using Helpers;
    using Maps;
    using Maps.Extensions;

    public class TrackService
    {
        public TrackService()
        {
            var filter = new GpsTrackFilter();
            DefaultFilterOptions = new GpsTrackFilterOptions
            {
                ModelPrecision = filter.ModelPrecision,
                OutlineSpeed = filter.OutlineSpeed,
                SensorPrecision = filter.SensorPrecision,
                ZeroSpeedDrift = filter.ZeroSpeedDrift
            };
        }

        public GpsTrackFilterOptions DefaultFilterOptions { get; }

        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new DoubleConverter()
            }
        };
        
        public async Task<IReadOnlyList<Track>> ReadAllCacheAsync()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var newDirectoryPath = Path.Combine(currentPath, "Tracks");

            Directory.CreateDirectory(newDirectoryPath);

            var files =
                Directory.EnumerateFiles(newDirectoryPath)
                         .Where(x => x.EndsWith(".rwt"))
                         .ToArray();

            var result = new ConcurrentBag<Track>();

            async Task SafeRead(string path)
            {
                var json = await File.ReadAllTextAsync(path);
                try
                {
                    var track = JsonSerializer.Deserialize<Track>(json, serializerOptions);
                    result.Add(track);
                }
                catch
                {
                    //
                }
            }

            var tasks = files.Select(SafeRead);
            await Task.WhenAll(tasks);

            return result.ToArray();
        }

        public Tracks FromRaw(IReadOnlyList<IndexedLocation> indexedLocations, GpsTrackFilterOptions filterOptions)
        {
            var rawGeoJson = indexedLocations.ToGeoJson();            
            var rawSummary = indexedLocations.ToTrackSummary();

            var directedLocations = indexedLocations.FixGpsTimestamp()
                .SmoothTimestamp()
                .ToDirectedLocations();

            var filter = new GpsTrackFilter
            {
                ModelPrecision = filterOptions.ModelPrecision,
                OutlineSpeed = filterOptions.OutlineSpeed,
                SensorPrecision = filterOptions.SensorPrecision,
                ZeroSpeedDrift = filterOptions.ZeroSpeedDrift
            };

            var filteredLocations = filter.Filter(directedLocations).ToDeviceLocations();
            var filteredSummary = filteredLocations.ToTrackSummary();
            var filteredGeoJson = filteredLocations.ToGeoJson();

            return new Tracks
            {
                Raw = rawGeoJson,
                RawSummary = rawSummary,
                Filtered = filteredGeoJson,
                FilteredSummary = filteredSummary
            };
        }
    }
}
