module Queries

open ReadModel
open Domain
open States
open System

type TableQueirs = {
    GetTables : unit -> Async<Table list>
}

type ToDoQueries = {
    GetChefToDos : unit -> Async<ChefToDo list>
    GetWaiterToDos : unit -> Async<WaiterToDo list>
    GetCashierToDos : unit -> Async<Payment list>
}

type Queris = {
    Table : TableQueirs
    ToDo : ToDoQueries
}
