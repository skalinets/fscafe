module OpenTabTests

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
open TestDsl

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

