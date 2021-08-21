namespace Binateq.GpsTrackFilter.Viewer
{
    public class Tracks
    {
        public string Raw { get; set; }
        public TrackSummary RawSummary { get; set; }
        public string Filtered { get; set; }
        public TrackSummary FilteredSummary { get; set; }
    }
}
