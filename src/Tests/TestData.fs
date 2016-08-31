module TestData

open Domain
open System

let coke = Drink { MenuNumber = 1; Name = "Coke"; Price = 1.5m }
let lemonade = Drink { MenuNumber = 2; Name = "Lemonade"; Price = 1.0m }
let appleJuice = Drink { MenuNumber = 2; Name = "Apple Juice"; Price = 1.0m }

let salad = Food { MenuNumber = 1; Name = "Salad"; Price = 1.0m }
let pizza = Food { MenuNumber = 2; Name = "Pizza"; Price = 1.6m }


let tab = {Id = Guid.NewGuid(); TableNumber = 1}
let order = {Tab = tab; Foods = []; Drinks = []}

let Place order = {
    PlacedOrder = order
    ServedDrinks = []
    PreparedFoods = []
    ServedFoods = []
}


