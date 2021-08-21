namespace Binateq.GpsTrackFilter.Viewer.Maps
{
    using System;
    using System.Collections.Generic;

    public class LocationEqualityComparer : IEqualityComparer<Location>
    {
        public bool Equals(Location x, Location y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Latitude.Equals(y.Latitude) && x.Longitude.Equals(y.Longitude);
        }

        public int GetHashCode(Location obj) => HashCode.Combine(obj.Latitude, obj.Longitude);
    }
}
