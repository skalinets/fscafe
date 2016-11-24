module InMemory

open Table
open Chef
open Waiter
open Cashier
open Projections
open Queries
open EventStore
open NEventStore

type InMemoryEventStore () =
    static member Instance =
                       Wireup.Init()
                        .UsingInMemoryPersistence()
                        .Build()

let inMemoryEventStore () =
    let eventStoreInstance = InMemoryEventStore.Instance
    {
        GetState = getState eventStoreInstance
        SaveEvent = saveEvent eventStoreInstance    
    }

let toDoQueries = {
    GetChefToDos = getChefToDos
    GetCashierToDos = getCashierToDos
    GetWaiterToDos = getWaiterToDos
}

let inMemoryQueries = {
    Table = tableQueries
    ToDo = toDoQueries
}

let inMemoryActions = {
    Table = tableActions
    Chef = chefActions
    Waiter = waiterActions
    Cashier = cashierActions
}
//
//let commandApiHandler eventStore (context : HttpContext) = async {
//    let payload = Encoding.UTF8.GetString context.request.rawForm
//    let! response =
//        handleCommandRequest
//            inMemoryQueries eventStore payload
//    match response with
//    | Ok ((state,events), _) ->
//        return! OK (sprintf "%A" state) context
//    | Bad (err) ->
//        return! BAD_REQUEST err.Head.Message context
//}
//
//let commandApi eventStore =
//    path "/command"
//        >=> POST
//        >=> commandApiHandler eventStore
//
//[<EntryPoint>]
//let main argv =
//    let app =
//        let eventStore = inMemoryEventStore ()
//        choose [
//            commandApi eventStore
//        ]
//
//    let cfg =
//        {defaultConfig with
//            bindings = [HttpBinding.mkSimple HTTP "0.0.0.0" 8083]}
//    startWebServer cfg app
//    0
//
//
