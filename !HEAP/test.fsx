let S size = seq {
    let cntr = ref size
    while !cntr > 0 do
        yield !cntr
        System.Threading.Thread.Sleep(10)
        cntr := !cntr - 1
}
S 9000
type MyTType () =
    member val ``Ky-ka-re-ky`` = 0 with get, set
let t = typeof<MyTType>    
t.Name

",mystring lul" |> Seq.tail |> (fun s -> s |> Seq.fold (fun f sd -> sprintf "%s%c" f sd) "") |> printfn "%s"
let method (param) = 
    let p = defaultArg param 2
    p * p
method (Some(4))    

let date = System.DateTime.Now
date.ToString() |> Seq.map (fun c -> if c = '.' then '-' else c) |> Seq.fold (sprintf "%s%c") ""

let ConvertDate = (fun (t: string) ->
        let hls = t.Split([|' '|])
        let dmy, tm = hls.[0], hls.[1]
        let ymd = dmy.Split([|'.'|]) |> Array.rev |> Array.fold (fun acc t -> sprintf "%s%s-" acc t) "" |> (fun x -> x.Substring(0, x.Length - 2))
        sprintf "%s %s" ymd tm
    )
ConvertDate <| date.ToString()    