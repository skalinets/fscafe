module CommandApi

open System.Text
open Queries
open OpenTab
open ApiCommandHandlers
open Chessie.ErrorHandling

let a = 1

let handleCommandRequest queries eventStore = function
    | OpenTabRequest tab ->
        queries.Table.GetTableByTableNumber
        |> openTabCommander
        |> handleCommand eventStore tab
    | _ -> err "Invalid command" |> fail |> async.Return