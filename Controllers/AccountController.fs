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
open Microsoft.AspNetCore.Http.Extensions
open Starikov

[<Authorize>]
type AccountController (context: IMyDBContext) =
    inherit Controller()

    member val ctx = context with get

    [<HttpGet>]
    [<AllowAnonymous>]
    member this.Register () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.View(new RegisterViewModel())
    [<HttpPost>]
    [<AllowAnonymousAttribute>]
    member this.Register (regInfo: RegisterViewModel(*Lastname: string, Firstname: string, Midlename: string, Email: string, Password: string, rePasword: string, SpecialWord: string*)) =
        //let regInfo = new RegisterViewModel(Lastname = Lastname(*, Firstname = Firstname, Midlename = Midlename, Email = Email, Password = Password, rePassword = rePasword, SpecialWord = SpecialWord*))
        printfn "User(%s) trying to register..." regInfo.Email
        let Authenticate user_id role =
            let claims = new List<Claim>()
            claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, (user_id |> string)))
            claims.Add(new Claim("PSTU_Role", role))
            let id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType)
            this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id))
        let self_user = 
            this.ctx.GetPeoples //|> Seq.toArray//|> Array.Parallel.partition
            |> Seq.filter (fun p -> (*(p.fam.ToLowerInvariant() = regInfo.Lastname.ToLowerInvariant()) && (p.name.ToLowerInvariant() = regInfo.Firstname.ToLowerInvariant()) && (p.otchestvo.ToLowerInvariant() = regInfo.Midlename.ToLowerInvariant()) && *)(p.e_mail.ToLowerInvariant() = regInfo.Email.ToLowerInvariant())) //|> fun (g, ng) -> g
            |> Seq.tryHead
        if self_user.IsNone || (regInfo.Password.ToLowerInvariant() <> regInfo.rePassword.ToLowerInvariant()) then this.RedirectToAction("Register", regInfo)
        else
            let user = self_user.Value
            let acc_op = this.ctx.GetAccounts |> Seq.filter (fun a -> user.id_man = a.id_man) |> Seq.tryHead
            if acc_op.IsSome then this.RedirectToAction("Register", regInfo)
            else
                let specialWord_curator = "ЯКуратор!Честно!"
                let acc = new Starikov.dbModels.Account(id_man = user.id_man, Email = regInfo.Email.ToLowerInvariant(), Password = regInfo.Password.ToLowerInvariant(), Role = "student", date_of_change = System.DateTime.Now.ToString("MM-dd-yyyy"))
                if regInfo.SpecialWord |> isNull |> not && regInfo.SpecialWord.ToLowerInvariant() = specialWord_curator.ToLowerInvariant() then acc.Role <- "curator"
                let res = this.ctx.InsertAccount acc
                if res then 
                    Authenticate acc.id_man acc.Role
                    this.RedirectToAction("Index", "Home")
                else
                    this.RedirectToAction("Register", regInfo)
    [<HttpGet>]
    [<AllowAnonymous>]
    member this.Login () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.View()
    [<AllowAnonymous>]
    [<HttpPost>]
    member this.Login (logInfo: LoginViewModel) =
        let Authenticate user_id role =
            let claims = new List<Claim>()
            claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, (user_id |> string)))
            claims.Add(new Claim("PSTU_Role", role))
            let id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType)
            this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id))//this.RedirectToAction("Login", "Account", "Wrong!")
        let acc_op = this.ctx.GetAccounts |> Seq.filter (fun a -> a.Email = logInfo.Login && a.Password = logInfo.Identity) |> Seq.tryHead
        if acc_op.IsNone then this.RedirectToAction("Login", "Account", "Wrong!")
        else
            let acc = acc_op.Value
            Authenticate acc.id_man acc.Role
            this.RedirectToAction("Index", "Home")
    member this.Logout () =
        this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme) |> ignore
        this.RedirectToAction("Index", "Home")
    member this.ManageAccount () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.ViewData.["FIO"] <- this.ctx.GetFIO this.User.Identity.Name
        this.View()
    member this.Info () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.ViewData.["FIO"] <- this.ctx.GetFIO this.User.Identity.Name
        let man_id = this.User.Identity.Name
        let acc = this.ctx.GetAccount man_id
        //let persPrNames = (acc.Person :> ICafedraEntities).GetNamesOfProperties
        let personVal = acc.Person |> Commands.Getter |> Array.zip ((acc.Person :> ICafedraEntities).GetNamesOfProperties()) |> Array.map (fun (n, (f, s)) -> new CSharpDuoTurple(PrName = n, PrRealName = f, PrValue = s))
        //let studPrNames = (acc.Student :> ICafedraEntities).GetNamesOfProperties
        let studVal = acc.Student |> Commands.Getter |> Array.zip ((acc.Student :> ICafedraEntities).GetNamesOfProperties()) |> Array.tail |> Array.map (fun (n, (f, s)) -> new CSharpDuoTurple(PrName = n, PrRealName = f, PrValue = s))
        let ret = Array.concat (seq { yield personVal; yield studVal})
        this.View(ret)
    member this.ChangePassword () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.ViewData.["FIO"] <- this.ctx.GetFIO this.User.Identity.Name
        this.View()
    [<HttpPost>]
    member this.ChangePassword (newPass: string, retrynewPass: string) =
        if newPass <> retrynewPass then this.RedirectToAction("ChangePassword")
        else
            let id_man = this.User.Identity.Name |> int
            let acc = this.ctx.GetAccounts |> Seq.filter (fun a -> a.id_man = id_man) |> Seq.head
            let new_acc = new Account(id_man = acc.id_man, Email = acc.Email, Password = newPass, Role = acc.Role, date_of_change = System.DateTime.Now.ToString("MM-dd-yyyy"))
            let query, logs = QueryBuilder.BuildUpdateQuery acc new_acc
            (this.ctx :?> IBaseSQLCommands).Execute query logs |> ignore
            this.RedirectToAction("ManageAccount")
    member this.ChangeEmail () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.ViewData.["FIO"] <- this.ctx.GetFIO this.User.Identity.Name
        this.View()
    [<HttpPost>]
    member this.ChangeEmail (newEmail: string) =
        if newEmail = "" || newEmail |> isNull then this.RedirectToAction("ChangePassword")
        else
            let id_man = this.User.Identity.Name |> int
            let acc = this.ctx.GetAccounts |> Seq.filter (fun a -> a.id_man = id_man) |> Seq.head
            let new_acc = new Account(id_man = acc.id_man, Email = newEmail, Password = acc.Password, Role = acc.Role, date_of_change = System.DateTime.Now.ToString("MM-dd-yyyy"))
            let query, logs = QueryBuilder.BuildUpdateQuery acc new_acc
            (this.ctx :?> IBaseSQLCommands).Execute query logs |> ignore
            this.RedirectToAction("ManageAccount")