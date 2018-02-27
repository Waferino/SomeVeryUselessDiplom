type Test_type () =
    member val first_prop = Unchecked.defaultof<string> with get, set
    member val second_prop = Unchecked.defaultof<float> with get, set
    member val third_prop = Unchecked.defaultof<System.DateTime> with get, set
let T = typeof<Test_type>
let GetProperties t =
    let T = t.GetType()
    T.GetProperties() :> seq<System.Reflection.PropertyInfo>
let GetValues t =
    let T = t.GetType()
    T.GetProperties() |> Seq.map (fun pi -> pi.GetValue(t :> obj))
let GetPropertiesAndValues t =
    Seq.zip (GetProperties t) (GetValues t)
let GetUpdateCommand (deft: 'T) (targ: 'T) =
    let dl', tl' = GetPropertiesAndValues <| deft, GetPropertiesAndValues <| targ
    let rec builder (names: string) (values: string) (dl: seq<System.Reflection.PropertyInfo * obj>, tl: seq<System.Reflection.PropertyInfo * obj>) = 
        match dl, tl with
        | s1, s2 when Seq.isEmpty <| s1 && Seq.isEmpty <| s2 -> names, values
        | d, t ->
            let (dpi, dv), (tpi, tv) = Seq.head <| d, Seq.head <| t
            if dv = tv then builder names values (Seq.tail <| d, Seq.tail <| t)
            else builder (sprintf "%s,`%s`" names (dpi.Name)) (sprintf "%s,'%O'" values tv) (Seq.tail <| d, Seq.tail <| t)
    let ret1, ret2 = builder "" "" (dl', tl')
    ret1.Substring(1), ret2.Substring(1)
//let rec W acc = function | s when Seq.isEmpty <| s -> acc | s -> W ((Seq.head <| s) :: acc) (Seq.tail <| s)
let t1 = new Test_type ()
let t2 = new Test_type (first_prop = "first", third_prop = System.DateTime.Now)
GetUpdateCommand t1 t2