module Types

open System

[<Struct>]
type internal SensorItem =
    {
       /// <summary>
       /// Latitude from -90 to +90 degrees.
       /// </summary>
       Latitude: float
       
       /// <summary>
       /// Longitude from -180 to +180 degrees.
       /// </summary>
       Longitude: float
       
       /// <summary>
       /// Speed in meters per second.
       /// <summary>
       Speed: float
       
       /// <summary>
       /// Speed direction related to North in degrees. Clockwise is positive.
       /// </summary>
       Heading: float
       
       /// <summary>
       /// Date and time of measure.
       /// </summary>
       Timestamp: DateTimeOffset }