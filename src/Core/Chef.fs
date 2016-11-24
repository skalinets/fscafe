module Chef

open System
open System.Collections.Generic
open Queries
open ReadModel
open Table
open Projections
open Domain

let private chefToDos = new Dictionary<Guid, ChefToDo>()

let private addFoosToPrepare tabId foods =
    match getTableByTabId tabId with
    | Some table -> 
        let tab = {Id = tabId; TableNumber = table.Number}
        let todo : ChefToDo = {Tab = tab; Foods = foods}
        chefToDos.Add(tabId, todo)
    | _ -> ()
    async.Return ()

let private removeFood tabId food = 
    let todo = chefToDos.[tabId]
    let chefToDo = 
        {todo with Foods =
                    List.filter (fun d -> d <> food) todo.Foods}

    chefToDos.[tabId] <- chefToDo
    async.Return ()

let private remove tabId =
    chefToDos.Remove(tabId) |> ignore
    async.Return ()

let getChefToDos () =
    chefToDos.Values
    |> Seq.toList
    |> async.Return

let chefActions = {
    AddFoodsToPrepare = addFoosToPrepare
    RemoveFood = removeFood
    Remove = remove
}


