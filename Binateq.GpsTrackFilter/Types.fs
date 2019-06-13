module Types

open System


// Fix the bag at Android's Mono:
//    System.ArgumentOutOfRangeException: Ticks must be between DateTime.MinValue.Ticks and DateTime.MaxValue.Ticks.
//                                        Parameter name: ticks
// at System.DateTime..ctor (System.Int64 ticks, System.DateTimeKind kind)
// at System.DateTime.SpecifyKind (System.DateTime value, System.DateTimeKind kind)
// at System.DateTimeOffset.get_UtcDateTime ()
// at System.DateTimeOffset.op_Subtraction (System.DateTimeOffset left, System.DateTimeOffset right)
// at Formulas.velocity(Types+SensorItem p1, Types+SensorItem p2)
type internal SensorItem(latitude: float, longitude: float, speed: float, heading: float, timestamp: DateTimeOffset) =
    member __.Latitude = latitude
    member __.Longitude = longitude
    member __.Timestamp = timestamp
    member __.Speed = speed
    member __.Heading = heading

    override __.GetHashCode() =
        hash (latitude, longitude, timestamp, speed, heading)

    override __.Equals(other) =
        match other with
        | :? SensorItem as x -> (latitude, longitude, timestamp, speed, heading) = (x.Latitude, x.Longitude, x.Timestamp, x.Speed, x.Heading)
        | _ -> false
