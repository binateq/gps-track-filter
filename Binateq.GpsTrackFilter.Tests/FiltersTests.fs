namespace Binateq.GpsTrackFilter.Tests

module FiltersTest =

    open System
    open Xunit
    open Types
    open Filters
    open SensorItem


    [<Fact>]
    let ``removeZeroOrNegativeTimespans - without points - returns empty list`` () =
        let source = []

        let actual = removeZeroOrNegativeTimespans source

        Assert.Empty(actual)


    [<Fact>]
    let ``removeZeroOrNegativeTimespans - with single point - returns single point`` () =
        let source = [sensorItem 0.0 0.0 "2018-12-07T16:38:00+03:00"]

        let actual = removeZeroOrNegativeTimespans source

        let expected = [sensorItem 0.0 0.0 "2018-12-07T16:38:00+03:00"]
        Assert.Equal<seq<SensorItem>>(expected, actual)


    [<Fact>]
    let ``removeZeroOrNegativeTimespans - with zero timespan - removes point`` () =
        let source = [sensorItem 0.0 0.0 "2018-12-07T16:38:15+03:00"
                      sensorItem 1.0 1.0 "2018-12-07T16:38:15+03:00"
                      sensorItem 2.0 2.0 "2018-12-07T16:38:16+03:00"]

        let actual = removeZeroOrNegativeTimespans source

        let expected = [sensorItem 0.0 0.0 "2018-12-07T16:38:15+03:00"
                        sensorItem 2.0 2.0 "2018-12-07T16:38:16+03:00"]
        Assert.Equal<seq<SensorItem>>(expected, actual)
    

    [<Fact>]
    let ``removeZeroOrNegativeTimespans - with negative timespan - removes point`` () =
        let source = [sensorItem 0.0 0.0 "2018-12-07T16:38:15+03:00"
                      sensorItem 1.0 1.0 "2018-12-07T16:38:14+03:00"
                      sensorItem 2.0 2.0 "2018-12-07T16:38:16+03:00"]

        let actual = removeZeroOrNegativeTimespans source

        let expected = [sensorItem 0.0 0.0 "2018-12-07T16:38:15+03:00"
                        sensorItem 2.0 2.0 "2018-12-07T16:38:16+03:00"]
        Assert.Equal<seq<SensorItem>>(expected, actual)
    

    [<Fact>]
    let ``removeZeroOrNegativeTimespans - with positive timespans - returns same list`` () =
        let source = [sensorItem 0.0 0.0 "2018-12-07T16:38:14+03:00"
                      sensorItem 1.0 1.0 "2018-12-07T16:38:15+03:00"
                      sensorItem 2.0 2.0 "2018-12-07T16:38:16+03:00"]

        let actual = removeZeroOrNegativeTimespans source

        let expected = [sensorItem 0.0 0.0 "2018-12-07T16:38:14+03:00"
                        sensorItem 1.0 1.0 "2018-12-07T16:38:15+03:00"
                        sensorItem 2.0 2.0 "2018-12-07T16:38:16+03:00"]
        Assert.Equal<seq<SensorItem>>(expected, actual)


    [<Fact>]
    let ``removeZeroOrNegativeTimespans - with two negative timespans - removes both points`` () =
        let source = [sensorItem 0.0 0.0 "2018-12-07T16:38:15+03:00"
                      sensorItem 1.0 1.0 "2018-12-07T16:38:13+03:00"
                      sensorItem 2.0 2.0 "2018-12-07T16:38:14+03:00"
                      sensorItem 3.0 3.0 "2018-12-07T16:38:16+03:00"]

        let actual = removeZeroOrNegativeTimespans source

        let expected = [sensorItem 0.0 0.0 "2018-12-07T16:38:15+03:00"
                        sensorItem 3.0 3.0 "2018-12-07T16:38:16+03:00"]

        Assert.Equal<seq<SensorItem>>(expected, actual)


    let oneDegreeOfMeridianInKm = 111.32
    

    [<Fact>]
    let ``removeOutlineSpeedValues - without points - returns empty list`` () =
        let source = []

        let actual = removeOutlineSpeedValues oneDegreeOfMeridianInKm source

        Assert.Empty(actual)


    [<Fact>]
    let ``removeOutlineSpeedValues - with single point - returns same list`` () =
        let source = [sensorItem 45.0 0.0 "2018-12-07T16:38:00+03:00"]

        let actual = removeOutlineSpeedValues oneDegreeOfMeridianInKm source

        let expected = [sensorItem 45.0 0.0 "2018-12-07T16:38:00+03:00"]
        Assert.Equal<seq<SensorItem>>(expected, actual)


    [<Fact>]
    let ``removeOutlineSpeedValues - with outline speed - removes point`` () =
        let source = [sensorItem 45.0 0.0 "2018-12-07T16:38:14+03:00"
                      sensorItem 46.1 0.0 "2018-12-07T17:38:14+03:00"
                      sensorItem 47.0 0.0 "2018-12-07T18:38:16+03:00"]

        let actual = removeOutlineSpeedValues oneDegreeOfMeridianInKm source

        let expected = [sensorItem 45.0 0.0 "2018-12-07T16:38:14+03:00"
                        sensorItem 47.0 0.0 "2018-12-07T18:38:16+03:00"]
        Assert.Equal<seq<SensorItem>>(expected, actual)


    [<Fact>]
    let ``removeOutlineSpeedValues - without outline speed - returns same list`` () =
        let source = [sensorItem 45.0 0.0 "2018-12-07T16:38:14+03:00"
                      sensorItem 46.0 0.0 "2018-12-07T17:38:14+03:00"
                      sensorItem 47.0 0.0 "2018-12-07T18:38:16+03:00"]

        let actual = removeOutlineSpeedValues oneDegreeOfMeridianInKm source

        let expected = [sensorItem 45.0 0.0 "2018-12-07T16:38:14+03:00"
                        sensorItem 46.0 0.0 "2018-12-07T17:38:14+03:00"
                        sensorItem 47.0 0.0 "2018-12-07T18:38:16+03:00"]
        Assert.Equal<seq<SensorItem>>(expected, actual)


    [<Fact>]
    let ``removeZeroSpeedDrift - without points - returns empty list`` () =
        let source = []

        let actual = removeZeroSpeedDrift (oneDegreeOfMeridianInKm/10.0) source

        Assert.Empty(actual)


    [<Fact>]
    let ``removeZeroSpeedDrift - with single point - returns same list`` () =
        let source = [sensorItem 45.0 0.0 "2018-12-07T16:38:00+03:00"]

        let actual = removeZeroSpeedDrift (oneDegreeOfMeridianInKm/10.0) source

        let expected = [sensorItem 45.0 0.0 "2018-12-07T16:38:00+03:00"]
        Assert.Equal<seq<SensorItem>>(expected, actual)


    [<Fact>]
    let ``removeZeroSpeedDrift - with very small drift - removes drift points`` () =
        let source = [sensorItem 45.0 0.0 "2018-12-07T16:38:14+03:00"
                      sensorItem 46.95 0.0 "2018-12-07T17:38:14+03:00"
                      sensorItem 47.0 0.0 "2018-12-07T18:38:16+03:00"]

        let actual = removeZeroSpeedDrift (oneDegreeOfMeridianInKm/10.0) source

        let expected = [sensorItem 45.0 0.0 "2018-12-07T16:38:14+03:00"
                        sensorItem 47.0 0.0 "2018-12-07T18:38:16+03:00"]
        Assert.Equal<seq<SensorItem>>(expected, actual)


    [<Fact>]
    let ``removeZeroSpeedDrift - with very small drift - keeps two end points`` () =
        let source = [sensorItem 45.0 0.0 "2018-12-07T16:38:14+03:00"
                      sensorItem 45.05 0.0 "2018-12-07T17:38:14+03:00"]

        let actual = removeZeroSpeedDrift (oneDegreeOfMeridianInKm/10.0) source

        let expected = [sensorItem 45.0 0.0 "2018-12-07T16:38:14+03:00"
                        sensorItem 45.05 0.0 "2018-12-07T17:38:14+03:00"]
        Assert.Equal<seq<SensorItem>>(expected, actual)


    [<Fact>]
    let ``removeZeroSpeedDrift - without drift - returns same list`` () =
        let source = [sensorItem 45.00 0.0 "2018-12-07T16:38:14+03:00"
                      sensorItem 45.15 0.0 "2018-12-07T17:38:14+03:00"
                      sensorItem 47.00 0.0 "2018-12-07T18:38:16+03:00"]

        let actual = removeZeroSpeedDrift (oneDegreeOfMeridianInKm/10.0) source

        let expected = [sensorItem 45.00 0.0 "2018-12-07T16:38:14+03:00"
                        sensorItem 45.15 0.0 "2018-12-07T17:38:14+03:00"
                        sensorItem 47.00 0.0 "2018-12-07T18:38:16+03:00"]
        Assert.Equal<seq<SensorItem>>(expected, actual)


    [<Fact>]
    let ``removeNonNumbers - with NaN - removes NaN`` () =
        let source = [sensorItem Double.NaN 1.0 "2018-12-07T16:38:14+03:00"
                      sensorItem 1.0 Double.NaN "2018-12-07T16:38:15+03:00"]

        let actual = removeNotNumbers source

        Assert.Empty(actual)
        

    [<Fact>]
    let ``removeNonNumbers - with infinity - removes infinity`` () =
        let source = [sensorItem Double.PositiveInfinity 1.0 "2018-12-07T16:38:14+03:00"
                      sensorItem 1.0 Double.NegativeInfinity "2018-12-07T16:38:15+03:00"]
        
        let actual = removeNotNumbers source
        
        Assert.Empty(actual)


    [<Fact>]
    let ``removeNonNumbers - with sub-normals - removes sub-normals`` () =
        let source = [sensorItem 1e-38 1.0 "2018-12-07T16:38:14+03:00"
                      sensorItem 1.0 1e-38 "2018-12-07T16:38:15+03:00"]

        let actual = removeNotNumbers source

        Assert.Empty(actual)