namespace Binateq.GpsTrackFilter.Viewer.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using Maps;
    using Models;
    using Location = Maps.Location;
    using FLocation = Binateq.GpsTrackFilter.Location;
    using DLocation = Binateq.GpsTrackFilter.DirectedLocation;
    using MapControlLocation = MapControl.Location;
    using MapControlLocationCollection = MapControl.LocationCollection;    

    public static class Extensions
    {
        public static string ToGeoJson(this IEnumerable<FLocation> filteredLocations)
        {
            return filteredLocations.ToLocations()
                                    .ToGeoJson();
        }

        public static string ToGeoJson(this IEnumerable<DLocation> directedLocations)
        {
            return directedLocations.ToLocations()
                                    .ToGeoJson();
        }

        public static IEnumerable<Location> ToLocations(this IEnumerable<FLocation> filteredLocations)
        {
            if (filteredLocations == null)
                throw new ArgumentNullException(nameof(filteredLocations));

            return filteredLocations.Select(x => new Location(x.Latitude, x.Longitude));
        }

        public static IEnumerable<Location> ToLocations(this IEnumerable<DLocation> directedLocations)
        {
            if (directedLocations == null)
                throw new ArgumentNullException(nameof(directedLocations));

            return directedLocations.Select(x => new Location(x.Latitude, x.Longitude));
        }

        public static string ToGeoJson(this IEnumerable<Location> locations)
        {
            if (locations == null)
                throw new ArgumentNullException(nameof(locations));

            return JsonSerializer.Serialize(new
            {
                type = "FeatureCollection",
                features = new[]
                {
                    new
                    {
                        type = "Feature",
                        properties = new { },
                        geometry = new
                        {
                            type = "LineString",
                            coordinates = locations.Select(x => new[] {x.Longitude, x.Latitude})
                                .ToArray()
                        }
                    }
                }
            });
        }
                
        public static Location ToLocation(this FLocation filteredLocation)
        {
            return new Location(filteredLocation.Latitude, filteredLocation.Longitude);
        }

        public static IReadOnlyList<DeviceLocation> ToDeviceLocations(this IReadOnlyList<FLocation> filteredLocations)
        {
            if (filteredLocations == null)
                throw new ArgumentNullException(nameof(filteredLocations));

            var result = new List<DeviceLocation>(filteredLocations.Count);
            result.AddRange(from filteredLocation in filteredLocations
                            select filteredLocation.ToDeviceLocation());
            return result;
        }

        public static DeviceLocation ToDeviceLocation(this FLocation filteredLocation)
        {
            return new DeviceLocation(filteredLocation.Latitude, 
                filteredLocation.Longitude, 
                0, 
                0, 
                filteredLocation.Timestamp);
        }

        public static IReadOnlyList<DLocation> ToDirectedLocations(this IReadOnlyList<DeviceLocation> deviceLocations)
        {
            if (deviceLocations == null)
                throw new ArgumentNullException(nameof(deviceLocations));

            var result = new List<DLocation>(deviceLocations.Count);
            result.AddRange(from deviceLocation in deviceLocations
                            select deviceLocation.ToDirectedLocation());

            return result;
        }

        public static DLocation ToDirectedLocation(this DeviceLocation deviceLocation)
        {
            if (deviceLocation == null)
                throw new ArgumentNullException(nameof(deviceLocation));

            return new DLocation(deviceLocation.Latitude, 
                                 deviceLocation.Longitude, 
                                 deviceLocation.Speed, 
                                 deviceLocation.Heading, 
                                 deviceLocation.Timestamp);
        }

        public static TrackView ToTrackView(this IReadOnlyList<MapControlLocation> locations)
        {
            if (locations == null)
                throw new ArgumentNullException(nameof(locations));

            return new TrackView {Locations = new MapControlLocationCollection(locations)};
        }

        public static string ToCsv(this IEnumerable<DeviceLocation> deviceLocations)
        {
            long i = 1;
            var lines = from trackPoint in deviceLocations
                let index = i++
                select string.Format(CultureInfo.InvariantCulture,
                    "{0}, {1}, {2}, {3}",
                    index,
                    trackPoint.Latitude,
                    trackPoint.Longitude,
                    trackPoint.Timestamp);

            return string.Join('\n', lines);
        }

        public static string ToCSharp(this IEnumerable<DeviceLocation> deviceLocations)
        {
            var lines = from trackPoint in deviceLocations
                select string.Format(CultureInfo.InvariantCulture,
                    "new Location({0}, {1}, new DateTimeOffset({2}, {3}, {4}, {5}, {6}, {7}, {8}, TimeSpan.Zero)),",
                    trackPoint.Latitude,
                    trackPoint.Longitude,
                    trackPoint.Timestamp.Year,
                    trackPoint.Timestamp.Month,
                    trackPoint.Timestamp.Day,
                    trackPoint.Timestamp.Hour,
                    trackPoint.Timestamp.Minute,
                    trackPoint.Timestamp.Second,
                    trackPoint.Timestamp.Millisecond);

            return string.Join('\n', lines);
        }

        public static string ToGpx(this IEnumerable<DeviceLocation> deviceLocations)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            builder.AppendLine(
                "<gpx xmlns=\"http://www.topografix.com/GPX/1/1\" xmlns:gpsies=\"https://www.gpsies.com/GPX/1/0\" creator=\"GPSies https://www.gpsies.com - GPSies Track\" version=\"1.1\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd https://www.gpsies.com/GPX/1/0 https://www.gpsies.com/gpsies.xsd\">");

            builder.AppendLine("<metadata>");
            builder.AppendLine("<name>GPSies Track</name>");
            builder.AppendLine("<link href=\"https://www.gpsies.com/\">");
            builder.AppendLine("<text>GPSies Track on GPSies.com</text>");
            builder.AppendLine("</link>");
            builder.AppendLine("<time>2018-07-12T15:54:48Z</time>");
            builder.AppendLine("</metadata>");

            builder.AppendLine("<trk>");
            builder.AppendLine("<name>GPSies Track on GPSies.com</name>");
            builder.AppendLine("<trkseg>");

            foreach (var x in deviceLocations)
            {
                builder.AppendLine(
                    $"<trkpt lat=\"{x.Latitude.ToString(CultureInfo.InvariantCulture)}\" lon=\"{x.Longitude.ToString(CultureInfo.InvariantCulture)}\">");
                builder.AppendLine($"<time>{x.Timestamp:o}</time>");
                builder.AppendLine("</trkpt>");
            }

            builder.AppendLine("</trkseg>");
            builder.AppendLine("</trk>");

            builder.AppendLine("</gpx>");
            return builder.ToString();
        }

        public static TrackSummary ToTrackSummary(this IReadOnlyList<DeviceLocation> deviceLocations)
        {
            if (deviceLocations.Count < 1)
                return TrackSummary.Empty;

            var meters = 0d;
            var duration = TimeSpan.Zero;

            var lastDeviceLocation = deviceLocations[0];
            foreach (var deviceLocation in deviceLocations)
            {
                // todo: Убрать после отладки
                if (double.IsNaN(deviceLocation.Latitude))
                    meters += lastDeviceLocation.GetDistanceTo(deviceLocation);
                else
                    meters += lastDeviceLocation.GetDistanceTo(deviceLocation);
                meters += lastDeviceLocation.GetDistanceTo(deviceLocation);
                duration += deviceLocation.Timestamp - lastDeviceLocation.Timestamp;
                lastDeviceLocation = deviceLocation;
            }

            return new TrackSummary(meters, duration);
        }
    }
}
