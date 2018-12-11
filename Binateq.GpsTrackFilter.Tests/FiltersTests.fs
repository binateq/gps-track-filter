module FiltersTests

open System
open System.Collections.Generic
open Xunit
open Filters

[<Fact>]
let ``filterBySimplifyKalman without points returns empty list`` () =
    let source = []

    let actual = filterBySimplifyKalman 1.0 1.0 source

    Assert.Empty(actual)

[<Fact>]
let ``filterBySimplifyKalman with single point returns same list`` () =
    let source = [(45.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:00+03:00"))]
    
    let actual = filterBySimplifyKalman 1.0 1.0 source

    let expected = [(45.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:00+03:00"))]
    Assert.Equal<IEnumerable<(float * float * DateTimeOffset)>>(expected, actual)

[<Fact>]
let ``filterBySimplifyKalman with points filter coordinates`` () =
    let source = [(45.0, 0.0, DateTimeOffset.Parse("2018-12-07T16:38:14+03:00"));
                  (45.5, 0.5, DateTimeOffset.Parse("2018-12-07T16:38:15+03:00"));
                  (45.0, 1.0, DateTimeOffset.Parse("2018-12-07T16:38:16+03:00"))]

    let actual = List.toArray (filterBySimplifyKalman 1.0 1.0 source)
    let latitude1, longitude1, _ = actual.[1]
    let latitude2, longitude2, _ = actual.[2]

    Assert.Equal(45.1111111111111, latitude1, 1)
    Assert.Equal(0.111111111111111, longitude1, 1)
    Assert.Equal(45.0836111111111, latitude2, 1)
    Assert.Equal(0.331111111111111, longitude2, 1)
