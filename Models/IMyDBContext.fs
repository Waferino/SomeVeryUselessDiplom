namespace Starikov

open System
open Starikov.dbModels
type IBaseSQLCommands =
    abstract member Get : string -> seq<obj []>
    abstract member GetWhere : string -> string -> seq<obj []>
    abstract member Insert : string -> string -> string
    abstract member Update : string -> string -> string -> string

type IMyDBContext =
    abstract member GetPeoples : System.Collections.Generic.List<Person>
    abstract member GetStudents : seq<Student>
    abstract member GetGroups : seq<Group>
    abstract member GetGroupStudents : int -> seq<Student>
    abstract member Log_People : LoginViewModel -> Person option
    abstract member LogInForStudent : LoginViewModel -> Account option
    abstract member LogInForCurator : LoginViewModel -> Account option 
    abstract member GetAccount : string -> Account