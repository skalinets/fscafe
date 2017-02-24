module Table

open Domain
open System.Collections.Generic
open ReadModel
open Projections
open Queries

let a = 4

let private tables =
    let dict = new Dictionary<int, Table> ()
    dict.Add(1, {Number = 1; Waiter = "X"; Status = Closed})
    dict.Add(2, {Number = 2; Waiter = "Y"; Status = Closed})
    dict.Add(3, {Number = 3; Waiter = "Z"; Status = Closed})
    dict

let private openTab tab =
    let tableNumber = tab.TableNumber
    let table = tables.[tableNumber] 
    tables.[tableNumber] <- {table with Status = Open(tab.Id)}
    async.Return ()

let private closeTab tab =
    let tableNumber = tab.TableNumber
    let table = tables.[tableNumber]
    tables.[tableNumber] <- {table with Status = Closed}
    async.Return ();

let getTableByTabId tabId =
    tables.Values 
    |> Seq.tryFind (
        fun t -> 
            match t.Status with
            | (Open id) | (InService id) -> id = tabId 
            | _ -> false)

let getTableByTableNumber tableNumber =
    if tables.ContainsKey tableNumber then 
        tables.[tableNumber] |> Some 
    else
        None
    |> async.Return

let private receiveOrder tabId =
    match getTableByTabId tabId with
    | Some table -> 
        let tableNumer = table.Number
        tables.[tableNumer] <- {table with Status = InService(tabId)}
    | _ -> ()
    async.Return ()

let tableActions = {
    OpenTab = openTab
    ReceivedOrder = receiveOrder
    CloseTab = closeTab
}

let getTables () =
    tables.Values
    |> Seq.toList
    |> async.Return

let tableQueries  = {
    GetTables = getTables
    GetTableByTableNumber = getTableByTableNumber
}
    
    
