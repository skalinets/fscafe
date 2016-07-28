module CommandHandlers

open Events
open System
open Domain
open Commands
open Chessie.ErrorHandling
open States
open Errors
    
let execute state command =
    match command with
    | OpenTab tab -> 
        printfn "%A" state 
        match state with
        | ClosedTab _ -> [TabOpened tab] |> ok
        | _ -> TabAlreadyOpened |> fail
    | _ -> failwith "todo"


let evolve state command =
    match execute state command with
    | Ok (events, _) -> 
        let newState = List.fold apply state events
        (newState, events) |> ok
    | Bad err -> Bad err

