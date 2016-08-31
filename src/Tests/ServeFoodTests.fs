module ServeFoodTests

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
let ``Can maintain order in progress while serving food`` () =
    let order = {order with Foods = [salad; pizza]}
    let oip = {Place order with PreparedFoods = [salad; pizza]}
    let expected = {oip with ServedFoods=[salad]}
    
    Given (OrderInProgress oip)
    |> When (ServeFood (salad, order.Tab.Id))
    |> ThenStateShouldBe (OrderInProgress expected)
    |> WithEvents [FoodServed (salad, order.Tab.Id)]

[<Fact>]
let ``Can serve only prepared food`` () =
    let order = {order with Foods = [salad;pizza]}  
    let orderInProgress = { Place order with PreparedFoods = [salad]  }
    Given (OrderInProgress orderInProgress)  
    |> When (ServeFood (pizza, order.Tab.Id))  
    |> ShouldFailWith (CanNotServeNonPreparedFood pizza)

[<Fact>]
let ``Can not serve non-ordered food`` () =  
    let order = {order with Foods = [salad;]}  
    let orderInProgress = { Place order with PreparedFoods = [salad] }
    Given (OrderInProgress orderInProgress)  
    |> When (ServeFood (pizza, order.Tab.Id))  
    |> ShouldFailWith (CanNotServeNonOrderedFood pizza)

[<Fact>] 
let ``Can not serve already served food`` () =  
    let order = {order with Foods = [salad;pizza]}  
    let orderInProgress = { Place order with ServedFoods = [salad]; PreparedFoods = [pizza]  }
    Given (OrderInProgress orderInProgress)  
    |> When (ServeFood (salad, order.Tab.Id))  
    |> ShouldFailWith (CanNotServeAlreadyServedFood salad)

[<Fact>]
let ``Can not serve for placed order`` () =
    Given (PlacedOrder order)
    |> When (ServeFood (salad, order.Tab.Id))
    |> ShouldFailWith (CanNotServeNonPreparedFood salad)

[<Fact>]
let ``Can not serve for non placed order`` () =
    Given (OpenedTab tab)
    |> When (ServeFood (salad, order.Tab.Id))
    |> ShouldFailWith (CanNotServeForNonPlacedOrder)
 
[<Fact>] 
let ``Can not serve for already served order`` () =
    Given (ServedOrder order)
    |> When (ServeFood (salad, order.Tab.Id))
    |> ShouldFailWith OrderAlreadyServed

[<Fact>]
let ``Can not serve with closed tab`` () =
    Given (ClosedTab None)
    |> When (ServeFood (salad, order.Tab.Id))
    |> ShouldFailWith CanNotServeWithClosedTab
     
