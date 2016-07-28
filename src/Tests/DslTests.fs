module TTests

open Chessie.ErrorHandling
open Xunit
open FsUnit.Xunit
open Commands
open Domain
open States
open Events
open CommandHandlers
open System
open Errors

let Given (state : State) = state

let When command state = (command, state)

let failAssert msg = Assert.True (false, msg)
    

let ThenStateShouldBe expectedState (command, state) =
    match evolve state command with 
    | Ok((actualState, events), _) ->
        actualState |> should equal expectedState
        events |> Some
    | Bad errs ->
        let msg = sprintf "Expected : %A, But Actual : %A" expectedState errs.Head
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
        sprintf "Expected : %A, But Actual : %A" expectedError r
        |> failAssert 
        
    

[<Fact>]
let just_test_it () = 1 |> should equal 1

[<Fact>]
let ``Can Open a new Tab`` () =
    let tab = {Id = Guid.NewGuid(); TableNumber = 1}

    Given (ClosedTab None)
    |> When (OpenTab tab)
    |> ThenStateShouldBe (OpenedTab tab)
    |> WithEvents [TabOpened tab]

[<Fact>]
let ``Cannot open an already opened tab`` () =
    let tab = {Id = Guid.NewGuid(); TableNumber = 1}

    Given (OpenedTab tab)
    |> When (OpenTab tab)
    |> ShouldFailWith TabAlreadyOpened

