module OpenTab
open FSharp.Data

[<Literal>]
let OpenTabJson = """ {
    "openTab": {
        "tableNumber" : 1
    }
}
"""

type OpenTabReq = JsonProvider<OpenTabJson>
