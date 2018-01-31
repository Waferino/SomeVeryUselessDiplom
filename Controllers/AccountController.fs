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
    member this.Login (Login : string, Identity: string) =
        let Authenticate user_id role =
            let claims = new List<Claim>()
            claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, (user_id |> string)))
            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role))
            let id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType)
            this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id))
        let ActionForCorrectLogined _Login = printfn "\"%s\" is entered!" _Login; this.RedirectToAction("Index", "Home")
        let ActionForWrongLogined = printfn "Wrong Login or Identity!"; this.RedirectToAction("Login", "Account", "Wrong!")
        if Login = null || Identity = null || Login = "" || Identity = "" then ActionForWrongLogined
        else
            let target = 
                let ret = new LoginViewModel()
                ret.Login <- Login
                ret.Identity <- Identity
                ret
            match this.ctx.LogInForStudent target with
                | Some(x) -> 
                    Authenticate x.Person.id_man "student" |> ignore
                    ActionForCorrectLogined Login
                | None -> 
                    match this.ctx.LogInForCurator target with
                        | Some(x) -> 
                            Authenticate x.Person.id_man "curator" |> ignore
                            ActionForCorrectLogined Login
                        | None -> ActionForWrongLogined
    member this.Logout () =
        this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme) |> ignore
        this.RedirectToAction("Index", "Home")
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
        
        
        
        