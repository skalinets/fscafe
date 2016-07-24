module CommandHandlers

open States
open Events
open System
open Domain
open Commands
open Chessie

let execute state command =
    match command with
    | OpenTab tab -> [TabOpened tab]
    | _ -> failwith "todo"


let evolve state command =
    let events = execute state command
    printfn "events: %O" events
    let newState = List.fold apply state events
    (newState, events)

