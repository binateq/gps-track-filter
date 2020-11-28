namespace Binateq.GpsTrackFilter.Tests

module FormulasTests =

    open Xunit
    open Formulas
    open SensorItem


    [<Fact>]
    let ``distance - between Moscow and Saint Petersburg - approximately equals 635km`` () =
       let MoscowLatitude = 55.753960
       let MoscowLongitude = 37.620393
       let SaintPetersburgLatitude = 59.9386300
       let SaintPetersburgLongitude = 30.3141300

       let actual = distance MoscowLatitude MoscowLongitude SaintPetersburgLatitude SaintPetersburgLongitude

       let expected = 634.37
       let epsilon = 0.005
       Assert.True(abs(actual - expected) < expected * epsilon)


    [<Fact>]
    let ``distance - 1 grade at equator - approximately equals 111km`` () =
        let startLatitude = 0.0
        let startLongitude = 0.0
        let endLatitude = 0.0
        let endLongitude = 1.0

        let actual = distance startLatitude startLongitude endLatitude endLongitude

        let expected = 40000.0/360.0
        Assert.Equal(expected, actual, 0)


    [<Fact>]
    let ``velocity - with 1 grade per hour at equator - approximately equals 111km per hour`` () =
        let startItem = sensorItem 0.0 0.0 "2019-04-26T11:00:00+00:00"
        let endItem = sensorItem 0.0 1.0 "2019-04-26T12:00:00+00:00"
        let actual = velocity startItem endItem

        let expected = 111.0
        Assert.Equal(expected, actual, 0)