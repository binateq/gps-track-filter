module Kalman

open System
open Types
open Formulas

/// <summary>
/// Filters points based on specified standard deviations with simplified Kalman filter.
/// </summary>
/// <remarks>
/// http://david.wf/kalmanfilter/
/// </remarks>
/// <param name="modelError">Sigma ksi. Standard deviation of the moving model.</param>
/// <param name="sensorError">Sigma eta. Standard deviation of the GPS sensor.</param>
/// <param name="points">Points after filtration.</param>
let internal smoothBySimplifiedKalman modelError sensorError (points: SensorItem list) =
  let rec recursive_filter error (p1: SensorItem) points =
    let correct_error error =
      let numerator = sensorError ** 2.0 * (error ** 2.0 + modelError ** 2.0)
      let denominator = sensorError ** 2.0 + error ** 2.0 + modelError ** 2.0
    
      sqrt numerator/denominator

    match points with
    | (p2: SensorItem)::points ->
      let next_latitude_error = correct_error (fst error)
      let next_longitude_error = correct_error (snd error)

      let K_latitude = next_latitude_error ** 2.0/sensorError ** 2.0
      let K_longitude = next_longitude_error ** 2.0/sensorError ** 2.0

      let next_latitude = (1.0 - K_latitude) * p1.Latitude + K_latitude * p2.Latitude
      let next_longitude = (1.0 - K_longitude) * p1.Longitude + K_longitude * p2.Longitude

      let next_error = (next_latitude_error, next_longitude_error)
      let next_point = { Latitude = next_latitude
                         Longitude = next_longitude
                         Speed = 0.0
                         Heading = 0.0
                         Timestamp = p2.Timestamp }

      next_point::(recursive_filter next_error next_point points)

    | _ -> points
    
  match points with
  | [] -> []
  | p1::points -> p1::(recursive_filter (sensorError, sensorError) p1 points)


let internal stepKalman modelVariance sensorVariance
                       (latitude, longitude, timestamp: DateTimeOffset, error) (p: SensorItem) =
    let a = error + modelVariance
    let b = sensorVariance
    let K = a/(a + b)
    let nextError = (a * b)/(a + b)
    let deltaTime = (p.Timestamp - timestamp).TotalHours
    let (latitudeVelocity, longitudeVelocity) = project latitude longitude p.Speed p.Heading
    let nextLatitude = K * p.Latitude + (1.0 - K) * (latitude + latitudeVelocity * deltaTime)
    let nextLongitude = K * p.Longitude + (1.0 - K) * (longitude + longitudeVelocity * deltaTime)

    (nextLatitude, nextLongitude, p.Timestamp, nextError)


/// <summary>
/// Filters points based on specified standard deviations with Kalman filter.
/// </summary>
/// <remarks>
/// http://david.wf/kalmanfilter/
/// </remarks>
/// <param name="modelError">Sigma ksi. Standard deviation of the moving model.</param>
/// <param name="sensorError">Sigma eta. Standard deviation of the GPS sensor.</param>
/// <param name="points">Points after filtration.</param>
let internal smoothByKalman modelError sensorError (points: SensorItem list) =
    let ξVariance = modelError ** 2.0
    let ηVariance = sensorError ** 2.0

    match points with
    | [] -> []
    | p1::ps -> let first = (p1.Latitude, p1.Longitude, p1.Timestamp, ηVariance)
                let smoothed = ps
                            |> List.scan (stepKalman ξVariance ηVariance) first
                            |> List.map (fun (latitude, longitude, timestamp, _) -> { Latitude = latitude
                                                                                      Longitude = longitude
                                                                                      Speed = 0.0
                                                                                      Heading = 0.0
                                                                                      Timestamp = timestamp })
        
                { Latitude = p1.Latitude
                  Longitude = p1.Longitude
                  Speed = 0.0
                  Heading = 0.0
                  Timestamp = p1.Timestamp}::smoothed
