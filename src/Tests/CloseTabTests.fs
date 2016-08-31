module CloseTabTests

open Xunit
open TestData
open Domain
open States
open Commands
open CommandHandlers
open Events
open TestDsl
open Errors

let order = {order with
                Foods = [salad; pizza]
                Drinks = [coke]}

[<Fact>]
let ``Can close the tab by paying full amount`` () =
    let payment = {Tab = tab; Amount = orderAmount order}

    Given (ServedOrder order)
    |> When (CloseTab payment)
    |> ThenStateShouldBe (ClosedTab (Some tab.Id))
    |> WithEvents [TabClosed payment]

[<Fact>]
let ``Can not close a tab with invalid payment amount`` () =
    Given (ServedOrder order)
    |> When (CloseTab {Tab = tab; Amount = 9.5m})
    |> ShouldFailWith  (InvalidPayment (orderAmount order, 9.5m))