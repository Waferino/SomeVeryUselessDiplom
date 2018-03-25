namespace Starikov
open System.Dynamic
type AboutUserModel () =
    member val Surname : string = null with get, set
    member val Name : string = null with get, set
    member val Patron : string = null with get, set
    member val INN : string = null with get, set
    member val SPasp : string = null with get, set
    member val NPasp : string = null with get, set
    member val WhoGiveP : string = null with get, set
    member val BirthDate : string = null with get, set
    member val BirthPlace : string = null with get, set
    member val Strana : string = null with get, set
    member val Region : string = null with get, set
    member val Index : string = null with get, set
    member val City : string = null with get, set
type MessageToGroupModel () =
    member val id_group = Unchecked.defaultof<int> with get, set
    member val message_subject = Unchecked.defaultof<string> with get, set
    member val message_body = Unchecked.defaultof<string> with get, set
type MessageToStudentModel () =
    member val id_student = Unchecked.defaultof<int> with get, set
    member val message_subject = Unchecked.defaultof<string> with get, set
    member val message_body = Unchecked.defaultof<string> with get, set