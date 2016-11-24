module Waiter

open System
open System.Collections.Generic
open Queries
open ReadModel
open Table
open Projections

let private waiterToDos = new Dictionary<Guid, WaiterToDo>()

let private addDrinksToServe tabId drinks = 
    match getTableByTabId tabId with
    | Some table -> 
        let todo = {
            Tab = {Id = tabId; TableNumber = table.Number}
            Foods = []
            Drinks = drinks
        }
        waiterToDos.Add(tabId, todo)
    | None -> ()
    async.Return ()

let private addFoodToServe tabId food =
    match waiterToDos.TryGetValue(tabId) with
    | true, todo -> 
        let waiterTodo = {todo with Foods = food :: todo.Foods}
        waiterToDos.[tabId] <- waiterTodo
    | _ -> 
        match getTableByTabId tabId with
        | Some table ->
            let todo = 
                {   Tab = {Id = tabId; TableNumber = table.Number}
                    Foods = [food]
                    Drinks = []
                }
            waiterToDos.Add(tabId, todo)
        | None -> ()
    
    async.Return ()

let private markDrinkServed tabId drink =
    let todo = waiterToDos.[tabId]
    let waiterToDo = {todo with Drinks = todo.Drinks |> List.except [drink]}
    waiterToDos.[tabId] <- waiterToDo
    async.Return ()

let private markFoodServed tabId food =
    let todo = waiterToDos.[tabId]
    let waiterToDo = {todo with Foods = todo.Foods |> List.except [food]}
    waiterToDos.[tabId] <- waiterToDo
    async.Return ()
        
let private remove tabId =
    waiterToDos.Remove(tabId) |> ignore
    async.Return ()

let waiterActions = {
    AddFoodToServe = addFoodToServe
    AddDrinksToServe = addDrinksToServe
    MarkDrinkServed = markDrinkServed
    MarkFoodServed = markFoodServed
    Remove = remove
}

let getWaiterToDos () =
    waiterToDos.Values
    |> Seq.toList
    |> async.Return


