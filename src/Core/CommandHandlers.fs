module CommandHandlers

open Events
open System
open Domain
open Commands
open Chessie.ErrorHandling
open States
open Errors

let todo () = failwith "todo"

let handleOpenTab tab = function
    | ClosedTab _ -> [TabOpened tab] |> ok
    | _ -> TabAlreadyOpened |> fail

let handlePlaceOrder order = function
    | OpenedTab _ ->
        if (List.isEmpty order.Drinks) && (List.isEmpty order.Foods) then
            CanNotPlaceEmptyOrder |> fail 
        else
            [OrderPlaced order] |> ok
    | ClosedTab _ -> CanNotOrderWithClosedTab |> fail
    | _ -> fail OrderAlreadyPlaced

let (|NonOrderedDrink|_|) order drink =
    match List.contains drink order.Drinks with
    | false -> Some drink
    | true -> None

let (|NonOrderedFood|_|) order food =
    match List.contains food order.Foods with
    | false -> Some food
    | true -> None

let (|DrinkAlreadyServed|_|) orderInProgress drink =
    match List.contains  drink orderInProgress.ServedDrinks with
    | true -> Some drink
    | _ -> None

let (|FoodAlreadyServed|_|) orderInProgress food =
    match List.contains food orderInProgress.ServedFoods with
    | true -> Some food
    | _ -> None

let isServingDrinkCompletesOrder order drink =
    List.isEmpty order.Foods && order.Drinks = [drink]


let (|ServeDrinksCompletesOrder|_|) order drink = 
    match isServingDrinkCompletesOrder order drink with
    | true -> Some drink
    | false -> None

let (|FoodAlreadyPrepared|_|) oip food =
    if oip.PreparedFoods |> List.contains food then
        Some food
    else None

let (|ServeDrinkCompletesIPOrder|_|) ipo drink =
    match isServingDrinkCompletesIPOrder ipo drink with
    | true -> Some drink
    | false -> None
    
let handlePrepareFood food tabId = function
    | PlacedOrder order ->
        match food with
        | NonOrderedFood order _ -> CanNotPrepareNonOrderedFood food |> fail
        | _ -> [FoodPrepared (food, tabId)] |> ok
    | OrderInProgress oip -> 
        match food with
        | NonOrderedFood oip.PlacedOrder _ -> CanNotPrepareNonOrderedFood food |> fail
        | FoodAlreadyPrepared oip _ -> CanNotPrepareAlreadyPreparedFood food |> fail
        | _ -> [FoodPrepared (food, tabId)] |> ok
    | ServedOrder _ -> OrderAlreadyServed |> fail
    | ClosedTab _ -> CanNotPrepareWithClosedTab |> fail
    | _ -> failwith "todo"

let (|ServeFoodCompletesIPOrder|_|) ipo food =
    if isServingFoodCompletesIPOrder ipo food then Some food else None       
  
let handleServeDrink drink tabId = function
    | PlacedOrder order ->
        let dse = DrinkServed (drink, tabId)
        match drink with
        | NonOrderedDrink order dr -> CanNotServeNonOrderedDrink drink |> fail
        | ServeDrinksCompletesOrder order _ -> 
            let payment = {Tab = order.Tab; Amount = orderAmount order}
            dse :: [OrderServed (order, payment)] |> ok
        | _ -> [dse] |> ok
    | OrderInProgress ipo ->
        let order = ipo.PlacedOrder
        let drinkServed = DrinkServed (drink, order.Tab.Id)
        
        match drink with
        | NonOrderedDrink ipo.PlacedOrder _ -> CanNotServeNonOrderedDrink drink |> fail 
        | ServeDrinkCompletesIPOrder ipo _ -> 
            drinkServed :: 
                [OrderServed (order, payment order)] 
                |> ok
        | DrinkAlreadyServed ipo _ -> CanNotServeAlreadyServedDrink drink |> fail 
        |_ ->    
            [drinkServed] |> ok

    | ServedOrder _ -> OrderAlreadyServed |> fail
    | ClosedTab _ -> CanNotServeWithClosedTab |> fail
    | OpenedTab _ -> CanNotServeForNonPlacedOrder |> fail

let (|FoodIsNotPrepared|_|) ipo food =
    match List.contains food ipo.PreparedFoods with
    | false -> Some food
    | true -> None

let handleServeFood food tabId = function
    | OrderInProgress ipo -> 
        let order = ipo.PlacedOrder
        let foodServed = FoodServed (food, tabId)
        let events = [foodServed]
        match food with
        | FoodAlreadyServed ipo _ -> CanNotServeAlreadyServedFood food |> fail
        | NonOrderedFood order _ -> CanNotServeNonOrderedFood food |> fail
        | FoodIsNotPrepared ipo _ -> CanNotServeNonPreparedFood food |> fail
        | ServeFoodCompletesIPOrder ipo _ -> 
            foodServed :: [OrderServed (order, payment order)] |> ok
        | _ -> events |> ok
    | PlacedOrder _ -> CanNotServeNonPreparedFood food |> fail
    | OpenedTab _ -> CanNotServeForNonPlacedOrder |> fail
    | ServedOrder _ -> OrderAlreadyServed |> fail
    | ClosedTab _ -> CanNotServeWithClosedTab |> fail

let handleCloseTab payment = function
    | ServedOrder order ->
        let orderAmount = orderAmount order
        if payment.Amount = orderAmount then 
            [TabClosed payment] |> ok
        else
            InvalidPayment (orderAmount, payment.Amount) |> fail
    | _ -> CanNotPayForNonServedOrder |> fail

let execute state command =
    match command with
    | OpenTab tab -> handleOpenTab tab state
    | PlaceOrder order -> handlePlaceOrder order state
    | ServeDrink (drink, tabId) -> handleServeDrink drink tabId state
    | PrepareFood (food, tabId) -> handlePrepareFood food tabId state
    | ServeFood (food, tabId) -> handleServeFood food tabId state
    | CloseTab payment -> handleCloseTab payment state

let evolve state command =
    match execute state command with
    | Ok (events, _) -> 
        let newState = List.fold apply state events
        (newState, events) |> ok
    | Bad err -> Bad err

