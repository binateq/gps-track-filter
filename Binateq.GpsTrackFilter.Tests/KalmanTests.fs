﻿module KalmanTests

open System
open Xunit
open Types
open Kalman

[<Fact>]
let ``smoothBySimplifiedKalman without points returns empty list`` () =
    let source = []

    let actual = smoothBySimplifiedKalman 1.0 1.0 source

    Assert.Empty(actual)


[<Fact>]
let ``smoothBySimplifiedKalman with single point returns same list`` () =
    let source = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:00+03:00"))]
    
    let actual = smoothBySimplifiedKalman 1.0 1.0 source

    let expected = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:00+03:00"))]
    Assert.Equal<SensorItem list>(expected, actual)


[<Fact>]
let ``smoothBySimplifiedKalman with points filter coordinates`` () =
    let source = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                  SensorItem(45.5, 0.5, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:15+03:00"));
                  SensorItem(45.0, 1.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:16+03:00"))]

    let actual = List.toArray (smoothBySimplifiedKalman 1.0 1.0 source)

    Assert.Equal(45.1111111111111, actual.[1].Latitude, 1)
    Assert.Equal(0.111111111111111, actual.[1].Longitude, 1)
    Assert.Equal(45.0836111111111, actual.[2].Latitude, 1)
    Assert.Equal(0.331111111111111, actual.[2].Longitude, 1)