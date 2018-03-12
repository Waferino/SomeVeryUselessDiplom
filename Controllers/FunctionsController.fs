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
open Microsoft.AspNetCore.Http.Extensions
open System.Reflection.Metadata


type FunctionsController (context: IMyDBContext) =
    inherit Controller()
    member val ctx = context with get
    //[<Authorize>]
    member this.GroupInfo () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        if this.User.Identity.IsAuthenticated then this.ViewData.["FIO"] <- this.ctx.GetFIO this.User.Identity.Name
        let groups = this.ctx.GetGroups
        this.View(groups)
    member this.OneGroupInfo (id : int) =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        if this.User.Identity.IsAuthenticated then this.ViewData.["FIO"] <- this.ctx.GetFIO this.User.Identity.Name
        this.ViewData.["Group_name"] <- this.ctx.GetOneGroup id |> Option.map (fun g -> g.name_group) |> Option.get
        let students = 
            this.ctx.GetGroupStudents id
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
            |> Seq.map (fun a -> 
                                        if Commands.IsCurator <| this.User.Claims then a
                                        else [| a.[0]; a.[1]; a.[2]; a.[3]; a.[10]; a.[26]; a.[27]; a.[59]; a.[61] |] )                            
        this.View(students)
    member this.StudentInfo () =    // VERY SLOW!!! NEED PARALLELING, BUT PROBLEMS IN DB ACCESS 
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        if this.User.Identity.IsAuthenticated then this.ViewData.["FIO"] <- this.ctx.GetFIO this.User.Identity.Name
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
            |> Seq.map (fun a -> 
                                        if Commands.IsCurator <| this.User.Claims then a
                                        else [| a.[0]; a.[1]; a.[2]; a.[3]; a.[10]; a.[26]; a.[27]; a.[59]; a.[61] |] )
        this.View(students)
    member this.EventsInfo () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.ViewData.["IsStudent"] <- this.User.Claims |> Commands.IsStudent
        if this.User.Identity.IsAuthenticated then this.ViewData.["FIO"] <- this.ctx.GetFIO this.User.Identity.Name
        let EventsInfos = this.ctx.GetEventsInfos
        let retD = EventsInfos |> Seq.filter (fun ei -> ei.DateOfThe.HasValue) |> Seq.sortBy (fun ei -> ei.DateOfThe.Value)
        let retU = EventsInfos |> Seq.filter (fun ei -> not <| ei.DateOfThe.HasValue) |> Seq.sortBy (fun ei -> ei.Name)
        let ret = seq { for ei in retD -> ei
                        for ei in retU -> ei }
        let mid, gid = this.ctx.GetAccount this.User.Identity.Name |> (fun a -> (a.Person.id_man, (if a.IsStudent then (if a.Student.id_group.HasValue then a.Student.id_group.Value else 0) else 0) ) )
        let ct = this.ctx :?> IBaseSQLCommands
        let evts = ct.Get (new Starikov.dbModels.Event())
        this.ViewData.["meEvents"] <- evts |> Seq.filter (fun e -> if e.isGroup_Event then e.fk_student_or_group = gid else e.fk_student_or_group = mid) |> Seq.map (fun e -> printfn "id_EI: %A"; e.id_EventInfo)
        this.ViewData.["groupEvents"] <- evts |> Seq.filter (fun e -> e.isGroup_Event) |> Seq.filter (fun e -> e.fk_student_or_group = gid) |> Seq.map (fun e -> printfn "id_EI: %A"; e.id_EventInfo)
        this.View(ret)
    [<Authorize>]
    member this.CreateEvent () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.ViewData.["FIO"] <- this.ctx.GetFIO this.User.Identity.Name
        this.ViewData.["id_man"] <- this.User.Identity.Name |> int
        this.View(new Starikov.dbModels.EventInfo())
    [<Authorize>]
    [<HttpPost>]
    member this.CreateEvent (event: EventInfo) = //INSERT INTO `www0005_base`.`eventinfo` (`DateOfThe`, `Name`) VALUES ('2010.11.10', 'Поход на лыжах');
        let sei = this.ctx.GetEventsInfos |> Seq.filter (fun ei -> ei.id_EventInfo = event.id_EventInfo) |> Seq.tryHead
        if sei.IsNone then
            let res = this.ctx.InsertEventInfo event
            res |> ignore
        else 
            let ei = sei.Value
            let query, logs = QueryBuilder.BuildUpdateQuery ei event
            (this.ctx :?> IBaseSQLCommands).Execute query logs |> ignore
        //if res then printfn "Event: \"%s\" was inserted" event.Name else printfn "Broken inserting Event: \"%s\"" event.Name
        this.RedirectToAction("EventsInfo")
    [<Authorize>]
    member this.EditEventInfo (id: int) =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.ViewData.["FIO"] <- this.ctx.GetFIO this.User.Identity.Name
        this.ViewData.["id_man"] <- this.User.Identity.Name |> int
        let ei = this.ctx.GetEventsInfos |> Seq.filter (fun ei -> ei.id_EventInfo = id) |> Seq.head
        this.View(ei)
    [<Authorize>]
    member this.RemoveEventInfo (id: int) =
        let res = this.ctx.GetEventsInfos |> Seq.filter (fun ei -> ei.id_EventInfo = id) |> Seq.head |> this.ctx.Remove
        this.RedirectToAction("EventsInfo")
    [<Authorize(Policy = "StudentOnly")>]
    member this.CheckInEvent (id_ei: int) =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.ViewData.["FIO"] <- this.ctx.GetFIO this.User.Identity.Name
        this.ViewData.["id_EventInfo"] <- id_ei
        printfn "ID: %A" id_ei
        let model = new Starikov.dbModels.Event(id_EventInfo = id_ei)
        this.View(model)
    [<Authorize(Policy = "StudentOnly")>]
    [<HttpPost>]
    member this.CheckInEvent (event: Starikov.dbModels.Event) =
        let id_man = this.User.Identity.Name |> int
        printfn "ID: %A" event.id_EventInfo
        if event.isGroup_Event then
            let id_group = this.ctx.GetStudents |> Seq.filter (fun s -> s.id_man = id_man) |> Seq.map (fun s -> s.id_group) |> Seq.head
            if id_group.HasValue then 
                event.fk_student_or_group <- id_group.Value
            else
                event.isGroup_Event <- false
                event.fk_student_or_group <- id_man
        else
            event.fk_student_or_group <- id_man
        event.creatingDate <- (System.DateTime.Now.ToString("MM-dd-yyyy"))
        let res = this.ctx.InsertEvent event
        this.RedirectToAction("EventsInfo")
    [<Authorize>]
    member this.StudentAnceta () =
        this.ViewData.["IsAuthenticated"] <- this.User.Identity.IsAuthenticated
        this.ViewData.["FIO"] <- this.ctx.GetFIO this.User.Identity.Name
        let groups = this.ctx.GetGroups |> Seq.map (fun g -> g.name_group)
        this.ViewData.["Groups"] <- groups
        this.View(this.ctx.GetAnceteData <| this.User.Identity.Name)
    [<Authorize>]
    [<HttpPost>]
    member this.StudentAnceta (ancet: Anceta) =
        let res = this.ctx.SetAnceteData this.User.Identity.Name ancet
        this.RedirectToAction("Index", "Home")