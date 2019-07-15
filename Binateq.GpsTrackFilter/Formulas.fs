module Formulas

open System
open Types

/// <summary>
/// Calculates radians from degrees.
/// </summary>
/// <param name="x">An angle in degrees.</param>
let internal radian x = x * Math.PI/180.0


/// <summary>
/// Calculates the distance in kilometers between two geographic points.
/// </summary>
/// <returns>The distance in kilometers.</returns>
let internal distance latitude1 longitude1 latitude2 longitude2 =
    // https://en.wikipedia.org/wiki/Haversine_formula
    let hav x = sin(x/2.0) ** 2.0
    
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
/// Calculates the velocity in kilometers per hour by coordinates and timestamps.
/// </summary>
let internal velocity (p1: SensorItem) (p2: SensorItem) =
    let Δtime = (p2.Timestamp - p1.Timestamp).TotalHours
    let Δdistance = distance p1.Latitude p1.Longitude p2.Latitude p2.Longitude

    Δdistance/Δtime


/// <summary>
/// Calcualates projection of velocity vector on specified meridian and parallel.
/// </summary>
/// <param name="latitude">The latitude of parallel to projection.</param>
/// <param name="longitude">The longitude of meridian to projection.</param>
/// <param name="speed">The velocity module in meters per second.</param>
/// <param name="heading">The velocity direction relatied to North in degrees.</param>
let internal project latitude longitude speed heading =
    let cartesianAngleFromHeading =
        let flipHorizontal angle = 360.0 - angle
        let rotateClockwise90 angle = (270.0 + angle) % 360.0
        
        flipHorizontal >> rotateClockwise90

    let angle = radian (cartesianAngleFromHeading heading)

    let kilometersPerHour metersPerSecond = 3.6 * metersPerSecond
    let velocity = kilometersPerHour speed

    let velocityLongitude = velocity * cos angle
    let velocityLatitude = velocity * sin angle

    let kilometersPerLongitudeGrade = distance latitude longitude latitude (longitude + 1.0)
    let kilometersPerLatitudeGrade = distance latitude longitude (latitude + 1.0) longitude

    (velocityLatitude / kilometersPerLatitudeGrade, velocityLongitude / kilometersPerLongitudeGrade)
