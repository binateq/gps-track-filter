module FormulasTests

open System
open Xunit
open Types
open Formulas


[<Fact>]
let ``distance - between Moscow and Saint Petersburg - approximately equals 630km`` () =
   let MoscowLatitude = 55.753960
   let MoscowLongitude = 37.620393
   let SaintPetersburgLatitude = 59.9386300
   let SaintPetersburtLongitude = 30.3141300

   let actual = distance MoscowLatitude MoscowLongitude SaintPetersburgLatitude SaintPetersburtLongitude

   let expected = 634.37
   let epsilon = 0.005
   Assert.True(abs(actual - expected) < expected * epsilon)


[<Fact>]
let ``distance - signle grade at equator - approximately eqauls 111km`` () =
    let startLatitude = 0.0
    let startLongitude = 0.0
    let endLatitude = 0.0
    let endLongitude = 1.0

    let actual = distance startLatitude startLongitude endLatitude endLongitude

    let expected = 40000.0/360.0
    Assert.Equal(expected, actual, 0)


[<Fact>]
let ``velocity - with single grade per hour at equator - approximately equals 111km per hour`` () =
    let startSensorItem = new SensorItem(0.0, 0.0, 0.0, 0.0, new DateTimeOffset(2019, 4, 26, 11, 00, 00, TimeSpan.Zero))
    let endSensorItem = new SensorItem(0.0, 1.0, 0.0, 0.0, new DateTimeOffset(2019, 4, 26, 12, 00, 00, TimeSpan.Zero))

    let actual = velocity startSensorItem endSensorItem

    let expected = 111.0
    Assert.Equal(expected, actual, 0)