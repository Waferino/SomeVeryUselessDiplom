namespace Starikov

open System
open System.Collections.Generic
open MySql.Data.MySqlClient

open Starikov.dbModels

type CafedraDBContext() =
    member val ConnectionString = @"server=localhost;userid=root;password=kagura;persistsecurityinfo=True;database=www0005_base" with get, set
    member private __.GetSqlConnection = new MySqlConnection(__.ConnectionString)
    interface IBaseSQLCommands with
        member this.Get table = 
            let ret = new List<obj []>()
            use conn = this.GetSqlConnection
            conn.Open()
            let cmd = new MySqlCommand((sprintf "SELECT * FROM %s" table), conn)
            use reader = cmd.ExecuteReader()
            let fields = reader.FieldCount
            while reader.Read() do
                //ret.Add(reader.GetString("fam"))
                let mutable internArr = 
                    [|for i = 0 to fields - 1 do
                        yield reader.GetValue(i)|]
                ret.Add(internArr)
            ret :> seq<obj []>
        member this.GetWhere table def =
            let ret = new List<obj []>()
            use conn = this.GetSqlConnection
            conn.Open()
            let cmd = new MySqlCommand((sprintf "SELECT * FROM %s WHERE %s" table def), conn)
            use reader = cmd.ExecuteReader()
            let fields = reader.FieldCount
            while reader.Read() do
                let mutable internArr = 
                    [|for i = 0 to fields - 1 do
                        yield reader.GetValue(i)|]
                ret.Add(internArr)
            ret :> seq<obj []>            
        member this.Insert table data =
            try
                use conn = this.GetSqlConnection
                conn.Open() //"INSERT INTO table (A, B) VALUES (a, b);"
                let query = sprintf "INSERT INTO %s VALUES %s;" table data
                let cmd = new MySqlCommand(query, conn)
                use reader = cmd.ExecuteReader()
                let mutable ret = ""
                while reader.Read() do 
                    ret <- ret + "*"
                conn.Close()
                ret
            with
                | :? System.Exception as ex -> ex.Message
        member this.Update table data keygen =
            try
                use conn = this.GetSqlConnection
                conn.Open() //"INSERT INTO table (A, B) VALUES (a, b);"
                let query = sprintf "UPDATE %s SET %s WHERE %s;" table data keygen
                let cmd = new MySqlCommand(query, conn)
                use reader = cmd.ExecuteReader()
                let mutable ret = ""
                while reader.Read() do 
                    ret <- ret + "*"
                conn.Close()
                ret
            with
                | :? System.Exception as ex -> ex.Message
    interface IMyDBContext with
        member this.GetPeoples = 
            let ct = this :> IBaseSQLCommands
            let ret = new List<Starikov.dbModels.Person>()
            for v in ct.Get "people" do
                ret.Add(Commands.Setter (new Starikov.dbModels.Person()) v)
            ret
        member this.GetStudents =
            let ct = this :> IBaseSQLCommands
            //let ret = new List<Starikov.dbModels.Person>()
            seq { for v in ct.Get "student" do yield (Commands.Setter (new Starikov.dbModels.Student()) v) }
        member this.GetGroups =
            let ct = this :> IBaseSQLCommands
            seq { for v in ct.Get "`group`" do yield (Commands.Setter (new Starikov.dbModels.Group()) v) }
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