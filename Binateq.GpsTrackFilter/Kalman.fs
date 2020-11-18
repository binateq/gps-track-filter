module Kalman

open System
open Types
open Formulas


/// <summary>
/// Calculates Kalman's coefficient for next iteration of Kalman's filter.
/// </summary>
/// <param name="modelVariance">Square of sigma ksi. Variance of moving model.</param>
/// <param name="sensorVariance">Square of sigma eta. Variance of sensor.</param>
/// <param name="squareError">Square of error of current iteration.</param>
/// <returns>Kalman's coefficient for next iteration.</returns>
let internal calculateNextK modelVariance sensorVariance squareError =
    (squareError + modelVariance)/(squareError + modelVariance + sensorVariance)


/// <summary>
/// Calculates square of error for next iteration of Kalman's filter.
/// </summary>
/// <param name="modelVariance">Square of sigma ksi. Variance of moving model.</param>
/// <param name="sensorVariance">Square of sigma eta. Variance of sensor.</param>
/// <param name="squareError">Square of error of current iteration.</param>
/// <returns>Square of error for next iteration.</returns>
let internal calculateNextSquareError modelVariance sensorVariance squareError =
    (sensorVariance * (squareError + modelVariance))/(sensorVariance + squareError + modelVariance)


/// <summary>
/// Calculates value between two known values.
/// </summary>
/// <param name="k">Interpolation coefficient in interval [0; 1].</param>
/// <param name="value1">First value.</param>
/// <param name="value2">Second value.</param>
/// <returns>Interpolated value.<returns>. 
let internal interpolate k value1 value2 =
  k * value1 + (1.0 - k) * value2


let internal nextPointWithVelocity modelVariance
                                   sensorVariance
                                   (latitude, longitude, timestamp: DateTimeOffset, squareError)
                                   (p: SensorItem) =
    let K = calculateNextK modelVariance sensorVariance squareError
    let nextSquareError = calculateNextSquareError modelVariance sensorVariance squareError
    let deltaTime = (p.Timestamp - timestamp).TotalHours
    let (latitudeVelocity, longitudeVelocity) = project latitude longitude p.Speed p.Heading
    let nextLatitude = interpolate K p.Latitude (latitude + latitudeVelocity * deltaTime)
    let nextLongitude = interpolate K p.Longitude (longitude + longitudeVelocity * deltaTime)

    (nextLatitude, nextLongitude, p.Timestamp, nextSquareError)


let internal nextPoint modelVariance
                       sensorVariance
                       (latitude, longitude, _, squareError)
                       (p: SensorItem) =
    let K = calculateNextK modelVariance sensorVariance squareError
    let nextSquareError = calculateNextSquareError modelVariance sensorVariance squareError
    let nextLatitude = K * p.Latitude + (1.0 - K) * latitude
    let nextLongitude = K * p.Longitude + (1.0 - K) * longitude

    (nextLatitude, nextLongitude, p.Timestamp, nextSquareError)


let internal toSensorItem (latitude, longitude, timestamp, _) =
  { Latitude = latitude
    Longitude = longitude
    Speed = 0.0
    Heading = 0.0
    Timestamp = timestamp }


/// <summary>
/// Smooths track with Kalman filter.
/// </summary>
/// <remarks>
/// http://david.wf/kalmanfilter/
/// </remarks>
/// <param name="algorithm">Method of calculating (with or without velocity).</param>
/// <param name="modelError">Sigma ksi. Standard deviation of the moving model.</param>
/// <param name="sensorError">Sigma eta. Standard deviation of the GPS sensor.</param>
/// <param name="points">Track after smoothing.</param>
let internal smoothByKalman algorithm modelError sensorError (points: SensorItem list) =
    let modelVariance = modelError ** 2.0
    let sensorVariance = sensorError ** 2.0

    match points with
    | [] -> points
    | _::[] -> points
    | p1::ps -> let init = (p1.Latitude, p1.Longitude, p1.Timestamp, sensorVariance)
                let smoothed = ps
                            |> List.scan (algorithm modelVariance sensorVariance) init
                            |> List.map toSensorItem
        
                p1::smoothed
