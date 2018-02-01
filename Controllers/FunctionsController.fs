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
open Starikov.dbModels
open Microsoft.Extensions.DependencyModel.Resolution
open Microsoft.AspNetCore.Mvc.ModelBinding


type FunctionsController (context: IMyDBContext) =
    inherit Controller()
    member val ctx = context with get
    //[<Authorize>]
    member this.GroupInfo () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        let groups = this.ctx.GetGroups
        this.View(groups)
    member this.StudentInfo () =    // VERY SLOW!!! NEED PARALLELING, BUT PROBLEMS IN DB ACCESS 
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        let students = 
            this.ctx.GetStudents 
            |> Seq.map (fun s ->
                                        let acc = this.ctx.GetAccount ((s.id_man).ToString())
                                        let personVal = 
                                            acc.Person 
                                            |> Commands.Getter 
                                            |> Array.zip ((acc.Person :> ICafedraEntities).GetNamesOfProperties())
                                            |> Array.Parallel.map (fun (n, (f, s)) -> new CSharpDuoTurple(PrName = n, PrRealName = f, PrValue = s))
                                        let studVal = 
                                            acc.Student 
                                            |> Commands.Getter 
                                            |> Array.zip ((acc.Student :> ICafedraEntities).GetNamesOfProperties()) 
                                            |> Array.tail 
                                            |> Array.Parallel.map (fun (n, (f, s)) -> new CSharpDuoTurple(PrName = n, PrRealName = f, PrValue = s))
                                        Array.concat (seq { yield personVal; yield studVal}) )
        this.View(students)
    