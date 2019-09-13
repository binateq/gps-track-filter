module Filters

open System
open Types
open Formulas


// <summary>
// Removes points with zero or negative time spans.
// </summary>
let internal removeZeroOrNegativeTimespans points =
    let rec filter (p1: SensorItem) points =
        match points with
        | (p2: SensorItem)::tail -> let Δtime = p2.Timestamp - p1.Timestamp
                                    if Δtime > TimeSpan.Zero
                                    then p2::filter p2 tail
                                    else filter p1 tail
        | _ -> points

    match points with
    | p1::tail -> p1::filter p1 tail
    | _ -> points


/// <summary>
/// Removes points with outlined speed.
/// </summary>
let internal removeOutlineSpeedValues hiLimit points =
    let isOutlineSpeed p1 p2 =
        let velocity = velocity p1 p2

        velocity > hiLimit

    let rec filter p1 points =
        match points with
        | p2::tail -> if isOutlineSpeed p1 p2
                      then filter p1 tail
                      else p2::filter p2 tail
        | _ -> points

    match points with
    | p1::tail -> p1::filter p1 tail
    | _ -> points


/// <summary>
/// Removes points with zero speed drift.
/// </summary>
let internal removeZeroSpeedDrift loLimit points =
    let isZeroDriftSpeed p1 p2 =
        let velocity = velocity p1 p2

        velocity < loLimit

    let rec filter3 p1 p2 points =
        match points with
        | _::p2::[p3] -> if isZeroDriftSpeed p2 p3
                         then p1::[p3]
                         else p1::p2::[p3]
        | p3::ps      -> if isZeroDriftSpeed p2 p3
                         then filter3 p1 p3 ps
                         else p1::filter3 p2 p3 ps
        | ps          -> p1::p2::ps

    match points with
    | p1::p2::ps -> filter3 p1 p2 ps
    | _ -> points
