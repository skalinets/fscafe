module EventStore

open States

let getStateFromEvents events = 
    events
    |> Seq.fold apply (ClosedTab None)
