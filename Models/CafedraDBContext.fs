namespace Starikov

open System
open System.Collections.Generic
open MySql.Data.MySqlClient

open Starikov.dbModels
open MySql.Data
open System.Text.RegularExpressions
open Microsoft.Data.Edm
open Microsoft.AspNetCore.Http.Extensions

type CafedraDBContext() =
    member val ConnectionString = @"server=localhost;userid=root;password=kagura;persistsecurityinfo=True;database=www0005_base" with get, set
    member private __.GetSqlConnection = new MySqlConnection(__.ConnectionString)
    interface IBaseSQLCommands with
        member this.Execute query logs =
            try
                use conn = this.GetSqlConnection
                conn.Open()
                let cmd = new MySqlCommand(query, conn)
                use reader = cmd.ExecuteReader()
                let mutable ret = "*"
                while reader.Read() do 
                    ret <- ret + "*"
                conn.Close()
                Logs.add_ExtLog query logs
                ret
            with
                | ex -> 
                    printfn "QUERY: %s" query
                    raise ex
        member this.Get table = 
            let ret = new List<obj []>()
            use conn = this.GetSqlConnection
            conn.Open()
            let cmd = new MySqlCommand((sprintf "SELECT * FROM `%s`" table), conn)
            use reader = cmd.ExecuteReader()
            let fields = reader.FieldCount
            while reader.Read() do
                //ret.Add(reader.GetString("fam"))
                let mutable internArr = 
                    [|for i = 0 to fields - 1 do
                        yield reader.GetValue(i)|]
                ret.Add(internArr)
            ret :> seq<obj []>
        member this.Get (t: 'T) =
            let tp = t.GetType()
            (this :> IBaseSQLCommands).Get tp.Name
            |> Seq.map (fun v -> (Commands.TypeSetter tp v) :?> 'T)
        member this.GetFromType tp = 
            let query = sprintf "SELECT * FROM `%s`" tp.Name
            use conn = this.GetSqlConnection
            conn.Open()
            let cmd = new MySqlCommand(query, conn)
            use reader = cmd.ExecuteReader()
            let fields = reader.FieldCount
            try
                let ret = seq { while reader.Read() do yield Commands.TypeSetter tp [| for i = 0 to fields - 1 do yield reader.GetValue(i) |] }
                Some <| (ret :?> seq<'T>)
            with
                | _ -> None
        member this.GetWhere table def =
            let ret = new List<obj []>()
            use conn = this.GetSqlConnection
            conn.Open()
            let cmd = new MySqlCommand((sprintf "SELECT * FROM `%s` WHERE %s" table def), conn)
            use reader = cmd.ExecuteReader()
            let fields = reader.FieldCount
            while reader.Read() do
                let mutable internArr = 
                    [|for i = 0 to fields - 1 do
                        yield reader.GetValue(i)|]
                ret.Add(internArr)
            ret :> seq<obj []>            
        member this.Insert table props data =
            try
                use conn = this.GetSqlConnection
                conn.Open() //"INSERT INTO table (A, B) VALUES (a, b);"
                let query = sprintf "INSERT INTO `%s` %s VALUES %s;" table props data
                printfn "Insert command: \"%s\"" query
                let cmd = new MySqlCommand(query, conn)
                use reader = cmd.ExecuteReader()
                let mutable ret = "*"
                while reader.Read() do 
                    ret <- ret + "*"
                conn.Close()
                Logs.add_Log query
                ret
            with
                | ex -> raise ex
        member this.Update table data keygen =
            try
                use conn = this.GetSqlConnection
                conn.Open() //"INSERT INTO table (A, B) VALUES (a, b);"
                let query = sprintf "UPDATE `%s` SET %s WHERE %s;" table data keygen
                let cmd = new MySqlCommand(query, conn)
                use reader = cmd.ExecuteReader()
                let mutable ret = ""
                while reader.Read() do 
                    ret <- ret + "*"
                conn.Close()
                Logs.add_Log query
                ret
            with
                | ex -> ex.Message
        member this.GetPK entity =
            try
                use conn = this.GetSqlConnection
                conn.Open()
                let query = sprintf "SELECT max(%s) FROM %s;" (entity.GetProperties().[0]).Name entity.Name
                let cmd = new MySqlCommand(query, conn)
                use reader = cmd.ExecuteReader()
                let mutable ret = 0
                while reader.Read() do 
                    ret <- reader.GetInt32(0) + 1
                conn.Close()
                ret
            with
                | ex -> raise ex
    interface IMyDBContext with
        member this.Remove entity =
            let ct = this :> IBaseSQLCommands
            let query, logs = QueryBuilder.BuildDeleteQuery entity
            ct.Execute query logs
        member this.GetPeoples = 
            let ct = this :> IBaseSQLCommands
            ct.Get (new Starikov.dbModels.Person())
        member this.GetStudents =
            let ct = this :> IBaseSQLCommands
            //let ret = new List<Starikov.dbModels.Person>()
            seq { for v in ct.Get "student" do yield (Commands.Setter (new Starikov.dbModels.Student()) v) }
        member this.GetGroups =
            let ct = this :> IBaseSQLCommands
            seq { for v in ct.Get "group" do yield (Commands.Setter (new Starikov.dbModels.Group()) v) }
            //ct.GetFromType <| typeof<Group> |> Option.get
        member this.GetOneGroup id_group =
            let ct = this :> IBaseSQLCommands
            ct.GetWhere "group" (sprintf "(id_group='%d')" id_group) |> Seq.tryHead |> Option.map (Commands.Setter (new Starikov.dbModels.Group()))
        member this.GetGroupStudents id_group =
            let ct = this :> IBaseSQLCommands
            seq { for v in ct.GetWhere "student" (sprintf "(id_group='%d')" id_group) do yield (Commands.Setter (new Starikov.dbModels.Student()) v) }
        member this.Log_People (target: LoginViewModel) =
            let L = target.Login.Split(' ')
            let context = this :> IBaseSQLCommands
            let People = 
                let qr = context.GetWhere "people" (sprintf "(fam='%s' AND name='%s' AND otchestvo='%s')" L.[0] L.[1] L.[2])
                if (Seq.length <| qr) >= 1 then (Seq.head <| qr) |> Some else printfn "Incorrect Login: {%s}!" target.Login; None
            People |> Option.map ( fun a -> Commands.Setter (new Person()) a )
        member this.LogInForStudent (target: LoginViewModel) =
            let People = (this :> IMyDBContext).Log_People target
            if People.IsSome then
                let qr = (this :> IBaseSQLCommands).GetWhere "student" (sprintf "(id_man='%O')" People.Value.id_man)
                let Student = qr |> Seq.head |> Commands.Setter (new Student())
                printfn "Student with Login{\"%s\"} was founded... His id is %d" target.Login Student.id_man
                if Student.number_zach = target.Identity then
                    let acc = new Account(IsStudent = true)
                    acc.Person <- People.Value
                    acc.Student <- Student
                    acc |> Some
                else None
            else None
        member this.LogInForCurator (target: LoginViewModel) =
            let People = (this :> IMyDBContext).Log_People target
            if People.IsSome then
                if target.Identity = "Cur" then
                    (new Account(Person = People.Value)) |> Some
                else None
            else None
        member this.GetAccount (man_id) =
            let ct = this :> IBaseSQLCommands
            let person = (ct.GetWhere "people" (sprintf "(id_man='%s')" man_id)) |> Seq.head |> Commands.Setter (new Person())
            let student = (ct.GetWhere "student" (sprintf "(id_man='%s')" man_id)) |> Seq.tryHead |> Option.map (Commands.Setter (new Student()))
            let retAcc = new Account(Person = person)
            if student.IsSome then
                retAcc.IsStudent <- true
                retAcc.Student <- student.Value
            retAcc
        member this.GetEventsInfos =
            let ct = this :> IBaseSQLCommands
            ct.Get (new Starikov.dbModels.EventInfo())
        member this.InsertEventInfo einfo =
            let ct = this :> IBaseSQLCommands
            let mutable fake = new DateTime()
            let names, values = Commands.Getter <| einfo |> Array.map (fun (n, v) -> (("`" + n + "`"), ( if DateTime.TryParse((v.ToString()), &fake) then sprintf "'%s'" (Commands.ConvertDate <| v.ToString()) else sprintf "'%O'" v)) ) |> Array.unzip
            let tableName = sprintf "%s" (((einfo.GetType()).Name).ToLower())
            let fNames = names.[1..] |> Array.fold (sprintf "%s, %s") "" |> Seq.tail |> Seq.fold (sprintf "%s%c") ""
            let fValues = values.[1..] |> Array.fold (sprintf "%s, %s") "" |> Seq.tail |> Seq.fold (sprintf "%s%c") "" |> sprintf "(%s)"
            try
                let res = ct.Insert tableName (sprintf "(%s)" fNames) fValues //((sprintf "( '%d" (ct.GetPK <| einfo.GetType())) + fValues.[4..] )
                printfn "Result: %s" res
                true
            with
                | _ -> false
        member this.GetAnceteData man_id =
            let ct = this :> IBaseSQLCommands
            let NC target = if target = null then "" else target
            let abPerson = ct.GetWhere "people" (sprintf "(id_man='%s')" man_id) |> Seq.head |> Commands.Setter (new Person())
            let abStudent = ct.GetWhere "student" (sprintf "(id_man='%s')" man_id) |> Seq.head |> Commands.Setter (new Student())
            let abGroup = if abStudent.id_group.HasValue then (ct.GetWhere "group" (sprintf "(id_group='%d')" abStudent.id_group.Value) |> Seq.head |> Commands.Setter (new dbModels.Group())) else (new dbModels.Group()) 
            let ret = //new Anceta(lastname = abPerson.fam, name = abPerson.name, patron = abPerson.otchestvo, group = abGroup.name_group, birthdate = DateTime.Parse(abPerson.data_rojdeniya), grajdanstvo = abPerson.nacionalnost, voinskii_uchet = abPerson.nomer_vb, education = abPerson.chto_zakonchil, family_status = abPerson.semeinoe_polojenie, (*Childrens = ,*) pasport_serial = abPerson.serial_pasport, pasport_number = abPerson.number_pasport, pasport_getter = (sprintf "%s %s" abPerson.data_vidachi_pasporta abPerson.kem_vidan), (*pasport_code = ,*) inn = abPerson.INN, PFRF = abPerson.sv_vo_PFR, pIndex = abPerson.index_1, pRegion = abPerson.region_1, pCity = abPerson.gorod_1, (*pDistrict = ,*) pStreet = abPerson.ulica_1, pHome = abPerson.dom_1, pRoom = abPerson.kv_1, fIndex = abPerson.index_2, fRegion = abPerson.region_2, fCity = abPerson.gorod_2, (*fDistrict = ,*) fStreet = abPerson.ulica_2, fHome = abPerson.dom_2, fRoom = abPerson.kv_2, d_tel = abPerson.telefon_dom, m_tel = abPerson.telefon_sot, (*alter_lang = ,*) (*Bonuses = ,*) (*educationType = ,*) dealNumber = abStudent.number_kontrakta (*, dealStartDate = ,*) (*whoPay = ,*) (*pastSport = ,*) (*presantSport = ,*) (*futureSport = ,*) (*motherContact = ,*) (*fatherContact = ,*) )
                let a = new Anceta(lastname = abPerson.fam, name = abPerson.name, patron = abPerson.otchestvo)
                a.group <- NC abGroup.name_group
                a.birthdate <- NC abPerson.data_rojdeniya
                a.grajdanstvo <- NC abPerson.nacionalnost
                a.voinskii_uchet <- NC abPerson.nomer_vb
                a.education <- abPerson.chto_zakonchil
                a.family_status <- NC abPerson.semeinoe_polojenie
                (*a.Childrens <- NC ,*)
                a.pasport_serial <- NC abPerson.serial_pasport
                a.pasport_number <- NC abPerson.number_pasport
                a.pasport_date <- NC abPerson.data_vidachi_pasporta
                a.pasport_getter <- NC abPerson.kem_vidan
                (*a.pasport_code <- NC ,*)
                a.inn <- NC abPerson.INN
                a.PFRF <- NC abPerson.sv_vo_PFR
                a.pIndex <- NC abPerson.index_1
                a.pRegion <- NC abPerson.region_1
                a.pCity <- NC abPerson.gorod_1
                (*a.pDistrict <- NC ,*)
                a.pStreet <- NC abPerson.ulica_1
                a.pHome <- NC abPerson.dom_1
                a.pRoom <- NC abPerson.kv_1
                a.fIndex <- NC abPerson.index_2
                a.fRegion <- NC abPerson.region_2
                a.fCity <- NC abPerson.gorod_2
                (*a.fDistrict <- NC ,*)
                a.fStreet <- NC abPerson.ulica_2
                a.fHome <- NC abPerson.dom_2
                a.fRoom <- NC abPerson.kv_2
                a.d_tel <- NC abPerson.telefon_dom
                a.m_tel <- NC abPerson.telefon_sot
                (*a.alter_lang <- NC ,*)
                (*a.Bonuses <- NC ,*)
                (*a.educationType <- NC ,*)
                a.dealNumber <- NC abStudent.number_kontrakta
                (*, a.dealStartDate <- NC ,*) (*a.whoPay <- NC ,*) (*a.pastSport <- NC ,*) (*a.presantSport <- NC ,*) (*a.futureSport <- NC ,*) (*a.motherContact <- NC ,*) (*a.fatherContact <- NC ,*)
                a
            ret
        member this.SetAnceteData man_id a =
            let ct = this :> IBaseSQLCommands
            let abP = ct.GetWhere "people" (sprintf "(id_man='%s')" man_id) |> Seq.head |> Commands.Setter (new Person())
            let abS = ct.GetWhere "student" (sprintf "(id_man='%s')" man_id) |> Seq.head |> Commands.Setter (new Student())
            let abG = if abS.id_group.HasValue then (ct.GetWhere "group" (sprintf "(id_group='%d')" abS.id_group.Value) |> Seq.head |> Commands.Setter (new dbModels.Group())) else (new dbModels.Group()) 
            let mct = this :> IMyDBContext
            let gnames = mct.GetGroups |> Seq.map (fun g -> g.name_group) |> Seq.toList
            let da = mct.GetAnceteData man_id
            let check = Commands.SC man_id

            if check (a.lastname, da.lastname) then abP.fam <- a.lastname
            if check (a.name, da.name) then abP.name <- a.name
            if check (a.patron, da.patron) then abP.otchestvo <- a.patron
            if check (a.group, da.group) then abS.id_group <- a.group |> (fun gname ->
                                                                            let cname = Checker.Choose gname gnames
                                                                            let f = mct.GetGroups |> Seq.filter (fun t -> t.name_group = cname)
                                                                            let ret = f |> Seq.tryHead |> Option.map (fun h -> h.id_group)
                                                                            if ret.IsSome then (new System.Nullable<int>(ret.Value)) else abS.id_group
                                                                         )
            if check (a.birthdate, da.birthdate) then abP.data_rojdeniya <- a.birthdate
            if check (a.grajdanstvo, da.grajdanstvo) then abP.nacionalnost <- a.grajdanstvo
            if check (a.voinskii_uchet, da.voinskii_uchet) then abP.nomer_vb <- a.voinskii_uchet
            if check (a.education, da.education) then abP.chto_zakonchil <- a.education
            if check (a.family_status, da.family_status) then abP.semeinoe_polojenie <- a.family_status
            //if check (a.Childrens, da.Childrens) then abP. <- a.Childrens
            if check (a.pasport_serial, da.pasport_serial) then abP.serial_pasport <- a.pasport_serial
            if check (a.pasport_number, da.pasport_number) then abP.number_pasport <- a.pasport_number
            if check (a.pasport_date, da.pasport_date) then abP.data_vidachi_pasporta <- a.pasport_date
            if check (a.pasport_getter, da.pasport_getter) then abP.kem_vidan <- a.pasport_getter
            //if check (a.pasport_code, da.pasport_code) then abP. <- a.pasport_code
            if check (a.inn, da.inn) then abP.INN <- a.inn
            if check (a.PFRF, da.PFRF) then abP.sv_vo_PFR <- a.PFRF
            if check (a.pIndex, da.pIndex) then abP.index_1 <- a.pIndex
            if check (a.pRegion, da.pRegion) then abP.region_1 <- a.pRegion
            if check (a.pCity, da.pCity) then abP.gorod_1 <- a.pCity
            //if check (a.pDistrict, da.pDistrict) then abP. <- a.pDistrict
            if check (a.pStreet, da.pStreet) then abP.ulica_1 <- a.pStreet
            if check (a.pHome, da.pHome) then abP.dom_1 <- a.pHome
            if check (a.pRoom, da.pRoom) then abP.kv_1 <- a.pRoom
            if check (a.fIndex, da.fIndex) then abP.index_2 <- a.fIndex
            if check (a.fRegion, da.fRegion) then abP.region_2 <- a.fRegion
            if check (a.fCity, da.fCity) then abP.gorod_2 <- a.fCity
            //if check (a.fDistrict, da.fDistrict) then abP. <- a.fDistrict
            if check (a.fStreet, da.fStreet) then abP.ulica_2 <- a.fStreet
            if check (a.fHome, da.fHome) then abP.dom_2 <- a.fHome
            if check (a.fRoom, da.fRoom) then abP.kv_2 <- a.fRoom
            if check (a.d_tel, da.d_tel) then abP.telefon_dom <- a.d_tel
            if check (a.m_tel, da.m_tel) then abP.telefon_sot <- a.m_tel
            //if check (a.alter_lang, da.alter_lang) then abP. <- a.alter_lang
            //if check (a.Bonuses, da.Bonuses) then abP. <- a.Bonuses
            //if check (a.educationType, da.educationType) then abP. <- a.educationType
            //if check (a.dealNumber, da.dealNumber) then abP. <- a.dealNumber
            //if check (a.dealStartDate, da.dealStartDate) then abP. <- a.dealStartDate
            //if check (a.whoPay, da.whoPay) then abP. <- a.whoPay
            //if check (a.pastSport, da.pastSport) then abP. <- a.pastSport
            //if check (a.presantSport, da.presantSport) then abP. <- a.presantSport
            //if check (a.futureSport, da.futureSport) then abP. <- a.futureSport
            //if check (a.motherContact, da.motherContact) then abP. <- a.motherContact
            //if check (a.fatherContact, da.fatherContact) then abP. <- a.fatherContact
            //Commands.SC man_id (anceta.birthdate.ToString(), defaultAnceta.birthdate.ToString(), &abPerson.data_rojdeniya)
            
            true