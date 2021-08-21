namespace Binateq.GpsTrackFilter.Viewer
{
    using System;
    using System.Collections.Generic;
    using Maps;

    public class Track
    {
        public long Id { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public IReadOnlyList<IndexedLocation> Locations { get; set; }
    }
}
