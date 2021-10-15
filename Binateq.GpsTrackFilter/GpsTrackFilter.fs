namespace Binateq.GpsTrackFilter

open System
open Microsoft.FSharp.Collections
open Types
open Filters
open Kalman
open System.Collections.Generic

/// <summary>
/// Describes GPS track point with latitude, longitude, and timestamp.
/// </summary>
[<Struct>]
type Location(latitude: float, longitude: float, timestamp: DateTimeOffset) =
    /// <summary>
    /// Latitude from -90 to +90 degrees.
    /// </summary>
    member __.Latitude = latitude

    /// <summary>
    /// Longitude from -180 to +180 degrees.
    /// </summary>
    member __.Longitude = longitude

    /// <summary>
    /// Date and time of measure.
    /// </summary>
    member __.Timestamp = timestamp


/// <summary>
/// Describes GPS track point with latitude, longitude, speed, heading, and timestamp.
/// </summary>
[<Struct>]
type DirectedLocation(latitude: float, longitude: float, speed: float, heading: float, timestamp: DateTimeOffset) =
    /// <summary>
    /// Latitude from -90 to +90 degrees.
    /// </summary>
    member __.Latitude = latitude

    /// <summary>
    /// Longitude from -180 to +180 degrees.
    /// </summary>
    member __.Longitude = longitude

    /// <summary>
    /// Speed in meters per second.
    /// </summary>
    member __.Speed = speed

    /// <summary>
    /// Speed direction related to North in degrees. Clockwise is positive.
    /// </summary>
    member __.Heading = heading

    /// <summary>
    /// Date and time of measure.
    /// </summary>
    member __.Timestamp = timestamp


/// <summary>
/// Implements a methods of fixation of GPS tracks.
/// </summary>
type GpsTrackFilter() =
    let mutable zeroSpeedDrift = 7.99
    let mutable outlineSpeed = 110.0
    let mutable modelPrecision = 2.13
    let mutable sensorPrecision = 0.77

    
    let fromLocation (x: Location) =
        { Latitude = x.Latitude
          Longitude = x.Longitude
          Speed = 0.0
          Heading = 0.0
          Timestamp = x.Timestamp }
        
        
    let toLocation (x: SensorItem) =
        Location(x.Latitude, x.Longitude, x.Timestamp)


    let fromTuple (latitude, longitude, timestamp: DateTimeOffset) =
        { Latitude = latitude
          Longitude = longitude
          Speed = 0.0
          Heading = 0.0
          Timestamp = timestamp }
        
        
    let toTuple (x: SensorItem) =
        (x.Latitude, x.Longitude, x.Timestamp)

        
    let fromDirectedLocation (x: DirectedLocation) =
        { Latitude = x.Latitude
          Longitude = x.Longitude
          Speed = x.Speed
          Heading = x.Heading
          Timestamp = x.Timestamp }
        
        
    let filterValidPoints points =
        points
        |> List.ofSeq
        |> removeNotNumbers
        |> removeZeroOrNegativeTimespans
        |> removeZeroSpeedDrift zeroSpeedDrift
        |> removeOutlineSpeedValues outlineSpeed


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
    /// </summary>
    /// <param name="points">Source GPS track with possible bad data.</param>
    /// <returns>
    /// Fixed track.
    /// </returns>
    member __.Filter(points: seq<Location>): IReadOnlyList<Location> =
           points
        |> Seq.map fromLocation
        |> filterValidPoints
        |> smoothByKalman nextPoint modelPrecision sensorPrecision
        |> List.map toLocation
        :> IReadOnlyList<Location>
    

    /// <summary>
    /// Fixes a GPS track.
    /// </summary>
    /// <param name="points">Source GPS track with possible bad data.</param>
    /// <returns>
    /// Fixed track.
    /// </returns>
    member __.Filter(points: seq<(float * float * DateTimeOffset)>): IReadOnlyList<(float * float * DateTimeOffset)> =
           points
        |> Seq.map fromTuple
        |> filterValidPoints
        |> smoothByKalman nextPoint modelPrecision sensorPrecision
        |> List.map toTuple
        :> IReadOnlyList<(float * float * DateTimeOffset)>


    /// <summary>
    /// Fixes a GPS track.
    /// </summary>
    /// <param name="points">Source GPS track with possible bad data.</param>
    /// <returns>
    /// Fixed track.
    /// </returns>
    member __.Filter(points: seq<DirectedLocation>): IReadOnlyList<Location> =
           points
        |> Seq.map fromDirectedLocation
        |> filterValidPoints
        |> smoothByKalman nextPointWithVelocity modelPrecision sensorPrecision
        |> List.map toLocation
        :> IReadOnlyList<Location>
