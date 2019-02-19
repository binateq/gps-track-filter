module Locations

open System

type ILocation =
    abstract member Latitude: float
    abstract member Longitude: float
    abstract member Timestamp: DateTimeOffset

type IDirectedLocation =
    inherit ILocation
    abstract member Speed: float
    abstract member Heading: float
    
[<Struct>]
type Location(latitude: float, longitude: float, timestamp: DateTimeOffset) =
    member __.Latitude = latitude
    member __.Longitude = longitude
    member __.Timestamp = timestamp

    interface ILocation with
        member this.Latitude = this.Latitude
        member this.Longitude = this.Longitude
        member this.Timestamp = this.Timestamp

[<Struct>]
type DirectedLocation(latitude: float, longitude: float, speed: float, heading: float, timestamp: DateTimeOffset) =
    member __.Latitude = latitude
    member __.Longitude = longitude
    member __.Timestamp = timestamp
    member __.Speed = speed
    member __.Heading = heading

    interface IDirectedLocation with
        member this.Latitude = this.Latitude
        member this.Longitude = this.Longitude
        member this.Timestamp = this.Timestamp
        member this.Speed = this.Speed
        member this.Heading = this.Heading
