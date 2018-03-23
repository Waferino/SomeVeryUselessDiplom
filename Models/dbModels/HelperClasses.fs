namespace Starikov.dbModels
open Unchecked

type AccountInfo () =
    member val Person = defaultof<People> with get, set
    member val IsStudent = false with get, set
    member val Student = defaultof<Student> with get, set
type CSharpDuoTurple () =
    member val PrName = defaultof<string> with get, set
    member val PrRealName = defaultof<string> with get, set
    member val PrValue = defaultof<obj> with get, set