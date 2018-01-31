namespace Starikov.dbModels
open Unchecked

type Account () =
    member val Person = defaultof<Person> with get, set
    member val IsStudent = false with get, set
    member val Student = defaultof<Student> with get, set
type CSharpDuoTurple () =
    member val PrName = defaultof<string> with get, set
    member val PrRealName = defaultof<string> with get, set
    member val PrValue = defaultof<obj> with get, set