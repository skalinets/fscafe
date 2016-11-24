module Cashier

open System
open System.Collections.Generic
open Queries
open ReadModel
open Table
open Projections
open Domain

let private cachierToDos = new Dictionary<Guid, Payment>()

let private addTabAmount tabId amount =
    match getTableByTabId tabId with 
    | Some table  -> 
        let payment = {Tab = {Id = tabId; TableNumber = table.Number}; Amount = amount}
        cachierToDos.Add(tabId, payment)
    | None -> ()
    async.Return ()

let private remove tabId = 
    cachierToDos.Remove (tabId) |> ignore
    async.Return ()

let cashierActions = {
    AddTabAmount = addTabAmount
    Remove = remove
}

let getCashierToDos () = cachierToDos.Values |> Seq.toList |> async.Return