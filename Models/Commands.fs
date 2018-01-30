namespace Starikov
module Commands =
    (*let BuildAccount = 
        let account = new EnteredAccount()
        account.IsEnter <- true
        //AccountContainer.AddNewEnteredAccount account
        account*)
    
    open System
    open Starikov.dbModels
    let Setter target values =
        let Set (pr_info: Reflection.PropertyInfo) value =
            try
                pr_info.SetValue(target, value)
            with
                | _ -> printfn "Bad instance with Property \"%s\"\tT: %A" pr_info.Name target
        let T = target.GetType()
        let pi = T.GetProperties()
        if pi.Length <> (Array.length <| values) then failwith "Error! Setter_arguments"
        for i = 0 to pi.Length - 1 do
            Set pi.[i] values.[i]
        target
    let Getter target =
        let t = target.GetType()
        let pri = t.GetProperties()
        [| for pi in pri do yield (pi.Name, pi.GetValue(target)) |]