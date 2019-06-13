namespace Binateq.GpsTrackFilter

open System
open Microsoft.FSharp.Collections
open Types
open Filters
open Kalman
open System.Collections.Generic

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

/// <summary>
/// Implements a few methods to fix bad GPS data.
/// </summary>
type GpsTrackFilter() =
    let mutable zeroSpeedDrift = 7.99
    let mutable outlineSpeed = 110.0
    let mutable modelPrecision = 2.13
    let mutable sensorPrecision = 0.77


    /// <summary>
    /// Gets or sets the minimal valid velocity for the filter of zero speed drift.
    /// </summary>
    member __.ZeroSpeedDrift with get () = zeroSpeedDrift
                                and set value = zeroSpeedDrift <- value

    
    /// <summary>
    /// Gets or sets the maximal valid velocity for the filter of outline speed.
    /// </summary>
    member __.OutlineSpeed with get () = outlineSpeed
                              and set value = outlineSpeed <- value

    
    /// <summary>
    /// Gets or sets the precision of the moving model of the Kalman's filter.
    /// </summary>
    member __.ModelPrecision with get () = modelPrecision
                                and set value = modelPrecision <- value

    
    /// <summary>
    /// Gets or sets the precision of the GPS sensor of the Kalman's filter.
    /// </summary>
    member __.SensorPrecision with get () = sensorPrecision
                                 and set value = sensorPrecision <- value

    
    /// <summary>
    /// Fixes a GPS track.
    /// <summary>
    /// <param name="points">Source GPS track with possible bad data.</param>
    /// <returns>
    /// Fixed track.
    /// </returns>
    member __.Filter(points: seq<Location>): IReadOnlyList<Location> =
           points
        |> Seq.map (fun x -> SensorItem(x.Latitude, x.Longitude, 0.0, 0.0, x.Timestamp))
        |> List.ofSeq
        |> removeZeroOrNegativeTimespans
        |> removeZeroSpeedDrift zeroSpeedDrift
        |> removeOutlineSpeedValues outlineSpeed
        |> smoothBySimplifiedKalman modelPrecision sensorPrecision
        |> List.map (fun x -> Location(x.Latitude, x.Longitude, x.Timestamp))
        :> IReadOnlyList<Location>
    

    /// <summary>
    /// Fixes a GPS track.
    /// <summary>
    /// <param name="points">Source GPS track with possible bad data.</param>
    /// <returns>
    /// Fixed track.
    /// </returns>
    member __.Filter(points: seq<(float * float * DateTimeOffset)>): IReadOnlyList<(float * float * DateTimeOffset)> =
           points
        |> Seq.map (fun (latitude, longitude, timestamp) -> SensorItem(latitude, longitude, 0.0, 0.0, timestamp))
        |> List.ofSeq
        |> removeZeroOrNegativeTimespans
        |> removeZeroSpeedDrift zeroSpeedDrift
        |> removeOutlineSpeedValues outlineSpeed
        |> smoothBySimplifiedKalman modelPrecision sensorPrecision
        |> List.map (fun x -> (x.Latitude, x.Longitude, x.Timestamp))
        :> IReadOnlyList<(float * float * DateTimeOffset)>


    /// <summary>
    /// Fixes a GPS track.
    /// <summary>
    /// <param name="points">Source GPS track with possible bad data.</param>
    /// <returns>
    /// Fixed track.
    /// </returns>
    member __.Filter(points: seq<DirectedLocation>): IReadOnlyList<Location> =
           points
        |> Seq.map (fun x -> SensorItem(x.Latitude, x.Longitude, x.Speed, x.Heading, x.Timestamp))
        |> List.ofSeq
        |> removeZeroOrNegativeTimespans
        |> removeZeroSpeedDrift zeroSpeedDrift
        |> removeOutlineSpeedValues outlineSpeed
        |> smoothByKalman modelPrecision sensorPrecision
        |> List.map (fun x -> Location(x.Latitude, x.Longitude, x.Timestamp))
        :> IReadOnlyList<Location>
