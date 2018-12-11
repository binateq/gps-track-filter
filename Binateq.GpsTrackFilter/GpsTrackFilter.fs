namespace Binateq.GpsTrackFilter

open System
open System.Collections.Generic
open Stabilizators
open Filters

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
    /// Gets or sets the precision of the GPS tracker of the Kalman's filter.
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
    member this.Fix(points: IEnumerable<(float * float * DateTimeOffset)>): IEnumerable<(float * float * DateTimeOffset)>
        = List.ofSeq points
       |> removeZeroOrNegativeTimespans
       |> replaceZeroSpeedDrift zeroSpeedDrift
       |> removeOutlineSpeedValues outlineSpeed
       |> filterBySimplifyKalman modelPrecision sensorPrecision
       |> List.toSeq
