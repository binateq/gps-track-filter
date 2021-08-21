namespace Binateq.GpsTrackFilter.Viewer.Maps
{
    using System;
    using Binateq.GpsTrackFilter;
    using SystemJsonConstructor = System.Text.Json.Serialization.JsonConstructorAttribute;
    using NewtonsoftJsonConstructor = Newtonsoft.Json.JsonConstructorAttribute;

    public class IndexedLocation : DeviceLocation
    {        
        public IndexedLocation(double latitude, double longitude, int index)
            : base(latitude, longitude)
        {
            Index = index;
        }

        [SystemJsonConstructor]
        [NewtonsoftJsonConstructor]
        public IndexedLocation(double latitude, double longitude, double heading, double speed, DateTimeOffset timestamp, int index) 
            : base(latitude, longitude, heading, speed, timestamp)
        {
            Index = index;
        }

        public int Index { get; set; }

        public DirectedLocation ToDirectedLocation()
        {
            return new(Latitude, Longitude, Speed, Heading, Timestamp);
        }        
    }
}