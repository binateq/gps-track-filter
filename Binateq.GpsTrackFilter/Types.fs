module Types

open System

type internal SensorItem(latitude: float, longitude: float, speed: float, heading: float, timestamp: DateTimeOffset) =
    member __.Latitude = latitude
    member __.Longitude = longitude
    member __.Timestamp = timestamp
    member __.Speed = speed
    member __.Heading = heading

    override __.GetHashCode() =
        hash (latitude, longitude, timestamp, speed, heading)

    override __.Equals(other) =
        match other with
        | :? SensorItem as x -> (latitude, longitude, timestamp, speed, heading) = (x.Latitude, x.Longitude, x.Timestamp, x.Speed, x.Heading)
        | _ -> false
