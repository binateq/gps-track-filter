module Kalman

open Locations

/// <summary>
/// Filters points based on specified standard deviations with simplified Kalman filter.
/// </summary>
/// <remarks>
/// http://david.wf/kalmanfilter/
/// </remarks>
/// <param name="σξ">Sigma ksi. Standard deviation of the moving model.</param>
/// <param name="ση">Sigma eta. Standard deviation of the GPS sensor.</param>
/// <param name="points">Points after f.</param>
let smoothBySimplifiedKalman σξ ση (points: Location list) =
  let rec recursive_filter error (p1: Location) points =
    let square x = x * x
    let correct_error error =
      let numerator = square ση * (square error + square σξ)
      let denominator = square ση + square error + square σξ
    
      sqrt numerator/denominator

    match points with
    | (p2: Location)::points ->
      let next_latitude_error = correct_error (fst error)
      let next_longitude_error = correct_error (snd error)

      let K_latitude = square next_latitude_error/square ση
      let K_longitude = square next_longitude_error/square ση

      let next_latitude = (1.0 - K_latitude) * p1.Latitude + K_latitude * p2.Latitude
      let next_longitude = (1.0 - K_longitude) * p1.Longitude + K_longitude * p2.Longitude

      let next_error = (next_latitude_error, next_longitude_error)
      let next_point = Location(next_latitude, next_longitude, p2.Timestamp)

      next_point::(recursive_filter next_error next_point points)

    | _ -> points
    
  match points with
  | [] -> []
  | p1::points -> p1::(recursive_filter (ση, ση) p1 points)
