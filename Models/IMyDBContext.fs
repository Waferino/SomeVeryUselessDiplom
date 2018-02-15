namespace Starikov

open System
open Starikov.dbModels
type IBaseSQLCommands =
    abstract member Get : string -> seq<obj []>
    abstract member GetFromType : System.Type -> seq<'T> option
    abstract member GetWhere : string -> string -> seq<obj []>
    abstract member Insert : string -> string -> string
    abstract member Update : string -> string -> string -> string
    abstract member GetPK : System.Type -> int

type IMyDBContext =
    abstract member GetPeoples : System.Collections.Generic.List<Person>
    abstract member GetStudents : seq<Student>
    abstract member GetGroups : seq<Group>
    abstract member GetOneGroup : int -> Group option
    abstract member GetGroupStudents : int -> seq<Student>
    abstract member Log_People : LoginViewModel -> Person option
    abstract member LogInForStudent : LoginViewModel -> Account option
    abstract member LogInForCurator : LoginViewModel -> Account option 
    abstract member GetAccount : string -> Account
    abstract member GetEventsInfos : seq<EventInfo>
    abstract member InsertEventInfo : EventInfo -> bool