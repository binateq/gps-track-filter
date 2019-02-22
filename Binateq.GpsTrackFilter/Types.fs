module Types

open System

[<Struct>]
type internal SensorItem(latitude: float, longitude: float, speed: float, heading: float, timestamp: DateTimeOffset) =
    member __.Latitude = latitude
    member __.Longitude = longitude
    member __.Timestamp = timestamp
    member __.Speed = speed
    member __.Heading = heading
