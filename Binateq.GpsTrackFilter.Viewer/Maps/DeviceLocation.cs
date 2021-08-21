namespace Binateq.GpsTrackFilter.Viewer.Maps
{
    using System;
    using System.Text.Json.Serialization;

    public class DeviceLocation : Location
    {
        public new static readonly DeviceLocation Empty = new();

        public static DeviceLocation Create(Location location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            return new DeviceLocation(location.Latitude, location.Longitude);
        }

        private DeviceLocation() : this(double.NaN, double.NaN)
        {
        }

        public DeviceLocation(double latitude, double longitude) : base(latitude, longitude)
        {
        }

        [JsonConstructor]
        public DeviceLocation(double latitude, double longitude, double heading, double speed, DateTimeOffset timestamp)
            : this(latitude, longitude)
        {
            Heading = heading;
            Speed = speed;
            Timestamp = timestamp;
        }

        public double Heading { get; }

        public double Speed { get; }

        public DateTimeOffset Timestamp { get; }

        public bool IsUnknown => this == Empty;

        public static bool IsNullOrEmpty(DeviceLocation location) => location == null || location.IsUnknown;
    }
}
