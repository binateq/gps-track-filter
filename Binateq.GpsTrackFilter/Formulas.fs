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
/// Calculates days before specified date.
/// </summary>
/// <param name="year">Year.</param>
/// <param name="month">Month.</param>
/// <param name="day">Day.</param>
/// <returns>The count of days before the date.</returns>
let internal daysBeforeDate year month day =
    let daysBeforeMonth month = match month with
        | 1 -> 0
        | 2 -> 31
        | 3 -> 59
        | 4 -> 90
        | 5 -> 120
        | 6 -> 151
        | 7 -> 181
        | 8 -> 212
        | 9 -> 243
        | 10 -> 273
        | 11 -> 304
        | 12 -> 334
        | _ -> raise (ArgumentException("Invalid month."))

    let daysBeforeYear year = (year - 1) * 365 + (year - 1)/4 - (year - 1)/100 + (year - 1)/400

    let isLeap year = year % 4 = 0 && ((year % 100 <> 0) || (year % 400 = 0))
    let daysBefore = day + daysBeforeMonth month + daysBeforeYear year - 1

    if isLeap year
    then daysBefore + 1
    else daysBefore


let internal secondsAtDateTime year month day hour minute second =
    let days = daysBeforeDate year month day
    
    (24L * 3600L * int64(days)) + 3600L * int64(hour) + 60L * int64(minute) + int64(second)


/// <summary>
/// Calculates the velocity in kilometers per hour by coordinates and timestamps.
/// </summary>
let internal velocity (p1: SensorItem) (p2: SensorItem) =
    // Fix the bag at Android's Mono:
    //    System.ArgumentOutOfRangeException: Ticks must be between DateTime.MinValue.Ticks and DateTime.MaxValue.Ticks.
    //                                        Parameter name: ticks
    // at System.DateTime..ctor (System.Int64 ticks, System.DateTimeKind kind)
    // at System.DateTime.SpecifyKind (System.DateTime value, System.DateTimeKind kind)
    // at System.DateTimeOffset.get_UtcDateTime ()
    // at System.DateTimeOffset.op_Subtraction (System.DateTimeOffset left, System.DateTimeOffset right)
    // at Formulas.velocity(Types+SensorItem p1, Types+SensorItem p2)
    // let Δtime = (p2.Timestamp - p1.Timestamp).TotalHours
    let secondsAt (x: DateTimeOffset) =
        secondsAtDateTime x.Year x.Month x.Day x.Hour x.Minute x.Second
    let Δseconds = float(secondsAt p2.Timestamp - secondsAt p1.Timestamp)
    let Δtime = TimeSpan.FromSeconds(Δseconds).TotalHours
    let Δdistance = distance p1.Latitude p1.Longitude p2.Latitude p2.Longitude

    Δdistance/Δtime


/// <summary>
/// Calcualates projection of velocity vector on specified meridian and parallel.
/// </summary>
/// <param name="latitude">The latitude of parallel to projection.</param>
/// <param name="longitude">The longitude of meridian to projection.</param>
/// <param name="speed">The velocity module in meters per second.</param>
/// <param name="heading">The velocity direction relatied to North in degrees.</param>
let internal projectSpeedOnAxis latitude longitude speed heading =
    let cartesianAngleFromHeading =
        let flip angle = 360.0 - angle
        let rotateClockwise90 angle = (270.0 + angle) % 360.0
        
        flip >> rotateClockwise90

    let velocityAngle = radian (cartesianAngleFromHeading heading)

    let kilometersPerHour metersPerSecond = 3.6 * metersPerSecond
    let velocityKmph = kilometersPerHour speed

    let velocityKmphLongitude = velocityKmph * cos velocityAngle
    let velocityKmphLatitude = velocityKmph * sin velocityAngle

    let kilometersPerLongitudeGrade = distance latitude longitude latitude (longitude + 1.0)
    let kilometersPerLatitudeGrade = distance latitude longitude (latitude + 1.0) longitude

    (velocityKmphLatitude / kilometersPerLatitudeGrade, velocityKmphLongitude / kilometersPerLongitudeGrade)
