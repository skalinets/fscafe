module PrepareFoodTests

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
let ``Can prepare food`` () =
    let order = {order with Foods = [salad]}
    let expected = {
        PlacedOrder = order
        ServedDrinks = []
        PreparedFoods = [salad]
        ServedFoods = []
    }
    Given (PlacedOrder order)
    |> When (PrepareFood (salad, order.Tab.Id))
    |> ThenStateShouldBe (OrderInProgress expected)
    |> WithEvents [FoodPrepared (salad, order.Tab.Id)]

[<Fact>]
let ``Can not prepare a non-ordered food`` () =
    let order = {order with Foods = [pizza]}
    Given (PlacedOrder order)
    |> When (PrepareFood (salad, order.Tab.Id))
    |> ShouldFailWith (CanNotPrepareNonOrderedFood salad)

[<Fact>]
let ``Can not prepare a food for served order`` () =
    Given (ServedOrder order)
    |> When (PrepareFood (pizza, order.Tab.Id))
    |> ShouldFailWith OrderAlreadyServed

[<Fact>]
let ``Can not prepare with closed tab`` () =
    Given (ClosedTab None)
    |> When (PrepareFood (salad, order.Tab.Id))
    |> ShouldFailWith CanNotPrepareWithClosedTab

[<Fact>]
let ``Can prepare food during order in progress`` () =
    let order = {order with Foods = [salad;pizza]}
    let oip = {Place order with PreparedFoods = [pizza]}
    let expected = {oip with PreparedFoods = [salad;pizza]}
    Given (OrderInProgress oip)
    |> When (PrepareFood (salad, order.Tab.Id))
    |> ThenStateShouldBe (OrderInProgress expected)
    |> WithEvents [FoodPrepared (salad, order.Tab.Id)]

[<Fact>]
let ``Can not prepare non-ordered food during order in progress`` () =
    let order = {order with Foods = [salad]}
    let oip = Place order
    Given (OrderInProgress oip)
    |> When (PrepareFood (pizza, order.Tab.Id))
    |> ShouldFailWith (CanNotPrepareNonOrderedFood pizza)

[<Fact>]
let ``Can not prepare already prepared food during order in progress`` () =
    let order = {order with Foods = [salad]}
    let oip = {Place order with PreparedFoods = [salad]}
    Given (OrderInProgress oip)
    |> When (PrepareFood (salad, order.Tab.Id))
    |> ShouldFailWith (CanNotPrepareAlreadyPreparedFood salad)