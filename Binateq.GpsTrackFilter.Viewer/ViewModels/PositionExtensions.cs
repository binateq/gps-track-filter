namespace Binateq.GpsTrackFilter.Viewer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MapControl;

    public static class PositionExtensions
    {
        private const double EquatorRadius = 63781.37;

        public static double DistanceBetween(this Location a, Location b)
        {
            var distance = Math.Acos(Math.Sin(a.Latitude) * Math.Sin(b.Latitude) +
                                     Math.Cos(a.Latitude) * Math.Cos(b.Latitude) * Math.Cos(b.Longitude - a.Longitude));

            return EquatorRadius * distance;
        }

        public static Location Center(this IReadOnlyCollection<Location> locations)
        {
            if (locations.Count == 1)
                return locations.Single();

            double x = 0;
            double y = 0;
            double z = 0;

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

        public static Location Center(this Location a, Location b)
        {
            return Center(new[] { a, b });
        }
    }
}


