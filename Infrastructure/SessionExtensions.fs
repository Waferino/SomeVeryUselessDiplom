namespace Starikov.Infrastructure

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.Features
open Newtonsoft.Json

module SessionExtensions =
    type ISession with
        member this.SetJson (key, value: obj) =
            this.SetString(key, JsonConvert.SerializeObject(value))
        member this.GetJson<'T> (key) =
            let sessionData = this.GetString(key)
            match this.GetString(key) with
            | null -> Unchecked.defaultof<'T>
            | data ->  JsonConvert.DeserializeObject<'T>(data)