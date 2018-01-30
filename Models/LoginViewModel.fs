namespace Starikov

open System

type LoginViewModel () =
    member val Login : string = null with get, set

    member val Identity : string = null with get, set