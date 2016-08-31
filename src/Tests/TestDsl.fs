module TestDsl

open States
open Xunit
open CommandHandlers
open Chessie.ErrorHandling
open FsUnit.Xunit
open Errors

let Given (state : State) = state

let When command state = (command, state)

let failAssert = failwith

let expectedActual a b = sprintf "\n ************** Expected > \n %A \n ************** Actual > \n %A" a b


let ThenStateShouldBe expectedState (command, state) =
    match evolve state command with 
    | Ok((actualState, events), _) ->
        actualState |> should equal expectedState
        events |> Some
    | Bad errs ->
        let msg = expectedActual expectedState errs.Head
        failAssert msg
        None

let WithEvents expectedEvents actualEvents =
    match actualEvents with 
    | Some (actualEvents) -> 
        actualEvents |> should equal expectedEvents
    | None -> None |> should equal expectedEvents

let ShouldFailWith (expectedError : Error) (command, state) =
    match evolve state command with
    | Bad errs -> errs.Head |> should equal expectedError
    | Ok (r,_) -> 
        expectedActual expectedError r |> failAssert 


