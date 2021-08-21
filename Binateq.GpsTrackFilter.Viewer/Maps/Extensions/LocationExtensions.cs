namespace Binateq.GpsTrackFilter.Viewer.Maps.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class LocationExtensions
    {
        public static Location GetCenter<TLocation>(this IReadOnlyCollection<TLocation> locations) where TLocation : Location
        {
            if (locations == null)
                throw new ArgumentNullException(nameof(locations));

            if (locations.Count == 1)
                return locations.First();

            var x = 0d;
            var y = 0d;
            var z = 0d;

            foreach (var geoCoordinate in locations)
            {
                var latitude = geoCoordinate.Latitude * Math.PI / 180;
                var longitude = geoCoordinate.Longitude * Math.PI / 180;

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            var total = locations.Count;

            x /= total;
            y /= total;
            z /= total;

            var centralLongitude = Math.Atan2(y, x);
            var centralSquareRoot = Math.Sqrt(x * x + y * y);
            var centralLatitude = Math.Atan2(z, centralSquareRoot);

            return new Location(centralLatitude * 180 / Math.PI, centralLongitude * 180 / Math.PI);
        }
    }
}
