namespace Binateq.GpsTrackFilter.Viewer
{
    using System;
    
    public class TrackSummary
    {
        public static readonly TrackSummary Empty = new(0d, TimeSpan.Zero);

        public TrackSummary(double meters, TimeSpan duration)
        {
            Meters = meters;
            Duration = duration;
        }

        public double Meters { get; }
        public TimeSpan Duration { get; }
    }
}
