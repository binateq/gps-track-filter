module FiltersTest

open System
open Xunit
open Types
open Formulas
open Filters


[<Fact>]
let ``distance between Moscow and Saint Petersburg approximately equals 634km`` () =
   let MoscowLatitude = 55.753960
   let MoscowLongitude = 37.620393
   let SaintPetersburgLatitude = 59.9386300
   let SaintPetersburtLongitude = 30.3141300

   let actual = distance MoscowLatitude MoscowLongitude SaintPetersburgLatitude SaintPetersburtLongitude

   let expected = 634.37
   Assert.Equal(expected, actual, 0)


[<Fact>]
let ``removeZeroOrNegativeTimespans - without points - returns empty list`` () =
    let source = []

    let actual = removeZeroOrNegativeTimespans source

    Assert.Empty(actual)


[<Fact>]
let ``removeZeroOrNegativeTimespans - with single point - returns same list`` () =
    let source = [SensorItem(0.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:00+03:00"))]

    let actual = removeZeroOrNegativeTimespans source

    let expected = [SensorItem(0.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:00+03:00"))]
    Assert.Equal<seq<SensorItem>>(expected, actual)


[<Fact>]
let ``removeZeroOrNegativeTimespans - with zero timespan - removes point`` () =
    let source = [SensorItem(0.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:15+03:00"));
                  SensorItem(1.0, 1.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:15+03:00"));
                  SensorItem(2.0, 2.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:16+03:00"))]

    let actual = removeZeroOrNegativeTimespans source

    let expected = [SensorItem(0.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:15+03:00"));
                    SensorItem(2.0, 2.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:16+03:00"))]
    Assert.Equal<seq<SensorItem>>(expected, actual)
    

[<Fact>]
let ``removeZeroOrNegativeTimespans - with negative timespan - removes point`` () =
    let source = [SensorItem(0.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:15+03:00"));
                  SensorItem(1.0, 1.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                  SensorItem(2.0, 2.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:16+03:00"))]

    let actual = removeZeroOrNegativeTimespans source

    let expected = [SensorItem(0.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:15+03:00"));
                    SensorItem(2.0, 2.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:16+03:00"))]
    Assert.Equal<seq<SensorItem>>(expected, actual)
    

[<Fact>]
let ``removeZeroOrNegativeTimespans - with positime timespans - returns same list`` () =
    let source = [SensorItem(0.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                  SensorItem(1.0, 1.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:15+03:00"));
                  SensorItem(2.0, 2.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:16+03:00"))]

    let actual = removeZeroOrNegativeTimespans source

    let expected = [SensorItem(0.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                    SensorItem(1.0, 1.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:15+03:00"));
                    SensorItem(2.0, 2.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:16+03:00"))]
    Assert.Equal<seq<SensorItem>>(expected, actual)


[<Fact>]
let ``removeZeroOrNegativeTimespans - with two negative timespans - removes both points`` () =
    let source = [SensorItem(0.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:15+03:00"));
                  SensorItem(1.0, 1.0, 0.0, 0.0, DateTimeOffset.Parse("2018-11-07T16:38:16+03:00"));
                  SensorItem(2.0, 2.0, 0.0, 0.0, DateTimeOffset.Parse("2018-11-07T16:38:17+03:00"));
                  SensorItem(3.0, 3.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:18+03:00"))]

    let actual = removeZeroOrNegativeTimespans source

    let expected = [SensorItem(0.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:15+03:00"));
                    SensorItem(3.0, 3.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:18+03:00"))]
    Assert.Equal<seq<SensorItem>>(expected, actual)


let oneDegreeOfMeridianInKm = 111.32
    

[<Fact>]
let ``removeOutlineSpeedValues without points returns empty list`` () =
    let source = []

    let actual = removeOutlineSpeedValues oneDegreeOfMeridianInKm source

    Assert.Empty(actual)


[<Fact>]
let ``removeOutlineSpeedValues with single point returns same list`` () =
    let source = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:00+03:00"))]

    let actual = removeOutlineSpeedValues oneDegreeOfMeridianInKm source

    let expected = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:00+03:00"))]
    Assert.Equal<seq<SensorItem>>(expected, actual)


[<Fact>]
let ``removeOutlineSpeedValues with outline speed removes point`` () =
    let source = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                  SensorItem(46.1, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T17:38:14+03:00"));
                  SensorItem(47.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T18:38:16+03:00"))]

    let actual = removeOutlineSpeedValues oneDegreeOfMeridianInKm source

    let expected = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                    SensorItem(47.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T18:38:16+03:00"))]
    Assert.Equal<seq<SensorItem>>(expected, actual)


[<Fact>]
let ``removeOutlineSpeedValues without outline speed returns same list`` () =
    let source = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                  SensorItem(46.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T17:38:14+03:00"));
                  SensorItem(47.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T18:38:16+03:00"))]

    let actual = removeOutlineSpeedValues oneDegreeOfMeridianInKm source

    let expected = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                    SensorItem(46.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T17:38:14+03:00"));
                    SensorItem(47.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T18:38:16+03:00"))]
    Assert.Equal<seq<SensorItem>>(expected, actual)


[<Fact>]
let ``replaceZeroSpeedDrift without points returns empty list`` () =
    let source = []

    let actual = replaceZeroSpeedDrift (oneDegreeOfMeridianInKm/10.0) source

    Assert.Empty(actual)


[<Fact>]
let ``replaceZeroSpeedDrift with single point returns same list`` () =
    let source = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:00+03:00"))]

    let actual = replaceZeroSpeedDrift (oneDegreeOfMeridianInKm/10.0) source

    let expected = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:00+03:00"))]
    Assert.Equal<seq<SensorItem>>(expected, actual)


[<Fact>]
let ``replaceZeroSpeedDrift with very small drift removes drift points`` () =
    let source = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                  SensorItem(46.95, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T17:38:14+03:00"));
                  SensorItem(47.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T18:38:16+03:00"))]

    let actual = replaceZeroSpeedDrift (oneDegreeOfMeridianInKm/10.0) source

    let expected = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                    SensorItem(47.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T18:38:16+03:00"))]
    Assert.Equal<seq<SensorItem>>(expected, actual)


[<Fact>]
let ``replaceZeroSpeedDrift with very small drift keeps two end points`` () =
    let source = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                  SensorItem(45.05, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T17:38:14+03:00"))]

    let actual = replaceZeroSpeedDrift (oneDegreeOfMeridianInKm/10.0) source

    let expected = [SensorItem(45.0, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                    SensorItem(45.05, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T17:38:14+03:00"))]
    Assert.Equal<seq<SensorItem>>(expected, actual)


[<Fact>]
let ``replaceZeroSpeedDrift without drift returns same list`` () =
    let source = [SensorItem(45.00, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                  SensorItem(45.15, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T17:38:14+03:00"));
                  SensorItem(47.00, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T18:38:16+03:00"))]

    let actual = replaceZeroSpeedDrift (oneDegreeOfMeridianInKm/10.0) source

    let expected = [SensorItem(45.00, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                    SensorItem(45.15, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T17:38:14+03:00"));
                    SensorItem(47.00, 0.0, 0.0, 0.0, DateTimeOffset.Parse("2018-12-07T18:38:16+03:00"))]
    Assert.Equal<seq<SensorItem>>(expected, actual)
