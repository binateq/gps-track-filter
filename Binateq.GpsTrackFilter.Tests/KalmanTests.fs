module KalmanTests

open Xunit
open Types
open Kalman
open SensorItem

[<Fact>]
let ``smoothBySimplifiedKalman - without points - returns empty list`` () =
    let source = []

    let actual = smoothBySimplifiedKalman 1.0 1.0 source

    Assert.Empty(actual)


[<Fact>]
let ``smoothBySimplifiedKalman - with single point - returns same list`` () =
    let source = [sensorItem 45.0 0.0 "2018-12-07T16:38:00+03:00"]
    
    let actual = smoothBySimplifiedKalman 1.0 1.0 source

    let expected = [sensorItem 45.0 0.0 "2018-12-07T16:38:00+03:00"]
    Assert.Equal<SensorItem list>(expected, actual)


[<Fact>]
let ``smoothBySimplifiedKalman - with points - filters coordinates`` () =
    let source = [sensorItem 45.0 0.0 "2018-12-07T16:38:14+03:00"
                  sensorItem 45.5 0.5 "2018-12-07T16:38:15+03:00"
                  sensorItem 45.0 1.0 "2018-12-07T16:38:16+03:00"]

    let actual = List.toArray (smoothBySimplifiedKalman 1.0 1.0 source)

    Assert.Equal(45.1111111111111, actual.[1].Latitude, 1)
    Assert.Equal(0.111111111111111, actual.[1].Longitude, 1)
    Assert.Equal(45.0836111111111, actual.[2].Latitude, 1)
    Assert.Equal(0.331111111111111, actual.[2].Longitude, 1)
