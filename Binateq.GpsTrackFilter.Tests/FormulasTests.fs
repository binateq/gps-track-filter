module FormulasTests

open System
open Xunit
open Types
open Formulas


[<Fact>]
let ``distance between Moscow and Saint Petersburg approximately equals 630km`` () =
   let MoscowLatitude = 55.753960
   let MoscowLongitude = 37.620393
   let SaintPetersburgLatitude = 59.9386300
   let SaintPetersburtLongitude = 30.3141300

   let actual = distance MoscowLatitude MoscowLongitude SaintPetersburgLatitude SaintPetersburtLongitude

   let expected = 634.37
   Assert.True(abs(actual - expected) < 10.0)


[<Fact>]
let ``distance signle grade at equator approximately eqauls 111km`` () =
    let startLatitude = 0.0
    let startLongitude = 0.0
    let endLatitude = 0.0
    let endLongitude = 1.0

    let actual = distance startLatitude startLongitude endLatitude endLongitude

    let expected = 40000.0/360.0
    Assert.Equal(expected, actual, 0)


[<Fact>]
let ``velocity at single grade per hour at equator approximately equals 111km per hour`` () =
    let startSensorItem = new SensorItem(0.0, 0.0, 0.0, 0.0, new DateTimeOffset(2019, 4, 26, 11, 00, 00, TimeSpan.Zero))
    let endSensorItem = new SensorItem(0.0, 1.0, 0.0, 0.0, new DateTimeOffset(2019, 4, 26, 12, 00, 00, TimeSpan.Zero))

    let actual = velocity startSensorItem endSensorItem

    let expected = 111.11
    Assert.Equal(expected, actual, 0)


[<Fact>]
let ``daysBeforeDate for 1 January 0001 AC returns 0`` () =
    let actual = daysBeforeDate 1 1 1

    Assert.Equal(0, actual)


[<Fact>]
let ``daysBeforeDate for 1 January 0002 AC returns 365`` () =
    let actual = daysBeforeDate 2 1 1

    Assert.Equal(365, actual)


[<Fact>]
let ``daysBeforeDate for 1 January 0101 AC returns 36524`` () =
    let actual = daysBeforeDate 101 1 1

    Assert.Equal(36524, actual)


[<Fact>]
let ``secondsAtDateTime for 2 January 0001 AC 0:0:0 returns 84600`` () =
    let actual = secondsAtDateTime 1 1 2 0 0 0

    Assert.Equal(86400L, actual)


[<Fact>]
let ``secondsAtDateTime for 1 January 0001 AC 10:10:10 returns 36610`` () =
    let actual = secondsAtDateTime 1 1 1 10 10 10

    Assert.Equal(36610L, actual)