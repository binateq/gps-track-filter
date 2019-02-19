module Stabilizators

open System
open Locations

/// <summary>
/// Calculates the distance in kilometers between two geographic points.
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
    if List.isEmpty points then points
    else
        let p1 = List.head points
        p1::(points |> List.pairwise
                    |> List.filter (fun (p1, p2) -> not (predicate p1 p2))
                    |> List.map (fun (_, p2) -> p2))


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

//let replaceZeroSpeedDrift2<'Location when 'Location :> ILocation> loLimit (points: 'Location list) =
//    let rec replace p1 points =
//        match points with
//        | p2::points ->
//            let velocity = velocity p1 p2
//            let latitude1, longitude1, _ = p1
//            let _, _, timestamp2 = p2

//            if velocity < loLimit
//            then L(p1.Latitude, longitude1, timestamp2)::(replace p1 points)
//            else p2::(replace p2 points)

//        | _ -> points

//    match points with
//    | p1::points -> p1::(replace p1 points)
//    | _ -> points


//let remove2 predicate points =
//    let rec compareAndRemove p1 points =
//        match points with
//        | p2::points ->
//            if (predicate p1 p2)
//            then compareAndRemove p1 points
//            else p2::(compareAndRemove p2 points)

//        | _ -> points

//    match points with
//    | p1::points -> p1::(compareAndRemove p1 points)
//    | _ -> points
