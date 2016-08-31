module Domain
open System

type Tab = {
    Id: Guid
    TableNumber : int
}

type Item = {
    MenuNumber : int
    Price : decimal
    Name : string
}

type Food = Food of Item
type Drink = Drink of Item

type Payment = {
    Tab : Tab
    Amount : decimal    
}

type Order = {
    Foods : Food list
    Drinks : Drink list
    Tab : Tab
}

type InProgressOrder = {
    PlacedOrder : Order
    ServedDrinks : Drink list
    ServedFoods : Food list
    PreparedFoods : Food list
}


let foodPrice (Food food) = food.Price
let drinkPrice (Drink drink) = drink.Price

let orderAmount order =
    let foodAmount = order.Foods |> List.map foodPrice |> List.sum
    let drinksAmount = order.Drinks|> List.map drinkPrice |> List.sum
    foodAmount + drinksAmount

let payment order = {Tab = order.Tab; Amount = orderAmount order}

let nonServedFoods ipo =
    List.except ipo.ServedFoods ipo.PlacedOrder.Foods
       
let nonServedDrinks ipo =
    ipo.PlacedOrder.Drinks 
    |> List.except ipo.ServedDrinks

let isServingDrinkCompletesIPOrder ipo drink =
    List.isEmpty (nonServedFoods ipo) && (nonServedDrinks ipo) = [drink]

let isServingFoodCompletesIPOrder ipo food =
    List.isEmpty (nonServedDrinks ipo) && (nonServedFoods ipo) = [food]

