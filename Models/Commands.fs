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
    let TypeSetter (tp: System.Type) values =
        let target = Activator.CreateInstance(tp)
        let Set (pr_info: Reflection.PropertyInfo) value =
            try
                pr_info.SetValue(target, value)
            with
                | _ -> printfn "Bad instance with Property \"%s\"\tT: %s" pr_info.Name tp.Name
        let pi = tp.GetProperties()
        if pi.Length <> (Array.length <| values) then failwith "Error! Setter_arguments"
        for i = 0 to pi.Length - 1 do
            Set pi.[i] values.[i]
        target       
    let Getter target =
        let t = target.GetType()
        let pri = t.GetProperties()
        [| for pi in pri do yield (pi.Name, pi.GetValue(target)) |]

    let ConvertDate = (fun (t: string) ->
        let hls = t.Split([|' '|])
        let dmy, tm = hls.[0], hls.[1]
        let ymd = dmy.Split([|'.'|]) |> Array.rev |> Array.fold (fun acc t -> sprintf "%s%s-" acc t) "" |> (fun x -> x.Substring(0, x.Length - 1))
        sprintf "%s %s" ymd tm
    )
    let SC man_id (a: 'T, dt: 'T) =    //SmartChecker
        if a <> Unchecked.defaultof<'T> && a <> dt then 
            printfn "For_Account(%s):\tOld value: %O\t->\tNew value: %O" man_id dt a
            true
        else false