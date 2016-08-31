module ServeDrinkTests

open Xunit
open TestData
open Domain
open States
open Commands
open CommandHandlers
open Events
open TestDsl
open Errors

[<Fact>]
let ``Can serve drink`` () =
    let order = {order with Drinks = [lemonade; coke]}
    let expected = {Place order with ServedDrinks = [coke]}
    Given (PlacedOrder order)
    |> When (ServeDrink (coke, order.Tab.Id))
    |> ThenStateShouldBe (OrderInProgress expected)
    |> WithEvents [DrinkServed (coke, order.Tab.Id)]

[<Fact>]
let ``Cannot serve non-ordered drinks`` () =
    let order = {order with Drinks = [coke]}
    Given (PlacedOrder order)
    |> When (ServeDrink (lemonade, order.Tab.Id))
    |> ShouldFailWith (CanNotServeNonOrderedDrink lemonade)

[<Fact>]
let ``Cannot serve drink for already served order`` () =
    Given (ServedOrder order)
    |> When (ServeDrink (coke, order.Tab.Id))
    |> ShouldFailWith OrderAlreadyServed

[<Fact>]
let ``Cannot serve drinks for non placed order`` () =
    Given (OpenedTab tab)
    |> When (ServeDrink (coke, tab.Id))
    |> ShouldFailWith CanNotServeForNonPlacedOrder

[<Fact>]
let ``Cannot serve with closed tab`` () =
    Given (ClosedTab None)
    |> When (ServeDrink (coke, tab.Id))
    |> ShouldFailWith CanNotServeWithClosedTab

[<Fact>]
let ``Can serve drink for order containing only one drink`` () =
    let order = { order with Drinks = [coke] }
    let payment = {Tab = order.Tab; Amount = drinkPrice coke}
    Given (PlacedOrder order)
    |> When (ServeDrink (coke, order.Tab.Id))
    |> ThenStateShouldBe (ServedOrder order)
    |> WithEvents  [
        DrinkServed (coke, order.Tab.Id)
        OrderServed (order, payment) ]

[<Fact>]
let ``Remain in order in progress while serving drinks`` () =
    let order = { order with Drinks = [coke; lemonade;  appleJuice]}
    let orderInProgress = {Place order with ServedDrinks = [coke]}
    let expected = {orderInProgress with ServedDrinks = lemonade::orderInProgress.ServedDrinks}
    Given (OrderInProgress orderInProgress)
    |> When (ServeDrink (lemonade, order.Tab.Id))
    |> ThenStateShouldBe (OrderInProgress expected)
    |> WithEvents [DrinkServed (lemonade, order.Tab.Id)]

[<Fact>]
let ``Can not serve non ordered drinks during order in progress`` () =
    let order = {order with Drinks = [coke;lemonade]}
    let orderInProgress = {Place order with ServedDrinks = [coke]}
    Given (OrderInProgress orderInProgress)
    |> When (ServeDrink (appleJuice, order.Tab.Id))
    |> ShouldFailWith (CanNotServeNonOrderedDrink appleJuice)

[<Fact>]
let ``Can not serve already served drinks`` () =
    let order = {order with Drinks = [coke;lemonade]}
    let oip = {Place order with ServedDrinks=[coke]}
    Given (OrderInProgress oip)
    |> When (ServeDrink (coke, order.Tab.Id))
    |> ShouldFailWith (CanNotServeAlreadyServedDrink coke)