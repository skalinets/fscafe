module OpenTab
open FSharp.Data
open System
open Domain
open ApiCommandHandlers
open Commands

[<Literal>]
let OpenTabJson = """ {
    "openTab": {
        "tableNumber" : 1
    }
}
"""

type OpenTabReq = JsonProvider<OpenTabJson>

let (|OpenTabRequest|_|) payLoad =
    try
        let req = OpenTabReq.Parse(payLoad).OpenTab
        {Id = Guid.NewGuid(); TableNumber = req.TableNumber}
        |> Some
    with
    | ex -> None

let validateOpenTab getTableByTableNumber tab = async {
    let! table = getTableByTableNumber tab.TableNumber
    match table with
    | Some table -> 
        return Choice1Of2 tab
    | _ ->
        return Choice2Of2 "Invalid Table Number"
}

let openTabCommander getTableByTableNumber = {
    Validate = validateOpenTab getTableByTableNumber
    ToCommand = OpenTab
}