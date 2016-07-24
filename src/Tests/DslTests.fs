module TTests

open Xunit
open FsUnit.Xunit
open Commands
open Domain
open States
open Events
open CommandHandlers
open System

let Given (state : State) = state

let When command state = (command, state)

let ThenStateShouldBe expectedState (command, state) = 
    let actualState, events = evolve state command
    actualState |> should equal expectedState
    events

let WithEvents expectedEvents actualEvents =
    actualEvents |> should equal expectedEvents

[<Fact>]
let just_test_it () = 1 |> should equal 1

[<Fact>]
let ``Can Open a new Tab`` () =
    let tab = {Id = Guid.NewGuid(); TableNumber = 1}

    Given (ClosedTab None)
    |> When (OpenTab tab)
    |> ThenStateShouldBe (OpenedTab tab)
    |> WithEvents [TabOpened tab]

type Class1() = 
    member this.X = "F#"
