namespace Binateq.GpsTrackFilter.Viewer.Maps
{
    using System;
    using System.Globalization;

    public class Location : IEquatable<Location>
    {
        public static readonly Location Empty = new(double.NaN, double.NaN);

        public static readonly LocationEqualityComparer EqualityComparer = new();

        public static bool operator ==(Location a, Location b) => EqualityComparer.Equals(a, b);

        public static bool operator !=(Location a, Location b) => !EqualityComparer.Equals(a, b);

        public override bool Equals(object obj) => obj is Location other && EqualityComparer.Equals(this, other);

        public bool Equals(Location other) => EqualityComparer.Equals(this, other);

        public override int GetHashCode() => EqualityComparer.GetHashCode(this);
        
        public override string ToString() => ToString("H");
        
        public string ToString(string format) =>
            format switch
            {
                "M" => string.Format(CultureInfo.InvariantCulture, "({0}{2} {1})", Latitude, Longitude, CultureInfo.InvariantCulture.TextInfo.ListSeparator),
                "W" => string.Format(CultureInfo.InvariantCulture, "POINT({1} {0})", Latitude, Longitude),
                "H" => string.Format(CultureInfo.InvariantCulture, "{0},{1}", Latitude, Longitude),
                _ => throw new FormatException("Unknown format.")
            };

        public static bool TryParse(string s, out Location result)
        {
            result = null;
            if (s == null) 
                return false;
            
            var splits = Array.Empty<string>();

            if (s.StartsWith("(") && s.Length > 2)
            {                
                s = s[1..^1];
                splits = s.Split(CultureInfo.InvariantCulture.TextInfo.ListSeparator);
                if (splits.LongLength != 2L)
                    return false;

                if (double.TryParse(splits[0], out var latitude) && double.TryParse(splits[1], out var longitude))
                {
                    result = new Location(latitude, longitude);
                    return true;
                }
            }
            else if (s.StartsWith("POINT(") && s.Length > 7)
            {
                s = s[6..^1];
                splits = s.Split(' ');
                if (splits.LongLength != 2L)
                    return false;

                if (double.TryParse(splits[0], out var longitude) && double.TryParse(splits[1], out var latitude))
                {
                    result = new Location(latitude, longitude);
                    return true;
                }
            }
            else
            {
                splits = s.Split(',');
                if (splits.LongLength != 2L)
                    return false;

                if (double.TryParse(splits[0], out var latitude) && double.TryParse(splits[1], out var longitude))
                {
                    result = new Location(latitude, longitude);
                    return true;
                }
            }
                            
            return false;
        }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
        
        public double Latitude { get; }

        public double Longitude { get; }
        
        public int GetDistanceTo(Location destination)
        {
            const double earthRadiusInMeters = 6371000;

            static double ToRadians(double degrees) => degrees * Math.PI / 180;

            var φ1 = ToRadians(Latitude);
            var φ2 = ToRadians(destination.Latitude);
            var δφ = ToRadians(destination.Latitude - Latitude);
            var δλ = ToRadians(destination.Longitude - Longitude);

            var a = Math.Sin(δφ / 2) * Math.Sin(δφ / 2) +
                    Math.Cos(φ1) * Math.Cos(φ2) *
                    Math.Sin(δλ / 2) * Math.Sin(δλ / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return Convert.ToInt32(earthRadiusInMeters * c);
        }
        
        public double[] ToArray() => new[] {Longitude, Latitude};

        public static Location FromArray(double[] point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));
            if (point.Length < 1)
                throw new ArgumentException("Parameter cannot be empty", nameof(point));

            return new Location(point[1], point[0]);
        }

        public static bool IsNullOrEmpty(Location location) => location == null || location == Empty;
    }
}