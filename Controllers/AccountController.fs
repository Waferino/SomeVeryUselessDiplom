namespace Starikov.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open System.Security.Claims
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Authorization
//open System.ComponentModel.DataAnnotations
open Starikov
open System.Security.Claims
open Starikov.dbModels

[<Authorize>]
type AccountController (context: IMyDBContext) =
    inherit Controller()

    member val ctx = context with get

    [<HttpGet>]
    [<AllowAnonymous>]
    member this.Login () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.View()

    [<HttpPost>]
    [<AllowAnonymous>]
    member this.Login (logInfo: LoginViewModel) =
        let Authenticate user_id role =
            let claims = new List<Claim>()
            claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, (user_id |> string)))
            claims.Add(new Claim("PSTU_Role", role))
            let id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType)
            this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id))
        let ActionForCorrectLogined _Login = printfn "\"%s\" is entered!" _Login; this.RedirectToAction("Index", "Home")
        let ActionForWrongLogined = printfn "Wrong Login or Identity!"; this.RedirectToAction("Login", "Account", "Wrong!")
        if logInfo.Login = null || logInfo.Identity = null || logInfo.Login = "" || logInfo.Identity = "" then ActionForWrongLogined
        else
            match this.ctx.LogInForStudent logInfo with
                | Some(x) -> 
                    Authenticate x.Person.id_man "student" |> ignore
                    ActionForCorrectLogined logInfo.Login
                | None -> 
                    match this.ctx.LogInForCurator logInfo with
                        | Some(x) -> 
                            Authenticate x.Person.id_man "curator" |> ignore
                            ActionForCorrectLogined logInfo.Login
                        | None -> ActionForWrongLogined
    member this.Logout () =
        this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme) |> ignore
        this.RedirectToAction("Index", "Home")
    member this.ManageAccount () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.View()
    member this.Info () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        let man_id = this.User.Identity.Name
        let acc = this.ctx.GetAccount man_id
        //let persPrNames = (acc.Person :> ICafedraEntities).GetNamesOfProperties
        let personVal = acc.Person |> Commands.Getter |> Array.zip ((acc.Person :> ICafedraEntities).GetNamesOfProperties()) |> Array.map (fun (n, (f, s)) -> new CSharpDuoTurple(PrName = n, PrRealName = f, PrValue = s))
        //let studPrNames = (acc.Student :> ICafedraEntities).GetNamesOfProperties
        let studVal = acc.Student |> Commands.Getter |> Array.zip ((acc.Student :> ICafedraEntities).GetNamesOfProperties()) |> Array.tail |> Array.map (fun (n, (f, s)) -> new CSharpDuoTurple(PrName = n, PrRealName = f, PrValue = s))
        let ret = Array.concat (seq { yield personVal; yield studVal})
        this.View(ret)
        
        
        
        