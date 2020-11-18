module KalmanTests

open Xunit
open Types
open Kalman
open SensorItem


[<Theory>]
[<InlineData(1.0, 1.0, 3.0, 0.8)>]
[<InlineData(1.0, 3.0, 1.0, 0.4)>]
let ``calculateNextK - with valid parameters - returns expected value`` (modelVariance: float,
                                                                         sensorVariance: float,
                                                                         squareError: float,
                                                                         expected: float) =
    let actual = calculateNextK modelVariance sensorVariance squareError
    
    Assert.Equal(expected, actual)


[<Theory>]
[<InlineData(1.0, 3.0, 1.0, 1.2)>]
let ``calculateNextSquareError - with valid parameters - returns expected value`` (modelVariance: float,
                                                                                   sensorVariance: float,
                                                                                   squareError: float,
                                                                                   expected: float) =
    let actual = calculateNextSquareError modelVariance sensorVariance squareError
    
    Assert.Equal(expected, actual)
    
    
[<Fact>]
let ``interpolate - with valid parameters - returns expected value`` () =
    Assert.Equal(0.7, interpolate 0.3 0.0 1.0)
    Assert.Equal(0.3, interpolate 0.3 1.0 0.0)
    Assert.Equal(3.0, interpolate 0.5 2.0 4.0)


[<Fact>]
let ``smoothByKalman nextPoint - without points - returns empty list`` () =
    let source = []

    let actual = smoothByKalman nextPoint 1.0 1.0 source

    Assert.Empty(actual)


[<Fact>]
let ``smoothByKalman nextPoint - with single point - returns same list`` () =
    let source = [sensorItem 45.0 0.0 "2018-12-07T16:38:00+03:00"]
    
    let actual =  smoothByKalman nextPoint 1.0 1.0 source

    let expected = [sensorItem 45.0 0.0 "2018-12-07T16:38:00+03:00"]
    Assert.Equal<SensorItem list>(expected, actual)


[<Fact>]
let ``smoothByKalman nextPoint - with points - filters coordinates`` () =
    let source = [sensorItem 45.0 0.0 "2018-12-07T16:38:14+03:00"
                  sensorItem 45.5 0.5 "2018-12-07T16:38:15+03:00"
                  sensorItem 45.0 1.0 "2018-12-07T16:38:16+03:00"]

    let actual = List.toArray ( smoothByKalman nextPoint 1.0 1.0 source)

    Assert.Equal(45.0, actual.[1].Latitude, 1)
    Assert.Equal(0.0, actual.[1].Longitude, 1)
    Assert.Equal(45.3, actual.[2].Latitude, 1)
    Assert.Equal(0.331111111111111, actual.[2].Longitude, 1)
