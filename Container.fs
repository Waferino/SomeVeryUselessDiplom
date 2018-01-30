namespace Starikov

open System
open System.Collections.Generic

type EnteredAccount () =
    member val IsEnter : bool = false with get, set

type AccountContainer () =
    static member val Accounts : List<EnteredAccount> = null with get, set
    static member AddNewEnteredAccount(target) = AccountContainer.Accounts.Add(target)