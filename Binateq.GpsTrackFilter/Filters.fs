module Filters

open System
open Types
open Formulas

/// <summary>
/// Removes points matching with specified predicate.
/// </summary>
/// <param name="predicate"></param>
/// <param name="points"></param>
let internal remove predicate points =
    match points with
    | [] -> []
    | p1::_ -> let filtered = points
                           |> List.pairwise
                           |> List.filter (fun (p1, p2) -> not (predicate p1 p2))
                           |> List.map (fun (_, p2) -> p2)
               p1::filtered


/// <summary>
/// Removes points with zero or negative time spans.
/// </summary>
let internal removeZeroOrNegativeTimespans points =
    let isZeroOrNegativeTimespan (p1: SensorItem) (p2: SensorItem) =
        let Δtime = p2.Timestamp - p1.Timestamp

        Δtime <= TimeSpan.Zero

    let rec filter p1 points =
        match points with
        | [] -> []
        | p2::points -> if isZeroOrNegativeTimespan p1 p2
                        then filter p1 points
                        else p2::filter p2 points

    match points with
    | [] -> []
    | p1::points -> p1::filter p1 points


/// <summary>
/// Removes points with outlined speed.
/// </summary>
let internal removeOutlineSpeedValues hiLimit points =
    let isOutlineSpeed p1 p2 =
        let velocity = velocity p1 p2

        velocity > hiLimit

    remove isOutlineSpeed points

/// <summary>
/// Replaces zero speed drift to zero.
/// </summary>
let internal replaceZeroSpeedDrift loLimit points =
    let isZeroDriftSpeed p1 p2 =
        let velocity = velocity p1 p2

        velocity < loLimit

    let rec filter p1 points =
        match points with
        | [] -> [p1]
        | [p2] -> if isZeroDriftSpeed p1 p2 then [p2] else p1::[p2]
        | p2::points -> if isZeroDriftSpeed p1 p2
                        then filter p2 points
                        else p1::filter p2 points

    match points with
    | p1::p2::points -> p1::filter p2 points
    | _ -> points
