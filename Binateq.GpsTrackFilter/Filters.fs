module Filters

open System
open Locations

/// <summary>
/// Calculates the distance in kilometers between two geographic points.
/// </summary>
/// <returns>The distance in kilometers.</returns>
let distance latitude1 longitude1 latitude2 longitude2 =
    let radian x = x * Math.PI/180.0

    // https://en.wikipedia.org/wiki/Haversine_formula
    let hav x = Math.Sin(x/2.0) ** 2.0
    
    // https://en.wikipedia.org/wiki/World_Geodetic_System
    let earthEquatorialRadius = 6378.137
    let earthPolarRadius = 6356.752
    let averageEarthRadius = (earthEquatorialRadius + earthPolarRadius)/2.0

    let φ1 = radian latitude1
    let λ1 = radian longitude1

    let φ2 = radian latitude2
    let λ2 = radian longitude2

    let h = hav (φ2 - φ1) + cos φ1 * cos φ2 * hav (λ2 - λ1)

    2.0 * asin (sqrt h) * averageEarthRadius


/// <summary>
/// Calculates the velocity by coordinates and timestamp.
/// </summary>
let velocity<'Location when 'Location :> ILocation> (p1: 'Location) (p2: 'Location) =
    let Δtime = (p2.Timestamp - p1.Timestamp).TotalHours
    let Δdistance = distance p1.Latitude p1.Longitude p2.Latitude p2.Longitude

    Δdistance/Δtime


/// <summary>
/// Removes points matching with specified predicate.
/// </summary>
/// <param name="predicate"></param>
/// <param name="points"></param>
let remove predicate points =
    match points with
    | [] -> []
    | p1::_ -> let filtered = points
                           |> List.pairwise
                           |> List.filter (fun (p1, p2) -> not (predicate p1 p2))
                           |> List.map (fun (_, p2) -> p2)
               p1::filtered


/// <summary>
/// Removes points with zero or negative time spans.
/// </summary>
let removeZeroOrNegativeTimespans<'Location when 'Location :> ILocation> (points: 'Location list) =
    let isZeroOrNegativeTimespan (p1: 'Location) (p2: 'Location) =
        let Δtime = p2.Timestamp - p1.Timestamp

        Δtime <= TimeSpan.Zero

    remove isZeroOrNegativeTimespan points


/// <summary>
/// Removes points with outlined speed.
/// </summary>
let removeOutlineSpeedValues hiLimit points =
    let isOutlineSpeed p1 p2 =
        let velocity = velocity p1 p2

        velocity > hiLimit

    remove isOutlineSpeed points

/// <summary>
/// Replaces zero speed drift to zero.
/// </summary>
let replaceZeroSpeedDrift<'Location when 'Location :> ILocation> loLimit (points: 'Location list) =
    let isZeroDriftSpeed p1 p2 =
        let velocity = velocity p1 p2

        velocity < loLimit

    remove isZeroDriftSpeed points
