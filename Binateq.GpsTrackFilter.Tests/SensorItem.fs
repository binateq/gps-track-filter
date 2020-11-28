namespace Binateq.GpsTrackFilter.Tests

module SensorItem =
    open Types
    open System


    let internal sensorItem latitude longitude dateTimeOffset =
        { Latitude = latitude
          Longitude = longitude
          Speed = 0.0
          Heading = 0.0
          Timestamp = DateTimeOffset.Parse(dateTimeOffset) }

