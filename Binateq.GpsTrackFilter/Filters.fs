module Filters

/// <summary>
/// Filters points based on specified standard deviations with simplified Kalman filter.
/// </summary>
/// <remarks>
/// http://david.wf/kalmanfilter/
/// </remarks>
/// <param name="σξ">Sigma ksi. Standard deviation of the moving model.</param>
/// <param name="ση">Sigma eta. Standard deviation of the GPS sensor.</param>
/// <param name="points">Points after f.</param>
let filterBySimplifyKalman σξ ση points =
  let rec recursive_filter error p1 points =
    let square x = x * x
    let correct_error error =
      let numerator = square ση * (square error + square σξ)
      let denominator = square ση + square error + square σξ
    
      sqrt numerator/denominator

    match p1, points with
    | (latitude, longitude, _), (raw_latitude, raw_longitude, raw_timespan)::points ->
      let next_latitude_error = correct_error (fst error)
      let next_longitude_error = correct_error (snd error)

      let K_latitude = square next_latitude_error/square ση
      let K_longitude = square next_longitude_error/square ση

      let next_latitude = (1.0 - K_latitude) * latitude + K_latitude * raw_latitude
      let next_longitude = (1.0 - K_longitude) * longitude + K_longitude * raw_longitude

      let next_error = (next_latitude_error, next_longitude_error)
      let next_point = (next_latitude, next_longitude, raw_timespan)

      next_point::(recursive_filter next_error next_point points)

    | _ -> points
    
  match points with
  | [] -> []
  | p1::points -> p1::(recursive_filter (ση, ση) p1 points)
