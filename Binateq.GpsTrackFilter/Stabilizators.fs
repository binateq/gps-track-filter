module Stabilizators

open System

/// <summary>
/// Calculates the distance in kilometers between two geographics points.
/// </summary>
/// <returns>The distance in kilometers.</returns>
let distance latitude1 longitude1 latitude2 longitude2 =
    let square x = x * x
    let radian x = x * Math.PI/180.0

    // https://en.wikipedia.org/wiki/Haversine_formula
    let hav x = square (Math.Sin(x/2.0))
    
    // https://en.wikipedia.org/wiki/World_Geodetic_System
    let earthEquatorialRadius = 6378.137
    let earthPolarRadius = 6356.752
    let averageEarthRadius = (earthEquatorialRadius + earthPolarRadius)/2.0

    let φ1 = radian latitude1
    let λ1 = radian longitude1

    let φ2 = radian latitude2
    let λ2 = radian longitude2

    let h = hav (φ2 - φ1) + cos φ1 * cos φ2 * hav (λ2 - λ1)
    2.0 * averageEarthRadius * asin (sqrt h)


/// <summary>
/// Calculates the velocity by coordinates and timestamp.
/// </summary>
let velocity (p1: float * float * DateTimeOffset) p2 =
    let latitude1, longitude1, timestamp1 = p1
    let latitude2, longitude2, timestamp2 = p2
    let Δtime = (timestamp2 - timestamp1).TotalHours
    let Δdistance = distance latitude1 longitude1 latitude2 longitude2

    Δdistance/Δtime


/// <summary>
/// Removes points matching with specified predicate.
/// </summary>
/// <param name="predicate"></param>
/// <param name="points"></param>
let remove predicate points =
    let rec compareAndRemove p1 points =
        match points with
        | p2::points ->
            if (predicate p1 p2)
            then compareAndRemove p1 points
            else p2::(compareAndRemove p2 points)

        | _ -> points

    match points with
    | p1::points -> p1::(compareAndRemove p1 points)
    | _ -> points


/// <summary>
/// Removes points with zero or negative timespans.
/// </summary>
let removeZeroOrNegativeTimespans (points: (float * float * DateTimeOffset) list) =
    let isZeroOrNegativeTimespan p1 p2 =
        let _, _, timestamp1 = p1
        let _, _, timestamp2 = p2
        let Δtime = timestamp2 - timestamp1

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
let replaceZeroSpeedDrift loLimit points =
    let rec replace p1 points =
        match points with
        | p2::points ->
            let velocity = velocity p1 p2
            let latitude1, longitude1, _ = p1
            let _, _, timestamp2 = p2

            if velocity < loLimit
            then (latitude1, longitude1, timestamp2)::(replace p1 points)
            else p2::(replace p2 points)

        | _ -> points

    match points with
    | p1::points -> p1::(replace p1 points)
    | _ -> points