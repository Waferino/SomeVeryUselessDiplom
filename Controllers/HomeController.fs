namespace Starikov.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Authorization
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
//open System.ComponentModel.DataAnnotations
open Starikov
open Microsoft.Extensions.DependencyModel.Resolution
open Microsoft.AspNetCore.Mvc.ModelBinding


type HomeController (context: IMyDBContext) =
    inherit Controller()
    member val ctx = context with get
    //[<Authorize>]
    member this.Index () =
        let IsAuthenticated = this.User.Identity.IsAuthenticated 
        this.ViewData.["IsAuthenticated"] <- IsAuthenticated
        if IsAuthenticated then 
            let AccountInfo = this.User.Identity.Name |> this.ctx.GetAccount
            this.ViewData.["AccountInfo"] <- AccountInfo
        this.View()

    member this.About () =
        this.ViewData.["Message"] <- "Your application description page."
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.View()

    member this.Contact () =
        this.ViewData.["Message"] <- "Igor Starikov"
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.View()

    member this.Error () =
        this.View();
