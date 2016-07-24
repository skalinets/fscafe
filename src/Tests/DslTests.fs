module TTests

open Xunit
open FsUnit.Xunit
open Domain
open States
open CommandHandlers

let Given (state : State) = state

let When command state = (command, state)

let ThenStateShouldBe expectedState (command, state) = 
    let actualState = evolve state command
    actualState |> should equal expectedState

let WithEvents expectedEvents actualEvents =
    actualEvents |> should equal expectedEvents

[<Fact>]
let just_test_it () = 1 |> should equal 1

type Class1() = 
    member this.X = "F#"
