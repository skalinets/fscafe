module States

open Domain
open System
open Events

type State =
    | ClosedTab of Guid option
    | OpenedTab of Tab
    | PlacedOrder of Order
    | OrderInProgress of InProgressOrder
    | ServedOrder of Order

let apply state event =
    match state, event with
    | ClosedTab _, TabOpened tab -> OpenedTab tab
    | OpenedTab _, OrderPlaced order -> PlacedOrder order
    | PlacedOrder order, DrinkServed (item, _) ->
        {
            PlacedOrder = order
            ServedDrinks = [item]
            ServedFoods = []
            PreparedFoods = []
        } |> OrderInProgress
    | OrderInProgress ipo, OrderServed _ -> ServedOrder ipo.PlacedOrder
    | OrderInProgress ipo, DrinkServed (item, _) -> 
        OrderInProgress {ipo with ServedDrinks = item::ipo.ServedDrinks}
    | PlacedOrder order, FoodPrepared (item, _) ->
        {
            PlacedOrder = order
            ServedDrinks = []
            ServedFoods = []
            PreparedFoods = [item]
        } |> OrderInProgress
    | OrderInProgress ipo, FoodPrepared (food, _) -> 
        OrderInProgress {ipo with PreparedFoods = food :: ipo.PreparedFoods}
    | OrderInProgress ipo, FoodServed (food, _) ->
        OrderInProgress {ipo with ServedFoods = food :: ipo.ServedFoods}  
    | ServedOrder order, TabClosed payment -> ClosedTab (Some payment.Tab.Id)
    | _ -> state





